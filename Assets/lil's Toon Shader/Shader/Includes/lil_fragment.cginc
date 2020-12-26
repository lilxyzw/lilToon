// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------------------
// fragment shader
float4 frag(g2f i) : SV_TARGET
{
    UNITY_BRANCH
    if(_Invisible)
    {
        //----------------------------------------------------------------------------------------------------------------------
        // 非表示
        discard;
        return 0.0;
    } else {
        //----------------------------------------------------------------------------------------------------------------------
        // 裏面の法線を反転
        i.normalDir = i.facing < (_FlipNormal-1) ? -i.normalDir : i.normalDir;

        //----------------------------------------------------------------------------------------------------------------------
        // 初期化用
        float4 col = 1.0;
        float4 colbuf = 1.0;
        #ifndef LIL_FOR_ADD
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz + float3(0.0, +0.001, 0.0));
        #else
            float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.worldPos.xyz, _WorldSpaceLightPos0.w));
        #endif
        float3 viewDirection = normalize(UnityWorldSpaceViewDir(i.worldPos.xyz));

        #ifdef LIL_FULL
            float2 uvs[5];
            {
                uvs[0] = i.uv0.xy;
                uvs[1] = i.uv0.zw;
                uvs[2] = i.uv1.xy;
                uvs[3] = i.uv1.zw;
                uvs[4] = float2(lilAtan2(viewDirection.x,viewDirection.z)*UNITY_INV_PI, 0.5-lilAsin(viewDirection.y)*UNITY_INV_PI);
            }
        #endif

        /*
        // 視差マップでごまかす
        #ifdef LIL_FUR
            float3 pomViewDir = viewDirection;
            //float3 pomViewDir =  normalize(viewDirection.x * float3(i.tangentDir.x,i.bitangentDir.x,i.normalDir.x) + 
            //                               viewDirection.y * float3(i.tangentDir.y,i.bitangentDir.y,i.normalDir.y) + 
            //                               viewDirection.z * float3(i.tangentDir.z,i.bitangentDir.z,i.normalDir.z));
            float pomDepth = _FurLength * 0.1 * pomViewDir.y;
            float2 pomUV = i.uv;
            float3 rayPos = i.worldPos;
            float rayHeight = 0.0;
            float objHeight = getMaskLite_V(_FurNoiseMask, i.uv*_FurNoiseMask_ST.xy+_FurNoiseMask_ST.zw, sampler_linear_repeat) * pomDepth - pomDepth;
            float3 rayStep = pomViewDir * (pomDepth / pomViewDir.y) / 32.0;
            for (int pomi=0;pomi<32&&objHeight<rayHeight;pomi++)
            {
                rayPos -= rayStep;
                pomUV = i.uv + rayPos.xz - i.worldPos.xz;
                float furAlpha = getMaskLite_V(_FurNoiseMask, pomUV*_FurNoiseMask_ST.xy+_FurNoiseMask_ST.zw, sampler_linear_repeat);
                objHeight = furAlpha * pomDepth - pomDepth;
                rayHeight = rayPos.y-i.worldPos.y;
            }
            float2 nextObjPoint = pomUV;
            float2 prevObjPoint = pomUV + rayStep.xz;
            float nextHeight = objHeight;
            float prevHeight = getMaskLite_V(_FurNoiseMask, prevObjPoint*_FurNoiseMask_ST.xy+_FurNoiseMask_ST.zw, sampler_linear_repeat) * pomDepth - pomDepth;
            nextHeight -= rayHeight;
            prevHeight -= rayHeight + rayStep.y;

            float weight = nextHeight / (nextHeight - prevHeight);
            pomUV = lerp(nextObjPoint, prevObjPoint, weight);
            i.uv = pomUV;
        #endif
        */

        //----------------------------------------------------------------------------------------------------------------------
        // Normal Map (ノーマルマップ)
        float3 normalmap = float3(0.0,0.0,1.0);
        #ifdef LIL_FULL
            // NormalMap
            UNITY_BRANCH
            if(_UseBumpMap)
            {
                normalmap = UnpackScaleNormal(GET_TEX(_BumpMap), GET_MASK(_BumpScaleMask) * _BumpScale);
            }
            // NormalMap2nd
            UNITY_BRANCH
            if(_UseBump2ndMap)
            {
                normalmap = mixNormal(normalmap, UnpackScaleNormal(GET_TEX(_Bump2ndMap), GET_MASK(_Bump2ndScaleMask) * _Bump2ndScale));
            }
            /* ノーマルマップ追加分
            // NormalMap3rd
            UNITY_BRANCH
            if(_UseBump3rdMap)
            {
                normalmap = mixNormal(normalmap, UnpackScaleNormal(GET_TEX(_Bump3rdMap), GET_MASK(_Bump3rdScaleMask) * _Bump3rdScale));
            }
            // NormalMap4th
            UNITY_BRANCH
            if(_UseBump4thMap)
            {
                normalmap = mixNormal(normalmap, UnpackScaleNormal(GET_TEX(_Bump4thMap), GET_MASK(_Bump4thScaleMask) * _Bump4thScale));
            }
            */
        #else
            // NormalMap
            UNITY_BRANCH
            if(_UseBumpMap)
            {
                normalmap = UnpackScaleNormal(_BumpMap.Sample(sampler_linear_repeat,i.uv*_BumpMap_ST.xy+_BumpMap_ST.zw), _BumpScale);
            }
            // NormalMap2nd
            UNITY_BRANCH
            if(_UseBump2ndMap)
            {
                normalmap = mixNormal(normalmap, UnpackScaleNormal(_Bump2ndMap.Sample(sampler_linear_repeat,i.uv*_Bump2ndMap_ST.xy+_Bump2ndMap_ST.zw), _Bump2ndScale));
            }
        #endif
        float3 normalDirection = normalize(normalmap.x * i.tangentDir + normalmap.y * i.bitangentDir + normalmap.z * i.normalDir);

        float nv = dot(normalDirection, viewDirection);
        float nvabs = abs(nv);
            nv = saturate(nv);

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting (光の当たり方と影の強さ)
        #if !defined(SHADOWS_SCREEN) && !defined(LIL_FOR_ADD) && LIL_RENDER == 2
            float attenuation = 1.0;
        #else
            UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);
        #endif

        #ifndef LIL_FOR_ADD
            // 球面調和
            float3 ShadeSH9Def = ShadeSH9(float4(normalDirection,1.0)) * 0.78;
            float ShadeSH9DefL = lilLuminance(ShadeSH9Def);
            #ifdef LIL_FULL
                float defaultShadingBlend = 0.0;
                UNITY_BRANCH
                if(_UseDefaultShading) defaultShadingBlend = GET_MASK(_DefaultShadingBlendMask) * _DefaultShadingBlend;
            #endif
        #endif

        // 影の処理は完全に感覚
        float shadowmix = 1.0;
        float directContribution = 1.0;
        float dc2nd = 1.0;
        // 加算パスは影付けない方が良さそう
        #ifndef LIL_FOR_ADD
            UNITY_BRANCH
            if(_UseShadow)
            {
                // 影
                directContribution = saturate(dot(lightDirection,normalDirection)*0.5+0.5);

                #ifndef LIL_FOR_ADD
                    // 環境光と比較して混ぜる
                    float lightDiff = saturate(ShadeSH9PlusL - lightColorL);
                    float lightDiffVert = saturate(lilLuminance(i.sh) - lightColorL - ShadeSH9PlusL);
                    float directContributionSH9 = saturate(ShadeSH9DefL/max(ShadeSH9AveL,0.001)-0.25);
                    float directContributionVert = saturate(dot(normalize(i.shNormal),normalDirection)*0.5+0.5);
                    directContribution = lerp(directContribution, directContributionSH9, lightDiff);
                    directContribution = lerp(directContribution, directContributionVert, lightDiffVert);
                    dc2nd = directContribution;
                    // 陰
                    lightDiff = lightDiff + lightDiffVert - lightDiff * lightDiffVert;
                    directContribution *= lerp(saturate(attenuation*5), 1.0, lightDiff);
                #else
                    dc2nd = directContribution;
                    // 陰
                    directContribution *= saturate(attenuation*5);
                #endif

                // トゥーン処理
                float shadowBlur = GET_MASK(_ShadowBlurMask) * _ShadowBlur;
                float shadowBorder = GET_MASK(_ShadowBorderMask) * _ShadowBorder * _ShadowBorder;
                float ShadowBorderMin = saturate(shadowBorder - shadowBlur*0.5);
                float ShadowBorderMax = saturate(shadowBorder + shadowBlur*0.5);
                directContribution = saturate((directContribution - ShadowBorderMin) / (ShadowBorderMax - ShadowBorderMin));

                // 背面を暗く
                directContribution = directContribution * (1 - (i.facing < 0) * _BackfaceForceShadow);

                // 影の濃さ (光が弱い場合は薄くする)
                shadowmix = directContribution;
                #ifdef UNITY_COLORSPACE_GAMMA
                    // Gamma色空間対応
                    _ShadowStrength = GammaToLinearSpace(_ShadowStrength);
                #endif
                #ifndef LIL_FOR_ADD
                    float shadowStrength = GET_MASK(_ShadowStrengthMask) * _ShadowStrength * saturate(lightColorL*1.25+ShadeSH9PlusL-ShadeSH9AveL);
                #else
                    #ifdef DIRECTIONAL
                        float shadowStrength = GET_MASK(_ShadowStrengthMask) * _ShadowStrength * saturate(lightColorL*1.25);
                    #else
                        float shadowStrength = GET_MASK(_ShadowStrengthMask) * _ShadowStrength * saturate(lightColorL*1.25*attenuation);
                    #endif
                #endif
                directContribution = 1.0 - mad(shadowStrength,-directContribution,shadowStrength);
            }
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Main Texture (メインの色)
        #ifdef LIL_FULL
            // Main
            colbuf = GET_TEX_SAMP(_MainTex) * _Color;
            colbuf.rgb = colorShifter(colbuf.rgb, _MainTexHue, _MainTexSaturation, _MainTexValue);
            col = colbuf;
            // Main2nd
            UNITY_BRANCH
            if(_UseMain2ndTex)
            {
                colbuf = GET_TEX_SAMP(_Main2ndTex) * _Color2nd;
                colbuf.rgb = colorShifter(colbuf.rgb, _Main2ndTexHue, _Main2ndTexSaturation, _Main2ndTexValue);
                col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_Main2ndBlendMask) * _Main2ndBlend * colbuf.a, _Main2ndTexMix);
            }
            // Main3rd
            UNITY_BRANCH
            if(_UseMain3rdTex)
            {
                colbuf = GET_TEX_SAMP(_Main3rdTex) * _Color3rd;
                colbuf.rgb = colorShifter(colbuf.rgb, _Main3rdTexHue, _Main3rdTexSaturation, _Main3rdTexValue);
                col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_Main3rdBlendMask) * _Main3rdBlend * colbuf.a, _Main3rdTexMix);
            }
            // Main4th
            UNITY_BRANCH
            if(_UseMain4thTex)
            {
                colbuf = GET_TEX_SAMP(_Main4thTex) * _Color4th;
                colbuf.rgb = colorShifter(colbuf.rgb, _Main4thTexHue, _Main4thTexSaturation, _Main4thTexValue);
                col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_Main4thBlendMask) * _Main4thBlend * colbuf.a, _Main4thTexMix);
            }
        #else
            // Main
            colbuf = _MainTex.Sample(sampler_MainTex,i.uv*_MainTex_ST.xy+_MainTex_ST.zw);
            colbuf = pow(colbuf,_MainTexTonecurve);
            colbuf *= _Color;
            colbuf.rgb = colorShifter(colbuf.rgb, _MainTexHue, _MainTexSaturation, _MainTexValue);
            col = colbuf;
            // Main2nd
            UNITY_BRANCH
            if(_UseMain2ndTex)
            {
                colbuf = GET_TEXSUB(_Main2ndTex) * _Color2nd;
                col.rgb = mixSubCol(col.rgb, colbuf.rgb, colbuf.a, _Main2ndTexMix);
            }
            // Main3rd
            UNITY_BRANCH
            if(_UseMain3rdTex)
            {
                colbuf = GET_TEXSUB(_Main3rdTex) * _Color3rd;
                col.rgb = mixSubCol(col.rgb, colbuf.rgb, colbuf.a, _Main3rdTexMix);
            }
            // Main4th
            UNITY_BRANCH
            if(_UseMain4thTex)
            {
                colbuf = GET_TEXSUB(_Main4thTex) * _Color4th;
                col.rgb = mixSubCol(col.rgb, colbuf.rgb, colbuf.a, _Main4thTexMix);
            }
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Vertex Color (頂点カラー)
        UNITY_BRANCH
        if(_UseVertexColor) col *= i.color;

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha Mask (アルファマスク)
        #if LIL_RENDER != 0
            #ifdef LIL_FULL
                // Alpha
                UNITY_BRANCH
                if(_UseAlphaMask) col.a = min(col.a, GET_MASK(_AlphaMask) * _Alpha);
                /* アルファマスク追加分
                // Alpha2nd
                UNITY_BRANCH
                if(_UseAlphaMask2nd) col.a = min(col.a, GET_MASK(_AlphaMask2nd) * _Alpha2nd);
                */
            #else
                // Alpha
                UNITY_BRANCH
                if(_UseAlphaMask) col.a = min(col.a, GET_MASK(_AlphaMask));
            #endif
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Outline Color (輪郭線の色)
        #ifdef LIL_FULL
            if(i.isOutline>0.5)
            {
                UNITY_BRANCH
                if(_OutlineMixMain)
                {
                    col.rgb = pow(col.rgb, _OutlineMixMainStrength);
                    col.rgb = colorShifterEx(col.rgb, _OutlineHue, _OutlineSaturation, _OutlineValue, _OutlineAutoHue);
                    col.rgb *= mad(lilLuminance(col.rgb), -saturate(_OutlineAutoValue), 1.0); // 輝度利用
                }
                UNITY_BRANCH
                if(_UseOutlineColor) col = GET_TEX(_OutlineColorTex) * _OutlineColor;
                clip(GET_MASK(_OutlineAlphaMask) * _OutlineAlpha-0.5);
            }
        #else
            if(i.isOutline>0.5)
            {
                UNITY_BRANCH
                if(_OutlineMixMain)
                {
                    col.rgb = pow(col.rgb, _OutlineMixMainStrength);
                    col.rgb = colorShifterEx(col.rgb, _OutlineHue, _OutlineSaturation, _OutlineValue, _OutlineAutoHue);
                    col.rgb *= mad(lilLuminance(col.rgb), -saturate(_OutlineAutoValue), 1.0); // 輝度利用
                }
                col *= _OutlineColor;
                clip(GET_MASK(_OutlineAlphaMask)-0.5);
            }
        #endif

        // コピー
        float3 bufMainTex = col.rgb;

        // 透過関係
        #if LIL_RENDER == 0
            col.a = 1;
        #elif LIL_RENDER == 1
            clip(col.a - _Cutoff);
            col.a = 1;
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            clip(col.a - 0.001);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Light Color (光の色を計算)
        float3 finalLight = 1.0;
        #ifndef LIL_FOR_ADD
            float3 directLighting = saturate(ShadeSH9Plus+lightColor);
            #ifdef LIL_FULL
                UNITY_BRANCH
                if(_UseDefaultShading) directLighting = lerp(directLighting, ShadeSH9Def+lightColor, defaultShadingBlend);
            #endif

            UNITY_BRANCH
            if(_UseShadow)
            {
                float3 indirectLighting = col.rgb;
                if(_UseShadowMixMainColor)
                {
                    indirectLighting *= pow(col.rgb, _ShadowMixMainColor + 0.0000000001);
                }
                indirectLighting = colorShifter(indirectLighting, _ShadowHue, _ShadowSaturation, _ShadowValue);
                #ifdef LIL_FULL
                    UNITY_BRANCH
                    if(_UseDefaultShading) indirectLighting *= lerp(saturate(ShadeSH9Ave), ShadeSH9Def, defaultShadingBlend);
                    else                   indirectLighting *= saturate(ShadeSH9Ave);
                #else
                    indirectLighting *= saturate(ShadeSH9Ave);
                #endif
                UNITY_BRANCH
                if(_UseShadowColor)
                {
                    colbuf = GET_TEX(_ShadowColorTex) * _ShadowColor;
                    colbuf.rgb *= 1.0 - _ShadowColorFromMain + col.rgb * _ShadowColorFromMain;
                    indirectLighting = mixSubCol(indirectLighting, colbuf.rgb, colbuf.a, _ShadowColorMix);
                }
                indirectLighting = lerp(indirectLighting, max(_ShadowGradColor.rgb * directLighting * col.rgb, indirectLighting), shadowmix * _ShadowGrad); // グラデーション
                UNITY_BRANCH
                if(_UseShadow2nd)
                {
                    colbuf = GET_TEX(_Shadow2ndColorTex) * _Shadow2ndColor;
                    colbuf.rgb *= 1.0 - _Shadow2ndColorFromMain + col.rgb * _Shadow2ndColorFromMain;
                    // トゥーン処理
                    float shadow2ndBorder = _Shadow2ndBorder * _Shadow2ndBorder;
                    float Shadow2ndBorderMin = saturate(shadow2ndBorder - _Shadow2ndBlur*0.5);
                    float Shadow2ndBorderMax = saturate(shadow2ndBorder + _Shadow2ndBlur*0.5);
                    dc2nd = saturate((dc2nd - Shadow2ndBorderMin) / (Shadow2ndBorderMax - Shadow2ndBorderMin));
                    // 背面を暗く
                    dc2nd = dc2nd * (1.0 - (i.facing < 0) * _BackfaceForceShadow);
                    dc2nd = 1.0-dc2nd;
                    indirectLighting = mixSubCol(indirectLighting, colbuf.rgb, dc2nd*colbuf.a, _Shadow2ndColorMix);
                }
                // 合成
                finalLight = lerp(indirectLighting,directLighting,directContribution)+i.sh; // 後の合成に使用
                col.rgb = lerp(indirectLighting,directLighting*col.rgb,directContribution)+i.sh*col.rgb;
            } else {
                finalLight = directLighting+i.sh;
                col.rgb *= finalLight;
            }
        #else
            finalLight = saturate(_LightColor0.rgb * 0.85 * attenuation);
            // 加算パスは影付けない方が良さそう
            //if(_UseShadow) finalLight *= directContribution;
            col.rgb *= finalLight;
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // ここから加算系（アウトラインには不要な処理）
        if(i.isOutline<0.5)
        {
            //----------------------------------------------------------------------------------------------------------------------
            // Refraction (屈折)
            #if defined(LIL_REFRACTION)
                float2 scnUV = UNITY_PROJ_COORD(i.projPos).xy/i.projPos.w;
                float refRim = pow(1.0 - nv, _RefractionFresnelPower);
                float2 refractUV = scnUV + (refRim * _RefractionStrength) * mul(UNITY_MATRIX_V, float4(normalDirection,0)).xy;
                // 手前にあるオブジェクトを描画しない処理だけどこれはこれで違和感がすごい
                //float refrFix = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, refractUV));
                //if(LinearEyeDepth(refrFix) < i.projPos.w) refractUV = scnUV;
                colbuf.rgb = tex2D(_BackgroundTexture, refractUV).rgb * _RefractionColor.rgb;
                if(_RefractionColorFromMain) colbuf.rgb *= bufMainTex;
                col.rgb = lerp(colbuf.rgb, col.rgb, col.a);
            #endif

            //----------------------------------------------------------------------------------------------------------------------
            // Reflection (鏡面反射)
            UNITY_BRANCH
            if(_UseReflection)
            {
                colbuf.rgb = 0;
                // Smoothness
                float Smoothness = GET_MASK(_SmoothnessTex) * _Smoothness;
                float perceptualRoughness = SmoothnessToPerceptualRoughness(Smoothness);
                float Roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
                // Metallic
                float metallic = GET_MASK(_MetallicGlossMap) * _Metallic;
                col.rgb = mad(col.rgb,-metallic,col.rgb);
                float3 specular = lerp(unity_ColorSpaceDielectricSpec.rgb,bufMainTex,metallic);
                float oneMinusReflectivity = mad(unity_ColorSpaceDielectricSpec.a,-metallic,unity_ColorSpaceDielectricSpec.a);
                // Specular
                UNITY_BRANCH
                if(_ApplySpecular)
                {
                    float3 attenColor = attenuation * lightColor;
                    float3 halfDirection = normalize(viewDirection+lightDirection);
                    float nl = saturate(dot(normalDirection, lightDirection));
                    float nh = saturate(dot(normalDirection, halfDirection));
                    float lh = saturate(dot(lightDirection, halfDirection));
                    colbuf.rgb += getSpecular(nv, nl, nh, lh, Roughness, specular) * attenColor;
                }
                // Reflection
                #ifndef LIL_FOR_ADD
                    UNITY_BRANCH
                    if(_ApplyReflection)
                    {
                        float3 reflUVW = reflect(-viewDirection, normalDirection);
                        float grazingTerm = saturate(Smoothness + (1.0-oneMinusReflectivity));
                        #ifdef UNITY_COLORSPACE_GAMMA
                            float surfaceReduction = 1.0-0.28*Roughness*perceptualRoughness;
                        #else
                            float surfaceReduction = 1.0 / (Roughness*Roughness + 1.0);
                        #endif
                        float3 rt = 0.0;
                        #ifdef LIL_FULL
                            UNITY_BRANCH
                            if(_ReflectionUseCubemap)
                            {
                                rt = getRefTex(_ReflectionCubemap, _ReflectionCubemap_HDR, reflUVW, perceptualRoughness);
                            } else {
                        #endif
                                float4 probeHDR[2] = {float4(0.0,0.0,0.0,0.0),float4(0.0,0.0,0.0,0.0)};
                                float4 boxMax[2] = {float4(0.0,0.0,0.0,0.0),float4(0.0,0.0,0.0,0.0)};
                                float4 boxMin[2] = {float4(0.0,0.0,0.0,0.0),float4(0.0,0.0,0.0,0.0)};
                                float4 probePosition[2] = {float4(0.0,0.0,0.0,0.0),float4(0.0,0.0,0.0,0.0)};
                                probeHDR[0] = unity_SpecCube0_HDR;
                                probeHDR[1] = unity_SpecCube1_HDR;
                                #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
                                    boxMin[0] = unity_SpecCube0_BoxMin;
                                #endif
                                #ifdef UNITY_SPECCUBE_BOX_PROJECTION
                                    boxMax[0] = unity_SpecCube0_BoxMax;
                                    probePosition[0] = unity_SpecCube0_ProbePosition;
                                    boxMax[1] = unity_SpecCube1_BoxMax;
                                    boxMin[1] = unity_SpecCube1_BoxMin;
                                    probePosition[1] = unity_SpecCube1_ProbePosition;
                                #endif
                                rt = reflectionTex(probeHDR, probePosition, boxMax, boxMin, i.worldPos, reflUVW, perceptualRoughness);
                        #ifdef LIL_FULL
                            }
                        #endif
                        rt *= (1 - _ReflectionShadeMix) + finalLight * _ReflectionShadeMix;
                        #ifdef LIL_REFRACTION
                            // 屈折率から適当に計算
                            //col.rgb = lerp(rt, col.rgb, col.a+nvabs-col.a*nvabs);
                            col.rgb = lerp(rt, col.rgb, col.a+(1-col.a)*pow(nvabs,_RefractionStrength*0.5+0.25));
                            colbuf.rgb += col.a * surfaceReduction * rt * FresnelLerp(specular, grazingTerm, nv);
                        #else
                            colbuf.rgb += surfaceReduction * rt * FresnelLerp(specular, grazingTerm, nv);
                        #endif
                    }
                #endif
                // Mix
                col.rgb += (GET_MASK(_ReflectionBlendMask) * _ReflectionBlend) * colbuf.rgb;
            }

            #ifndef LIL_FOR_ADD
                //----------------------------------------------------------------------------------------------------------------------
                // Matcap (マットキャップ)
                float3 normalDirectionMatcap = 0.0;
                float2 matUV = 0.0;
                float2x2 matRot = calcMatRot(i.isMirror);

                #ifdef LIL_FULL
                    // Matcap
                    UNITY_BRANCH
                    if(_UseMatcap)
                    {
                        normalDirectionMatcap = normalize(lerp(i.normalDir,normalDirection,_MatcapNormalMix));
                        matUV = ComputeTransformCap(normalDirectionMatcap);
                        matUV = uvRotate(matUV, matRot);
                        if(i.isMirror > 0.5) matUV.x = 1-matUV.x;
                        colbuf = getMatcapTex(matUV, _MatcapTex, _MatcapColor, _MatcapTex_ST);
                        colbuf.rgb *= 1 - _MatcapShadeMix + finalLight * _MatcapShadeMix;
                        col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_MatcapBlendMask) * _MatcapBlend * colbuf.a, _MatcapMix);
                    }
                    // Matcap2nd
                    UNITY_BRANCH
                    if(_UseMatcap2nd)
                    {
                        normalDirectionMatcap = normalize(lerp(i.normalDir,normalDirection,_Matcap2ndNormalMix));
                        matUV = ComputeTransformCap(normalDirectionMatcap);
                        if(i.isMirror > 0.5) matUV.x = 1-matUV.x;
                        colbuf = getMatcapTex(matUV, _Matcap2ndTex, _Matcap2ndColor, _Matcap2ndTex_ST);
                        colbuf.rgb *= 1 - _Matcap2ndShadeMix + finalLight * _Matcap2ndShadeMix;
                        col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_Matcap2ndBlendMask) * _Matcap2ndBlend * colbuf.a, _Matcap2ndMix);
                    }
                    /* マットキャップ追加分
                    // Matcap3rd
                    UNITY_BRANCH
                    if(_UseMatcap3rd)
                    {
                        normalDirectionMatcap = normalize(lerp(i.normalDir,normalDirection,_Matcap3rdNormalMix));
                        matUV = ComputeTransformCap(normalDirectionMatcap);
                        if(i.isMirror > 0.5) matUV.x = 1-matUV.x;
                        colbuf = getMatcapTex(matUV, _Matcap3rdTex, _Matcap3rdColor, _Matcap3rdTex_ST);
                        colbuf.rgb *= 1 - _Matcap3rdShadeMix + finalLight * _Matcap3rdShadeMix;
                        col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_Matcap3rdBlendMask) * _Matcap3rdBlend * colbuf.a, _Matcap3rdMix);
                    }
                    // Matcap4th
                    UNITY_BRANCH
                    if(_UseMatcap4th)
                    {
                        normalDirectionMatcap = normalize(lerp(i.normalDir,normalDirection,_Matcap4thNormalMix));
                        matUV = ComputeTransformCap(normalDirectionMatcap);
                        if(i.isMirror > 0.5) matUV.x = 1-matUV.x;
                        colbuf = getMatcapTex(matUV, _Matcap4thTex, _Matcap4thColor, _Matcap4thTex_ST);
                        colbuf.rgb *= 1 - _Matcap4thShadeMix + finalLight * _Matcap4thShadeMix;
                        col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_Matcap4thBlendMask) * _Matcap4thBlend * colbuf.a, _Matcap4thMix);
                    }
                    */
                #else
                    // Matcap
                    UNITY_BRANCH
                    if(_UseMatcap)
                    {
                        matUV = ComputeTransformCap(normalDirection);
                        matUV = uvRotate(matUV, matRot);
                        if(i.isMirror > 0.5) matUV.x = 1-matUV.x;
                        colbuf = UNITY_SAMPLE_TEX2D_SAMPLER(_MatcapTex, _linear_repeat, matUV) * _MatcapColor;
                        colbuf.rgb *= 1 - _MatcapShadeMix + finalLight * _MatcapShadeMix;
                        col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_MatcapBlendMask) * _MatcapBlend * colbuf.a, _MatcapMix);
                    }
                #endif

                //----------------------------------------------------------------------------------------------------------------------
                // Rim Light (リムライト)
                float rim = 0;
                #ifdef LIL_FULL
                    // Rim
                    if(_UseRim)
                    {
                        rim = pow(saturate(1.0 - nvabs + _RimUpperSideWidth), _RimFresnelPower);
                        colbuf = GET_TEX(_RimColorTex) * _RimColor;
                        if(_RimToon)
                        {
                            float RimBlur = GET_MASK(_RimBlurMask) * _RimBlur;
                            float RimBorder = GET_MASK(_RimBorderMask) * _RimBorder;
                            float RimBorderMin = saturate(RimBorder - RimBlur*0.5);
                            float RimBorderMax = saturate(RimBorder + RimBlur*0.5);
                            rim = 1.0 - (1.0 - saturate((rim - RimBorderMin) / (RimBorderMax - RimBorderMin)));
                        }
                        colbuf.rgb = lerp(colbuf.rgb, colbuf.rgb * finalLight, _RimShadeMix);
                        rim *= GET_MASK(_RimBlendMask) * _RimBlend * colbuf.a * (i.facing > 0.0 ? 1.0 : 0.0);
                        if(_RimShadowMask) rim *= shadowmix;
                        col.rgb = mad(colbuf.rgb, rim, col.rgb);
                    }

                    /* リムライト追加分
                    // Rim2nd
                    if(_UseRim2nd)
                    {
                        rim = pow(saturate(1.0 - nvabs + _Rim2ndUpperSideWidth), _Rim2ndFresnelPower);
                        colbuf = GET_TEX(_Rim2ndColorTex);
                        colbuf *= _Rim2ndColor;
                        if(_Rim2ndToon)
                        {
                            float Rim2ndBlur = GET_MASK(_Rim2ndBlurMask) * _Rim2ndBlur;
                            float Rim2ndBorder = GET_MASK(_Rim2ndBorderMask) * _Rim2ndBorder; // arktoonにざっくり合わせる
                            float Rim2ndBorderMin = saturate(Rim2ndBorder - Rim2ndBlur*0.5); // この場合saturateはコスト０でmaxより軽量です
                            float Rim2ndBorderMax = saturate(Rim2ndBorder + Rim2ndBlur*0.5); // この場合saturateはコスト０でminより軽量です
                            rim = 1.0 - (1.0 - saturate((rim - Rim2ndBorderMin) / (Rim2ndBorderMax - Rim2ndBorderMin)));
                        }
                        colbuf.rgb *= 1 - _Rim2ndShadeMix + finalLight * _Rim2ndShadeMix;
                        rim *= GET_MASK(_Rim2ndBlendMask) * _Rim2ndBlend * colbuf.a * (i.facing > 0.0 ? 1.0 : 0.0);
                        col.rgb = mixSubCol(col.rgb, colbuf.rgb, rim, _Rim2ndColorMix);
                    }
                    */
                #else
                    // Rim
                    if(_UseRim)
                    {
                        rim = pow(saturate(1.0 - nvabs + _RimUpperSideWidth), _RimFresnelPower);
                        if(_RimToon)
                        {
                            float RimBorderMin = saturate(_RimBorder - _RimBlur*0.5);
                            float RimBorderMax = saturate(_RimBorder + _RimBlur*0.5);
                            rim = 1.0 - (1.0 - saturate((rim - RimBorderMin) / (RimBorderMax - RimBorderMin)));
                        }
                        rim *= GET_MASK(_RimBlendMask) * _RimBlend * _RimColor.a * (i.facing > 0.0 ? 1.0 : 0.0);
                        if(_RimShadowMask) rim *= shadowmix;
                        col.rgb = col.rgb + rim * lerp(_RimColor.rgb, _RimColor.rgb * finalLight, _RimShadeMix);
                    }
                #endif

                //----------------------------------------------------------------------------------------------------------------------
                // Emission (発光)
                #ifdef LIL_FULL
                    // Emission
                    UNITY_BRANCH
                    if(_UseEmission)
                    {
                        float2 _EmissionMapParaTex = uvs[_EmissionMapUV] + _EmissionParallaxDepth * mul(float3x3(i.tangentDir, i.bitangentDir, i.normalDir), viewDirection).xy;
                        colbuf = GET_EMITEX(_EmissionMap) * _EmissionColor;
                        if(_EmissionUseGrad) colbuf *= getGradTex(_EmissionGradSpeed, _EmissionGradTex);
                        colbuf.rgb = colorShifter(colbuf.rgb, _EmissionHue, _EmissionSaturation, _EmissionValue);
                        col.rgb = mad(colbuf.rgb, GET_MASK(_EmissionBlendMask) * _EmissionBlend * _EmissionBlink * colbuf.a, col.rgb);
                    }
                    // Emission2nd
                    UNITY_BRANCH
                    if(_UseEmission2nd)
                    {
                        float2 _Emission2ndMapParaTex = uvs[_Emission2ndMapUV] + _Emission2ndParallaxDepth * mul(float3x3(i.tangentDir, i.bitangentDir, i.normalDir), viewDirection).xy;
                        colbuf = GET_EMITEX(_Emission2ndMap) * _Emission2ndColor;
                        if(_Emission2ndUseGrad) colbuf *= getGradTex(_Emission2ndGradSpeed, _Emission2ndGradTex);
                        colbuf.rgb = colorShifter(colbuf.rgb, _Emission2ndHue, _Emission2ndSaturation, _Emission2ndValue);
                        col.rgb = mad(colbuf.rgb, GET_MASK(_Emission2ndBlendMask) * _Emission2ndBlend * _Emission2ndBlink * colbuf.a, col.rgb);
                    }
                    // Emission3rd
                    UNITY_BRANCH
                    if(_UseEmission3rd)
                    {
                        float2 _Emission3rdMapParaTex = uvs[_Emission3rdMapUV] + _Emission3rdParallaxDepth * mul(float3x3(i.tangentDir, i.bitangentDir, i.normalDir), viewDirection).xy;
                        colbuf = GET_EMITEX(_Emission3rdMap) * _Emission3rdColor;
                        if(_Emission3rdUseGrad) colbuf *= getGradTex(_Emission3rdGradSpeed, _Emission3rdGradTex);
                        colbuf.rgb = colorShifter(colbuf.rgb, _Emission3rdHue, _Emission3rdSaturation, _Emission3rdValue);
                        col.rgb = mad(colbuf.rgb, GET_MASK(_Emission3rdBlendMask) * _Emission3rdBlend * _Emission3rdBlink * colbuf.a, col.rgb);
                    }
                    // Emission4th
                    UNITY_BRANCH
                    if(_UseEmission4th)
                    {
                        float2 _Emission4thMapParaTex = uvs[_Emission4thMapUV] + _Emission4thParallaxDepth * mul(float3x3(i.tangentDir, i.bitangentDir, i.normalDir), viewDirection).xy;
                        colbuf = GET_EMITEX(_Emission4thMap) * _Emission4thColor;
                        if(_Emission4thUseGrad) colbuf *= getGradTex(_Emission4thGradSpeed, _Emission4thGradTex);
                        colbuf.rgb = colorShifter(colbuf.rgb, _Emission4thHue, _Emission4thSaturation, _Emission4thValue);
                        col.rgb = mad(colbuf.rgb, GET_MASK(_Emission4thBlendMask) * _Emission4thBlend * _Emission4thBlink * colbuf.a, col.rgb);
                    }
                #else
                    // Emission
                    UNITY_BRANCH
                    if(_UseEmission)
                    {
                        float2 _EmissionMapParaTex = i.uv + _EmissionParallaxDepth * mul(float3x3(i.tangentDir, i.bitangentDir, i.normalDir), viewDirection).xy;
                        colbuf = GET_EMITEX(_EmissionMap) * _EmissionColor;
                        if(_EmissionUseGrad) colbuf *= getGradTex(_EmissionGradSpeed, _EmissionGradTex);
                        col.rgb = mad(colbuf.rgb, GET_EMIMASK(_EmissionBlendMask) * _EmissionBlend * _EmissionBlink * colbuf.a, col.rgb);
                    }
                    // Emission2nd
                    UNITY_BRANCH
                    if(_UseEmission2nd)
                    {
                        float2 _Emission2ndMapParaTex = i.uv + _Emission2ndParallaxDepth * mul(float3x3(i.tangentDir, i.bitangentDir, i.normalDir), viewDirection).xy;
                        colbuf = GET_EMITEX(_Emission2ndMap) * _Emission2ndColor;
                        if(_Emission2ndUseGrad) colbuf *= getGradTex(_Emission2ndGradSpeed, _Emission2ndGradTex);
                        col.rgb = mad(colbuf.rgb, GET_EMIMASK(_Emission2ndBlendMask) * _Emission2ndBlend * _Emission2ndBlink * colbuf.a, col.rgb);
                    }
                #endif
            #endif
        }

        #ifdef LIL_REFRACTION
            col.a = 1;
        #endif

        //-------------------------------------------------------------------------------------------------------------------------
        // フォグを適用して書き出し
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
    }
}
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
        // マスク
        clip(GET_MASK(_FurMask)-0.5);

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

        //----------------------------------------------------------------------------------------------------------------------
        // Normal Map (ノーマルマップ)
        float3 normalDirection = normalize(i.normalDir);

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting (光の当たり方と影の強さ)
        #if !defined(SHADOWS_SCREEN) && !defined(LIL_FOR_ADD) || defined(LIL_FUR)
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
        colbuf = _MainTex.Sample(sampler_MainTex,i.uv*_MainTex_ST.xy+_MainTex_ST.zw);
        colbuf = pow(colbuf,_MainTexTonecurve);
        colbuf *= _Color;
        colbuf.rgb = colorShifter(colbuf.rgb, _MainTexHue, _MainTexSaturation, _MainTexValue);
        col = colbuf;

        //----------------------------------------------------------------------------------------------------------------------
        // Fur
        float furalpha = getMaskLite(_FurNoiseMask, i.uv*_FurNoiseMask_ST.xy+_FurNoiseMask_ST.zw, sampler_linear_repeat);
        furalpha = saturate(furalpha-pow(i.furLayer,3));
        col.a=furalpha;
        //col.rgb *= saturate(i.furLayer) * _FurAO*2 + 1 - _FurAO;
        col.rgb *= (1-furalpha) * _FurAO * 1.25 + 1 - _FurAO;

        // コピー
        float3 bufMainTex = col.rgb;

        // 透過関係
        #if LIL_RENDER == 0
            col.a = 1;
        #elif  LIL_RENDER == 1
            clip(col.a - _Cutoff);
            col.a = 1;
        #else
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
        #ifndef LIL_FOR_ADD
            //----------------------------------------------------------------------------------------------------------------------
            // Emission (発光)
            // Emission
            UNITY_BRANCH
            if(_UseEmission)
            {
                float2 _EmissionMapParaTex = i.uv;
                colbuf = GET_EMITEX(_EmissionMap) * _EmissionColor;
                if(_EmissionUseGrad) colbuf *= getGradTex(_EmissionGradSpeed, _EmissionGradTex);
                col.rgb = mad(colbuf.rgb, GET_MASK(_EmissionBlendMask) * _EmissionBlend * _EmissionBlink * colbuf.a, col.rgb);
            }
            // Emission2nd
            UNITY_BRANCH
            if(_UseEmission2nd)
            {
                float2 _Emission2ndMapParaTex = i.uv;
                colbuf = GET_EMITEX(_Emission2ndMap) * _Emission2ndColor;
                if(_Emission2ndUseGrad) colbuf *= getGradTex(_Emission2ndGradSpeed, _Emission2ndGradTex);
                col.rgb = mad(colbuf.rgb, GET_MASK(_Emission2ndBlendMask) * _Emission2ndBlend * _Emission2ndBlink * colbuf.a, col.rgb);
            }
        #endif

        //-------------------------------------------------------------------------------------------------------------------------
        // フォグを適用して書き出し
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
    }
}
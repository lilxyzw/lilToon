#ifndef LIL_FRAGMENT_INCLUDED
#define LIL_FRAGMENT_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Fragment shader
float4 frag(v2f input, float facing : VFACE) : SV_Target
{
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
    LIL_GET_VERTEXLIGHT(input, vertexLightColor);
    LIL_GET_ADDITIONALLIGHT(input.positionWS, additionalLightColor);
    if(_AsUnlit)
    {
        #if !defined(LIL_PASS_FORWARDADD)
            lightColor = 1.0;
            vertexLightColor = 0.0;
            additionalLightColor = 0.0;
        #else
            lightColor = 0.0;
        #endif
    }

    //--------------------------------------------------------------------------------------------------------------------------
    // Apply Matelial & Lighting
    #if defined(LIL_OUTLINE)
        //--------------------------------------------------------------------------------------------------------------------------
        // UV
        #if defined(LIL_FEATURE_ANIMATE_OUTLINE_UV)
            float2 uvMain = lilCalcUV(input.uv, _OutlineTex_ST, _OutlineTex_ScrollRotate);
        #else
            float2 uvMain = lilCalcUV(input.uv, _OutlineTex_ST);
        #endif

        //--------------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = 1.0;
        if(Exists_OutlineTex)
        {
            col = LIL_SAMPLE_2D(_OutlineTex, sampler_OutlineTex, uvMain);
            #if defined(LIL_FEATURE_OUTLINE_TONE_CORRECTION)
                col.rgb = lilToneCorrection(col.rgb, _OutlineTexHSVG);
            #endif
        }
        col *= _OutlineColor;

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting
        if(_OutlineEnableLighting) col.rgb *= saturate(lightColor + vertexLightColor + additionalLightColor);
    #elif defined(LIL_FUR)
        //--------------------------------------------------------------------------------------------------------------------------
        // UV
        #if defined(LIL_FEATURE_ANIMATE_MAIN_UV)
            float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
        #else
            float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);
        #endif

        //--------------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = 1.0;
        if(Exists_MainTex) col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain);
        col *= _Color;

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Fur AO
        #if LIL_RENDER == 1
            col.rgb *= 1.0-_FurAO;
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Copy
        float3 albedo = col.rgb;

        #ifndef LIL_PASS_FORWARDADD
            //----------------------------------------------------------------------------------------------------------------------
            // Lighting
            #if defined(LIL_FEATURE_SHADOW)
                float3 normalDirection = normalize(input.normalWS);
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
                float shadowmix = 1.0;
                lilGetShading(col, shadowmix, albedo, uvMain, facing, normalDirection, 1, lightDirection);
            #endif
            col.rgb *= saturate(lightColor + vertexLightColor + additionalLightColor);
        #else
            col.rgb *= lightColor;
            // Premultiply for ForwardAdd
            #if LIL_RENDER == 2 && LIL_PREMULTIPLY_FA
                col.rgb *= col.a;
            #endif
        #endif
    #else
        //--------------------------------------------------------------------------------------------------------------------------
        // UV
        #if defined(LIL_FEATURE_ANIMATE_MAIN_UV)
            float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
        #else
            float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);
        #endif

        //--------------------------------------------------------------------------------------------------------------------------
        // View Direction
        #if defined(LIL_SHOULD_POSITION_WS)
            float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
        #endif
        #if defined(LIL_SHOULD_TBN)
            float3x3 tbnWS = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
            #if defined(LIL_SHOULD_POSITION_WS)
                float3 parallaxViewDirection = mul(tbnWS, viewDirection);
                float2 parallaxOffset = (parallaxViewDirection.xy / (parallaxViewDirection.z+0.5));
            #endif
        #endif

        //--------------------------------------------------------------------------------------------------------------------------
        // Parallax
        #if defined(LIL_FEATURE_PARALLAX)
            LIL_BRANCH
            if(Exists_ParallaxMap && _UseParallax)
            {
                float height = (LIL_SAMPLE_2D_LOD(_ParallaxMap,sampler_linear_repeat,uvMain,0).r - _ParallaxOffset) * _Parallax;
                uvMain += height * parallaxOffset;
                input.uv += height * parallaxOffset;
            }
        #endif

        //--------------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = 1.0;
        if(Exists_MainTex)
        {
            col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain);
            #if defined(LIL_FEATURE_MAIN_TONE_CORRECTION)
                col.rgb = lilToneCorrection(col.rgb, _MainTexHSVG);
            #endif
        }
        col *= _Color;

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Normal
        #if defined(LIL_SHOULD_NORMAL)
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                float3 normalmap = float3(0.0,0.0,1.0);

                // 1st
                #if defined(LIL_FEATURE_NORMAL_1ST)
                    LIL_BRANCH
                    if(Exists_BumpMap && _UseBumpMap)
                    {
                        float4 normalTex = LIL_SAMPLE_2D_ST(_BumpMap, sampler_MainTex, uvMain);
                        normalmap = UnpackNormalScale(normalTex, _BumpScale);
                    }
                #endif

                // 2nd
                #if defined(LIL_FEATURE_NORMAL_2ND)
                    LIL_BRANCH
                    if(Exists_Bump2ndMap && _UseBump2ndMap)
                    {
                        float4 normal2ndTex = LIL_SAMPLE_2D_ST(_Bump2ndMap, sampler_MainTex, uvMain);
                        float bump2ndScale = _Bump2ndScale;
                        if(Exists_Bump2ndScaleMask) bump2ndScale *= LIL_SAMPLE_2D(_Bump2ndScaleMask, sampler_MainTex, uvMain).r;
                        normalmap = lilBlendNormal(normalmap, UnpackNormalScale(normal2ndTex, bump2ndScale));
                    }
                #endif

                float3 normalDirection = normalize(mul(normalmap, tbnWS));
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
            #else
                float3 normalDirection = normalize(input.normalWS);
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
            #endif
            #if defined(LIL_SHOULD_POSITION_WS)
                float nv = saturate(dot(normalDirection, viewDirection));
                float nvabs = abs(dot(normalDirection, viewDirection));
            #else
                float nv = 1.0;
                float nvabs = 1.0;
            #endif
        #else
            float nv = 1.0;
        #endif

        //--------------------------------------------------------------------------------------------------------------------------
        // Layer Color
        #if defined(LIL_SHOULD_TANGENT_W)
            bool isRightHand = input.tangentW > 0.0;
        #else
            bool isRightHand = true;
        #endif
        // 2nd
        #if defined(LIL_FEATURE_MAIN2ND)
            #if !(defined(LIL_FEATURE_DECAL) && defined(LIL_FEATURE_ANIMATE_DECAL))
                float4 _Main2ndTexDecalAnimation = 0.0;
                float4 _Main2ndTexDecalSubParam = 0.0;
            #endif
            #if !defined(LIL_FEATURE_DECAL)
                bool _Main2ndTexIsDecal = false;
                bool _Main2ndTexIsLeftOnly = false;
                bool _Main2ndTexIsRightOnly = false;
                bool _Main2ndTexShouldCopy = false;
                bool _Main2ndTexShouldFlipMirror = false;
                bool _Main2ndTexShouldFlipCopy = false;
            #endif
            float4 color2nd = _Color2nd;
            LIL_BRANCH
            if(_UseMain2ndTex)
            {
                if(Exists_Main2ndTex) color2nd *= LIL_GET_SUBTEX(_Main2ndTex, input.uv);
                if(Exists_Main2ndBlendMask) color2nd.a *= LIL_SAMPLE_2D(_Main2ndBlendMask, sampler_MainTex, uvMain).r;
                if(_Main2ndEnableLighting) col.rgb = lilBlendColor(col.rgb, color2nd.rgb, color2nd.a, _Main2ndTexBlendMode);
            }
        #endif

        // 3rd
        #if defined(LIL_FEATURE_MAIN3RD)
            #if !(defined(LIL_FEATURE_DECAL) && defined(LIL_FEATURE_ANIMATE_DECAL))
                float4 _Main3rdTexDecalAnimation = 0.0;
                float4 _Main3rdTexDecalSubParam = 0.0;
            #endif
            #if !defined(LIL_FEATURE_DECAL)
                bool _Main3rdTexIsDecal = false;
                bool _Main3rdTexIsLeftOnly = false;
                bool _Main3rdTexIsRightOnly = false;
                bool _Main3rdTexShouldCopy = false;
                bool _Main3rdTexShouldFlipMirror = false;
                bool _Main3rdTexShouldFlipCopy = false;
            #endif
            float4 color3rd = _Color3rd;
            LIL_BRANCH
            if(_UseMain3rdTex)
            {
                if(Exists_Main3rdTex) color3rd *= LIL_GET_SUBTEX(_Main3rdTex, input.uv);
                if(Exists_Main3rdBlendMask) color3rd.a *= LIL_SAMPLE_2D(_Main3rdBlendMask, sampler_MainTex, uvMain).r;
                if(_Main3rdEnableLighting) col.rgb = lilBlendColor(col.rgb, color3rd.rgb, color3rd.a, _Main3rdTexBlendMode);
            }
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Copy
        float3 albedo = col.rgb;

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting
        #ifndef LIL_PASS_FORWARDADD
            float shadowmix = 1.0;
            #if defined(LIL_FEATURE_SHADOW)
                lilGetShading(col, shadowmix, albedo, uvMain, facing, normalDirection, attenuation, lightDirection);
            #endif
            col.rgb *= lightColor;

            lightColor += vertexLightColor;
            shadowmix += lilLuminance(vertexLightColor);
            col.rgb += albedo * vertexLightColor;

            lightColor += additionalLightColor;
            shadowmix += lilLuminance(additionalLightColor);
            col.rgb += albedo * additionalLightColor;

            lightColor = saturate(lightColor);
            shadowmix = saturate(shadowmix);
            col.rgb = min(col.rgb, albedo);

            #if defined(LIL_FEATURE_MAIN2ND)
                if(_UseMain2ndTex && !_Main2ndEnableLighting) col.rgb = lilBlendColor(col.rgb, color2nd.rgb, color2nd.a, _Main2ndTexBlendMode);
            #endif
            #if defined(LIL_FEATURE_MAIN3RD)
                if(_UseMain3rdTex && !_Main3rdEnableLighting) col.rgb = lilBlendColor(col.rgb, color3rd.rgb, color3rd.a, _Main3rdTexBlendMode);
            #endif
        #else
            col.rgb *= lightColor;
            #if defined(LIL_FEATURE_MAIN2ND)
                if(_UseMain2ndTex && !_Main2ndEnableLighting) col.rgb = col.rgb - col.rgb * color2nd.a;
            #endif
            #if defined(LIL_FEATURE_MAIN3RD)
                if(_UseMain3rdTex && !_Main3rdEnableLighting) col.rgb = col.rgb - col.rgb * color3rd.a;
            #endif
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Premultiply
        #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            col.rgb *= col.a;
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Refraction
        #if defined(LIL_REFRACTION) && !defined(LIL_PASS_FORWARDADD)
            float2 scnUV = input.positionSS.xy/input.positionSS.w;
            float2 refractUV = scnUV + (pow(1.0 - nv, _RefractionFresnelPower) * _RefractionStrength) * mul(LIL_MATRIX_V, float4(normalDirection,0)).xy;
            // Hide objects in front
            //float refrFix = UNITY_SAMPLE_DEPTH(LIL_SAMPLE_2D(_CameraDepthTexture, sampler_CameraDepthTexture, refractUV));
            //if(LinearEyeDepth(refrFix) < input.positionSS.w) refractUV = scnUV;
            #if defined(LIL_REFRACTION_BLUR2) && defined(LIL_FEATURE_REFLECTION)
                float smoothness = _Smoothness;
                if(Exists_SmoothnessTex) smoothness *= LIL_SAMPLE_2D(_SmoothnessTex, sampler_MainTex, uvMain).r;
                float perceptualRoughness = 1.0 - smoothness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float3 refractCol = 0;
                float sum = 0;
                float blurOffset = perceptualRoughness / input.positionSS.z * (0.0005 / LIL_REFRACTION_SAMPNUM);
                for(int j = -16; j <= 16; j++)
                {
                    refractCol += LIL_SAMPLE_2D(_GrabTexture, sampler_GrabTexture, refractUV + float2(0,j*blurOffset)).rgb * LIL_REFRACTION_GAUSDIST(j);
                    sum += LIL_REFRACTION_GAUSDIST(j);
                }
                refractCol /= sum;
                refractCol *= _RefractionColor.rgb;
            #elif defined(LIL_REFRACTION_BLUR2)
                float3 refractCol = LIL_SAMPLE_2D(_GrabTexture, sampler_GrabTexture, refractUV).rgb * _RefractionColor.rgb;
            #else
                float3 refractCol = LIL_SAMPLE_2D(_BackgroundTexture, sampler_BackgroundTexture, refractUV).rgb * _RefractionColor.rgb;
            #endif
            if(_RefractionColorFromMain) refractCol *= albedo;
            col.rgb = lerp(refractCol, col.rgb, col.a);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Reflection
        #if defined(LIL_FEATURE_REFLECTION)
            #ifndef LIL_PASS_FORWARDADD
                LIL_BRANCH
                if(_UseReflection)
            #else
                LIL_BRANCH
                if(_UseReflection && _ApplySpecular)
            #endif
            {
                float3 reflectCol = 0;
                // Smoothness
                #if !defined(LIL_REFRACTION_BLUR2) || defined(LIL_PASS_FORWARDADD)
                    float smoothness = _Smoothness;
                    if(Exists_SmoothnessTex) smoothness *= LIL_SAMPLE_2D(_SmoothnessTex, sampler_MainTex, uvMain).r;
                    float perceptualRoughness = 1.0 - smoothness;
                    float roughness = perceptualRoughness * perceptualRoughness;
                #endif
                // Metallic
                float metallic = _Metallic;
                if(Exists_MetallicGlossMap) metallic *= LIL_SAMPLE_2D(_MetallicGlossMap, sampler_MainTex, uvMain).r;
                col.rgb = col.rgb - metallic * col.rgb;
                float3 specular = lerp(LIL_DIELECTRIC_SPECULAR.rgb,albedo,metallic);
                // Specular
                #ifndef LIL_PASS_FORWARDADD
                    LIL_BRANCH
                    if(_ApplySpecular)
                #endif
                {
                    float3 lightDirectionSpc = lilGetLightDirection(input.positionWS);
                    float3 halfDirection = normalize(viewDirection + lightDirectionSpc);
                    float nl = saturate(dot(normalDirection, lightDirectionSpc));
                    float nh = saturate(dot(normalDirection, halfDirection));
                    float lh = saturate(dot(lightDirectionSpc, halfDirection));
                    #if defined(SHADOWS_SCREEN) || defined(LIL_PASS_FORWARDADD)
                        reflectCol = lilCalcSpecular(nv, nl, nh, lh, roughness, specular, _SpecularToon, attenuation) * _MainLightColor.rgb;
                    #else
                        reflectCol = lilCalcSpecular(nv, nl, nh, lh, roughness, specular, _SpecularToon) * _MainLightColor.rgb;
                    #endif
                }
                // Reflection
                #ifndef LIL_PASS_FORWARDADD
                    LIL_BRANCH
                    if(_ApplyReflection)
                    {
                        LIL_GET_ENVIRONMENT_REFLECTION(viewDirection, normalDirection, perceptualRoughness, input.positionWS, envReflectionColor);

                        float oneMinusReflectivity = LIL_DIELECTRIC_SPECULAR.a - metallic * LIL_DIELECTRIC_SPECULAR.a;
                        float grazingTerm = saturate(smoothness + (1.0-oneMinusReflectivity));
                        #ifdef LIL_COLORSPACE_GAMMA
                            float surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
                        #else
                            float surfaceReduction = 1.0 / (roughness * roughness + 1.0);
                        #endif

                        #ifdef LIL_REFRACTION
                            col.rgb = lerp(envReflectionColor, col.rgb, col.a+(1.0-col.a)*pow(nvabs,_RefractionStrength*0.5+0.25));
                            reflectCol += col.a * surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, nv);
                            col.a = 1.0;
                        #else
                            reflectCol += surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, nv);
                        #endif
                    }
                #endif
                // Mix
                float4 reflectionColor = _ReflectionColor;
                if(Exists_ReflectionColorTex) reflectionColor *= LIL_SAMPLE_2D(_ReflectionColorTex, sampler_MainTex, uvMain);
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_ReflectionApplyTransparency) reflectionColor.a *= col.a;
                #endif
                col.rgb += reflectionColor.rgb * reflectionColor.a * reflectCol;
            }
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        #if defined(LIL_FEATURE_MATCAP)
            LIL_BRANCH
            if(_UseMatCap)
            {
                float2 matUV = lilCalcMatCapUV(normalDirection);
                float4 matCapColor = _MatCapColor;
                if(Exists_MatCapTex) matCapColor *= LIL_SAMPLE_2D(_MatCapTex, sampler_MainTex, matUV);
                if(_MatCapEnableLighting) matCapColor.rgb *= lightColor;
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_MatCapApplyTransparency) matCapColor.a *= col.a;
                #endif
                if(Exists_MatCapBlendMask) matCapColor.a *= LIL_SAMPLE_2D(_MatCapBlendMask, sampler_MainTex, uvMain).r;
                col.rgb = lilBlendColor(col.rgb, matCapColor.rgb, _MatCapBlend * matCapColor.a, _MatCapBlendMode);
            }
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Rim light
        #if defined(LIL_FEATURE_RIMLIGHT)
            #ifndef LIL_PASS_FORWARDADD
                LIL_BRANCH
                if(_UseRim)
            #else
                LIL_BRANCH
                if(_UseRim && _RimEnableLighting)
            #endif
            {
                float4 rimColor = _RimColor;
                if(Exists_RimColorTex) rimColor *= LIL_SAMPLE_2D(_RimColorTex, sampler_MainTex, uvMain);
                float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
                rim = lilTooning(rim, _RimBorder, _RimBlur);
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_RimApplyTransparency) rim *= col.a;
                #endif
                #ifndef LIL_PASS_FORWARDADD
                    if(_RimShadowMask) rim *= shadowmix;
                    if(_RimEnableLighting) rimColor.rgb *= lightColor;
                    col.rgb += rim * rimColor.a * rimColor.rgb;
                #else
                    col.rgb += rim * rimColor.a * rimColor.rgb * lightColor;
                #endif
            }
        #endif

        #ifndef LIL_PASS_FORWARDADD
            float3 invLighting = saturate((1.0 - lightColor) * sqrt(lightColor));
            //----------------------------------------------------------------------------------------------------------------------
            // Emission
            #if defined(LIL_FEATURE_EMISSION_1ST)
                LIL_BRANCH
                if(_UseEmission)
                {
                    float2 _EmissionMapParaTex = input.uv + _EmissionParallaxDepth * parallaxOffset;
                    float4 emissionColor = _EmissionColor;
                    // Texture
                    #if defined(LIL_FEATURE_EMISSION_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                        if(Exists_EmissionMap) emissionColor *= LIL_GET_EMITEX(_EmissionMap, _EmissionMapParaTex);
                    #elif defined(LIL_FEATURE_EMISSION_UV)
                        if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D(_EmissionMap, sampler_EmissionMap, lilCalcUV(_EmissionMapParaTex, _EmissionMap_ST));
                    #else
                        if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D(_EmissionMap, sampler_EmissionMap, uvMain + _EmissionParallaxDepth * parallaxOffset);
                    #endif
                    // Mask
                    #if defined(LIL_FEATURE_EMISSION_MASK_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                        if(Exists_EmissionBlendMask) emissionColor *= LIL_GET_EMIMASK(_EmissionBlendMask, input.uv);
                    #elif defined(LIL_FEATURE_EMISSION_MASK_UV)
                        if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D(_EmissionBlendMask, sampler_MainTex, lilCalcUV(input.uv, _EmissionBlendMask_ST));
                    #else
                        if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D(_EmissionBlendMask, sampler_MainTex, uvMain);
                    #endif
                    // Gradation
                    #if defined(LIL_FEATURE_EMISSION_GRADATION)
                        if(Exists_EmissionGradTex && _EmissionUseGrad) emissionColor *= LIL_SAMPLE_1D(_EmissionGradTex, sampler_linear_repeat, _EmissionGradSpeed*LIL_TIME);
                    #endif
                    #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                        emissionColor.a *= col.a;
                    #endif
                    emissionColor.rgb = lerp(emissionColor.rgb, emissionColor.rgb * invLighting, _EmissionFluorescence);
                    col.rgb += _EmissionBlend * lilCalcBlink(_EmissionBlink) * emissionColor.a * emissionColor.rgb;
                }
            #endif

            // Emission2nd
            #if defined(LIL_FEATURE_EMISSION_2ND)
                LIL_BRANCH
                if(_UseEmission2nd)
                {
                    float2 _Emission2ndMapParaTex = input.uv + _Emission2ndParallaxDepth * parallaxOffset;
                    float4 emission2ndColor = _Emission2ndColor;
                    // Texture
                    #if defined(LIL_FEATURE_EMISSION_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                        if(Exists_Emission2ndMap) emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap, _Emission2ndMapParaTex);
                    #elif defined(LIL_FEATURE_EMISSION_UV)
                        if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndMap, sampler_Emission2ndMap, lilCalcUV(_Emission2ndMapParaTex, _Emission2ndMap_ST));
                    #else
                        if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndMap, sampler_Emission2ndMap, uvMain + _Emission2ndParallaxDepth * parallaxOffset);
                    #endif
                    // Mask
                    #if defined(LIL_FEATURE_EMISSION_MASK_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                        if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_GET_EMIMASK(_Emission2ndBlendMask, input.uv);
                    #elif defined(LIL_FEATURE_EMISSION_MASK_UV)
                        if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndBlendMask, sampler_MainTex, lilCalcUV(input.uv, _Emission2ndBlendMask_ST));
                    #else
                        if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndBlendMask, sampler_MainTex, uvMain);
                    #endif
                    // Gradation
                    #if defined(LIL_FEATURE_EMISSION_GRADATION)
                        if(Exists_Emission2ndGradTex && _Emission2ndUseGrad) emission2ndColor *= LIL_SAMPLE_1D(_Emission2ndGradTex, sampler_linear_repeat, _Emission2ndGradSpeed*LIL_TIME);
                    #endif
                    #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                        emission2ndColor.a *= col.a;
                    #endif
                    emission2ndColor.rgb = lerp(emission2ndColor.rgb, emission2ndColor.rgb * invLighting, _Emission2ndFluorescence);
                    col.rgb += _Emission2ndBlend * lilCalcBlink(_Emission2ndBlink) * emission2ndColor.a * emission2ndColor.rgb;
                }
            #endif
        #endif
    #endif

    //--------------------------------------------------------------------------------------------------------------------------
    // Distance Fade
    #if defined(LIL_FEATURE_DISTANCE_FADE)
        float depth = length(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
        float distFade = saturate((depth - _DistanceFade.x) / (_DistanceFade.y - _DistanceFade.x)) * _DistanceFade.z;
        #if defined(LIL_PASS_FORWARDADD)
            col.rgb = lerp(col.rgb, 0.0, distFade);
        #elif LIL_RENDER == 2
            col.rgb = lerp(col.rgb, _DistanceFadeColor.rgb, distFade);
            col.a = lerp(col.a, col.a * _DistanceFadeColor.a, distFade);
        #else
            col.rgb = lerp(col.rgb, _DistanceFadeColor.rgb, distFade);
        #endif
    #endif

    //--------------------------------------------------------------------------------------------------------------------------
    // Fog
    LIL_APPLY_FOG(col, input.fogCoord);

    return col;
}

#endif
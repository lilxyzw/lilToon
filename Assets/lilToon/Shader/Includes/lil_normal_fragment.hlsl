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

    //--------------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
    #if !defined(LIL_OUTLINE) && !defined(LIL_FUR)
        //--------------------------------------------------------------------------------------------------------------------------
        // View Direction
        float3x3 tbnWS = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
        float3 viewDirection = LIL_GET_VIEWDIR_WS(input.positionWS.xyz);
        float3 parallaxViewDirection = mul(tbnWS, viewDirection);
        float2 parallaxOffset = (parallaxViewDirection.xy / (parallaxViewDirection.z+0.5));

        //--------------------------------------------------------------------------------------------------------------------------
        // Parallax
        if(_UseParallax)
        {
            float height = (LIL_SAMPLE_2D_LOD(_ParallaxMap,sampler_linear_repeat,uvMain,0).r - _ParallaxOffset) * _Parallax;
            float2 parallaxOffset2 = height * parallaxOffset;
            uvMain += parallaxOffset2;
            input.uv += parallaxOffset2;
        }
    #endif

    //--------------------------------------------------------------------------------------------------------------------------
    // Main Color
    float4 col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain);
    #ifndef LIL_FUR
        col.rgb = lilToneCorrection(col.rgb, _MainTexHSVG);
    #endif
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
        clip(col.a - 0.001);
    #endif

    //--------------------------------------------------------------------------------------------------------------------------
    // Apply Matelial & Lighting
    #if defined(LIL_OUTLINE)
        //----------------------------------------------------------------------------------------------------------------------
        // Outline Color
        // Multiply Main Color
        if(_OutlineUseMainColor) col.rgb *= lerp(1, col.rgb, _OutlineMainStrength);
        else                     col.rgb = 1.0;

        // Apply Color
        #if LIL_RENDER == 2
            col *= _OutlineColor;
        #else
            col.rgb *= _OutlineColor.rgb;
        #endif

        col.rgb *= saturate(lightColor + vertexLightColor + additionalLightColor);
    #elif defined(LIL_FUR)
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
            // Normal
            float3 normalDirection = normalize(input.normalWS);
            normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;

            //----------------------------------------------------------------------------------------------------------------------
            // Lighting
            float shadowmix = 1.0;
            lilGetShading(col, shadowmix, albedo, uvMain, facing, normalDirection, 1, lightDirection);
            col.rgb *= lightColor + vertexLightColor + additionalLightColor;

            col.rgb = min(col.rgb, albedo);
        #else
            col.rgb *= lightColor;
            // Premultiply for ForwardAdd
            #if LIL_RENDER == 2 && LIL_PREMULTIPLY_FA
                col.rgb *= col.a;
            #endif
        #endif
    #else
        //--------------------------------------------------------------------------------------------------------------------------
        // Layer Color
        bool isRightHand = input.tangentW > 0.0;
        // 2nd
        LIL_BRANCH
        if(_UseMain2ndTex)
        {
            _Color2nd *= LIL_GET_SUBTEX(_Main2ndTex, input.uv);
            col.rgb = lilBlendColor(col.rgb, _Color2nd.rgb, LIL_SAMPLE_2D(_Main2ndBlendMask, sampler_MainTex, uvMain).r * _Color2nd.a, _Main2ndTexBlendMode);
        }

        // 3rd
        LIL_BRANCH
        if(_UseMain3rdTex)
        {
            _Color3rd *= LIL_GET_SUBTEX(_Main3rdTex, input.uv);
            col.rgb = lilBlendColor(col.rgb, _Color3rd.rgb, LIL_SAMPLE_2D(_Main3rdBlendMask, sampler_MainTex, uvMain).r * _Color3rd.a, _Main3rdTexBlendMode);
        }

        //----------------------------------------------------------------------------------------------------------------------
        // Copy
        float3 albedo = col.rgb;

        //----------------------------------------------------------------------------------------------------------------------
        // Normal
        float3 normalmap = float3(0.0,0.0,1.0);

        // 1st
        LIL_BRANCH
        if(_UseBumpMap)
        {
            float4 normalTex = LIL_SAMPLE_2D_ST(_BumpMap, sampler_MainTex, input.uv);
            normalmap = UnpackNormalScale(normalTex, _BumpScale);
        }

        // 2nd
        LIL_BRANCH
        if(_UseBump2ndMap)
        {
            float4 normal2ndTex = LIL_SAMPLE_2D_ST(_Bump2ndMap, sampler_MainTex, input.uv);
            _Bump2ndScale *= LIL_SAMPLE_2D(_Bump2ndScaleMask, sampler_MainTex, uvMain).r;
            normalmap = lilBlendNormal(normalmap, UnpackNormalScale(normal2ndTex, _Bump2ndScale));
        }

        float3 normalDirection = normalize(mul(normalmap, tbnWS));
        normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
        float nv = saturate(dot(normalDirection, viewDirection));
        float nvabs = abs(dot(normalDirection, viewDirection));

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting
        #ifndef LIL_PASS_FORWARDADD
            float shadowmix = 1.0;
            lilGetShading(col, shadowmix, albedo, uvMain, facing, normalDirection, attenuation, lightDirection);
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
        #else
            col.rgb *= lightColor;
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Refraction
        #if defined(LIL_REFRACTION) && !defined(LIL_PASS_FORWARDADD)
            float2 scnUV = input.positionSS.xy/input.positionSS.w;
            float2 refractUV = scnUV + (pow(1.0 - nv, _RefractionFresnelPower) * _RefractionStrength) * mul(LIL_MATRIX_V, float4(normalDirection,0)).xy;
            // Hide objects in front
            //float refrFix = UNITY_SAMPLE_DEPTH(LIL_SAMPLE_2D(_CameraDepthTexture, sampler_CameraDepthTexture, refractUV));
            //if(LinearEyeDepth(refrFix) < input.positionSS.w) refractUV = scnUV;
            #ifdef LIL_REFRACTION_BLUR2
                float smoothness = LIL_SAMPLE_2D(_SmoothnessTex, sampler_MainTex, uvMain).r * _Smoothness;
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
            #else
                float3 refractCol = LIL_SAMPLE_2D(_BackgroundTexture, sampler_BackgroundTexture, refractUV).rgb * _RefractionColor.rgb;
            #endif
            if(_RefractionColorFromMain) refractCol *= albedo;
            col.rgb = lerp(refractCol, col.rgb, col.a);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Reflection
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
            #ifndef LIL_REFRACTION_BLUR2
                float smoothness = LIL_SAMPLE_2D(_SmoothnessTex, sampler_MainTex, uvMain).r * _Smoothness;
                float perceptualRoughness = 1.0 - smoothness;
                float roughness = perceptualRoughness * perceptualRoughness;
            #endif
            // Metallic
            float metallic = LIL_SAMPLE_2D(_MetallicGlossMap, sampler_MainTex, uvMain).r * _Metallic;
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
                        col.rgb = lerp(envReflectionColor, col.rgb, col.a+(1-col.a)*pow(nvabs,_RefractionStrength*0.5+0.25));
                        reflectCol += col.a * surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, nv);
                        col.a = 1.0;
                    #else
                        reflectCol += surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, nv);
                    #endif
                }
            #endif
            // Mix
            _ReflectionColor *= LIL_SAMPLE_2D(_ReflectionColorTex, sampler_MainTex, uvMain);
            col.rgb += _ReflectionColor.rgb * _ReflectionColor.a * reflectCol;
        }

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        LIL_BRANCH
        if(_UseMatCap)
        {
            float2 matUV = lilCalcMatCapUV(normalDirection);
            _MatCapColor *= LIL_SAMPLE_2D(_MatCapTex, sampler_MainTex, matUV);
            if(_MatCapBlendLight) _MatCapColor.rgb *= lightColor;
            col.rgb = lilBlendColor(col.rgb, _MatCapColor.rgb, LIL_SAMPLE_2D(_MatCapBlendMask, sampler_MainTex, uvMain).r * _MatCapBlend * _MatCapColor.a, _MatCapBlendMode);
        }

        //----------------------------------------------------------------------------------------------------------------------
        // Rim light
        #ifndef LIL_PASS_FORWARDADD
            LIL_BRANCH
            if(_UseRim)
        #else
            LIL_BRANCH
            if(_UseRim && _RimBlendLight)
        #endif
        {
            _RimColor *= LIL_SAMPLE_2D(_RimColorTex, sampler_MainTex, uvMain);
            float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
            float rimBorderMin = saturate(_RimBorder - _RimBlur * 0.5);
            float rimBorderMax = saturate(_RimBorder + _RimBlur * 0.5);
            rim = saturate((rim - rimBorderMin) / (rimBorderMax - rimBorderMin));
            #ifndef LIL_PASS_FORWARDADD
                if(_RimShadowMask) rim *= shadowmix;
                if(_RimBlendLight) _RimColor.rgb *= lightColor;
                col.rgb += rim * _RimColor.a * _RimColor.rgb;
            #else
                col.rgb += rim * _RimColor.a * _RimColor.rgb * lightColor;
            #endif
        }

        #ifndef LIL_PASS_FORWARDADD
            float3 invLighting = saturate((1.0 - lightColor) * sqrt(lightColor));
            //----------------------------------------------------------------------------------------------------------------------
            // Emission
            float3 emittionColor = 0;
            LIL_BRANCH
            if(_UseEmission)
            {
                float2 _EmissionMapParaTex = input.uv + _EmissionParallaxDepth * parallaxOffset;
                _EmissionColor *= LIL_GET_EMITEX(_EmissionMap,_EmissionMapParaTex);
                _EmissionColor.rgb = lerp(_EmissionColor.rgb, _EmissionColor.rgb * invLighting, _EmissionFluorescence);
                float _EmissionBlinkSeq = lilCalcBlink(_EmissionBlink);
                if(_EmissionUseGrad) _EmissionColor *= LIL_SAMPLE_1D(_EmissionGradTex, sampler_linear_repeat, _EmissionGradSpeed*LIL_TIME);
                emittionColor = LIL_GET_EMIMASK(_EmissionBlendMask,input.uv) * _EmissionBlend * _EmissionBlinkSeq * _EmissionColor.a * _EmissionColor.rgb;
            }
            col.rgb += emittionColor;

            // Emission2nd
            float3 emittion2ndColor = 0;
            LIL_BRANCH
            if(_UseEmission2nd)
            {
                float2 _Emission2ndMapParaTex = input.uv + _Emission2ndParallaxDepth * parallaxOffset;
                _Emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap,_Emission2ndMapParaTex);
                _Emission2ndColor.rgb = lerp(_Emission2ndColor.rgb, _Emission2ndColor.rgb * invLighting, _Emission2ndFluorescence);
                float _Emission2ndBlinkSeq = lilCalcBlink(_Emission2ndBlink);
                if(_Emission2ndUseGrad) _Emission2ndColor *= LIL_SAMPLE_1D(_Emission2ndGradTex, sampler_linear_repeat, _Emission2ndGradSpeed*LIL_TIME);
                emittion2ndColor = LIL_GET_EMIMASK(_Emission2ndBlendMask,input.uv) * _Emission2ndBlend * _Emission2ndBlinkSeq * _Emission2ndColor.a * _Emission2ndColor.rgb;
            }
            col.rgb += emittion2ndColor;
        #else
            //----------------------------------------------------------------------------------------------------------------------
            // Premultiply for ForwardAdd
            #if LIL_RENDER == 2 && LIL_PREMULTIPLY_FA
                col.rgb *= col.a;
            #endif
        #endif
    #endif

    //--------------------------------------------------------------------------------------------------------------------------
    // Fog
    LIL_APPLY_FOG(col, input.fogCoord);

    return col;
}

#endif
#ifndef LIL_FRAGMENT_LITE_INCLUDED
#define LIL_FRAGMENT_LITE_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Fragment shader
float4 frag(v2f input, float facing : VFACE) : SV_Target
{
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
    LIL_GET_VERTEXLIGHT(input, vertexLightColor);
    LIL_GET_ADDITIONALLIGHT(input.positionWS, additionalLightColor);
    #if !defined(LIL_PASS_FORWARDADD)
        #if defined(LIL_USE_LIGHTMAP)
            lightColor = max(lightColor, _LightMinLimit);
            lightColor = lerp(lightColor, 1.0, _AsUnlit);
        #endif
        #if defined(_ADDITIONAL_LIGHTS)
            float3 addLightColor = vertexLightColor + lerp(additionalLightColor, 0.0, _AsUnlit);
        #else
            float3 addLightColor = vertexLightColor;
        #endif
    #else
        lightColor = lerp(lightColor, 0.0, _AsUnlit);
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Apply Matelial & Lighting
    #if defined(LIL_OUTLINE)
        //----------------------------------------------------------------------------------------------------------------------
        // UV
        float2 uvMain = lilCalcUV(input.uv, _OutlineTex_ST, _OutlineTex_ScrollRotate);

        //----------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = _OutlineColor;
        col *= LIL_SAMPLE_2D(_OutlineTex, sampler_OutlineTex, uvMain);

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            clip(col.a - _Cutoff);
        #elif LIL_RENDER == 2
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Copy
        float3 albedo = col.rgb;

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting
        col.rgb = lerp(col.rgb, col.rgb * saturate(lightColor + addLightColor), _OutlineEnableLighting);
    #else
        //----------------------------------------------------------------------------------------------------------------------
        // UV
        float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);

        //----------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = _Color;
        col *= LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain);
        float4 triMask = 1.0;
        triMask = LIL_SAMPLE_2D(_TriMask, sampler_MainTex, uvMain);

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            clip(col.a - _Cutoff);
        #elif LIL_RENDER == 2
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Normal
        float3 normalDirection = normalize(input.normalWS);
        normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        if(_UseMatCap)
        {
            float3 matcap = 1.0;
            matcap = LIL_SAMPLE_2D(_MatCapTex, sampler_MainTex, input.uvMat).rgb;
            col.rgb = lerp(col.rgb, _MatCapMul ? col.rgb * matcap : col.rgb + matcap, triMask.r);
        }

        //----------------------------------------------------------------------------------------------------------------------
        // Copy
        float3 albedo = col.rgb;

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting
        #ifndef LIL_PASS_FORWARDADD
            float shadowmix = 1.0;
            lilGetShadingLite(col, shadowmix, albedo, lightColor, input.indLightColor, uvMain, facing, normalDirection, lightDirection, sampler_MainTex);

            lightColor += addLightColor;
            shadowmix += lilLuminance(addLightColor);
            col.rgb += albedo * addLightColor;

            lightColor = saturate(lightColor);
            shadowmix = saturate(shadowmix);
            col.rgb = min(col.rgb, albedo);
        #else
            col.rgb *= lightColor;
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Rim light
        LIL_BRANCH
        if(_UseRim)
        {
            float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
            float nvabs = abs(dot(normalDirection, viewDirection));
            float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
            rim = lilTooning(rim, _RimBorder, _RimBlur);
            #ifndef LIL_PASS_FORWARDADD
                if(_RimShadowMask) rim *= shadowmix;
            #endif
            col.rgb += rim * triMask.g * _RimColor.rgb * lightColor;
        }

        #ifndef LIL_PASS_FORWARDADD
            //----------------------------------------------------------------------------------------------------------------------
            // Emission
            if(_UseEmission)
            {
                float emissionBlinkSeq = lilCalcBlink(_EmissionBlink);
                float4 emissionColor = _EmissionColor;
                emissionColor *= LIL_GET_EMITEX(_EmissionMap,input.uv);
                col.rgb += emissionBlinkSeq * triMask.b * emissionColor.rgb;
            }
        #endif
    #endif

    //-------------------------------------------------------------------------------------------------------------------------
    // Fog
    LIL_APPLY_FOG(col, input.fogCoord);

    return col;
}

#endif
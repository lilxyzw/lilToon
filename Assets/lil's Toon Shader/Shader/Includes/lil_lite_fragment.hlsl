// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

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

    //----------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);

    //----------------------------------------------------------------------------------------------------------------------
    // Main Color
    float4 col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain) * _Color;
    float4 triMask = LIL_SAMPLE_2D(_TriMask, sampler_MainTex, uvMain);

    //----------------------------------------------------------------------------------------------------------------------
    // Alpha
    #if LIL_RENDER == 0
        // Opaque
    #elif LIL_RENDER == 1
        // Cutout
        clip(col.a - _Cutoff);
    #elif LIL_RENDER == 2
        // Transparent
        clip(col.a - 0.001);
    #endif

    //----------------------------------------------------------------------------------------------------------------------
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
    #else
        //----------------------------------------------------------------------------------------------------------------------
        // Normal
        float3 normalDirection = normalize(input.normalWS);
        normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        if(_UseMatCap)
        {
            float3 matcap = LIL_SAMPLE_2D(_MatCapTex, sampler_MainTex, input.uvMat).rgb;
            col.rgb = lerp(col.rgb, _MatCapMul ? col.rgb * matcap : col.rgb + matcap, triMask.r);
        }

        //----------------------------------------------------------------------------------------------------------------------
        // Copy
        float3 albedo = col.rgb;

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting
        #ifndef LIL_PASS_FORWARDADD
            float shadowmix = 1.0;
            lilGetShadingLite(col, shadowmix, albedo, uvMain, facing, normalDirection, lightDirection);
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
        // Rim light
        LIL_BRANCH
        if(_UseRim)
        {
            float3 viewDirection = LIL_GET_VIEWDIR_WS(input.positionWS.xyz);
            float nvabs = abs(dot(normalDirection, viewDirection));
            float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
            float rimBorderMin = saturate(_RimBorder - _RimBlur * 0.5);
            float rimBorderMax = saturate(_RimBorder + _RimBlur * 0.5);
            rim = saturate((rim - rimBorderMin) / (rimBorderMax - rimBorderMin));
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
                float _EmissionBlinkSeq = lilCalcBlink(_EmissionBlink);
                col.rgb += _EmissionBlinkSeq * triMask.b * LIL_GET_EMITEX(_EmissionMap,input.uv).rgb;
            }
        #endif
    #endif

    //-------------------------------------------------------------------------------------------------------------------------
    // Fog
    LIL_APPLY_FOG(col, input.fogCoord);

    return col;
}

#endif
#ifndef LIL_FRAGMENT_FUR_INCLUDED
#define LIL_FRAGMENT_FUR_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Fragment shader
float4 frag(g2f input) : SV_Target
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

    float facing = 1.0;

    //--------------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);

    //--------------------------------------------------------------------------------------------------------------------------
    // Main Color
    float4 col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain) * _Color;

    //--------------------------------------------------------------------------------------------------------------------------
    // Fur
    col.a *= LIL_SAMPLE_2D(_FurMask, sampler_MainTex, uvMain).r;
    float furAlpha = LIL_SAMPLE_2D_ST(_FurNoiseMask, sampler_MainTex, input.uv).r;
    #if LIL_RENDER == 1
        furAlpha = saturate(furAlpha - input.furLayer * input.furLayer * input.furLayer * input.furLayer + 0.25);
        col.a *= furAlpha;
        col.rgb *= saturate(input.furLayer) * _FurAO * 2.0 + 1.0 - _FurAO;
    #else
        furAlpha = saturate(furAlpha - input.furLayer * input.furLayer * input.furLayer);
        col.a *= furAlpha;
        col.rgb *= (1.0-furAlpha) * _FurAO * 1.25 + 1.0 - _FurAO;
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Copy
    float3 albedo = col.rgb;

    //----------------------------------------------------------------------------------------------------------------------
    // Alpha
    #if LIL_RENDER == 1
        // Cutout
        col.a = saturate(col.a*5.0-2.0);
    #else
        // Transparent
        clip(col.a - 0.001);
    #endif

    #ifndef LIL_PASS_FORWARDADD
        //--------------------------------------------------------------------------------------------------------------------------
        // Normal
        float3 normalDirection = normalize(input.normalWS);

        //--------------------------------------------------------------------------------------------------------------------------
        // Lighting
        float shadowmix = 1.0;
        lilGetShading(col, shadowmix, albedo, uvMain, facing, normalDirection, 1, lightDirection, false);
        col.rgb *= lightColor + vertexLightColor + additionalLightColor;
        col.rgb = min(col.rgb, albedo);
    #else
        col.rgb *= lightColor;
    #endif

    //--------------------------------------------------------------------------------------------------------------------------
    // Fog
    LIL_APPLY_FOG(col, input.fogCoord);

    return col;
}

#endif
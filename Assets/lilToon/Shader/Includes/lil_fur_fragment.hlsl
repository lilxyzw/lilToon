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
    #if !defined(LIL_PASS_FORWARDADD)
        lightColor = lerp(lightColor, 1.0, _AsUnlit);
        vertexLightColor = lerp(vertexLightColor, 0.0, _AsUnlit);
        additionalLightColor = lerp(additionalLightColor, 0.0, _AsUnlit);
    #else
        lightColor = lerp(lightColor, 0.0, _AsUnlit);
    #endif

    float facing = 1.0;

    //--------------------------------------------------------------------------------------------------------------------------
    // UV
    #if defined(LIL_FEATURE_ANIMATE_MAIN_UV)
        float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
    #else
        float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);
    #endif

    //--------------------------------------------------------------------------------------------------------------------------
    // Main Color
    float4 col = _Color;
    if(Exists_MainTex) col *= LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain);

    //--------------------------------------------------------------------------------------------------------------------------
    // Fur
    if(Exists_FurMask) col.a *= LIL_SAMPLE_2D(_FurMask, sampler_MainTex, uvMain).r;
    float furAlpha = 1.0;
    if(Exists_FurNoiseMask) furAlpha = LIL_SAMPLE_2D_ST(_FurNoiseMask, sampler_MainTex, input.uv).r;
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
        clip(col.a - _Cutoff);
    #endif

    #ifndef LIL_PASS_FORWARDADD
        //--------------------------------------------------------------------------------------------------------------------------
        // Lighting
        #if defined(LIL_FEATURE_SHADOW)
            float3 normalDirection = normalize(input.normalWS);
            float shadowmix = 1.0;
            lilGetShading(col, shadowmix, albedo, lightColor, uvMain, facing, normalDirection, 1, lightDirection, false);
        #else
            col.rgb *= lightColor;
        #endif
        col.rgb += albedo * (vertexLightColor + additionalLightColor);
        col.rgb = min(col.rgb, albedo);
        col.rgb = max(col.rgb, albedo * _LightMinLimit);
    #else
        col.rgb *= lightColor;
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
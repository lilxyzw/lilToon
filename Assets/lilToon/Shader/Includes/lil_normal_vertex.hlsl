#ifndef LIL_VERTEX_INCLUDED
#define LIL_VERTEX_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Vertex shader
v2f vert(appdata input)
{
    v2f output;
    LIL_INITIALIZE_STRUCT(v2f, output);

    //----------------------------------------------------------------------------------------------------------------------
    // Invisible
    LIL_BRANCH
    if(_Invisible) return output;

    //----------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, output);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    #if defined(LIL_OUTLINE)
        float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);
        float outlineWidth = _OutlineWidth * 0.01;
        if(Exists_OutlineWidthMask) outlineWidth *= LIL_SAMPLE_2D_LOD(_OutlineWidthMask, sampler_MainTex, uvMain, 0).r;
        if(_OutlineVertexR2Width) outlineWidth *= input.color.r;
        if(_OutlineFixWidth) outlineWidth *= saturate(length(LIL_GET_VIEWDIR_WS(lilOptMul(LIL_MATRIX_M, input.positionOS.xyz).xyz)));
        input.positionOS.xyz += input.normalOS.xyz * outlineWidth;
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Copy
    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    #if defined(LIL_OUTLINE) || defined(LIL_SHOULD_NORMAL) && (defined(LIL_FUR) || !defined(LIL_SHOULD_TANGENT))
        LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
    #elif defined(LIL_SHOULD_NORMAL)
        LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, vertexNormalInput);
    #endif

    output.uv           = input.uv;
    output.positionCS   = vertexInput.positionCS;

    #if defined(LIL_OUTLINE)
        //--------------------------------------------------------------------------------------------------------------
        // Outline
        #if defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP)
            output.positionWS   = vertexInput.positionWS;
        #endif
        #if defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
            output.normalWS     = vertexNormalInput.normalWS;
        #endif
    #elif defined(LIL_FUR)
        //--------------------------------------------------------------------------------------------------------------
        // Fur
        #if !defined(LIL_BRP)
            output.positionWS   = vertexInput.positionWS;
            #if defined(LIL_SHOULD_NORMAL)
                output.normalWS     = vertexNormalInput.normalWS;
            #endif
        #elif defined(LIL_PASS_FORWARDADD)
            output.positionWS   = vertexInput.positionWS;
        #else
            output.uv           = input.uv;
            output.positionCS   = vertexInput.positionCS;
            #if defined(LIL_FEATURE_DISTANCE_FADE)
                output.positionWS   = vertexInput.positionWS;
            #endif
            #if defined(LIL_SHOULD_NORMAL)
                output.normalWS     = vertexNormalInput.normalWS;
            #endif
        #endif
    #else
        //--------------------------------------------------------------------------------------------------------------
        // Normal
        #if defined(LIL_SHOULD_POSITION_WS)
            output.positionWS   = vertexInput.positionWS;
        #endif
        #if defined(LIL_SHOULD_NORMAL)
            output.normalWS     = vertexNormalInput.normalWS;
        #endif
        #if defined(LIL_SHOULD_TBN)
            output.tangentWS    = vertexNormalInput.tangentWS;
            output.bitangentWS  = vertexNormalInput.bitangentWS;
        #endif
        #if defined(LIL_SHOULD_TANGENT_W)
            output.tangentW     = input.tangentOS.w;
        #endif
        #if defined(LIL_REFRACTION) && !defined(LIL_PASS_FORWARDADD)
            output.positionSS = vertexInput.positionSS;
        #endif
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Clipping Canceller
    #if defined(LIL_FEATURE_CLIPPING_CANCELLER)
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            if(output.positionCS.w < _ProjectionParams.y * 1.01 && output.positionCS.w > 0) output.positionCS.z = output.positionCS.z * 0.0001 + output.positionCS.w * 0.999;
        #else
            // OpenGL
            if(output.positionCS.w < _ProjectionParams.y * 1.01 && output.positionCS.w > 0) output.positionCS.z = output.positionCS.z * 0.0001 - output.positionCS.w * 0.999;
        #endif
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Fog & Lightmap & Vertex light
    LIL_TRANSFER_SHADOW(vertexInput, input.uv1, output);
    LIL_TRANSFER_FOG(vertexInput, output);
    LIL_TRANSFER_LIGHTMAPUV(input.uv1, output);
    LIL_CALC_VERTEXLIGHT(vertexInput, output);

    return output;
}

#endif
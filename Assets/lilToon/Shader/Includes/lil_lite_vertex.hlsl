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
        outlineWidth *= LIL_SAMPLE_2D_LOD(_OutlineWidthMask, sampler_MainTex, uvMain, 0).r;
        if(_OutlineVertexR2Width) outlineWidth *= input.color.r;
        if(_OutlineFixWidth) outlineWidth *= saturate(length(LIL_GET_VIEWDIR_WS(lilOptMul(LIL_MATRIX_M, input.positionOS.xyz).xyz)));
        input.positionOS.xyz += input.normalOS.xyz * outlineWidth;
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Copy
    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);

    #if defined(LIL_OUTLINE)
        //--------------------------------------------------------------------------------------------------------------
        // Outline
        output.uv           = input.uv;
        output.positionCS   = vertexInput.positionCS;
        #if defined(LIL_PASS_FORWARDADD) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
            output.positionWS   = vertexInput.positionWS;
        #endif
        #if defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
            output.normalWS     = vertexNormalInput.normalWS;
        #endif
    #else
        //--------------------------------------------------------------------------------------------------------------
        // Lite
        output.uv           = input.uv;
        output.positionWS   = vertexInput.positionWS;
        output.positionCS   = vertexInput.positionCS;
        output.normalWS     = vertexNormalInput.normalWS;
        output.uvMat        = lilCalcMatCapUV(vertexNormalInput.normalWS, _MatCapZRotCancel);
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Fog & Lightmap & Vertex light
    LIL_CALC_MAINLIGHT(vertexInput, output);
    LIL_TRANSFER_FOG(vertexInput, output);
    LIL_TRANSFER_LIGHTMAPUV(input.uv1, output);
    LIL_CALC_VERTEXLIGHT(vertexInput, output);

    return output;
}

#endif
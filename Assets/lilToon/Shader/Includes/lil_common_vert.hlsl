#ifndef LIL_VERTEX_INCLUDED
#define LIL_VERTEX_INCLUDED

#if defined(LIL_V2F_OUT_BASE)
    #undef LIL_V2F_OUT_BASE
#endif
#if defined(LIL_V2F_OUT)
    #undef LIL_V2F_OUT
#endif
#if defined(LIL_V2F_TYPE)
    #undef LIL_V2F_TYPE
#endif

#if defined(LIL_ONEPASS_OUTLINE)
    #define LIL_V2F_OUT_BASE output.base
    #define LIL_V2F_OUT output
    #define LIL_V2F_TYPE v2g
#else
    #define LIL_V2F_OUT_BASE output
    #define LIL_V2F_OUT output
    #define LIL_V2F_TYPE v2f
#endif

#if !defined(LIL_CUSTOM_VERT_COPY)
    #define LIL_CUSTOM_VERT_COPY
#endif

void lilCustomVertexOS(inout appdata input, inout float2 uvMain, inout float4 positionOS)
{
    #if defined(LIL_CUSTOM_VERTEX_OS)
        LIL_CUSTOM_VERTEX_OS
    #endif
}

void lilCustomVertexOS(inout appdata input, inout float2 uvMain, inout float3 positionOS)
{
    float4 positionOS4 = float4(positionOS, 1.0);
    lilCustomVertexOS(input, uvMain, positionOS4);
    positionOS = positionOS4.xyz;
}

void lilCustomVertexWS(inout appdata input, inout float2 uvMain, inout lilVertexPositionInputs vertexInput, inout lilVertexNormalInputs vertexNormalInput)
{
    #if defined(LIL_CUSTOM_VERTEX_WS)
        LIL_CUSTOM_VERTEX_WS
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Vertex shader
LIL_V2F_TYPE vert(appdata input)
{
    LIL_V2F_TYPE LIL_V2F_OUT;
    LIL_INITIALIZE_STRUCT(v2f, LIL_V2F_OUT_BASE);
    #if defined(LIL_ONEPASS_OUTLINE)
        LIL_V2F_OUT.positionCSOL = 0.0;
        #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
            LIL_V2F_OUT.previousPositionCSOL = 0.0;
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    #if defined(LIL_TESSELLATION)
        LIL_BRANCH
        if(!_Invisible)
        {
    #else
        // In the tessellation shader this gives a warning
        LIL_BRANCH
        if(_Invisible) return LIL_V2F_OUT;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, LIL_V2F_OUT_BASE);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(LIL_V2F_OUT_BASE);

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv0, _MainTex_ST);

    //------------------------------------------------------------------------------------------------------------------------------
    // Object space direction
    #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
        float3 bitangentOS = cross(input.normalOS, input.tangentOS.xyz) * (input.tangentOS.w * LIL_NEGATIVE_SCALE);
        float3x3 tbnOS = float3x3(input.tangentOS.xyz, bitangentOS, input.normalOS);
    #else
        float3 bitangentOS = 0.0;
        float3x3 tbnOS = 0.0;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Vertex Modification
    #include "Includes/lil_vert_encryption.hlsl"
    lilCustomVertexOS(input, uvMain, input.positionOS);
    #include "Includes/lil_vert_audiolink.hlsl"
    #if !defined(LIL_ONEPASS_OUTLINE)
        #include "Includes/lil_vert_outline.hlsl"
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Previous Position
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        input.previousPositionOS = unity_MotionVectorsParams.x > 0.0 ? input.previousPositionOS : input.positionOS.xyz;
        #if defined(_ADD_PRECOMPUTED_VELOCITY)
            input.previousPositionOS -= input.precomputedVelocity;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Vertex Modification
        #define LIL_MODIFY_PREVPOS
        #include "Includes/lil_vert_encryption.hlsl"
        lilCustomVertexOS(input, uvMain, input.previousPositionOS);
        #include "Includes/lil_vert_audiolink.hlsl"
        #undef LIL_MODIFY_PREVPOS

        //------------------------------------------------------------------------------------------------------------------------------
        // Transform
        LIL_VERTEX_POSITION_INPUTS(input.previousPositionOS, previousVertexInput);
        #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
            LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, previousVertexNormalInput);
        #elif defined(LIL_APP_NORMAL)
            LIL_VERTEX_NORMAL_INPUTS(input.normalOS, previousVertexNormalInput);
        #else
            lilVertexNormalInputs previousVertexNormalInput = lilGetVertexNormalInputs();
        #endif
        previousVertexInput.positionWS = TransformPreviousObjectToWorld(input.previousPositionOS);
        lilCustomVertexWS(input, uvMain, previousVertexInput, previousVertexNormalInput);
        LIL_V2F_OUT_BASE.previousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(previousVertexInput.positionWS, 1.0));

        #if defined(LIL_ONEPASS_OUTLINE)
            #define LIL_MODIFY_PREVPOS
            #include "Includes/lil_vert_outline.hlsl"
            #undef LIL_MODIFY_PREVPOS
            LIL_VERTEX_POSITION_INPUTS(input.previousPositionOS, previousOLVertexInput);
            previousOLVertexInput.positionWS = TransformPreviousObjectToWorld(input.previousPositionOS);
            lilCustomVertexWS(input, uvMain, previousOLVertexInput, previousVertexNormalInput);
            LIL_V2F_OUT.previousPositionCSOL = mul(UNITY_MATRIX_PREV_VP, float4(previousOLVertexInput.positionWS, 1.0));
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Transform
    #if defined(LIL_APP_POSITION)
        LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    #endif
    #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
        LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, vertexNormalInput);
    #elif defined(LIL_APP_NORMAL)
        LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
    #else
        lilVertexNormalInputs vertexNormalInput = lilGetVertexNormalInputs();
    #endif
    lilCustomVertexWS(input, uvMain, vertexInput, vertexNormalInput);
    #if defined(LIL_CUSTOM_VERTEX_WS)
        LIL_RE_VERTEX_POSITION_INPUTS(vertexInput);
    #endif
    float3 viewDirection = normalize(lilViewDirection(lilToAbsolutePositionWS(vertexInput.positionWS)));
    float3 headDirection = normalize(lilHeadDirection(lilToAbsolutePositionWS(vertexInput.positionWS)));

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy

    // UV
    #if defined(LIL_V2F_TEXCOORD0)
        LIL_V2F_OUT_BASE.uv0            = input.uv0;
    #endif
    #if defined(LIL_V2F_TEXCOORD1)
        LIL_V2F_OUT_BASE.uv1            = input.uv1;
    #endif
    #if defined(LIL_V2F_TEXCOORD2)
        LIL_V2F_OUT_BASE.uv2            = input.uv2;
    #endif
    #if defined(LIL_V2F_TEXCOORD3)
        LIL_V2F_OUT_BASE.uv3            = input.uv3;
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD01)
        LIL_V2F_OUT_BASE.uv01.xy        = input.uv0;
        LIL_V2F_OUT_BASE.uv01.zw        = input.uv1;
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD23)
        LIL_V2F_OUT_BASE.uv23.xy        = input.uv2;
        LIL_V2F_OUT_BASE.uv23.zw        = input.uv3;
    #endif
    #if defined(LIL_V2F_UVMAT)
        LIL_V2F_OUT_BASE.uvMat          = lilCalcMatCapUV(input.uv1, vertexNormalInput.normalWS, viewDirection, headDirection, _MatCapTex_ST, _MatCapBlendUV1.xy, _MatCapZRotCancel, _MatCapPerspective, _MatCapVRParallaxStrength);
    #endif

    // Position
    #if defined(LIL_V2F_POSITION_CS)
        LIL_V2F_OUT_BASE.positionCS     = vertexInput.positionCS;
    #endif
    #if defined(LIL_V2F_POSITION_OS)
        LIL_V2F_OUT_BASE.positionOS     = input.positionOS.xyz;
    #endif
    #if defined(LIL_V2F_POSITION_WS)
        LIL_V2F_OUT_BASE.positionWS     = vertexInput.positionWS;
    #endif

    // Normal
    #if defined(LIL_V2F_NORMAL_WS) && defined(LIL_NORMALIZE_NORMAL_IN_VS) && !defined(SHADER_QUALITY_LOW)
        LIL_V2F_OUT_BASE.normalWS       = normalize(vertexNormalInput.normalWS);
    #elif defined(LIL_V2F_NORMAL_WS)
        LIL_V2F_OUT_BASE.normalWS       = vertexNormalInput.normalWS;
    #endif
    #if defined(LIL_V2F_TANGENT_WS)
        LIL_V2F_OUT_BASE.tangentWS      = float4(vertexNormalInput.tangentWS, input.tangentOS.w);
    #endif

    LIL_CUSTOM_VERT_COPY

    //------------------------------------------------------------------------------------------------------------------------------
    // Meta
    #if defined(LIL_PASS_META_INCLUDED) && !defined(LIL_HDRP)
        LIL_TRANSFER_METAPASS(input,LIL_V2F_OUT_BASE);
        #if defined(EDITOR_VISUALIZATION)
            if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
                LIL_V2F_OUT_BASE.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, input.uv0, input.uv1, input.uv2, unity_EditorViz_Texture_ST);
            else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
            {
                LIL_V2F_OUT_BASE.vizUV = input.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
                LIL_V2F_OUT_BASE.lightCoord = mul(unity_EditorViz_WorldToLight, LIL_TRANSFORM_POS_OS_TO_WS(input.positionOS.xyz));
            }
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog & Lighting
    lilFragData fd = lilInitFragData();
    LIL_GET_HDRPDATA(vertexInput,fd);
    #if defined(LIL_V2F_LIGHTCOLOR) || defined(LIL_V2F_LIGHTDIRECTION) || defined(LIL_V2F_INDLIGHTCOLOR)
        LIL_CALC_MAINLIGHT(vertexInput, lightdataInput);
    #endif
    #if defined(LIL_V2F_LIGHTCOLOR)
        LIL_V2F_OUT_BASE.lightColor     = lightdataInput.lightColor;
    #endif
    #if defined(LIL_V2F_LIGHTDIRECTION)
        LIL_V2F_OUT_BASE.lightDirection = lightdataInput.lightDirection;
    #endif
    #if defined(LIL_V2F_INDLIGHTCOLOR)
        LIL_V2F_OUT_BASE.indLightColor  = lightdataInput.indLightColor * _ShadowEnvStrength;
    #endif
    #if defined(LIL_V2F_SHADOW)
        LIL_TRANSFER_SHADOW(vertexInput, input.uv1, LIL_V2F_OUT_BASE);
    #endif
    #if defined(LIL_V2F_VERTEXLIGHT_FOG)
        LIL_TRANSFER_FOG(vertexInput, LIL_V2F_OUT_BASE);
        LIL_CALC_VERTEXLIGHT(vertexInput, LIL_V2F_OUT_BASE);
    #endif
    #if defined(LIL_V2F_SHADOW_CASTER)
        LIL_TRANSFER_SHADOW_CASTER(input, LIL_V2F_OUT_BASE);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Clipping Canceller
    #if defined(LIL_V2F_POSITION_CS) && defined(LIL_FEATURE_CLIPPING_CANCELLER) && !defined(LIL_LITE) && !defined(LIL_PASS_SHADOWCASTER_INCLUDED) && !defined(LIL_PASS_META_INCLUDED)
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            if(LIL_V2F_OUT_BASE.positionCS.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT_BASE.positionCS.w > 0)
            {
                LIL_V2F_OUT_BASE.positionCS.z = LIL_V2F_OUT_BASE.positionCS.z * 0.0001 + LIL_V2F_OUT_BASE.positionCS.w * 0.999;
            }
        #else
            // OpenGL
            if(LIL_V2F_OUT_BASE.positionCS.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT_BASE.positionCS.w > 0)
            {
                LIL_V2F_OUT_BASE.positionCS.z = LIL_V2F_OUT_BASE.positionCS.z * 0.0001 - LIL_V2F_OUT_BASE.positionCS.w * 0.999;
            }
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // One Pass Outline
    #if defined(LIL_ONEPASS_OUTLINE) && (!defined(LIL_MULTI) || defined(LIL_MULTI) && defined(LIL_MULTI_OUTLINE))
        #include "Includes/lil_vert_outline.hlsl"
        vertexInput = lilGetVertexPositionInputs(input.positionOS);
        lilCustomVertexWS(input, uvMain, vertexInput, vertexNormalInput);
        #if defined(LIL_CUSTOM_VERTEX_WS)
            LIL_RE_VERTEX_POSITION_INPUTS(vertexInput);
        #endif
        LIL_V2F_OUT.positionCSOL = vertexInput.positionCS;

        //------------------------------------------------------------------------------------------------------------------------------
        // Clipping Canceller
        #if defined(LIL_FEATURE_CLIPPING_CANCELLER) && !defined(LIL_LITE) && !defined(LIL_PASS_SHADOWCASTER_INCLUDED) && !defined(LIL_PASS_META_INCLUDED)
            #if defined(UNITY_REVERSED_Z)
                // DirectX
                if(LIL_V2F_OUT.positionCSOL.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT.positionCSOL.w > 0)
                {
                    LIL_V2F_OUT.positionCSOL.z = LIL_V2F_OUT.positionCSOL.z * 0.0001 + LIL_V2F_OUT.positionCSOL.w * 0.999;
                }
            #else
                // OpenGL
                if(LIL_V2F_OUT.positionCSOL.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT.positionCSOL.w > 0)
                {
                    LIL_V2F_OUT.positionCSOL.z = LIL_V2F_OUT.positionCSOL.z * 0.0001 - LIL_V2F_OUT.positionCSOL.w * 0.999;
                }
            #endif
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Offset z for Less ZTest
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            LIL_V2F_OUT.positionCSOL.z -= 0.0001;
        #else
            // OpenGL
            LIL_V2F_OUT.positionCSOL.z += 0.0001;
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Offset z for Less ZTest
    #if defined(SHADERPASS) && SHADERPASS == SHADERPASS_DEPTH_ONLY && defined(LIL_OUTLINE) && (!defined(LIL_MULTI) || defined(LIL_MULTI) && defined(LIL_MULTI_OUTLINE))
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            LIL_V2F_OUT.positionCS.z -= 0.0001;
        #else
            // OpenGL
            LIL_V2F_OUT.positionCS.z += 0.0001;
        #endif
    #endif

    #if defined(LIL_TESSELLATION)
        }
    #endif

    return LIL_V2F_OUT;
}

#if defined(LIL_ONEPASS_OUTLINE)
    [maxvertexcount(12)]
    void geom(triangle v2g input[3], inout TriangleStream<v2f> outStream)
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Invisible
        LIL_BRANCH
        if(_Invisible) return;

        v2f output[3];
        LIL_INITIALIZE_STRUCT(v2f, output[0]);
        LIL_INITIALIZE_STRUCT(v2f, output[1]);
        LIL_INITIALIZE_STRUCT(v2f, output[2]);

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        for(uint i = 0; i < 3; i++)
        {
            output[i] = input[i].base;
        }
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0].base);

        // Front
        LIL_BRANCH
        if(_Cull != 1)
        {
            outStream.Append(output[0]);
            outStream.Append(output[1]);
            outStream.Append(output[2]);
            outStream.RestartStrip();
        }

        // Back
        LIL_BRANCH
        if(_Cull != 2)
        {
            outStream.Append(output[2]);
            outStream.Append(output[1]);
            outStream.Append(output[0]);
            outStream.RestartStrip();
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Outline
        #if !defined(LIL_MULTI) || defined(LIL_MULTI) && defined(LIL_MULTI_OUTLINE)
            for(uint j = 0; j < 3; j++)
            {
                output[j].positionCS = input[j].positionCSOL;
                #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
                    output[j].previousPositionCS = input[j].previousPositionCSOL;
                #endif
            }

            // Front
            LIL_BRANCH
            if(_OutlineCull != 1)
            {
                outStream.Append(output[0]);
                outStream.Append(output[1]);
                outStream.Append(output[2]);
                outStream.RestartStrip();
            }

            // Back
            LIL_BRANCH
            if(_OutlineCull != 2)
            {
                outStream.Append(output[2]);
                outStream.Append(output[1]);
                outStream.Append(output[0]);
                outStream.RestartStrip();
            }
        #endif
    }
#endif

#undef LIL_V2F_OUT_BASE
#undef LIL_V2F_OUT

#endif
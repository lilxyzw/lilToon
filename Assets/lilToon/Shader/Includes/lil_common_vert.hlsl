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

//------------------------------------------------------------------------------------------------------------------------------
// Insert a process for a custom shader
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
    bool dissolveActive = true;
    bool dissolveInvert = false;
    
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
    #if defined(LIL_OUTLINE) && !defined(LIL_LITE) && !defined(LIL_PASS_SHADOWCASTER) && defined(USING_STEREO_MATRICES)
        #define LIL_VERTEX_CONDITION (_Invisible || _OutlineDisableInVR)
    #elif defined(LIL_OUTLINE) && !defined(LIL_LITE) && !defined(LIL_PASS_SHADOWCASTER)
        #define LIL_VERTEX_CONDITION (_Invisible || _OutlineDisableInVR && (abs(LIL_MATRIX_P._m02) > 0.000001))
    #else
        #define LIL_VERTEX_CONDITION (_Invisible)
    #endif

    #if !defined(SHADER_STAGE_VERTEX) || defined(LIL_CUSTOM_SAFEVERT)
        if(!LIL_VERTEX_CONDITION)
        {
    #else
        if(LIL_VERTEX_CONDITION) return LIL_V2F_OUT;
    #endif

    #undef LIL_VERTEX_CONDITION
    
    //------------------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, LIL_V2F_OUT_BASE);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(LIL_V2F_OUT_BASE);

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv0, _MainTex_ST);
    float2 uvs[4] = {uvMain,input.uv1,input.uv2,input.uv3};

    //------------------------------------------------------------------------------------------------------------------------------
    // Object space direction
    #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
        float3 bitangentOS = normalize(cross(input.normalOS, input.tangentOS.xyz)) * (input.tangentOS.w * length(input.normalOS));
        float3x3 tbnOS = float3x3(input.tangentOS.xyz, bitangentOS, input.normalOS);
    #else
        float3 bitangentOS = 0.0;
        float3x3 tbnOS = 0.0;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Vertex Modification
    #include "lil_vert_encryption.hlsl"
    lilCustomVertexOS(input, uvMain, input.positionOS);
    #include "lil_vert_audiolink.hlsl"
    #if !defined(LIL_ONEPASS_OUTLINE)
        #include "lil_vert_outline.hlsl"
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Previous Position (for HDRP)
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        input.previousPositionOS = lilSelectPreviousPosition(input.previousPositionOS, input.positionOS.xyz);
        #if defined(_ADD_PRECOMPUTED_VELOCITY)
            input.previousPositionOS -= input.precomputedVelocity;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Vertex Modification
        #define LIL_MODIFY_PREVPOS
        #include "lil_vert_encryption.hlsl"
        lilCustomVertexOS(input, uvMain, input.previousPositionOS);
        #include "lil_vert_audiolink.hlsl"
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
        previousVertexInput.positionWS = lilTransformPreviousObjectToWorld(input.previousPositionOS.xyz);
        lilCustomVertexWS(input, uvMain, previousVertexInput, previousVertexNormalInput);
        LIL_V2F_OUT_BASE.previousPositionCS = mul(LIL_MATRIX_PREV_VP, float4(previousVertexInput.positionWS, 1.0));

        #if defined(LIL_ONEPASS_OUTLINE)
            #define LIL_MODIFY_PREVPOS
            #include "lil_vert_outline.hlsl"
            #undef LIL_MODIFY_PREVPOS
            LIL_VERTEX_POSITION_INPUTS(input.previousPositionOS, previousOLVertexInput);
            previousOLVertexInput.positionWS = lilTransformPreviousObjectToWorld(input.previousPositionOS.xyz);
            lilCustomVertexWS(input, uvMain, previousOLVertexInput, previousVertexNormalInput);
            LIL_V2F_OUT.previousPositionCSOL = mul(LIL_MATRIX_PREV_VP, float4(previousOLVertexInput.positionWS, 1.0));
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
        LIL_V2F_OUT_BASE.positionOSdissolve.xyz = input.positionOS.xyz;
    #endif
    #if defined(LIL_V2F_POSITION_WS)
        LIL_V2F_OUT_BASE.positionWS     = vertexInput.positionWS;
    #endif
    #if defined(LIL_V2F_POSITION_CS_NO_JITTER)
        LIL_V2F_OUT_BASE.positionCSNoJitter = mul(_NonJitteredViewProjMatrix, float4(vertexInput.positionWS.xyz, 1));
        #if defined(LIL_V2F_POSITION_CS)
            lilApplyMotionVectorZBias(LIL_V2F_OUT_BASE.positionCS);
        #endif
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
                LIL_V2F_OUT_BASE.lightCoord = mul(unity_EditorViz_WorldToLight, float4(lilTransformOStoWS(input.positionOS.xyz), 1.0));
            }
        #endif
    #endif
    
    //------------------------------------------------------------------------------------------------------------------------------
    // Fog & Lighting
    lilFragData fd = lilInitFragData();
    LIL_GET_HDRPDATA(vertexInput,fd);
    #if defined(LIL_V2F_LIGHTCOLOR) || defined(LIL_V2F_LIGHTDIRECTION) || defined(LIL_V2F_INDLIGHTCOLOR) || defined(LIL_V2F_NDOTL)
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
    #if defined(LIL_V2F_NDOTL)
        float2 outlineNormalVS = normalize(lilTransformDirWStoVSCenter(vertexNormalInput.normalWS).xy);
        #if defined(LIL_PASS_FORWARDADD)
            float2 outlineLightVS = normalize(lilTransformDirWStoVSCenter(normalize(_WorldSpaceLightPos0.xyz - vertexInput.positionWS * _WorldSpaceLightPos0.w)).xy);
        #else
            float2 outlineLightVS = normalize(lilTransformDirWStoVSCenter(lightdataInput.lightDirection).xy);
        #endif
        LIL_V2F_OUT_BASE.NdotL          = dot(outlineNormalVS, outlineLightVS) * 0.5 + 0.5;
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
            if(LIL_V2F_OUT_BASE.positionCS.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT_BASE.positionCS.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
            {
                LIL_V2F_OUT_BASE.positionCS.z = LIL_V2F_OUT_BASE.positionCS.z * 0.0001 + LIL_V2F_OUT_BASE.positionCS.w * 0.999;
            }
        #else
            // OpenGL
            if(LIL_V2F_OUT_BASE.positionCS.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT_BASE.positionCS.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
            {
                LIL_V2F_OUT_BASE.positionCS.z = LIL_V2F_OUT_BASE.positionCS.z * 0.0001 - LIL_V2F_OUT_BASE.positionCS.w * 0.999;
            }
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // One Pass Outline (for HDRP)
    #if defined(LIL_ONEPASS_OUTLINE)
        #include "lil_vert_outline.hlsl"
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
                if(LIL_V2F_OUT.positionCSOL.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT.positionCSOL.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
                {
                    LIL_V2F_OUT.positionCSOL.z = LIL_V2F_OUT.positionCSOL.z * 0.0001 + LIL_V2F_OUT.positionCSOL.w * 0.999;
                }
            #else
                // OpenGL
                if(LIL_V2F_OUT.positionCSOL.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT.positionCSOL.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
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
    #if defined(SHADERPASS) && SHADERPASS == SHADERPASS_DEPTH_ONLY && defined(LIL_OUTLINE)
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            LIL_V2F_OUT_BASE.positionCS.z -= 0.0001;
        #else
            // OpenGL
            LIL_V2F_OUT_BASE.positionCS.z += 0.0001;
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Remove Outline
    #if defined(LIL_ONEPASS_OUTLINE) && defined(LIL_PASS_FORWARD_INCLUDED)
        float width = lilGetOutlineWidth(uvMain, input.color, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width LIL_SAMP_IN(lil_sampler_linear_repeat));
        if(width > -0.000001 && width < 0.000001 && _OutlineDeleteMesh) LIL_V2F_OUT.positionCSOL = 0.0/0.0;
    #elif defined(LIL_OUTLINE) && defined(LIL_PASS_FORWARD_INCLUDED)
        float width = lilGetOutlineWidth(uvMain, input.color, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width LIL_SAMP_IN(lil_sampler_linear_repeat));
        if(width > -0.000001 && width < 0.000001 && _OutlineDeleteMesh) LIL_V2F_OUT_BASE.positionCS = 0.0/0.0;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // IDMask
    #if defined(LIL_FEATURE_IDMASK) && !defined(LIL_NOT_SUPPORT_VERTEXID) && !defined(LIL_LITE)
        int idMaskIndices[8] = {_IDMaskIndex1,_IDMaskIndex2,_IDMaskIndex3,_IDMaskIndex4,_IDMaskIndex5,_IDMaskIndex6,_IDMaskIndex7,_IDMaskIndex8};
        float idMaskFlags[8] = {_IDMask1,_IDMask2,_IDMask3,_IDMask4,_IDMask5,_IDMask6,_IDMask7,_IDMask8};
        float idMaskPriorFlags[8] = {_IDMaskPrior1,_IDMaskPrior2,_IDMaskPrior3,_IDMaskPrior4,_IDMaskPrior5,_IDMaskPrior6,_IDMaskPrior7,_IDMaskPrior8};
        uint idMaskArg = 0;
        switch(_IDMaskFrom)
        {
            #if defined(LIL_APP_TEXCOORD0)
                case 0: idMaskArg = input.uv0.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD1)
                case 1: idMaskArg = input.uv1.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD2)
                case 2: idMaskArg = input.uv2.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD3)
                case 3: idMaskArg = input.uv3.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD4)
                case 4: idMaskArg = input.uv4.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD5)
                case 5: idMaskArg = input.uv5.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD6)
                case 6: idMaskArg = input.uv6.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD7)
                case 7: idMaskArg = input.uv7.x; break;
            #endif
            default: idMaskArg = input.vertexID; break;
        }
        bool idMasked = IDMask(idMaskArg,_IDMaskIsBitmap,idMaskIndices,idMaskFlags);
        if(_IDMaskControlsDissolve)
        {
            bool priorIdMasked = IDMask(idMaskArg, _IDMaskIsBitmap, idMaskIndices, idMaskPriorFlags);
            dissolveActive = idMasked != priorIdMasked;
            dissolveInvert = priorIdMasked;
            idMasked = idMasked && priorIdMasked;
        }
        #if defined(LIL_V2F_POSITION_CS)
            LIL_V2F_OUT_BASE.positionCS = idMasked ? 0.0/0.0 : LIL_V2F_OUT_BASE.positionCS;
        #endif
        #if defined(LIL_ONEPASS_OUTLINE)
            LIL_V2F_OUT.positionCSOL = idMasked ? 0.0/0.0 : LIL_V2F_OUT.positionCSOL;
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // UDIM Discard
    #if defined(LIL_FEATURE_UDIMDISCARD) && !defined(LIL_LITE)
    if(_UDIMDiscardMode == 0 && _UDIMDiscardCompile == 1 && LIL_CHECK_UDIMDISCARD(input)) // Discard Vertices instead of just pixels
    {
        #if defined(LIL_V2F_POSITION_CS)
        LIL_V2F_OUT_BASE.positionCS = 0.0/0.0;
        #endif
        #if defined(LIL_ONEPASS_OUTLINE)
        LIL_V2F_OUT.positionCSOL = 0.0/0.0;
        #endif
    }
    #endif

    #if defined(LIL_V2F_POSITION_OS)
    LIL_V2F_OUT_BASE.positionOSdissolve.w = (dissolveActive | (dissolveInvert << 1));
    #endif
    
    #if !defined(SHADER_STAGE_VERTEX) || defined(LIL_CUSTOM_SAFEVERT)
        }
    #endif
    
    return LIL_V2F_OUT;
}

//------------------------------------------------------------------------------------------------------------------------------
// Geometry shader (for HDRP)
#if defined(LIL_ONEPASS_OUTLINE)
    [maxvertexcount(12)]
    void geom(triangle v2g input[3], inout TriangleStream<v2f> outStream)
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Invisible
        if(_Invisible) return;

        v2f output[3];
        LIL_INITIALIZE_STRUCT(v2f, output[0]);
        LIL_INITIALIZE_STRUCT(v2f, output[1]);
        LIL_INITIALIZE_STRUCT(v2f, output[2]);

        LIL_SETUP_INSTANCE_ID(input[0].base);
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0].base);

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        for(uint i = 0; i < 3; i++)
        {
            output[i] = input[i].base;
        }

        // Front
        if(_Cull != 1)
        {
            outStream.Append(output[0]);
            outStream.Append(output[1]);
            outStream.Append(output[2]);
            outStream.RestartStrip();
        }

        // Back
        if(_Cull != 2)
        {
            outStream.Append(output[2]);
            outStream.Append(output[1]);
            outStream.Append(output[0]);
            outStream.RestartStrip();
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Outline
        for(uint j = 0; j < 3; j++)
        {
            output[j].positionCS = input[j].positionCSOL;
            #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
                output[j].previousPositionCS = input[j].previousPositionCSOL;
            #endif
        }

        // Front
        if(_OutlineCull != 1)
        {
            outStream.Append(output[0]);
            outStream.Append(output[1]);
            outStream.Append(output[2]);
            outStream.RestartStrip();
        }

        // Back
        if(_OutlineCull != 2)
        {
            outStream.Append(output[2]);
            outStream.Append(output[1]);
            outStream.Append(output[0]);
            outStream.RestartStrip();
        }
    }
#endif

#undef LIL_V2F_OUT_BASE
#undef LIL_V2F_OUT

#endif
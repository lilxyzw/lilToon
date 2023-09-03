#ifndef LIL_TESSELLATION_INCLUDED
#define LIL_TESSELLATION_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Tessellation

//------------------------------------------------------------------------------------------------------------------------------
// Structure
struct lilTessellationFactors {
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

//------------------------------------------------------------------------------------------------------------------------------
// Vertex Shader
appdataCopy vertTess(appdata input)
{
    return input;
}

//------------------------------------------------------------------------------------------------------------------------------
// Hull Shader
[domain("tri")]
[partitioning("integer")]
[outputtopology("triangle_cw")]
[patchconstantfunc("hullConst")]
[outputcontrolpoints(3)]
appdataCopy hull(InputPatch<appdataCopy, 3> input, uint id : SV_OutputControlPointID)
{
    return input[id];
}

lilTessellationFactors hullConst(InputPatch<appdataCopy, 3> input)
{
    lilTessellationFactors output;
    LIL_INITIALIZE_STRUCT(lilTessellationFactors, output);
    LIL_SETUP_INSTANCE_ID(input[0]);

    if(_Invisible) return output;

    LIL_VERTEX_POSITION_INPUTS(input[0].positionOS, vertexInput_0);
    LIL_VERTEX_POSITION_INPUTS(input[1].positionOS, vertexInput_1);
    LIL_VERTEX_POSITION_INPUTS(input[2].positionOS, vertexInput_2);
    LIL_VERTEX_NORMAL_INPUTS(input[0].normalOS, vertexNormalInput_0);
    LIL_VERTEX_NORMAL_INPUTS(input[1].normalOS, vertexNormalInput_1);
    LIL_VERTEX_NORMAL_INPUTS(input[2].normalOS, vertexNormalInput_2);

    float4 tessFactor;
    tessFactor.x = lilCalcEdgeTessFactor(vertexInput_1.positionWS, vertexInput_2.positionWS, _TessEdge);
    tessFactor.y = lilCalcEdgeTessFactor(vertexInput_2.positionWS, vertexInput_0.positionWS, _TessEdge);
    tessFactor.z = lilCalcEdgeTessFactor(vertexInput_0.positionWS, vertexInput_1.positionWS, _TessEdge);
    tessFactor.xyz = min(tessFactor.xyz, _TessFactorMax);

    // Rim
    float3 nv = float3(abs(dot(vertexNormalInput_0.normalWS, normalize(lilViewDirection(lilToAbsolutePositionWS(vertexInput_0.positionWS))))),
                       abs(dot(vertexNormalInput_1.normalWS, normalize(lilViewDirection(lilToAbsolutePositionWS(vertexInput_1.positionWS))))),
                       abs(dot(vertexNormalInput_2.normalWS, normalize(lilViewDirection(lilToAbsolutePositionWS(vertexInput_2.positionWS))))));
    nv = saturate(1.0 - float3(nv.y + nv.z, nv.z + nv.x, nv.x + nv.y) * 0.5);
    tessFactor.xyz = max(tessFactor.xyz * nv * nv, 1.0);
    tessFactor.w = (tessFactor.x+tessFactor.y+tessFactor.z) / 3.0;

    // Cull out of screen
    float4 pos[3] = {vertexInput_0.positionCS, vertexInput_1.positionCS, vertexInput_2.positionCS};
    pos[0].xy = pos[0].xy/abs(pos[0].w);
    pos[1].xy = pos[1].xy/abs(pos[1].w);
    pos[2].xy = pos[2].xy/abs(pos[2].w);
    tessFactor = (pos[0].x >  1.01 && pos[1].x >  1.01 && pos[2].x >  1.01) ||
                 (pos[0].x < -1.01 && pos[1].x < -1.01 && pos[2].x < -1.01) ||
                 (pos[0].y >  1.01 && pos[1].y >  1.01 && pos[2].y >  1.01) ||
                 (pos[0].y < -1.01 && pos[1].y < -1.01 && pos[2].y < -1.01) ? 0.0 : tessFactor;

    output.edge[0] = tessFactor.x;
    output.edge[1] = tessFactor.y;
    output.edge[2] = tessFactor.z;
    output.inside  = tessFactor.w;
        
    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Domain Shader
[domain("tri")]
#if defined(LIL_ONEPASS_OUTLINE)
v2g domain(lilTessellationFactors hsConst, const OutputPatch<appdataCopy, 3> input, float3 bary : SV_DomainLocation)
#else
v2f domain(lilTessellationFactors hsConst, const OutputPatch<appdataCopy, 3> input, float3 bary : SV_DomainLocation)
#endif
{
    appdata output;
    LIL_INITIALIZE_STRUCT(appdata, output);
    LIL_TRANSFER_INSTANCE_ID(input[0], output);

    #if defined(LIL_APP_POSITION)
        LIL_TRI_INTERPOLATION(input,output,bary,positionOS);
    #endif
    #if defined(LIL_APP_TEXCOORD0)
        LIL_TRI_INTERPOLATION(input,output,bary,uv0);
    #endif
    #if defined(LIL_APP_TEXCOORD1)
        LIL_TRI_INTERPOLATION(input,output,bary,uv1);
    #endif
    #if defined(LIL_APP_TEXCOORD2)
        LIL_TRI_INTERPOLATION(input,output,bary,uv2);
    #endif
    #if defined(LIL_APP_TEXCOORD3)
        LIL_TRI_INTERPOLATION(input,output,bary,uv3);
    #endif
    #if defined(LIL_APP_TEXCOORD4)
        LIL_TRI_INTERPOLATION(input,output,bary,uv4);
    #endif
    #if defined(LIL_APP_TEXCOORD5)
        LIL_TRI_INTERPOLATION(input,output,bary,uv5);
    #endif
    #if defined(LIL_APP_TEXCOORD6)
        LIL_TRI_INTERPOLATION(input,output,bary,uv6);
    #endif
    #if defined(LIL_APP_TEXCOORD7)
        LIL_TRI_INTERPOLATION(input,output,bary,uv7);
    #endif
    #if defined(LIL_APP_COLOR)
        LIL_TRI_INTERPOLATION(input,output,bary,color);
    #endif
    #if defined(LIL_APP_NORMAL)
        LIL_TRI_INTERPOLATION(input,output,bary,normalOS);
    #endif
    #if defined(LIL_APP_TANGENT)
        LIL_TRI_INTERPOLATION(input,output,bary,tangentOS);
    #endif
    #if defined(LIL_APP_VERTEXID)
        output.vertexID = input[0].vertexID;
    #endif
    #if defined(LIL_APP_PREVPOS)
        LIL_TRI_INTERPOLATION(input,output,bary,previousPositionOS);
    #endif
    #if defined(LIL_APP_PREVEL)
        LIL_TRI_INTERPOLATION(input,output,bary,precomputedVelocity);
    #endif

    output.normalOS = normalize(output.normalOS);
    float3 pt[3];
    for(int i = 0; i < 3; i++)
        pt[i] = input[i].normalOS * (dot(input[i].positionOS.xyz, input[i].normalOS) - dot(output.positionOS.xyz, input[i].normalOS) - _TessShrink*0.01);
    output.positionOS.xyz += (pt[0] * bary.x + pt[1] * bary.y + pt[2] * bary.z) * _TessStrength;

    return vert(output);
}

#endif
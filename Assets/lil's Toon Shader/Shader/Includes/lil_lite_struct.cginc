// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------------------
// 構造体
struct appdata
{
    float4 vertex   : POSITION;
    float3 normal   : NORMAL;
    #ifndef LIL_LITE_OUTLINE
        float4 tangent  : TANGENT;
    #endif
    float4 color    : COLOR;
    float2 uv       : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID // シングルパスインスタンシングレンダリング用
};

struct v2f
{
    float4 pos              : SV_POSITION;
    float4 color            : COLOR;
    float2 uv               : TEXCOORD0;
    float3 worldPos         : TEXCOORD1;
    float3 normalDir        : TEXCOORD2;
    #ifndef LIL_LITE_OUTLINE
        float3 tangentDir       : TEXCOORD3;
        float3 bitangentDir     : TEXCOORD4;
    #endif
    UNITY_SHADOW_COORDS(5)
    UNITY_FOG_COORDS(6)
    float3 sh               : TEXCOORD7;
    #ifndef LIL_LITE_OUTLINE
        float isMirror          : TEXCOORD8;
    #endif
    UNITY_VERTEX_OUTPUT_STEREO // シングルパスインスタンシングレンダリング用
};
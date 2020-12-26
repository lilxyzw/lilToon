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
    float4 tangent  : TANGENT;
    float4 color    : COLOR;
    float2 uv       : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID // シングルパスインスタンシングレンダリング用
};

struct v2g
{
    float4 pos              : SV_POSITION;
    float4 color            : COLOR;
    float2 uv               : TEXCOORD0;
    float3 worldPos         : TEXCOORD1;
    float3 normalDir        : TEXCOORD2;
    float3 tangentDir       : TEXCOORD3;
    float3 bitangentDir     : TEXCOORD4;
    UNITY_FOG_COORDS(5)
    #ifndef LIL_FOR_ADD
        float3 shNormal         : TEXCOORD6;
        float3 sh               : TEXCOORD7;
    #endif
    UNITY_VERTEX_OUTPUT_STEREO // シングルパスインスタンシングレンダリング用
};

// ファー用に削れるところは削る
struct g2f
{
    float4 pos              : SV_POSITION;
    float2 uv               : TEXCOORD0;
    float3 worldPos         : TEXCOORD1;
    float3 normalDir        : TEXCOORD2;
    UNITY_FOG_COORDS(3)
    #ifndef LIL_FOR_ADD
        float3 shNormal         : TEXCOORD4;
        float3 sh               : TEXCOORD5;
    #endif
    float furLayer          : TEXCOORD6;
    UNITY_VERTEX_OUTPUT_STEREO // シングルパスインスタンシングレンダリング用
};
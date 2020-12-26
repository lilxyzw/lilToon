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
    #ifdef LIL_FULL
        float2 uv0      : TEXCOORD0;
        float2 uv1      : TEXCOORD1;
        float2 uv2      : TEXCOORD2;
        float2 uv3      : TEXCOORD3;
    #else
        float2 uv       : TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID // シングルパスインスタンシングレンダリング用
};

struct v2g
{
    float4 pos              : SV_POSITION;
    float4 color            : COLOR;
    #ifdef LIL_FULL
        float4 uv0              : TEXCOORD0;
        float4 uv1              : TEXCOORD1;
    #else
        float2 uv               : TEXCOORD0;
    #endif
    float3 worldPos         : TEXCOORD2;
    float3 normalDir        : TEXCOORD3;
    float3 tangentDir       : TEXCOORD4;
    float3 bitangentDir     : TEXCOORD5;
    UNITY_SHADOW_COORDS(6)
    UNITY_FOG_COORDS(7)
    #ifndef LIL_FOR_ADD
        float3 shNormal         : TEXCOORD8;
        float3 sh               : TEXCOORD9;
        float isMirror          : TEXCOORD10;
    #endif
    #ifdef LIL_REFRACTION
        float4 projPos          : TEXCOORD11;
    #endif
    UNITY_VERTEX_OUTPUT_STEREO // シングルパスインスタンシングレンダリング用
};

struct g2f
{
    float4 pos              : SV_POSITION;
    float4 color            : COLOR;
    #ifdef LIL_FULL
        float4 uv0              : TEXCOORD0;
        float4 uv1              : TEXCOORD1;
    #else
        float2 uv               : TEXCOORD0;
    #endif
    float3 worldPos         : TEXCOORD2;
    float3 normalDir        : TEXCOORD3;
    float3 tangentDir       : TEXCOORD4;
    float3 bitangentDir     : TEXCOORD5;
    UNITY_SHADOW_COORDS(6)
    UNITY_FOG_COORDS(7)
    #ifndef LIL_FOR_ADD
        float3 shNormal         : TEXCOORD8;
        float3 sh               : TEXCOORD9;
        float isMirror          : TEXCOORD10;
    #endif
    #ifdef LIL_REFRACTION
        float4 projPos          : TEXCOORD11;
    #endif
    float isOutline         : TEXCOORD12;
    float facing            : TEXCOORD13;
    UNITY_VERTEX_OUTPUT_STEREO // シングルパスインスタンシングレンダリング用
};
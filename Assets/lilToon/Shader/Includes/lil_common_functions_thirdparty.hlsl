#ifndef LIL_FUNCTIONS_THIRDPARTY_INCLUDED
#define LIL_FUNCTIONS_THIRDPARTY_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Inverse trigonometric functions GPU optimization for AMD GCN architecture
// https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/
float lilAcos(float x)
{
    #if 0
        float res = sqrt(1.0 - abs(x)) * LIL_HALF_PI;
        return (x >= 0.0) ? res : LIL_PI - res;
    #else
        float ox = abs(x);
        float res = -0.156583 * ox + LIL_HALF_PI;
        res *= sqrt(1.0 - ox);
        return (x >= 0.0) ? res : LIL_PI - res;
    #endif
}

float lilAsin(float x)
{
    return LIL_HALF_PI - lilAcos(x);
}

float lilAtanPos(float x)
{
    #if 1
        float t0 = (x < 1.0f) ? x : 1.0f / x;
        float t1 = (-0.269408 * t0 + 1.05863) * t0;
        return (x < 1.0f) ? t1 : LIL_HALF_PI - t1;
    #else
        float t0 = (x < 1.0) ? x : 1.0 / x;
        float t1 = t0 * t0;
        float poly = 0.0872929;
        poly = -0.301895 + poly * t1;
        poly = 1.0f + poly * t1;
        poly = poly * t0;
        return (x < 1.0) ? poly : LIL_HALF_PI - poly;
    #endif
}

float lilAtan(float x)
{
    float t0 = lilAtanPos(abs(x));
    return (x < 0.0) ? -t0 : t0;
}

float lilAtan(float x, float y)
{
    return lilAtan(x/y) + LIL_PI * (y<0) * (x<0?-1:1);
}

//------------------------------------------------------------------------------------------------------------------------------
// sRGB Approximations for HLSL
// http://chilliant.blogspot.com/2012/08/srgb-approximations-for-hlsl.html?m=1
float3 lilLinearToSRGB(float3 col)
{
    return saturate(1.055 * pow(abs(col), 0.416666667) - 0.055);
}

float3 lilSRGBToLinear(float3 col)
{
    return col * (col * (col * 0.305306011 + 0.682171111) + 0.012522878);
}

//------------------------------------------------------------------------------------------------------------------------------
// Simplest Fastest 2D Hash
// https://www.shadertoy.com/view/MdcfDj
void lilHashRGB4(float2 pos, out float3 noise0, out float3 noise1, out float3 noise2, out float3 noise3)
{
    // Hash
    // https://www.shadertoy.com/view/MdcfDj
    #define M1 1597334677U
    #define M2 3812015801U
    #define M3 2912667907U
    uint2 q = (uint2)pos;
    uint4 q2 = uint4(q.x, q.y, q.x+1, q.y+1) * uint4(M1, M2, M1, M2);
    uint3 n0 = (q2.x ^ q2.y) * uint3(M1, M2, M3);
    uint3 n1 = (q2.z ^ q2.y) * uint3(M1, M2, M3);
    uint3 n2 = (q2.x ^ q2.w) * uint3(M1, M2, M3);
    uint3 n3 = (q2.z ^ q2.w) * uint3(M1, M2, M3);
    noise0 = float3(n0) * (1.0/float(0xffffffffU));
    noise1 = float3(n1) * (1.0/float(0xffffffffU));
    noise2 = float3(n2) * (1.0/float(0xffffffffU));
    noise3 = float3(n3) * (1.0/float(0xffffffffU));
    #undef M1
    #undef M2
    #undef M3
}

//------------------------------------------------------------------------------------------------------------------------------
// Udon AudioLink
// https://github.com/llealloo/vrc-udon-audio-link
bool lilCheckAudioLink()
{
    #if defined(LIL_FEATURE_AUDIOLINK)
        #if defined(LIL_LWTEX)
            return _AudioTexture_TexelSize.z > 16;
        #else
            int width, height;
            _AudioTexture.GetDimensions(width, height);
            return width > 16;
        #endif
    #else
        return false;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// GTAvaCrypt
// https://github.com/rygo6/GTAvaCrypt/blob/master/LICENSE
#if defined(LIL_FEATURE_ENCRYPTION)
#include "GTModelDecode.cginc"
#endif

#endif
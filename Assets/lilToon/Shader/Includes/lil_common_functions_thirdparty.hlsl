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
// LTCGI
// https://github.com/PiMaker/ltcgi
#if defined(LIL_FEATURE_LTCGI)
float3 NearestPointOnLine(float3 p, float3 pos1, float3 pos2)
{
    float3 v = pos2 - pos1;
    float3 w = p - pos1;
    float c1 = dot(w, v);
    float c2 = dot(v, v);
    float b = c1 / c2;
    float3 pb = pos1 + b * v;
    if(c1 <= 0.0) return pos1;
    if(c2 <= c1) return pos2;
    return pb;
}

float3 NearestPointOnQuad(float3 p, float3 v[4])
{
    float3 v1 = NearestPointOnLine(p, v[0], v[1]);
    float3 v2 = NearestPointOnLine(p, v[2], v[3]);
    return NearestPointOnLine(p, v1, v2);
}

#define Sample(smp,uv) SampleLevel(smp,uv,0)
#include "Packages/at.pimaker.ltcgi/Shaders/LTCGI_structs.cginc"
struct lil_ltcgi_struct
{
    float3 diff;
    float3 spec;
    float3 L;
    float3 N;
    float3 V;
};
float lilLTCGIAttenuation(inout lil_ltcgi_struct ltcgi, in ltcgi_output output)
{
    float3 C = (output.input.Lw[1] + output.input.Lw[0] + output.input.Lw[3] + output.input.Lw[0]) * 0.25;
    float3 P = NearestPointOnQuad(float3(0,0,0), output.input.Lw);
    float3 N = normalize(cross(output.input.Lw[1]-output.input.Lw[0], output.input.Lw[3]-output.input.Lw[0]));
    float3 L = normalize(lerp(P, C, 0.3));
    float dist = lerp(length(P), length(C), 0.3) + 1;
    float rim = saturate(1 - abs(dot(ltcgi.N,ltcgi.V)) * 2 + dot(ltcgi.N,L));
    float atten = abs(dot(N, normalize(P))) / (pow(dist, 3));
    atten *= lerp(rim, 1, saturate(dot(ltcgi.V,L) * 0.5 + 0.5));
    atten = saturate(atten);
    ltcgi.L += atten * dot(output.color,0.333333) * L;
    return atten;
}
void callback_diffuse(inout lil_ltcgi_struct ltcgi, in ltcgi_output output) {
    float atten = lilLTCGIAttenuation(ltcgi, output);
    ltcgi.diff += atten * output.color * 2;
    //ltcgi.diff += output.intensity * output.color;
}
void callback_specular(inout lil_ltcgi_struct ltcgi, in ltcgi_output output) {
    float atten = lilLTCGIAttenuation(ltcgi, output);
    ltcgi.spec += atten * output.color;
    //ltcgi.spec += output.intensity * output.color;
}
#define LTCGI_V2_CUSTOM_INPUT lil_ltcgi_struct
#define LTCGI_V2_DIFFUSE_CALLBACK callback_diffuse
#define LTCGI_V2_SPECULAR_CALLBACK callback_specular
#include "Packages/at.pimaker.ltcgi/Shaders/LTCGI.cginc"

void lilLTCGI(inout lilLightData o, float3 positionWS, float3 N, float3 V, float2 uv1)
{
    lil_ltcgi_struct ltcgi = (lil_ltcgi_struct)0;
    ltcgi.N = N;
    ltcgi.V = V;
    LTCGI_Contribution(ltcgi, positionWS, N, V, 1, uv1);
    o.lightColor += ltcgi.diff + ltcgi.spec;
    //o.lightDirection = normalize(o.lightDirection * dot(o.lightColor+0.001,0.333333) + ltcgi.L);
}
#undef Sample
#endif

//------------------------------------------------------------------------------------------------------------------------------
// GTAvaCrypt
// https://github.com/rygo6/GTAvaCrypt/blob/master/LICENSE
#if defined(LIL_FEATURE_ENCRYPTION)
#include "GTModelDecode.cginc"
#endif

//------------------------------------------------------------------------------------------------------------------------------
// UDIM Discard (UV Tile Discard, original implementation by Razgriz for Poiyomi)
// https://github.com/poiyomi/PoiyomiToonShader/blob/master/LICENSE
bool lilUDIMDiscard(
    float2 uv0,
    float2 uv1,
    float2 uv2,
    float2 uv3,
    float udimDiscardCompile,
    float udimDiscardMode,
    float udimDiscardUV,
    float udimDiscardRow3_0,
    float udimDiscardRow3_1,
    float udimDiscardRow3_2,
    float udimDiscardRow3_3,
    float udimDiscardRow2_0,
    float udimDiscardRow2_1,
    float udimDiscardRow2_2,
    float udimDiscardRow2_3,
    float udimDiscardRow1_0,
    float udimDiscardRow1_1,
    float udimDiscardRow1_2,
    float udimDiscardRow1_3,
    float udimDiscardRow0_0,
    float udimDiscardRow0_1,
    float udimDiscardRow0_2,
    float udimDiscardRow0_3
)
{
    // Branchless (inspired by s-ilent)
    float2 udim = 0; 
    // Select UV
    udim += (uv0 * (udimDiscardUV == 0));
    udim += (uv1 * (udimDiscardUV == 1));
    udim += (uv2 * (udimDiscardUV == 2));
    udim += (uv3 * (udimDiscardUV == 3));

    float isDiscarded = 0;
    float4 xMask = float4(  (udim.x >= 0 && udim.x < 1), 
                            (udim.x >= 1 && udim.x < 2),
                            (udim.x >= 2 && udim.x < 3),
                            (udim.x >= 3 && udim.x < 4));

    isDiscarded += (udim.y >= 0 && udim.y < 1) * dot(float4(udimDiscardRow0_0, udimDiscardRow0_1, udimDiscardRow0_2, udimDiscardRow0_3), xMask);
    isDiscarded += (udim.y >= 1 && udim.y < 2) * dot(float4(udimDiscardRow1_0, udimDiscardRow1_1, udimDiscardRow1_2, udimDiscardRow1_3), xMask);
    isDiscarded += (udim.y >= 2 && udim.y < 3) * dot(float4(udimDiscardRow2_0, udimDiscardRow2_1, udimDiscardRow2_2, udimDiscardRow2_3), xMask);
    isDiscarded += (udim.y >= 3 && udim.y < 4) * dot(float4(udimDiscardRow3_0, udimDiscardRow3_1, udimDiscardRow3_2, udimDiscardRow3_3), xMask);

    isDiscarded *= any(float4(udim.y >= 0, udim.y < 4, udim.x >= 0, udim.x < 4)); // never discard outside 4x4 grid in pos coords 

    // Use a threshold so that there's some room for animations to be close to 0, but not exactly 0
    const float threshold = 0.001;
    return isDiscarded > threshold;
}

#define LIL_CHECK_UDIMDISCARD(i) lilUDIMDiscard( \
    i.uv0.xy, \
    i.uv1.xy, \
    i.uv2.xy, \
    i.uv3.xy, \
    _UDIMDiscardCompile, \
    _UDIMDiscardMode, \
    _UDIMDiscardUV, \
    _UDIMDiscardRow3_0, \
    _UDIMDiscardRow3_1, \
    _UDIMDiscardRow3_2, \
    _UDIMDiscardRow3_3, \
    _UDIMDiscardRow2_0, \
    _UDIMDiscardRow2_1, \
    _UDIMDiscardRow2_2, \
    _UDIMDiscardRow2_3, \
    _UDIMDiscardRow1_0, \
    _UDIMDiscardRow1_1, \
    _UDIMDiscardRow1_2, \
    _UDIMDiscardRow1_3, \
    _UDIMDiscardRow0_0, \
    _UDIMDiscardRow0_1, \
    _UDIMDiscardRow0_2, \
    _UDIMDiscardRow0_3 \
)

#endif
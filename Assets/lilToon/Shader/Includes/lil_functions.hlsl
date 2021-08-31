#ifndef LIL_FUNCTIONS_INCLUDED
#define LIL_FUNCTIONS_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Optimized inverse trigonometric function
// https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/
float lilAcos(float x) 
{ 
    float ox = abs(x); 
    float res = -0.156583 * ox + LIL_HALF_PI; 
    res *= sqrt(1.0 - ox); 
    return (x >= 0.0) ? res : LIL_PI - res; 
}

float lilAsin(float x)
{
    return LIL_HALF_PI - lilAcos(x);
}

float lilAtanPos(float x) 
{ 
    float t0 = (x < 1.0) ? x : 1.0 / x;
    float t1 = t0 * t0;
    float poly = 0.0872929f;
    poly = -0.301895f + poly * t1;
    poly = 1.0f + poly * t1;
    poly = poly * t0;
    return (x < 1.0) ? poly : LIL_HALF_PI - poly;
}

float lilAtan(float x) 
{     
    float t0 = lilAtanPos(abs(x));     
    return (x < 0.0) ? -t0 : t0; 
}

float lilAtan2(float x, float y)
{
    return lilAtan(x/y) + LIL_PI * (y<0) * (x<0?-1:1);
}

//------------------------------------------------------------------------------------------------------------------------------
// Math
#if LIL_ANTIALIAS_MODE == 0
    float lilIsIn0to1(float f)
    {
        return saturate(f) == f;
    }

    float lilIsIn0to1(float f, float nv)
    {
        return saturate(f) == f;
    }

    float lilTooning(float value, float border)
    {
        return step(border, value);
    }

    float lilTooning(float value, float border, float blur)
    {
        float borderMin = saturate(border - blur * 0.5);
        float borderMax = saturate(border + blur * 0.5);
        return saturate((value - borderMin) / saturate(borderMax - borderMin));
    }

    float lilTooning(float value, float border, float blur, float borderRange)
    {
        float borderMin = saturate(border - blur * 0.5 - borderRange);
        float borderMax = saturate(border + blur * 0.5);
        return saturate((value - borderMin) / saturate(borderMax - borderMin));
    }
#else
    float lilIsIn0to1(float f)
    {
        float value = 0.5 - abs(f-0.5);
        return saturate(value / clamp(fwidth(value), 0.0001, 1.0));
    }

    float lilIsIn0to1(float f, float nv)
    {
        float value = 0.5 - abs(f-0.5);
        return saturate(value / clamp(fwidth(value), 0.0001, nv));
    }

    float lilTooning(float value, float border)
    {
        return saturate((value - border) / clamp(fwidth(value), 0.0001, 1.0));
    }

    float lilTooning(float value, float border, float blur)
    {
        float borderMin = saturate(border - blur * 0.5);
        float borderMax = saturate(border + blur * 0.5);
        return saturate((value - borderMin) / saturate(borderMax - borderMin + fwidth(value)));
    }

    float lilTooning(float value, float border, float blur, float borderRange)
    {
        float borderMin = saturate(border - blur * 0.5 - borderRange);
        float borderMax = saturate(border + blur * 0.5);
        return saturate((value - borderMin) / saturate(borderMax - borderMin + fwidth(value)));
    }
#endif

float4 lilOptMul(float4x4 mat, float3 pos)
{
    #if LIL_OPTIMIZE_TRANSFORM == 0
        return mul(mat, float4(pos,1.0));
    #else
        return mat._m00_m10_m20_m30 * pos.x + (mat._m01_m11_m21_m31 * pos.y + (mat._m02_m12_m22_m32 * pos.z + mat._m03_m13_m23_m33));
    #endif
}

float lilIsIn0to1(float2 f)
{
    return lilIsIn0to1(f.x) * lilIsIn0to1(f.y);
}

float lilIsIn0to1(float2 f, float nv)
{
    return lilIsIn0to1(f.x, nv) * lilIsIn0to1(f.y, nv);
}

float3 lilBlendNormal(float3 dstNormal, float3 srcNormal)
{
    return float3(dstNormal.xy + srcNormal.xy, dstNormal.z * srcNormal.z);
}

float lilMedian(float r, float g, float b) {
    return max(min(r, g), min(max(r, g), b));
}

float lilMSDF(float3 msd)
{
    float sd = lilMedian(msd.r, msd.g, msd.b);
    return saturate((sd - 0.5)/clamp(fwidth(sd), 0.01, 1.0));
}

float lilIntervalTime(float interval)
{
    return floor(LIL_TIME / interval) * interval;
}

float lilNsqDistance(float2 a, float2 b)
{
    return dot(a-b,a-b);
}

//------------------------------------------------------------------------------------------------------------------------------
// Encryption (https://github.com/rygo6/GTAvaCrypt)
#if !defined(LIL_LITE) && !defined(LIL_BAKER) &&  defined(LIL_FEATURE_ENCRYPTION)
float4 vertexDecode(float4 positionOS, float3 normalOS, float2 uv6, float2 uv7)
{
    if(_IgnoreEncryption) return positionOS;

    float4 keys = floor(_Keys + 0.5);
    keys = keys.x == 0 ? float4(0,0,0,0) : floor(keys / 3) * 3 + 1;

    keys.x *= 1;
    keys.y *= 2;
    keys.z *= 3;
    keys.w *= 4;

    positionOS.xyz -= normalOS * uv6.x * (sin((keys.z - keys.y) * 2) * cos(keys.w - keys.x));
    positionOS.xyz -= normalOS * uv6.y * (sin((keys.w - keys.x) * 3) * cos(keys.z - keys.y));
    positionOS.xyz -= normalOS * uv7.x * (sin((keys.x - keys.w) * 4) * cos(keys.y - keys.z));
    positionOS.xyz -= normalOS * uv7.y * (sin((keys.y - keys.z) * 5) * cos(keys.x - keys.w));

    return positionOS;
}
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Transform
float3 lilTransformNormalOStoWS(float3 normalOS)
{
    #ifdef UNITY_ASSUME_UNIFORM_SCALING
        return mul((float3x3)LIL_MATRIX_M, normalOS);
    #else
        return mul(normalOS, (float3x3)LIL_MATRIX_I_M);
    #endif
}

struct lilVertexPositionInputs
{
    float3 positionWS; // World space
    float3 positionVS; // View space
    float4 positionCS; // Clip space
    float4 positionSS; // Screen space
};

lilVertexPositionInputs lilGetVertexPositionInputs(float4 positionOS)
{
    lilVertexPositionInputs output;
    output.positionWS = lilOptMul(LIL_MATRIX_M, positionOS.xyz).xyz;
    output.positionVS = lilOptMul(LIL_MATRIX_V, output.positionWS).xyz;
    output.positionCS = lilOptMul(LIL_MATRIX_VP, output.positionWS);
    #if defined(LIL_BRP)
        output.positionSS = ComputeGrabScreenPos(output.positionCS);
    #else
        float4 ndc = output.positionCS * 0.5f;
        output.positionSS.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
        output.positionSS.zw = output.positionCS.zw;
    #endif
    return output;
}

struct lilVertexNormalInputs
{
    float3 tangentWS;
    float3 bitangentWS;
    float3 normalWS;
};

lilVertexNormalInputs lilGetVertexNormalInputs(float3 normalOS)
{
    lilVertexNormalInputs output;
    output.normalWS     = lilTransformNormalOStoWS(normalOS);
    output.tangentWS    = float3(1.0, 0.0, 0.0);
    output.bitangentWS  = float3(0.0, 1.0, 0.0);
    return output;
}

lilVertexNormalInputs lilGetVertexNormalInputs(float3 normalOS, float4 tangentOS)
{
    lilVertexNormalInputs output;
    output.normalWS     = lilTransformNormalOStoWS(normalOS);
    output.tangentWS    = mul((float3x3)LIL_MATRIX_M, tangentOS.xyz);
    output.bitangentWS  = cross(output.normalWS, output.tangentWS) * (tangentOS.w * LIL_NEGATIVE_SCALE);
    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Color
float3 lilBlendColor(float3 dstCol, float3 srcCol, float srcA, uint blendMode)
{
    float3 ad = dstCol + srcCol;
    float3 mu = dstCol * srcCol;
    float3 outCol;
    if(blendMode == 0) outCol = srcCol;               // Normal
    if(blendMode == 1) outCol = ad;                   // Add
    if(blendMode == 2) outCol = max(ad - mu, dstCol); // Screen
    if(blendMode == 3) outCol = mu;                   // Multiply
    return lerp(dstCol, outCol, srcA);
}

float lilLuminance(float3 rgb)
{
    #ifdef LIL_COLORSPACE_GAMMA
        return dot(rgb, float3(0.22, 0.707, 0.071));
    #else
        return dot(rgb, float3(0.0396819152, 0.458021790, 0.00609653955));
    #endif
}

float lilGray(float3 rgb)
{
    return dot(rgb, float3(1.0/3.0, 1.0/3.0, 1.0/3.0));
}

float3 lilToneCorrection(float3 c, float4 hsvg)
{
    // gamma
    c = pow(abs(c), hsvg.w);
    // rgb -> hsv
    float4 p = (c.b > c.g) ? float4(c.bg,-1.0,2.0/3.0) : float4(c.gb,0.0,-1.0/3.0);
    float4 q = (p.x > c.r) ? float4(p.xyw, c.r) : float4(c.r, p.yzx);
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    float3 hsv = float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
    // shift
    hsv = float3(hsv.x+hsvg.x,saturate(hsv.y*hsvg.y),saturate(hsv.z*hsvg.z));
    // hsv -> rgb
    return hsv.z - hsv.z * hsv.y + hsv.z * hsv.y * saturate(abs(frac(hsv.x + float3(1.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0) - 1.0);
}

float3 lilGradationMap(float3 col, TEXTURE2D(gradationMap), SAMPLER(sampstate), float strength)
{
    if(strength == 0.0) return col;
    #if !defined(LIL_COLORSPACE_GAMMA)
        col = LinearToSRGB(col);
    #endif
    float R = LIL_SAMPLE_1D(gradationMap, sampstate, col.r).r;
    float G = LIL_SAMPLE_1D(gradationMap, sampstate, col.g).g;
    float B = LIL_SAMPLE_1D(gradationMap, sampstate, col.b).b;
    float3 outrgb = float3(R,G,B);
    #if !defined(LIL_COLORSPACE_GAMMA)
        col = SRGBToLinear(col);
        outrgb = SRGBToLinear(outrgb);
    #endif
    return lerp(col, outrgb, strength);
}

//------------------------------------------------------------------------------------------------------------------------------
// UV
float2 lilRotateUV(float2 uv, float2x2 rotMatrix)
{
    return mul(rotMatrix, uv - 0.5) + 0.5;
}

float2 lilRotateUV(float2 uv, float angle)
{
    float si,co;
    sincos(angle, si, co);
    float2 outuv = uv - 0.5;
    outuv = float2(
        outuv.x * co - outuv.y * si,
        outuv.x * si + outuv.y * co
    );
    outuv += 0.5;
    return outuv;
}

float2 lilCalcUV(float2 uv, float4 uv_st)
{
    return uv * uv_st.xy + uv_st.zw;
}

float2 lilCalcUV(float2 uv, float4 uv_st, float angle)
{
    float2 outuv = uv * uv_st.xy + uv_st.zw;
    outuv = lilRotateUV(outuv, angle);
    return outuv;
}

float2 lilCalcUV(float2 uv, float4 uv_st, float2 uv_sr)
{
    return uv * uv_st.xy + uv_st.zw + frac(uv_sr * LIL_TIME);
}

float2 lilCalcUV(float2 uv, float4 uv_st, float4 uv_sr)
{
    float2 outuv = uv * uv_st.xy + uv_st.zw + frac(uv_sr.xy * LIL_TIME);
    outuv = lilRotateUV(outuv, uv_sr.z + uv_sr.w * LIL_TIME);
    return outuv;
}

float2 lilCalcDecalUV(
    float2 uv,
    float4 uv_ST,
    float angle,
    bool isLeftOnly,
    bool isRightOnly,
    bool shouldCopy,
    bool shouldFlipMirror,
    bool shouldFlipCopy,
    bool isRightHand)
{
    float2 outUV = uv;

    // Copy
    if(shouldCopy) outUV.x = abs(outUV.x - 0.5) + 0.5;

    // Scale & Offset
    outUV = outUV * uv_ST.xy + uv_ST.zw;

    // Flip
    if(shouldFlipCopy && uv.x<0.5) outUV.x = 1.0 - outUV.x;
    if(shouldFlipMirror && isRightHand) outUV.x = 1.0 - outUV.x;

    // Hide
    if(isLeftOnly && isRightHand) outUV.x = -1.0;
    if(isRightOnly && !isRightHand) outUV.x = -1.0;

    // Rotate
    outUV = (outUV - uv_ST.zw) / uv_ST.xy;
    outUV = lilRotateUV(outUV, angle);
    outUV = outUV * uv_ST.xy + uv_ST.zw;

    return outUV;
}

float2 lilCalcAtlasAnimation(float2 uv, float4 decalAnimation, float4 decalSubParam)
{
    float2 outuv = lerp(float2(uv.x, 1.0-uv.y), 0.5, decalSubParam.z);
    uint animTime = (uint)(LIL_TIME * decalAnimation.w) % (uint)decalAnimation.z;
    uint offsetX = animTime % (uint)decalAnimation.x;
    uint offsetY = animTime / (uint)decalAnimation.x;
    outuv = (outuv + float2(offsetX,offsetY)) * decalSubParam.xy / decalAnimation.xy;
    outuv.y = -outuv.y;
    return outuv;
}

float2 lilCalcUVWithoutAnimation(float2 uv, float4 uv_st, float4 uv_sr)
{
    return lilRotateUV(uv * uv_st.xy + uv_st.zw, uv_sr.z);
}

float2 lilCalcMatCapUV(float3 normalWS, bool zRotCancel = true)
{
    #if LIL_MATCAP_MODE == 0
        // Simple
        return mul((float3x3)LIL_MATRIX_V, normalWS).xy * 0.5 + 0.5;
    #elif LIL_MATCAP_MODE == 1
        // Fix Z-Rotation
        bool isMirror = unity_CameraProjection._m20 != 0.0 || unity_CameraProjection._m21 != 0.0;
        float2 outuv = mul((float3x3)LIL_MATRIX_V, normalWS).xy * 0.5;
        if(zRotCancel)
        {
            //outuv.y = isMirror ? -outuv.y : outuv.y;

            float3 tan = LIL_MATRIX_V._m00_m01_m02;
            float3 bitan = float3(-LIL_MATRIX_V._m22, 0.0, LIL_MATRIX_V._m20);
            float co = dot(tan,bitan) / length(bitan);
            float si = LIL_MATRIX_V._m01;
            co = isMirror ? -co : co;

            outuv = float2(
                outuv.x * co - outuv.y * si,
                outuv.x * si + outuv.y * co
            );
        }
        outuv += 0.5;
        outuv.x = isMirror ? -outuv.x : outuv.x;

        return outuv;
    #endif
}

float lilCalcBlink(float4 blink)
{
    float outBlink = sin(LIL_TIME * blink.z + blink.w) * 0.5 + 0.5;
    if(blink.y > 0.5) outBlink = round(outBlink);
    return lerp(1.0, outBlink, blink.x);
}

float2 lilGetPanoramaUV(float3 viewDirection)
{
    return float2(lilAtan2(viewDirection.x, viewDirection.z), lilAcos(viewDirection.y)) * LIL_INV_PI;
}

void lilCalcDissolve(
    inout float alpha,
    inout float dissolveAlpha,
    float2 uv,
    float3 positionOS,
    float4 dissolveParams,
    float4 dissolvePos,
    TEXTURE2D(dissolveMask),
    float4 dissolveMask_ST,
    SamplerState sampstate)
{
    if(dissolveParams.r)
    {
        float dissolveMaskVal = 1.0;
        if(dissolveParams.r == 1.0)
        {
            dissolveMaskVal = LIL_SAMPLE_2D(dissolveMask, sampstate, lilCalcUV(uv, dissolveMask_ST)).r;
        }
        if(dissolveParams.r == 1.0)
        {
            dissolveAlpha = 1.0 - saturate(abs(dissolveMaskVal - dissolveParams.b) / dissolveParams.a);
            dissolveMaskVal = dissolveMaskVal > dissolveParams.b ? 1.0 : 0.0;
        }
        if(dissolveParams.r == 2.0)
        {
            dissolveAlpha = dissolveParams.g == 1.0 ? lilRotateUV(uv, dissolvePos.w).x : distance(uv, dissolvePos.xy);
            dissolveMaskVal *= dissolveAlpha > dissolveParams.b ? 1.0 : 0.0;
            dissolveAlpha = 1.0 - saturate(abs(dissolveAlpha - dissolveParams.b) / dissolveParams.a);
        }
        if(dissolveParams.r == 3.0)
        {
            dissolveAlpha = dissolveParams.g == 1.0 ? dot(positionOS, normalize(dissolvePos.xyz)).x : distance(positionOS, dissolvePos.xyz);
            dissolveMaskVal *= dissolveAlpha > dissolveParams.b ? 1.0 : 0.0;
            dissolveAlpha = 1.0 - saturate(abs(dissolveAlpha - dissolveParams.b) / dissolveParams.a);
        }
        alpha *= dissolveMaskVal;
    }
}

void lilCalcDissolveWithNoise(
    inout float alpha,
    inout float dissolveAlpha,
    float2 uv,
    float3 positionOS,
    float4 dissolveParams,
    float4 dissolvePos,
    TEXTURE2D(dissolveMask),
    float4 dissolveMask_ST,
    TEXTURE2D(dissolveNoiseMask),
    float4 dissolveNoiseMask_ST,
    float4 dissolveNoiseMask_ScrollRotate,
    float dissolveNoiseStrength,
    SamplerState sampstate)
{
    if(dissolveParams.r)
    {
        float dissolveMaskVal = 1.0;
        float dissolveNoise = 0.0;
        if(dissolveParams.r == 1.0)
        {
            dissolveMaskVal = LIL_SAMPLE_2D(dissolveMask, sampstate, lilCalcUV(uv, dissolveMask_ST)).r;
        }
        dissolveNoise = LIL_SAMPLE_2D(dissolveNoiseMask, sampstate, lilCalcUV(uv, dissolveNoiseMask_ST, dissolveNoiseMask_ScrollRotate.xy)).r - 0.5;
        dissolveNoise *= dissolveNoiseStrength;
        if(dissolveParams.r == 1.0)
        {
            dissolveAlpha = 1.0 - saturate(abs(dissolveMaskVal + dissolveNoise - dissolveParams.b) / dissolveParams.a);
            dissolveMaskVal = dissolveMaskVal + dissolveNoise > dissolveParams.b ? 1.0 : 0.0;
        }
        if(dissolveParams.r == 2.0)
        {
            dissolveAlpha = dissolveParams.g == 1.0 ? dot(uv, normalize(dissolvePos.xy)) + dissolveNoise : distance(uv, dissolvePos.xy) + dissolveNoise;
            dissolveMaskVal *= dissolveAlpha > dissolveParams.b ? 1.0 : 0.0;
            dissolveAlpha = 1.0 - saturate(abs(dissolveAlpha - dissolveParams.b) / dissolveParams.a);
        }
        if(dissolveParams.r == 3.0)
        {
            dissolveAlpha = dissolveParams.g == 1.0 ? dot(positionOS, normalize(dissolvePos.xyz)) + dissolveNoise : distance(positionOS, dissolvePos.xyz) + dissolveNoise;
            dissolveMaskVal *= dissolveAlpha > dissolveParams.b ? 1.0 : 0.0;
            dissolveAlpha = 1.0 - saturate(abs(dissolveAlpha - dissolveParams.b) / dissolveParams.a);
        }
        alpha *= dissolveMaskVal;
    }
}

//------------------------------------------------------------------------------------------------------------------------------
// Sub Texture
float4 lilGetSubTex(
    bool existsTex,
    Texture2D tex,
    float4 uv_ST,
    float angle,
    float2 uv,
    float nv,
    SAMPLER(sampstate),
    bool isDecal,
    bool isLeftOnly,
    bool isRightOnly,
    bool shouldCopy,
    bool shouldFlipMirror,
    bool shouldFlipCopy,
    bool isMSDF,
    bool isRightHand,
    float4 decalAnimation,
    float4 decalSubParam)
{
    #if defined(LIL_FEATURE_DECAL)
        float2 uv2 = lilCalcDecalUV(uv, uv_ST, angle, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, isRightHand);
        #if defined(LIL_FEATURE_ANIMATE_DECAL)
            float2 uv2samp = lilCalcAtlasAnimation(uv2, decalAnimation, decalSubParam);
        #else
            float2 uv2samp = uv2;
        #endif
        float4 outCol = 1.0;
        if(existsTex) outCol = LIL_SAMPLE_2D(tex,sampstate,uv2samp);
        LIL_BRANCH
        if(isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
        LIL_BRANCH
        if(isDecal) outCol.a *= lilIsIn0to1(uv2, saturate(nv-0.05));
        return outCol;
    #else
        float2 uv2 = lilCalcUV(uv, uv_ST, angle);
        float4 outCol = 1.0;
        if(existsTex) outCol = LIL_SAMPLE_2D(tex,sampstate,uv2);
        LIL_BRANCH
        if(isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
        return outCol;
    #endif
}

float4 lilGetSubTexWithoutAnimation(
    bool existsTex,
    Texture2D tex,
    float4 uv_ST,
    float angle,
    float2 uv,
    float nv,
    SamplerState sampstate,
    bool isDecal,
    bool isLeftOnly,
    bool isRightOnly,
    bool shouldCopy,
    bool shouldFlipMirror,
    bool shouldFlipCopy,
    bool isMSDF,
    bool isRightHand)
{
    #if defined(LIL_FEATURE_DECAL)
        float2 uv2 = lilCalcDecalUV(uv, uv_ST, angle, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, isRightHand);
        float4 outCol = 1.0;
        if(existsTex) outCol = LIL_SAMPLE_2D(tex,sampstate,uv2);
        LIL_BRANCH
        if(isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
        LIL_BRANCH
        if(isDecal) outCol.a *= lilIsIn0to1(uv2, saturate(nv-0.05));
        return outCol;
    #else
        float2 uv2 = lilCalcUV(uv, uv_ST, angle);
        float4 outCol = 1.0;
        if(existsTex) outCol = LIL_SAMPLE_2D(tex,sampstate,uv2);
        LIL_BRANCH
        if(isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
        return outCol;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Light Direction
float3 lilGetLightDirection()
{
    #if LIL_LIGHT_DIRECTION_MODE == 0
        return normalize(_MainLightPosition.xyz + float3(0.0,0.001,0.0));
    #else
        return normalize(_MainLightPosition.xyz * lilLuminance(_MainLightColor.rgb) + 
                        unity_SHAr.xyz * 0.333333 + unity_SHAg.xyz * 0.333333 + unity_SHAb.xyz * 0.333333 + 
                        float3(0.0,0.001,0.0));
    #endif
}

float3 lilGetLightDirection(float3 positionWS)
{
    #if defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
        return normalize(_MainLightPosition.xyz - positionWS);
    #else
        return _MainLightPosition.xyz;
    #endif
}

float3 lilGetLightMapDirection(float2 uv)
{
    #if defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DIRLIGHTMAP)
        float4 lightmapDirection = LIL_SAMPLE_LIGHTMAP(LIL_DIRLIGHTMAP_TEX,  LIL_LIGHTMAP_SAMP, uv);
        return lightmapDirection.xyz * 2.0 - 1.0;
    #else
        return 0;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// SH Lighting
float3 lilGetSHToon()
{
    float3 N = lilGetLightDirection() * 0.666666;
    float3 res = float3(unity_SHAr.w,unity_SHAg.w,unity_SHAb.w);
    res.r += dot(unity_SHAr.rgb, N);
    res.g += dot(unity_SHAg.rgb, N);
    res.b += dot(unity_SHAb.rgb, N);
    float4 vB = N.xyzz * N.yzzx;
    res.r += dot(unity_SHBr, vB);
    res.g += dot(unity_SHBg, vB);
    res.b += dot(unity_SHBb, vB);
    res += unity_SHC.rgb * (N.x * N.x - N.y * N.y);
    #ifdef UNITY_COLORSPACE_GAMMA
        res = LinearToSRGB(res);
    #endif
    return res;
}

float3 lilGetSHToon(float3 positionWS)
{
    float4 SHAr = unity_SHAr;
    float4 SHAg = unity_SHAg;
    float4 SHAb = unity_SHAb;
    #if defined(LIL_USE_LPPV)
        if(unity_ProbeVolumeParams.x == 1.0)
        {
            float3 position = (unity_ProbeVolumeParams.y == 1.0) ? lilOptMul(unity_ProbeVolumeWorldToObject, positionWS).xyz : positionWS;
            float3 texCoord = (position - unity_ProbeVolumeMin.xyz) * unity_ProbeVolumeSizeInv.xyz;
            texCoord.x = texCoord.x * 0.25;
            float texCoordX = clamp(texCoord.x, 0.5 * unity_ProbeVolumeParams.z, 0.25 - 0.5 * unity_ProbeVolumeParams.z);
            texCoord.x = texCoordX;
            SHAr = LIL_SAMPLE_3D(unity_ProbeVolumeSH, samplerunity_ProbeVolumeSH, texCoord);
            texCoord.x = texCoordX + 0.25;
            SHAg = LIL_SAMPLE_3D(unity_ProbeVolumeSH, samplerunity_ProbeVolumeSH, texCoord);
            texCoord.x = texCoordX + 0.5;
            SHAb = LIL_SAMPLE_3D(unity_ProbeVolumeSH, samplerunity_ProbeVolumeSH, texCoord);
        }
    #endif
    float3 N = lilGetLightDirection() * 0.666666;
    float3 res = float3(SHAr.w,SHAg.w,SHAb.w);
    res.r += dot(SHAr.rgb, N);
    res.g += dot(SHAg.rgb, N);
    res.b += dot(SHAb.rgb, N);
    float4 vB = N.xyzz * N.yzzx;
    res.r += dot(unity_SHBr, vB);
    res.g += dot(unity_SHBg, vB);
    res.b += dot(unity_SHBb, vB);
    res += unity_SHC.rgb * (N.x * N.x - N.y * N.y);
    #ifdef UNITY_COLORSPACE_GAMMA
        res = LinearToSRGB(res);
    #endif
    return res;
}

float3 lilGetSHToonMin()
{
    float3 N = -lilGetLightDirection() * 0.666666;
    float3 res = float3(unity_SHAr.w,unity_SHAg.w,unity_SHAb.w);
    res.r += dot(unity_SHAr.rgb, N);
    res.g += dot(unity_SHAg.rgb, N);
    res.b += dot(unity_SHAb.rgb, N);
    float4 vB = N.xyzz * N.yzzx;
    res.r += dot(unity_SHBr, vB);
    res.g += dot(unity_SHBg, vB);
    res.b += dot(unity_SHBb, vB);
    res += unity_SHC.rgb * (N.x * N.x - N.y * N.y);
    #ifdef UNITY_COLORSPACE_GAMMA
        res = LinearToSRGB(res);
    #endif
    return res;
}

float3 lilGetSHToonMin(float3 lightDirection)
{
    float3 N = -lightDirection * 0.666666;
    float3 res = float3(unity_SHAr.w,unity_SHAg.w,unity_SHAb.w);
    res.r += dot(unity_SHAr.rgb, N);
    res.g += dot(unity_SHAg.rgb, N);
    res.b += dot(unity_SHAb.rgb, N);
    float4 vB = N.xyzz * N.yzzx;
    res.r += dot(unity_SHBr, vB);
    res.g += dot(unity_SHBg, vB);
    res.b += dot(unity_SHBb, vB);
    res += unity_SHC.rgb * (N.x * N.x - N.y * N.y);
    #ifdef UNITY_COLORSPACE_GAMMA
        res = LinearToSRGB(res);
    #endif
    return res;
}

void lilGetToonSHDouble(float3 lightDirection, out float3 shMax, out float3 shMin)
{
    float3 N = lightDirection * 0.666666;
    float4 vB = N.xyzz * N.yzzx;
    // L0 L2
    float3 res = float3(unity_SHAr.w,unity_SHAg.w,unity_SHAb.w);
    res.r += dot(unity_SHBr, vB);
    res.g += dot(unity_SHBg, vB);
    res.b += dot(unity_SHBb, vB);
    res += unity_SHC.rgb * (N.x * N.x - N.y * N.y);
    // L1
    float3 l1;
    l1.r = dot(unity_SHAr.rgb, N);
    l1.g = dot(unity_SHAg.rgb, N);
    l1.b = dot(unity_SHAb.rgb, N);
    shMax = res + l1;
    shMin = res - l1;
    #ifdef UNITY_COLORSPACE_GAMMA
        shMax = LinearToSRGB(shMax);
        shMin = LinearToSRGB(shMin);
    #endif
}

void lilGetToonSHDouble(float3 lightDirection, float3 positionWS, out float3 shMax, out float3 shMin)
{
    float4 SHAr = unity_SHAr;
    float4 SHAg = unity_SHAg;
    float4 SHAb = unity_SHAb;
    #if defined(LIL_USE_LPPV)
        if(unity_ProbeVolumeParams.x == 1.0)
        {
            float3 position = (unity_ProbeVolumeParams.y == 1.0) ? lilOptMul(unity_ProbeVolumeWorldToObject, positionWS).xyz : positionWS;
            float3 texCoord = (position - unity_ProbeVolumeMin.xyz) * unity_ProbeVolumeSizeInv.xyz;
            texCoord.x = texCoord.x * 0.25;
            float texCoordX = clamp(texCoord.x, 0.5 * unity_ProbeVolumeParams.z, 0.25 - 0.5 * unity_ProbeVolumeParams.z);
            texCoord.x = texCoordX;
            SHAr = LIL_SAMPLE_3D(unity_ProbeVolumeSH, samplerunity_ProbeVolumeSH, texCoord);
            texCoord.x = texCoordX + 0.25;
            SHAg = LIL_SAMPLE_3D(unity_ProbeVolumeSH, samplerunity_ProbeVolumeSH, texCoord);
            texCoord.x = texCoordX + 0.5;
            SHAb = LIL_SAMPLE_3D(unity_ProbeVolumeSH, samplerunity_ProbeVolumeSH, texCoord);
        }
    #endif
    float3 N = lightDirection * 0.666666;
    float4 vB = N.xyzz * N.yzzx;
    // L0 L2
    float3 res = float3(SHAr.w,SHAg.w,SHAb.w);
    res.r += dot(unity_SHBr, vB);
    res.g += dot(unity_SHBg, vB);
    res.b += dot(unity_SHBb, vB);
    res += unity_SHC.rgb * (N.x * N.x - N.y * N.y);
    // L1
    float3 l1;
    l1.r = dot(SHAr.rgb, N);
    l1.g = dot(SHAg.rgb, N);
    l1.b = dot(SHAb.rgb, N);
    shMax = res + l1;
    shMin = res - l1;
    #ifdef UNITY_COLORSPACE_GAMMA
        shMax = LinearToSRGB(shMax);
        shMin = LinearToSRGB(shMin);
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Lighting
float3 lilGetLightColor()
{
    return saturate(_MainLightColor.rgb + lilGetSHToon());
}

float3 lilGetLightColor(float3 positionWS)
{
    return saturate(_MainLightColor.rgb + lilGetSHToon(positionWS));
}

float3 lilGetIndirLightColor()
{
    return saturate(lilGetSHToonMin());
}

float3 lilGetIndirLightColor(float3 lightDirection)
{
    return saturate(lilGetSHToonMin(lightDirection));
}

void lilGetLightColorDouble(float3 lightDirection, float shadowEnvStrength, out float3 lightColor, out float3 indLightColor)
{
    float3 shMax, shMin;
    lilGetToonSHDouble(lightDirection, shMax, shMin);
    lightColor = saturate(_MainLightColor.rgb + lilGetSHToon());
    indLightColor = saturate(lilGetSHToonMin()) * shadowEnvStrength;
}

float3 lilGetLightMapColor(float2 uv)
{
    float3 outCol = 0;
    #ifdef LIL_USE_LIGHTMAP
        float4 lightmap = LIL_SAMPLE_LIGHTMAP(LIL_LIGHTMAP_TEX, LIL_LIGHTMAP_SAMP, uv);
        outCol += LIL_DECODE_LIGHTMAP(lightmap);
    #endif
    #ifdef LIL_USE_DYNAMICLIGHTMAP
        float4 dynlightmap = LIL_SAMPLE_LIGHTMAP(LIL_DYNAMICLIGHTMAP_TEX, LIL_DYNAMICLIGHTMAP_SAMP, uv);
        outCol += LIL_DECODE_DYNAMICLIGHTMAP(dynlightmap);
    #endif
    return outCol;
}

#if !defined(LIL_BAKER)
float3 lilGetVertexLights(float3 positionWS, float vertexLightStrength = 1.0)
{
    #ifdef LIL_BRP
        float4 toLightX = unity_4LightPosX0 - positionWS.x;
        float4 toLightY = unity_4LightPosY0 - positionWS.y;
        float4 toLightZ = unity_4LightPosZ0 - positionWS.z;

        float4 lengthSq = toLightX * toLightX + 0.000001;
        lengthSq += toLightY * toLightY;
        lengthSq += toLightZ * toLightZ;

        #if LIL_VERTEXLIGHT_MODE == 0
            // Off
            float4 atten = 0.0;
        #elif LIL_VERTEXLIGHT_MODE == 1
            // Simple
            float4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
        #elif LIL_VERTEXLIGHT_MODE == 2
            // Accurate
            float4 fade = saturate(1.0 - lengthSq * unity_4LightAtten0 / 25.0);
            float4 atten = saturate(fade * fade / (1.0 + lengthSq * unity_4LightAtten0));
        #elif LIL_VERTEXLIGHT_MODE == 3
            // Approximate _LightTextureB0
            float4 atten = saturate(saturate((25.0 - lengthSq * unity_4LightAtten0) * 0.111375) / (0.987725 + lengthSq * unity_4LightAtten0));
        #elif LIL_VERTEXLIGHT_MODE == 4
            // Lookup _LightTextureB0
            float4 normalizedDist = lengthSq * unity_4LightAtten0 / 25.0;
            float4 atten;
            atten.x = LIL_SAMPLE_2D_LOD(_LightTextureB0, sampler_LightTextureB0, normalizedDist.xx, 0).UNITY_ATTEN_CHANNEL;
            atten.y = LIL_SAMPLE_2D_LOD(_LightTextureB0, sampler_LightTextureB0, normalizedDist.yy, 0).UNITY_ATTEN_CHANNEL;
            atten.z = LIL_SAMPLE_2D_LOD(_LightTextureB0, sampler_LightTextureB0, normalizedDist.zz, 0).UNITY_ATTEN_CHANNEL;
            atten.w = LIL_SAMPLE_2D_LOD(_LightTextureB0, sampler_LightTextureB0, normalizedDist.ww, 0).UNITY_ATTEN_CHANNEL;
        #endif

        float3 outCol;
        outCol =                   unity_LightColor[0].rgb * atten.x;
        outCol =          outCol + unity_LightColor[1].rgb * atten.y;
        outCol =          outCol + unity_LightColor[2].rgb * atten.z;
        outCol = saturate(outCol + unity_LightColor[3].rgb * atten.w);

        return outCol * vertexLightStrength;
    #else
        float3 outCol = 0.0;

        #ifdef _ADDITIONAL_LIGHTS_VERTEX
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0; lightIndex < lightsCount; lightIndex++)
            {
                Light light = GetAdditionalLight(lightIndex, positionWS);
                outCol += light.color * light.distanceAttenuation;
            }
        #endif

        return outCol * vertexLightStrength;
    #endif
}
#endif

float3 lilGetAdditionalLights(float3 positionWS)
{
    float3 outCol = 0.0;
    #ifdef _ADDITIONAL_LIGHTS
        uint lightsCount = GetAdditionalLightsCount();
        for (uint lightIndex = 0; lightIndex < lightsCount; lightIndex++)
        {
            Light light = GetAdditionalLight(lightIndex, positionWS);
            outCol += light.distanceAttenuation * light.shadowAttenuation * light.color;
        }
    #endif
    return outCol;
}

//------------------------------------------------------------------------------------------------------------------------------
// Shading
#if !defined(LIL_LITE) && !defined(LIL_GEM) && !defined(LIL_BAKER) && defined(LIL_FEATURE_SHADOW)
void lilGetShading(
    inout float4 col,
    inout float shadowmix,
    float3 albedo,
    float3 lightColor,
    float3 indLightColor,
    float2 uv,
    float facing,
    float3 normalDirection,
    float attenuation,
    float3 lightDirection,
    SamplerState sampstate,
    bool cullOff = true)
{
    LIL_BRANCH
    if(_UseShadow)
    {
        // Shade
        float ln = saturate(dot(lightDirection,normalDirection)*0.5+0.5);
        if(Exists_ShadowBorderMask) ln *= LIL_SAMPLE_2D(_ShadowBorderMask, sampstate, uv).r;
        float ln2 = ln;
        float lnB = ln;

        // Shadow
        #if defined(LIL_USE_SHADOW) || (defined(LIL_LIGHTMODE_SHADOWMASK) && defined(LIL_FEATURE_RECEIVE_SHADOW))
            float shadowAttenuation = saturate(attenuation + distance(lightDirection, _MainLightPosition.xyz));
            if(_ShadowReceive) ln *= shadowAttenuation;
            if(_ShadowReceive) lnB *= shadowAttenuation;
        #endif

        // Toon
        float shadowBlur = _ShadowBlur;
        if(Exists_ShadowBlurMask) shadowBlur *= LIL_SAMPLE_2D(_ShadowBlurMask, sampstate, uv).r;
        ln = lilTooning(ln, _ShadowBorder, shadowBlur);
        ln2 = lilTooning(ln2, _Shadow2ndBorder, _Shadow2ndBlur);
        lnB = lilTooning(lnB, _ShadowBorder, shadowBlur, _ShadowBorderRange);

        if(cullOff)
        {
            // Force shadow on back face
            float bfshadow = (facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
            ln *= bfshadow;
            ln2 *= bfshadow;
            lnB *= bfshadow;
        }

        // Copy
        shadowmix = ln;

        // Strength
        float shadowStrength = _ShadowStrength;
        #ifdef UNITY_COLORSPACE_GAMMA
            shadowStrength = SRGBToLinear(shadowStrength);
        #endif
        if(Exists_ShadowStrengthMask) shadowStrength *= LIL_SAMPLE_2D(_ShadowStrengthMask, sampstate, uv).r;
        ln = lerp(1.0, ln, shadowStrength);

        // Shadow Color 1
        float4 shadowColorTex = 0.0;
        if(Exists_ShadowColorTex) shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, sampstate, uv);
        float3 indirectCol = lerp(albedo, shadowColorTex.rgb, shadowColorTex.a) * _ShadowColor.rgb;
        // Shadow Color 2
        float4 shadow2ndColorTex = 0.0;
        if(Exists_Shadow2ndColorTex) shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, sampstate, uv);
        shadow2ndColorTex.rgb = lerp(albedo, shadow2ndColorTex.rgb, shadow2ndColorTex.a) * _Shadow2ndColor.rgb;
        ln2 = _Shadow2ndColor.a - ln2 * _Shadow2ndColor.a;
        indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, ln2);
        // Multiply Main Color
        indirectCol = lerp(indirectCol, indirectCol*albedo, _ShadowMainStrength);

        // Apply Light
        float3 directCol = albedo * lightColor;
        indirectCol = indirectCol * lightColor;

        // Environment Light
        indirectCol = lerp(indirectCol, albedo, indLightColor);
        // Fix
        indirectCol = min(indirectCol, directCol);
        // Gradation
        indirectCol = lerp(indirectCol, directCol, lnB * _ShadowBorderColor.rgb);

        // Mix
        col.rgb = lerp(indirectCol, directCol, ln);
    }
    else
    {
        col.rgb *= lightColor;
    }
}
#endif

#if defined(LIL_LITE)
void lilGetShadingLite(
    inout float4 col,
    inout float shadowmix,
    float3 albedo,
    float3 lightColor,
    float3 indLightColor,
    float2 uv,
    float facing,
    float3 normalDirection,
    float3 lightDirection,
    SamplerState sampstate,
    bool cullOff = true)
{
    LIL_BRANCH
    if(_UseShadow)
    {
        // Shade
        float ln = saturate(dot(lightDirection,normalDirection)*0.5+0.5);
        float ln2 = ln;
        float lnB = ln;

        // Toon
        ln = lilTooning(ln, _ShadowBorder, _ShadowBlur);
        ln2 = lilTooning(ln2, _Shadow2ndBorder, _Shadow2ndBlur);
        lnB = lilTooning(lnB, _ShadowBorder, _ShadowBlur, _ShadowBorderRange);

        if(cullOff)
        {
            // Force shadow on back face
            float bfshadow = (facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
            ln *= bfshadow;
            ln2 *= bfshadow;
            lnB *= bfshadow;
        }

        // Copy
        shadowmix = ln;

        // Shadow Color 1
        float4 shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, sampstate, uv);
        float3 indirectCol = lerp(albedo, shadowColorTex.rgb, shadowColorTex.a);
        // Shadow Color 2
        float4 shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, sampstate, uv);
        indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, shadow2ndColorTex.a - ln2 * shadow2ndColorTex.a);

        // Apply Light
        float3 directCol = albedo * lightColor;
        indirectCol = indirectCol * lightColor;

        // Environment Light
        indirectCol = lerp(indirectCol, albedo, indLightColor);
        // Fix
        indirectCol = min(indirectCol, directCol);
        // Gradation
        indirectCol = lerp(indirectCol, directCol, lnB * _ShadowBorderColor.rgb);

        // Mix
        col.rgb = lerp(indirectCol, directCol, ln);
    }
    else
    {
        col.rgb *= lightColor;
    }
}
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Specular
float3 lilFresnelTerm(float3 F0, float cosA)
{
    float a = 1.0-cosA;
    return F0 + (1-F0) * a * a * a * a * a;
}

float3 lilFresnelLerp(float3 F0, float3 F90, float cosA)
{
    float a = 1.0-cosA;
    return lerp(F0, F90, a * a * a * a * a);
}

float3 lilCalcSpecular(float nv, float nl, float nh, float lh, float roughness, float3 specular, bool isSpecularToon, float attenuation = 1.0)
{
    #if LIL_SPECULAR_MODE == 0
        // BRP Specular
        float roughness2 = max(roughness, 0.002);

        float lambdaV = nl * (nv * (1.0 - roughness2) + roughness2);
        float lambdaL = nv * (nl * (1.0 - roughness2) + roughness2);
        #if defined(SHADER_API_SWITCH)
            float sjggx =  0.5 / (lambdaV + lambdaL + 1e-4f);
        #else
            float sjggx =  0.5 / (lambdaV + lambdaL + 1e-5f);
        #endif

        float r2 = roughness2 * roughness2;
        float d = (nh * r2 - nh) * nh + 1.0;
        float ggx = r2 / (d * d + 1e-7f);

        //float specularTerm = SmithJointGGXVisibilityTerm(nl,nv,roughness2) * GGXTerm(nh,roughness2) * LIL_PI;
        float specularTerm = sjggx * ggx;
        #ifdef UNITY_COLORSPACE_GAMMA
            specularTerm = sqrt(max(1e-4h, specularTerm));
        #endif
        specularTerm *= nl * attenuation;
        if(isSpecularToon) return lilTooning(specularTerm, 0.5);
        else               return specularTerm * lilFresnelTerm(specular, lh);
    #elif LIL_SPECULAR_MODE == 1
        // URP Specular
        float roughness2 = max(roughness, 0.002);
        float r2 = roughness2 * roughness2;
        float d = (nh * r2 - nh) * nh + 1.00001;
        float specularTerm = r2 / ((d * d) * max(0.1, lh * lh) * (roughness * 4.0 + 2.0));
        #if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
            specularTerm = clamp(specularTerm, 0.0, 100.0);
        #endif
        if(isSpecularToon) return lilTooning(specularTerm, 0.5);
        else               return specularTerm * specular;
    #else
        // Fast Specular
        float smoothness = 1.0/max(roughness, 0.002);
        float specularTerm = pow(nh, smoothness);
        if(isSpecularToon) return lilTooning(specularTerm, 0.5);
        else               return specularTerm * smoothness * 0.1 * specular;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Glitter
float3 lilGlitter(float2 uv, float3 normalDirection, float3 viewDirection, float3 lightDirection, float4 glitterParams1, float4 glitterParams2)
{
    // glitterParams1
    // x: Scale, y: Scale, z: Size, w: Contrast
    // glitterParams2
    // x: Speed, y: Angle, z: Light Direction, w: 

    float2 pos = uv * glitterParams1.xy;

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
    float3 noise0 = float3(n0) * (1.0/float(0xffffffffU));
    float3 noise1 = float3(n1) * (1.0/float(0xffffffffU));
    float3 noise2 = float3(n2) * (1.0/float(0xffffffffU));
    float3 noise3 = float3(n3) * (1.0/float(0xffffffffU));

    // Get the nearest position
    float4 fracpos = frac(pos).xyxy + float4(0.5,0.5,-0.5,-0.5);
    float4 dist4 = float4(lilNsqDistance(fracpos.xy,noise0.xy), lilNsqDistance(fracpos.zy,noise1.xy), lilNsqDistance(fracpos.xw,noise2.xy), lilNsqDistance(fracpos.zw,noise3.xy));
    float4 near0 = dist4.x < dist4.y ? float4(noise0,dist4.x) : float4(noise1,dist4.y);
    float4 near1 = dist4.z < dist4.w ? float4(noise2,dist4.z) : float4(noise3,dist4.w);
    float4 near = near0.w < near1.w ? near0 : near1;

    #define GLITTER_DEBUG_MODE 0
    #define GLITTER_ANTIALIAS 1

    #if GLITTER_DEBUG_MODE == 1
        // Voronoi
        return near.x;
    #else
        // Glitter
        float3 glitterNormal = abs(frac(near.xyz*14.274 + _Time.x * glitterParams2.x) * 2.0 - 1.0);
        glitterNormal = normalize(glitterNormal * 2.0 - 1.0);
        float glitter = dot(glitterNormal, viewDirection);
        glitter = saturate(1.0 - (glitter * glitterParams1.w + glitterParams1.w));
        // Circle
        #if GLITTER_ANTIALIAS == 1
            glitter *= saturate((glitterParams1.z-near.w) / fwidth(near.w));
        #else
            glitter = near.w < glitterParams1.z ? glitter : 0.0;
        #endif
        // Angle
        float3 halfDirection = normalize(viewDirection + lightDirection * glitterParams2.z);
        float nh = saturate(dot(normalDirection, halfDirection));
        glitter = saturate(glitter * saturate(nh * glitterParams2.y + 1.0 - glitterParams2.y));
        // Random Color
        float3 glitterColor = glitter - glitter * frac(near.xyz*278.436) * glitterParams2.w;
        return glitterColor;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Tessellation
float lilCalcEdgeTessFactor(float3 wpos0, float3 wpos1, float edgeLen)
{
    float dist = distance(0.5 * (wpos0+wpos1), _WorldSpaceCameraPos);
    return max(distance(wpos0, wpos1) * _ScreenParams.y / (edgeLen * dist), 1.0);
}

#endif
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
    return mat._m00_m10_m20_m30 * pos.x + (mat._m01_m11_m21_m31 * pos.y + (mat._m02_m12_m22_m32 * pos.z + mat._m03_m13_m23_m33));
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

float2 lilCalcUV(float2 uv, float4 uv_st, float4 uv_sr)
{
    float2 outuv = uv * uv_st.xy + uv_st.zw + frac(uv_sr.xy * LIL_TIME);
    outuv = lilRotateUV(outuv, uv_sr.z + uv_sr.w * LIL_TIME);
    return outuv;
}

float2 lilCalcDecalUV(float2 uv, float4 uv_ST, float angle, bool isLeftOnly, bool isRightOnly, bool shouldCopy, bool shouldFlipMirror, bool shouldFlipCopy, bool isRightHand)
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

float2 lilCalcMatCapUV(float3 normalWS)
{
    #if LIL_MATCAP_MODE == 0
        // Simple
        return mul((float3x3)LIL_MATRIX_V, normalWS).xy * 0.5 + 0.5;
    #elif LIL_MATCAP_MODE == 1
        // Fix Z-Rotation
        bool isMirror = unity_CameraProjection._m20 != 0.0 || unity_CameraProjection._m21 != 0.0;
        float2 outuv = mul((float3x3)LIL_MATRIX_V, normalWS).xy * 0.5;
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

//------------------------------------------------------------------------------------------------------------------------------
// Sub Texture
float4 lilGetSubTex(bool existsTex, Texture2D tex, float4 uv_ST, float angle, float2 uv, float nv, SamplerState sampstate, bool isDecal, bool isLeftOnly, bool isRightOnly, bool shouldCopy, bool shouldFlipMirror, bool shouldFlipCopy, bool isMSDF, bool isRightHand, float4 decalAnimation, float4 decalSubParam)
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

float4 lilGetSubTexWithoutAnimation(bool existsTex, Texture2D tex, float4 uv_ST, float angle, float2 uv, float nv, SamplerState sampstate, bool isDecal, bool isLeftOnly, bool isRightOnly, bool shouldCopy, bool shouldFlipMirror, bool shouldFlipCopy, bool isMSDF, bool isRightHand)
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
float3 lilGetSHZero()
{
    // L0
    #ifdef LIL_COLORSPACE_GAMMA
        return LinearToSRGB(float3(unity_SHAr.w,unity_SHAg.w,unity_SHAb.w));
    #else
        return float3(unity_SHAr.w,unity_SHAg.w,unity_SHAb.w);
    #endif
}

float3 lilGetSHLength()
{
    float3 x1, x2;
    // L0 & L1
    x1.r = length(unity_SHAr);
    x1.g = length(unity_SHAg);
    x1.b = length(unity_SHAb);

    #if LIL_SH_DIRECT_MODE == 0
        // L2
        x2.r = sqrt(dot(unity_SHBr,unity_SHBr)+dot(unity_SHC.r,unity_SHC.r));
        x2.g = sqrt(dot(unity_SHBg,unity_SHBg)+dot(unity_SHC.g,unity_SHC.g));
        x2.b = sqrt(dot(unity_SHBb,unity_SHBb)+dot(unity_SHC.b,unity_SHC.b));
    #elif LIL_SH_DIRECT_MODE == 1
        // L2
        x2.r = length(unity_SHBr);
        x2.g = length(unity_SHBg);
        x2.b = length(unity_SHBb);
    #elif LIL_SH_DIRECT_MODE == 2
        // L2
        x2 = 0.0;
    #else
        // L2
        x2.r = length(unity_SHBr);
        x2.g = length(unity_SHBg);
        x2.b = length(unity_SHBb);
    #endif

    #ifdef UNITY_COLORSPACE_GAMMA
        return LinearToSRGB(x1 + x2);
    #else
        return x1 + x2;
    #endif
}

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

float3 lilGetSHMagic()
{
    // return ShadeSH9(normalize(unity_SHAr+unity_SHAg+unity_SHAb));
    float4 N = normalize(unity_SHAr+unity_SHAg+unity_SHAb);
    float3 res;
    res.r = dot(unity_SHAr, N);
    res.g = dot(unity_SHAg, N);
    res.b = dot(unity_SHAb, N);

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

float3 lilGetSHStrongest()
{
    return SampleSH(normalize(unity_SHAr.rgb+unity_SHAg.rgb+unity_SHAb.rgb));
}

float3 lilGetSHWeakest()
{
    return SampleSH(-normalize(unity_SHAr.rgb+unity_SHAg.rgb+unity_SHAb.rgb));
}

float3 lilGetSHMax()
{
    float3 x1;
	x1 =         SampleSH(float3(+1.0, 0.0, 0.0));
	x1 = max(x1, SampleSH(float3(-1.0, 0.0, 0.0)));
	x1 = max(x1, SampleSH(float3( 0.0,+1.0, 0.0)));
	x1 = max(x1, SampleSH(float3( 0.0,-1.0, 0.0)));
	x1 = max(x1, SampleSH(float3( 0.0, 0.0,+1.0)));
	x1 = max(x1, SampleSH(float3( 0.0, 0.0,-1.0)));
    return x1;
}

float3 lilGetSHMin()
{
    float3 x1;
	x1 =         SampleSH(float3(+1.0, 0.0, 0.0));
	x1 = min(x1, SampleSH(float3(-1.0, 0.0, 0.0)));
	x1 = min(x1, SampleSH(float3( 0.0,+1.0, 0.0)));
	x1 = min(x1, SampleSH(float3( 0.0,-1.0, 0.0)));
	x1 = min(x1, SampleSH(float3( 0.0, 0.0,+1.0)));
	x1 = min(x1, SampleSH(float3( 0.0, 0.0,-1.0)));
    return x1;
}

float3 lilGetSHAverage()
{
    float3 x1 = 0;
    x1 += SampleSH(float3(+1.0, 0.0, 0.0));
    x1 += SampleSH(float3(-1.0, 0.0, 0.0));
    x1 += SampleSH(float3( 0.0, 0.0,+1.0));
    x1 += SampleSH(float3( 0.0, 0.0,-1.0));
    x1 /= 4;
    x1 += SampleSH(float3( 0.0,+1.0, 0.0));
    x1 += SampleSH(float3( 0.0,-1.0, 0.0));
    return x1 / 3;
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

//------------------------------------------------------------------------------------------------------------------------------
// Lighting
float3 lilGetLightColor()
{
    #if LIL_SH_DIRECT_MODE == 0
        // Length of unity_SHA, SHB, SHC
        return saturate(_MainLightColor.rgb + lilGetSHLength());
    #elif LIL_SH_DIRECT_MODE == 1
        // Length of unity_SHA, SHB (Arktoon / ArxCharacterShaders)
        return saturate(_MainLightColor.rgb + lilGetSHLength());
    #elif LIL_SH_DIRECT_MODE == 2
        // Length of unity_SHA
        return saturate(_MainLightColor.rgb + lilGetSHLength());
    #elif LIL_SH_DIRECT_MODE == 3
        // Length of unity_SHA, SHB / ShadeSH9(unity_SHA) (Poiyomi Toon Shader)
        return saturate(lerp(saturate(lilGetSHMagic()+_MainLightColor.rgb), lilGetSHLength(), 0.2));
    #elif LIL_SH_DIRECT_MODE == 4
        // unity_SHA.w (VRChat Mobile Toon Lit / MnMrShader / Reflex Shader)
        return saturate(_MainLightColor.rgb + lilGetSHZero());
    #elif LIL_SH_DIRECT_MODE == 5
        // Average value of 2 directions, top and bottom (MToon)
        return saturate(_MainLightColor.rgb + (SampleSH(float3(0.0,1.0,0.0)) + SampleSH(float3(0.0,-1.0,0.0)))*0.5);
    #elif LIL_SH_DIRECT_MODE == 6
        // Maximum value of unity_SHA.w, SampleSH(down), 0.05 (UTS2)
        float unlitIntensity = 1.0;
        float3 shLight = max(lilGetSHZero(), SampleSH(float3(0.0,-1.0,0.0))) * unlitIntensity;
        return clamp(0.05 * unlitIntensity, 1.0, max(_MainLightColor.rgb, shLight));
    #elif LIL_SH_DIRECT_MODE == 7
        // Maximum value of 6 directions (Sunao Shader)
        return saturate(_MainLightColor.rgb + lilGetSHMax());
    #elif LIL_SH_DIRECT_MODE == 8
        // Average value of 6 directions (UnlitWF)
        return saturate(_MainLightColor.rgb + lilGetSHAverage());
    #elif LIL_SH_DIRECT_MODE == 9
        // Strongest direction
        return saturate(_MainLightColor.rgb + lilGetSHStrongest());
    #elif LIL_SH_DIRECT_MODE == 10
        // Approximation of Standard (lilToon)
        return saturate(_MainLightColor.rgb + lilGetSHToon());
    #endif
}

float3 lilGetIndirLightColor()
{
    #if LIL_SH_INDIRECT_MODE == 0
        return saturate(lilGetSHZero());
    #elif LIL_SH_INDIRECT_MODE == 1
        return saturate(lilGetSHMin());
    #elif LIL_SH_INDIRECT_MODE == 2
        return saturate(lilGetSHWeakest());
    #elif LIL_SH_INDIRECT_MODE == 3
        return saturate(lilGetSHToonMin());
    #endif
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

float3 lilGetVertexLights(float3 positionWS)
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

        return outCol;
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

        return outCol;
    #endif
}

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
#if !defined(LIL_LITE) && !defined(LIL_BAKER) && defined(LIL_FEATURE_SHADOW)
void lilGetShading(inout float4 col, inout float shadowmix, float3 albedo, float3 lightColor, float2 uv, float facing, float3 normalDirection, float attenuation, float3 lightDirection, bool cullOff = true)
{
    LIL_BRANCH
    if(_UseShadow)
    {
        // Shade
        float ln = saturate(dot(lightDirection,normalDirection)*0.5+0.5);
        if(Exists_ShadowBorderMask) ln *= LIL_SAMPLE_2D(_ShadowBorderMask, sampler_MainTex, uv).r;
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
        if(Exists_ShadowBlurMask) shadowBlur *= LIL_SAMPLE_2D(_ShadowBlurMask, sampler_MainTex, uv).r;
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
        if(Exists_ShadowStrengthMask) shadowStrength *= LIL_SAMPLE_2D(_ShadowStrengthMask, sampler_MainTex, uv).r;
        ln = lerp(1.0, ln, shadowStrength);

        // Shadow Color 1
        float4 shadowColorTex = 0.0;
        if(Exists_ShadowColorTex) shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, sampler_MainTex, uv);
        float3 indirectCol = lerp(albedo, shadowColorTex.rgb, shadowColorTex.a) * _ShadowColor.rgb;
        // Shadow Color 2
        float4 shadow2ndColorTex = 0.0;
        if(Exists_Shadow2ndColorTex) shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, sampler_MainTex, uv);
        shadow2ndColorTex.rgb = lerp(albedo, shadow2ndColorTex.rgb, shadow2ndColorTex.a) * _Shadow2ndColor.rgb;
        ln2 = _Shadow2ndColor.a - ln2 * _Shadow2ndColor.a;
        indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, ln2);
        // Multiply Main Color
        indirectCol = lerp(indirectCol, indirectCol*albedo, _ShadowMainStrength);

        // Apply Light
        float3 directCol = albedo * lightColor;
        indirectCol = indirectCol * lightColor;

        // Environment Light
        indirectCol = lerp(indirectCol, albedo, lilGetIndirLightColor() * _ShadowEnvStrength);
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
void lilGetShadingLite(inout float4 col, inout float shadowmix, float3 albedo, float3 lightColor, float2 uv, float facing, float3 normalDirection, float3 lightDirection, bool cullOff = true)
{
    LIL_BRANCH
    if(_UseShadow)
    {
        // Shade
        float ln = saturate(dot(lightDirection,normalDirection)*0.5+0.5);

        // Toon
        ln = lilTooning(ln, _ShadowBorder, _ShadowBlur);

        if(cullOff)
        {
            // Force shadow on back face
            float bfshadow = (facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
            ln *= bfshadow;
        }

        // Copy
        shadowmix = ln;

        // Shadow Color
        float4 shadowColorTex = 1.0;
        if(Exists_ShadowColorTex) shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, sampler_MainTex, uv);
        float3 indirectCol = shadowColorTex.rgb;
        // Apply Light
        float3 directCol = albedo * lightColor;
        indirectCol = indirectCol * lightColor;
        // Environment Light
        indirectCol = lerp(indirectCol, albedo, lilGetIndirLightColor() * _ShadowEnvStrength);
        // Fix
        indirectCol = min(indirectCol, directCol);

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
// Tessellation
float lilCalcEdgeTessFactor(float3 wpos0, float3 wpos1, float edgeLen)
{
    float dist = distance(0.5 * (wpos0+wpos1), _WorldSpaceCameraPos);
    return max(distance(wpos0, wpos1) * _ScreenParams.y / (edgeLen * dist), 1.0);
}

#endif
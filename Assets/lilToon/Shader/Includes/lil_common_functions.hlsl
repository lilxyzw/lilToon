#ifndef LIL_FUNCTIONS_INCLUDED
#define LIL_FUNCTIONS_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Optimized inverse trigonometric function
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
// Math

// Tooning
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

// Optimized matrix calculation
float4 lilOptMul(float4x4 mat, float3 pos)
{
    return mat._m00_m10_m20_m30 * pos.x + (mat._m01_m11_m21_m31 * pos.y + (mat._m02_m12_m22_m32 * pos.z + mat._m03_m13_m23_m33));
}

// Check if the value is within range
float lilIsIn0to1(float2 f)
{
    return lilIsIn0to1(f.x) * lilIsIn0to1(f.y);
}

float lilIsIn0to1(float2 f, float nv)
{
    return lilIsIn0to1(f.x, nv) * lilIsIn0to1(f.y, nv);
}

// Normal blend in tangent space
float3 lilBlendNormal(float3 dstNormal, float3 srcNormal)
{
    return float3(dstNormal.xy + srcNormal.xy, dstNormal.z * srcNormal.z);
}

float lilMedian(float r, float g, float b)
{
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

float3 lilOrthoNormalize(float3 tangent, float3 normal)
{
    return normalize(tangent - normal * dot(normal, tangent));
}

//------------------------------------------------------------------------------------------------------------------------------
// Position Transform
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
    output.positionWS = lilTransformOStoWS(positionOS);
    output.positionVS = lilTransformWStoVS(output.positionWS);
    output.positionCS = lilTransformWStoCS(output.positionWS);
    output.positionSS = lilTransformCStoSS(output.positionCS);
    return output;
}

lilVertexPositionInputs lilGetVertexPositionInputs(float3 positionOS)
{
    return lilGetVertexPositionInputs(float4(positionOS, 1.0));
}

lilVertexPositionInputs lilReGetVertexPositionInputs(lilVertexPositionInputs output)
{
    output.positionVS = lilTransformWStoVS(output.positionWS);
    output.positionCS = lilTransformWStoCS(output.positionWS);
    output.positionSS = lilTransformCStoSS(output.positionCS);
    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Normal Transform
struct lilVertexNormalInputs
{
    float3 tangentWS;
    float3 bitangentWS;
    float3 normalWS;
};

lilVertexNormalInputs lilGetVertexNormalInputs()
{
    lilVertexNormalInputs output;
    output.normalWS     = float3(1.0, 0.0, 0.0);
    output.tangentWS    = float3(1.0, 0.0, 0.0);
    output.bitangentWS  = float3(0.0, 1.0, 0.0);
    return output;
}

lilVertexNormalInputs lilGetVertexNormalInputs(float3 normalOS)
{
    lilVertexNormalInputs output;
    output.normalWS     = lilTransformNormalOStoWS(normalOS, true);
    output.tangentWS    = float3(1.0, 0.0, 0.0);
    output.bitangentWS  = float3(0.0, 1.0, 0.0);
    return output;
}

lilVertexNormalInputs lilGetVertexNormalInputs(float3 normalOS, float4 tangentOS)
{
    lilVertexNormalInputs output;
    output.normalWS     = lilTransformNormalOStoWS(normalOS, true);
    output.tangentWS    = lilTransformDirOStoWS(tangentOS.xyz, true);
    output.bitangentWS  = cross(output.normalWS, output.tangentWS) * (tangentOS.w * LIL_NEGATIVE_SCALE);
    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Outline
float lilGetOutlineWidth(float3 positionOS, float2 uv, float4 color, float outlineWidth, TEXTURE2D(outlineWidthMask), lilBool outlineVertexR2Width, lilBool outlineFixWidth LIL_SAMP_IN_FUNC(samp))
{
    outlineWidth *= 0.01;
    if(Exists_OutlineWidthMask) outlineWidth *= LIL_SAMPLE_2D_LOD(outlineWidthMask, samp, uv, 0).r;
    if(outlineVertexR2Width) outlineWidth *= color.r;
    if(outlineFixWidth) outlineWidth *= saturate(length(lilHeadDirection(lilToAbsolutePositionWS(lilOptMul(LIL_MATRIX_M, positionOS).xyz))));
    return outlineWidth;
}

float3 lilGetOutlineVector(float3x3 tbnOS, float2 uv, float outlineVectorScale, TEXTURE2D(outlineVectorTex) LIL_SAMP_IN_FUNC(samp))
{
    float3 outlineVector = UnpackNormalScale(LIL_SAMPLE_2D_LOD(outlineVectorTex, samp, uv, 0), outlineVectorScale);
    outlineVector = mul(outlineVector, tbnOS);
    return outlineVector;
}

void lilCalcOutlinePosition(inout float3 positionOS, float2 uv, float4 color, float3 normalOS, float3x3 tbnOS, float outlineWidth, TEXTURE2D(outlineWidthMask), lilBool outlineVertexR2Width, lilBool outlineFixWidth, float outlineVectorScale, TEXTURE2D(outlineVectorTex) LIL_SAMP_IN_FUNC(samp))
{
    float width = lilGetOutlineWidth(positionOS, uv, color, outlineWidth, outlineWidthMask, outlineVertexR2Width, outlineFixWidth LIL_SAMP_IN(samp));
    float3 outlineN = normalOS;
    if(Exists_OutlineVectorTex) outlineN = lilGetOutlineVector(tbnOS, uv, outlineVectorScale, outlineVectorTex LIL_SAMP_IN(samp));
    positionOS += outlineN * width;
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
// Color

// http://chilliant.blogspot.com/2012/08/srgb-approximations-for-hlsl.html?m=1
float3 lilLinearToSRGB(float3 col)
{
    return saturate(1.055 * pow(abs(col), 0.416666667) - 0.055);
}

float3 lilSRGBToLinear(float3 col)
{
    return col * (col * (col * 0.305306011 + 0.682171111) + 0.012522878);
}

float3 lilBlendColor(float3 dstCol, float3 srcCol, float3 srcA, uint blendMode)
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

float3 lilBlendColor(float3 dstCol, float3 srcCol, float srcA, uint blendMode)
{
    return lilBlendColor(dstCol, srcCol, float3(srcA,srcA,srcA), blendMode);
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

float3 lilGradationMap(float3 col, TEXTURE2D(gradationMap), float strength)
{
    if(strength == 0.0) return col;
    #if !defined(LIL_COLORSPACE_GAMMA)
        col = lilLinearToSRGB(col);
    #endif
    float R = LIL_SAMPLE_1D(gradationMap, sampler_linear_clamp, col.r).r;
    float G = LIL_SAMPLE_1D(gradationMap, sampler_linear_clamp, col.g).g;
    float B = LIL_SAMPLE_1D(gradationMap, sampler_linear_clamp, col.b).b;
    float3 outrgb = float3(R,G,B);
    #if !defined(LIL_COLORSPACE_GAMMA)
        col = lilSRGBToLinear(col);
        outrgb = lilSRGBToLinear(outrgb);
    #endif
    return lerp(col, outrgb, strength);
}

//------------------------------------------------------------------------------------------------------------------------------
// UV

// Rotation
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

// Tiling, offset, animation calculations
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
    float2 outuv = uv * uv_st.xy + uv_st.zw;
    outuv = lilRotateUV(outuv, uv_sr.z + uv_sr.w * LIL_TIME) + frac(uv_sr.xy * LIL_TIME);
    return outuv;
}

float2 lilCalcUVWithoutAnimation(float2 uv, float4 uv_st, float4 uv_sr)
{
    return lilRotateUV(uv * uv_st.xy + uv_st.zw, uv_sr.z);
}

float2 lilCalcDoubleSideUV(float2 uv, float facing, float shiftBackfaceUV)
{
    return facing < (shiftBackfaceUV-1.0) ? uv + float2(1.0,0.0) : uv;
}

// Decal
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

// MatCap
float2 lilCalcMatCapUV(float2 uv1, float3 normalWS, float3 viewDirection, float3 headDirection, float4 matcap_ST, float2 matcapBlendUV1, bool zRotCancel, bool matcapPerspective, float matcapVRParallaxStrength)
{
    #if LIL_MATCAP_MODE == 0
        // Simple
        return mul((float3x3)LIL_MATRIX_V, normalWS).xy * 0.5 + 0.5;
    #elif LIL_MATCAP_MODE == 1
        #if defined(USING_STEREO_MATRICES)
            float3 normalVD = lerp(headDirection, viewDirection, matcapVRParallaxStrength);
        #else
            float3 normalVD = viewDirection;
        #endif
        normalVD = matcapPerspective ? normalVD : LIL_MATRIX_V._m20_m21_m22;
        float3 bitangentVD = zRotCancel ? float3(0,1,0) : LIL_MATRIX_V._m10_m11_m12;
        bitangentVD = lilOrthoNormalize(bitangentVD, normalVD);
        float3 tangentVD = cross(normalVD, bitangentVD);
        float3x3 tbnVD = float3x3(tangentVD, bitangentVD, normalVD);
        float2 uvMat = mul(tbnVD, normalWS).xy;
        uvMat = lerp(uvMat, uv1*2-1, matcapBlendUV1);
        uvMat = uvMat * matcap_ST.xy + matcap_ST.zw;
        uvMat = uvMat * 0.5 + 0.5;
        return uvMat;
    #endif
}

// Panorama
float2 lilGetPanoramaUV(float3 viewDirection)
{
    return float2(lilAtan(viewDirection.x, viewDirection.z), lilAcos(viewDirection.y)) * LIL_INV_PI;
}

// Parallax
void lilParallax(inout float2 uvMain, inout float2 uv, lilBool useParallax, float2 parallaxOffset, TEXTURE2D(parallaxMap), float parallaxScale, float parallaxOffsetParam)
{
    LIL_BRANCH
    if(useParallax)
    {
        float height = (LIL_SAMPLE_2D_LOD(parallaxMap,sampler_linear_repeat,uvMain,0).r - parallaxOffsetParam) * parallaxScale;
        uvMain += height * parallaxOffset;
        uv += height * parallaxOffset;
    }
}

void lilPOM(inout float2 uvMain, inout float2 uv, lilBool useParallax, float4 uv_st, float3 parallaxViewDirection, TEXTURE2D(parallaxMap), float parallaxScale, float parallaxOffsetParam)
{
    #define LIL_POM_DETAIL 200
    LIL_BRANCH
    if(useParallax)
    {
        float height;
        float height2;
        float3 rayStep = -parallaxViewDirection;
        float3 rayPos = float3(uvMain, 1.0) + (1.0-parallaxOffsetParam) * parallaxScale * parallaxViewDirection;
        rayStep.xy *= uv_st.xy;
        rayStep = rayStep / LIL_POM_DETAIL;
        rayStep.z /= parallaxScale;

        for(int i = 0; i < LIL_POM_DETAIL * 2 * parallaxScale; ++i)
        {
            height2 = height;
            rayPos += rayStep;
            height = LIL_SAMPLE_2D_LOD(parallaxMap,sampler_linear_repeat,rayPos.xy,0).r;
            if(height >= rayPos.z) break;
        }

        float2 prevObjPoint = rayPos.xy - rayStep.xy;
        float nextHeight = height - rayPos.z;
        float prevHeight = height2 - rayPos.z + rayStep.z;

        float weight = nextHeight / (nextHeight - prevHeight);
        rayPos.xy = lerp(rayPos.xy, prevObjPoint, weight);

        uv += rayPos.xy - uvMain;
        uvMain = rayPos.xy;
    }
}

//------------------------------------------------------------------------------------------------------------------------------
// Effect
float lilCalcBlink(float4 blink)
{
    float outBlink = sin(LIL_TIME * blink.z + blink.w) * 0.5 + 0.5;
    if(blink.y > 0.5) outBlink = round(outBlink);
    return lerp(1.0, outBlink, blink.x);
}

void lilCalcDissolve(
    inout float alpha,
    inout float dissolveAlpha,
    float2 uv,
    float3 positionOS,
    float4 dissolveParams,
    float4 dissolvePos,
    TEXTURE2D(dissolveMask),
    float4 dissolveMask_ST
    LIL_SAMP_IN_FUNC(samp))
{
    if(dissolveParams.r)
    {
        float dissolveMaskVal = 1.0;
        if(dissolveParams.r == 1.0)
        {
            dissolveMaskVal = LIL_SAMPLE_2D(dissolveMask, samp, lilCalcUV(uv, dissolveMask_ST)).r;
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
    float dissolveNoiseStrength
    LIL_SAMP_IN_FUNC(samp))
{
    if(dissolveParams.r)
    {
        float dissolveMaskVal = 1.0;
        float dissolveNoise = 0.0;
        if(dissolveParams.r == 1.0)
        {
            dissolveMaskVal = LIL_SAMPLE_2D(dissolveMask, samp, lilCalcUV(uv, dissolveMask_ST)).r;
        }
        dissolveNoise = LIL_SAMPLE_2D(dissolveNoiseMask, samp, lilCalcUV(uv, dissolveNoiseMask_ST, dissolveNoiseMask_ScrollRotate.xy)).r - 0.5;
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
// Sub Texture
float4 lilGetSubTex(
    bool existsTex,
    TEXTURE2D(tex),
    float4 uv_ST,
    float angle,
    float2 uv,
    float nv,
    bool isDecal,
    bool isLeftOnly,
    bool isRightOnly,
    bool shouldCopy,
    bool shouldFlipMirror,
    bool shouldFlipCopy,
    bool isMSDF,
    bool isRightHand,
    float4 decalAnimation,
    float4 decalSubParam
    LIL_SAMP_IN_FUNC(samp))
{
    #if defined(LIL_FEATURE_DECAL)
        float2 uv2 = lilCalcDecalUV(uv, uv_ST, angle, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, isRightHand);
        #if defined(LIL_FEATURE_ANIMATE_DECAL)
            float2 uv2samp = lilCalcAtlasAnimation(uv2, decalAnimation, decalSubParam);
        #else
            float2 uv2samp = uv2;
        #endif
        float4 outCol = 1.0;
        if(existsTex) outCol = LIL_SAMPLE_2D(tex,samp,uv2samp);
        LIL_BRANCH
        if(isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
        LIL_BRANCH
        if(isDecal) outCol.a *= lilIsIn0to1(uv2, saturate(nv-0.05));
        return outCol;
    #else
        float2 uv2 = lilCalcUV(uv, uv_ST, angle);
        float4 outCol = 1.0;
        if(existsTex) outCol = LIL_SAMPLE_2D(tex,samp,uv2);
        LIL_BRANCH
        if(isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
        return outCol;
    #endif
}

float4 lilGetSubTexWithoutAnimation(
    bool existsTex,
    TEXTURE2D(tex),
    float4 uv_ST,
    float angle,
    float2 uv,
    float nv,
    bool isDecal,
    bool isLeftOnly,
    bool isRightOnly,
    bool shouldCopy,
    bool shouldFlipMirror,
    bool shouldFlipCopy,
    bool isMSDF,
    bool isRightHand
    LIL_SAMP_IN_FUNC(samp))
{
    #if defined(LIL_FEATURE_DECAL)
        float2 uv2 = lilCalcDecalUV(uv, uv_ST, angle, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, isRightHand);
        float4 outCol = 1.0;
        if(existsTex) outCol = LIL_SAMPLE_2D(tex,samp,uv2);
        LIL_BRANCH
        if(isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
        LIL_BRANCH
        if(isDecal) outCol.a *= lilIsIn0to1(uv2, saturate(nv-0.05));
        return outCol;
    #else
        float2 uv2 = lilCalcUV(uv, uv_ST, angle);
        float4 outCol = 1.0;
        if(existsTex) outCol = LIL_SAMPLE_2D(tex,samp,uv2);
        LIL_BRANCH
        if(isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
        return outCol;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Light Direction
float3 lilGetLightDirection(float4 lightDirectionOverride)
{
    #if LIL_LIGHT_DIRECTION_MODE == 0
        return normalize(LIL_MAINLIGHT_DIRECTION + lightDirectionOverride.xyz);
    #else
        return normalize(LIL_MAINLIGHT_DIRECTION * lilLuminance(LIL_MAINLIGHT_COLOR) + 
                        unity_SHAr.xyz * 0.333333 + unity_SHAg.xyz * 0.333333 + unity_SHAb.xyz * 0.333333 + 
                        lightDirectionOverride.xyz);
    #endif
}
float3 lilGetLightDirection()
{
    return lilGetLightDirection(float4(0.0,0.001,0.0,0.0));
}

float3 lilGetLightDirection(float3 positionWS)
{
    #if defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
        return normalize(LIL_MAINLIGHT_DIRECTION - positionWS);
    #else
        return LIL_MAINLIGHT_DIRECTION;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// SH Lighting
float3 lilShadeSH9(float4 normalWS)
{
    float3 res;
    res.r = dot(unity_SHAr,normalWS);
    res.g = dot(unity_SHAg,normalWS);
    res.b = dot(unity_SHAb,normalWS);
    float4 vB = normalWS.xyzz * normalWS.yzzx;
    res.r += dot(unity_SHBr,vB);
    res.g += dot(unity_SHBg,vB);
    res.b += dot(unity_SHBb,vB);
    res += unity_SHC.rgb * (normalWS.x * normalWS.x - normalWS.y * normalWS.y);
    #ifdef LIL_COLORSPACE_GAMMA
        res = lilLinearToSRGB(res);
    #endif
    return res;
}

float3 lilShadeSH9(float3 normalWS)
{
    return lilShadeSH9(float4(normalWS,1.0));
}

float3 lilShadeSH9LPPV(float4 normalWS, float3 positionWS)
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
    float3 res;
    res.r = dot(SHAr,normalWS);
    res.g = dot(SHAg,normalWS);
    res.b = dot(SHAb,normalWS);
    float4 vB = normalWS.xyzz * normalWS.yzzx;
    res.r += dot(unity_SHBr,vB);
    res.g += dot(unity_SHBg,vB);
    res.b += dot(unity_SHBb,vB);
    res += unity_SHC.rgb * (normalWS.x * normalWS.x - normalWS.y * normalWS.y);
    #ifdef LIL_COLORSPACE_GAMMA
        res = lilLinearToSRGB(res);
    #endif
    return res;
}

float3 lilShadeSH9LPPV(float3 normalWS, float3 positionWS)
{
    return lilShadeSH9LPPV(float4(normalWS,1.0), positionWS);
}

float3 lilGetSHToon(float4 lightDirectionOverride)
{
    return lilShadeSH9(lilGetLightDirection(lightDirectionOverride) * 0.666666);
}

float3 lilGetSHToon()
{
    return lilGetSHToon(float4(0.0,0.001,0.0,0.0));
}

float3 lilGetSHToon(float3 positionWS, float4 lightDirectionOverride)
{
    return lilShadeSH9LPPV(lilGetLightDirection(lightDirectionOverride) * 0.666666, positionWS);
}

float3 lilGetSHToon(float3 positionWS)
{
    return lilGetSHToon(positionWS, float4(0.0,0.001,0.0,0.0));
}

float3 lilGetSHToonMin(float4 lightDirectionOverride)
{
    return lilShadeSH9(-lilGetLightDirection(lightDirectionOverride) * 0.666666);
}

float3 lilGetSHToonMin()
{
    return lilGetSHToonMin(float4(0.0,0.001,0.0,0.0));
}

float3 lilGetSHToonMin(float3 positionWS, float4 lightDirectionOverride)
{
    return lilShadeSH9LPPV(-lilGetLightDirection(lightDirectionOverride) * 0.666666, positionWS);
}

float3 lilGetSHToonMin(float3 positionWS)
{
    return lilGetSHToonMin(positionWS, float4(0.0,0.001,0.0,0.0));
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
    #ifdef LIL_COLORSPACE_GAMMA
        shMax = lilLinearToSRGB(shMax);
        shMin = lilLinearToSRGB(shMin);
    #endif
}

void lilGetToonSHDoubleLPPV(float3 lightDirection, float3 positionWS, out float3 shMax, out float3 shMin)
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
    #ifdef LIL_COLORSPACE_GAMMA
        shMax = lilLinearToSRGB(shMax);
        shMin = lilLinearToSRGB(shMin);
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Lighting
float3 lilGetLightColor()
{
    return LIL_MAINLIGHT_COLOR + lilGetSHToon();
}

float3 lilGetLightColor(float3 positionWS)
{
    return LIL_MAINLIGHT_COLOR + lilGetSHToon(positionWS);
}

float3 lilGetIndirLightColor()
{
    return saturate(lilGetSHToonMin());
}

float3 lilGetIndirLightColor(float3 positionWS)
{
    return saturate(lilGetSHToonMin(positionWS));
}

void lilGetLightColorDouble(float3 lightDirection, out float3 lightColor, out float3 indLightColor)
{
    float3 shMax, shMin;
    lilGetToonSHDouble(lightDirection, shMax, shMin);
    lightColor = LIL_MAINLIGHT_COLOR + shMax;
    indLightColor = saturate(shMin);
}

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

float3 lilGetAnisotropyNormalWS(float3 normalWS, float3 anisoTangentWS, float3 anisoBitangentWS, float3 viewDirection, float anisotropy)
{
    float3 anisoDirectionWS = anisotropy > 0.0 ? anisoBitangentWS : anisoTangentWS;
    anisoDirectionWS = lilOrthoNormalize(viewDirection, anisoDirectionWS);
    return normalize(lerp(normalWS, anisoDirectionWS, abs(anisotropy)));
}

//------------------------------------------------------------------------------------------------------------------------------
// Glitter
float3 lilCalcGlitter(float2 uv, float3 normalDirection, float3 viewDirection, float3 lightDirection, float4 glitterParams1, float4 glitterParams2)
{
    // glitterParams1
    // x: Scale, y: Scale, z: Size, w: Contrast
    // glitterParams2
    // x: Speed, y: Angle, z: Light Direction, w: 

    float2 pos = abs(uv * glitterParams1.xy + glitterParams1.xy);

    #if defined(SHADER_API_D3D9) || defined(SHADER_API_D3D11_9X)
        #define M1 46203.4357
        #define M2 21091.5327
        #define M3 35771.1966
        float2 q = trunc(pos);
        float4 q2 = float4(q.x, q.y, q.x+1, q.y+1);
        float3 noise0 = frac(sin(dot(q2.xy,float2(12.9898,78.233))) * float3(M1, M2, M3));
        float3 noise1 = frac(sin(dot(q2.zy,float2(12.9898,78.233))) * float3(M1, M2, M3));
        float3 noise2 = frac(sin(dot(q2.xw,float2(12.9898,78.233))) * float3(M1, M2, M3));
        float3 noise3 = frac(sin(dot(q2.zw,float2(12.9898,78.233))) * float3(M1, M2, M3));
    #else
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
    #endif

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
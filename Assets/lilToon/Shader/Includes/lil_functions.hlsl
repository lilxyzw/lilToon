#ifndef LIL_FUNCTIONS_INCLUDED
#define LIL_FUNCTIONS_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Transform
float3 lilTransformNormalOStoWS(float3 normalOS)
{
    #ifdef UNITY_ASSUME_UNIFORM_SCALING
        #ifndef SHADER_STAGE_RAY_TRACING
            return mul((float3x3)GetObjectToWorldMatrix(), normalOS);
        #else
            return mul((float3x3)unity_ObjectToWorld, normalOS);
        #endif
    #else
        return mul(normalOS, (float3x3)unity_WorldToObject);
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
    #if defined(LIL_BRP)
        output.positionWS = mul(unity_ObjectToWorld, float4(positionOS.xyz,1.0)).xyz;
        output.positionVS = mul(UNITY_MATRIX_V, float4(output.positionWS, 1.0)).xyz;
        output.positionCS = UnityWorldToClipPos(output.positionWS);
        output.positionSS = ComputeGrabScreenPos(output.positionCS);
    #elif defined(LIL_LWRP)
        VertexPositionInputs input = GetVertexPositionInputs(positionOS.xyz);
        output.positionWS = input.positionWS;
        output.positionVS = input.positionVS;
        output.positionCS = input.positionCS;
        output.positionSS = ComputeScreenPos(input.positionCS);
    #else
        VertexPositionInputs input = GetVertexPositionInputs(positionOS.xyz);
        output.positionWS = input.positionWS;
        output.positionVS = input.positionVS;
        output.positionCS = input.positionCS;
        output.positionSS = input.positionNDC;
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
    output.tangentWS    = mul((float3x3)unity_ObjectToWorld, tangentOS.xyz);
    output.bitangentWS  = cross(output.normalWS, output.tangentWS) * tangentOS.w * unity_WorldTransformParams.w;
    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Optimized inverse trigonometric function
// https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/
float lilAcos(float x) 
{ 
    float ox = abs(x); 
    float res = -0.156583f * ox + LIL_HALF_PI; 
    res *= sqrt(1.0f - ox); 
    return (x >= 0) ? res : LIL_PI - res; 
}

float lilAsin(float x)
{
    return LIL_HALF_PI - lilAcos(x);
}

float lilAtanPos(float x) 
{ 
    float t0 = (x < 1.0f) ? x : 1.0f / x;
    float t1 = t0 * t0;
    float poly = 0.0872929f;
    poly = -0.301895f + poly * t1;
    poly = 1.0f + poly * t1;
    poly = poly * t0;
    return (x < 1.0f) ? poly : LIL_HALF_PI - poly;
}

float lilAtan(float x) 
{     
    float t0 = lilAtanPos(abs(x));     
    return (x < 0.0f) ? -t0 : t0; 
}

float lilAtan2(float x, float y)
{
    return lilAtan(x/y) + LIL_PI * (y<0) * (x<0?-1:1);
}

//------------------------------------------------------------------------------------------------------------------------------
// Math
float lilIsIn0to1(float f)
{
    return saturate(f) == f;
}

float lilIsIn0to1(float2 f)
{
    return lilIsIn0to1(f.x) * lilIsIn0to1(f.y);
}

float3 lilBlendColor(float3 dstCol, float3 srcCol, float srcA, int blendMode)
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

float3 lilBlendNormal(float3 dstNormal, float3 srcNormal)
{
    return float3(dstNormal.xy + srcNormal.xy, dstNormal.z * srcNormal.z);
}

float lilLuminance(float3 rgb)
{
    #ifdef LIL_COLORSPACE_GAMMA
        return dot(rgb, float3(0.22, 0.707, 0.071));
    #else
        return dot(rgb, float3(0.0396819152, 0.458021790, 0.00609653955));
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Lighting
//unity_SHAr unity_SHAg unity_SHAb unity_SHBr unity_SHBg unity_SHBb unity_SHC
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

float3 lilGetLightMapDirection(float2 uv)
{
    #if defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DIRLIGHTMAP)
        float4 lightmapDirection = LIL_SAMPLE_LIGHTMAP(LIL_DIRLIGHTMAP_TEX,  LIL_LIGHTMAP_SAMP, uv);
        return lightmapDirection.xyz * 2.0 - 1.0;
    #else
        return 0;
    #endif
}

float3 lilGetLightDirection()
{
    #if LIL_LIGHT_DIRECTION_MODE == 0
        return normalize(_MainLightPosition.xyz + float3(0.0,0.001,0.0));
    #else
        return normalize(_MainLightPosition.xyz * lilLuminance(_MainLightColor.rgb) + 
                        unity_SHAr.xyz + unity_SHAg.xyz + unity_SHAb.xyz + 
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

float3 lilGetVertexLights(float3 positionWS)
{
    #ifdef LIL_BRP
        float4 toLightX = unity_4LightPosX0 - positionWS.x;
        float4 toLightY = unity_4LightPosY0 - positionWS.y;
        float4 toLightZ = unity_4LightPosZ0 - positionWS.z;

        float4 lengthSq = toLightX * toLightX;
        lengthSq += toLightY * toLightY;
        lengthSq += toLightZ * toLightZ;
        lengthSq = max(lengthSq, 0.000001);

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
            for (uint lightIndex = 0; lightIndex < lightsCount; ++lightIndex)
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
        int additionalLightsCount = GetAdditionalLightsCount();
        for (int adlcount = 0; adlcount < additionalLightsCount; ++adlcount)
        {
            Light light = GetAdditionalLight(adlcount, positionWS);
            outCol += light.distanceAttenuation * light.shadowAttenuation * light.color;
        }
    #endif
    return outCol;
}

float3 lilFresnelTerm(float3 F0, float cosA)
{
    return F0 + (1-F0) * (1-cosA) * (1-cosA) * (1-cosA) * (1-cosA) * (1-cosA);
}

float3 lilFresnelLerp(float3 F0, float3 F90, float cosA)
{
    return lerp(F0, F90, (1-cosA) * (1-cosA) * (1-cosA) * (1-cosA) * (1-cosA));
}

float3 lilCalcSpecular(float nv, float nl, float nh, float lh, float roughness, float3 specular, bool isSpecularToon, float attenuation = 1.0)
{
    #if LIL_SPECULAR_MODE == 0
        // BRP Specular
        float roughness2 = max(roughness, 0.002);

        float lambdaV = nl * (nv * (1 - roughness2) + roughness2);
        float lambdaL = nv * (nl * (1 - roughness2) + roughness2);
        #if defined(SHADER_API_SWITCH)
            float sjggx =  0.5f / (lambdaV + lambdaL + 1e-4f);
        #else
            float sjggx =  0.5f / (lambdaV + lambdaL + 1e-5f);
        #endif

        float r2 = roughness2 * roughness2;
        float d = (nh * r2 - nh) * nh + 1.0f;
        float ggx = r2 / (d * d + 1e-7f);

        //float specularTerm = SmithJointGGXVisibilityTerm(nl,nv,roughness2) * GGXTerm(nh,roughness2) * LIL_PI;
        float specularTerm = sjggx * ggx;
        #ifdef UNITY_COLORSPACE_GAMMA
            specularTerm = sqrt(max(1e-4h, specularTerm));
        #endif
        specularTerm *= nl * attenuation;
        if(isSpecularToon) return step(0.5, specularTerm);
        else               return specularTerm * lilFresnelTerm(specular, lh);
    #elif LIL_SPECULAR_MODE == 1
        // URP Specular
        float roughness2 = max(roughness, 0.002);
        float r2 = roughness2 * roughness2;
        float d = (nh * r2 - nh) * nh + 1.00001;
        half specularTerm = r2 / ((d * d) * max(0.1, lh * lh) * (roughness * 4.0 + 2.0));
        #if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
            specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
        #endif
        if(isSpecularToon) return step(0.5, specularTerm);
        else               return specularTerm * specular;
    #else
        // Fast Specular
        float smoothness = 1.0/max(roughness, 0.002);
        half specularTerm = pow(nh, smoothness);
        if(isSpecularToon) return step(0.5, specularTerm);
        else               return specularTerm * smoothness * 0.1 * specular;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Math
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
    outuv = (outuv + float2(offsetX,offsetY)) * decalSubParam.xy / decalAnimation.xy - decalAnimation.xy;
    outuv = float2(outuv.x,-outuv.y);
    return outuv;
}

float2 lilCalcUVWithoutAnimation(float2 uv, float4 uv_st, float4 uv_sr)
{
    float2 outuv = uv * uv_st.xy + uv_st.zw;
    outuv = lilRotateUV(outuv, uv_sr.z);
    return outuv;
}

float2 lilCalcMatCapUV(float3 normalWS)
{
    float2 outuv = mul((float3x3)LIL_MATRIX_V, normalWS).xy * 0.5;

    bool isMirror = unity_CameraProjection._m20 != 0.0 || unity_CameraProjection._m21 != 0.0;
    float3 matV1 = LIL_MATRIX_V._m00_m01_m02;
    float3 matV2 = float3(-LIL_MATRIX_V._m22, 0.0, LIL_MATRIX_V._m20); //cross(LIL_MATRIX_V._m20_m21_m22, float3(0,1,0));
    float matR = dot(matV1,matV2) / sqrt(dot(matV1,matV1)*dot(matV2,matV2));
    matR = isMirror ? -matR : matR;
    matR = lilAcos(clamp(matR, -1, 1));
    matR = LIL_MATRIX_V._m01 < 0 ? -matR : matR;

    float si,co;
    sincos(matR,si,co);
    outuv = float2(
        outuv.x * co - outuv.y * si,
        outuv.x * si + outuv.y * co
    );
    outuv += 0.5;
    outuv.x = isMirror ? -outuv.x : outuv.x;

    return outuv;
}

float3 lilToneCorrection(float3 c, float4 hsvg)
{
    // gamma
    c = pow(abs(c), hsvg.w);
    float e = 1.0e-10;
    // rgb -> hsv
    float4 p = lerp(float4(c.bg, float2(-1.0, 2.0/3.0)), float4(c.gb, float2(0.0,-1.0/3.0)), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float3 hsv = float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
    // shift
    hsv = float3(hsv.x+hsvg.x,saturate(hsv.y*hsvg.y),saturate(hsv.z*hsvg.z));
    // hsv -> rgb
    return hsv.z - hsv.z * hsv.y + hsv.z * hsv.y * saturate(abs(frac(hsv.x + float3(1.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0) - 1.0);
}

float lilCalcBlink(float4 blink)
{
    float outBlink = sin(LIL_TIME * blink.z + blink.w) * 0.5 + 0.5;
    if(blink.y > 0.5) outBlink = round(outBlink);
    outBlink = 1.0 - mad(blink.x, -outBlink, blink.x);
    return outBlink;
}

void lilGetShading(inout float4 col, inout float shadowmix, float3 albedo, float2 uv, float facing, float3 normalDirection, float attenuation, float3 lightDirection, bool cullOff = true)
{
    UNITY_BRANCH
    if(_UseShadow)
    {
        // Shade
        float directContribution = saturate(dot(lightDirection,normalDirection)*0.5+0.5) * LIL_SAMPLE_2D(_ShadowBorderMask, sampler_MainTex, uv).r;
        float directContribution2nd = directContribution;
        float directContributionB = directContribution;

        // Shadow
        #if defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)
            if(_ShadowReceive) directContribution *= saturate(attenuation + distance(lightDirection, _MainLightPosition.xyz));
            if(_ShadowReceive) directContributionB *= saturate(attenuation + distance(lightDirection, _MainLightPosition.xyz));
        #endif

        // Toon
        _ShadowBlur *= LIL_SAMPLE_2D(_ShadowBlurMask, sampler_MainTex, uv).r;
        float shadowBorderMin = saturate(_ShadowBorder - _ShadowBlur * 0.5);
        float shadowBorderMax = saturate(_ShadowBorder + _ShadowBlur * 0.5);
        directContribution = saturate((directContribution - shadowBorderMin) / (shadowBorderMax - shadowBorderMin));
        float shadow2ndBorderMin = saturate(_Shadow2ndBorder - _Shadow2ndBlur * 0.5);
        float shadow2ndBorderMax = saturate(_Shadow2ndBorder + _Shadow2ndBlur * 0.5);
        directContribution2nd = saturate((directContribution2nd - shadow2ndBorderMin) / (shadow2ndBorderMax - shadow2ndBorderMin));
        float shadowBorderMinB = saturate(shadowBorderMin - _ShadowBorderRange);
        directContributionB = saturate((directContributionB - shadowBorderMinB) / (shadowBorderMax - shadowBorderMinB));

        if(cullOff)
        {
            // Force shadow on back face
            float bfshadow = facing < 0.0 ? 1.0 - _BackfaceForceShadow : 1.0;
            directContribution *= bfshadow;
            directContribution2nd *= bfshadow;
            directContributionB *= bfshadow;
        }

        // Copy
        shadowmix = directContribution;

        // Strength
        #ifdef UNITY_COLORSPACE_GAMMA
            _ShadowStrength = SRGBToLinear(_ShadowStrength);
        #endif
        _ShadowStrength *= LIL_SAMPLE_2D(_ShadowStrengthMask, sampler_MainTex, uv).r;
        directContribution = lerp(1.0, directContribution, _ShadowStrength);

        // Shadow Color 1
        float4 shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, sampler_MainTex, uv);
        float3 indirectCol = lerp(albedo, shadowColorTex.rgb, shadowColorTex.a) * _ShadowColor.rgb;
        // Shadow Color 2
        float4 shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, sampler_MainTex, uv);
        shadow2ndColorTex.rgb = lerp(albedo, shadow2ndColorTex.rgb, shadow2ndColorTex.a) * _Shadow2ndColor.rgb;
        directContribution2nd = _Shadow2ndColor.a - directContribution2nd * _Shadow2ndColor.a;
        indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, directContribution2nd);
        // Multiply Main Color
        indirectCol = lerp(indirectCol, indirectCol*albedo, _ShadowMainStrength);
        // Gradation
        indirectCol = lerp(indirectCol, albedo, directContributionB * _ShadowBorderColor.rgb);
        // Environment Light
        indirectCol = lerp(indirectCol, albedo, lilGetIndirLightColor() * _ShadowEnvStrength);

        // Mix
        col.rgb = lerp(indirectCol, albedo, directContribution);
    }
}

void lilGetShadingLite(inout float4 col, inout float shadowmix, float3 albedo, float2 uv, float facing, float3 normalDirection, float3 lightDirection, bool cullOff = true)
{
    UNITY_BRANCH
    if(_UseShadow)
    {
        // Shade
        float directContribution = saturate(dot(lightDirection,normalDirection)*0.5+0.5);

        // Toon
        float shadowBorderMin = saturate(_ShadowBorder - _ShadowBlur * 0.5);
        float shadowBorderMax = saturate(_ShadowBorder + _ShadowBlur * 0.5);
        directContribution = saturate((directContribution - shadowBorderMin) / (shadowBorderMax - shadowBorderMin));

        if(cullOff)
        {
            // Force shadow on back face
            float bfshadow = facing < 0.0 ? 1.0 - _BackfaceForceShadow : 1.0;
            directContribution *= bfshadow;
        }

        // Copy
        shadowmix = directContribution;

        // Shadow Color
        float4 shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, sampler_MainTex, uv);
        float3 indirectCol = lerp(shadowColorTex.rgb, albedo, lilGetIndirLightColor() * _ShadowEnvStrength);

        // Mix
        col.rgb = lerp(indirectCol, albedo, directContribution);
    }
}

float4 lilGetSubTex(Texture2D tex, float4 uv_ST, float angle, float2 uv, SamplerState sampstate, bool isDecal, bool isLeftOnly, bool isRightOnly, bool shouldCopy, bool shouldFlipMirror, bool shouldFlipCopy, bool isRightHand, float4 decalAnimation, float4 decalSubParam)
{
    float2 uv2 = lilCalcDecalUV(uv, uv_ST, angle, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, isRightHand);
    float2 uv2samp = lilCalcAtlasAnimation(uv2, decalAnimation, decalSubParam);
    float4 outCol = LIL_SAMPLE_2D(tex,sampstate,uv2samp);
    if(isDecal) outCol *= lilIsIn0to1(uv2);
    return outCol;
}

float4 lilGetSubTexWithoutAnimation(Texture2D tex, float4 uv_ST, float angle, float2 uv, SamplerState sampstate, bool isDecal, bool isLeftOnly, bool isRightOnly, bool shouldCopy, bool shouldFlipMirror, bool shouldFlipCopy, bool isRightHand)
{
    float2 uv2 = lilCalcDecalUV(uv, uv_ST, angle, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, isRightHand);
    float4 outCol = LIL_SAMPLE_2D(tex,sampstate,uv2);
    if(isDecal) outCol *= lilIsIn0to1(uv2);
    return outCol;
}

#endif
// OpenLit Library 1.0.2
// This code is licensed under CC0 1.0 Universal.
// https://creativecommons.org/publicdomain/zero/1.0/

#if !defined(OPENLIT_CORE_INCLUDED)
#define OPENLIT_CORE_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Macro
#define OPENLIT_LIGHT_COLOR     _LightColor0.rgb
#define OPENLIT_LIGHT_DIRECTION _WorldSpaceLightPos0.xyz
#define OPENLIT_MATRIX_M        unity_ObjectToWorld
#define OPENLIT_FALLBACK_DIRECTION  float4(0.001,0.002,0.001,0)

//------------------------------------------------------------------------------------------------------------------------------
// SRGB <-> Linear
float3 OpenLitLinearToSRGB(float3 col)
{
    return LinearToGammaSpace(col);
}

float3 OpenLitSRGBToLinear(float3 col)
{
    return GammaToLinearSpace(col);
}

//------------------------------------------------------------------------------------------------------------------------------
// Color
float OpenLitLuminance(float3 rgb)
{
    #if defined(UNITY_COLORSPACE_GAMMA)
        return dot(rgb, float3(0.22, 0.707, 0.071));
    #else
        return dot(rgb, float3(0.0396819152, 0.458021790, 0.00609653955));
    #endif
}

float OpenLitGray(float3 rgb)
{
    return dot(rgb, float3(1.0/3.0, 1.0/3.0, 1.0/3.0));
}

//------------------------------------------------------------------------------------------------------------------------------
// Structure
struct OpenLitLightDatas
{
    float3 lightDirection;
    float3 directLight;
    float3 indirectLight;
};

//------------------------------------------------------------------------------------------------------------------------------
// Light Direction
// Use `UnityWorldSpaceLightDir(float3 positionWS)` for ForwardAdd passes
float3 ComputeCustomLightDirection(float4 lightDirectionOverride)
{
    float3 customDir = length(lightDirectionOverride.xyz) * normalize(mul((float3x3)OPENLIT_MATRIX_M, lightDirectionOverride.xyz));
    return lightDirectionOverride.w ? customDir : lightDirectionOverride.xyz;
}

void ComputeLightDirection(out float3 lightDirection, out float3 lightDirectionForSH9, float4 lightDirectionOverride)
{
    float3 mainDir = OPENLIT_LIGHT_DIRECTION * OpenLitLuminance(OPENLIT_LIGHT_COLOR);
    #if !defined(LIGHTMAP_ON) && UNITY_SHOULD_SAMPLE_SH
        float3 sh9Dir = unity_SHAr.xyz * 0.333333 + unity_SHAg.xyz * 0.333333 + unity_SHAb.xyz * 0.333333;
        float3 sh9DirAbs = float3(sh9Dir.x, abs(sh9Dir.y), sh9Dir.z);
    #else
        float3 sh9Dir = 0;
        float3 sh9DirAbs = 0;
    #endif
    float3 customDir = ComputeCustomLightDirection(lightDirectionOverride);

    lightDirection = normalize(sh9DirAbs + mainDir + customDir);
    lightDirectionForSH9 = sh9Dir + mainDir;
    lightDirectionForSH9 = dot(lightDirectionForSH9,lightDirectionForSH9) < 0.000001 ? 0 : normalize(lightDirectionForSH9);
}

void ComputeLightDirection(out float3 lightDirection, out float3 lightDirectionForSH9)
{
    ComputeLightDirection(lightDirection, lightDirectionForSH9, OPENLIT_FALLBACK_DIRECTION);
}

//------------------------------------------------------------------------------------------------------------------------------
// ShadeSH9
void ShadeSH9ToonDouble(float3 lightDirection, out float3 shMax, out float3 shMin)
{
    #if !defined(LIGHTMAP_ON) && UNITY_SHOULD_SAMPLE_SH
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
        #if defined(UNITY_COLORSPACE_GAMMA)
            shMax = OpenLitLinearToSRGB(shMax);
            shMin = OpenLitLinearToSRGB(shMin);
        #endif
    #else
        shMax = 0.0;
        shMin = 0.0;
    #endif
}

void ShadeSH9ToonDouble(out float3 shMax, out float3 shMin)
{
    float3 lightDirection, lightDirectionForSH9;
    ComputeLightDirection(lightDirection, lightDirectionForSH9, OPENLIT_FALLBACK_DIRECTION);
    ShadeSH9ToonDouble(lightDirectionForSH9, shMax, shMin);
}

float3 ShadeSH9Toon()
{
    float3 shMax, shMin;
    ShadeSH9ToonDouble(shMax, shMin);
    return shMax;
}

float3 ShadeSH9ToonIndirect()
{
    float3 shMax, shMin;
    ShadeSH9ToonDouble(shMax, shMin);
    return shMin;
}

//------------------------------------------------------------------------------------------------------------------------------
// Lighting
void ComputeSHLightsAndDirection(out float3 lightDirection, out float3 directLight, out float3 indirectLight, float4 lightDirectionOverride)
{
    float3 lightDirectionForSH9;
    ComputeLightDirection(lightDirection, lightDirectionForSH9, lightDirectionOverride);
    ShadeSH9ToonDouble(lightDirectionForSH9, directLight, indirectLight);
}

void ComputeSHLightsAndDirection(out float3 lightDirection, out float3 directLight, out float3 indirectLight)
{
    ComputeSHLightsAndDirection(lightDirection, directLight, indirectLight, OPENLIT_FALLBACK_DIRECTION);
}

void ComputeLights(out float3 lightDirection, out float3 directLight, out float3 indirectLight, float4 lightDirectionOverride)
{
    ComputeSHLightsAndDirection(lightDirection, directLight, indirectLight, lightDirectionOverride);
    directLight += OPENLIT_LIGHT_COLOR;
}

void ComputeLights(out float3 lightDirection, out float3 directLight, out float3 indirectLight)
{
    ComputeSHLightsAndDirection(lightDirection, directLight, indirectLight);
    directLight += OPENLIT_LIGHT_COLOR;
}

void ComputeLights(out OpenLitLightDatas lightDatas, float4 lightDirectionOverride)
{
    ComputeLights(lightDatas.lightDirection, lightDatas.directLight, lightDatas.indirectLight, lightDirectionOverride);
}

void ComputeLights(out OpenLitLightDatas lightDatas)
{
    ComputeLights(lightDatas.lightDirection, lightDatas.directLight, lightDatas.indirectLight);
}

//------------------------------------------------------------------------------------------------------------------------------
// Correct
void CorrectLights(inout OpenLitLightDatas lightDatas, float lightMinLimit, float lightMaxLimit, float monochromeLighting, float asUnlit)
{
    lightDatas.directLight = clamp(lightDatas.directLight, lightMinLimit, lightMaxLimit);
    lightDatas.directLight = lerp(lightDatas.directLight, OpenLitGray(lightDatas.directLight), monochromeLighting);
    lightDatas.directLight = lerp(lightDatas.directLight, 1.0, asUnlit);
    lightDatas.indirectLight = clamp(lightDatas.indirectLight, 0.0, lightMaxLimit);
}

//------------------------------------------------------------------------------------------------------------------------------
// Vertex Lighting
float3 ComputeAdditionalLights(float3 positionWS, float3 positionCS)
{
    float4 toLightX = unity_4LightPosX0 - positionWS.x;
    float4 toLightY = unity_4LightPosY0 - positionWS.y;
    float4 toLightZ = unity_4LightPosZ0 - positionWS.z;

    float4 lengthSq = toLightX * toLightX + 0.000001;
    lengthSq += toLightY * toLightY;
    lengthSq += toLightZ * toLightZ;

    //float4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
    float4 atten = saturate(saturate((25.0 - lengthSq * unity_4LightAtten0) * 0.111375) / (0.987725 + lengthSq * unity_4LightAtten0));

    float3 additionalLightColor;
    additionalLightColor =                        unity_LightColor[0].rgb * atten.x;
    additionalLightColor = additionalLightColor + unity_LightColor[1].rgb * atten.y;
    additionalLightColor = additionalLightColor + unity_LightColor[2].rgb * atten.z;
    additionalLightColor = additionalLightColor + unity_LightColor[3].rgb * atten.w;

    return additionalLightColor;
}

//------------------------------------------------------------------------------------------------------------------------------
// Encode and decode
#if !defined(SHADER_API_GLES)
// -1 - 1
uint EncodeNormalizedFloat3ToUint(float3 vec)
{
    uint valx = abs(vec.x) >= 1 ? 511 : abs(vec.x) * 511;
    uint valy = abs(vec.y) >= 1 ? 511 : abs(vec.y) * 511;
    uint valz = abs(vec.z) >= 1 ? 511 : abs(vec.z) * 511;
    valx = valx & 0x000001ffu;
    valy = valy & 0x000001ffu;
    valz = valz & 0x000001ffu;
    valx += vec.x > 0 ? 0 : 512;
    valy += vec.y > 0 ? 0 : 512;
    valz += vec.z > 0 ? 0 : 512;

    valy = valy << 10;
    valz = valz << 20;
    return valx | valy | valz;
}

float3 DecodeNormalizedFloat3FromUint(uint val)
{
    // 5 math in target 5.0
    uint3 val3 = val >> uint3(0,10,20);
    float3 vec = val3 & 0x000001ffu;
    vec /= (val3 & 0x00000200u) == 0x00000200u ? -511.0 : 511.0;
    return vec;
}

// 0 - 999
uint EncodeHDRColorToUint(float3 col)
{
    col = clamp(col, 0, 999);
    float maxcol = max(col.r,max(col.g,col.b));

    float floatDigit = maxcol == 0 ? 0 : log10(maxcol);
    uint digit = floatDigit >= 0 ? floatDigit + 1 : 0;
    if(digit > 3) digit = 3;
    float scale = pow(10,digit);
    col /= scale;

    uint R = col.r * 1023;
    uint G = col.g * 1023;
    uint B = col.b * 1023;
    uint M = digit;
    R = R & 0x000003ffu;
    G = G & 0x000003ffu;
    B = B & 0x000003ffu;

    G = G << 10;
    B = B << 20;
    M = M << 30;
    return R | G | B | M;
}

float3 DecodeHDRColorFromUint(uint val)
{
    // 5 math in target 5.0
    uint4 RGBM = val >> uint4(0,10,20,30);
    return float3(RGBM.rgb & 0x000003ffu) / 1023.0 * pow(10,RGBM.a);
}

void PackLightDatas(out uint3 pack, OpenLitLightDatas lightDatas)
{
    pack = uint3(
        EncodeNormalizedFloat3ToUint(lightDatas.lightDirection),
        EncodeHDRColorToUint(lightDatas.directLight),
        EncodeHDRColorToUint(lightDatas.indirectLight)
    );
}

void UnpackLightDatas(out OpenLitLightDatas lightDatas, uint3 pack)
{
    lightDatas.lightDirection = DecodeNormalizedFloat3FromUint(pack.x);
    lightDatas.directLight = DecodeHDRColorFromUint(pack.y);
    lightDatas.indirectLight = DecodeHDRColorFromUint(pack.z);
}
#endif // #if !defined(SHADER_API_GLES)

#endif // #if !defined(OPENLIT_CORE_INCLUDED)
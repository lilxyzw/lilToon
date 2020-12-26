// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------------------
// 逆三角関数を高速化
// https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/
#define QUATER_PI 0.785398163397
float lilAcos(float x) 
{ 
    float ox = abs(x); 
    float res = -0.156583f * ox + UNITY_HALF_PI; 
    res *= sqrt(1.0f - ox); 
    return (x >= 0) ? res : UNITY_PI - res; 
}
float lilAsin(float x)
{
    return UNITY_HALF_PI - lilAcos(x);
}
float lilAtanPos(float x) 
{ 
    float t0 = (x < 1.0f) ? x : 1.0f / x;
    float t1 = t0 * t0;
    float poly = 0.0872929f;
    poly = -0.301895f + poly * t1;
    poly = 1.0f + poly * t1;
    poly = poly * t0;
    return (x < 1.0f) ? poly : UNITY_HALF_PI - poly;
}
float lilAtan(float x) 
{     
    float t0 = lilAtanPos(abs(x));     
    return (x < 0.0f) ? -t0 : t0; 
}
float lilAtan2(float x, float y)
{
    return lilAtan(x/y) + UNITY_PI * (y<0) * (x<0?-1:1);
}


//------------------------------------------------------------------------------------------------------------------------------
// Math
float inRange(float f)
{
    int v = (int)((f - 0.5) * 2);
    return 1 - (float)v / (v - 0.0001);
}

float inRange(float2 f)
{
    return inRange(f.x) * inRange(f.y);
}

float3 mixSubCol(float3 maincol, float3 subcol, float strength, int mixmode)
{
    UNITY_BRANCH
    if(mixmode == 0)        return lerp(maincol, subcol, strength);                                                                                     //通常
    else if(mixmode == 1)   return maincol + subcol * strength;                                                                                         //加算
    else if(mixmode == 2)   return lerp(maincol, max(maincol + subcol - maincol * subcol, maincol), strength);                                          //スクリーン
    else if(mixmode == 3)   return lerp(maincol, maincol * subcol, strength);                                                                           //乗算
    else                    return lerp(maincol, maincol < 0.5 ? 2.0 * maincol * subcol : max(maincol+subcol-maincol*subcol,maincol)*2-1, strength);    //オーバーレイ
}

float4 mixSubCol(float4 maincol, float4 subcol, float strength, int mixmode)
{
    strength *= subcol.a;
    return float4(mixSubCol(maincol.rgb, subcol.rgb, strength, mixmode), maincol.a);
}

float3 mixNormal(float3 normal1, float3 normal2)
{
    return normalize(float3(normal1.xy + normal2.xy, normal1.z * normal2.z));
}

float2 ComputeTransformCap(float3 ndir)
{
    /*
    // Matcapが画面端で破綻するのをごまかす VRだと違和感があるので非推奨
    float3 n = mul(UNITY_MATRIX_V,float4(ndir,0)).xyz;
    float3 v = mul(UNITY_MATRIX_V,float4(vdir,0)).xyz*float3(-1,-1,1) + float3(0,0,1);
    float2 uv_old = n.rg;
    n *= float3(-1,-1,1);
    float2 uv = v.rg*dot(v,n)/v.z - n.rg;
    return lerp(uv,uv_old,max(0,n.z))*0.5+0.5; // 視差による違和感をごまかす
    */
    return mul(UNITY_MATRIX_V,float4(ndir,0)).xy*0.5+0.5;
}

float3 getSpecular(float nv, float nl, float nh, float lh, float roughness, float3 specular)
{
    roughness = max(roughness, 0.002);
    float specularTerm = SmithJointGGXVisibilityTerm(nl,nv,roughness) * GGXTerm(nh,roughness) * UNITY_PI;
    #ifdef UNITY_COLORSPACE_GAMMA
        specularTerm = sqrt(max(1e-4h, specularTerm));
    #endif
    specularTerm = max(0, specularTerm * nl);
    return specularTerm * FresnelTerm(specular, lh);
}

float3 GetSHLength()
{
    float3 x, x1;
    x.r = length(unity_SHAr);
    x.g = length(unity_SHAg);
    x.b = length(unity_SHAb);
    x1.r = length(unity_SHBr);
    x1.g = length(unity_SHBg);
    x1.b = length(unity_SHBb);
    return saturate(x + x1);
}

float lilLuminance(float3 rgb)
{
    return dot(rgb,float3(0.22, 0.707, 0.071));
}

float2 uvRotate(float2 uv, float2x2 rotMat)
{
    float2 uvrot = uv - 0.5;
    uv = mul(rotMat, uvrot);
    uvrot += 0.5;
    return uvrot;
}

float2 calcUV(float2 uv, float2 tiling, float2 offset, float rot)
{
    float co = cos(rot);
    float si = sin(rot);
    float2 uvcalc = uv - 0.5;
    uvcalc = float2(
        co * uvcalc.x - si * uvcalc.y,
        si * uvcalc.x + co * uvcalc.y
    );
    uvcalc += 0.5;
    uvcalc = uvcalc * tiling + offset;
    return uvcalc;
}

float2x2 calcMatRot(bool isMirror)
{
    float3 matV1 = UNITY_MATRIX_V._m00_m01_m02;
    float3 matV2 = cross(UNITY_MATRIX_V._m20_m21_m22, float3(0,1,0));
    if(isMirror > 0.5) matV2 = -matV2;
    float matR = dot(matV1,matV2) / (length(matV1)*length(matV2));
    matR = lilAcos(clamp(matR, -1, 1));
    matR = UNITY_MATRIX_V._m01 < 0 ? - matR : matR;
    return float2x2(cos(matR), -sin(matR), sin(matR), cos(matR));
}

//------------------------------------------------------------------------------------------------------------------------------
// Get Texture
float4 getTex(Texture2D tex, float4 uv_ST, float2 offset, float rot, float2 uv, bool trim, SamplerState sampstate)
{
    float2 exuv = calcUV(uv, uv_ST.xy, offset, rot);
    float uvInRange = saturate(inRange(exuv) + !trim);
    return tex.Sample(sampstate,exuv) * uvInRange;
    /* コンパイル後がえげつないのでボツ
    if(uvnum == 5)
    {
        if(samp == 0)       {return tex.SampleLevel(sampler_linear_repeat,exuv,0) * uvInRange;}
        else if(samp == 1)  {return tex.SampleLevel(sampler_linear_clamp,exuv,0) * uvInRange;}
        else if(samp == 2)  {return tex.SampleLevel(sampler_linear_mirror,exuv,0) * uvInRange;}
        else if(samp == 3)  {return tex.SampleLevel(sampler_linear_clamp,exuv,0) * uvInRange;}
        else if(samp == 4)  {return tex.SampleLevel(sampler_point_repeat,exuv,0) * uvInRange;}
        else if(samp == 5)  {return tex.SampleLevel(sampler_point_clamp,exuv,0) * uvInRange;}
        else if(samp == 6)  {return tex.SampleLevel(sampler_point_mirror,exuv,0) * uvInRange;}
        else if(samp == 7)  {return tex.SampleLevel(sampler_point_clamp,exuv,0) * uvInRange;}
        else                {return tex.SampleLevel(sampler_linear_repeat,exuv,0) * uvInRange;}
    }
    else
    {
        if(samp == 0)       {return tex.Sample(sampler_linear_repeat,exuv) * uvInRange;}
        else if(samp == 1)  {return tex.Sample(sampler_linear_clamp,exuv) * uvInRange;}
        else if(samp == 2)  {return tex.Sample(sampler_linear_mirror,exuv) * uvInRange;}
        else if(samp == 3)  {return tex.Sample(sampler_linear_clamp,exuv) * uvInRange;}
        else if(samp == 4)  {return tex.Sample(sampler_point_repeat,exuv) * uvInRange;}
        else if(samp == 5)  {return tex.Sample(sampler_point_clamp,exuv) * uvInRange;}
        else if(samp == 6)  {return tex.Sample(sampler_point_mirror,exuv) * uvInRange;}
        else if(samp == 7)  {return tex.Sample(sampler_point_clamp,exuv) * uvInRange;}
        else                {return tex.Sample(sampler_linear_repeat,exuv) * uvInRange;}
    }
    */
}
float getMask(Texture2D tex, float4 uv_ST, float2 offset, float rot, float2 uv, bool trim, SamplerState sampstate)
{
    float2 exuv = calcUV(uv, uv_ST.xy, offset, rot);
    float uvInRange = saturate(inRange(exuv) + !trim);
    return tex.Sample(sampstate,exuv).r * uvInRange;
}
float4 getTex_V(Texture2D tex, float4 uv_ST, float2 offset, float rot, float2 uv, bool trim, SamplerState sampstate)
{
    float2 exuv = calcUV(uv, uv_ST.xy, offset, rot);
    float uvInRange = saturate(inRange(exuv) + !trim);
    return tex.SampleLevel(sampstate,exuv,0) * uvInRange;
}
float getMask_V(Texture2D tex, float4 uv_ST, float2 offset, float rot, float2 uv, bool trim, SamplerState sampstate)
{
    float2 exuv = calcUV(uv, uv_ST.xy, offset, rot);
    float uvInRange = saturate(inRange(exuv) + !trim);
    return tex.SampleLevel(sampstate,exuv,0).r * uvInRange;
}

float4 getTexLite(Texture2D tex, float2 uv, SamplerState sampstate)
{
    return tex.Sample(sampstate,uv);
}
float getMaskLite(Texture2D tex, float2 uv, SamplerState sampstate)
{
    return tex.Sample(sampstate,uv).r;
}
float4 getTexLite_V(Texture2D tex, float2 uv, SamplerState sampstate)
{
    return tex.SampleLevel(sampstate,uv,0);
}
float getMaskLite_V(Texture2D tex, float2 uv, SamplerState sampstate)
{
    return tex.SampleLevel(sampstate,uv,0).r;
}

float4 getMatcapTex(float2 uv, Texture2D samp, float4 col, float4 uv_ST)
{
    return UNITY_SAMPLE_TEX2D_SAMPLER(samp, _linear_repeat, uv * uv_ST.xy + uv_ST.zw) * col;
}

float4 getGradTex(float speed, Texture2D samp)
{
    return UNITY_SAMPLE_TEX2D_SAMPLER(samp, _linear_repeat, float2(speed*_Time.r, 0.5));
}

float3 getRefTex(samplerCUBE tex, float4 hdr, float3 reflUVW, float roughness)
{
    float perceptualRoughness = roughness * mad(roughness, -0.7, 1.7);

    float mip = perceptualRoughness * UNITY_SPECCUBE_LOD_STEPS;
    float4 rgbm = texCUBEbias(tex, float4(reflUVW, mip));

    return DecodeHDR(rgbm, hdr);
}

float3 getUnityRefTex(UNITY_ARGS_TEXCUBE(tex), float4 hdr, float3 reflUVW, float roughness)
{
    float perceptualRoughness = roughness * mad(roughness, -0.7, 1.7);

    float mip = perceptualRoughness * UNITY_SPECCUBE_LOD_STEPS;
    float4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(tex, reflUVW, mip);

    return DecodeHDR(rgbm, hdr);
}

float3 reflectionTex(float4 probeHDR[2], float4 probePosition[2], float4 boxMax[2], float4 boxMin[2], float3 worldPos, float3 reflUVW, float roughness)
{
    float3 specular;

    // Box Projection 時は reflUVW を修正
    #ifdef UNITY_SPECCUBE_BOX_PROJECTION
        float3 originalReflUVW = reflUVW;
        reflUVW = BoxProjectedCubemapDirection(originalReflUVW, worldPos, probePosition[0], boxMin[0], boxMax[0]);
    #endif

    // リフレクションプローブのサンプリング
    #ifdef _GLOSSYREFLECTIONS_OFF
        specular = unity_IndirectSpecColor.rgb;
    #else
        float3 env0 = getUnityRefTex(UNITY_PASS_TEXCUBE(unity_SpecCube0), probeHDR[0], reflUVW, roughness);
        #ifdef UNITY_SPECCUBE_BLENDING
            // probeの合成
            float blendLerp = boxMin[0].w;
            UNITY_BRANCH
            if (blendLerp < 0.99999)
            {
                #ifdef UNITY_SPECCUBE_BOX_PROJECTION
                    reflUVW = BoxProjectedCubemapDirection (originalReflUVW, worldPos, probePosition[1], boxMin[1], boxMax[1]);
                #endif

                float3 env1 = getUnityRefTex(UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1,unity_SpecCube0), probeHDR[1], reflUVW, roughness);
                specular = lerp(env1, env0, blendLerp);
            }
            else
            {
                specular = env0;
            }
        #else
            specular = env0;
        #endif
    #endif

    // こちらもオクルージョンを反映して返す
    //return specular * occlusion;
    return specular;
}


//------------------------------------------------------------------------------------------------------------------------------
// HSV
float3 colorShifter(float3 c, float h, float s, float v){
    float e = 1.0e-10;
    // rgb -> hsv
    float4 p = lerp(float4(c.bg, float2(-1, 2.0/3.0)), float4(c.gb, float2(0,-1.0/3.0)), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float3 hsv = float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
    // shift
    hsv = float3(hsv.x+h,saturate(hsv.y*s),hsv.z*v);
    // hsv -> rgb
    return hsv.z * lerp(1, clamp(abs(frac(hsv.xxx + float3(1, 2.0/3.0, 1.0/3.0)) * 6 - 3) - 1, 0, 1), hsv.y);
}

float3 colorShifterEx(float3 c, float h, float s, float v, float AH){
    float e = 1.0e-10;
    // rgb -> hsv
    float4 p = lerp(float4(c.bg, float2(-1, 2.0/3.0)), float4(c.gb, float2(0,-1.0/3.0)), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float3 hsv = float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
    // shift
    hsv = float3(hsv.x+h,saturate(hsv.y*s),hsv.z*v);
    hsv.x = mad(hsv.x, 2, -1);
    hsv.x = (hsv.x<0?-1:1) * pow(abs(hsv.x), 1.001-AH);
    hsv.x = mad(hsv.x, 0.5, 0.5);
    // hsv -> rgb
    return hsv.z * lerp(1, clamp(abs(frac(hsv.xxx + float3(1, 2.0/3.0, 1.0/3.0)) * 6 - 3) - 1, 0, 1), hsv.y);
}

//------------------------------------------------------------------------------------------------------------------------------
// シェーダー外で計算できるものはここで
static float3 lightColor = saturate(_LightColor0.rgb);
static float lightColorL = lilLuminance(lightColor);
#ifndef LIL_FOR_ADD
static float3 ShadeSH9Ave = saturate(ShadeSH9(float4(0.0,0.0,0.0,1.0)));
static float3 ShadeSH9Plus = max(ShadeSH9Ave, GetSHLength());
static float ShadeSH9AveL = lilLuminance(ShadeSH9Ave);
static float ShadeSH9PlusL = lilLuminance(ShadeSH9Plus);
#endif
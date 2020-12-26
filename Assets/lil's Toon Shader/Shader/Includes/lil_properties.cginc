// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------------------
// サンプラー
SamplerState sampler_linear_repeat;

//------------------------------------------------------------------------------------------------------------------------------
// テクスチャで使うパラメーターをセットアップ
#ifdef LIL_FULL
    #define GET_TEX(tex) getTex(tex, tex##_ST, tex##UVoffset, tex##RotMix, uvs[tex##UV], tex##Trim, sampler_linear_repeat)
    #define GET_MASK(tex) getMask(tex, tex##_ST, tex##UVoffset, tex##RotMix, uvs[tex##UV], tex##Trim, sampler_linear_repeat)
    #define GET_TEX_V(tex) getTex_V(tex, tex##_ST, tex##UVoffset, tex##RotMix, uvs[tex##UV], tex##Trim, sampler_linear_repeat)
    #define GET_MASK_V(tex) getMask_V(tex, tex##_ST, tex##UVoffset, tex##RotMix, uvs[tex##UV], tex##Trim, sampler_linear_repeat)
    #define GET_TEX_SAMP(tex) getTex(tex, tex##_ST, tex##UVoffset, tex##RotMix, uvs[tex##UV], tex##Trim, sampler##tex)
    #define GET_EMITEX(tex) getTex(tex, tex##_ST, tex##UVoffset, tex##RotMix, tex##ParaTex, tex##Trim, sampler##tex)
#else
    #define GET_TEX(tex) getTexLite(tex, i.uv, sampler_linear_repeat)
    #define GET_MASK(tex) getMaskLite(tex, i.uv, sampler_linear_repeat)
    #define GET_TEX_V(tex) getTexLite_V(tex, o.uv, sampler_linear_repeat)
    #define GET_MASK_V(tex) getMaskLite_V(tex, o.uv, sampler_linear_repeat)
    #define GET_TEX_SAMP(tex) getTexLite(tex, i.uv, sampler##tex)
    #define GET_EMITEX(tex) getTex(tex, tex##_ST, tex##UVoffset, tex##RotMix, tex##ParaTex, tex##Trim, sampler##tex)
    #define GET_EMIMASK(tex) getMask(tex, tex##_ST, tex##UVoffset, tex##RotMix, i.uv, tex##Trim, sampler_linear_repeat)
    #define GET_TEXSUB(tex) getTex(tex, tex##_ST, tex##_ST.zw, tex##RotMix, i.uv, tex##Trim, sampler##tex)
    #define SET_TEXSUB_PROPERTIES(tex) \
        float4          tex##_ST; \
        float           tex##Angle; \
        bool            tex##Trim; \
        static float    tex##RotMix = tex##Angle * UNITY_PI;
#endif

#define SET_TEX_PROPERTIES(tex) \
    float4          tex##_ST; \
    float           tex##ScrollX; \
    float           tex##ScrollY; \
    float           tex##Angle; \
    float           tex##Rotate; \
    uint            tex##UV; \
    bool            tex##Trim; \
    static float2   tex##UVoffset = tex##_ST.zw + float2(tex##ScrollX,tex##ScrollY) * _Time.r; \
    static float    tex##RotMix = (tex##Angle + tex##Rotate * _Time.r) * UNITY_PI;

//------------------------------------------------------------------------------------------------------------------------------
// 各種プロパティ取得
float   _CullMode;
#if LIL_RENDER == 1
    float   _Cutoff;
#endif
float   _FlipNormal;
float   _BackfaceForceShadow;
float   _Invisible;
bool    _UseVertexColor;

#ifndef LIL_FULL
//------------------------------------------------------------------------------------------------------------------------------
// メイン
float4  _Color;
UNITY_DECLARE_TEX2D(_MainTex);
float4  _MainTex_ST;
float   _MainTexTonecurve;
float   _MainTexHue;
float   _MainTexSaturation;
float   _MainTexValue;

bool    _UseMain2ndTex;
float4  _Color2nd;
UNITY_DECLARE_TEX2D(_Main2ndTex);
SET_TEXSUB_PROPERTIES(_Main2ndTex);
float   _Main2ndTexMix;

bool    _UseMain3rdTex;
float4  _Color3rd;
UNITY_DECLARE_TEX2D(_Main3rdTex);
SET_TEXSUB_PROPERTIES(_Main3rdTex);
float   _Main3rdTexMix;

bool    _UseMain4thTex;
float4  _Color4th;
UNITY_DECLARE_TEX2D(_Main4thTex);
SET_TEXSUB_PROPERTIES(_Main4thTex);
float   _Main4thTexMix;

//------------------------------------------------------------------------------------------------------------------------------
// アルファ
#if LIL_RENDER != 0
    bool    _UseAlphaMask;
    UNITY_DECLARE_TEX2D_NOSAMPLER(_AlphaMask);
#endif

//------------------------------------------------------------------------------------------------------------------------------
// ノーマル
bool    _UseBumpMap;
UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpMap);
float4  _BumpMap_ST;
float   _BumpScale;

bool    _UseBump2ndMap;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Bump2ndMap);
float4  _Bump2ndMap_ST;
float   _Bump2ndScale;

//------------------------------------------------------------------------------------------------------------------------------
// 影
bool    _UseShadow;
bool    _UseShadowMixMainColor;
float   _ShadowGrad;
float4  _ShadowGradColor;
float   _ShadowHue;
float   _ShadowSaturation;
float   _ShadowValue;
bool    _UseShadowColor;
float   _ShadowColorFromMain;
float   _ShadowBorder;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowBorderMask);
float   _ShadowBlur;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowBlurMask);
float   _ShadowStrength;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowStrengthMask);
float   _ShadowMixMainColor;
float4  _ShadowColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowColorTex);
uint    _ShadowColorMix;

bool    _UseShadow2nd;
float   _Shadow2ndBorder;
float   _Shadow2ndBlur;
float   _Shadow2ndColorFromMain;
float4  _Shadow2ndColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Shadow2ndColorTex);
uint    _Shadow2ndColorMix;

//------------------------------------------------------------------------------------------------------------------------------
// 輪郭線
bool    _UseOutline;
bool    _OutlineMixMain;
float   _OutlineMixMainStrength;
float   _OutlineHue;
float   _OutlineSaturation;
float   _OutlineValue;
float   _OutlineAutoHue;
float   _OutlineAutoValue;
float   _OutlineWidth;
UNITY_DECLARE_TEX2D_NOSAMPLER(_OutlineWidthMask);
UNITY_DECLARE_TEX2D_NOSAMPLER(_OutlineAlphaMask);
float4  _OutlineColor;
uint    _VertexColor2Width;
static int _vc2w = _VertexColor2Width-1;

//------------------------------------------------------------------------------------------------------------------------------
// 屈折
#ifdef LIL_REFRACTION
    sampler2D _BackgroundTexture;
    sampler2D _CameraDepthTexture;
    float _RefractionStrength;
    float _RefractionFresnelPower;
    bool _RefractionColorFromMain;
    float4 _RefractionColor;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// ファー
#ifdef LIL_FUR
    UNITY_DECLARE_TEX2D_NOSAMPLER(_FurNoiseMask);
    float4 _FurNoiseMask_ST;
    UNITY_DECLARE_TEX2D_NOSAMPLER(_FurMask);
    UNITY_DECLARE_TEX2D_NOSAMPLER(_FurVectorTex);
    float _FurVectorScale;
    float _FurVectorX;
    float _FurVectorY;
    float _FurVectorZ;
    float _FurLength;
    float _FurGravity;
    bool _VertexColor2FurVector;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// 反射
bool    _UseReflection;
// 滑らかさ
float   _Smoothness;
UNITY_DECLARE_TEX2D_NOSAMPLER(_SmoothnessTex);
// 金属度
float   _Metallic;
UNITY_DECLARE_TEX2D_NOSAMPLER(_MetallicGlossMap);
// 反射
bool    _ApplySpecular;
bool    _ApplyReflection;
// 合成
float   _ReflectionBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ReflectionBlendMask);
float   _ReflectionShadeMix;

//------------------------------------------------------------------------------------------------------------------------------
// マットキャップ
bool    _UseMatcap;
float4  _MatcapColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_MatcapTex);
float   _MatcapBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_MatcapBlendMask);
float   _MatcapShadeMix;
uint    _MatcapMix;

//------------------------------------------------------------------------------------------------------------------------------
// リムライト
bool    _UseRim;
float4  _RimColor;
float   _RimBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_RimBlendMask);
float   _RimToon;
float   _RimBorder;
float   _RimBlur;
float   _RimUpperSideWidth;
float   _RimFresnelPower;
float   _RimShadeMix;
bool    _RimShadowMask;

//------------------------------------------------------------------------------------------------------------------------------
// 発光 というか実質unlit
bool    _UseEmission;
float4  _EmissionColor;
UNITY_DECLARE_TEX2D(_EmissionMap);
SET_TEX_PROPERTIES(_EmissionMap);
float   _EmissionBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_EmissionBlendMask);
SET_TEX_PROPERTIES(_EmissionBlendMask);
bool    _EmissionUseBlink;
float   _EmissionBlinkStrength;
float   _EmissionBlinkSpeed;
float   _EmissionBlinkOffset;
bool    _EmissionBlinkType;
bool    _EmissionUseGrad;
UNITY_DECLARE_TEX2D_NOSAMPLER(_EmissionGradTex);
float   _EmissionGradSpeed;
float   _EmissionParallaxDepth;
static float _EmissionBlinkSin = sin(_Time.r*_EmissionBlinkSpeed+_EmissionBlinkOffset)*0.5+0.5;
static float _EmissionBlinkRou = _EmissionBlinkType ? round(_EmissionBlinkSin) : _EmissionBlinkSin;
static float _EmissionBlink = 1.0 - mad(_EmissionBlinkStrength, -_EmissionBlinkRou, _EmissionBlinkStrength) * _EmissionUseBlink;

bool    _UseEmission2nd;
float4  _Emission2ndColor;
UNITY_DECLARE_TEX2D(_Emission2ndMap);
SET_TEX_PROPERTIES(_Emission2ndMap);
float   _Emission2ndBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission2ndBlendMask);
SET_TEX_PROPERTIES(_Emission2ndBlendMask);
bool    _Emission2ndUseBlink;
float   _Emission2ndBlinkStrength;
float   _Emission2ndBlinkSpeed;
float   _Emission2ndBlinkOffset;
bool    _Emission2ndBlinkType;
bool    _Emission2ndUseGrad;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission2ndGradTex);
float   _Emission2ndGradSpeed;
float   _Emission2ndParallaxDepth;
static float _Emission2ndBlinkSin = sin(_Time.r*_Emission2ndBlinkSpeed+_Emission2ndBlinkOffset)*0.5+0.5;
static float _Emission2ndBlinkRou = _Emission2ndBlinkType ? round(_Emission2ndBlinkSin) : _Emission2ndBlinkSin;
static float _Emission2ndBlink = 1.0 - mad(_Emission2ndBlinkStrength, -_Emission2ndBlinkRou, _Emission2ndBlinkStrength) * _Emission2ndUseBlink;




#else
//------------------------------------------------------------------------------------------------------------------------------
// メイン
float4  _Color;
UNITY_DECLARE_TEX2D(_MainTex);
SET_TEX_PROPERTIES(_MainTex);
float   _MainTexTonecurve;
float   _MainTexHue;
float   _MainTexSaturation;
float   _MainTexValue;

bool    _UseMain2ndTex;
float4  _Color2nd;
UNITY_DECLARE_TEX2D(_Main2ndTex);
SET_TEX_PROPERTIES(_Main2ndTex);
float   _Main2ndBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Main2ndBlendMask);
SET_TEX_PROPERTIES(_Main2ndBlendMask);
float   _Main2ndTexMix;
float   _Main2ndTexHue;
float   _Main2ndTexSaturation;
float   _Main2ndTexValue;

bool    _UseMain3rdTex;
float4  _Color3rd;
UNITY_DECLARE_TEX2D(_Main3rdTex);
SET_TEX_PROPERTIES(_Main3rdTex);
float   _Main3rdBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Main3rdBlendMask);
SET_TEX_PROPERTIES(_Main3rdBlendMask);
float   _Main3rdTexMix;
float   _Main3rdTexHue;
float   _Main3rdTexSaturation;
float   _Main3rdTexValue;

bool    _UseMain4thTex;
float4  _Color4th;
UNITY_DECLARE_TEX2D(_Main4thTex);
SET_TEX_PROPERTIES(_Main4thTex);
float   _Main4thBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Main4thBlendMask);
SET_TEX_PROPERTIES(_Main4thBlendMask);
float   _Main4thTexMix;
float   _Main4thTexHue;
float   _Main4thTexSaturation;
float   _Main4thTexValue;

//------------------------------------------------------------------------------------------------------------------------------
// アルファ
#if LIL_RENDER != 0
    bool    _UseAlphaMask;
    float   _Alpha;
    UNITY_DECLARE_TEX2D_NOSAMPLER(_AlphaMask);
    SET_TEX_PROPERTIES(_AlphaMask);
    float   _AlphaMaskMixMain;

    /* アルファマスク追加分
    bool    _UseAlphaMask2nd;
    float   _Alpha2nd;
    UNITY_DECLARE_TEX2D_NOSAMPLER(_AlphaMask2nd);
    SET_TEX_PROPERTIES(_AlphaMask2nd);
    float   _AlphaMask2ndMixMain;
    */
#endif

//------------------------------------------------------------------------------------------------------------------------------
// ノーマル
bool    _UseBumpMap;
UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpMap);
SET_TEX_PROPERTIES(_BumpMap);
float   _BumpScale;
UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpScaleMask);
SET_TEX_PROPERTIES(_BumpScaleMask);

bool    _UseBump2ndMap;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Bump2ndMap);
SET_TEX_PROPERTIES(_Bump2ndMap);
float   _Bump2ndScale;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Bump2ndScaleMask);
SET_TEX_PROPERTIES(_Bump2ndScaleMask);

/* ノーマルマップ追加分
bool    _UseBump3rdMap;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Bump3rdMap);
SET_TEX_PROPERTIES(_Bump3rdMap);
float   _Bump3rdScale;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Bump3rdScaleMask);
SET_TEX_PROPERTIES(_Bump3rdScaleMask);

bool    _UseBump4thMap;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Bump4thMap);
SET_TEX_PROPERTIES(_Bump4thMap);
float   _Bump4thScale;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Bump4thScaleMask);
SET_TEX_PROPERTIES(_Bump4thScaleMask);
*/

//------------------------------------------------------------------------------------------------------------------------------
// 影
bool    _UseShadow;
bool    _UseShadowMixMainColor;
float   _ShadowGrad;
float4  _ShadowGradColor;
float   _ShadowHue;
float   _ShadowSaturation;
float   _ShadowValue;
bool    _UseShadowColor;
float   _ShadowColorFromMain;
float   _ShadowBorder;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowBorderMask);
SET_TEX_PROPERTIES(_ShadowBorderMask);
float   _ShadowBlur;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowBlurMask);
SET_TEX_PROPERTIES(_ShadowBlurMask);
float   _ShadowStrength;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowStrengthMask);
SET_TEX_PROPERTIES(_ShadowStrengthMask);
float   _ShadowMixMainColor;
float4  _ShadowColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowColorTex);
SET_TEX_PROPERTIES(_ShadowColorTex);
uint    _ShadowColorMix;

bool    _UseShadow2nd;
float   _Shadow2ndBorder;
float   _Shadow2ndBlur;
float   _Shadow2ndColorFromMain;
float4  _Shadow2ndColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Shadow2ndColorTex);
SET_TEX_PROPERTIES(_Shadow2ndColorTex);
uint    _Shadow2ndColorMix;

bool    _UseDefaultShading;
float   _DefaultShadingBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_DefaultShadingBlendMask);
SET_TEX_PROPERTIES(_DefaultShadingBlendMask);

//------------------------------------------------------------------------------------------------------------------------------
// 輪郭線
bool    _UseOutline;
bool    _OutlineMixMain;
float   _OutlineMixMainStrength;
float   _OutlineHue;
float   _OutlineSaturation;
float   _OutlineValue;
float   _OutlineAutoHue;
float   _OutlineAutoValue;
float   _OutlineWidth;
UNITY_DECLARE_TEX2D_NOSAMPLER(_OutlineWidthMask);
SET_TEX_PROPERTIES(_OutlineWidthMask);
float   _OutlineAlpha;
UNITY_DECLARE_TEX2D_NOSAMPLER(_OutlineAlphaMask);
SET_TEX_PROPERTIES(_OutlineAlphaMask);
bool    _UseOutlineColor;
float4  _OutlineColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_OutlineColorTex);
SET_TEX_PROPERTIES(_OutlineColorTex);
uint    _VertexColor2Width;
static int _vc2w = _VertexColor2Width-1;

//------------------------------------------------------------------------------------------------------------------------------
// 反射
bool    _UseReflection;
// 滑らかさ
float   _Smoothness;
UNITY_DECLARE_TEX2D_NOSAMPLER(_SmoothnessTex);
SET_TEX_PROPERTIES(_SmoothnessTex);
// 金属度
float   _Metallic;
UNITY_DECLARE_TEX2D_NOSAMPLER(_MetallicGlossMap);
SET_TEX_PROPERTIES(_MetallicGlossMap);
// 反射
bool    _ApplySpecular;
bool    _ApplyReflection;
bool    _ReflectionUseCubemap;
samplerCUBE _ReflectionCubemap;
float4      _ReflectionCubemap_HDR;
// 合成
float   _ReflectionBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ReflectionBlendMask);
SET_TEX_PROPERTIES(_ReflectionBlendMask);
float   _ReflectionShadeMix;

//------------------------------------------------------------------------------------------------------------------------------
// マットキャップ
bool    _UseMatcap;
float4  _MatcapColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_MatcapTex);
float4  _MatcapTex_ST;
float   _MatcapBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_MatcapBlendMask);
SET_TEX_PROPERTIES(_MatcapBlendMask);
float   _MatcapNormalMix;
float   _MatcapShadeMix;
uint    _MatcapMix;

bool    _UseMatcap2nd;
float4  _Matcap2ndColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Matcap2ndTex);
float4  _Matcap2ndTex_ST;
float   _Matcap2ndBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Matcap2ndBlendMask);
SET_TEX_PROPERTIES(_Matcap2ndBlendMask);
float   _Matcap2ndNormalMix;
float   _Matcap2ndShadeMix;
uint    _Matcap2ndMix;

/* マットキャップ追加分
bool    _UseMatcap3rd;
float4  _Matcap3rdColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Matcap3rdTex);
float4  _Matcap3rdTex_ST;
float   _Matcap3rdBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Matcap3rdBlendMask);
SET_TEX_PROPERTIES(_Matcap3rdBlendMask);
float   _Matcap3rdNormalMix;
float   _Matcap3rdShadeMix;
uint    _Matcap3rdMix;

bool    _UseMatcap4th;
float4  _Matcap4thColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Matcap4thTex);
float4  _Matcap4thTex_ST;
float   _Matcap4thBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Matcap4thBlendMask);
SET_TEX_PROPERTIES(_Matcap4thBlendMask);
float   _Matcap4thNormalMix;
float   _Matcap4thShadeMix;
uint    _Matcap4thMix;
*/

//------------------------------------------------------------------------------------------------------------------------------
// リムライト
bool    _UseRim;
float4  _RimColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_RimColorTex);
SET_TEX_PROPERTIES(_RimColorTex);
float   _RimBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_RimBlendMask);
SET_TEX_PROPERTIES(_RimBlendMask);
float   _RimToon;
float   _RimBorder;
UNITY_DECLARE_TEX2D_NOSAMPLER(_RimBorderMask);
SET_TEX_PROPERTIES(_RimBorderMask);
float   _RimBlur;
UNITY_DECLARE_TEX2D_NOSAMPLER(_RimBlurMask);
SET_TEX_PROPERTIES(_RimBlurMask);
float   _RimUpperSideWidth;
float   _RimFresnelPower;
float   _RimShadeMix;
bool    _RimShadowMask;

/* リムライト追加分
bool    _UseRim2nd;
float4  _Rim2ndColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Rim2ndColorTex);
SET_TEX_PROPERTIES(_Rim2ndColorTex);
float   _Rim2ndBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Rim2ndBlendMask);
SET_TEX_PROPERTIES(_Rim2ndBlendMask);
float   _Rim2ndToon;
float   _Rim2ndBorder;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Rim2ndBorderMask);
SET_TEX_PROPERTIES(_Rim2ndBorderMask);
float   _Rim2ndBlur;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Rim2ndBlurMask);
SET_TEX_PROPERTIES(_Rim2ndBlurMask);
float   _Rim2ndUpperSideWidth;
float   _Rim2ndFresnelPower;
float   _Rim2ndShadeMix;
bool    _Rim2ndShadowMask;
*/

//------------------------------------------------------------------------------------------------------------------------------
// 発光 というか実質unlit
bool    _UseEmission;
float4  _EmissionColor;
UNITY_DECLARE_TEX2D(_EmissionMap);
SET_TEX_PROPERTIES(_EmissionMap);
float   _EmissionBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_EmissionBlendMask);
SET_TEX_PROPERTIES(_EmissionBlendMask);
float   _EmissionHue;
float   _EmissionSaturation;
float   _EmissionValue;
bool    _EmissionUseBlink;
float   _EmissionBlinkStrength;
float   _EmissionBlinkSpeed;
float   _EmissionBlinkOffset;
bool    _EmissionBlinkType;
bool    _EmissionUseGrad;
UNITY_DECLARE_TEX2D_NOSAMPLER(_EmissionGradTex);
float   _EmissionGradSpeed;
float   _EmissionParallaxDepth;
static float _EmissionBlinkSin = sin(_Time.r*_EmissionBlinkSpeed+_EmissionBlinkOffset)*0.5+0.5;
static float _EmissionBlinkRou = _EmissionBlinkType ? round(_EmissionBlinkSin) : _EmissionBlinkSin;
static float _EmissionBlink = 1.0 - mad(_EmissionBlinkStrength, -_EmissionBlinkRou, _EmissionBlinkStrength) * _EmissionUseBlink;

bool    _UseEmission2nd;
float4  _Emission2ndColor;
UNITY_DECLARE_TEX2D(_Emission2ndMap);
SET_TEX_PROPERTIES(_Emission2ndMap);
float   _Emission2ndBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission2ndBlendMask);
SET_TEX_PROPERTIES(_Emission2ndBlendMask);
float   _Emission2ndHue;
float   _Emission2ndSaturation;
float   _Emission2ndValue;
bool    _Emission2ndUseBlink;
float   _Emission2ndBlinkStrength;
float   _Emission2ndBlinkSpeed;
float   _Emission2ndBlinkOffset;
bool    _Emission2ndBlinkType;
bool    _Emission2ndUseGrad;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission2ndGradTex);
float   _Emission2ndGradSpeed;
float   _Emission2ndParallaxDepth;
static float _Emission2ndBlinkSin = sin(_Time.r*_Emission2ndBlinkSpeed+_Emission2ndBlinkOffset)*0.5+0.5;
static float _Emission2ndBlinkRou = _Emission2ndBlinkType ? round(_Emission2ndBlinkSin) : _Emission2ndBlinkSin;
static float _Emission2ndBlink = 1.0 - mad(_Emission2ndBlinkStrength, -_Emission2ndBlinkRou, _Emission2ndBlinkStrength) * _Emission2ndUseBlink;

bool    _UseEmission3rd;
float4  _Emission3rdColor;
UNITY_DECLARE_TEX2D(_Emission3rdMap);
SET_TEX_PROPERTIES(_Emission3rdMap);
float   _Emission3rdBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission3rdBlendMask);
SET_TEX_PROPERTIES(_Emission3rdBlendMask);
float   _Emission3rdHue;
float   _Emission3rdSaturation;
float   _Emission3rdValue;
bool    _Emission3rdUseBlink;
float   _Emission3rdBlinkStrength;
float   _Emission3rdBlinkSpeed;
float   _Emission3rdBlinkOffset;
bool    _Emission3rdBlinkType;
bool    _Emission3rdUseGrad;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission3rdGradTex);
float   _Emission3rdGradSpeed;
float   _Emission3rdParallaxDepth;
static float _Emission3rdBlinkSin = sin(_Time.r*_Emission3rdBlinkSpeed+_Emission3rdBlinkOffset)*0.5+0.5;
static float _Emission3rdBlinkRou = _Emission3rdBlinkType ? round(_Emission3rdBlinkSin) : _Emission3rdBlinkSin;
static float _Emission3rdBlink = 1.0 - mad(_Emission3rdBlinkStrength, -_Emission3rdBlinkRou, _Emission3rdBlinkStrength) * _Emission3rdUseBlink;

bool    _UseEmission4th;
float4  _Emission4thColor;
UNITY_DECLARE_TEX2D(_Emission4thMap);
SET_TEX_PROPERTIES(_Emission4thMap);
float   _Emission4thBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission4thBlendMask);
SET_TEX_PROPERTIES(_Emission4thBlendMask);
float   _Emission4thHue;
float   _Emission4thSaturation;
float   _Emission4thValue;
bool    _Emission4thUseBlink;
float   _Emission4thBlinkStrength;
float   _Emission4thBlinkSpeed;
float   _Emission4thBlinkOffset;
bool    _Emission4thBlinkType;
bool    _Emission4thUseGrad;
UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission4thGradTex);
float   _Emission4thGradSpeed;
float   _Emission4thParallaxDepth;
static float _Emission4thBlinkSin = sin(_Time.r*_Emission4thBlinkSpeed+_Emission4thBlinkOffset)*0.5+0.5;
static float _Emission4thBlinkRou = _Emission4thBlinkType ? round(_Emission4thBlinkSin) : _Emission4thBlinkSin;
static float _Emission4thBlink = 1.0 - mad(_Emission4thBlinkStrength, -_Emission4thBlinkRou, _Emission4thBlinkStrength) * _Emission4thUseBlink;
#endif
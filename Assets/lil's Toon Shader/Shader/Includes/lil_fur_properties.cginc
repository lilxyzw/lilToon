// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------------------
// サンプラー
SamplerState sampler_linear_repeat;

//------------------------------------------------------------------------------------------------------------------------------
// テクスチャで使うパラメーターをセットアップ
#define GET_TEX(tex) getTexLite(tex, i.uv, sampler_linear_repeat)
#define GET_MASK(tex) getMaskLite(tex, i.uv, sampler_linear_repeat)
#define GET_TEX_V(tex) getTexLite_V(tex, o.uv, sampler_linear_repeat)
#define GET_MASK_V(tex) getMaskLite_V(tex, o.uv, sampler_linear_repeat)
#define GET_TEX_SAMP(tex) getTexLite(tex, i.uv, sampler##tex)
#define GET_EMITEX(tex) getTex(tex, tex##_ST, tex##UVoffset, tex##RotMix, tex##ParaTex, tex##Trim, sampler##tex)
#define GET_EMIMASK(tex) getMask(tex, tex##_ST, tex##UVoffset, tex##RotMix, i.uv, tex##Trim, sampler_linear_repeat)
#define GET_TEXSUB(tex) getTex(tex, tex##_ST, tex##_ST.zw, tex##RotMix, i.uv, tex##Trim, sampler##tex)
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

//------------------------------------------------------------------------------------------------------------------------------
// メイン
float4  _Color;
UNITY_DECLARE_TEX2D(_MainTex);
float4  _MainTex_ST;
float   _MainTexTonecurve;
float   _MainTexHue;
float   _MainTexSaturation;
float   _MainTexValue;

//------------------------------------------------------------------------------------------------------------------------------
// アルファ
#if LIL_RENDER != 0
    bool    _UseAlphaMask;
    UNITY_DECLARE_TEX2D_NOSAMPLER(_AlphaMask);
#endif

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
// ファー
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
float _FurAO;
bool _VertexColor2FurVector;
uint _FurLayerNum;
static float _InvFurLayerNum = 1.0/_FurLayerNum;

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
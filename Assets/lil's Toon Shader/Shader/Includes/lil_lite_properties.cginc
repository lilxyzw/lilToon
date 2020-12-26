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
#define SET_TEX_PROPERTIES(tex) \
    float4          tex##_ST; \
    float           tex##ScrollX; \
    float           tex##ScrollY; \
    float           tex##Angle; \
    float           tex##Rotate; \
    int             tex##UV; \
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

bool    _UseMain2ndTex;
float4  _Color2nd;
UNITY_DECLARE_TEX2D(_Main2ndTex);
SET_TEXSUB_PROPERTIES(_Main2ndTex);
float   _Main2ndTexMix;

//------------------------------------------------------------------------------------------------------------------------------
// アルファ
#if LIL_RENDER != 0
    bool    _UseAlphaMask;
    UNITY_DECLARE_TEX2D_NOSAMPLER(_AlphaMask);
#endif

//------------------------------------------------------------------------------------------------------------------------------
// 影
bool    _UseShadow;
float   _ShadowBorder;
float   _ShadowBlur;
UNITY_DECLARE_TEX2D_NOSAMPLER(_ShadowStrengthMask);
float4  _ShadowColor;

//------------------------------------------------------------------------------------------------------------------------------
// 輪郭線
bool    _UseOutline;
bool    _OutlineMixMain;
float   _OutlineMixMainStrength;
float   _OutlineWidth;
UNITY_DECLARE_TEX2D_NOSAMPLER(_OutlineAlphaMask);
float4  _OutlineColor;
int     _VertexColor2Width;
static int _vc2w = _VertexColor2Width-1;

//------------------------------------------------------------------------------------------------------------------------------
// マットキャップ
bool    _UseMatcap;
float4  _MatcapColor;
UNITY_DECLARE_TEX2D_NOSAMPLER(_MatcapTex);
float   _MatcapBlend;
UNITY_DECLARE_TEX2D_NOSAMPLER(_MatcapBlendMask);
float   _MatcapShadeMix;
int     _MatcapMix;

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

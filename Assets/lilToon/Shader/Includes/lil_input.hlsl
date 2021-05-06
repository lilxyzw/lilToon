#ifndef LIL_INPUT_INCLUDED
#define LIL_INPUT_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Variables from Unity
#if !defined(SPOT) && !defined(POINT_COOKIE) && LIL_VERTEXLIGHT_MODE == 4
    TEXTURE2D(_LightTextureB0);
    SAMPLER(sampler_LightTextureB0);
#endif

TEXTURE3D(_DitherMaskLOD);
TEXTURE2D(_BackgroundTexture);
TEXTURE2D(_GrabTexture);
TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_linear_repeat);
SAMPLER(sampler_DitherMaskLOD);
SAMPLER(sampler_BackgroundTexture);
SAMPLER(sampler_GrabTexture);
SAMPLER(sampler_CameraDepthTexture);
float4 _GrabTexture_TexelSize;

//------------------------------------------------------------------------------------------------------------------------------
// Input

// ST               x:scale y:scale z:offset w:offset
// ScrollRotate     x:scroll y:scroll z:angle w:rotation
// Blink            x:strength y:type z:speed w:offset
// HSVG             x:hue y:saturation z:value w:gamma
// BlendMode        0:normal 1:add 2:screen 3:multiply

// bool does not work in cbuffer
//#define lilBool bool
#define lilBool uint

CBUFFER_START(UnityPerMaterial)
//------------------------------------------------------------------------------------------------------------------------------
// Common
float   _Cutoff;
float   _FlipNormal;
float   _BackfaceForceShadow;
lilBool _Invisible;
float _TessEdge;
float _TessStrength;
float _TessShrink;
float _TessFactorMax;

//------------------------------------------------------------------------------------------------------------------------------
// Texture properties
LIL_TEX_PROPERTIES(_MainTex);
LIL_TEXSUB_PROPERTIES(_Main2ndTex);
LIL_TEXSUB_PROPERTIES(_Main3rdTex);
LIL_TEX_PROPERTIES(_EmissionMap);
LIL_TEX_PROPERTIES(_EmissionBlendMask);
LIL_TEX_PROPERTIES(_Emission2ndMap);
LIL_TEX_PROPERTIES(_Emission2ndBlendMask);

//------------------------------------------------------------------------------------------------------------------------------
// Main
float4  _Color;
float4  _MainTexHSVG;

lilBool _UseMain2ndTex;
float4  _Color2nd;
uint    _Main2ndTexBlendMode;

lilBool _UseMain3rdTex;
float4  _Color3rd;
uint    _Main3rdTexBlendMode;

//------------------------------------------------------------------------------------------------------------------------------
// Normal Map
lilBool _UseBumpMap;
float4  _BumpMap_ST;
float   _BumpScale;

lilBool _UseBump2ndMap;
float4  _Bump2ndMap_ST;
float   _Bump2ndScale;

//------------------------------------------------------------------------------------------------------------------------------
// Shadow
lilBool _UseShadow;
float   _ShadowBorder;
float   _ShadowBlur;
float   _ShadowStrength;

float4  _ShadowColor;
float   _Shadow2ndBorder;
float   _Shadow2ndBlur;
float4  _Shadow2ndColor;
float   _ShadowMainStrength;
float   _ShadowEnvStrength;
float4  _ShadowBorderColor;
float   _ShadowBorderRange;
lilBool _ShadowReceive;

//------------------------------------------------------------------------------------------------------------------------------
// Outline
float4  _OutlineColor;
lilBool _OutlineUseMainColor;
float   _OutlineMainStrength;
float   _OutlineWidth;

//------------------------------------------------------------------------------------------------------------------------------
// Reflection
lilBool _UseReflection;
float   _Smoothness;
float   _Metallic;
lilBool _ApplySpecular;
lilBool _SpecularToon;
lilBool _ApplyReflection;
float4  _ReflectionColor;

//------------------------------------------------------------------------------------------------------------------------------
// MatCap
lilBool _UseMatCap;
float4  _MatCapColor;
float   _MatCapBlend;
lilBool _MatCapBlendLight;
uint    _MatCapBlendMode;
lilBool _MatCapMul;

//------------------------------------------------------------------------------------------------------------------------------
// Rim light
lilBool _UseRim;
float4  _RimColor;
float   _RimBorder;
float   _RimBlur;
float   _RimFresnelPower;
lilBool _RimBlendLight;
lilBool _RimShadowMask;

//------------------------------------------------------------------------------------------------------------------------------
// Emission
lilBool _UseEmission;
float4  _EmissionColor;
float   _EmissionBlend;
float4  _EmissionBlink; // x:strength y:type z:speed w:offset
lilBool _EmissionUseGrad;
float   _EmissionGradSpeed;
float   _EmissionParallaxDepth;
float   _EmissionFluorescence;

lilBool _UseEmission2nd;
float4  _Emission2ndColor;
float   _Emission2ndBlend;
float4  _Emission2ndBlink; // x:strength y:type z:speed w:offset
lilBool _Emission2ndUseGrad;
float   _Emission2ndGradSpeed;
float   _Emission2ndParallaxDepth;
float   _Emission2ndFluorescence;

//------------------------------------------------------------------------------------------------------------------------------
// Parallax
lilBool _UseParallax;
float   _Parallax;
float   _ParallaxOffset;

//------------------------------------------------------------------------------------------------------------------------------
// Fur
float4  _FurNoiseMask_ST;
float   _FurVectorScale;
float4  _FurVector;
float   _FurGravity;
float   _FurAO;
lilBool _VertexColor2FurVector;
uint    _FurLayerNum;

//------------------------------------------------------------------------------------------------------------------------------
// Refraction
float   _RefractionStrength;
float   _RefractionFresnelPower;
lilBool _RefractionColorFromMain;
float4  _RefractionColor;
CBUFFER_END

//------------------------------------------------------------------------------------------------------------------------------
// Texture
TEXTURE2D(_MainTex);
TEXTURE2D(_Main2ndTex);
TEXTURE2D(_Main2ndBlendMask);
TEXTURE2D(_Main3rdTex);
TEXTURE2D(_Main3rdBlendMask);
TEXTURE2D(_BumpMap);
TEXTURE2D(_Bump2ndMap);
TEXTURE2D(_Bump2ndScaleMask);
TEXTURE2D(_ShadowBorderMask);
TEXTURE2D(_ShadowBlurMask);
TEXTURE2D(_ShadowStrengthMask);
TEXTURE2D(_ShadowColorTex);
TEXTURE2D(_Shadow2ndColorTex);
TEXTURE2D(_OutlineWidthMask);
TEXTURE2D(_SmoothnessTex);
TEXTURE2D(_MetallicGlossMap);
TEXTURE2D(_ReflectionColorTex);
TEXTURE2D(_MatCapTex);
TEXTURE2D(_MatCapBlendMask);
TEXTURE2D(_RimColorTex);
TEXTURE2D(_EmissionMap);
TEXTURE2D(_EmissionBlendMask);
TEXTURE2D(_EmissionGradTex);
TEXTURE2D(_Emission2ndMap);
TEXTURE2D(_Emission2ndBlendMask);
TEXTURE2D(_Emission2ndGradTex);
TEXTURE2D(_ParallaxMap);
TEXTURE2D(_FurNoiseMask);
TEXTURE2D(_FurMask);
TEXTURE2D(_FurVectorTex);
TEXTURE2D(_TriMask);
SAMPLER(sampler_MainTex);
SAMPLER(sampler_Main2ndTex);
SAMPLER(sampler_Main3rdTex);
SAMPLER(sampler_EmissionMap);
SAMPLER(sampler_Emission2ndMap);

#endif
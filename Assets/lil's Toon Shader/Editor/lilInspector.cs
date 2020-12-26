using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.IO;

// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE
namespace lilToon
{
    public class lilToonInspector : ShaderGUI
    {
        public static bool Foldout(string title, string help, bool display)
        {
            // ShurikenModuleの拡張版
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Editor/rounded-x-mplus-1c-bold.ttf", typeof(Font)) as Font;
            style.fontSize = 12;
            style.border = new RectOffset(15, 7, 4, 4);
            style.contentOffset = new Vector2(20f, -2f);
            style.fixedHeight = 22;
            var rect = GUILayoutUtility.GetRect(16f, 20f, style);
            GUI.Box(rect, new GUIContent(title, help), style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                display = !display;
                e.Use();
            }

            return display;
        }

        public Vector2 makeScrLabel(Vector2 vector)
        {
            GUIContent ScrLabel = new GUIContent();
            ScrLabel.text = loc["sScroll"];
            Rect position = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
            position.y -= 2; // Tiling・Offsetにフィット
            float labelWidth = EditorGUIUtility.labelWidth - 15;
            Rect totalPosition = new Rect(position.x, position.y, labelWidth, 16f);
            Rect rect2 = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, 16f);
            EditorGUI.PrefixLabel(totalPosition, ScrLabel);

            vector = EditorGUI.Vector2Field(rect2, GUIContent.none, vector);
            return vector;
        }

        public Vector3 makeVec3Label(Vector3 vector)
        {
            GUIContent ScrLabel = new GUIContent();
            ScrLabel.text = loc["sVector"];
            Rect position = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
            float labelWidth = EditorGUIUtility.labelWidth;
            Rect totalPosition = new Rect(position.x, position.y, labelWidth, 16f);
            Rect rect2 = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, 16f);
            EditorGUI.PrefixLabel(totalPosition, ScrLabel);

            vector = EditorGUI.Vector3Field(rect2, GUIContent.none, vector);
            return vector;
        }

        public enum EditorMode // 編集モード
        {
            Limited,    // 色替えのみ
            Advanced,   // アバター製作者向け
            Preset      // プリセット
        }

        public enum RenderingMode // レンダリングモード
        {
            Opaque,     // 透過なし
            Cutout,     // 透過部分を閾値でカット
            Transparent,// 半透明
            Add,        // 加算
            Multiply,   // 乗算
            Refraction, // 屈折
            Fur         // ファー
        }

        public enum UVSet // UV
        {
            UV0,
            UV1,
            UV2,
            UV3,
            Skybox
        }

        public enum MixMode // Mix
        {
            Alpha,
            Add,
            Screen,
            Mul,
            Overlay
        }

        Dictionary<string, string> loc = new Dictionary<string, string>();
        //string langdataRaw;
        //string[] langdataArray;

        public string[] TrimlerStateList = // TrimlerStateのやつ
        {
            "Linear Repeat (線形補間・ループ)",
            "Linear Clamp (線形補間・ループなし)",
            "Linear Mirror (線形補間・ミラー)",
            "Linear Border (線形補間・トリミング)",
            "Point Repeat (補間なし・ループ)",
            "Point Clamp (補間なし・ループなし)",
            "Point Mirror (補間なし・ミラー)",
            "Point Border (補間なし・トリミング)"
        };

        static EditorMode edMode = EditorMode.Limited;
        static int langNum = 0;

        // エディター用変数
        RenderingMode rendModeBuf;
        #region isShow
        static bool isShowDefaultShadingMask = false;
	    static bool isShowMain = false;
	        static bool isShowMainTex = false;
	        static bool isShowMain2ndTex = false;
	        static bool isShowMain2ndBlendMask = false;
	        static bool isShowMain3rdTex = false;
	        static bool isShowMain3rdBlendMask = false;
	        static bool isShowMain4thTex = false;
	        static bool isShowMain4thBlendMask = false;
        static bool isShowAlpha = false;
	        static bool isShowAlphaMask = false;
	        static bool isShowAlphaMask2nd = false;
	    static bool isShowShadow = false;
            static bool isShowShadowBorderMask = false;
            static bool isShowShadowBlurMask = false;
            static bool isShowShadowStrengthMask = false;
            static bool isShowShadowColor = false;
            static bool isShowShadow2ndColor = false;
	    static bool isShowOutline = false;
        	static bool isShowOutlineColorTex = false;
        	static bool isShowOutlineWidthMask = false;
        	static bool isShowOutlineAlphaMask = false;
	    static bool isShowBump = false;
            static bool isShowBumpMap = false;
            static bool isShowBumpScaleMask = false;
            static bool isShowBump2ndMap = false;
            static bool isShowBump2ndScaleMask = false;
            static bool isShowBump3rdMap = false;
            static bool isShowBump3rdScaleMask = false;
            static bool isShowBump4thMap = false;
            static bool isShowBump4thScaleMask = false;
	    static bool isShowReflections = false;
            static bool isShowMetallic = false;
            static bool isShowSmoothness = false;
            static bool isShowReflectionBlendMask = false;
	        static bool isShowMatcapTex = false;
	        static bool isShowMatcapBlendMask = false;
	        static bool isShowMatcap2ndTex = false;
	        static bool isShowMatcap2ndBlendMask = false;
            /* マットキャップ追加分
	        static bool isShowMatcap3rdTex = false;
	        static bool isShowMatcap3rdBlendMask = false;
	        static bool isShowMatcap4thTex = false;
	        static bool isShowMatcap4thBlendMask = false;
            */
	        static bool isShowRimColorTex = false;
	        static bool isShowRimBlendMask = false;
            static bool isShowRimBorderMask = false;
            static bool isShowRimBlurMask = false;
	        static bool isShowRim2ndColorTex = false;
	        static bool isShowRim2ndBlendMask = false;
            static bool isShowRim2ndBorderMask = false;
            static bool isShowRim2ndBlurMask = false;
	        //static bool isShowClearCoat = false;
	        //static bool isShowGlass = false;
	    static bool isShowEmission = false;
            static bool isShowEmissionMap = false;
            static bool isShowEmissionBlendMask = false;
            static bool isShowEmission2ndMap = false;
            static bool isShowEmission2ndBlendMask = false;
            static bool isShowEmission3rdMap = false;
            static bool isShowEmission3rdBlendMask = false;
            static bool isShowEmission4thMap = false;
            static bool isShowEmission4thBlendMask = false;
	    static bool isShowStencil = false;
	    static bool isShowAnimationTexture = false;
	    static bool isShowRefraction = false;
	    static bool isShowFur = false;
	    static bool isShowRendering = false;
	    static bool isShowPresets = false;
        #endregion

        #region MaterialProperties
        MaterialProperty invisible;
        MaterialProperty renderingMode;
		MaterialProperty cullMode;
		MaterialProperty srcBlend;
		MaterialProperty dstBlend;
		MaterialProperty zwrite;
		MaterialProperty ztest;
        MaterialProperty cutoff;
        MaterialProperty flipNormal;
        MaterialProperty backfaceForceShadow;
        MaterialProperty useVertexColor;
        MaterialProperty stencilRef;
        MaterialProperty stencilComp;
        MaterialProperty stencilPass;
        MaterialProperty stencilFail;
        MaterialProperty stencilZFail;
        //MaterialProperty useMainTex;
            MaterialProperty mainColor;
            MaterialProperty mainTex;
            MaterialProperty mainTexScrollX;
            MaterialProperty mainTexScrollY;
            MaterialProperty mainTexAngle;
            MaterialProperty mainTexRotate;
            MaterialProperty mainTexUV;
            MaterialProperty mainTexTrim;
            MaterialProperty mainTexTonecurve;
            MaterialProperty mainTexHue;
            MaterialProperty mainTexSaturation;
            MaterialProperty mainTexValue;
        MaterialProperty useMain2ndTex;
            MaterialProperty mainColor2nd;
            MaterialProperty main2ndTex;
            MaterialProperty main2ndTexScrollX;
            MaterialProperty main2ndTexScrollY;
            MaterialProperty main2ndTexAngle;
            MaterialProperty main2ndTexRotate;
            MaterialProperty main2ndTexUV;
            MaterialProperty main2ndTexTrim;
            MaterialProperty main2ndBlend;
            MaterialProperty main2ndBlendMask;
            MaterialProperty main2ndBlendMaskScrollX;
            MaterialProperty main2ndBlendMaskScrollY;
            MaterialProperty main2ndBlendMaskAngle;
            MaterialProperty main2ndBlendMaskRotate;
            MaterialProperty main2ndBlendMaskUV;
            MaterialProperty main2ndBlendMaskTrim;
            MaterialProperty main2ndTexMix;
            MaterialProperty main2ndTexHue;
            MaterialProperty main2ndTexSaturation;
            MaterialProperty main2ndTexValue;
        MaterialProperty useMain3rdTex;
            MaterialProperty mainColor3rd;
            MaterialProperty main3rdTex;
            MaterialProperty main3rdTexScrollX;
            MaterialProperty main3rdTexScrollY;
            MaterialProperty main3rdTexAngle;
            MaterialProperty main3rdTexRotate;
            MaterialProperty main3rdTexUV;
            MaterialProperty main3rdTexTrim;
            MaterialProperty main3rdBlend;
            MaterialProperty main3rdBlendMask;
            MaterialProperty main3rdBlendMaskScrollX;
            MaterialProperty main3rdBlendMaskScrollY;
            MaterialProperty main3rdBlendMaskAngle;
            MaterialProperty main3rdBlendMaskRotate;
            MaterialProperty main3rdBlendMaskUV;
            MaterialProperty main3rdBlendMaskTrim;
            MaterialProperty main3rdTexMix;
            MaterialProperty main3rdTexHue;
            MaterialProperty main3rdTexSaturation;
            MaterialProperty main3rdTexValue;
        MaterialProperty useMain4thTex;
            MaterialProperty mainColor4th;
            MaterialProperty main4thTex;
            MaterialProperty main4thTexScrollX;
            MaterialProperty main4thTexScrollY;
            MaterialProperty main4thTexAngle;
            MaterialProperty main4thTexRotate;
            MaterialProperty main4thTexUV;
            MaterialProperty main4thTexTrim;
            MaterialProperty main4thBlend;
            MaterialProperty main4thBlendMask;
            MaterialProperty main4thBlendMaskScrollX;
            MaterialProperty main4thBlendMaskScrollY;
            MaterialProperty main4thBlendMaskAngle;
            MaterialProperty main4thBlendMaskRotate;
            MaterialProperty main4thBlendMaskUV;
            MaterialProperty main4thBlendMaskTrim;
            MaterialProperty main4thTexMix;
            MaterialProperty main4thTexHue;
            MaterialProperty main4thTexSaturation;
            MaterialProperty main4thTexValue;
        MaterialProperty useAlphaMask;
            MaterialProperty alpha;
            MaterialProperty alphaMask;
            MaterialProperty alphaMaskScrollX;
            MaterialProperty alphaMaskScrollY;
            MaterialProperty alphaMaskAngle;
            MaterialProperty alphaMaskRotate;
            MaterialProperty alphaMaskUV;
            MaterialProperty alphaMaskTrim;
            MaterialProperty alphaMaskMixMain;
        /* アルファマスク追加分
        MaterialProperty useAlphaMask2nd;
            MaterialProperty alpha2nd;
            MaterialProperty alphaMask2nd;
            MaterialProperty alphaMask2ndScrollX;
            MaterialProperty alphaMask2ndScrollY;
            MaterialProperty alphaMask2ndAngle;
            MaterialProperty alphaMask2ndRotate;
            MaterialProperty alphaMask2ndUV;
            MaterialProperty alphaMask2ndTrim;
            MaterialProperty alphaMask2ndMixMain;
        */
        MaterialProperty useShadow;
            MaterialProperty shadowBorder;
            MaterialProperty shadowBorderMask;
            MaterialProperty shadowBorderMaskScrollX;
            MaterialProperty shadowBorderMaskScrollY;
            MaterialProperty shadowBorderMaskAngle;
            MaterialProperty shadowBorderMaskRotate;
            MaterialProperty shadowBorderMaskUV;
            MaterialProperty shadowBorderMaskTrim;
            MaterialProperty shadowBlur;
            MaterialProperty shadowBlurMask;
            MaterialProperty shadowBlurMaskScrollX;
            MaterialProperty shadowBlurMaskScrollY;
            MaterialProperty shadowBlurMaskAngle;
            MaterialProperty shadowBlurMaskRotate;
            MaterialProperty shadowBlurMaskUV;
            MaterialProperty shadowBlurMaskTrim;
            MaterialProperty shadowStrength;
            MaterialProperty shadowStrengthMask;
            MaterialProperty shadowStrengthMaskScrollX;
            MaterialProperty shadowStrengthMaskScrollY;
            MaterialProperty shadowStrengthMaskRotate;
            MaterialProperty shadowStrengthMaskAngle;
            MaterialProperty shadowStrengthMaskUV;
            MaterialProperty shadowStrengthMaskTrim;
            MaterialProperty useShadowMixMainColor;
            MaterialProperty shadowMixMainColor;
            MaterialProperty shadowGrad;
            MaterialProperty shadowGradColor;
            MaterialProperty shadowHue;
            MaterialProperty shadowSaturation;
            MaterialProperty shadowValue;
            MaterialProperty useShadowColor;
            MaterialProperty shadowColorFromMain;
            MaterialProperty shadowColor;
            MaterialProperty shadowColorTex;
            MaterialProperty shadowColorTexScrollX;
            MaterialProperty shadowColorTexScrollY;
            MaterialProperty shadowColorTexAngle;
            MaterialProperty shadowColorTexRotate;
            MaterialProperty shadowColorTexUV;
            MaterialProperty shadowColorTexTrim;
            MaterialProperty shadowColorMix;
        MaterialProperty useShadow2nd;
            MaterialProperty shadow2ndBorder;
            MaterialProperty shadow2ndBlur;
            MaterialProperty shadow2ndColorFromMain;
            MaterialProperty shadow2ndColor;
            MaterialProperty shadow2ndColorTex;
            MaterialProperty shadow2ndColorTexScrollX;
            MaterialProperty shadow2ndColorTexScrollY;
            MaterialProperty shadow2ndColorTexAngle;
            MaterialProperty shadow2ndColorTexRotate;
            MaterialProperty shadow2ndColorTexUV;
            MaterialProperty shadow2ndColorTexTrim;
            MaterialProperty shadow2ndColorMix;
        MaterialProperty useDefaultShading;
            MaterialProperty defaultShadingBlend;
            MaterialProperty defaultShadingBlendMask;
            MaterialProperty defaultShadingBlendMaskScrollX;
            MaterialProperty defaultShadingBlendMaskScrollY;
            MaterialProperty defaultShadingBlendMaskAngle;
            MaterialProperty defaultShadingBlendMaskRotate;
            MaterialProperty defaultShadingBlendMaskUV;
            MaterialProperty defaultShadingBlendMaskTrim;
        MaterialProperty useOutline;
            MaterialProperty outlineMixMainStrength;
            MaterialProperty outlineHue;
            MaterialProperty outlineSaturation;
            MaterialProperty outlineValue;
            MaterialProperty outlineAutoHue;
            MaterialProperty outlineAutoValue;
            MaterialProperty useOutlineColor;
            MaterialProperty outlineColor;
            MaterialProperty outlineColorTex;
            MaterialProperty outlineColorTexScrollX;
            MaterialProperty outlineColorTexScrollY;
            MaterialProperty outlineColorTexAngle;
            MaterialProperty outlineColorTexRotate;
            MaterialProperty outlineColorTexUV;
            MaterialProperty outlineColorTexTrim;
            MaterialProperty outlineWidth;
            MaterialProperty outlineWidthMask;
            MaterialProperty outlineWidthMaskScrollX;
            MaterialProperty outlineWidthMaskScrollY;
            MaterialProperty outlineWidthMaskAngle;
            MaterialProperty outlineWidthMaskRotate;
            MaterialProperty outlineWidthMaskUV;
            MaterialProperty outlineWidthMaskTrim;
            MaterialProperty outlineAlpha;
            MaterialProperty outlineAlphaMask;
            MaterialProperty outlineAlphaMaskScrollX;
            MaterialProperty outlineAlphaMaskScrollY;
            MaterialProperty outlineAlphaMaskAngle;
            MaterialProperty outlineAlphaMaskRotate;
            MaterialProperty outlineAlphaMaskUV;
            MaterialProperty outlineAlphaMaskTrim;
            MaterialProperty vertexColor2Width;
        //MaterialProperty useRefraction;
            MaterialProperty refractionStrength;
            MaterialProperty refractionFresnelPower;
            MaterialProperty refractionColorFromMain;
            MaterialProperty refractionColor;
        //MaterialProperty useFur;
            MaterialProperty furNoiseMask;
            MaterialProperty furMask;
            MaterialProperty furVectorTex;
            MaterialProperty furVectorScale;
            MaterialProperty furVectorX;
            MaterialProperty furVectorY;
            MaterialProperty furVectorZ;
            MaterialProperty furLength;
            MaterialProperty furGravity;
            MaterialProperty furAO;
            MaterialProperty vertexColor2FurVector;
            MaterialProperty furLayerNum;
        MaterialProperty useBumpMap;
            MaterialProperty bumpMap;
            MaterialProperty bumpMapScrollX;
            MaterialProperty bumpMapScrollY;
            MaterialProperty bumpMapAngle;
            MaterialProperty bumpMapRotate;
            MaterialProperty bumpMapUV;
            MaterialProperty bumpMapTrim;
            MaterialProperty bumpScale;
            MaterialProperty bumpScaleMask;
            MaterialProperty bumpScaleMaskScrollX;
            MaterialProperty bumpScaleMaskScrollY;
            MaterialProperty bumpScaleMaskAngle;
            MaterialProperty bumpScaleMaskRotate;
            MaterialProperty bumpScaleMaskUV;
            MaterialProperty bumpScaleMaskTrim;
        MaterialProperty useBump2ndMap;
            MaterialProperty bump2ndMap;
            MaterialProperty bump2ndMapScrollX;
            MaterialProperty bump2ndMapScrollY;
            MaterialProperty bump2ndMapAngle;
            MaterialProperty bump2ndMapRotate;
            MaterialProperty bump2ndMapUV;
            MaterialProperty bump2ndMapTrim;
            MaterialProperty bump2ndScale;
            MaterialProperty bump2ndScaleMask;
            MaterialProperty bump2ndScaleMaskScrollX;
            MaterialProperty bump2ndScaleMaskScrollY;
            MaterialProperty bump2ndScaleMaskAngle;
            MaterialProperty bump2ndScaleMaskRotate;
            MaterialProperty bump2ndScaleMaskUV;
            MaterialProperty bump2ndScaleMaskTrim;
        /* ノーマルマップ追加分
        MaterialProperty useBump3rdMap;
            MaterialProperty bump3rdMap;
            MaterialProperty bump3rdMapScrollX;
            MaterialProperty bump3rdMapScrollY;
            MaterialProperty bump3rdMapAngle;
            MaterialProperty bump3rdMapRotate;
            MaterialProperty bump3rdMapUV;
            MaterialProperty bump3rdMapTrim;
            MaterialProperty bump3rdScale;
            MaterialProperty bump3rdScaleMask;
            MaterialProperty bump3rdScaleMaskScrollX;
            MaterialProperty bump3rdScaleMaskScrollY;
            MaterialProperty bump3rdScaleMaskAngle;
            MaterialProperty bump3rdScaleMaskRotate;
            MaterialProperty bump3rdScaleMaskUV;
            MaterialProperty bump3rdScaleMaskTrim;
        MaterialProperty useBump4thMap;
            MaterialProperty bump4thMap;
            MaterialProperty bump4thMapScrollX;
            MaterialProperty bump4thMapScrollY;
            MaterialProperty bump4thMapAngle;
            MaterialProperty bump4thMapRotate;
            MaterialProperty bump4thMapUV;
            MaterialProperty bump4thMapTrim;
            MaterialProperty bump4thScale;
            MaterialProperty bump4thScaleMask;
            MaterialProperty bump4thScaleMaskScrollX;
            MaterialProperty bump4thScaleMaskScrollY;
            MaterialProperty bump4thScaleMaskAngle;
            MaterialProperty bump4thScaleMaskRotate;
            MaterialProperty bump4thScaleMaskUV;
            MaterialProperty bump4thScaleMaskTrim;
        */
        MaterialProperty useReflection;
            MaterialProperty metallic;
            MaterialProperty metallicGlossMap;
            MaterialProperty metallicGlossMapScrollX;
            MaterialProperty metallicGlossMapScrollY;
            MaterialProperty metallicGlossMapAngle;
            MaterialProperty metallicGlossMapRotate;
            MaterialProperty metallicGlossMapUV;
            MaterialProperty metallicGlossMapTrim;
            MaterialProperty smoothness;
            MaterialProperty smoothnessTex;
            MaterialProperty smoothnessTexScrollX;
            MaterialProperty smoothnessTexScrollY;
            MaterialProperty smoothnessTexAngle;
            MaterialProperty smoothnessTexRotate;
            MaterialProperty smoothnessTexUV;
            MaterialProperty smoothnessTexTrim;
            MaterialProperty reflectionBlend;
            MaterialProperty reflectionBlendMask;
            MaterialProperty reflectionBlendMaskScrollX;
            MaterialProperty reflectionBlendMaskScrollY;
            MaterialProperty reflectionBlendMaskAngle;
            MaterialProperty reflectionBlendMaskRotate;
            MaterialProperty reflectionBlendMaskUV;
            MaterialProperty reflectionBlendMaskTrim;
            MaterialProperty applySpecular;
            MaterialProperty applyReflection;
            MaterialProperty reflectionUseCubemap;
            MaterialProperty reflectionCubemap;
            MaterialProperty reflectionShadeMix;
        MaterialProperty useMatcap;
            MaterialProperty matcapTex;
            MaterialProperty matcapColor;
            MaterialProperty matcapBlend;
            MaterialProperty matcapBlendMask;
            MaterialProperty matcapBlendMaskScrollX;
            MaterialProperty matcapBlendMaskScrollY;
            MaterialProperty matcapBlendMaskAngle;
            MaterialProperty matcapBlendMaskRotate;
            MaterialProperty matcapBlendMaskUV;
            MaterialProperty matcapBlendMaskTrim;
            MaterialProperty matcapNormalMix;
            MaterialProperty matcapShadeMix;
            MaterialProperty matcapMix;
        MaterialProperty useMatcap2nd;
            MaterialProperty matcap2ndTex;
            MaterialProperty matcap2ndColor;
            MaterialProperty matcap2ndBlend;
            MaterialProperty matcap2ndBlendMask;
            MaterialProperty matcap2ndBlendMaskScrollX;
            MaterialProperty matcap2ndBlendMaskScrollY;
            MaterialProperty matcap2ndBlendMaskAngle;
            MaterialProperty matcap2ndBlendMaskRotate;
            MaterialProperty matcap2ndBlendMaskUV;
            MaterialProperty matcap2ndBlendMaskTrim;
            MaterialProperty matcap2ndNormalMix;
            MaterialProperty matcap2ndShadeMix;
            MaterialProperty matcap2ndMix;
        /* マットキャップ追加分
        MaterialProperty useMatcap3rd;
            MaterialProperty matcap3rdTex;
            MaterialProperty matcap3rdColor;
            MaterialProperty matcap3rdBlend;
            MaterialProperty matcap3rdBlendMask;
            MaterialProperty matcap3rdBlendMaskScrollX;
            MaterialProperty matcap3rdBlendMaskScrollY;
            MaterialProperty matcap3rdBlendMaskAngle;
            MaterialProperty matcap3rdBlendMaskRotate;
            MaterialProperty matcap3rdBlendMaskUV;
            MaterialProperty matcap3rdBlendMaskTrim;
            MaterialProperty matcap3rdNormalMix;
            MaterialProperty matcap3rdShadeMix;
            MaterialProperty matcap3rdMix;
        MaterialProperty useMatcap4th;
            MaterialProperty matcap4thTex;
            MaterialProperty matcap4thColor;
            MaterialProperty matcap4thBlend;
            MaterialProperty matcap4thBlendMask;
            MaterialProperty matcap4thBlendMaskScrollX;
            MaterialProperty matcap4thBlendMaskScrollY;
            MaterialProperty matcap4thBlendMaskAngle;
            MaterialProperty matcap4thBlendMaskRotate;
            MaterialProperty matcap4thBlendMaskUV;
            MaterialProperty matcap4thBlendMaskTrim;
            MaterialProperty matcap4thNormalMix;
            MaterialProperty matcap4thShadeMix;
            MaterialProperty matcap4thMix;
        */
        MaterialProperty useRim;
            MaterialProperty rimColor;
            MaterialProperty rimColorTex;
            MaterialProperty rimColorTexScrollX;
            MaterialProperty rimColorTexScrollY;
            MaterialProperty rimColorTexAngle;
            MaterialProperty rimColorTexRotate;
            MaterialProperty rimColorTexUV;
            MaterialProperty rimColorTexTrim;
            MaterialProperty rimBlend;
            MaterialProperty rimBlendMask;
            MaterialProperty rimBlendMaskScrollX;
            MaterialProperty rimBlendMaskScrollY;
            MaterialProperty rimBlendMaskAngle;
            MaterialProperty rimBlendMaskRotate;
            MaterialProperty rimBlendMaskUV;
            MaterialProperty rimBlendMaskTrim;
            MaterialProperty rimToon;
            MaterialProperty rimBorder;
            MaterialProperty rimBorderMask;
            MaterialProperty rimBorderMaskScrollX;
            MaterialProperty rimBorderMaskScrollY;
            MaterialProperty rimBorderMaskAngle;
            MaterialProperty rimBorderMaskRotate;
            MaterialProperty rimBorderMaskUV;
            MaterialProperty rimBorderMaskTrim;
            MaterialProperty rimBlur;
            MaterialProperty rimBlurMask;
            MaterialProperty rimBlurMaskScrollX;
            MaterialProperty rimBlurMaskScrollY;
            MaterialProperty rimBlurMaskAngle;
            MaterialProperty rimBlurMaskRotate;
            MaterialProperty rimBlurMaskUV;
            MaterialProperty rimBlurMaskTrim;
            MaterialProperty rimUpperSideWidth;
            MaterialProperty rimFresnelPower;
            MaterialProperty rimShadeMix;
            MaterialProperty rimShadowMask;
        /* リムライト追加分
        MaterialProperty useRim2nd;
            MaterialProperty rim2ndColor;
            MaterialProperty rim2ndColorTex;
            MaterialProperty rim2ndColorTexScrollX;
            MaterialProperty rim2ndColorTexScrollY;
            MaterialProperty rim2ndColorTexAngle;
            MaterialProperty rim2ndColorTexRotate;
            MaterialProperty rim2ndColorTexUV;
            MaterialProperty rim2ndColorTexTrim;
            MaterialProperty rim2ndBlend;
            MaterialProperty rim2ndBlendMask;
            MaterialProperty rim2ndBlendMaskScrollX;
            MaterialProperty rim2ndBlendMaskScrollY;
            MaterialProperty rim2ndBlendMaskAngle;
            MaterialProperty rim2ndBlendMaskRotate;
            MaterialProperty rim2ndBlendMaskUV;
            MaterialProperty rim2ndBlendMaskTrim;
            MaterialProperty rim2ndToon;
            MaterialProperty rim2ndBorder;
            MaterialProperty rim2ndBorderMask;
            MaterialProperty rim2ndBorderMaskScrollX;
            MaterialProperty rim2ndBorderMaskScrollY;
            MaterialProperty rim2ndBorderMaskAngle;
            MaterialProperty rim2ndBorderMaskRotate;
            MaterialProperty rim2ndBorderMaskUV;
            MaterialProperty rim2ndBorderMaskTrim;
            MaterialProperty rim2ndBlur;
            MaterialProperty rim2ndBlurMask;
            MaterialProperty rim2ndBlurMaskScrollX;
            MaterialProperty rim2ndBlurMaskScrollY;
            MaterialProperty rim2ndBlurMaskAngle;
            MaterialProperty rim2ndBlurMaskRotate;
            MaterialProperty rim2ndBlurMaskUV;
            MaterialProperty rim2ndBlurMaskTrim;
            MaterialProperty rim2ndUpperSideWidth;
            MaterialProperty rim2ndFresnelPower;
            MaterialProperty rim2ndShadeMix;
            MaterialProperty rim2ndShadowMask;
        */
        MaterialProperty useEmission;
            MaterialProperty emissionColor;
            MaterialProperty emissionMap;
            MaterialProperty emissionMapScrollX;
            MaterialProperty emissionMapScrollY;
            MaterialProperty emissionMapAngle;
            MaterialProperty emissionMapRotate;
            MaterialProperty emissionMapUV;
            MaterialProperty emissionMapTrim;
            MaterialProperty emissionBlend;
            MaterialProperty emissionBlendMask;
            MaterialProperty emissionBlendMaskScrollX;
            MaterialProperty emissionBlendMaskScrollY;
            MaterialProperty emissionBlendMaskAngle;
            MaterialProperty emissionBlendMaskRotate;
            MaterialProperty emissionBlendMaskUV;
            MaterialProperty emissionBlendMaskTrim;
            MaterialProperty emissionHue;
            MaterialProperty emissionSaturation;
            MaterialProperty emissionValue;
            MaterialProperty emissionUseBlink;
            MaterialProperty emissionBlinkStrength;
            MaterialProperty emissionBlinkSpeed;
            MaterialProperty emissionBlinkOffset;
            MaterialProperty emissionBlinkType;
            MaterialProperty emissionUseGrad;
            MaterialProperty emissionGradTex;
            MaterialProperty emissionGradSpeed;
            MaterialProperty emissionParallaxDepth;
        MaterialProperty useEmission2nd;
            MaterialProperty emission2ndColor;
            MaterialProperty emission2ndMap;
            MaterialProperty emission2ndMapScrollX;
            MaterialProperty emission2ndMapScrollY;
            MaterialProperty emission2ndMapAngle;
            MaterialProperty emission2ndMapRotate;
            MaterialProperty emission2ndMapUV;
            MaterialProperty emission2ndMapTrim;
            MaterialProperty emission2ndBlend;
            MaterialProperty emission2ndBlendMask;
            MaterialProperty emission2ndBlendMaskScrollX;
            MaterialProperty emission2ndBlendMaskScrollY;
            MaterialProperty emission2ndBlendMaskAngle;
            MaterialProperty emission2ndBlendMaskRotate;
            MaterialProperty emission2ndBlendMaskUV;
            MaterialProperty emission2ndBlendMaskTrim;
            MaterialProperty emission2ndHue;
            MaterialProperty emission2ndSaturation;
            MaterialProperty emission2ndValue;
            MaterialProperty emission2ndUseBlink;
            MaterialProperty emission2ndBlinkStrength;
            MaterialProperty emission2ndBlinkSpeed;
            MaterialProperty emission2ndBlinkOffset;
            MaterialProperty emission2ndBlinkType;
            MaterialProperty emission2ndUseGrad;
            MaterialProperty emission2ndGradTex;
            MaterialProperty emission2ndGradSpeed;
            MaterialProperty emission2ndParallaxDepth;
        MaterialProperty useEmission3rd;
            MaterialProperty emission3rdColor;
            MaterialProperty emission3rdMap;
            MaterialProperty emission3rdMapScrollX;
            MaterialProperty emission3rdMapScrollY;
            MaterialProperty emission3rdMapAngle;
            MaterialProperty emission3rdMapRotate;
            MaterialProperty emission3rdMapUV;
            MaterialProperty emission3rdMapTrim;
            MaterialProperty emission3rdBlend;
            MaterialProperty emission3rdBlendMask;
            MaterialProperty emission3rdBlendMaskScrollX;
            MaterialProperty emission3rdBlendMaskScrollY;
            MaterialProperty emission3rdBlendMaskAngle;
            MaterialProperty emission3rdBlendMaskRotate;
            MaterialProperty emission3rdBlendMaskUV;
            MaterialProperty emission3rdBlendMaskTrim;
            MaterialProperty emission3rdHue;
            MaterialProperty emission3rdSaturation;
            MaterialProperty emission3rdValue;
            MaterialProperty emission3rdUseBlink;
            MaterialProperty emission3rdBlinkStrength;
            MaterialProperty emission3rdBlinkSpeed;
            MaterialProperty emission3rdBlinkOffset;
            MaterialProperty emission3rdBlinkType;
            MaterialProperty emission3rdUseGrad;
            MaterialProperty emission3rdGradTex;
            MaterialProperty emission3rdGradSpeed;
            MaterialProperty emission3rdParallaxDepth;
        MaterialProperty useEmission4th;
            MaterialProperty emission4thColor;
            MaterialProperty emission4thMap;
            MaterialProperty emission4thMapScrollX;
            MaterialProperty emission4thMapScrollY;
            MaterialProperty emission4thMapAngle;
            MaterialProperty emission4thMapRotate;
            MaterialProperty emission4thMapUV;
            MaterialProperty emission4thMapTrim;
            MaterialProperty emission4thBlend;
            MaterialProperty emission4thBlendMask;
            MaterialProperty emission4thBlendMaskScrollX;
            MaterialProperty emission4thBlendMaskScrollY;
            MaterialProperty emission4thBlendMaskAngle;
            MaterialProperty emission4thBlendMaskRotate;
            MaterialProperty emission4thBlendMaskUV;
            MaterialProperty emission4thBlendMaskTrim;
            MaterialProperty emission4thHue;
            MaterialProperty emission4thSaturation;
            MaterialProperty emission4thValue;
            MaterialProperty emission4thUseBlink;
            MaterialProperty emission4thBlinkStrength;
            MaterialProperty emission4thBlinkSpeed;
            MaterialProperty emission4thBlinkOffset;
            MaterialProperty emission4thBlinkType;
            MaterialProperty emission4thUseGrad;
            MaterialProperty emission4thGradTex;
            MaterialProperty emission4thGradSpeed;
            MaterialProperty emission4thParallaxDepth;
        #endregion
        Gradient emiGrad = new Gradient();
        Gradient emi2Grad = new Gradient();
        Gradient emi3Grad = new Gradient();
        Gradient emi4Grad = new Gradient();

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
	    {
            Material material = materialEditor.target as Material;
            bool isFull = material.shader.name.Contains("Full");
            bool isLite = material.shader.name.Contains("Lite");
            bool isStWr = material.shader.name.Contains("Writer");
            bool isRefr = material.shader.name.Contains("Refraction");
            bool isFur = material.shader.name.Contains("Fur");

            // プロパティ読み込み
            #region FindProperties
            if(isFull)
            {
                invisible = FindProperty("_Invisible", props);
                renderingMode = FindProperty("_Mode", props);
                cullMode = FindProperty("_CullMode", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                zwrite = FindProperty("_ZWrite", props);
                ztest = FindProperty("_ZTest", props);
                cutoff = FindProperty("_Cutoff", props);
                flipNormal = FindProperty("_FlipNormal", props);
                backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
                useVertexColor = FindProperty("_UseVertexColor", props);
                stencilRef = FindProperty("_StencilRef", props);
                stencilComp = FindProperty("_StencilComp", props);
                stencilPass = FindProperty("_StencilPass", props);
                stencilFail = FindProperty("_StencilFail", props);
                stencilZFail = FindProperty("_StencilZFail", props);
                // Main
                //useMainTex = FindProperty("_UseMainTex", props);
                    mainColor = FindProperty("_Color", props);
                    mainTex = FindProperty("_MainTex", props);
                    mainTexScrollX = FindProperty("_MainTexScrollX", props);
                    mainTexScrollY = FindProperty("_MainTexScrollY", props);
                    mainTexAngle = FindProperty("_MainTexAngle", props);
                    mainTexRotate = FindProperty("_MainTexRotate", props);
                    mainTexUV = FindProperty("_MainTexUV", props);
                    mainTexTrim = FindProperty("_MainTexTrim", props);
                    mainTexTonecurve = FindProperty("_MainTexTonecurve", props);
                    mainTexHue = FindProperty("_MainTexHue", props);
                    mainTexSaturation = FindProperty("_MainTexSaturation", props);
                    mainTexValue = FindProperty("_MainTexValue", props);
                useMain2ndTex = FindProperty("_UseMain2ndTex", props);
                    mainColor2nd = FindProperty("_Color2nd", props);
                    main2ndTex = FindProperty("_Main2ndTex", props);
                    main2ndTexScrollX = FindProperty("_Main2ndTexScrollX", props);
                    main2ndTexScrollY = FindProperty("_Main2ndTexScrollY", props);
                    main2ndTexAngle = FindProperty("_Main2ndTexAngle", props);
                    main2ndTexRotate = FindProperty("_Main2ndTexRotate", props);
                    main2ndTexUV = FindProperty("_Main2ndTexUV", props);
                    main2ndTexTrim = FindProperty("_Main2ndTexTrim", props);
                    main2ndBlend = FindProperty("_Main2ndBlend", props);
                    main2ndBlendMask = FindProperty("_Main2ndBlendMask", props);
                    main2ndBlendMaskScrollX = FindProperty("_Main2ndBlendMaskScrollX", props);
                    main2ndBlendMaskScrollY = FindProperty("_Main2ndBlendMaskScrollY", props);
                    main2ndBlendMaskAngle = FindProperty("_Main2ndBlendMaskAngle", props);
                    main2ndBlendMaskRotate = FindProperty("_Main2ndBlendMaskRotate", props);
                    main2ndBlendMaskUV = FindProperty("_Main2ndBlendMaskUV", props);
                    main2ndBlendMaskTrim = FindProperty("_Main2ndBlendMaskTrim", props);
                    main2ndTexMix = FindProperty("_Main2ndTexMix", props);
                    main2ndTexHue = FindProperty("_Main2ndTexHue", props);
                    main2ndTexSaturation = FindProperty("_Main2ndTexSaturation", props);
                    main2ndTexValue = FindProperty("_Main2ndTexValue", props);
                useMain3rdTex = FindProperty("_UseMain3rdTex", props);
                    mainColor3rd = FindProperty("_Color3rd", props);
                    main3rdTex = FindProperty("_Main3rdTex", props);
                    main3rdTexScrollX = FindProperty("_Main3rdTexScrollX", props);
                    main3rdTexScrollY = FindProperty("_Main3rdTexScrollY", props);
                    main3rdTexAngle = FindProperty("_Main3rdTexAngle", props);
                    main3rdTexRotate = FindProperty("_Main3rdTexRotate", props);
                    main3rdTexUV = FindProperty("_Main3rdTexUV", props);
                    main3rdTexTrim = FindProperty("_Main3rdTexTrim", props);
                    main3rdBlend = FindProperty("_Main3rdBlend", props);
                    main3rdBlendMask = FindProperty("_Main3rdBlendMask", props);
                    main3rdBlendMaskScrollX = FindProperty("_Main3rdBlendMaskScrollX", props);
                    main3rdBlendMaskScrollY = FindProperty("_Main3rdBlendMaskScrollY", props);
                    main3rdBlendMaskAngle = FindProperty("_Main3rdBlendMaskAngle", props);
                    main3rdBlendMaskRotate = FindProperty("_Main3rdBlendMaskRotate", props);
                    main3rdBlendMaskUV = FindProperty("_Main3rdBlendMaskUV", props);
                    main3rdBlendMaskTrim = FindProperty("_Main3rdBlendMaskTrim", props);
                    main3rdTexMix = FindProperty("_Main3rdTexMix", props);
                    main3rdTexHue = FindProperty("_Main3rdTexHue", props);
                    main3rdTexSaturation = FindProperty("_Main3rdTexSaturation", props);
                    main3rdTexValue = FindProperty("_Main3rdTexValue", props);
                useMain4thTex = FindProperty("_UseMain4thTex", props);
                    mainColor4th = FindProperty("_Color4th", props);
                    main4thTex = FindProperty("_Main4thTex", props);
                    main4thTexScrollX = FindProperty("_Main4thTexScrollX", props);
                    main4thTexScrollY = FindProperty("_Main4thTexScrollY", props);
                    main4thTexAngle = FindProperty("_Main4thTexAngle", props);
                    main4thTexRotate = FindProperty("_Main4thTexRotate", props);
                    main4thTexUV = FindProperty("_Main4thTexUV", props);
                    main4thTexTrim = FindProperty("_Main4thTexTrim", props);
                    main4thBlend = FindProperty("_Main4thBlend", props);
                    main4thBlendMask = FindProperty("_Main4thBlendMask", props);
                    main4thBlendMaskScrollX = FindProperty("_Main4thBlendMaskScrollX", props);
                    main4thBlendMaskScrollY = FindProperty("_Main4thBlendMaskScrollY", props);
                    main4thBlendMaskAngle = FindProperty("_Main4thBlendMaskAngle", props);
                    main4thBlendMaskRotate = FindProperty("_Main4thBlendMaskRotate", props);
                    main4thBlendMaskUV = FindProperty("_Main4thBlendMaskUV", props);
                    main4thBlendMaskTrim = FindProperty("_Main4thBlendMaskTrim", props);
                    main4thTexMix = FindProperty("_Main4thTexMix", props);
                    main4thTexHue = FindProperty("_Main4thTexHue", props);
                    main4thTexSaturation = FindProperty("_Main4thTexSaturation", props);
                    main4thTexValue = FindProperty("_Main4thTexValue", props);
                // Alpha
                useAlphaMask = FindProperty("_UseAlphaMask", props);
                    alpha = FindProperty("_Alpha", props);
                    alphaMask = FindProperty("_AlphaMask", props);
                    alphaMaskScrollX = FindProperty("_AlphaMaskScrollX", props);
                    alphaMaskScrollY = FindProperty("_AlphaMaskScrollY", props);
                    alphaMaskAngle = FindProperty("_AlphaMaskAngle", props);
                    alphaMaskRotate = FindProperty("_AlphaMaskRotate", props);
                    alphaMaskUV = FindProperty("_AlphaMaskUV", props);
                    alphaMaskTrim = FindProperty("_AlphaMaskTrim", props);
                    alphaMaskMixMain = FindProperty("_AlphaMaskMixMain", props);
                /* アルファマスク追加分
                useAlphaMask2nd = FindProperty("_UseAlphaMask2nd", props);
                    alpha2nd = FindProperty("_Alpha2nd", props);
                    alphaMask2nd = FindProperty("_AlphaMask2nd", props);
                    alphaMask2ndScrollX = FindProperty("_AlphaMask2ndScrollX", props);
                    alphaMask2ndScrollY = FindProperty("_AlphaMask2ndScrollY", props);
                    alphaMask2ndAngle = FindProperty("_AlphaMask2ndAngle", props);
                    alphaMask2ndRotate = FindProperty("_AlphaMask2ndRotate", props);
                    alphaMask2ndUV = FindProperty("_AlphaMask2ndUV", props);
                    alphaMask2ndTrim = FindProperty("_AlphaMask2ndTrim", props);
                    alphaMask2ndMixMain = FindProperty("_AlphaMask2ndMixMain", props);
                */
                // Shadow
                useShadow = FindProperty("_UseShadow", props);
                    shadowBorder = FindProperty("_ShadowBorder", props);
                    shadowBorderMask = FindProperty("_ShadowBorderMask", props);
                    shadowBorderMaskScrollX = FindProperty("_ShadowBorderMaskScrollX", props);
                    shadowBorderMaskScrollY = FindProperty("_ShadowBorderMaskScrollY", props);
                    shadowBorderMaskAngle = FindProperty("_ShadowBorderMaskAngle", props);
                    shadowBorderMaskRotate = FindProperty("_ShadowBorderMaskRotate", props);
                    shadowBorderMaskUV = FindProperty("_ShadowBorderMaskUV", props);
                    shadowBorderMaskTrim = FindProperty("_ShadowBorderMaskTrim", props);
                    shadowBlur = FindProperty("_ShadowBlur", props);
                    shadowBlurMask = FindProperty("_ShadowBlurMask", props);
                    shadowBlurMaskScrollX = FindProperty("_ShadowBlurMaskScrollX", props);
                    shadowBlurMaskScrollY = FindProperty("_ShadowBlurMaskScrollY", props);
                    shadowBlurMaskAngle = FindProperty("_ShadowBlurMaskAngle", props);
                    shadowBlurMaskRotate = FindProperty("_ShadowBlurMaskRotate", props);
                    shadowBlurMaskUV = FindProperty("_ShadowBlurMaskUV", props);
                    shadowBlurMaskTrim = FindProperty("_ShadowBlurMaskTrim", props);
                    shadowStrength = FindProperty("_ShadowStrength", props);
                    shadowStrengthMask = FindProperty("_ShadowStrengthMask", props);
                    shadowStrengthMaskScrollX = FindProperty("_ShadowStrengthMaskScrollX", props);
                    shadowStrengthMaskScrollY = FindProperty("_ShadowStrengthMaskScrollY", props);
                    shadowStrengthMaskAngle = FindProperty("_ShadowStrengthMaskAngle", props);
                    shadowStrengthMaskRotate = FindProperty("_ShadowStrengthMaskRotate", props);
                    shadowStrengthMaskUV = FindProperty("_ShadowStrengthMaskUV", props);
                    shadowStrengthMaskTrim = FindProperty("_ShadowStrengthMaskTrim", props);
                    useShadowMixMainColor = FindProperty("_UseShadowMixMainColor", props);
                    shadowMixMainColor = FindProperty("_ShadowMixMainColor", props);
                    shadowHue = FindProperty("_ShadowHue", props);
                    shadowSaturation = FindProperty("_ShadowSaturation", props);
                    shadowValue = FindProperty("_ShadowValue", props);
                    shadowGrad = FindProperty("_ShadowGrad", props);
                    shadowGradColor = FindProperty("_ShadowGradColor", props);
                    useShadowColor = FindProperty("_UseShadowColor", props);
                    shadowColorFromMain = FindProperty("_ShadowColorFromMain", props);
                    shadowColor = FindProperty("_ShadowColor", props);
                    shadowColorTex = FindProperty("_ShadowColorTex", props);
                    shadowColorTexScrollX = FindProperty("_ShadowColorTexScrollX", props);
                    shadowColorTexScrollY = FindProperty("_ShadowColorTexScrollY", props);
                    shadowColorTexAngle = FindProperty("_ShadowColorTexAngle", props);
                    shadowColorTexRotate = FindProperty("_ShadowColorTexRotate", props);
                    shadowColorTexUV = FindProperty("_ShadowColorTexUV", props);
                    shadowColorTexTrim = FindProperty("_ShadowColorTexTrim", props);
                    shadowColorMix = FindProperty("_ShadowColorMix", props);
                useShadow2nd = FindProperty("_UseShadow2nd", props);
                    shadow2ndBorder = FindProperty("_Shadow2ndBorder", props);
                    shadow2ndBlur = FindProperty("_Shadow2ndBlur", props);
                    shadow2ndColorFromMain = FindProperty("_Shadow2ndColorFromMain", props);
                    shadow2ndColor = FindProperty("_Shadow2ndColor", props);
                    shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props);
                    shadow2ndColorTexScrollX = FindProperty("_Shadow2ndColorTexScrollX", props);
                    shadow2ndColorTexScrollY = FindProperty("_Shadow2ndColorTexScrollY", props);
                    shadow2ndColorTexAngle = FindProperty("_Shadow2ndColorTexAngle", props);
                    shadow2ndColorTexRotate = FindProperty("_Shadow2ndColorTexRotate", props);
                    shadow2ndColorTexUV = FindProperty("_Shadow2ndColorTexUV", props);
                    shadow2ndColorTexTrim = FindProperty("_Shadow2ndColorTexTrim", props);
                    shadow2ndColorMix = FindProperty("_Shadow2ndColorMix", props);
                useDefaultShading = FindProperty("_UseDefaultShading", props);
                    defaultShadingBlend = FindProperty("_DefaultShadingBlend", props);
                    defaultShadingBlendMask = FindProperty("_DefaultShadingBlendMask", props);
                    defaultShadingBlendMaskScrollX = FindProperty("_DefaultShadingBlendMaskScrollX", props);
                    defaultShadingBlendMaskScrollY = FindProperty("_DefaultShadingBlendMaskScrollY", props);
                    defaultShadingBlendMaskAngle = FindProperty("_DefaultShadingBlendMaskAngle", props);
                    defaultShadingBlendMaskRotate = FindProperty("_DefaultShadingBlendMaskRotate", props);
                    defaultShadingBlendMaskUV = FindProperty("_DefaultShadingBlendMaskUV", props);
                    defaultShadingBlendMaskTrim = FindProperty("_DefaultShadingBlendMaskTrim", props);
                // Outline
                useOutline = FindProperty("_UseOutline", props);
                    outlineMixMainStrength = FindProperty("_OutlineMixMainStrength", props);
                    outlineHue = FindProperty("_OutlineHue", props);
                    outlineSaturation = FindProperty("_OutlineSaturation", props);
                    outlineValue = FindProperty("_OutlineValue", props);
                    outlineAutoHue = FindProperty("_OutlineAutoHue", props);
                    outlineAutoValue = FindProperty("_OutlineAutoValue", props);
                    useOutlineColor = FindProperty("_UseOutlineColor", props);
                    outlineColor = FindProperty("_OutlineColor", props);
                    outlineColorTex = FindProperty("_OutlineColorTex", props);
                    outlineColorTexScrollX = FindProperty("_OutlineColorTexScrollX", props);
                    outlineColorTexScrollY = FindProperty("_OutlineColorTexScrollY", props);
                    outlineColorTexAngle = FindProperty("_OutlineColorTexAngle", props);
                    outlineColorTexRotate = FindProperty("_OutlineColorTexRotate", props);
                    outlineColorTexUV = FindProperty("_OutlineColorTexUV", props);
                    outlineColorTexTrim = FindProperty("_OutlineColorTexTrim", props);
                    outlineWidth = FindProperty("_OutlineWidth", props);
                    outlineWidthMask = FindProperty("_OutlineWidthMask", props);
                    outlineWidthMaskScrollX = FindProperty("_OutlineWidthMaskScrollX", props);
                    outlineWidthMaskScrollY = FindProperty("_OutlineWidthMaskScrollY", props);
                    outlineWidthMaskAngle = FindProperty("_OutlineWidthMaskAngle", props);
                    outlineWidthMaskRotate = FindProperty("_OutlineWidthMaskRotate", props);
                    outlineWidthMaskUV = FindProperty("_OutlineWidthMaskUV", props);
                    outlineWidthMaskTrim = FindProperty("_OutlineWidthMaskTrim", props);
                    outlineAlpha = FindProperty("_OutlineAlpha", props);
                    outlineAlphaMask = FindProperty("_OutlineAlphaMask", props);
                    outlineAlphaMaskScrollX = FindProperty("_OutlineAlphaMaskScrollX", props);
                    outlineAlphaMaskScrollY = FindProperty("_OutlineAlphaMaskScrollY", props);
                    outlineAlphaMaskAngle = FindProperty("_OutlineAlphaMaskAngle", props);
                    outlineAlphaMaskRotate = FindProperty("_OutlineAlphaMaskRotate", props);
                    outlineAlphaMaskUV = FindProperty("_OutlineAlphaMaskUV", props);
                    outlineAlphaMaskTrim = FindProperty("_OutlineAlphaMaskTrim", props);
                    vertexColor2Width = FindProperty("_VertexColor2Width", props);
                // Normal
                useBumpMap = FindProperty("_UseBumpMap", props);
                    bumpMap = FindProperty("_BumpMap", props);
                    bumpMapScrollX = FindProperty("_BumpMapScrollX", props);
                    bumpMapScrollY = FindProperty("_BumpMapScrollY", props);
                    bumpMapAngle = FindProperty("_BumpMapAngle", props);
                    bumpMapRotate = FindProperty("_BumpMapRotate", props);
                    bumpMapUV = FindProperty("_BumpMapUV", props);
                    bumpMapTrim = FindProperty("_BumpMapTrim", props);
                    bumpScale = FindProperty("_BumpScale", props);
                    bumpScaleMask = FindProperty("_BumpScaleMask", props);
                    bumpScaleMaskScrollX = FindProperty("_BumpScaleMaskScrollX", props);
                    bumpScaleMaskScrollY = FindProperty("_BumpScaleMaskScrollY", props);
                    bumpScaleMaskAngle = FindProperty("_BumpScaleMaskAngle", props);
                    bumpScaleMaskRotate = FindProperty("_BumpScaleMaskRotate", props);
                    bumpScaleMaskUV = FindProperty("_BumpScaleMaskUV", props);
                    bumpScaleMaskTrim = FindProperty("_BumpScaleMaskTrim", props);
                useBump2ndMap = FindProperty("_UseBump2ndMap", props);
                    bump2ndMap = FindProperty("_Bump2ndMap", props);
                    bump2ndMapScrollX = FindProperty("_Bump2ndMapScrollX", props);
                    bump2ndMapScrollY = FindProperty("_Bump2ndMapScrollY", props);
                    bump2ndMapAngle = FindProperty("_Bump2ndMapAngle", props);
                    bump2ndMapRotate = FindProperty("_Bump2ndMapRotate", props);
                    bump2ndMapUV = FindProperty("_Bump2ndMapUV", props);
                    bump2ndMapTrim = FindProperty("_Bump2ndMapTrim", props);
                    bump2ndScale = FindProperty("_Bump2ndScale", props);
                    bump2ndScaleMask = FindProperty("_Bump2ndScaleMask", props);
                    bump2ndScaleMaskScrollX = FindProperty("_Bump2ndScaleMaskScrollX", props);
                    bump2ndScaleMaskScrollY = FindProperty("_Bump2ndScaleMaskScrollY", props);
                    bump2ndScaleMaskAngle = FindProperty("_Bump2ndScaleMaskAngle", props);
                    bump2ndScaleMaskRotate = FindProperty("_Bump2ndScaleMaskRotate", props);
                    bump2ndScaleMaskUV = FindProperty("_Bump2ndScaleMaskUV", props);
                    bump2ndScaleMaskTrim = FindProperty("_Bump2ndScaleMaskTrim", props);
                /* ノーマルマップ追加分
                useBump3rdMap = FindProperty("_UseBump3rdMap", props);
                    bump3rdMap = FindProperty("_Bump3rdMap", props);
                    bump3rdMapScrollX = FindProperty("_Bump3rdMapScrollX", props);
                    bump3rdMapScrollY = FindProperty("_Bump3rdMapScrollY", props);
                    bump3rdMapAngle = FindProperty("_Bump3rdMapAngle", props);
                    bump3rdMapRotate = FindProperty("_Bump3rdMapRotate", props);
                    bump3rdMapUV = FindProperty("_Bump3rdMapUV", props);
                    bump3rdMapTrim = FindProperty("_Bump3rdMapTrim", props);
                    bump3rdScale = FindProperty("_Bump3rdScale", props);
                    bump3rdScaleMask = FindProperty("_Bump3rdScaleMask", props);
                    bump3rdScaleMaskScrollX = FindProperty("_Bump3rdScaleMaskScrollX", props);
                    bump3rdScaleMaskScrollY = FindProperty("_Bump3rdScaleMaskScrollY", props);
                    bump3rdScaleMaskAngle = FindProperty("_Bump3rdScaleMaskAngle", props);
                    bump3rdScaleMaskRotate = FindProperty("_Bump3rdScaleMaskRotate", props);
                    bump3rdScaleMaskUV = FindProperty("_Bump3rdScaleMaskUV", props);
                    bump3rdScaleMaskTrim = FindProperty("_Bump3rdScaleMaskTrim", props);
                useBump4thMap = FindProperty("_UseBump4thMap", props);
                    bump4thMap = FindProperty("_Bump4thMap", props);
                    bump4thMapScrollX = FindProperty("_Bump4thMapScrollX", props);
                    bump4thMapScrollY = FindProperty("_Bump4thMapScrollY", props);
                    bump4thMapAngle = FindProperty("_Bump4thMapAngle", props);
                    bump4thMapRotate = FindProperty("_Bump4thMapRotate", props);
                    bump4thMapUV = FindProperty("_Bump4thMapUV", props);
                    bump4thMapTrim = FindProperty("_Bump4thMapTrim", props);
                    bump4thScale = FindProperty("_Bump4thScale", props);
                    bump4thScaleMask = FindProperty("_Bump4thScaleMask", props);
                    bump4thScaleMaskScrollX = FindProperty("_Bump4thScaleMaskScrollX", props);
                    bump4thScaleMaskScrollY = FindProperty("_Bump4thScaleMaskScrollY", props);
                    bump4thScaleMaskAngle = FindProperty("_Bump4thScaleMaskAngle", props);
                    bump4thScaleMaskRotate = FindProperty("_Bump4thScaleMaskRotate", props);
                    bump4thScaleMaskUV = FindProperty("_Bump4thScaleMaskUV", props);
                    bump4thScaleMaskTrim = FindProperty("_Bump4thScaleMaskTrim", props);
                */
                useReflection = FindProperty("_UseReflection", props);
                    smoothness = FindProperty("_Smoothness", props);
                    smoothnessTex = FindProperty("_SmoothnessTex", props);
                    smoothnessTexScrollX = FindProperty("_SmoothnessTexScrollX", props);
                    smoothnessTexScrollY = FindProperty("_SmoothnessTexScrollY", props);
                    smoothnessTexAngle = FindProperty("_SmoothnessTexAngle", props);
                    smoothnessTexRotate = FindProperty("_SmoothnessTexRotate", props);
                    smoothnessTexUV = FindProperty("_SmoothnessTexUV", props);
                    smoothnessTexTrim = FindProperty("_SmoothnessTexTrim", props);
                    metallic = FindProperty("_Metallic", props);
                    metallicGlossMap = FindProperty("_MetallicGlossMap", props);
                    metallicGlossMapScrollX = FindProperty("_MetallicGlossMapScrollX", props);
                    metallicGlossMapScrollY = FindProperty("_MetallicGlossMapScrollY", props);
                    metallicGlossMapAngle = FindProperty("_MetallicGlossMapAngle", props);
                    metallicGlossMapRotate = FindProperty("_MetallicGlossMapRotate", props);
                    metallicGlossMapUV = FindProperty("_MetallicGlossMapUV", props);
                    metallicGlossMapTrim = FindProperty("_MetallicGlossMapTrim", props);
                    reflectionBlend = FindProperty("_ReflectionBlend", props);
                    reflectionBlendMask = FindProperty("_ReflectionBlendMask", props);
                    reflectionBlendMaskScrollX = FindProperty("_ReflectionBlendMaskScrollX", props);
                    reflectionBlendMaskScrollY = FindProperty("_ReflectionBlendMaskScrollY", props);
                    reflectionBlendMaskAngle = FindProperty("_ReflectionBlendMaskAngle", props);
                    reflectionBlendMaskRotate = FindProperty("_ReflectionBlendMaskRotate", props);
                    reflectionBlendMaskUV = FindProperty("_ReflectionBlendMaskUV", props);
                    reflectionBlendMaskTrim = FindProperty("_ReflectionBlendMaskTrim", props);
                    applySpecular = FindProperty("_ApplySpecular", props);
                    applyReflection = FindProperty("_ApplyReflection", props);
                    reflectionUseCubemap = FindProperty("_ReflectionUseCubemap", props);
                    reflectionCubemap = FindProperty("_ReflectionCubemap", props);
                    reflectionShadeMix = FindProperty("_ReflectionShadeMix", props);
                useMatcap = FindProperty("_UseMatcap", props);
                    matcapTex = FindProperty("_MatcapTex", props);
                    matcapColor = FindProperty("_MatcapColor", props);
                    matcapBlend = FindProperty("_MatcapBlend", props);
                    matcapBlendMask = FindProperty("_MatcapBlendMask", props);
                    matcapBlendMaskScrollX = FindProperty("_MatcapBlendMaskScrollX", props);
                    matcapBlendMaskScrollY = FindProperty("_MatcapBlendMaskScrollY", props);
                    matcapBlendMaskAngle = FindProperty("_MatcapBlendMaskAngle", props);
                    matcapBlendMaskRotate = FindProperty("_MatcapBlendMaskRotate", props);
                    matcapBlendMaskUV = FindProperty("_MatcapBlendMaskUV", props);
                    matcapBlendMaskTrim = FindProperty("_MatcapBlendMaskTrim", props);
                    matcapNormalMix = FindProperty("_MatcapNormalMix", props);
                    matcapShadeMix = FindProperty("_MatcapShadeMix", props);
                    matcapMix = FindProperty("_MatcapMix", props);
                useMatcap2nd = FindProperty("_UseMatcap2nd", props);
                    matcap2ndTex = FindProperty("_Matcap2ndTex", props);
                    matcap2ndColor = FindProperty("_Matcap2ndColor", props);
                    matcap2ndBlend = FindProperty("_Matcap2ndBlend", props);
                    matcap2ndBlendMask = FindProperty("_Matcap2ndBlendMask", props);
                    matcap2ndBlendMaskScrollX = FindProperty("_Matcap2ndBlendMaskScrollX", props);
                    matcap2ndBlendMaskScrollY = FindProperty("_Matcap2ndBlendMaskScrollY", props);
                    matcap2ndBlendMaskAngle = FindProperty("_Matcap2ndBlendMaskAngle", props);
                    matcap2ndBlendMaskRotate = FindProperty("_Matcap2ndBlendMaskRotate", props);
                    matcap2ndBlendMaskUV = FindProperty("_Matcap2ndBlendMaskUV", props);
                    matcap2ndBlendMaskTrim = FindProperty("_Matcap2ndBlendMaskTrim", props);
                    matcap2ndNormalMix = FindProperty("_Matcap2ndNormalMix", props);
                    matcap2ndShadeMix = FindProperty("_Matcap2ndShadeMix", props);
                    matcap2ndMix = FindProperty("_Matcap2ndMix", props);
                /* マットキャップ追加分
                useMatcap3rd = FindProperty("_UseMatcap3rd", props);
                    matcap3rdTex = FindProperty("_Matcap3rdTex", props);
                    matcap3rdColor = FindProperty("_Matcap3rdColor", props);
                    matcap3rdBlend = FindProperty("_Matcap3rdBlend", props);
                    matcap3rdBlendMask = FindProperty("_Matcap3rdBlendMask", props);
                    matcap3rdBlendMaskScrollX = FindProperty("_Matcap3rdBlendMaskScrollX", props);
                    matcap3rdBlendMaskScrollY = FindProperty("_Matcap3rdBlendMaskScrollY", props);
                    matcap3rdBlendMaskAngle = FindProperty("_Matcap3rdBlendMaskAngle", props);
                    matcap3rdBlendMaskRotate = FindProperty("_Matcap3rdBlendMaskRotate", props);
                    matcap3rdBlendMaskUV = FindProperty("_Matcap3rdBlendMaskUV", props);
                    matcap3rdBlendMaskTrim = FindProperty("_Matcap3rdBlendMaskTrim", props);
                    matcap3rdNormalMix = FindProperty("_Matcap3rdNormalMix", props);
                    matcap3rdShadeMix = FindProperty("_Matcap3rdShadeMix", props);
                    matcap3rdMix = FindProperty("_Matcap3rdMix", props);
                useMatcap4th = FindProperty("_UseMatcap4th", props);
                    matcap4thTex = FindProperty("_Matcap4thTex", props);
                    matcap4thColor = FindProperty("_Matcap4thColor", props);
                    matcap4thBlend = FindProperty("_Matcap4thBlend", props);
                    matcap4thBlendMask = FindProperty("_Matcap4thBlendMask", props);
                    matcap4thBlendMaskScrollX = FindProperty("_Matcap4thBlendMaskScrollX", props);
                    matcap4thBlendMaskScrollY = FindProperty("_Matcap4thBlendMaskScrollY", props);
                    matcap4thBlendMaskAngle = FindProperty("_Matcap4thBlendMaskAngle", props);
                    matcap4thBlendMaskRotate = FindProperty("_Matcap4thBlendMaskRotate", props);
                    matcap4thBlendMaskUV = FindProperty("_Matcap4thBlendMaskUV", props);
                    matcap4thBlendMaskTrim = FindProperty("_Matcap4thBlendMaskTrim", props);
                    matcap4thNormalMix = FindProperty("_Matcap4thNormalMix", props);
                    matcap4thShadeMix = FindProperty("_Matcap4thShadeMix", props);
                    matcap4thMix = FindProperty("_Matcap4thMix", props);
                */
                useRim = FindProperty("_UseRim", props);
                    rimColor = FindProperty("_RimColor", props);
                    rimColorTex = FindProperty("_RimColorTex", props);
                    rimColorTexScrollX = FindProperty("_RimColorTexScrollX", props);
                    rimColorTexScrollY = FindProperty("_RimColorTexScrollY", props);
                    rimColorTexAngle = FindProperty("_RimColorTexAngle", props);
                    rimColorTexRotate = FindProperty("_RimColorTexRotate", props);
                    rimColorTexUV = FindProperty("_RimColorTexUV", props);
                    rimColorTexTrim = FindProperty("_RimColorTexTrim", props);
                    rimBlend = FindProperty("_RimBlend", props);
                    rimBlendMask = FindProperty("_RimBlendMask", props);
                    rimBlendMaskScrollX = FindProperty("_RimBlendMaskScrollX", props);
                    rimBlendMaskScrollY = FindProperty("_RimBlendMaskScrollY", props);
                    rimBlendMaskAngle = FindProperty("_RimBlendMaskAngle", props);
                    rimBlendMaskRotate = FindProperty("_RimBlendMaskRotate", props);
                    rimBlendMaskUV = FindProperty("_RimBlendMaskUV", props);
                    rimBlendMaskTrim = FindProperty("_RimBlendMaskTrim", props);
                    rimToon = FindProperty("_RimToon", props);
                    rimBorder = FindProperty("_RimBorder", props);
                    rimBorderMask = FindProperty("_RimBorderMask", props);
                    rimBorderMaskScrollX = FindProperty("_RimBorderMaskScrollX", props);
                    rimBorderMaskScrollY = FindProperty("_RimBorderMaskScrollY", props);
                    rimBorderMaskAngle = FindProperty("_RimBorderMaskAngle", props);
                    rimBorderMaskRotate = FindProperty("_RimBorderMaskRotate", props);
                    rimBorderMaskUV = FindProperty("_RimBorderMaskUV", props);
                    rimBorderMaskTrim = FindProperty("_RimBorderMaskTrim", props);
                    rimBlur = FindProperty("_RimBlur", props);
                    rimBlurMask = FindProperty("_RimBlurMask", props);
                    rimBlurMaskScrollX = FindProperty("_RimBlurMaskScrollX", props);
                    rimBlurMaskScrollY = FindProperty("_RimBlurMaskScrollY", props);
                    rimBlurMaskAngle = FindProperty("_RimBlurMaskAngle", props);
                    rimBlurMaskRotate = FindProperty("_RimBlurMaskRotate", props);
                    rimBlurMaskUV = FindProperty("_RimBlurMaskUV", props);
                    rimBlurMaskTrim = FindProperty("_RimBlurMaskTrim", props);
                    rimUpperSideWidth = FindProperty("_RimUpperSideWidth", props);
                    rimFresnelPower = FindProperty("_RimFresnelPower", props);
                    rimShadeMix = FindProperty("_RimShadeMix", props);
                    rimShadowMask = FindProperty("_RimShadowMask", props);
                /* リムライト追加分
                useRim2nd = FindProperty("_UseRim2nd", props);
                    rim2ndColor = FindProperty("_Rim2ndColor", props);
                    rim2ndColorTex = FindProperty("_Rim2ndColorTex", props);
                    rim2ndColorTexScrollX = FindProperty("_Rim2ndColorTexScrollX", props);
                    rim2ndColorTexScrollY = FindProperty("_Rim2ndColorTexScrollY", props);
                    rim2ndColorTexAngle = FindProperty("_Rim2ndColorTexAngle", props);
                    rim2ndColorTexRotate = FindProperty("_Rim2ndColorTexRotate", props);
                    rim2ndColorTexUV = FindProperty("_Rim2ndColorTexUV", props);
                    rim2ndColorTexTrim = FindProperty("_Rim2ndColorTexTrim", props);
                    rim2ndBlend = FindProperty("_Rim2ndBlend", props);
                    rim2ndBlendMask = FindProperty("_Rim2ndBlendMask", props);
                    rim2ndBlendMaskScrollX = FindProperty("_Rim2ndBlendMaskScrollX", props);
                    rim2ndBlendMaskScrollY = FindProperty("_Rim2ndBlendMaskScrollY", props);
                    rim2ndBlendMaskAngle = FindProperty("_Rim2ndBlendMaskAngle", props);
                    rim2ndBlendMaskRotate = FindProperty("_Rim2ndBlendMaskRotate", props);
                    rim2ndBlendMaskUV = FindProperty("_Rim2ndBlendMaskUV", props);
                    rim2ndBlendMaskTrim = FindProperty("_Rim2ndBlendMaskTrim", props);
                    rim2ndToon = FindProperty("_Rim2ndToon", props);
                    rim2ndBorder = FindProperty("_Rim2ndBorder", props);
                    rim2ndBorderMask = FindProperty("_Rim2ndBorderMask", props);
                    rim2ndBorderMaskScrollX = FindProperty("_Rim2ndBorderMaskScrollX", props);
                    rim2ndBorderMaskScrollY = FindProperty("_Rim2ndBorderMaskScrollY", props);
                    rim2ndBorderMaskAngle = FindProperty("_Rim2ndBorderMaskAngle", props);
                    rim2ndBorderMaskRotate = FindProperty("_Rim2ndBorderMaskRotate", props);
                    rim2ndBorderMaskUV = FindProperty("_Rim2ndBorderMaskUV", props);
                    rim2ndBorderMaskTrim = FindProperty("_Rim2ndBorderMaskTrim", props);
                    rim2ndBlur = FindProperty("_Rim2ndBlur", props);
                    rim2ndBlurMask = FindProperty("_Rim2ndBlurMask", props);
                    rim2ndBlurMaskScrollX = FindProperty("_Rim2ndBlurMaskScrollX", props);
                    rim2ndBlurMaskScrollY = FindProperty("_Rim2ndBlurMaskScrollY", props);
                    rim2ndBlurMaskAngle = FindProperty("_Rim2ndBlurMaskAngle", props);
                    rim2ndBlurMaskRotate = FindProperty("_Rim2ndBlurMaskRotate", props);
                    rim2ndBlurMaskUV = FindProperty("_Rim2ndBlurMaskUV", props);
                    rim2ndBlurMaskTrim = FindProperty("_Rim2ndBlurMaskTrim", props);
                    rim2ndUpperSideWidth = FindProperty("_Rim2ndUpperSideWidth", props);
                    rim2ndFresnelPower = FindProperty("_Rim2ndFresnelPower", props);
                    rim2ndShadeMix = FindProperty("_Rim2ndShadeMix", props);
                    rim2ndShadowMask = FindProperty("_Rim2ndShadowMask", props);
                */
                useEmission = FindProperty("_UseEmission", props);
                    emissionColor = FindProperty("_EmissionColor", props);
                    emissionMap = FindProperty("_EmissionMap", props);
                    emissionMapScrollX = FindProperty("_EmissionMapScrollX", props);
                    emissionMapScrollY = FindProperty("_EmissionMapScrollY", props);
                    emissionMapAngle = FindProperty("_EmissionMapAngle", props);
                    emissionMapRotate = FindProperty("_EmissionMapRotate", props);
                    emissionMapUV = FindProperty("_EmissionMapUV", props);
                    emissionMapTrim = FindProperty("_EmissionMapTrim", props);
                    emissionBlend = FindProperty("_EmissionBlend", props);
                    emissionBlendMask = FindProperty("_EmissionBlendMask", props);
                    emissionBlendMaskScrollX = FindProperty("_EmissionBlendMaskScrollX", props);
                    emissionBlendMaskScrollY = FindProperty("_EmissionBlendMaskScrollY", props);
                    emissionBlendMaskAngle = FindProperty("_EmissionBlendMaskAngle", props);
                    emissionBlendMaskRotate = FindProperty("_EmissionBlendMaskRotate", props);
                    emissionBlendMaskUV = FindProperty("_EmissionBlendMaskUV", props);
                    emissionBlendMaskTrim = FindProperty("_EmissionBlendMaskTrim", props);
                    emissionHue = FindProperty("_EmissionHue", props);
                    emissionSaturation = FindProperty("_EmissionSaturation", props);
                    emissionValue = FindProperty("_EmissionValue", props);
                    emissionUseBlink = FindProperty("_EmissionUseBlink", props);
                    emissionBlinkStrength = FindProperty("_EmissionBlinkStrength", props);
                    emissionBlinkSpeed = FindProperty("_EmissionBlinkSpeed", props);
                    emissionBlinkOffset = FindProperty("_EmissionBlinkOffset", props);
                    emissionBlinkType = FindProperty("_EmissionBlinkType", props);
                    emissionUseGrad = FindProperty("_EmissionUseGrad", props);
                    emissionGradTex = FindProperty("_EmissionGradTex", props);
                    emissionGradSpeed = FindProperty("_EmissionGradSpeed", props);
                    emissionParallaxDepth = FindProperty("_EmissionParallaxDepth", props);
                useEmission2nd = FindProperty("_UseEmission2nd", props);
                    emission2ndColor = FindProperty("_Emission2ndColor", props);
                    emission2ndMap = FindProperty("_Emission2ndMap", props);
                    emission2ndMapScrollX = FindProperty("_Emission2ndMapScrollX", props);
                    emission2ndMapScrollY = FindProperty("_Emission2ndMapScrollY", props);
                    emission2ndMapAngle = FindProperty("_Emission2ndMapAngle", props);
                    emission2ndMapRotate = FindProperty("_Emission2ndMapRotate", props);
                    emission2ndMapUV = FindProperty("_Emission2ndMapUV", props);
                    emission2ndMapTrim = FindProperty("_Emission2ndMapTrim", props);
                    emission2ndBlend = FindProperty("_Emission2ndBlend", props);
                    emission2ndBlendMask = FindProperty("_Emission2ndBlendMask", props);
                    emission2ndBlendMaskScrollX = FindProperty("_Emission2ndBlendMaskScrollX", props);
                    emission2ndBlendMaskScrollY = FindProperty("_Emission2ndBlendMaskScrollY", props);
                    emission2ndBlendMaskAngle = FindProperty("_Emission2ndBlendMaskAngle", props);
                    emission2ndBlendMaskRotate = FindProperty("_Emission2ndBlendMaskRotate", props);
                    emission2ndBlendMaskUV = FindProperty("_Emission2ndBlendMaskUV", props);
                    emission2ndBlendMaskTrim = FindProperty("_Emission2ndBlendMaskTrim", props);
                    emission2ndHue = FindProperty("_Emission2ndHue", props);
                    emission2ndSaturation = FindProperty("_Emission2ndSaturation", props);
                    emission2ndValue = FindProperty("_Emission2ndValue", props);
                    emission2ndUseBlink = FindProperty("_Emission2ndUseBlink", props);
                    emission2ndBlinkStrength = FindProperty("_Emission2ndBlinkStrength", props);
                    emission2ndBlinkSpeed = FindProperty("_Emission2ndBlinkSpeed", props);
                    emission2ndBlinkOffset = FindProperty("_EmissionBlinkOffset", props);
                    emission2ndBlinkType = FindProperty("_Emission2ndBlinkType", props);
                    emission2ndUseGrad = FindProperty("_Emission2ndUseGrad", props);
                    emission2ndGradTex = FindProperty("_Emission2ndGradTex", props);
                    emission2ndGradSpeed = FindProperty("_Emission2ndGradSpeed", props);
                    emission2ndParallaxDepth = FindProperty("_Emission2ndParallaxDepth", props);
                useEmission3rd = FindProperty("_UseEmission3rd", props);
                    emission3rdColor = FindProperty("_Emission3rdColor", props);
                    emission3rdMap = FindProperty("_Emission3rdMap", props);
                    emission3rdMapScrollX = FindProperty("_Emission3rdMapScrollX", props);
                    emission3rdMapScrollY = FindProperty("_Emission3rdMapScrollY", props);
                    emission3rdMapAngle = FindProperty("_Emission3rdMapAngle", props);
                    emission3rdMapRotate = FindProperty("_Emission3rdMapRotate", props);
                    emission3rdMapUV = FindProperty("_Emission3rdMapUV", props);
                    emission3rdMapTrim = FindProperty("_Emission3rdMapTrim", props);
                    emission3rdBlend = FindProperty("_Emission3rdBlend", props);
                    emission3rdBlendMask = FindProperty("_Emission3rdBlendMask", props);
                    emission3rdBlendMaskScrollX = FindProperty("_Emission3rdBlendMaskScrollX", props);
                    emission3rdBlendMaskScrollY = FindProperty("_Emission3rdBlendMaskScrollY", props);
                    emission3rdBlendMaskAngle = FindProperty("_Emission3rdBlendMaskAngle", props);
                    emission3rdBlendMaskRotate = FindProperty("_Emission3rdBlendMaskRotate", props);
                    emission3rdBlendMaskUV = FindProperty("_Emission3rdBlendMaskUV", props);
                    emission3rdBlendMaskTrim = FindProperty("_Emission3rdBlendMaskTrim", props);
                    emission3rdHue = FindProperty("_Emission3rdHue", props);
                    emission3rdSaturation = FindProperty("_Emission3rdSaturation", props);
                    emission3rdValue = FindProperty("_Emission3rdValue", props);
                    emission3rdUseBlink = FindProperty("_Emission3rdUseBlink", props);
                    emission3rdBlinkStrength = FindProperty("_Emission3rdBlinkStrength", props);
                    emission3rdBlinkSpeed = FindProperty("_Emission3rdBlinkSpeed", props);
                    emission3rdBlinkOffset = FindProperty("_EmissionBlinkOffset", props);
                    emission3rdBlinkType = FindProperty("_Emission3rdBlinkType", props);
                    emission3rdUseGrad = FindProperty("_Emission3rdUseGrad", props);
                    emission3rdGradTex = FindProperty("_Emission3rdGradTex", props);
                    emission3rdGradSpeed = FindProperty("_Emission3rdGradSpeed", props);
                    emission3rdParallaxDepth = FindProperty("_Emission3rdParallaxDepth", props);
                useEmission4th = FindProperty("_UseEmission4th", props);
                    emission4thColor = FindProperty("_Emission4thColor", props);
                    emission4thMap = FindProperty("_Emission4thMap", props);
                    emission4thMapScrollX = FindProperty("_Emission4thMapScrollX", props);
                    emission4thMapScrollY = FindProperty("_Emission4thMapScrollY", props);
                    emission4thMapAngle = FindProperty("_Emission4thMapAngle", props);
                    emission4thMapRotate = FindProperty("_Emission4thMapRotate", props);
                    emission4thMapUV = FindProperty("_Emission4thMapUV", props);
                    emission4thMapTrim = FindProperty("_Emission4thMapTrim", props);
                    emission4thBlend = FindProperty("_Emission4thBlend", props);
                    emission4thBlendMask = FindProperty("_Emission4thBlendMask", props);
                    emission4thBlendMaskScrollX = FindProperty("_Emission4thBlendMaskScrollX", props);
                    emission4thBlendMaskScrollY = FindProperty("_Emission4thBlendMaskScrollY", props);
                    emission4thBlendMaskAngle = FindProperty("_Emission4thBlendMaskAngle", props);
                    emission4thBlendMaskRotate = FindProperty("_Emission4thBlendMaskRotate", props);
                    emission4thBlendMaskUV = FindProperty("_Emission4thBlendMaskUV", props);
                    emission4thBlendMaskTrim = FindProperty("_Emission4thBlendMaskTrim", props);
                    emission4thHue = FindProperty("_Emission4thHue", props);
                    emission4thSaturation = FindProperty("_Emission4thSaturation", props);
                    emission4thValue = FindProperty("_Emission4thValue", props);
                    emission4thUseBlink = FindProperty("_Emission4thUseBlink", props);
                    emission4thBlinkStrength = FindProperty("_Emission4thBlinkStrength", props);
                    emission4thBlinkSpeed = FindProperty("_Emission4thBlinkSpeed", props);
                    emission4thBlinkOffset = FindProperty("_EmissionBlinkOffset", props);
                    emission4thBlinkType = FindProperty("_Emission4thBlinkType", props);
                    emission4thUseGrad = FindProperty("_Emission4thUseGrad", props);
                    emission4thGradTex = FindProperty("_Emission4thGradTex", props);
                    emission4thGradSpeed = FindProperty("_Emission4thGradSpeed", props);
                    emission4thParallaxDepth = FindProperty("_EmissionParallaxDepth", props);
            }
            else if(isLite)
            {
                invisible = FindProperty("_Invisible", props);
                renderingMode = FindProperty("_Mode", props);
                cullMode = FindProperty("_CullMode", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                zwrite = FindProperty("_ZWrite", props);
                ztest = FindProperty("_ZTest", props);
                cutoff = FindProperty("_Cutoff", props);
                flipNormal = FindProperty("_FlipNormal", props);
                backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
                useVertexColor = FindProperty("_UseVertexColor", props);
                stencilRef = FindProperty("_StencilRef", props);
                stencilComp = FindProperty("_StencilComp", props);
                stencilPass = FindProperty("_StencilPass", props);
                stencilFail = FindProperty("_StencilFail", props);
                stencilZFail = FindProperty("_StencilZFail", props);
                // Main
                //useMainTex = FindProperty("_UseMainTex", props);
                    mainColor = FindProperty("_Color", props);
                    mainTex = FindProperty("_MainTex", props);
                    mainTexTonecurve = FindProperty("_MainTexTonecurve", props);
                    mainTexHue = FindProperty("_MainTexHue", props);
                    mainTexSaturation = FindProperty("_MainTexSaturation", props);
                    mainTexValue = FindProperty("_MainTexValue", props);
                useMain2ndTex = FindProperty("_UseMain2ndTex", props);
                    mainColor2nd = FindProperty("_Color2nd", props);
                    main2ndTex = FindProperty("_Main2ndTex", props);
                    main2ndTexAngle = FindProperty("_Main2ndTexAngle", props);
                    main2ndTexTrim = FindProperty("_Main2ndTexTrim", props);
                    main2ndTexMix = FindProperty("_Main2ndTexMix", props);
                // Alpha
                useAlphaMask = FindProperty("_UseAlphaMask", props);
                    alphaMask = FindProperty("_AlphaMask", props);
                // Shadow
                useShadow = FindProperty("_UseShadow", props);
                    shadowBorder = FindProperty("_ShadowBorder", props);
                    shadowBlur = FindProperty("_ShadowBlur", props);
                    shadowStrengthMask = FindProperty("_ShadowStrengthMask", props);
                    shadowColor = FindProperty("_ShadowColor", props);
                // Outline
                useOutline = FindProperty("_UseOutline", props);
                    outlineColor = FindProperty("_OutlineColor", props);
                    outlineWidth = FindProperty("_OutlineWidth", props);
                    outlineAlphaMask = FindProperty("_OutlineAlphaMask", props);
                    vertexColor2Width = FindProperty("_VertexColor2Width", props);
                useMatcap = FindProperty("_UseMatcap", props);
                    matcapTex = FindProperty("_MatcapTex", props);
                    matcapColor = FindProperty("_MatcapColor", props);
                    matcapBlend = FindProperty("_MatcapBlend", props);
                    matcapBlendMask = FindProperty("_MatcapBlendMask", props);
                    matcapShadeMix = FindProperty("_MatcapShadeMix", props);
                    matcapMix = FindProperty("_MatcapMix", props);
                useEmission = FindProperty("_UseEmission", props);
                    emissionColor = FindProperty("_EmissionColor", props);
                    emissionMap = FindProperty("_EmissionMap", props);
                    emissionMapScrollX = FindProperty("_EmissionMapScrollX", props);
                    emissionMapScrollY = FindProperty("_EmissionMapScrollY", props);
                    emissionMapAngle = FindProperty("_EmissionMapAngle", props);
                    emissionMapRotate = FindProperty("_EmissionMapRotate", props);
                    emissionMapUV = FindProperty("_EmissionMapUV", props);
                    emissionMapTrim = FindProperty("_EmissionMapTrim", props);
                    emissionBlend = FindProperty("_EmissionBlend", props);
                    emissionBlendMask = FindProperty("_EmissionBlendMask", props);
                    emissionBlendMaskScrollX = FindProperty("_EmissionBlendMaskScrollX", props);
                    emissionBlendMaskScrollY = FindProperty("_EmissionBlendMaskScrollY", props);
                    emissionBlendMaskAngle = FindProperty("_EmissionBlendMaskAngle", props);
                    emissionBlendMaskRotate = FindProperty("_EmissionBlendMaskRotate", props);
                    emissionBlendMaskUV = FindProperty("_EmissionBlendMaskUV", props);
                    emissionBlendMaskTrim = FindProperty("_EmissionBlendMaskTrim", props);
                    emissionUseBlink = FindProperty("_EmissionUseBlink", props);
                    emissionBlinkStrength = FindProperty("_EmissionBlinkStrength", props);
                    emissionBlinkSpeed = FindProperty("_EmissionBlinkSpeed", props);
                    emissionBlinkOffset = FindProperty("_EmissionBlinkOffset", props);
                    emissionBlinkType = FindProperty("_EmissionBlinkType", props);
                    emissionUseGrad = FindProperty("_EmissionUseGrad", props);
                    emissionGradTex = FindProperty("_EmissionGradTex", props);
                    emissionGradSpeed = FindProperty("_EmissionGradSpeed", props);
                    emissionParallaxDepth = FindProperty("_EmissionParallaxDepth", props);
            }
            else
            {
                invisible = FindProperty("_Invisible", props);
                renderingMode = FindProperty("_Mode", props);
                cullMode = FindProperty("_CullMode", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                zwrite = FindProperty("_ZWrite", props);
                ztest = FindProperty("_ZTest", props);
                cutoff = FindProperty("_Cutoff", props);
                flipNormal = FindProperty("_FlipNormal", props);
                backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
                useVertexColor = FindProperty("_UseVertexColor", props);
                stencilRef = FindProperty("_StencilRef", props);
                stencilComp = FindProperty("_StencilComp", props);
                stencilPass = FindProperty("_StencilPass", props);
                stencilFail = FindProperty("_StencilFail", props);
                stencilZFail = FindProperty("_StencilZFail", props);
                // Main
                //useMainTex = FindProperty("_UseMainTex", props);
                    mainColor = FindProperty("_Color", props);
                    mainTex = FindProperty("_MainTex", props);
                    mainTexTonecurve = FindProperty("_MainTexTonecurve", props);
                    mainTexHue = FindProperty("_MainTexHue", props);
                    mainTexSaturation = FindProperty("_MainTexSaturation", props);
                    mainTexValue = FindProperty("_MainTexValue", props);
                useMain2ndTex = FindProperty("_UseMain2ndTex", props);
                    mainColor2nd = FindProperty("_Color2nd", props);
                    main2ndTex = FindProperty("_Main2ndTex", props);
                    main2ndTexAngle = FindProperty("_Main2ndTexAngle", props);
                    main2ndTexTrim = FindProperty("_Main2ndTexTrim", props);
                    main2ndTexMix = FindProperty("_Main2ndTexMix", props);
                useMain3rdTex = FindProperty("_UseMain3rdTex", props);
                    mainColor3rd = FindProperty("_Color3rd", props);
                    main3rdTex = FindProperty("_Main3rdTex", props);
                    main3rdTexAngle = FindProperty("_Main3rdTexAngle", props);
                    main3rdTexTrim = FindProperty("_Main3rdTexTrim", props);
                    main3rdTexMix = FindProperty("_Main3rdTexMix", props);
                useMain4thTex = FindProperty("_UseMain4thTex", props);
                    mainColor4th = FindProperty("_Color4th", props);
                    main4thTex = FindProperty("_Main4thTex", props);
                    main4thTexAngle = FindProperty("_Main4thTexAngle", props);
                    main4thTexTrim = FindProperty("_Main4thTexTrim", props);
                    main4thTexMix = FindProperty("_Main4thTexMix", props);
                // Alpha
                useAlphaMask = FindProperty("_UseAlphaMask", props);
                    alphaMask = FindProperty("_AlphaMask", props);
                // Shadow
                useShadow = FindProperty("_UseShadow", props);
                    shadowBorder = FindProperty("_ShadowBorder", props);
                    shadowBorderMask = FindProperty("_ShadowBorderMask", props);
                    shadowBlur = FindProperty("_ShadowBlur", props);
                    shadowBlurMask = FindProperty("_ShadowBlurMask", props);
                    shadowStrength = FindProperty("_ShadowStrength", props);
                    shadowStrengthMask = FindProperty("_ShadowStrengthMask", props);
                    useShadowMixMainColor = FindProperty("_UseShadowMixMainColor", props);
                    shadowMixMainColor = FindProperty("_ShadowMixMainColor", props);
                    shadowHue = FindProperty("_ShadowHue", props);
                    shadowSaturation = FindProperty("_ShadowSaturation", props);
                    shadowValue = FindProperty("_ShadowValue", props);
                    shadowGrad = FindProperty("_ShadowGrad", props);
                    shadowGradColor = FindProperty("_ShadowGradColor", props);
                    useShadowColor = FindProperty("_UseShadowColor", props);
                    shadowColorFromMain = FindProperty("_ShadowColorFromMain", props);
                    shadowColor = FindProperty("_ShadowColor", props);
                    shadowColorTex = FindProperty("_ShadowColorTex", props);
                    shadowColorMix = FindProperty("_ShadowColorMix", props);
                useShadow2nd = FindProperty("_UseShadow2nd", props);
                    shadow2ndBorder = FindProperty("_Shadow2ndBorder", props);
                    shadow2ndBlur = FindProperty("_Shadow2ndBlur", props);
                    shadow2ndColorFromMain = FindProperty("_Shadow2ndColorFromMain", props);
                    shadow2ndColor = FindProperty("_Shadow2ndColor", props);
                    shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props);
                    shadow2ndColorMix = FindProperty("_Shadow2ndColorMix", props);
                // Outline
                useOutline = FindProperty("_UseOutline", props);
                    outlineMixMainStrength = FindProperty("_OutlineMixMainStrength", props);
                    outlineHue = FindProperty("_OutlineHue", props);
                    outlineSaturation = FindProperty("_OutlineSaturation", props);
                    outlineValue = FindProperty("_OutlineValue", props);
                    outlineAutoHue = FindProperty("_OutlineAutoHue", props);
                    outlineAutoValue = FindProperty("_OutlineAutoValue", props);
                    outlineColor = FindProperty("_OutlineColor", props);
                    outlineWidth = FindProperty("_OutlineWidth", props);
                    outlineWidthMask = FindProperty("_OutlineWidthMask", props);
                    outlineAlphaMask = FindProperty("_OutlineAlphaMask", props);
                    vertexColor2Width = FindProperty("_VertexColor2Width", props);
                // Refraction
                if(isRefr)
                {
                    refractionStrength = FindProperty("_RefractionStrength", props);
                    refractionFresnelPower = FindProperty("_RefractionFresnelPower", props);
                    refractionColorFromMain = FindProperty("_RefractionColorFromMain", props);
                    refractionColor = FindProperty("_RefractionColor", props);
                }
                // Fur
                if(isFur)
                {
                    furNoiseMask = FindProperty("_FurNoiseMask", props);
                    furMask = FindProperty("_FurMask", props);
                    furVectorTex = FindProperty("_FurVectorTex", props);
                    furVectorScale = FindProperty("_FurVectorScale", props);
                    furVectorX = FindProperty("_FurVectorX", props);
                    furVectorY = FindProperty("_FurVectorY", props);
                    furVectorZ = FindProperty("_FurVectorZ", props);
                    furLength = FindProperty("_FurLength", props);
                    furGravity = FindProperty("_FurGravity", props);
                    furAO = FindProperty("_FurAO", props);
                    vertexColor2FurVector = FindProperty("_VertexColor2FurVector", props);
                    furLayerNum = FindProperty("_FurLayerNum", props);
                }
                // Normal
                useBumpMap = FindProperty("_UseBumpMap", props);
                    bumpMap = FindProperty("_BumpMap", props);
                    bumpScale = FindProperty("_BumpScale", props);
                useBump2ndMap = FindProperty("_UseBump2ndMap", props);
                    bump2ndMap = FindProperty("_Bump2ndMap", props);
                    bump2ndScale = FindProperty("_Bump2ndScale", props);
                useReflection = FindProperty("_UseReflection", props);
                    smoothness = FindProperty("_Smoothness", props);
                    smoothnessTex = FindProperty("_SmoothnessTex", props);
                    metallic = FindProperty("_Metallic", props);
                    metallicGlossMap = FindProperty("_MetallicGlossMap", props);
                    reflectionBlend = FindProperty("_ReflectionBlend", props);
                    reflectionBlendMask = FindProperty("_ReflectionBlendMask", props);
                    applySpecular = FindProperty("_ApplySpecular", props);
                    applyReflection = FindProperty("_ApplyReflection", props);
                    reflectionShadeMix = FindProperty("_ReflectionShadeMix", props);
                useMatcap = FindProperty("_UseMatcap", props);
                    matcapTex = FindProperty("_MatcapTex", props);
                    matcapColor = FindProperty("_MatcapColor", props);
                    matcapBlend = FindProperty("_MatcapBlend", props);
                    matcapBlendMask = FindProperty("_MatcapBlendMask", props);
                    matcapShadeMix = FindProperty("_MatcapShadeMix", props);
                    matcapMix = FindProperty("_MatcapMix", props);
                useRim = FindProperty("_UseRim", props);
                    rimColor = FindProperty("_RimColor", props);
                    rimBlend = FindProperty("_RimBlend", props);
                    rimBlendMask = FindProperty("_RimBlendMask", props);
                    rimToon = FindProperty("_RimToon", props);
                    rimBorder = FindProperty("_RimBorder", props);
                    rimBlur = FindProperty("_RimBlur", props);
                    rimUpperSideWidth = FindProperty("_RimUpperSideWidth", props);
                    rimFresnelPower = FindProperty("_RimFresnelPower", props);
                    rimShadeMix = FindProperty("_RimShadeMix", props);
                    rimShadowMask = FindProperty("_RimShadowMask", props);
                useEmission = FindProperty("_UseEmission", props);
                    emissionColor = FindProperty("_EmissionColor", props);
                    emissionMap = FindProperty("_EmissionMap", props);
                    emissionMapScrollX = FindProperty("_EmissionMapScrollX", props);
                    emissionMapScrollY = FindProperty("_EmissionMapScrollY", props);
                    emissionMapAngle = FindProperty("_EmissionMapAngle", props);
                    emissionMapRotate = FindProperty("_EmissionMapRotate", props);
                    emissionMapUV = FindProperty("_EmissionMapUV", props);
                    emissionMapTrim = FindProperty("_EmissionMapTrim", props);
                    emissionBlend = FindProperty("_EmissionBlend", props);
                    emissionBlendMask = FindProperty("_EmissionBlendMask", props);
                    emissionBlendMaskScrollX = FindProperty("_EmissionBlendMaskScrollX", props);
                    emissionBlendMaskScrollY = FindProperty("_EmissionBlendMaskScrollY", props);
                    emissionBlendMaskAngle = FindProperty("_EmissionBlendMaskAngle", props);
                    emissionBlendMaskRotate = FindProperty("_EmissionBlendMaskRotate", props);
                    emissionBlendMaskUV = FindProperty("_EmissionBlendMaskUV", props);
                    emissionBlendMaskTrim = FindProperty("_EmissionBlendMaskTrim", props);
                    emissionUseBlink = FindProperty("_EmissionUseBlink", props);
                    emissionBlinkStrength = FindProperty("_EmissionBlinkStrength", props);
                    emissionBlinkSpeed = FindProperty("_EmissionBlinkSpeed", props);
                    emissionBlinkOffset = FindProperty("_EmissionBlinkOffset", props);
                    emissionBlinkType = FindProperty("_EmissionBlinkType", props);
                    emissionUseGrad = FindProperty("_EmissionUseGrad", props);
                    emissionGradTex = FindProperty("_EmissionGradTex", props);
                    emissionGradSpeed = FindProperty("_EmissionGradSpeed", props);
                    emissionParallaxDepth = FindProperty("_EmissionParallaxDepth", props);
                useEmission2nd = FindProperty("_UseEmission2nd", props);
                    emission2ndColor = FindProperty("_Emission2ndColor", props);
                    emission2ndMap = FindProperty("_Emission2ndMap", props);
                    emission2ndMapScrollX = FindProperty("_Emission2ndMapScrollX", props);
                    emission2ndMapScrollY = FindProperty("_Emission2ndMapScrollY", props);
                    emission2ndMapAngle = FindProperty("_Emission2ndMapAngle", props);
                    emission2ndMapRotate = FindProperty("_Emission2ndMapRotate", props);
                    emission2ndMapUV = FindProperty("_Emission2ndMapUV", props);
                    emission2ndMapTrim = FindProperty("_Emission2ndMapTrim", props);
                    emission2ndBlend = FindProperty("_Emission2ndBlend", props);
                    emission2ndBlendMask = FindProperty("_Emission2ndBlendMask", props);
                    emission2ndBlendMaskScrollX = FindProperty("_Emission2ndBlendMaskScrollX", props);
                    emission2ndBlendMaskScrollY = FindProperty("_Emission2ndBlendMaskScrollY", props);
                    emission2ndBlendMaskAngle = FindProperty("_Emission2ndBlendMaskAngle", props);
                    emission2ndBlendMaskRotate = FindProperty("_Emission2ndBlendMaskRotate", props);
                    emission2ndBlendMaskUV = FindProperty("_Emission2ndBlendMaskUV", props);
                    emission2ndBlendMaskTrim = FindProperty("_Emission2ndBlendMaskTrim", props);
                    emission2ndUseBlink = FindProperty("_Emission2ndUseBlink", props);
                    emission2ndBlinkStrength = FindProperty("_Emission2ndBlinkStrength", props);
                    emission2ndBlinkSpeed = FindProperty("_Emission2ndBlinkSpeed", props);
                    emission2ndBlinkOffset = FindProperty("_EmissionBlinkOffset", props);
                    emission2ndBlinkType = FindProperty("_Emission2ndBlinkType", props);
                    emission2ndUseGrad = FindProperty("_Emission2ndUseGrad", props);
                    emission2ndGradTex = FindProperty("_Emission2ndGradTex", props);
                    emission2ndGradSpeed = FindProperty("_Emission2ndGradSpeed", props);
                    emission2ndParallaxDepth = FindProperty("_Emission2ndParallaxDepth", props);
            }
            #endregion

            // 見出し
            Font rmp_bold = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Editor/rounded-x-mplus-1c-bold.ttf", typeof(Font)) as Font;
            Texture2D midashiBack = new Texture2D(1,1);
            midashiBack.SetPixel(0,0,new Color(0.2f,0.2f,0.2f,1));
            midashiBack.Apply();
            GUIStyle midashi = new GUIStyle();
            midashi.normal.textColor = new Color(1,1,1,1);
            midashi.normal.background = midashiBack;
            midashi.fontSize = 16;
            midashi.font = rmp_bold;

            // マージンなしのボックス
            GUIStyle noMarginBox = new GUIStyle(GUI.skin.box);
            noMarginBox.margin = new RectOffset(0,0,0,0);

            GUIStyle circleDark = new GUIStyle((AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Editor/gui_circle_dark.guiskin", typeof(GUISkin)) as GUISkin).box);
            circleDark.margin = new RectOffset(20,3,3,3);
            GUIStyle halfcircleGray = new GUIStyle((AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Editor/gui_halfcircle_gray.guiskin", typeof(GUISkin)) as GUISkin).box);
            GUIStyle circleGray = new GUIStyle((AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Editor/gui_circle_gray.guiskin", typeof(GUISkin)) as GUISkin).box);

            // Toggle
            GUIStyle customToggleFont = new GUIStyle();
            customToggleFont.normal.textColor = new Color(1,1,1,1);
            customToggleFont.fontSize = 12;
            customToggleFont.contentOffset = new Vector2(0f, -1f);
            customToggleFont.font = rmp_bold;

            EditorGUI.BeginChangeCheck();

            // シェーダーキーワードを自動削除
            DeleteShaderKeyword(material);

            // 言語
            langNum = selectLang(langNum);
            // 編集モード
            edMode = (EditorMode)EditorGUILayout.Popup("EditorMode",(int)edMode,new String[]{loc["sEditorModeLimited"],loc["sEditorModeAdvanced"],loc["sEditorModePreset"]});
            switch (edMode)
            {
                case EditorMode.Limited:
                    EditorGUILayout.HelpBox(loc["sHelpLimited"],MessageType.Info);
                    break;
                case EditorMode.Advanced:
                    EditorGUILayout.HelpBox(loc["sHelpAdvanced"],MessageType.Info);
                    break;
                case EditorMode.Preset:
                    EditorGUILayout.HelpBox(loc["sHelpPreset"],MessageType.Info);
                    break;
            }
            GUILayout.Space(10);

            if(edMode == EditorMode.Limited)
            {
            // Limited
                EditorGUILayout.BeginVertical(circleDark);
                GUILayout.Label(loc["sBaseSetting"], customToggleFont);
                EditorGUILayout.BeginVertical(halfcircleGray);
                materialEditor.ShaderProperty(invisible, loc["sInvisible"]);
                {
                    rendModeBuf = (RenderingMode)material.GetFloat("_Mode");
                    renderingMode.floatValue = (float)EditorGUILayout.Popup(loc["sRenderingMode"],(int)renderingMode.floatValue,new String[]{loc["sRenderingModeOpaque"],loc["sRenderingModeCutout"],loc["sRenderingModeTransparent"],loc["sRenderingModeAdd"],loc["sRenderingModeMultiply"],loc["sRenderingModeRefraction"],loc["sRenderingModeFur"]});
                    if(rendModeBuf != (RenderingMode)material.GetFloat("_Mode"))
                    {
                        SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat("_Mode"), isFull, isLite, isStWr);
                    }
                    if((RenderingMode)material.GetFloat("_Mode") == RenderingMode.Cutout)
                    {
                        materialEditor.ShaderProperty(cutoff, loc["sCutoff"]);
                    }
                    if((RenderingMode)material.GetFloat("_Mode") >= RenderingMode.Transparent)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpRenderingTransparent"],MessageType.Warning);
                    }
                    cullMode.floatValue = (float)EditorGUILayout.Popup(loc["sCullMode"],(int)cullMode.floatValue,new String[]{loc["sCullModeOff"],loc["sCullModeFront"],loc["sCullModeBack"]});
                    if(cullMode.floatValue == 1.0f)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpCullMode"],MessageType.Warning);
                    }
                    // 外部シェーダーでfalseになっている場面を多く見かけるのでこっちでも編集可能にする
                    materialEditor.ShaderProperty(zwrite, loc["sZWrite"]);
                    if(zwrite.floatValue != 1.0f)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpZWrite"],MessageType.Warning);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                if(isFull)
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), mainTex, mainColor);
                        materialEditor.ShaderProperty(mainTexTonecurve, loc["sTonecurve"]);
                        materialEditor.ShaderProperty(mainTexHue, loc["sHue"]);
                        materialEditor.ShaderProperty(mainTexSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(mainTexValue, loc["sValue"]);
                        if(GUILayout.Button("Reset"))
                        {
                            mainTexTonecurve.floatValue = 1.0f;
                            mainTexHue.floatValue = 0.0f;
                            mainTexSaturation.floatValue = 1.0f;
                            mainTexValue.floatValue = 1.0f;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}
                    // Main 2nd
                    if(useMain2ndTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor2nd"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main2ndTex, mainColor2nd);
                        materialEditor.ShaderProperty(main2ndTexHue, loc["sHue"]);
                        materialEditor.ShaderProperty(main2ndTexSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(main2ndTexValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Main 3rd
                    if(useMain3rdTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor3rd"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main3rdTex, mainColor3rd);
                        materialEditor.ShaderProperty(main3rdTexHue, loc["sHue"]);
                        materialEditor.ShaderProperty(main3rdTexSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(main3rdTexValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Main 4th
                    if(useMain4thTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor4th"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main4thTex, mainColor4th);
                        materialEditor.ShaderProperty(main4thTexHue, loc["sHue"]);
                        materialEditor.ShaderProperty(main4thTexSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(main4thTexValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Shadow
                    if(useShadow.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimShadowStrength"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.ShaderProperty(shadowStrength, loc["sStrength"]);
                        materialEditor.ShaderProperty(shadowHue, loc["sHue"]);
                        materialEditor.ShaderProperty(shadowSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(shadowValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // 輪郭線
                    if(useOutline.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimOutline"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        if(useOutlineColor.floatValue == 1) materialEditor.ShaderProperty(outlineColor, loc["sColor"]);
                        materialEditor.ShaderProperty(outlineHue, loc["sHue"]);
                        materialEditor.ShaderProperty(outlineSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(outlineValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Emission
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimEmission"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), emissionMap, emissionColor);
                        materialEditor.ShaderProperty(emissionHue, loc["sHue"]);
                        materialEditor.ShaderProperty(emissionSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(emissionValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Emission 2nd
                    if(useEmission2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimEmission"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), emission2ndMap, emission2ndColor);
                        materialEditor.ShaderProperty(emission2ndHue, loc["sHue"]);
                        materialEditor.ShaderProperty(emission2ndSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(emission2ndValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Emission 3rd
                    if(useEmission3rd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimEmission"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), emission3rdMap, emission3rdColor);
                        materialEditor.ShaderProperty(emission3rdHue, loc["sHue"]);
                        materialEditor.ShaderProperty(emission3rdSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(emission3rdValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Emission 4th
                    if(useEmission4th.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimEmission"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), emission4thMap, emission4thColor);
                        materialEditor.ShaderProperty(emission4thHue, loc["sHue"]);
                        materialEditor.ShaderProperty(emission4thSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(emission4thValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
                else if(isLite)
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), mainTex, mainColor);
                        materialEditor.ShaderProperty(mainTexTonecurve, loc["sTonecurve"]);
                        materialEditor.ShaderProperty(mainTexHue, loc["sHue"]);
                        materialEditor.ShaderProperty(mainTexSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(mainTexValue, loc["sValue"]);
                        if(GUILayout.Button("Reset"))
                        {
                            mainTexTonecurve.floatValue = 1.0f;
                            mainTexHue.floatValue = 0.0f;
                            mainTexSaturation.floatValue = 1.0f;
                            mainTexValue.floatValue = 1.0f;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}
                    // Main 2nd
                    if(useMain2ndTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor2nd"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main2ndTex, mainColor2nd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Shadow
                    if(useShadow.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimShadowStrength"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.ShaderProperty(shadowColor, loc["sShadowColor"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // 輪郭線
                    if(useOutline.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimOutline"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.ShaderProperty(outlineColor, loc["sColor"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Emission
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimEmission"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), emissionMap, emissionColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), mainTex, mainColor);
                        materialEditor.ShaderProperty(mainTexTonecurve, loc["sTonecurve"]);
                        materialEditor.ShaderProperty(mainTexHue, loc["sHue"]);
                        materialEditor.ShaderProperty(mainTexSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(mainTexValue, loc["sValue"]);
                        if(GUILayout.Button("Reset"))
                        {
                            mainTexTonecurve.floatValue = 1.0f;
                            mainTexHue.floatValue = 0.0f;
                            mainTexSaturation.floatValue = 1.0f;
                            mainTexValue.floatValue = 1.0f;
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}
                    // Main 2nd
                    if(useMain2ndTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor2nd"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main2ndTex, mainColor2nd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Main 3rd
                    if(useMain3rdTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor3rd"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main3rdTex, mainColor3rd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Main 4th
                    if(useMain4thTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimMainColor4th"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main4thTex, mainColor4th);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Shadow
                    if(useShadow.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimShadowStrength"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.ShaderProperty(shadowStrength, loc["sStrength"]);
                        materialEditor.ShaderProperty(shadowHue, loc["sHue"]);
                        materialEditor.ShaderProperty(shadowSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(shadowValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // 輪郭線
                    if(useOutline.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimOutline"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.ShaderProperty(outlineColor, loc["sColor"]);
                        materialEditor.ShaderProperty(outlineHue, loc["sHue"]);
                        materialEditor.ShaderProperty(outlineSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(outlineValue, loc["sValue"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Emission
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimEmission"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), emissionMap, emissionColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    // Emission 2nd
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sLimEmission"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), emission2ndMap, emission2ndColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
            } else if(edMode == EditorMode.Preset) {
            // Preset
                GUILayout.Label(loc["sPresets"], midashi);
                GUIStyle PresetButton = new GUIStyle(GUI.skin.button);
                PresetButton.alignment = TextAnchor.MiddleLeft;
                //isShowPresets = Foldout(loc["sPresets"], loc["sPresets"], isShowPresets);
                //if(isShowPresets)
                //{
                    EditorGUILayout.BeginVertical(circleDark);
                    EditorGUILayout.BeginVertical(circleGray);
                    if(isLite)
                    {
                        // 肌
                        GUILayout.Label(loc["sSkin"]);
                        if(GUILayout.Button(loc["sPresetsSkinFaceParts"],PresetButton))
                        {
                            cullMode.floatValue = 2.0f;
                            useShadow.floatValue = 0.0f;
                            useOutline.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsSkinLine"],PresetButton))
                        {
                            cullMode.floatValue = 2.0f;
                            useShadow.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineWidth.floatValue = 0.07f;
                                outlineColor.colorValue = new Color(0.875f,0.6f,0.475f,1.0f);
                            useMatcap.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsSkinLineShadow"],PresetButton))
                        {
                            cullMode.floatValue = 2.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.4f;
                                shadowBlur.floatValue = 0.25f;
                                shadowColor.colorValue = new Color(0.85f,0.7f,0.65f,1.0f);
                            useOutline.floatValue = 1.0f;
                                outlineWidth.floatValue = 0.07f;
                                outlineColor.colorValue = new Color(0.875f,0.6f,0.475f,1.0f);
                            useMatcap.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsSkinAnime"],PresetButton))
                        {
                            cullMode.floatValue = 2.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.4f;
                                shadowBlur.floatValue = 0.01f;
                                shadowColor.colorValue = new Color(0.85f,0.7f,0.65f,1.0f);
                            useOutline.floatValue = 1.0f;
                                outlineWidth.floatValue = 0.1f;
                                outlineColor.colorValue = new Color(0.875f,0.6f,0.475f,1.0f);
                            useMatcap.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        GUILayout.Space(10);
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        // 髪
                        GUILayout.Label(loc["sHair"]);
                        if(GUILayout.Button(loc["sPresetsHair"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.25f;
                                shadowColor.colorValue = new Color(0.95f,0.9f,0.9f,1.0f);
                            useOutline.floatValue = 1.0f;
                                outlineWidth.floatValue = 0.07f;
                                outlineColor.colorValue = new Color(0.3f,0.3f,0.3f,1.0f);
                            useMatcap.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsHairAnime"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.01f;
                                shadowColor.colorValue = new Color(0.85f,0.7f,0.7f,1.0f);
                            useOutline.floatValue = 1.0f;
                                outlineWidth.floatValue = 0.1f;
                                outlineColor.colorValue = new Color(0.3f,0.3f,0.3f,1.0f);
                            useMatcap.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        GUILayout.Space(10);
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        // 衣服
                        GUILayout.Label(loc["sCostume"]);
                        if(GUILayout.Button(loc["sPresetsCostume"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.25f;
                                shadowColor.colorValue = new Color(0.9f,0.925f,0.95f,1.0f);
                            useOutline.floatValue = 1.0f;
                                outlineWidth.floatValue = 0.07f;
                                outlineColor.colorValue = new Color(0.3f,0.3f,0.3f,1.0f);
                            useMatcap.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsCostumeAnime"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.01f;
                                shadowColor.colorValue = new Color(0.7f,0.8f,0.9f,1.0f);
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineWidth.floatValue = 0.1f;
                                outlineColor.colorValue = new Color(0.3f,0.3f,0.3f,1.0f);
                            useMatcap.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                    }
                    else
                    {
                        // 肌
                        GUILayout.Label(loc["sSkin"]);
                        if(GUILayout.Button(loc["sPresetsSkinFaceParts"],PresetButton))
                        {
                            cullMode.floatValue = 2.0f;
                            useShadow.floatValue = 0.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 0.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsSkinLine"],PresetButton))
                        {
                            cullMode.floatValue = 2.0f;
                            useShadow.floatValue = 0.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineMixMainStrength.floatValue = 2.0f;
                                outlineHue.floatValue = 0.0f;
                                outlineSaturation.floatValue = 0.75f;
                                outlineValue.floatValue = -0.1f;
                                outlineAutoHue.floatValue = 0.35f;
                                outlineAutoValue.floatValue = 0.25f;
                                outlineWidth.floatValue = 0.07f;
                                if(isFull) outlineAlpha.floatValue = 1.0f;
                                outlineColor.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 0.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsSkinLineShadow"],PresetButton))
                        {
                            cullMode.floatValue = 2.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.4f;
                                shadowBlur.floatValue = 0.25f;
                                shadowStrength.floatValue = 0.35f;
                                useShadowMixMainColor.floatValue = 1.0f;
                                shadowMixMainColor.floatValue = 2.0f;
                                shadowGrad.floatValue = 1.0f;
                                shadowHue.floatValue = 0.0f;
                                shadowSaturation.floatValue = 0.0f;
                                shadowValue.floatValue = 0.0f;
                                useShadowColor.floatValue = 1.0f;
                                shadowColorFromMain.floatValue = 1.0f;
                                shadowColor.colorValue = new Color(0.73f,0.39f,0.37f,1.0f);
                                shadowColorMix.floatValue = 2.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineMixMainStrength.floatValue = 2.0f;
                                outlineHue.floatValue = 0.0f;
                                outlineSaturation.floatValue = 0.75f;
                                outlineValue.floatValue = -0.1f;
                                outlineAutoHue.floatValue = 0.35f;
                                outlineAutoValue.floatValue = 0.25f;
                                outlineWidth.floatValue = 0.07f;
                                if(isFull) outlineAlpha.floatValue = 1.0f;
                                outlineColor.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 0.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsSkinAnime"],PresetButton))
                        {
                            cullMode.floatValue = 2.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.4f;
                                shadowBlur.floatValue = 0.01f;
                                shadowStrength.floatValue = 0.7f;
                                useShadowMixMainColor.floatValue = 1.0f;
                                shadowMixMainColor.floatValue = 2.0f;
                                shadowGrad.floatValue = 1.0f;
                                shadowHue.floatValue = 0.0f;
                                shadowSaturation.floatValue = 0.0f;
                                shadowValue.floatValue = 0.0f;
                                useShadowColor.floatValue = 1.0f;
                                shadowColorFromMain.floatValue = 1.0f;
                                shadowColor.colorValue = new Color(0.73f,0.39f,0.37f,1.0f);
                                shadowColorMix.floatValue = 2.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineMixMainStrength.floatValue = 2.0f;
                                outlineHue.floatValue = 0.0f;
                                outlineSaturation.floatValue = 0.75f;
                                outlineValue.floatValue = -0.1f;
                                outlineAutoHue.floatValue = 0.35f;
                                outlineAutoValue.floatValue = 0.25f;
                                outlineWidth.floatValue = 0.1f;
                                if(isFull) outlineAlpha.floatValue = 1.0f;
                                outlineColor.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 0.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        GUILayout.Space(10);
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        // 髪
                        GUILayout.Label(loc["sHair"]);
                        if(GUILayout.Button(loc["sPresetsHair"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.25f;
                                shadowStrength.floatValue = 0.15f;
                                useShadowMixMainColor.floatValue = 1.0f;
                                shadowMixMainColor.floatValue = 2.0f;
                                shadowGrad.floatValue = 1.0f;
                                shadowHue.floatValue = 0.9f;
                                shadowSaturation.floatValue = 0.0f;
                                shadowValue.floatValue = 0.0f;
                                useShadowColor.floatValue = 0.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineMixMainStrength.floatValue = 2.0f;
                                outlineHue.floatValue = 0.0f;
                                outlineSaturation.floatValue = 0.75f;
                                outlineValue.floatValue = -0.1f;
                                outlineAutoHue.floatValue = 0.35f;
                                outlineAutoValue.floatValue = 0.25f;
                                outlineWidth.floatValue = 0.07f;
                                if(isFull) outlineAlpha.floatValue = 1.0f;
                                outlineColor.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 0.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsHairRim"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.25f;
                                shadowStrength.floatValue = 0.15f;
                                useShadowMixMainColor.floatValue = 1.0f;
                                shadowMixMainColor.floatValue = 2.0f;
                                shadowGrad.floatValue = 1.0f;
                                shadowHue.floatValue = 0.9f;
                                shadowSaturation.floatValue = 0.0f;
                                shadowValue.floatValue = 0.0f;
                                useShadowColor.floatValue = 0.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineMixMainStrength.floatValue = 2.0f;
                                outlineHue.floatValue = 0.0f;
                                outlineSaturation.floatValue = 0.75f;
                                outlineValue.floatValue = -0.1f;
                                outlineAutoHue.floatValue = 0.35f;
                                outlineAutoValue.floatValue = 0.25f;
                                outlineWidth.floatValue = 0.07f;
                                if(isFull) outlineAlpha.floatValue = 1.0f;
                                outlineColor.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 1.0f;
                                rimColor.floatValue = 1.0f;
                                rimBlend.floatValue = 0.75f;
                                rimToon.floatValue = 0.0f;
                                rimUpperSideWidth.floatValue = 0.0f;
                                rimFresnelPower.floatValue = 10.0f;
                                rimShadeMix.floatValue = 1.0f;
                                rimShadowMask.floatValue = 1.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsHairAnime"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.01f;
                                shadowStrength.floatValue = 0.5f;
                                useShadowMixMainColor.floatValue = 1.0f;
                                shadowMixMainColor.floatValue = 2.0f;
                                shadowGrad.floatValue = 1.0f;
                                shadowHue.floatValue = 0.9f;
                                shadowSaturation.floatValue = 0.0f;
                                shadowValue.floatValue = 0.0f;
                                useShadowColor.floatValue = 0.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineMixMainStrength.floatValue = 2.0f;
                                outlineHue.floatValue = 0.0f;
                                outlineSaturation.floatValue = 0.75f;
                                outlineValue.floatValue = -0.1f;
                                outlineAutoHue.floatValue = 0.35f;
                                outlineAutoValue.floatValue = 0.25f;
                                outlineWidth.floatValue = 0.1f;
                                if(isFull) outlineAlpha.floatValue = 1.0f;
                                outlineColor.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 1.0f;
                                rimColor.floatValue = 1.0f;
                                rimBlend.floatValue = 0.5f;
                                rimToon.floatValue = 1.0f;
                                    rimBorder.floatValue = 0.65f;
                                    rimBlur.floatValue = 0.2f;
                                rimUpperSideWidth.floatValue = 0.0f;
                                rimFresnelPower.floatValue = 10.0f;
                                rimShadeMix.floatValue = 1.0f;
                                rimShadowMask.floatValue = 1.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        GUILayout.Space(10);
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        // 衣服
                        GUILayout.Label(loc["sCostume"]);
                        if(GUILayout.Button(loc["sPresetsCostume"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.25f;
                                shadowStrength.floatValue = 0.25f;
                                useShadowMixMainColor.floatValue = 1.0f;
                                shadowMixMainColor.floatValue = 2.0f;
                                shadowGrad.floatValue = 1.0f;
                                shadowHue.floatValue = 0.9f;
                                shadowSaturation.floatValue = 0.0f;
                                shadowValue.floatValue = 0.0f;
                                useShadowColor.floatValue = 0.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineMixMainStrength.floatValue = 2.0f;
                                outlineHue.floatValue = 0.0f;
                                outlineSaturation.floatValue = 0.75f;
                                outlineValue.floatValue = -0.1f;
                                outlineAutoHue.floatValue = 0.35f;
                                outlineAutoValue.floatValue = 0.25f;
                                outlineWidth.floatValue = 0.07f;
                                if(isFull) outlineAlpha.floatValue = 1.0f;
                                outlineColor.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 1.0f;
                                rimColor.floatValue = 1.0f;
                                rimBlend.floatValue = 0.25f;
                                rimToon.floatValue = 0.0f;
                                rimUpperSideWidth.floatValue = 0.0f;
                                rimFresnelPower.floatValue = 5.0f;
                                rimShadeMix.floatValue = 1.0f;
                                rimShadowMask.floatValue = 1.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        if(GUILayout.Button(loc["sPresetsCostumeAnime"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.01f;
                                shadowStrength.floatValue = 0.5f;
                                useShadowMixMainColor.floatValue = 1.0f;
                                shadowMixMainColor.floatValue = 2.0f;
                                shadowGrad.floatValue = 1.0f;
                                shadowHue.floatValue = 0.9f;
                                shadowSaturation.floatValue = 0.0f;
                                shadowValue.floatValue = 0.0f;
                                useShadowColor.floatValue = 0.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 1.0f;
                                outlineMixMainStrength.floatValue = 2.0f;
                                outlineHue.floatValue = 0.0f;
                                outlineSaturation.floatValue = 0.75f;
                                outlineValue.floatValue = -0.1f;
                                outlineAutoHue.floatValue = 0.35f;
                                outlineAutoValue.floatValue = 0.25f;
                                outlineWidth.floatValue = 0.1f;
                                if(isFull) outlineAlpha.floatValue = 1.0f;
                                outlineColor.floatValue = 0.0f;
                            useReflection.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 1.0f;
                                rimColor.floatValue = 1.0f;
                                rimBlend.floatValue = 0.5f;
                                rimToon.floatValue = 1.0f;
                                    rimBorder.floatValue = 0.65f;
                                    rimBlur.floatValue = 0.2f;
                                rimUpperSideWidth.floatValue = 0.0f;
                                rimFresnelPower.floatValue = 10.0f;
                                rimShadeMix.floatValue = 1.0f;
                                rimShadowMask.floatValue = 1.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                        GUILayout.Space(10);
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        // その他
                        GUILayout.Label(loc["sOther"]);
                        if(GUILayout.Button(loc["sPresetsMetal"],PresetButton))
                        {
                            cullMode.floatValue = 0.0f;
                            flipNormal.floatValue = 0.0f;
                            useShadow.floatValue = 1.0f;
                                shadowBorder.floatValue = 0.6f;
                                shadowBlur.floatValue = 0.25f;
                                shadowStrength.floatValue = 0.5f;
                                useShadowMixMainColor.floatValue = 1.0f;
                                shadowMixMainColor.floatValue = 2.0f;
                                shadowGrad.floatValue = 1.0f;
                                shadowHue.floatValue = 0.9f;
                                shadowSaturation.floatValue = 0.0f;
                                shadowValue.floatValue = 0.0f;
                                useShadowColor.floatValue = 0.0f;
                            if(isFull) useDefaultShading.floatValue = 0.0f;
                            useOutline.floatValue = 0.0f;
                            useReflection.floatValue = 1.0f;
                                smoothness.floatValue = 0.95f;
                                metallic.floatValue = 1.0f;
                                reflectionBlend.floatValue = 1.0f;
                                applySpecular.floatValue = 1.0f;
                                applyReflection.floatValue = 1.0f;
                                if(isFull) reflectionUseCubemap.floatValue = 0.0f;
                                reflectionShadeMix.floatValue = 0.0f;
                            useMatcap.floatValue = 0.0f;
                            if(isFull) useMatcap2nd.floatValue = 0.0f;
                            //useMatcap3rd.floatValue = 0.0f;
                            //useMatcap4th.floatValue = 0.0f;
                            useRim.floatValue = 0.0f;
                            //useRim2nd.floatValue = 0.0f;
                            useEmission.floatValue = 0.0f;
                        }
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                //}
            } else {
            // Advanced
                // レンダリングモード
                GUILayout.Label(loc["sBaseSetting"], midashi);
                EditorGUILayout.BeginVertical(noMarginBox);
                materialEditor.ShaderProperty(invisible, loc["sInvisible"]);
                {
                    rendModeBuf = (RenderingMode)material.GetFloat("_Mode");
                    renderingMode.floatValue = (float)EditorGUILayout.Popup(loc["sRenderingMode"],(int)renderingMode.floatValue,new String[]{loc["sRenderingModeOpaque"],loc["sRenderingModeCutout"],loc["sRenderingModeTransparent"],loc["sRenderingModeAdd"],loc["sRenderingModeMultiply"],loc["sRenderingModeRefraction"],loc["sRenderingModeFur"]});
                    if(rendModeBuf != (RenderingMode)material.GetFloat("_Mode"))
                    {
                        SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat("_Mode"), isFull, isLite, isStWr);
                    }
                    if((RenderingMode)material.GetFloat("_Mode") == RenderingMode.Cutout)
                    {
                        materialEditor.ShaderProperty(cutoff, loc["sCutoff"]);
                    }
                    if((RenderingMode)material.GetFloat("_Mode") >= RenderingMode.Transparent)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpRenderingTransparent"],MessageType.Warning);
                    }
                    cullMode.floatValue = (float)EditorGUILayout.Popup(loc["sCullMode"],(int)cullMode.floatValue,new String[]{loc["sCullModeOff"],loc["sCullModeFront"],loc["sCullModeBack"]});
                    if(cullMode.floatValue == 1.0f)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpCullMode"],MessageType.Warning);
                    }
                    if(cullMode.floatValue <= 1.0f)
                    {
                        materialEditor.ShaderProperty(flipNormal, loc["sFlipBackfaceNormal"]);
                        materialEditor.ShaderProperty(backfaceForceShadow, loc["sBackfaceForceShadow"]);
                    }
                    // 外部シェーダーでfalseになっている場面を多く見かけるのでこっちでも編集可能にする
                    materialEditor.ShaderProperty(zwrite, loc["sZWrite"]);
                    if(zwrite.floatValue != 1.0f)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpZWrite"],MessageType.Warning);
                    }
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(10);

                GUILayout.Label(loc["sColors"], midashi);
                // 基本色
                isShowMain = Foldout(loc["sMainColor"], loc["sMainColorTips"], isShowMain);
                if(isShowMain)
                {
                    // Main
                    EditorGUILayout.BeginVertical(circleDark);
                    GUILayout.Label(loc["sLimMainColor"], customToggleFont);
                    //useMainTex.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMainColor"], useMainTex.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        if(isFull)  isShowMainTex = TextureGui(materialEditor, isShowMainTex, loc["sTexture"], loc["sTextureRGBA"], mainTex, mainColor, mainTexScrollX, mainTexScrollY, mainTexAngle, mainTexRotate, mainTexUV, mainTexTrim);
                        else
                        {
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), mainTex, mainColor);
                            materialEditor.TextureScaleOffsetProperty(mainTex);
                        }
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        materialEditor.ShaderProperty(mainTexTonecurve, loc["sTonecurve"]);
                        materialEditor.ShaderProperty(mainTexHue, loc["sHue"]);
                        materialEditor.ShaderProperty(mainTexSaturation, loc["sSaturation"]);
                        materialEditor.ShaderProperty(mainTexValue, loc["sValue"]);
                        if(GUILayout.Button("Reset"))
                        {
                            mainTexTonecurve.floatValue = 1.0f;
                            mainTexHue.floatValue = 0.0f;
                            mainTexSaturation.floatValue = 1.0f;
                            mainTexValue.floatValue = 1.0f;
                        }
                        EditorGUILayout.EndVertical();
                    //}
                    EditorGUILayout.EndVertical();
                    // 2nd
                    EditorGUILayout.BeginVertical(circleDark);
                    useMain2ndTex.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMainColor2nd"], useMain2ndTex.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                    if(useMain2ndTex.floatValue == 1)
                    {
                        if(isFull)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowMain2ndTex = TextureGui(materialEditor, isShowMain2ndTex, loc["sTexture"], loc["sTextureRGBA"], main2ndTex, mainColor2nd, main2ndTexScrollX, main2ndTexScrollY, main2ndTexAngle, main2ndTexRotate, main2ndTexUV, main2ndTexTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowMain2ndBlendMask = MaskTextureGui(materialEditor, isShowMain2ndBlendMask, loc["sMask"], loc["sBlendR"], main2ndBlendMask, main2ndBlend, main2ndBlendMaskScrollX, main2ndBlendMaskScrollY, main2ndBlendMaskAngle, main2ndBlendMaskRotate, main2ndBlendMaskUV, main2ndBlendMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            main2ndTexMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)main2ndTexMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(main2ndTexHue, loc["sHue"]);
                            materialEditor.ShaderProperty(main2ndTexSaturation, loc["sSaturation"]);
                            materialEditor.ShaderProperty(main2ndTexValue, loc["sValue"]);
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main2ndTex, mainColor2nd);
                            materialEditor.TextureScaleOffsetProperty(main2ndTex);
                            materialEditor.ShaderProperty(main2ndTexAngle, loc["sAngle"]);
                            materialEditor.ShaderProperty(main2ndTexTrim, loc["sTrim"]);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            main2ndTexMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)main2ndTexMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndVertical();
                    if(!isLite)
                    {
                        // 3rd
                        EditorGUILayout.BeginVertical(circleDark);
                        useMain3rdTex.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMainColor3rd"], useMain3rdTex.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useMain3rdTex.floatValue == 1)
                        {
                            if(isFull)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowMain3rdTex = TextureGui(materialEditor, isShowMain3rdTex, loc["sTexture"], loc["sTextureRGBA"], main3rdTex, mainColor3rd, main3rdTexScrollX, main3rdTexScrollY, main3rdTexAngle, main3rdTexRotate, main3rdTexUV, main3rdTexTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowMain3rdBlendMask = MaskTextureGui(materialEditor, isShowMain3rdBlendMask, loc["sMask"], loc["sBlendR"], main3rdBlendMask, main3rdBlend, main3rdBlendMaskScrollX, main3rdBlendMaskScrollY, main3rdBlendMaskAngle, main3rdBlendMaskRotate, main3rdBlendMaskUV, main3rdBlendMaskTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                main3rdTexMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)main3rdTexMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(main3rdTexHue, loc["sHue"]);
                                materialEditor.ShaderProperty(main3rdTexSaturation, loc["sSaturation"]);
                                materialEditor.ShaderProperty(main3rdTexValue, loc["sValue"]);
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main3rdTex, mainColor3rd);
                                materialEditor.TextureScaleOffsetProperty(main3rdTex);
                                materialEditor.ShaderProperty(main3rdTexAngle, loc["sAngle"]);
                                materialEditor.ShaderProperty(main3rdTexTrim, loc["sTrim"]);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                main3rdTexMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)main3rdTexMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndVertical();
                        // 4th
                        EditorGUILayout.BeginVertical(circleDark);
                        useMain4thTex.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMainColor4th"], useMain4thTex.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useMain4thTex.floatValue == 1)
                        {
                            if(isFull)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowMain4thTex = TextureGui(materialEditor, isShowMain4thTex, loc["sTexture"], loc["sTextureRGBA"], main4thTex, mainColor4th, main4thTexScrollX, main4thTexScrollY, main4thTexAngle, main4thTexRotate, main4thTexUV, main4thTexTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowMain4thBlendMask = MaskTextureGui(materialEditor, isShowMain4thBlendMask, loc["sMask"], loc["sBlendR"], main4thBlendMask, main4thBlend, main4thBlendMaskScrollX, main4thBlendMaskScrollY, main4thBlendMaskAngle, main4thBlendMaskRotate, main4thBlendMaskUV, main4thBlendMaskTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                main4thTexMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)main4thTexMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(main4thTexHue, loc["sHue"]);
                                materialEditor.ShaderProperty(main4thTexSaturation, loc["sSaturation"]);
                                materialEditor.ShaderProperty(main4thTexValue, loc["sValue"]);
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sTexture"], loc["sTextureRGBA"]), main4thTex, mainColor4th);
                                materialEditor.TextureScaleOffsetProperty(main4thTex);
                                materialEditor.ShaderProperty(main4thTexAngle, loc["sAngle"]);
                                materialEditor.ShaderProperty(main4thTexTrim, loc["sTrim"]);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                main4thTexMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)main4thTexMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                // 透過
                isShowAlpha = Foldout(loc["sAlphaMaskF"], loc["sAlphaMaskTips"], isShowAlpha);
                if(isShowAlpha)
                {
                    // Alpha
                    EditorGUILayout.BeginVertical(circleDark);
                    useAlphaMask.floatValue = EditorGUILayout.ToggleLeft(loc["sUseAlphaMask"], useAlphaMask.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                    if(useAlphaMask.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        if(isFull)
                        {
                            isShowAlphaMask = MaskTextureGui(materialEditor, isShowAlphaMask, loc["sAlphaMask"], loc["sTransparencyA"], alphaMask, alpha, alphaMaskScrollX, alphaMaskScrollY, alphaMaskAngle, alphaMaskRotate, alphaMaskUV, alphaMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(alphaMaskMixMain, loc["sMixMainAlpha"]);
                        }
                        else
                        {
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sAlphaMask"], loc["sTransparencyA"]), alphaMask);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                    /* アルファマスク追加分
                    // Alpha2nd
                    EditorGUILayout.BeginVertical(circleDark);
                    useAlphaMask2nd.floatValue = EditorGUILayout.ToggleLeft(loc["sUseAlphaMask2nd"], useAlphaMask2nd.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                    if(useAlphaMask2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        isShowAlphaMask2nd = MaskTextureGui(materialEditor, isShowAlphaMask2nd, loc["sAlphaMask"], loc["sTransparencyA"], alphaMask2nd, alpha2nd, alphaMask2ndScrollX, alphaMask2ndScrollY, alphaMask2ndAngle, alphaMask2ndRotate, alphaMask2ndUV, alphaMask2ndTrim);
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        materialEditor.ShaderProperty(alphaMask2ndMixMain, loc["sMixMainAlpha"]);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                    */
                }

                // 影設定
                isShowShadow = Foldout(loc["sShadow"], loc["sShadowTips"], isShowShadow);
                if(isShowShadow)
                {
                    // Shadow
                    EditorGUILayout.BeginVertical(circleDark);
                    useShadow.floatValue = EditorGUILayout.ToggleLeft(loc["sUseShadow"], useShadow.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                    if(useShadow.floatValue == 1)
                    {
                        if(isFull)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowShadowBorderMask = MaskTextureGui(materialEditor, isShowShadowBorderMask, loc["sBorder"], loc["sBorderR"], shadowBorderMask, shadowBorder, shadowBorderMaskScrollX, shadowBorderMaskScrollY, shadowBorderMaskAngle, shadowBorderMaskRotate, shadowBorderMaskUV, shadowBorderMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowShadowBlurMask = MaskTextureGui(materialEditor, isShowShadowBlurMask, loc["sBlur"], loc["sBlurR"], shadowBlurMask, shadowBlur, shadowBlurMaskScrollX, shadowBlurMaskScrollY, shadowBlurMaskAngle, shadowBlurMaskRotate, shadowBlurMaskUV, shadowBlurMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowShadowStrengthMask = MaskTextureGui(materialEditor, isShowShadowStrengthMask, loc["sStrength"], loc["sStrengthR"], shadowStrengthMask, shadowStrength, shadowStrengthMaskScrollX, shadowStrengthMaskScrollY, shadowStrengthMaskAngle, shadowStrengthMaskRotate, shadowStrengthMaskUV, shadowStrengthMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(useShadowMixMainColor, loc["sMixMainColor"]);
                            if(useShadowMixMainColor.floatValue == 1)
                            {
                                materialEditor.ShaderProperty(shadowMixMainColor, " ");
                            }
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(shadowHue, loc["sHue"]);
                            materialEditor.ShaderProperty(shadowSaturation, loc["sSaturation"]);
                            materialEditor.ShaderProperty(shadowValue, loc["sValue"]);
                            materialEditor.ShaderProperty(shadowGrad, loc["sShadowGradation"]);
                            materialEditor.ShaderProperty(shadowGradColor, loc["sShadowGradColor"]);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(useShadowColor, loc["sUseCustomColor"]);
                            if(useShadowColor.floatValue == 1)
                            {
                                isShowShadowColor = TextureGui(materialEditor, isShowShadowColor, loc["sShadowColor"], loc["sColorRGBS"], shadowColorTex, shadowColor, shadowColorTexScrollX, shadowColorTexScrollY, shadowColorTexAngle, shadowColorTexRotate, shadowColorTexUV, shadowColorTexTrim);
                                materialEditor.ShaderProperty(shadowColorFromMain, loc["sShadowColorFromMain"]);
                                shadowColorMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)shadowColorMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            }
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(useShadow2nd, loc["sUseShadow2nd"]);
                            if(useShadow2nd.floatValue == 1)
                            {
                                materialEditor.ShaderProperty(shadow2ndBorder, loc["sBorder"]);
                                materialEditor.ShaderProperty(shadow2ndBlur, loc["sBlur"]);
                                isShowShadow2ndColor = TextureGui(materialEditor, isShowShadow2ndColor, loc["sShadowColor"], loc["sColorRGBS"], shadow2ndColorTex, shadow2ndColor, shadow2ndColorTexScrollX, shadow2ndColorTexScrollY, shadow2ndColorTexAngle, shadow2ndColorTexRotate, shadow2ndColorTexUV, shadow2ndColorTexTrim);
                                materialEditor.ShaderProperty(shadow2ndColorFromMain, loc["sShadowColorFromMain"]);
                                shadow2ndColorMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)shadow2ndColorMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            }
                            EditorGUILayout.EndVertical();
                        }
                        else if(isLite)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            materialEditor.ShaderProperty(shadowBorder, loc["sBorder"]);
                            materialEditor.ShaderProperty(shadowBlur, loc["sBlur"]);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sStrength"], loc["sStrengthR"]), shadowStrengthMask);
                            materialEditor.ShaderProperty(shadowColor, loc["sShadowColor"]);
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sBorder"], loc["sBorderR"]), shadowBorderMask, shadowBorder);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sBlur"], loc["sBlurR"]), shadowBlurMask, shadowBlur);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sStrength"], loc["sStrengthR"]), shadowStrengthMask, shadowStrength);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(useShadowMixMainColor, loc["sMixMainColor"]);
                            if(useShadowMixMainColor.floatValue == 1)
                            {
                                materialEditor.ShaderProperty(shadowMixMainColor, " ");
                            }
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(shadowHue, loc["sHue"]);
                            materialEditor.ShaderProperty(shadowSaturation, loc["sSaturation"]);
                            materialEditor.ShaderProperty(shadowValue, loc["sValue"]);
                            materialEditor.ShaderProperty(shadowGrad, loc["sShadowGradation"]);
                            materialEditor.ShaderProperty(shadowGradColor, loc["sShadowGradColor"]);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(useShadowColor, loc["sUseCustomColor"]);
                            if(useShadowColor.floatValue == 1)
                            {
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadowColor"], loc["sColorRGBS"]), shadowColorTex, shadowColor);
                                materialEditor.ShaderProperty(shadowColorFromMain, loc["sShadowColorFromMain"]);
                                shadowColorMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)shadowColorMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            }
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(useShadow2nd, loc["sUseShadow2nd"]);
                            if(useShadow2nd.floatValue == 1)
                            {
                                materialEditor.ShaderProperty(shadow2ndBorder, loc["sBorder"]);
                                materialEditor.ShaderProperty(shadow2ndBlur, loc["sBlur"]);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadowColor"], loc["sColorRGBS"]), shadow2ndColorTex, shadow2ndColor);
                                materialEditor.ShaderProperty(shadow2ndColorFromMain, loc["sShadowColorFromMain"]);
                                shadow2ndColorMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)shadow2ndColorMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndVertical();
                    // Default Shading
                    if(isFull)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        useDefaultShading.floatValue = EditorGUILayout.ToggleLeft(loc["sUseDefaultShading"], useDefaultShading.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useDefaultShading.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowDefaultShadingMask = MaskTextureGui(materialEditor, isShowDefaultShadingMask, loc["sBlend"], loc["sBlendR"], defaultShadingBlendMask, defaultShadingBlend, defaultShadingBlendMaskScrollX, defaultShadingBlendMaskScrollY, defaultShadingBlendMaskAngle, defaultShadingBlendMaskRotate, defaultShadingBlendMaskUV, defaultShadingBlendMaskTrim);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                // 輪郭線
                isShowOutline = Foldout(loc["sOutline"], loc["sOutlineTips"], isShowOutline);
                if(isShowOutline)
                {
                    EditorGUILayout.BeginVertical(circleDark);
                    useOutline.floatValue = EditorGUILayout.ToggleLeft(loc["sUseOutline"], useOutline.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                    if(useOutline.floatValue == 1)
                    {
                        if(isLite)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            materialEditor.ShaderProperty(outlineWidth, loc["sWidth"]);
                            materialEditor.ShaderProperty(vertexColor2Width, loc["sVertexColor2Width"]);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sAlpha"], loc["sAlphaR"]), outlineAlphaMask);
                            materialEditor.ShaderProperty(outlineColor, loc["sColor"]);
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            materialEditor.ShaderProperty(outlineMixMainStrength, loc["sStrength"]);
                            materialEditor.ShaderProperty(outlineHue, loc["sHue"]);
                            materialEditor.ShaderProperty(outlineSaturation, loc["sSaturation"]);
                            materialEditor.ShaderProperty(outlineValue, loc["sValue"]);
                            materialEditor.ShaderProperty(outlineAutoHue, loc["sAutoHue"]);
                            materialEditor.ShaderProperty(outlineAutoValue, loc["sAutoValue"]);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            if(isFull)  isShowOutlineWidthMask = MaskTextureGui(materialEditor, isShowOutlineWidthMask, loc["sWidth"], loc["sWidthR"], outlineWidthMask, outlineWidth, outlineWidthMaskScrollX, outlineWidthMaskScrollY, outlineWidthMaskAngle, outlineWidthMaskRotate, outlineWidthMaskUV, outlineWidthMaskTrim);
                            else        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sWidth"], loc["sWidthR"]), outlineWidthMask, outlineWidth);
                            materialEditor.ShaderProperty(vertexColor2Width, loc["sVertexColor2Width"]);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            if(isFull)  isShowOutlineAlphaMask = MaskTextureGui(materialEditor, isShowOutlineAlphaMask, loc["sAlpha"], loc["sAlphaR"], outlineAlphaMask, outlineAlpha, outlineAlphaMaskScrollX, outlineAlphaMaskScrollY, outlineAlphaMaskAngle, outlineAlphaMaskRotate, outlineAlphaMaskUV, outlineAlphaMaskTrim);
                            else        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sAlpha"], loc["sAlphaR"]), outlineAlphaMask);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            if(isFull)
                            {
                                materialEditor.ShaderProperty(useOutlineColor, loc["sUseCustomColor"]);
                                if(useOutlineColor.floatValue == 1)
                                {
                                    isShowOutlineColorTex = TextureGui(materialEditor, isShowOutlineColorTex, loc["sColor"], loc["sColorRGBS"], outlineColorTex, outlineColor, outlineColorTexScrollX, outlineColorTexScrollY, outlineColorTexAngle, outlineColorTexRotate, outlineColorTexUV, outlineColorTexTrim);
                                }
                            }
                            else
                            {
                                materialEditor.ShaderProperty(outlineColor, loc["sColor"]);
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                // 発光
                isShowEmission = Foldout(loc["sEmission"], loc["sEmissionTips"], isShowEmission);
                if(isShowEmission)
                {
                    // Emission
                    EditorGUILayout.BeginVertical(circleDark);
                    useEmission.floatValue = EditorGUILayout.ToggleLeft(loc["sUseEmission"], useEmission.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        isShowEmissionMap = TextureGui(materialEditor, isShowEmissionMap, loc["sColor"], loc["sTextureRGBA"], emissionMap, emissionColor, emissionMapScrollX, emissionMapScrollY, emissionMapAngle, emissionMapRotate, emissionMapUV, emissionMapTrim);
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        isShowEmissionBlendMask = MaskTextureGui(materialEditor, isShowEmissionBlendMask, loc["sBlend"], loc["sBlendR"], emissionBlendMask, emissionBlend, emissionBlendMaskScrollX, emissionBlendMaskScrollY, emissionBlendMaskAngle, emissionBlendMaskRotate, emissionBlendMaskUV, emissionBlendMaskTrim);
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        materialEditor.ShaderProperty(emissionUseBlink, loc["sUseBlink"]);
                        if(emissionUseBlink.floatValue == 1)
                        {
                            materialEditor.ShaderProperty(emissionBlinkStrength, loc["sStrength"]);
                            materialEditor.ShaderProperty(emissionBlinkSpeed, loc["sBlinkSpeed"]);
                            materialEditor.ShaderProperty(emissionBlinkOffset, loc["sBlinkOffset"]);
                            materialEditor.ShaderProperty(emissionBlinkType, loc["sBlinkType"]);
                        }
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        materialEditor.ShaderProperty(emissionUseGrad, loc["sUseGrad"]);
                        if(emissionUseGrad.floatValue == 1)
                        {
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sGradTexSpeed"], loc["sTextureRGBA"]), emissionGradTex, emissionGradSpeed);
                            gradientEditor(material, "_eg", emiGrad, "_EmissionGradTex");
                        }
                        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        if(isFull)
                        {
                            materialEditor.ShaderProperty(emissionHue, loc["sHue"]);
                            materialEditor.ShaderProperty(emissionSaturation, loc["sSaturation"]);
                            materialEditor.ShaderProperty(emissionValue, loc["sValue"]);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                        }
                        materialEditor.ShaderProperty(emissionParallaxDepth, loc["sParallaxDepth"]);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                    if(!isLite)
                    {
                        // Emission 2nd
                        EditorGUILayout.BeginVertical(circleDark);
                        useEmission2nd.floatValue = EditorGUILayout.ToggleLeft(loc["sUseEmission2nd"], useEmission2nd.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useEmission2nd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowEmission2ndMap = TextureGui(materialEditor, isShowEmission2ndMap, loc["sColor"], loc["sTextureRGBA"], emission2ndMap, emission2ndColor, emission2ndMapScrollX, emission2ndMapScrollY, emission2ndMapAngle, emission2ndMapRotate, emission2ndMapUV, emission2ndMapTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowEmission2ndBlendMask = MaskTextureGui(materialEditor, isShowEmission2ndBlendMask, loc["sBlend"], loc["sBlendR"], emission2ndBlendMask, emission2ndBlend, emission2ndBlendMaskScrollX, emission2ndBlendMaskScrollY, emission2ndBlendMaskAngle, emission2ndBlendMaskRotate, emission2ndBlendMaskUV, emission2ndBlendMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(emission2ndUseBlink, loc["sUseBlink"]);
                            if(emission2ndUseBlink.floatValue == 1)
                            {
                                materialEditor.ShaderProperty(emission2ndBlinkStrength, loc["sStrength"]);
                                materialEditor.ShaderProperty(emission2ndBlinkSpeed, loc["sBlinkSpeed"]);
                                materialEditor.ShaderProperty(emission2ndBlinkOffset, loc["sBlinkOffset"]);
                                materialEditor.ShaderProperty(emission2ndBlinkType, loc["sBlinkType"]);
                            }
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(emission2ndUseGrad, loc["sUseGrad"]);
                            if(emission2ndUseGrad.floatValue == 1)
                            {
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sGradTexSpeed"], loc["sTextureRGBA"]), emission2ndGradTex, emission2ndGradSpeed);
                                gradientEditor(material, "_e2g", emi2Grad, "_Emission2ndGradTex");
                            }
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            if(isFull)
                            {
                                materialEditor.ShaderProperty(emission2ndHue, loc["sHue"]);
                                materialEditor.ShaderProperty(emission2ndSaturation, loc["sSaturation"]);
                                materialEditor.ShaderProperty(emission2ndValue, loc["sValue"]);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            }
                            materialEditor.ShaderProperty(emission2ndParallaxDepth, loc["sParallaxDepth"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                        if(isFull)
                        {
                            // Emission 3rd
                            EditorGUILayout.BeginVertical(circleDark);
                            useEmission3rd.floatValue = EditorGUILayout.ToggleLeft(loc["sUseEmission3rd"], useEmission3rd.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                            if(useEmission3rd.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowEmission3rdMap = TextureGui(materialEditor, isShowEmission3rdMap, loc["sColor"], loc["sTextureRGBA"], emission3rdMap, emission3rdColor, emission3rdMapScrollX, emission3rdMapScrollY, emission3rdMapAngle, emission3rdMapRotate, emission3rdMapUV, emission3rdMapTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowEmission3rdBlendMask = MaskTextureGui(materialEditor, isShowEmission3rdBlendMask, loc["sBlend"], loc["sBlendR"], emission3rdBlendMask, emission3rdBlend, emission3rdBlendMaskScrollX, emission3rdBlendMaskScrollY, emission3rdBlendMaskAngle, emission3rdBlendMaskRotate, emission3rdBlendMaskUV, emission3rdBlendMaskTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(emission3rdUseBlink, loc["sUseBlink"]);
                                if(emission3rdUseBlink.floatValue == 1)
                                {
                                    materialEditor.ShaderProperty(emission3rdBlinkStrength, loc["sStrength"]);
                                    materialEditor.ShaderProperty(emission3rdBlinkSpeed, loc["sBlinkSpeed"]);
                                    materialEditor.ShaderProperty(emission3rdBlinkOffset, loc["sBlinkOffset"]);
                                    materialEditor.ShaderProperty(emission3rdBlinkType, loc["sBlinkType"]);
                                }
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(emission3rdUseGrad, loc["sUseGrad"]);
                                if(emission3rdUseGrad.floatValue == 1)
                                {
                                    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sGradTexSpeed"], loc["sTextureRGBA"]), emission3rdGradTex, emission3rdGradSpeed);
                                    gradientEditor(material, "_e3g", emi3Grad, "_Emission3rdGradTex");
                                }
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(emission3rdHue, loc["sHue"]);
                                materialEditor.ShaderProperty(emission3rdSaturation, loc["sSaturation"]);
                                materialEditor.ShaderProperty(emission3rdValue, loc["sValue"]);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(emission3rdParallaxDepth, loc["sParallaxDepth"]);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                            // Emission 4th
                            EditorGUILayout.BeginVertical(circleDark);
                            useEmission4th.floatValue = EditorGUILayout.ToggleLeft(loc["sUseEmission4th"], useEmission4th.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                            if(useEmission4th.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowEmission4thMap = TextureGui(materialEditor, isShowEmission4thMap, loc["sColor"], loc["sTextureRGBA"], emission4thMap, emission4thColor, emission4thMapScrollX, emission4thMapScrollY, emission4thMapAngle, emission4thMapRotate, emission4thMapUV, emission4thMapTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowEmission4thBlendMask = MaskTextureGui(materialEditor, isShowEmission4thBlendMask, loc["sBlend"], loc["sBlendR"], emission4thBlendMask, emission4thBlend, emission4thBlendMaskScrollX, emission4thBlendMaskScrollY, emission4thBlendMaskAngle, emission4thBlendMaskRotate, emission4thBlendMaskUV, emission4thBlendMaskTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(emission4thUseBlink, loc["sUseBlink"]);
                                if(emission4thUseBlink.floatValue == 1)
                                {
                                    materialEditor.ShaderProperty(emission4thBlinkStrength, loc["sStrength"]);
                                    materialEditor.ShaderProperty(emission4thBlinkSpeed, loc["sBlinkSpeed"]);
                                    materialEditor.ShaderProperty(emission4thBlinkOffset, loc["sBlinkOffset"]);
                                    materialEditor.ShaderProperty(emission4thBlinkType, loc["sBlinkType"]);
                                }
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(emission4thUseGrad, loc["sUseGrad"]);
                                if(emission4thUseGrad.floatValue == 1)
                                {
                                    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sGradTexSpeed"], loc["sTextureRGBA"]), emission4thGradTex, emission4thGradSpeed);
                                    gradientEditor(material, "_e4g", emi4Grad, "_Emission4thGradTex");
                                }
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(emission4thHue, loc["sHue"]);
                                materialEditor.ShaderProperty(emission4thSaturation, loc["sSaturation"]);
                                materialEditor.ShaderProperty(emission4thValue, loc["sValue"]);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(emission4thParallaxDepth, loc["sParallaxDepth"]);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                }
                GUILayout.Space(10);

                GUILayout.Label(loc["sNormalReflection"], midashi);
                // ノーマルマップ
                if(!isLite)
                {
                    isShowBump = Foldout(loc["sNormal"], loc["sNormalTips"], isShowBump);
                    if(isShowBump)
                    {
                        if(isFull)
                        {
                            // Normal
                            EditorGUILayout.BeginVertical(circleDark);
                            useBumpMap.floatValue = EditorGUILayout.ToggleLeft(loc["sUseNormalMap"], useBumpMap.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                            if(useBumpMap.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowBumpMap = NormalTextureGui(materialEditor, isShowBumpMap, loc["sNormalMap"], loc["sNormalRGB"], bumpMap, bumpMapScrollX, bumpMapScrollY, bumpMapAngle, bumpMapRotate, bumpMapUV, bumpMapTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowBumpScaleMask = MaskTextureGui(materialEditor, isShowBumpScaleMask, loc["sScale"], loc["sScaleR"], bumpScaleMask, bumpScale, bumpScaleMaskScrollX, bumpScaleMaskScrollY, bumpScaleMaskAngle, bumpScaleMaskRotate, bumpScaleMaskUV, bumpScaleMaskTrim);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                            // 2nd
                            EditorGUILayout.BeginVertical(circleDark);
                            useBump2ndMap.floatValue = EditorGUILayout.ToggleLeft(loc["sUseNormalMap2nd"], useBump2ndMap.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                            if(useBump2ndMap.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowBump2ndMap = NormalTextureGui(materialEditor, isShowBump2ndMap, loc["sNormalMap"], loc["sNormalRGB"], bump2ndMap, bump2ndMapScrollX, bump2ndMapScrollY, bump2ndMapAngle, bump2ndMapRotate, bump2ndMapUV, bump2ndMapTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowBump2ndScaleMask = MaskTextureGui(materialEditor, isShowBump2ndScaleMask, loc["sScale"], loc["sScaleR"], bump2ndScaleMask, bump2ndScale, bump2ndScaleMaskScrollX, bump2ndScaleMaskScrollY, bump2ndScaleMaskAngle, bump2ndScaleMaskRotate, bump2ndScaleMaskUV, bump2ndScaleMaskTrim);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                            /* ノーマルマップ追加分
                            // 3rd
                            EditorGUILayout.BeginVertical(circleDark);
                            useBump3rdMap.floatValue = EditorGUILayout.ToggleLeft(loc["sUseNormalMap3rd"], useBump3rdMap.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                            if(useBump3rdMap.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowBump3rdMap = NormalTextureGui(materialEditor, isShowBump3rdMap, loc["sNormalMap"], loc["sNormalRGB"], bump3rdMap, bump3rdMapScrollX, bump3rdMapScrollY, bump3rdMapAngle, bump3rdMapRotate, bump3rdMapUV, bump3rdMapTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowBump3rdScaleMask = MaskTextureGui(materialEditor, isShowBump3rdScaleMask, loc["sScale"], loc["sScaleR"], bump3rdScaleMask, bump3rdScale, bump3rdScaleMaskScrollX, bump3rdScaleMaskScrollY, bump3rdScaleMaskAngle, bump3rdScaleMaskRotate, bump3rdScaleMaskUV, bump3rdScaleMaskTrim);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                            // 4th
                            EditorGUILayout.BeginVertical(circleDark);
                            useBump4thMap.floatValue = EditorGUILayout.ToggleLeft(loc["sUseNormalMap4th"], useBump4thMap.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                            if(useBump4thMap.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowBump4thMap = NormalTextureGui(materialEditor, isShowBump4thMap, loc["sNormalMap"], loc["sNormalRGB"], bump4thMap, bump4thMapScrollX, bump4thMapScrollY, bump4thMapAngle, bump4thMapRotate, bump4thMapUV, bump4thMapTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowBump4thScaleMask = MaskTextureGui(materialEditor, isShowBump4thScaleMask, loc["sScale"], loc["sScaleR"], bump4thScaleMask, bump4thScale, bump4thScaleMaskScrollX, bump4thScaleMaskScrollY, bump4thScaleMaskAngle, bump4thScaleMaskRotate, bump4thScaleMaskUV, bump4thScaleMaskTrim);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                            */
                        }
                        else
                        {
                            // Normal
                            EditorGUILayout.BeginVertical(circleDark);
                            useBumpMap.floatValue = EditorGUILayout.ToggleLeft(loc["sUseNormalMap"], useBumpMap.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                            if(useBumpMap.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sNormalMap"], loc["sNormalRGB"]), bumpMap, bumpScale);
                                materialEditor.TextureScaleOffsetProperty(bumpMap);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                            // 2nd
                            EditorGUILayout.BeginVertical(circleDark);
                            useBump2ndMap.floatValue = EditorGUILayout.ToggleLeft(loc["sUseNormalMap2nd"], useBump2ndMap.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                            if(useBump2ndMap.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sNormalMap"], loc["sNormalRGB"]), bump2ndMap, bump2ndScale);
                                materialEditor.TextureScaleOffsetProperty(bump2ndMap);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                // 質感
                isShowReflections = Foldout(loc["sReflections"], loc["sReflectionsTips"], isShowReflections);
                if(isShowReflections)
                {
                    if(!isLite)
                    {
                        // Reflection
                        EditorGUILayout.BeginVertical(circleDark);
                        useReflection.floatValue = EditorGUILayout.ToggleLeft(loc["sUseReflection"], useReflection.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useReflection.floatValue == 1)
                        {
                            if(isFull)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowSmoothness = MaskTextureGui(materialEditor, isShowSmoothness, loc["sSmoothness"], loc["sSmoothnessR"], smoothnessTex, smoothness, smoothnessTexScrollX, smoothnessTexScrollY, smoothnessTexAngle, smoothnessTexRotate, smoothnessTexUV, smoothnessTexTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowMetallic = MaskTextureGui(materialEditor, isShowMetallic, loc["sMetallic"], loc["sMetallicR"], metallicGlossMap, metallic, metallicGlossMapScrollX, metallicGlossMapScrollY, metallicGlossMapAngle, metallicGlossMapRotate, metallicGlossMapUV, metallicGlossMapTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowReflectionBlendMask = MaskTextureGui(materialEditor, isShowReflectionBlendMask, loc["sBlend"], loc["sBlendR"], reflectionBlendMask, reflectionBlend, reflectionBlendMaskScrollX, reflectionBlendMaskScrollY, reflectionBlendMaskAngle, reflectionBlendMaskRotate, reflectionBlendMaskUV, reflectionBlendMaskTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(applySpecular, loc["sApplySpecular"]);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(applyReflection, loc["sApplyReflection"]);
                                if(applyReflection.floatValue == 1)
                                {
                                    materialEditor.ShaderProperty(reflectionUseCubemap, loc["sUseCubemap"]);
                                    if(reflectionUseCubemap.floatValue == 1)
                                    {
                                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sCubemap"], loc["sSelectCubemap"]), reflectionCubemap);
                                    }
                                    EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                    materialEditor.ShaderProperty(reflectionShadeMix, loc["sShadeMix"]);
                                }
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sSmoothness"], loc["sSmoothnessR"]), smoothnessTex, smoothness);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sMetallic"], loc["sMetallicR"]), metallicGlossMap, metallic);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sBlend"], loc["sBlendR"]), reflectionBlendMask, reflectionBlend);
                                materialEditor.ShaderProperty(applySpecular, loc["sApplySpecular"]);
                                materialEditor.ShaderProperty(applyReflection, loc["sApplyReflection"]);
                                if(applyReflection.floatValue == 1) materialEditor.ShaderProperty(reflectionShadeMix, loc["sShadeMix"]);
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    // Matcap
                    if(isFull)
                    {
                        // Matcap
                        EditorGUILayout.BeginVertical(circleDark);
                        useMatcap.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMatcap"], useMatcap.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useMatcap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowMatcapTex = MatcapTextureGui(materialEditor, isShowMatcapTex, loc["sMatcap"], loc["sTextureRGBA"], matcapTex, matcapColor);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowMatcapBlendMask = MaskTextureGui(materialEditor, isShowMatcapBlendMask, loc["sBlend"], loc["sBlendR"], matcapBlendMask, matcapBlend, matcapBlendMaskScrollX, matcapBlendMaskScrollY, matcapBlendMaskAngle, matcapBlendMaskRotate, matcapBlendMaskUV, matcapBlendMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(matcapNormalMix, loc["sNormalMapMix"]);
                            materialEditor.ShaderProperty(matcapShadeMix, loc["sShadeMix"]);
                            matcapMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)matcapMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            EditorGUILayout.EndVertical();
                        }
                        // Matcap2nd
                        EditorGUILayout.BeginVertical(circleDark);
                        useMatcap2nd.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMatcap2nd"], useMatcap2nd.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useMatcap2nd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowMatcap2ndTex = MatcapTextureGui(materialEditor, isShowMatcap2ndTex, loc["sMatcap"], loc["sTextureRGBA"], matcap2ndTex, matcap2ndColor);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowMatcap2ndBlendMask = MaskTextureGui(materialEditor, isShowMatcap2ndBlendMask, loc["sBlend"], loc["sBlendR"], matcap2ndBlendMask, matcap2ndBlend, matcap2ndBlendMaskScrollX, matcap2ndBlendMaskScrollY, matcap2ndBlendMaskAngle, matcap2ndBlendMaskRotate, matcap2ndBlendMaskUV, matcap2ndBlendMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(matcap2ndNormalMix, loc["sNormalMapMix"]);
                            materialEditor.ShaderProperty(matcap2ndShadeMix, loc["sShadeMix"]);
                            matcap2ndMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)matcap2ndMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                        /* マットキャップ追加分
                        // Matcap3rd
                        EditorGUILayout.BeginVertical(circleDark);
                        useMatcap3rd.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMatcap3rd"], useMatcap3rd.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useMatcap3rd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowMatcap3rdTex = MatcapTextureGui(materialEditor, isShowMatcap3rdTex, loc["sMatcap"], loc["sTextureRGBA"], matcap3rdTex, matcap3rdColor);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowMatcap3rdBlendMask = MaskTextureGui(materialEditor, isShowMatcap3rdBlendMask, loc["sBlend"], loc["sBlendR"], matcap3rdBlendMask, matcap3rdBlend, matcap3rdBlendMaskScrollX, matcap3rdBlendMaskScrollY, matcap3rdBlendMaskAngle, matcap3rdBlendMaskRotate, matcap3rdBlendMaskUV, matcap3rdBlendMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(matcap3rdNormalMix, loc["sNormalMapMix"]);
                            materialEditor.ShaderProperty(matcap3rdShadeMix, loc["sShadeMix"]);
                            matcap3rdMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)matcap3rdMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                        // Matcap4th
                        EditorGUILayout.BeginVertical(circleDark);
                        useMatcap4th.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMatcap4th"], useMatcap4th.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useMatcap4th.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowMatcap4thTex = MatcapTextureGui(materialEditor, isShowMatcap4thTex, loc["sMatcap"], loc["sTextureRGBA"], matcap4thTex, matcap4thColor);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowMatcap4thBlendMask = MaskTextureGui(materialEditor, isShowMatcap4thBlendMask, loc["sBlend"], loc["sBlendR"], matcap4thBlendMask, matcap4thBlend, matcap4thBlendMaskScrollX, matcap4thBlendMaskScrollY, matcap4thBlendMaskAngle, matcap4thBlendMaskRotate, matcap4thBlendMaskUV, matcap4thBlendMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(matcap4thNormalMix, loc["sNormalMapMix"]);
                            materialEditor.ShaderProperty(matcap4thShadeMix, loc["sShadeMix"]);
                            matcap4thMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)matcap4thMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                        */
                    }
                    else
                    {
                        // Matcap
                        EditorGUILayout.BeginVertical(circleDark);
                        useMatcap.floatValue = EditorGUILayout.ToggleLeft(loc["sUseMatcap"], useMatcap.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useMatcap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sMatcap"], loc["sTextureRGBA"]), matcapTex, matcapColor);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sBlend"], loc["sBlendR"]), matcapBlendMask, matcapBlend);
                            materialEditor.ShaderProperty(matcapShadeMix, loc["sShadeMix"]);
                            matcapMix.floatValue = (float)EditorGUILayout.Popup(loc["sMixMode"],(int)matcapMix.floatValue,new String[]{loc["sMixModeAlpha"],loc["sMixModeAdd"],loc["sMixModeScreen"],loc["sMixModeMul"],loc["sMixModeOverlay"]});
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    if(!isLite)
                    {
                        // Rim
                        EditorGUILayout.BeginVertical(circleDark);
                        useRim.floatValue = EditorGUILayout.ToggleLeft(loc["sUseRim"], useRim.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useRim.floatValue == 1)
                        {
                            if(isFull)
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                isShowRimColorTex = TextureGui(materialEditor, isShowRimColorTex, loc["sColor"], loc["sTextureRGBA"], rimColorTex, rimColor, rimColorTexScrollX, rimColorTexScrollY, rimColorTexAngle, rimColorTexRotate, rimColorTexUV, rimColorTexTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowRimBlendMask = MaskTextureGui(materialEditor, isShowRimBlendMask, loc["sBlend"], loc["sBlendR"], rimBlendMask, rimBlend, rimBlendMaskScrollX, rimBlendMaskScrollY, rimBlendMaskAngle, rimBlendMaskRotate, rimBlendMaskUV, rimBlendMaskTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(rimToon, loc["sToon"]);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                if(rimToon.floatValue == 1)
                                {
                                    isShowRimBorderMask = MaskTextureGui(materialEditor, isShowRimBorderMask, loc["sBorder"], loc["sBorderR"], rimBorderMask, rimBorder, rimBorderMaskScrollX, rimBorderMaskScrollY, rimBorderMaskAngle, rimBorderMaskRotate, rimBorderMaskUV, rimBorderMaskTrim);
                                    EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                    isShowRimBlurMask = MaskTextureGui(materialEditor, isShowRimBlurMask, loc["sBlur"], loc["sBlurR"], rimBlurMask, rimBlur, rimBlurMaskScrollX, rimBlurMaskScrollY, rimBlurMaskAngle, rimBlurMaskRotate, rimBlurMaskUV, rimBlurMaskTrim);
                                    EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                }
                                materialEditor.ShaderProperty(rimUpperSideWidth, loc["sUpperSideWidth"]);
                                materialEditor.ShaderProperty(rimFresnelPower, loc["sFresnelPower"]);
                                materialEditor.ShaderProperty(rimShadeMix, loc["sShadeMix"]);
                                materialEditor.ShaderProperty(rimShadowMask, loc["sShadowMask"]);
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical(halfcircleGray);
                                materialEditor.ShaderProperty(rimColor, loc["sColor"]);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sBlend"], loc["sBlendR"]), rimBlendMask, rimBlend);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(rimToon, loc["sToon"]);
                                if(rimToon.floatValue == 1)
                                {
                                    materialEditor.ShaderProperty(rimBorder, loc["sBorder"]);
                                    materialEditor.ShaderProperty(rimBlur, loc["sBlur"]);
                                }
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                materialEditor.ShaderProperty(rimUpperSideWidth, loc["sUpperSideWidth"]);
                                materialEditor.ShaderProperty(rimFresnelPower, loc["sFresnelPower"]);
                                materialEditor.ShaderProperty(rimShadeMix, loc["sShadeMix"]);
                                materialEditor.ShaderProperty(rimShadowMask, loc["sShadowMask"]);
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndVertical();
                        /* リムライト追加分
                        // Rim2nd
                        EditorGUILayout.BeginVertical(circleDark);
                        useRim2nd.floatValue = EditorGUILayout.ToggleLeft(loc["sUseRim2nd"], useRim2nd.floatValue > 0.5f, customToggleFont) ? 1.0f : 0.0f;
                        if(useRim2nd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(halfcircleGray);
                            isShowRim2ndColorTex = TextureGui(materialEditor, isShowRim2ndColorTex, loc["sColor"], loc["sTextureRGBA"], rim2ndColorTex, rim2ndColor, rim2ndColorTexScrollX, rim2ndColorTexScrollY, rim2ndColorTexAngle, rim2ndColorTexRotate, rim2ndColorTexUV, rim2ndColorTexTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            isShowRim2ndBlendMask = MaskTextureGui(materialEditor, isShowRim2ndBlendMask, loc["sBlend"], loc["sBlendR"], rim2ndBlendMask, rim2ndBlend, rim2ndBlendMaskScrollX, rim2ndBlendMaskScrollY, rim2ndBlendMaskAngle, rim2ndBlendMaskRotate, rim2ndBlendMaskUV, rim2ndBlendMaskTrim);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            materialEditor.ShaderProperty(rim2ndToon, loc["sToon"]);
                            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            if(rim2ndToon.floatValue == 1)
                            {
                                isShowRim2ndBorderMask = MaskTextureGui(materialEditor, isShowRim2ndBorderMask, loc["sBorder"], loc["sBorderR"], rim2ndBorderMask, rim2ndBorder, rim2ndBorderMaskScrollX, rim2ndBorderMaskScrollY, rim2ndBorderMaskAngle, rim2ndBorderMaskRotate, rim2ndBorderMaskUV, rim2ndBorderMaskTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                                isShowRim2ndBlurMask = MaskTextureGui(materialEditor, isShowRim2ndBlurMask, loc["sBlur"], loc["sBlurR"], rim2ndBlurMask, rim2ndBlur, rim2ndBlurMaskScrollX, rim2ndBlurMaskScrollY, rim2ndBlurMaskAngle, rim2ndBlurMaskRotate, rim2ndBlurMaskUV, rim2ndBlurMaskTrim);
                                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f,0.25f,0.25f,1));
                            }
                            materialEditor.ShaderProperty(rim2ndUpperSideWidth, loc["sUpperSideWidth"]);
                            materialEditor.ShaderProperty(rim2ndFresnelPower, loc["sFresnelPower"]);
                            materialEditor.ShaderProperty(rim2ndShadeMix, loc["sShadeMix"]);
                            materialEditor.ShaderProperty(rim2ndShadowMask, loc["sShadowMask"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                        */
                    }
                }
                GUILayout.Space(10);

                GUILayout.Label(loc["sAdvanced"], midashi);
                // ステンシル
                isShowStencil = Foldout(loc["sStencil"], loc["sStencilTips"], isShowStencil);
                if(isShowStencil)
                {
                    EditorGUILayout.BeginVertical(circleDark);
                    EditorGUILayout.BeginVertical(circleGray);
                    materialEditor.ShaderProperty(stencilRef, "Ref");
                    materialEditor.ShaderProperty(stencilComp, "Comp");
                    materialEditor.ShaderProperty(stencilPass, "Pass");
                    materialEditor.ShaderProperty(stencilFail, "Fail");
                    materialEditor.ShaderProperty(stencilZFail, "ZFail");
                    if(GUILayout.Button("Set Writer"))
                    {
                        isStWr = true;
                        stencilRef.floatValue = 1;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat("_Mode"), isFull, isLite, isStWr);
                    }
                    if(GUILayout.Button("Set Reader"))
                    {
                        isStWr = false;
                        stencilRef.floatValue = 1;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat("_Mode"), isFull, isLite, isStWr);
                    }
                    if(GUILayout.Button("Reset"))
                    {
                        isStWr = false;
                        stencilRef.floatValue = 0;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat("_Mode"), isFull, isLite, isStWr);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }
                //isShowAnimationTexture = Foldout(loc["sAnimationTexture"], loc["sAnimationTextureTips"], isShowAnimationTexture);
                // 屈折
                if(isRefr)
                {
                    isShowRefraction = Foldout(loc["sRefraction"], loc["sRefractionTips"], isShowRefraction);
                    if(isShowRefraction)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sRefractionLabel"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.ShaderProperty(refractionStrength, loc["sRefractionStrength"]);
                        materialEditor.ShaderProperty(refractionFresnelPower, loc["sRefractionFresnel"]);
                        materialEditor.ShaderProperty(refractionColorFromMain, loc["sRefractionColorFromMain"]);
                        materialEditor.ShaderProperty(refractionColor, loc["sColor"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
                // ファー
                if(isFur)
                {
                    isShowFur = Foldout(loc["sFur"], loc["sFurTips"], isShowFur);
                    if(isShowFur)
                    {
                        EditorGUILayout.BeginVertical(circleDark);
                        GUILayout.Label(loc["sFurLabel"], customToggleFont);
                        EditorGUILayout.BeginVertical(halfcircleGray);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sFurNoise"], loc["sFurNoiseR"]), furNoiseMask);
                        materialEditor.TextureScaleOffsetProperty(furNoiseMask);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sAlpha"], loc["sAlphaR"]), furMask);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sNormalMap"], loc["sNormalRGB"]), furVectorTex,furVectorScale);
                        Vector3Gui(materialEditor, furVectorX, furVectorY, furVectorZ);
                        materialEditor.ShaderProperty(furLength, loc["sFurLength"]);
                        materialEditor.ShaderProperty(furGravity, loc["sFurGravity"]);
                        materialEditor.ShaderProperty(furAO, loc["sFurAO"]);
                        materialEditor.ShaderProperty(vertexColor2FurVector, loc["sVertexColor2FurVector"]);
                        materialEditor.ShaderProperty(furLayerNum, loc["sFurLayerNum"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
                isShowRendering = Foldout(loc["sRendering"], loc["sRenderingTips"], isShowRendering);
                if(isShowRendering)
                {
                    EditorGUILayout.BeginVertical(circleDark);
                    EditorGUILayout.BeginVertical(circleGray);
                    // シェーダーの種類選択 Fullとか普通使わないので目立たないこっちに移動
                    int shaderType = 0;
                    if(isFull) shaderType = 1;
                    if(isLite) shaderType = 2;
                    int shaderTypeBuf = shaderType;
                    shaderType = EditorGUILayout.Popup(loc["sShaderType"],shaderType,new String[]{loc["sShaderTypeNormal"],loc["sShaderTypeFull"],loc["sShaderTypeLite"]});
                    if(shaderTypeBuf != shaderType)
                    {
                        if(shaderType==0) {isFull = false; isLite = false;}
                        if(shaderType==1) {isFull = true;  isLite = false;}
                        if(shaderType==2) {isFull = false; isLite = true;}
                        SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat("_Mode"), isFull, isLite, isStWr);
                    }
                    if(isFull)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpShaderFull"],MessageType.Warning);
                    }
                    // レンダリング方法
                    materialEditor.ShaderProperty(zwrite, loc["sZWrite"]);
                    materialEditor.ShaderProperty(ztest, loc["sZTest"]);
                    materialEditor.ShaderProperty(srcBlend, loc["sSrcBlend"]);
                    materialEditor.ShaderProperty(dstBlend, loc["sDstBlend"]);
                    materialEditor.EnableInstancingField();
                    materialEditor.RenderQueueField();
                    materialEditor.ShaderProperty(useVertexColor, loc["sVertexColor"]);
                    if(GUILayout.Button(loc["sRenderingReset"]))
                    {
                        zwrite.floatValue = 1.0f;
                        ztest.floatValue = 4.0f;
                        material.enableInstancing = false;
                        SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat("_Mode"), isFull, isLite, isStWr);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
            }
	    }

        public int selectLang(int lnum)
        {
            string[] langGuid = AssetDatabase.FindAssets("lang_", new[] {"Assets/lil's Toon Shader/Editor"});
            string[] langName = new string[langGuid.Length];
            for(int i=0;i<langGuid.Length;i++)
            {
                string langBuf = (AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(langGuid[i]), typeof(TextAsset)) as TextAsset).text;
                langName[i] = langBuf.Substring(0,langBuf.IndexOf("\n"));
            }
            int outnum = EditorGUILayout.Popup("Language", lnum, langName);
            string langRaw = (AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(langGuid[outnum]), typeof(TextAsset)) as TextAsset).text;
            string[] langData = langRaw.Split(char.Parse("\n"));
            for(int ii = 0; ii < langData.Length; ii++)
            {
                string line = langData[ii];
                if(line.Contains("string"))
                {
                    string name = line.Substring(line.IndexOf("string")+6,line.IndexOf("=")-(line.IndexOf("string")+7));
                    name = name.Trim();
                    string localized = line.Substring(line.IndexOf("=")+1,line.IndexOf(";")-(line.IndexOf("=")+1));
                    localized = localized.Trim();
                    localized = localized.Trim('"');
                    loc[name] = localized;
                }
            }
            return outnum;
        }

        public void Vector3Gui(MaterialEditor materialEditor, MaterialProperty vX, MaterialProperty vY, MaterialProperty vZ)
        {
            Vector3 vec = new Vector3(vX.floatValue, vY.floatValue, vZ.floatValue);
            vec = makeVec3Label(vec);
            vX.floatValue = vec.x;
            vY.floatValue = vec.y;
            vZ.floatValue = vec.z;
        }

        public bool TextureGui(MaterialEditor materialEditor, bool isShow, string menuname, string helpText, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollX, MaterialProperty scrollY, MaterialProperty angle, MaterialProperty rotate, MaterialProperty UVSet, MaterialProperty texTrim)
        {
            EditorGUI.indentLevel++;
            var rect = materialEditor.TexturePropertySingleLine(new GUIContent(menuname, helpText), textureName, rgba);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if (isShow)
            {
                EditorGUI.indentLevel++;
                materialEditor.TextureScaleOffsetProperty(textureName);
                Vector2 scrMain = new Vector2(scrollX.floatValue, scrollY.floatValue);
                scrMain = makeScrLabel(scrMain);
                scrollX.floatValue = scrMain.x;
                scrollY.floatValue = scrMain.y;
                materialEditor.ShaderProperty(angle, loc["sAngle"]);
                materialEditor.ShaderProperty(rotate, loc["sRotate"]);
                UVSet.floatValue = (float)EditorGUILayout.Popup(loc["sUVSet"],(int)UVSet.floatValue,new string[]{"UV0","UV1","UV2","UV3","Skybox"});
                materialEditor.ShaderProperty(texTrim, loc["sTrim"]);

                EditorGUI.indentLevel--;
            }
            return isShow;
        }

        public bool MaskTextureGui(MaterialEditor materialEditor, bool isShow, string menuname, string helpText, MaterialProperty textureName, MaterialProperty strength, MaterialProperty scrollX, MaterialProperty scrollY, MaterialProperty angle, MaterialProperty rotate, MaterialProperty UVSet, MaterialProperty texTrim)
        {
            EditorGUI.indentLevel++;
            var rect = materialEditor.TexturePropertySingleLine(new GUIContent(menuname, helpText), textureName, strength);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if (isShow)
            {
                EditorGUI.indentLevel++;
                materialEditor.TextureScaleOffsetProperty(textureName);
                Vector2 scrMain = new Vector2(scrollX.floatValue, scrollY.floatValue);
                scrMain = makeScrLabel(scrMain);
                scrollX.floatValue = scrMain.x;
                scrollY.floatValue = scrMain.y;
                materialEditor.ShaderProperty(angle, loc["sAngle"]);
                materialEditor.ShaderProperty(rotate, loc["sRotate"]);
                UVSet.floatValue = (float)EditorGUILayout.Popup(loc["sUVSet"],(int)UVSet.floatValue,new string[]{"UV0","UV1","UV2","UV3","Skybox"});
                materialEditor.ShaderProperty(texTrim, loc["sTrim"]);

                EditorGUI.indentLevel--;
            }
            return isShow;
        }

        public bool NormalTextureGui(MaterialEditor materialEditor, bool isShow, string menuname, string helpText, MaterialProperty textureName, MaterialProperty scrollX, MaterialProperty scrollY, MaterialProperty angle, MaterialProperty rotate, MaterialProperty UVSet, MaterialProperty texTrim)
        {
            EditorGUI.indentLevel++;
            var rect = materialEditor.TexturePropertySingleLine(new GUIContent(menuname, helpText), textureName);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if (isShow)
            {
                EditorGUI.indentLevel++;
                materialEditor.TextureScaleOffsetProperty(textureName);
                Vector2 scrMain = new Vector2(scrollX.floatValue, scrollY.floatValue);
                scrMain = makeScrLabel(scrMain);
                scrollX.floatValue = scrMain.x;
                scrollY.floatValue = scrMain.y;
                materialEditor.ShaderProperty(angle, loc["sAngle"]);
                materialEditor.ShaderProperty(rotate, loc["sRotate"]);
                UVSet.floatValue = (float)EditorGUILayout.Popup(loc["sUVSet"],(int)UVSet.floatValue,new string[]{"UV0","UV1","UV2","UV3","Skybox"});
                materialEditor.ShaderProperty(texTrim, loc["sTrim"]);

                EditorGUI.indentLevel--;
            }
            return isShow;
        }

        public bool MatcapTextureGui(MaterialEditor materialEditor, bool isShow, string menuname, string helpText, MaterialProperty textureName, MaterialProperty rgba)
        {
            EditorGUI.indentLevel++;
            var rect = materialEditor.TexturePropertySingleLine(new GUIContent(menuname, helpText), textureName, rgba);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if (isShow)
            {
                EditorGUI.indentLevel++;
                materialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
            return isShow;
        }

        public void gradientEditor(Material material, string eminame, Gradient ingrad, string texname)
        {
            ingrad = gradientFromMat(material, eminame);
            ingrad = EditorGUILayout.GradientField(loc["sGradColor"], ingrad);
            gradientToMat(material, eminame, ingrad);
            Texture2D tex = MakeGradTexture(ingrad);
            if(GUILayout.Button("Save"))
            {
                tex = SaveTextureToPng(material, tex, texname);
                material.SetTexture(texname, tex);
            }
            string gradtexpath = AssetDatabase.GetAssetPath(material.GetTexture(texname));
            if(gradtexpath.Length > 0) GUILayout.Label(gradtexpath);
        }

        public Gradient gradientFromMat(Material material, string eminame)
        {
            Gradient outgrad = new Gradient();
            GradientColorKey[] cK = new GradientColorKey[material.GetInt(eminame + "ci")];
            GradientAlphaKey[] aK = new GradientAlphaKey[material.GetInt(eminame + "ai")];
            for(int ic=0;ic<cK.Length;ic++)
            {
                cK[ic].color = material.GetColor(eminame + "c" + ic.ToString());
                cK[ic].time = material.GetColor(eminame + "c" + ic.ToString()).a;
            }
            for(int ia=0;ia<aK.Length;ia++)
            {
                aK[ia].alpha = material.GetColor(eminame + "a" + ia.ToString()).r;
                aK[ia].time = material.GetColor(eminame + "a" + ia.ToString()).a;
            }
            outgrad.SetKeys(cK, aK);
            return outgrad;
        }

        public void gradientToMat(Material material, string eminame, Gradient ingrad)
        {
            material.SetInt(eminame + "ci", ingrad.colorKeys.Length);
            material.SetInt(eminame + "ai", ingrad.alphaKeys.Length);
            for(int ic=0;ic<ingrad.colorKeys.Length;ic++)
            {
                material.SetColor(eminame + "c" + ic.ToString(), new Color(ingrad.colorKeys[ic].color.r, ingrad.colorKeys[ic].color.g, ingrad.colorKeys[ic].color.b, ingrad.colorKeys[ic].time));
            }
            for(int ia=0;ia<ingrad.alphaKeys.Length;ia++)
            {
                material.SetColor(eminame + "a" + ia.ToString(), new Color(ingrad.alphaKeys[ia].alpha, 0, 0, ingrad.alphaKeys[ia].time));
            }
        }

        public Texture2D MakeGradTexture(Gradient grad)
        {
            Texture2D tex = new Texture2D(128, 1);
            for (int w = 0; w < tex.width; w++)
            {
                tex.SetPixel(w, 0, grad.Evaluate((float)w / tex.width));
            }

            tex.Apply();
            tex.wrapMode = TextureWrapMode.Repeat;
            return tex;
        }

        public Texture2D SaveTextureToPng(Material material, Texture2D tex, string texname)
        {
            string texpath = AssetDatabase.GetAssetPath(material.GetTexture(texname));

            byte [] pngData = tex.EncodeToPNG();
            string path = "";
            if(texpath.Length > 1)  path = EditorUtility.SaveFilePanel("Save Texture", texpath, "", "png");
            else                    path = EditorUtility.SaveFilePanel("Save Texture", "Assets", tex.name + ".png", "png");
            if (path.Length > 0) {
                File.WriteAllBytes(path, pngData);
                path = path.Substring(path.IndexOf("Assets"));
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            }
            else
            {
                return (Texture2D)material.GetTexture(texname);
            }
        }

        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, bool isfull, bool islite, bool isstencil)
        {
            // シェーダーをロード
            Shader lts      = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/lts.shader", typeof(Shader)) as Shader;
            Shader ltsc     = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/lts_cutout.shader", typeof(Shader)) as Shader;
            Shader ltst     = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/lts_trans.shader", typeof(Shader)) as Shader;
            Shader ltsw     = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/lts_sw.shader", typeof(Shader)) as Shader;
            Shader ltscw    = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/lts_cutout_sw.shader", typeof(Shader)) as Shader;
            Shader ltstw    = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/lts_trans_sw.shader", typeof(Shader)) as Shader;
            Shader ltsref   = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/lts_ref.shader", typeof(Shader)) as Shader;
            Shader ltsfur   = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/lts_fur.shader", typeof(Shader)) as Shader;
            Shader ltsf     = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsf.shader", typeof(Shader)) as Shader;
            Shader ltsfc    = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsf_cutout.shader", typeof(Shader)) as Shader;
            Shader ltsft    = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsf_trans.shader", typeof(Shader)) as Shader;
            Shader ltsfw    = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsf_sw.shader", typeof(Shader)) as Shader;
            Shader ltsfcw   = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsf_cutout_sw.shader", typeof(Shader)) as Shader;
            Shader ltsftw   = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsf_trans_sw.shader", typeof(Shader)) as Shader;
            Shader ltsl     = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsl.shader", typeof(Shader)) as Shader;
            Shader ltslc    = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsl_cutout.shader", typeof(Shader)) as Shader;
            Shader ltslt    = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsl_trans.shader", typeof(Shader)) as Shader;
            Shader ltslw    = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsl_sw.shader", typeof(Shader)) as Shader;
            Shader ltslcw   = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsl_cutout_sw.shader", typeof(Shader)) as Shader;
            Shader ltsltw   = AssetDatabase.LoadAssetAtPath("Assets/lil's Toon Shader/Shader/ltsl_trans_sw.shader", typeof(Shader)) as Shader;
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    if(isstencil && isfull)         material.shader = ltsfw;
                    else if(isstencil && islite)    material.shader = ltslw;
                    else if(isstencil)              material.shader = ltsw;
                    else if(isfull)                 material.shader = ltsf;
                    else if(islite)                 material.shader = ltsl;
                    else                            material.shader = lts;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case RenderingMode.Cutout:
                    if(isstencil && isfull)         material.shader = ltsfcw;
                    else if(isstencil && islite)    material.shader = ltslcw;
                    else if(isstencil)              material.shader = ltscw;
                    else if(isfull)                 material.shader = ltsfc;
                    else if(islite)                 material.shader = ltslc;
                    else                            material.shader = ltsc;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case RenderingMode.Transparent:
                    if(isstencil && isfull)         material.shader = ltsftw;
                    else if(isstencil && islite)    material.shader = ltsltw;
                    else if(isstencil)              material.shader = ltstw;
                    else if(isfull)                 material.shader = ltsft;
                    else if(islite)                 material.shader = ltslt;
                    else                            material.shader = ltst;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
                case RenderingMode.Add:
                    if(isstencil && isfull)         material.shader = ltsftw;
                    else if(isstencil && islite)    material.shader = ltsltw;
                    else if(isstencil)              material.shader = ltstw;
                    else if(isfull)                 material.shader = ltsft;
                    else if(islite)                 material.shader = ltslt;
                    else                            material.shader = ltst;
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    break;
                case RenderingMode.Multiply:
                    if(isstencil && isfull)         material.shader = ltsftw;
                    else if(isstencil && islite)    material.shader = ltsltw;
                    else if(isstencil)              material.shader = ltstw;
                    else if(isfull)                 material.shader = ltsft;
                    else if(islite)                 material.shader = ltslt;
                    else                            material.shader = ltst;
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.DstColor);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case RenderingMode.Refraction:
                    material.shader = ltsref;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
                case RenderingMode.Fur:
                    material.shader = ltsfur;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
            }
        }

        public static void DeleteShaderKeyword(Material material)
        {
            // キーワード全削除
            var keywords = new List<string>(material.shaderKeywords);
            keywords.ForEach(keyword => material.DisableKeyword(keyword));
        }
    }

    public class lilToggleDrawer : MaterialPropertyDrawer
    {
        // shader keywordをセットしないToggle
        // [lilToggle]をプロパティ前に追加
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            bool value = (prop.floatValue != 0.0f);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUI.Toggle(position, label, value);
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value ? 1.0f : 0.0f;
            }
        }
    }
}
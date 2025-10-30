Shader "Hidden/lilToonMultiRefraction"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Dummy
        _DummyProperty ("If you are seeing this, some script is broken.", Float) = 0
        _DummyProperty ("This also happens if something other than lilToon is broken.", Float) = 0
        _DummyProperty ("You need to check the error on the console and take appropriate action, such as reinstalling the relevant tool.", Float) = 0
        _DummyProperty (" ", Float) = 0
        _DummyProperty ("これが表示されている場合、なんらかのスクリプトが壊れています。", Float) = 0
        _DummyProperty ("これはlilToon以外のものが壊れている場合にも発生します。", Float) = 0
        _DummyProperty ("コンソールでエラーを確認し、該当するツールを入れ直すなどの対処を行う必要があります。", Float) = 0
        [Space(1000)]
        _DummyProperty ("", Float) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle]     _Invisible                  ("sInvisible", Int) = 0
                        _AsUnlit                    ("sAsUnlit", Range(0, 1)) = 0
                        _Cutoff                     ("sCutoff", Range(-0.001,1.001)) = 0.5
                        _SubpassCutoff              ("sSubpassCutoff", Range(0,1)) = 0.5
        [lilToggle]     _FlipNormal                 ("sFlipBackfaceNormal", Int) = 0
        [lilToggle]     _ShiftBackfaceUV            ("sShiftBackfaceUV", Int) = 0
                        _BackfaceForceShadow        ("sBackfaceForceShadow", Range(0,1)) = 0
        [lilHDR]        _BackfaceColor              ("sColor", Color) = (0,0,0,0)
                        _VertexLightStrength        ("sVertexLightStrength", Range(0,1)) = 0
                        _LightMinLimit              ("sLightMinLimit", Range(0,1)) = 0.05
                        _LightMaxLimit              ("sLightMaxLimit", Range(0,10)) = 1
                        _BeforeExposureLimit        ("sBeforeExposureLimit", Float) = 10000
                        _MonochromeLighting         ("sMonochromeLighting", Range(0,1)) = 0
                        _AlphaBoostFA               ("sAlphaBoostFA", Range(1,100)) = 10
                        _lilDirectionalLightStrength ("sDirectionalLightStrength", Range(0,1)) = 1
        [lilVec3B]      _LightDirectionOverride     ("sLightDirectionOverrides", Vector) = (0.001,0.002,0.001,0)
                        _AAStrength                 ("sAAShading", Range(0, 1)) = 1
        [lilToggle]     _UseDither                  ("sDither", Int) = 0
        [NoScaleOffset] _DitherTex                  ("Dither", 2D) = "white" {}
                        _DitherMaxValue             ("Max Value", Float) = 255
                        _EnvRimBorder               ("[VRCLV] Rim Border", Range(0, 3)) = 3.0
                        _EnvRimBlur                 ("[VRCLV] Rim Blur", Range(0, 1)) = 0.35

        //----------------------------------------------------------------------------------------------------------------------
        // Main
        [lilHDR] [MainColor] _Color                 ("sColor", Color) = (1,1,1,1)
        [MainTexture]   _MainTex                    ("Texture", 2D) = "white" {}
        [lilUVAnim]     _MainTex_ScrollRotate       ("sScrollRotates", Vector) = (0,0,0,0)
        [lilHSVG]       _MainTexHSVG                ("sHSVGs", Vector) = (0,1,1,1)
                        _MainGradationStrength      ("Gradation Strength", Range(0, 1)) = 0
        [NoScaleOffset] _MainGradationTex           ("Gradation Map", 2D) = "white" {}
        [NoScaleOffset] _MainColorAdjustMask        ("Adjust Mask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // Main2nd
        [lilToggleLeft] _UseMain2ndTex              ("sMainColor2nd", Int) = 0
        [lilHDR]        _Color2nd                   ("sColor", Color) = (1,1,1,1)
                        _Main2ndTex                 ("Texture", 2D) = "white" {}
        [lilAngle]      _Main2ndTexAngle            ("sAngle", Float) = 0
        [lilUVAnim]     _Main2ndTex_ScrollRotate    ("sScrollRotates", Vector) = (0,0,0,0)
        [lilEnum]       _Main2ndTex_UVMode          ("UV Mode|UV0|UV1|UV2|UV3|MatCap", Int) = 0
        [lilEnum]       _Main2ndTex_Cull            ("sCullModes", Int) = 0
        [lilDecalAnim]  _Main2ndTexDecalAnimation   ("sDecalAnimations", Vector) = (1,1,1,30)
        [lilDecalSub]   _Main2ndTexDecalSubParam    ("sDecalSubParams", Vector) = (1,1,0,1)
        [lilToggle]     _Main2ndTexIsDecal          ("sAsDecal", Int) = 0
        [lilToggle]     _Main2ndTexIsLeftOnly       ("Left Only", Int) = 0
        [lilToggle]     _Main2ndTexIsRightOnly      ("Right Only", Int) = 0
        [lilToggle]     _Main2ndTexShouldCopy       ("Copy", Int) = 0
        [lilToggle]     _Main2ndTexShouldFlipMirror ("Flip Mirror", Int) = 0
        [lilToggle]     _Main2ndTexShouldFlipCopy   ("Flip Copy", Int) = 0
        [lilToggle]     _Main2ndTexIsMSDF           ("sAsMSDF", Int) = 0
        [NoScaleOffset] _Main2ndBlendMask           ("Mask", 2D) = "white" {}
        [lilEnum]       _Main2ndTexBlendMode        ("sBlendModes", Int) = 0
        [lilEnum]       _Main2ndTexAlphaMode        ("sAlphaModes", Int) = 0
                        _Main2ndEnableLighting      ("sEnableLighting", Range(0, 1)) = 1
                        _Main2ndDissolveMask        ("Dissolve Mask", 2D) = "white" {}
                        _Main2ndDissolveNoiseMask   ("Dissolve Noise Mask", 2D) = "gray" {}
        [lilUVAnim]     _Main2ndDissolveNoiseMask_ScrollRotate ("Scroll", Vector) = (0,0,0,0)
                        _Main2ndDissolveNoiseStrength ("Dissolve Noise Strength", float) = 0.1
        [lilHDR]        _Main2ndDissolveColor       ("sColor", Color) = (1,1,1,1)
        [lilDissolve]   _Main2ndDissolveParams      ("sDissolveParams", Vector) = (0,0,0.5,0.1)
        [lilDissolveP]  _Main2ndDissolvePos         ("Dissolve Position", Vector) = (0,0,0,0)
        [lilFFFB]       _Main2ndDistanceFade        ("sDistanceFadeSettings", Vector) = (0.1,0.01,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Main3rd
        [lilToggleLeft] _UseMain3rdTex              ("sMainColor3rd", Int) = 0
        [lilHDR]        _Color3rd                   ("sColor", Color) = (1,1,1,1)
                        _Main3rdTex                 ("Texture", 2D) = "white" {}
        [lilAngle]      _Main3rdTexAngle            ("sAngle", Float) = 0
        [lilUVAnim]     _Main3rdTex_ScrollRotate    ("sScrollRotates", Vector) = (0,0,0,0)
        [lilEnum]       _Main3rdTex_UVMode          ("UV Mode|UV0|UV1|UV2|UV3|MatCap", Int) = 0
        [lilEnum]       _Main3rdTex_Cull            ("sCullModes", Int) = 0
        [lilDecalAnim]  _Main3rdTexDecalAnimation   ("sDecalAnimations", Vector) = (1,1,1,30)
        [lilDecalSub]   _Main3rdTexDecalSubParam    ("sDecalSubParams", Vector) = (1,1,0,1)
        [lilToggle]     _Main3rdTexIsDecal          ("sAsDecal", Int) = 0
        [lilToggle]     _Main3rdTexIsLeftOnly       ("Left Only", Int) = 0
        [lilToggle]     _Main3rdTexIsRightOnly      ("Right Only", Int) = 0
        [lilToggle]     _Main3rdTexShouldCopy       ("Copy", Int) = 0
        [lilToggle]     _Main3rdTexShouldFlipMirror ("Flip Mirror", Int) = 0
        [lilToggle]     _Main3rdTexShouldFlipCopy   ("Flip Copy", Int) = 0
        [lilToggle]     _Main3rdTexIsMSDF           ("sAsMSDF", Int) = 0
        [NoScaleOffset] _Main3rdBlendMask           ("Mask", 2D) = "white" {}
        [lilEnum]       _Main3rdTexBlendMode        ("sBlendModes", Int) = 0
        [lilEnum]       _Main3rdTexAlphaMode        ("sAlphaModes", Int) = 0
                        _Main3rdEnableLighting      ("sEnableLighting", Range(0, 1)) = 1
                        _Main3rdDissolveMask        ("Dissolve Mask", 2D) = "white" {}
                        _Main3rdDissolveNoiseMask   ("Dissolve Noise Mask", 2D) = "gray" {}
        [lilUVAnim]     _Main3rdDissolveNoiseMask_ScrollRotate ("Scroll", Vector) = (0,0,0,0)
                        _Main3rdDissolveNoiseStrength ("Dissolve Noise Strength", float) = 0.1
        [lilHDR]        _Main3rdDissolveColor       ("sColor", Color) = (1,1,1,1)
        [lilDissolve]   _Main3rdDissolveParams      ("sDissolveParams", Vector) = (0,0,0.5,0.1)
        [lilDissolveP]  _Main3rdDissolvePos         ("Dissolve Position", Vector) = (0,0,0,0)
        [lilFFFB]       _Main3rdDistanceFade        ("sDistanceFadeSettings", Vector) = (0.1,0.01,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha Mask
        [lilEnumLabel]  _AlphaMaskMode              ("sAlphaMaskModes", Int) = 0
                        _AlphaMask                  ("AlphaMask", 2D) = "white" {}
                        _AlphaMaskScale             ("Scale", Float) = 1
                        _AlphaMaskValue             ("Offset", Float) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap
        [lilToggleLeft] _UseBumpMap                 ("sNormalMap", Int) = 0
        [Normal]        _BumpMap                    ("Normal Map", 2D) = "bump" {}
                        _BumpScale                  ("Scale", Range(-10,10)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap 2nd
        [lilToggleLeft] _UseBump2ndMap              ("sNormalMap2nd", Int) = 0
        [Normal]        _Bump2ndMap                 ("Normal Map", 2D) = "bump" {}
        [lilEnum]       _Bump2ndMap_UVMode          ("UV Mode|UV0|UV1|UV2|UV3", Int) = 0
                        _Bump2ndScale               ("Scale", Range(-10,10)) = 1
        [NoScaleOffset] _Bump2ndScaleMask           ("Mask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // Anisotropy
        [lilToggleLeft] _UseAnisotropy              ("sAnisotropy", Int) = 0
        [Normal]        _AnisotropyTangentMap       ("Tangent Map", 2D) = "bump" {}
                        _AnisotropyScale            ("Scale", Range(-1,1)) = 1
        [NoScaleOffset] _AnisotropyScaleMask        ("Scale Mask", 2D) = "white" {}
                        _AnisotropyTangentWidth     ("sTangentWidth", Range(0,10)) = 1
                        _AnisotropyBitangentWidth   ("sBitangentWidth", Range(0,10)) = 1
                        _AnisotropyShift            ("sOffset", Range(-10,10)) = 0
                        _AnisotropyShiftNoiseScale  ("sNoiseStrength", Range(-1,1)) = 0
                        _AnisotropySpecularStrength ("sStrength", Range(0,10)) = 1
                        _Anisotropy2ndTangentWidth  ("sTangentWidth", Range(0,10)) = 1
                        _Anisotropy2ndBitangentWidth ("sBitangentWidth", Range(0,10)) = 1
                        _Anisotropy2ndShift         ("sOffset", Range(-10,10)) = 0
                        _Anisotropy2ndShiftNoiseScale ("sNoiseStrength", Range(-1,1)) = 0
                        _Anisotropy2ndSpecularStrength ("sStrength", Range(0,10)) = 0
                        _AnisotropyShiftNoiseMask   ("sNoise", 2D) = "white" {}
        [lilToggle]     _Anisotropy2Reflection      ("sReflection", Int) = 0
        [lilToggle]     _Anisotropy2MatCap          ("sMatCap", Int) = 0
        [lilToggle]     _Anisotropy2MatCap2nd       ("sMatCap2nd", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Backlight
        [lilToggleLeft] _UseBacklight               ("sBacklight", Int) = 0
        [lilHDR]        _BacklightColor             ("sColor", Color) = (0.85,0.8,0.7,1.0)
        [NoScaleOffset] _BacklightColorTex          ("Texture", 2D) = "white" {}
                        _BacklightMainStrength      ("sMainColorPower", Range(0, 1)) = 0
                        _BacklightNormalStrength    ("sNormalStrength", Range(0, 1)) = 1.0
                        _BacklightBorder            ("Border", Range(0, 1)) = 0.35
                        _BacklightBlur              ("sBlur", Range(0, 1)) = 0.05
                        _BacklightDirectivity       ("sDirectivity", Float) = 5.0
                        _BacklightViewStrength      ("sViewDirectionStrength", Range(0, 1)) = 1
        [lilToggle]     _BacklightReceiveShadow     ("sReceiveShadow", Int) = 1
        [lilToggle]     _BacklightBackfaceMask      ("sBackfaceMask", Int) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggleLeft] _UseShadow                  ("sShadow", Int) = 0
                        _ShadowStrength             ("sStrength", Range(0, 1)) = 1
        [NoScaleOffset] _ShadowStrengthMask         ("sStrength", 2D) = "white" {}
        [lilLOD]        _ShadowStrengthMaskLOD      ("LOD", Range(0, 1)) = 0
        [NoScaleOffset] _ShadowBorderMask           ("sBorder", 2D) = "white" {}
        [lilLOD]        _ShadowBorderMaskLOD        ("LOD", Range(0, 1)) = 0
        [NoScaleOffset] _ShadowBlurMask             ("sBlur", 2D) = "white" {}
        [lilLOD]        _ShadowBlurMaskLOD          ("LOD", Range(0, 1)) = 0
        [lilFFFF]       _ShadowAOShift              ("1st Scale|1st Offset|2nd Scale|2nd Offset", Vector) = (1,0,1,0)
        [lilFF]         _ShadowAOShift2             ("3rd Scale|3rd Offset", Vector) = (1,0,1,0)
        [lilToggle]     _ShadowPostAO               ("sIgnoreBorderProperties", Int) = 0
        [lilEnum]       _ShadowColorType            ("sShadowColorTypes", Int) = 0
                        _ShadowColor                ("Shadow Color", Color) = (0.82,0.76,0.85,1.0)
        [NoScaleOffset] _ShadowColorTex             ("Shadow Color", 2D) = "black" {}
                        _ShadowNormalStrength       ("sNormalStrength", Range(0, 1)) = 1.0
                        _ShadowBorder               ("sBorder", Range(0, 1)) = 0.5
                        _ShadowBlur                 ("sBlur", Range(0, 1)) = 0.1
                        _ShadowReceive              ("sReceiveShadow", Range(0, 1)) = 0
                        _Shadow2ndColor             ("2nd Color", Color) = (0.68,0.66,0.79,1)
        [NoScaleOffset] _Shadow2ndColorTex          ("2nd Color", 2D) = "black" {}
                        _Shadow2ndNormalStrength    ("sNormalStrength", Range(0, 1)) = 1.0
                        _Shadow2ndBorder            ("sBorder", Range(0, 1)) = 0.15
                        _Shadow2ndBlur              ("sBlur", Range(0, 1)) = 0.1
                        _Shadow2ndReceive           ("sReceiveShadow", Range(0, 1)) = 0
                        _Shadow3rdColor             ("3rd Color", Color) = (0,0,0,0)
        [NoScaleOffset] _Shadow3rdColorTex          ("3rd Color", 2D) = "black" {}
                        _Shadow3rdNormalStrength    ("sNormalStrength", Range(0, 1)) = 1.0
                        _Shadow3rdBorder            ("sBorder", Range(0, 1)) = 0.25
                        _Shadow3rdBlur              ("sBlur", Range(0, 1)) = 0.1
                        _Shadow3rdReceive           ("sReceiveShadow", Range(0, 1)) = 0
                        _ShadowBorderColor          ("sShadowBorderColor", Color) = (1,0.1,0,1)
                        _ShadowBorderRange          ("sShadowBorderRange", Range(0, 1)) = 0.08
                        _ShadowMainStrength         ("sContrast", Range(0, 1)) = 0
                        _ShadowEnvStrength          ("sShadowEnvStrength", Range(0, 1)) = 0
        [lilEnum]       _ShadowMaskType             ("sShadowMaskTypes", Int) = 0
                        _ShadowFlatBorder           ("sBorder", Range(-2, 2)) = 1
                        _ShadowFlatBlur             ("sBlur", Range(0.001, 2)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Rim Shade
        [lilToggleLeft] _UseRimShade                ("RimShade", Int) = 0
                        _RimShadeColor              ("sColor", Color) = (0.5,0.5,0.5,1.0)
        [NoScaleOffset] _RimShadeMask               ("Mask", 2D) = "white" {}
                        _RimShadeNormalStrength     ("sNormalStrength", Range(0, 1)) = 1.0
                        _RimShadeBorder             ("sBorder", Range(0, 1)) = 0.5
                        _RimShadeBlur               ("sBlur", Range(0, 1)) = 1.0
        [PowerSlider(3.0)]_RimShadeFresnelPower     ("sFresnelPower", Range(0.01, 50)) = 1.0

        //----------------------------------------------------------------------------------------------------------------------
        // Reflection
        [lilToggleLeft] _UseReflection              ("sReflection", Int) = 0
        // Smoothness
                        _Smoothness                 ("Smoothness", Range(0, 1)) = 1
        [NoScaleOffset] _SmoothnessTex              ("Smoothness", 2D) = "white" {}
        // Metallic
        [Gamma]         _Metallic                   ("Metallic", Range(0, 1)) = 0
        [NoScaleOffset] _MetallicGlossMap           ("Metallic", 2D) = "white" {}
        // Reflectance
        [Gamma]         _Reflectance                ("sReflectance", Range(0, 1)) = 0.04
        // Reflection
                        _GSAAStrength               ("GSAA", Range(0, 1)) = 0
        [lilToggle]     _ApplySpecular              ("Apply Specular", Int) = 1
        [lilToggle]     _ApplySpecularFA            ("sMultiLightSpecular", Int) = 1
        [lilToggle]     _SpecularToon               ("Specular Toon", Int) = 1
                        _SpecularNormalStrength     ("sNormalStrength", Range(0, 1)) = 1.0
                        _SpecularBorder             ("sBorder", Range(0, 1)) = 0.5
                        _SpecularBlur               ("sBlur", Range(0, 1)) = 0.0
        [lilToggle]     _ApplyReflection            ("sApplyReflection", Int) = 0
                        _ReflectionNormalStrength   ("sNormalStrength", Range(0, 1)) = 1.0
        [lilHDR]        _ReflectionColor            ("sColor", Color) = (1,1,1,1)
        [NoScaleOffset] _ReflectionColorTex         ("sColor", 2D) = "white" {}
        [lilToggle]     _ReflectionApplyTransparency ("sApplyTransparency", Int) = 1
        [NoScaleOffset] _ReflectionCubeTex          ("Cubemap Fallback", Cube) = "black" {}
        [lilHDR]        _ReflectionCubeColor        ("sColor", Color) = (0,0,0,1)
        [lilToggle]     _ReflectionCubeOverride     ("Override", Int) = 0
                        _ReflectionCubeEnableLighting ("sEnableLighting+ (Fallback)", Range(0, 1)) = 1
        [lilEnum]       _ReflectionBlendMode        ("sBlendModes", Int) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        [lilToggleLeft] _UseMatCap                  ("sMatCap", Int) = 0
        [lilHDR]        _MatCapColor                ("sColor", Color) = (1,1,1,1)
                        _MatCapTex                  ("Texture", 2D) = "white" {}
                        _MatCapMainStrength         ("sMainColorPower", Range(0, 1)) = 0
        [lilVec2R]      _MatCapBlendUV1             ("sBlendUV1", Vector) = (0,0,0,0)
        [lilToggle]     _MatCapZRotCancel           ("sMatCapZRotCancel", Int) = 1
        [lilToggle]     _MatCapPerspective          ("sFixPerspective", Int) = 1
                        _MatCapVRParallaxStrength   ("sVRParallaxStrength", Range(0, 1)) = 1
                        _MatCapBlend                ("Blend", Range(0, 1)) = 1
        [NoScaleOffset] _MatCapBlendMask            ("Mask", 2D) = "white" {}
                        _MatCapEnableLighting       ("sEnableLighting", Range(0, 1)) = 1
                        _MatCapShadowMask           ("sShadowMask", Range(0, 1)) = 0
        [lilToggle]     _MatCapBackfaceMask         ("sBackfaceMask", Int) = 0
                        _MatCapLod                  ("sBlur", Range(0, 10)) = 0
        [lilEnum]       _MatCapBlendMode            ("sBlendModes", Int) = 1
        [lilToggle]     _MatCapApplyTransparency    ("sApplyTransparency", Int) = 1
                        _MatCapNormalStrength       ("sNormalStrength", Range(0, 1)) = 1.0
        [lilToggle]     _MatCapCustomNormal         ("sMatCapCustomNormal", Int) = 0
        [Normal]        _MatCapBumpMap              ("Normal Map", 2D) = "bump" {}
                        _MatCapBumpScale            ("Scale", Range(-10,10)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap 2nd
        [lilToggleLeft] _UseMatCap2nd               ("sMatCap2nd", Int) = 0
        [lilHDR]        _MatCap2ndColor             ("sColor", Color) = (1,1,1,1)
                        _MatCap2ndTex               ("Texture", 2D) = "white" {}
                        _MatCap2ndMainStrength      ("sMainColorPower", Range(0, 1)) = 0
        [lilVec2R]      _MatCap2ndBlendUV1          ("sBlendUV1", Vector) = (0,0,0,0)
        [lilToggle]     _MatCap2ndZRotCancel        ("sMatCapZRotCancel", Int) = 1
        [lilToggle]     _MatCap2ndPerspective       ("sFixPerspective", Int) = 1
                        _MatCap2ndVRParallaxStrength ("sVRParallaxStrength", Range(0, 1)) = 1
                        _MatCap2ndBlend             ("Blend", Range(0, 1)) = 1
        [NoScaleOffset] _MatCap2ndBlendMask         ("Mask", 2D) = "white" {}
                        _MatCap2ndEnableLighting    ("sEnableLighting", Range(0, 1)) = 1
                        _MatCap2ndShadowMask        ("sShadowMask", Range(0, 1)) = 0
        [lilToggle]     _MatCap2ndBackfaceMask      ("sBackfaceMask", Int) = 0
                        _MatCap2ndLod               ("sBlur", Range(0, 10)) = 0
        [lilEnum]       _MatCap2ndBlendMode         ("sBlendModes", Int) = 1
        [lilToggle]     _MatCap2ndApplyTransparency ("sApplyTransparency", Int) = 1
                        _MatCap2ndNormalStrength    ("sNormalStrength", Range(0, 1)) = 1.0
        [lilToggle]     _MatCap2ndCustomNormal      ("sMatCapCustomNormal", Int) = 0
        [Normal]        _MatCap2ndBumpMap           ("Normal Map", 2D) = "bump" {}
                        _MatCap2ndBumpScale         ("Scale", Range(-10,10)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Rim
        [lilToggleLeft] _UseRim                     ("sRimLight", Int) = 0
        [lilHDR]        _RimColor                   ("sColor", Color) = (0.66,0.5,0.48,1)
        [NoScaleOffset] _RimColorTex                ("Texture", 2D) = "white" {}
        [lilEnum]       _RimColorTex_UVMode         ("UV Mode|UV0|UV1|UV2|UV3", Int) = 0
                        _RimMainStrength            ("sMainColorPower", Range(0, 1)) = 0
                        _RimNormalStrength          ("sNormalStrength", Range(0, 1)) = 1.0
                        _RimBorder                  ("sBorder", Range(0, 1)) = 0.5
                        _RimBlur                    ("sBlur", Range(0, 1)) = 0.65
        [PowerSlider(3.0)]_RimFresnelPower          ("sFresnelPower", Range(0.01, 50)) = 3.5
                        _RimEnableLighting          ("sEnableLighting", Range(0, 1)) = 1
                        _RimShadowMask              ("sShadowMask", Range(0, 1)) = 0.5
        [lilToggle]     _RimBackfaceMask            ("sBackfaceMask", Int) = 1
                        _RimVRParallaxStrength      ("sVRParallaxStrength", Range(0, 1)) = 1
        [lilToggle]     _RimApplyTransparency       ("sApplyTransparency", Int) = 1
                        _RimDirStrength             ("sRimLightDirection", Range(0, 1)) = 0
                        _RimDirRange                ("sRimDirectionRange", Range(-1, 1)) = 0
                        _RimIndirRange              ("sRimIndirectionRange", Range(-1, 1)) = 0
        [lilHDR]        _RimIndirColor              ("sColor", Color) = (1,1,1,1)
                        _RimIndirBorder             ("sBorder", Range(0, 1)) = 0.5
                        _RimIndirBlur               ("sBlur", Range(0, 1)) = 0.1
        [lilEnum]       _RimBlendMode               ("sBlendModes", Int) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Glitter
        [lilToggleLeft] _UseGlitter                 ("sGlitter", Int) = 0
        [lilEnum]       _GlitterUVMode              ("UV Mode|UV0|UV1", Int) = 0
        [lilHDR]        _GlitterColor               ("sColor", Color) = (1,1,1,1)
                        _GlitterColorTex            ("Texture", 2D) = "white" {}
        [lilEnum]       _GlitterColorTex_UVMode     ("UV Mode|UV0|UV1|UV2|UV3", Int) = 0
                        _GlitterMainStrength        ("sMainColorPower", Range(0, 1)) = 0
                        _GlitterNormalStrength      ("sNormalStrength", Range(0, 1)) = 1.0
                        _GlitterScaleRandomize      ("sRandomize+ (Size)", Range(0, 1)) = 0
        [lilToggle]     _GlitterApplyShape          ("Shape", Int) = 0
                        _GlitterShapeTex            ("Texture", 2D) = "white" {}
        [lilVec2]       _GlitterAtras               ("Atras", Vector) = (1,1,0,0)
        [lilToggle]     _GlitterAngleRandomize      ("sRandomize+ (+sAngle+)", Int) = 0
        [lilGlitParam1] _GlitterParams1             ("Tiling|Particle Size|Contrast", Vector) = (256,256,0.16,50)
        [lilGlitParam2] _GlitterParams2             ("sGlitterParams2", Vector) = (0.25,0,0,0)
                        _GlitterPostContrast        ("sPostContrast", Float) = 1
                        _GlitterSensitivity         ("Sensitivity", Float) = 0.25
                        _GlitterEnableLighting      ("sEnableLighting", Range(0, 1)) = 1
                        _GlitterShadowMask          ("sShadowMask", Range(0, 1)) = 0
        [lilToggle]     _GlitterBackfaceMask        ("sBackfaceMask", Int) = 0
        [lilToggle]     _GlitterApplyTransparency   ("sApplyTransparency", Int) = 1
                        _GlitterVRParallaxStrength  ("sVRParallaxStrength", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision
        [lilToggleLeft] _UseEmission                ("sEmission", Int) = 0
        [HDR][lilHDR]   _EmissionColor              ("sColor", Color) = (1,1,1,1)
                        _EmissionMap                ("Texture", 2D) = "white" {}
        [lilUVAnim]     _EmissionMap_ScrollRotate   ("sScrollRotates", Vector) = (0,0,0,0)
        [lilEnum]       _EmissionMap_UVMode         ("UV Mode|UV0|UV1|UV2|UV3|Rim", Int) = 0
                        _EmissionMainStrength       ("sMainColorPower", Range(0, 1)) = 0
                        _EmissionBlend              ("Blend", Range(0,1)) = 1
                        _EmissionBlendMask          ("Mask", 2D) = "white" {}
        [lilUVAnim]     _EmissionBlendMask_ScrollRotate ("sScrollRotates", Vector) = (0,0,0,0)
        [lilEnum]       _EmissionBlendMode          ("sBlendModes", Int) = 1
        [lilBlink]      _EmissionBlink              ("sBlinkSettings", Vector) = (0,0,3.141593,0)
        [lilToggle]     _EmissionUseGrad            ("sGradation", Int) = 0
        [NoScaleOffset] _EmissionGradTex            ("Gradation Texture", 2D) = "white" {}
                        _EmissionGradSpeed          ("Gradation Speed", Float) = 1
                        _EmissionParallaxDepth      ("sParallaxDepth", float) = 0
                        _EmissionFluorescence       ("sFluorescence", Range(0,1)) = 0
        // Gradation
        [HideInInspector] _egci ("", Int) = 2
        [HideInInspector] _egai ("", Int) = 2
        [HideInInspector] _egc0 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc1 ("", Color) = (1,1,1,1)
        [HideInInspector] _egc2 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc3 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc4 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc5 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc6 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc7 ("", Color) = (1,1,1,0)
        [HideInInspector] _ega0 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega1 ("", Color) = (1,0,0,1)
        [HideInInspector] _ega2 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega3 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega4 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega5 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega6 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega7 ("", Color) = (1,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision2nd
        [lilToggleLeft] _UseEmission2nd             ("sEmission2nd", Int) = 0
        [HDR][lilHDR]   _Emission2ndColor           ("sColor", Color) = (1,1,1,1)
                        _Emission2ndMap             ("Texture", 2D) = "white" {}
        [lilUVAnim]     _Emission2ndMap_ScrollRotate ("sScrollRotates", Vector) = (0,0,0,0)
        [lilEnum]       _Emission2ndMap_UVMode      ("UV Mode|UV0|UV1|UV2|UV3|Rim", Int) = 0
                        _Emission2ndMainStrength    ("sMainColorPower", Range(0, 1)) = 0
                        _Emission2ndBlend           ("Blend", Range(0,1)) = 1
                        _Emission2ndBlendMask       ("Mask", 2D) = "white" {}
        [lilUVAnim]     _Emission2ndBlendMask_ScrollRotate ("sScrollRotates", Vector) = (0,0,0,0)
        [lilEnum]       _Emission2ndBlendMode       ("sBlendModes", Int) = 1
        [lilBlink]      _Emission2ndBlink           ("sBlinkSettings", Vector) = (0,0,3.141593,0)
        [lilToggle]     _Emission2ndUseGrad         ("sGradation", Int) = 0
        [NoScaleOffset] _Emission2ndGradTex         ("Gradation Texture", 2D) = "white" {}
                        _Emission2ndGradSpeed       ("Gradation Speed", Float) = 1
                        _Emission2ndParallaxDepth   ("sParallaxDepth", float) = 0
                        _Emission2ndFluorescence    ("sFluorescence", Range(0,1)) = 0
        // Gradation
        [HideInInspector] _e2gci ("", Int) = 2
        [HideInInspector] _e2gai ("", Int) = 2
        [HideInInspector] _e2gc0 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc1 ("", Color) = (1,1,1,1)
        [HideInInspector] _e2gc2 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc3 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc4 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc5 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc6 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc7 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2ga0 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga1 ("", Color) = (1,0,0,1)
        [HideInInspector] _e2ga2 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga3 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga4 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga5 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga6 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga7 ("", Color) = (1,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Parallax
        [lilToggleLeft] _UseParallax                ("sParallax", Int) = 0
        [lilToggle]     _UsePOM                     ("sPOM", Int) = 0
        [NoScaleOffset] _ParallaxMap                ("Parallax Map", 2D) = "gray" {}
                        _Parallax                   ("Parallax Scale", float) = 0.02
                        _ParallaxOffset             ("sParallaxOffset", float) = 0.5

        //----------------------------------------------------------------------------------------------------------------------
        // Distance Fade
        [lilHDR]        _DistanceFadeColor          ("sColor", Color) = (0,0,0,1)
        [lilFFFB]       _DistanceFade               ("sDistanceFadeSettings", Vector) = (0.1,0.01,0,0)
        [lilEnum]       _DistanceFadeMode           ("sDistanceFadeModes", Int) = 0
        [lilHDR]        _DistanceFadeRimColor       ("sColor", Color) = (0,0,0,0)
        [PowerSlider(3.0)]_DistanceFadeRimFresnelPower ("sFresnelPower", Range(0.01, 50)) = 5.0

        //----------------------------------------------------------------------------------------------------------------------
        // AudioLink
        [lilToggleLeft] _UseAudioLink               ("sAudioLink", Int) = 0
        [lilFRFR]       _AudioLinkDefaultValue      ("Strength|Blink Strength|Blink Speed|Blink Threshold", Vector) = (0.0,0.0,2.0,0.75)
        [lilEnum]       _AudioLinkUVMode            ("sAudioLinkUVModes", Int) = 1
        [lilALUVParams] _AudioLinkUVParams          ("Scale|Offset|sAngle|Band|Bass|Low Mid|High Mid|Treble", Vector) = (0.25,0,0,0.125)
        [lilVec3]       _AudioLinkStart             ("sAudioLinkStartPosition", Vector) = (0.0,0.0,0.0,0.0)
                        _AudioLinkMask              ("Mask", 2D) = "blue" {}
        [lilUVAnim]     _AudioLinkMask_ScrollRotate ("sScrollRotates", Vector) = (0,0,0,0)
        [lilEnum]       _AudioLinkMask_UVMode       ("UV Mode|UV0|UV1|UV2|UV3", Int) = 0
        [lilToggle]     _AudioLink2Main2nd          ("sMainColor2nd", Int) = 0
        [lilToggle]     _AudioLink2Main3rd          ("sMainColor3rd", Int) = 0
        [lilToggle]     _AudioLink2Emission         ("sEmission", Int) = 0
        [lilToggle]     _AudioLink2EmissionGrad     ("sEmission+sGradation", Int) = 0
        [lilToggle]     _AudioLink2Emission2nd      ("sEmission2nd", Int) = 0
        [lilToggle]     _AudioLink2Emission2ndGrad  ("sEmission2nd+sGradation", Int) = 0
        [lilToggle]     _AudioLink2Vertex           ("sVertex", Int) = 0
        [lilEnum]       _AudioLinkVertexUVMode      ("sAudioLinkVertexUVModes", Int) = 1
        [lilALUVParams] _AudioLinkVertexUVParams    ("Scale|Offset|sAngle|Band|Bass|Low Mid|High Mid|Treble", Vector) = (0.25,0,0,0.125)
        [lilVec3]       _AudioLinkVertexStart       ("sAudioLinkStartPosition", Vector) = (0.0,0.0,0.0,0.0)
        [lilVec3Float]  _AudioLinkVertexStrength    ("sAudioLinkVertexStrengths", Vector) = (0.0,0.0,0.0,1.0)
        [lilToggle]     _AudioLinkAsLocal           ("sAudioLinkAsLocal", Int) = 0
        [NoScaleOffset] _AudioLinkLocalMap          ("Local Map", 2D) = "black" {}
        [lilALLocal]    _AudioLinkLocalMapParams    ("sAudioLinkLocalMapParams", Vector) = (120,1,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Dissolve
                        _DissolveMask               ("Dissolve Mask", 2D) = "white" {}
                        _DissolveNoiseMask          ("Dissolve Noise Mask", 2D) = "gray" {}
        [lilUVAnim]     _DissolveNoiseMask_ScrollRotate ("Scroll", Vector) = (0,0,0,0)
                        _DissolveNoiseStrength      ("Dissolve Noise Strength", float) = 0.1
        [lilHDR]        _DissolveColor              ("sColor", Color) = (1,1,1,1)
        [lilDissolve]   _DissolveParams             ("sDissolveParamsModes", Vector) = (0,0,0.5,0.1)
        [lilDissolveP]  _DissolvePos                ("Dissolve Position", Vector) = (0,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // ID Mask
        // _IDMaskCompile will enable compilation of IDMask-related systems. For compatibility, setting certain
        // parameters to non-zero values will also enable the IDMask feature, but this enable switch ensures that
        // animator-controlled IDMasked meshes will be compiled correctly. Note that this _only_ controls compilation,
        // and is ignored at runtime.
        [ToggleUI]      _IDMaskCompile              ("_IDMaskCompile", Int) = 0
        [lilEnum]       _IDMaskFrom                 ("_IDMaskFrom|0: UV0|1: UV1|2: UV2|3: UV3|4: UV4|5: UV5|6: UV6|7: UV7|8: VertexID", Int) = 8
        [ToggleUI]      _IDMask1                    ("_IDMask1", Int) = 0
        [ToggleUI]      _IDMask2                    ("_IDMask2", Int) = 0
        [ToggleUI]      _IDMask3                    ("_IDMask3", Int) = 0
        [ToggleUI]      _IDMask4                    ("_IDMask4", Int) = 0
        [ToggleUI]      _IDMask5                    ("_IDMask5", Int) = 0
        [ToggleUI]      _IDMask6                    ("_IDMask6", Int) = 0
        [ToggleUI]      _IDMask7                    ("_IDMask7", Int) = 0
        [ToggleUI]      _IDMask8                    ("_IDMask8", Int) = 0
        [ToggleUI]      _IDMaskIsBitmap             ("_IDMaskIsBitmap", Int) = 0
                        _IDMaskIndex1               ("_IDMaskIndex1", Int) = 0
                        _IDMaskIndex2               ("_IDMaskIndex2", Int) = 0
                        _IDMaskIndex3               ("_IDMaskIndex3", Int) = 0
                        _IDMaskIndex4               ("_IDMaskIndex4", Int) = 0
                        _IDMaskIndex5               ("_IDMaskIndex5", Int) = 0
                        _IDMaskIndex6               ("_IDMaskIndex6", Int) = 0
                        _IDMaskIndex7               ("_IDMaskIndex7", Int) = 0
                        _IDMaskIndex8               ("_IDMaskIndex8", Int) = 0

        [ToggleUI]      _IDMaskControlsDissolve     ("_IDMaskControlsDissolve", Int) = 0
        [ToggleUI]      _IDMaskPrior1               ("_IDMaskPrior1", Int) = 0
        [ToggleUI]      _IDMaskPrior2               ("_IDMaskPrior2", Int) = 0
        [ToggleUI]      _IDMaskPrior3               ("_IDMaskPrior3", Int) = 0
        [ToggleUI]      _IDMaskPrior4               ("_IDMaskPrior4", Int) = 0
        [ToggleUI]      _IDMaskPrior5               ("_IDMaskPrior5", Int) = 0
        [ToggleUI]      _IDMaskPrior6               ("_IDMaskPrior6", Int) = 0
        [ToggleUI]      _IDMaskPrior7               ("_IDMaskPrior7", Int) = 0
        [ToggleUI]      _IDMaskPrior8               ("_IDMaskPrior8", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // UDIM Discard
        [lilToggleLeft] _UDIMDiscardCompile         ("sUDIMDiscard", Int) = 0
        [lilEnum]       _UDIMDiscardUV              ("sUDIMDiscardUV|0: UV0|1: UV1|2: UV2|3: UV3", Int) = 0
        [lilEnum]       _UDIMDiscardMode            ("sUDIMDiscardMode|0: Vertex|1: Pixel (slower)", Int) = 0
        [lilToggle]     _UDIMDiscardRow3_3          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow3_2          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow3_1          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow3_0          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow2_3          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow2_2          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow2_1          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow2_0          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow1_3          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow1_2          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow1_1          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow1_0          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow0_3          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow0_2          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow0_1          ("", Int) = 0
        [lilToggle]     _UDIMDiscardRow0_0          ("", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Outline
        [lilHDR]        _OutlineColor               ("sColor", Color) = (0.6,0.56,0.73,1)
                        _OutlineTex                 ("Texture", 2D) = "white" {}
        [lilUVAnim]     _OutlineTex_ScrollRotate    ("sScrollRotates", Vector) = (0,0,0,0)
        [lilHSVG]       _OutlineTexHSVG             ("sHSVGs", Vector) = (0,1,1,1)
        [lilHDR]        _OutlineLitColor            ("sColor", Color) = (1.0,0.2,0,0)
        [lilToggle]     _OutlineLitApplyTex         ("sColorFromMain", Int) = 0
                        _OutlineLitScale            ("Scale", Float) = 10
                        _OutlineLitOffset           ("Offset", Float) = -8
        [lilToggle]     _OutlineLitShadowReceive    ("sReceiveShadow", Int) = 0
        [lilOLWidth]    _OutlineWidth               ("Width", Range(0,1)) = 0.08
        [NoScaleOffset] _OutlineWidthMask           ("Width", 2D) = "white" {}
                        _OutlineFixWidth            ("sFixWidth", Range(0,1)) = 0.5
        [lilEnum]       _OutlineVertexR2Width       ("sOutlineVertexColorUsages", Int) = 0
        [lilToggle]     _OutlineDeleteMesh          ("sDeleteMesh0", Int) = 0
        [NoScaleOffset][Normal] _OutlineVectorTex   ("Vector", 2D) = "bump" {}
        [lilEnum]       _OutlineVectorUVMode        ("UV Mode|UV0|UV1|UV2|UV3", Int) = 0
                        _OutlineVectorScale         ("Vector scale", Range(-10,10)) = 1
                        _OutlineEnableLighting      ("sEnableLighting", Range(0, 1)) = 1
                        _OutlineZBias               ("Z Bias", Float) = 0
        [lilToggle]     _OutlineDisableInVR         ("sDisableInVR", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Tessellation
                        _TessEdge                   ("sTessellationEdge", Range(1, 100)) = 10
                        _TessStrength               ("sStrength", Range(0, 1)) = 0.5
                        _TessShrink                 ("sTessellationShrink", Range(0, 1)) = 0.0
        [IntRange]      _TessFactorMax              ("sTessellationFactor", Range(1, 8)) = 3

        //----------------------------------------------------------------------------------------------------------------------
        // For Multi
        [lilToggleLeft] _UseOutline                 ("Use Outline", Int) = 0
        [lilEnum]       _TransparentMode            ("Rendering Mode|Opaque|Cutout|Transparent|Refraction|Fur|FurCutout|Gem", Int) = 0
        [lilToggle]     _UseClippingCanceller       ("sSettingClippingCanceller", Int) = 0
        [lilToggle]     _AsOverlay                  ("sAsOverlay", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Save (Unused)
        [HideInInspector]                               _BaseColor          ("sColor", Color) = (1,1,1,1)
        [HideInInspector]                               _BaseMap            ("Texture", 2D) = "white" {}
        [HideInInspector]                               _BaseColorMap       ("Texture", 2D) = "white" {}
        [HideInInspector]                               _lilToonVersion     ("Version", Int) = 45

        //----------------------------------------------------------------------------------------------------------------------
        // VRChat
        _Ramp ("Shadow Ramp", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [lilEnum]                                       _Cull               ("sCullModes", Int) = 2
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlend           ("sSrcBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlend           ("sDstBlendRGB", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendAlpha      ("sSrcBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendAlpha      ("sDstBlendAlpha", Int) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOp            ("sBlendOpRGB", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpAlpha       ("sBlendOpAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendFA         ("sSrcBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendFA         ("sDstBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendAlphaFA    ("sSrcBlendAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendAlphaFA    ("sDstBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpFA          ("sBlendOpRGB", Int) = 4
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpAlphaFA     ("sBlendOpAlpha", Int) = 4
        [lilToggle]                                     _ZClip              ("sZClip", Int) = 1
        [lilToggle]                                     _ZWrite             ("sZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]   _ZTest              ("sZTest", Int) = 4
        [IntRange]                                      _StencilRef         ("Ref", Range(0, 255)) = 0
        [IntRange]                                      _StencilReadMask    ("ReadMask", Range(0, 255)) = 255
        [IntRange]                                      _StencilWriteMask   ("WriteMask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]   _StencilComp        ("Comp", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilPass        ("Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilFail        ("Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilZFail       ("ZFail", Float) = 0
                                                        _OffsetFactor       ("sOffsetFactor", Float) = 0
                                                        _OffsetUnits        ("sOffsetUnits", Float) = 0
        [lilColorMask]                                  _ColorMask          ("sColorMask", Int) = 15
        [lilToggle]                                     _AlphaToMask        ("sAlphaToMask", Int) = 0
                                                        _lilShadowCasterBias ("Shadow Caster Bias", Float) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Refraction
                        _RefractionStrength         ("sStrength", Range(-1,1)) = 0.1
        [PowerSlider(3.0)]_RefractionFresnelPower   ("sRefractionFresnel", Range(0.01, 10)) = 0.5
        [lilToggle]     _RefractionColorFromMain    ("sColorFromMain", Int) = 0
                        _RefractionColor            ("sColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "Queue" = "Transparent-100"}
        HLSLINCLUDE
            #define LIL_OPTIMIZE_APPLY_SHADOW_FA
            #define LIL_OPTIMIZE_USE_FORWARDADD
            #define LIL_OPTIMIZE_USE_VERTEXLIGHT
            #define LIL_FEATURE_VRCLIGHTVOLUMES_WITHOUTPACKAGE
            #pragma target 3.5
            #pragma fragmentoption ARB_precision_hint_fastest
            #define LIL_REFRACTION
            #define LIL_MULTI
            #define LIL_MULTI_INPUTS_MAIN_TONECORRECTION
            #define LIL_MULTI_INPUTS_MAIN2ND
            #define LIL_MULTI_INPUTS_MAIN3RD
            #define LIL_MULTI_INPUTS_ALPHAMASK
            #define LIL_MULTI_INPUTS_SHADOW
            #define LIL_MULTI_INPUTS_RIMSHADE
            #define LIL_MULTI_INPUTS_BACKLIGHT
            #define LIL_MULTI_INPUTS_EMISSION
            #define LIL_MULTI_INPUTS_EMISSION_2ND
            #define LIL_MULTI_INPUTS_NORMAL
            #define LIL_MULTI_INPUTS_NORMAL_2ND
            #define LIL_MULTI_INPUTS_ANISOTROPY
            #define LIL_MULTI_INPUTS_REFLECTION
            #define LIL_MULTI_INPUTS_MATCAP
            #define LIL_MULTI_INPUTS_MATCAP_2ND
            #define LIL_MULTI_INPUTS_RIM
            #define LIL_MULTI_INPUTS_GLITTER
            #define LIL_MULTI_INPUTS_PARALLAX
            #define LIL_MULTI_INPUTS_DISTANCE_FADE
            #define LIL_MULTI_INPUTS_AUDIOLINK
            #define LIL_MULTI_INPUTS_DISSOLVE
            #define LIL_MULTI_INPUTS_IDMASK
            #define LIL_MULTI_INPUTS_UDIMDISCARD

            #pragma skip_variants DECALS_OFF DECALS_3RT DECALS_4RT DECAL_SURFACE_GRADIENT _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma skip_variants _ADDITIONAL_LIGHT_SHADOWS
            #pragma skip_variants PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
            #pragma skip_variants _SCREEN_SPACE_OCCLUSION
        ENDHLSL

        // GrabPass
        GrabPass {"_lilBackgroundTexture"}


        // Forward
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "ForwardBase"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #define LIL_PASS_FORWARD

            // AlphaMask and Dissolve
            #pragma shader_feature_local _COLOROVERLAY_ON
            #pragma shader_feature_local GEOM_TYPE_BRANCH_DETAIL

            // Main
            #pragma shader_feature_local EFFECT_HUE_VARIATION
            #pragma shader_feature_local _COLORADDSUBDIFF_ON
            #pragma shader_feature_local _COLORCOLOR_ON
            #pragma shader_feature_local _SUNDISK_NONE
            #pragma shader_feature_local GEOM_TYPE_FROND
            #pragma shader_feature_local _REQUIRE_UV2
            #pragma shader_feature_local AUTO_KEY_VALUE
            #pragma shader_feature_local ANTI_FLICKER
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local GEOM_TYPE_BRANCH
            #pragma shader_feature_local _SUNDISK_SIMPLE
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local EFFECT_BUMP
            #pragma shader_feature_local SOURCE_GBUFFER
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local GEOM_TYPE_MESH
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local GEOM_TYPE_LEAF
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local PIXELSNAP_ON
            #pragma shader_feature_local _FADING_ON
            #pragma shader_feature_local _MAPPING_6_FRAMES_LAYOUT
            #pragma shader_feature_local _SUNDISK_HIGH_QUALITY

            // Replace keywords
            #include "Includes/lil_replace_keywords.hlsl"

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pipeline_brp.hlsl"
            #include "Includes/lil_common.hlsl"
            // Insert functions and includes that depend on Unity here

            #include "Includes/lil_pass_forward.hlsl"

            ENDHLSL
        }

        //----------------------------------------------------------------------------------------------------------------------
        // ForwardAdd Start
        //

        // ForwardAdd
        Pass
        {
            Name "FORWARD_ADD"
            Tags {"LightMode" = "ForwardAdd"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZClip [_ZClip]
            ZWrite Off
            ZTest LEqual
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            Blend [_SrcBlendFA] [_DstBlendFA], Zero One
            BlendOp [_BlendOpFA], [_BlendOpAlphaFA]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fragment POINT DIRECTIONAL SPOT POINT_COOKIE DIRECTIONAL_COOKIE
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #define LIL_PASS_FORWARDADD

            // AlphaMask and Dissolve
            #pragma shader_feature_local _COLOROVERLAY_ON
            #pragma shader_feature_local GEOM_TYPE_BRANCH_DETAIL

            // Main
            #pragma shader_feature_local EFFECT_HUE_VARIATION
            #pragma shader_feature_local _COLORADDSUBDIFF_ON
            #pragma shader_feature_local _COLORCOLOR_ON
            #pragma shader_feature_local _SUNDISK_NONE
            #pragma shader_feature_local GEOM_TYPE_FROND
            #pragma shader_feature_local _REQUIRE_UV2
            #pragma shader_feature_local AUTO_KEY_VALUE
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local EFFECT_BUMP
            #pragma shader_feature_local SOURCE_GBUFFER
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local GEOM_TYPE_MESH
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local GEOM_TYPE_LEAF
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local PIXELSNAP_ON
            #pragma shader_feature_local _FADING_ON
            #pragma shader_feature_local _MAPPING_6_FRAMES_LAYOUT
            #pragma shader_feature_local _SUNDISK_HIGH_QUALITY

            // Replace keywords
            #include "Includes/lil_replace_keywords.hlsl"

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pipeline_brp.hlsl"
            #include "Includes/lil_common.hlsl"
            // Insert functions and includes that depend on Unity here

            #include "Includes/lil_pass_forward.hlsl"

            ENDHLSL
        }

        //
        // ForwardAdd End

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Offset 1, 1
            Cull [_Cull]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            #define LIL_PASS_SHADOWCASTER

            // AlphaMask and Dissolve
            #pragma shader_feature_local _COLOROVERLAY_ON
            #pragma shader_feature_local GEOM_TYPE_BRANCH_DETAIL

            // Replace keywords
            #include "Includes/lil_replace_keywords.hlsl"

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pipeline_brp.hlsl"
            #include "Includes/lil_common.hlsl"
            // Insert functions and includes that depend on Unity here

            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {"LightMode" = "Meta"}
            Cull Off

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature EDITOR_VISUALIZATION
            #define LIL_PASS_META

            // Tone correction and emission
            #pragma shader_feature_local EFFECT_HUE_VARIATION
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local GEOM_TYPE_BRANCH
            #pragma shader_feature_local _SUNDISK_SIMPLE

            // Replace keywords
            #include "Includes/lil_replace_keywords.hlsl"

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pipeline_brp.hlsl"
            #include "Includes/lil_common.hlsl"
            // Insert functions and includes that depend on Unity here

            #include "Includes/lil_pass_meta.hlsl"

            ENDHLSL
        }

    }
    Fallback "Unlit/Texture"

    CustomEditor "lilToon.lilToonInspector"
}


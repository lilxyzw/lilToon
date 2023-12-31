namespace lilToon
{
    public class lilPropertyNameChecker
    {
        private static bool IsRenderingPropertyInternal(string name)
        {
            return
                name.Contains("Cull") ||
                name.Contains("Src") ||
                name.Contains("Dst") ||
                name.Contains("BlendOp") ||
                name.Contains("ZClip") ||
                name.Contains("ZWrite") ||
                name.Contains("ZTest") ||
                name.Contains("Stencil") ||
                name.Contains("OffsetFactor") ||
                name.Contains("OffsetUnits") ||
                name.Contains("ColorMask") ||
                name.Contains("AlphaToMask");
        }

        private static bool IsStencilPropertyInternal(string name)
        {
            return name.Contains("Stencil");
        }

        public static bool IsDummyProperty(string name)
        {
            bool res = false;
            res = res || name == "_BaseColor";
            res = res || name == "_BaseMap";
            res = res || name == "_BaseColorMap";
            res = res || name == "_lilToonVersion";
            res = res || name.Contains("_egc");
            res = res || name.Contains("_ega");
            res = res || name.Contains("_e2gc");
            res = res || name.Contains("_e2ga");
            return res;
        }

        public static bool IsBaseProperty(string name)
        {
            bool res = false;
            res = res || name == "_Invisible";
            res = res || name == "_Cutoff";
            res = res || name == "_FlipNormal";
            res = res || name == "_BackfaceForceShadow";
            res = res || name == "_BackfaceColor";
            res = res || name == "_FakeShadowVector";
            res = res || name == "_TriMask";
            res = res || name == "_TransparentMode";
            res = res || name == "_UseClippingCanceller";
            res = res || name == "_AsOverlay";
            res = res || name == "_AAStrength";
            res = res || name == "_UseDither";
            res = res || name.Contains("_Dither");
            return res;
        }

        public static bool IsLightingProperty(string name)
        {
            bool res = false;
            res = res || name == "_LightMinLimit";
            res = res || name == "_LightMaxLimit";
            res = res || name == "_MonochromeLighting";
            res = res || name == "_AsUnlit";
            res = res || name == "_VertexLightStrength";
            res = res || name == "_BeforeExposureLimit";
            res = res || name == "_AlphaBoostFA";
            res = res || name == "_lilDirectionalLightStrength";
            res = res || name == "_LightDirectionOverride";
            return res;
        }

        public static bool IsUVProperty(string name)
        {
            bool res = false;
            res = res || name == "_MainTex";
            res = res || name == "_MainTex_ScrollRotate";
            res = res || name == "_ShiftBackfaceUV";
            return res;
        }

        public static bool IsMainProperty(string name)
        {
            bool res = false;
            res = res || name == "_Color";
            res = res || name.Contains("_Main") && !name.Contains("_ScrollRotate") && !name.Contains("2nd") && !name.Contains("3rd");
            return res;
        }

        public static bool IsMain2ndProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseMain2ndTex";
            res = res || name == "_Color2nd";
            res = res || name.Contains("_Main2nd");
            return res;
        }

        public static bool IsMain3rdProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseMain3rdTex";
            res = res || name == "_Color3rd";
            res = res || name.Contains("_Main3rd");
            return res;
        }

        public static bool IsAlphaMaskProperty(string name)
        {
            bool res = false;
            res = res || name.Contains("_AlphaMask");
            return res;
        }

        public static bool IsShadowProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseShadow";
            res = res || name == "_lilShadowCasterBias";
            res = res || name.Contains("_Shadow");
            return res;
        }

        public static bool IsRimShadeProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseRimShade";
            res = res || name.Contains("_RimShade");
            return res;
        }

        public static bool IsEmissionProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseEmission";
            res = res || name.Contains("_Emission") && !name.Contains("2nd");
            return res;
        }

        public static bool IsEmission2ndProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseEmission2nd";
            res = res || name.Contains("_Emission2nd");
            return res;
        }

        public static bool IsNormalMapProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseBumpMap";
            res = res || name == "_BumpMap";
            res = res || name == "_BumpScale";
            return res;
        }

        public static bool IsNormalMap2ndProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseBump2ndMap";
            res = res || name == "_Bump2ndMap";
            res = res || name == "_Bump2ndScale";
            res = res || name == "_Bump2ndScaleMask";
            return res;
        }

        public static bool IsAnisotropyProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseAnisotropy";
            res = res || name.Contains("_Anisotropy");
            return res;
        }

        public static bool IsBacklightProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseBacklight";
            res = res || name.Contains("_Backlight");
            return res;
        }

        public static bool IsReflectionProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseReflection";
            res = res || name == "_Smoothness";
            res = res || name == "_SmoothnessTex";
            res = res || name == "_Metallic";
            res = res || name == "_MetallicGlossMap";
            res = res || name == "_Reflectance";
            res = res || name == "_GSAAStrength";
            res = res || name == "_ApplySpecular";
            res = res || name == "_ApplySpecularFA";
            res = res || name == "_ApplyReflection";
            res = res || name.Contains("_Specular");
            res = res || name.Contains("_Reflection");
            return res;
        }

        public static bool IsMatCapProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseMatCap";
            res = res || name.Contains("_MatCap") && !name.Contains("2nd");
            return res;
        }

        public static bool IsMatCap2ndProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseMatCap2nd";
            res = res || name.Contains("_MatCap2nd");
            return res;
        }

        public static bool IsRimProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseRim";
            res = res || name.Contains("_Rim");
            return res;
        }

        public static bool IsGlitterProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseGlitter";
            res = res || name.Contains("_Glitter");
            return res;
        }

        public static bool IsParallaxProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseParallax";
            res = res || name == "_UsePOM";
            res = res || name.Contains("_Parallax");
            return res;
        }

        public static bool IsDistanceFadeProperty(string name)
        {
            bool res = false;
            res = res || name.Contains("_DistanceFade");
            return res;
        }

        public static bool IsAudioLinkProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseAudioLink";
            res = res || name.Contains("_AudioLink");
            return res;
        }

        public static bool IsDissolveProperty(string name)
        {
            bool res = false;
            res = res || name.Contains("_Dissolve");
            return res;
        }

        public static bool IsRefractionProperty(string name)
        {
            bool res = false;
            res = res || name.Contains("_Refraction");
            return res;
        }

        public static bool IsGemProperty(string name)
        {
            bool res = false;
            res = res || name.Contains("_Gem");
            res = res || IsReflectionProperty(name);
            res = res || IsRefractionProperty(name);
            return res;
        }

        public static bool IsTessellationProperty(string name)
        {
            bool res = false;
            res = res || name.Contains("_Tess");
            return res;
        }

        public static bool IsOutlineProperty(string name)
        {
            bool res = false;
            res = res || name == "_UseOutline";
            res = res || name.Contains("_Outline") && !IsRenderingPropertyInternal(name) && !IsStencilPropertyInternal(name);
            return res;
        }

        public static bool IsFurProperty(string name)
        {
            bool res = false;
            res = res || name == "_VertexColor2FurVector";
            res = res || name.Contains("_Fur") && !IsRenderingPropertyInternal(name) && !IsStencilPropertyInternal(name);
            return res;
        }

        public static bool IsStencilProperty(string name)
        {
            bool res = false;
            res = res || IsStencilPropertyInternal(name);
            return res;
        }

        public static bool IsRenderingProperty(string name)
        {
            bool res = false;
            res = res || !name.Contains("_Outline") && !name.Contains("_Fur") && IsRenderingPropertyInternal(name);
            res = res || name == "_SubpassCutoff";
            res = res || name == "_lilShadowCasterBias";
            return res;
        }

        public static bool IsOutlineRenderingProperty(string name)
        {
            bool res = false;
            res = res || name.Contains("_Outline") && IsRenderingPropertyInternal(name);
            return res;
        }

        public static bool IsFurRenderingProperty(string name)
        {
            bool res = false;
            res = res || name.Contains("_Fur") && IsRenderingPropertyInternal(name);
            return res;
        }
    }
}
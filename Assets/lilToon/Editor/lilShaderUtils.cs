#if UNITY_EDITOR

namespace lilToon
{
    public static class lilShaderUtils
    {
        public static bool IsLiteShaderName(string shaderName)
        {
            var separatorIndex = shaderName.LastIndexOf('/');
            if (separatorIndex == -1 || separatorIndex + 1 == shaderName.Length)
            {
                return false;
            }

            if (shaderName.IndexOf("Lite", separatorIndex + 1) != -1)
            {
                return true;
            }

            // For following custom shader names.
            // - Hidden/*LIL_SHADER_NAME*/Lite/Cutout
            // - Hidden/*LIL_SHADER_NAME*/Lite/Transparent
            // - Hidden/*LIL_SHADER_NAME*/Lite/OnePassTransparent
            // - Hidden/*LIL_SHADER_NAME*/Lite/TwoPassTransparent
            // - Hidden/*LIL_SHADER_NAME*/Lite/OpaqueOutline
            // - Hidden/*LIL_SHADER_NAME*/Lite/CutoutOutline
            // - Hidden/*LIL_SHADER_NAME*/Lite/TransparentOutline
            // - Hidden/*LIL_SHADER_NAME*/Lite/OnePassTransparentOutline
            // - Hidden/*LIL_SHADER_NAME*/Lite/TwoPassTransparentOutline
            var partIndex = shaderName.LastIndexOf("/Lite/");
            if (partIndex != -1 && partIndex + 5 == separatorIndex)
            {
                return true;
            }

            return false;
        }

        public static bool IsCutoutShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "Cutout");
        }

        public static bool IsTransparentShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "Transparent");
        }

        public static bool IsOverlayShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "Overlay");
        }

        public static bool IsOutlineShaderName(string shaderName)
        {
            var separatorIndex = shaderName.LastIndexOf('/');
            if (separatorIndex == -1 || separatorIndex + 1 == shaderName.Length)
            {
                return false;
            }

            if (shaderName.IndexOf("Outline", separatorIndex + 1) != -1)
            {
                return true;
            }

            // For following custom shader names.
            // - *LIL_SHADER_NAME*/[Optional] OutlineOnly/Opaque
            // - *LIL_SHADER_NAME*/[Optional] OutlineOnly/Cutout
            // - *LIL_SHADER_NAME*/[Optional] OutlineOnly/Transparent
            var partIndex = shaderName.LastIndexOf("/[Optional] OutlineOnly/");
            if (partIndex != -1 && partIndex + 23 == separatorIndex)
            {
                return true;
            }

            return false;
        }

        public static bool IsRefractionShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "Refraction");
        }

        public static bool IsRefractionBlurShaderName(string shaderName)
        {
            return shaderName.EndsWith("RefractionBlur");
        }

        public static bool IsBlurShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "Blur");
        }

        public static bool IsFurShaderName(string shaderName)
        {
            var separatorIndex = shaderName.LastIndexOf('/');
            if (separatorIndex == -1 || separatorIndex + 1 == shaderName.Length)
            {
                return false;
            }

            if (shaderName.IndexOf("Fur", separatorIndex + 1) != -1)
            {
                return true;
            }

            // For following custom shader names.
            // - *LIL_SHADER_NAME*/[Optional] FurOnly/Transparent
            // - *LIL_SHADER_NAME*/[Optional] FurOnly/Cutout
            // - *LIL_SHADER_NAME*/[Optional] FurOnly/TwoPass
            var partIndex = shaderName.LastIndexOf("/[Optional] FurOnly/");
            if (partIndex != -1 && partIndex + 19 == separatorIndex)
            {
                return true;
            }

            return false;
        }

        public static bool IsFurCutoutShaderName(string shaderName)
        {
            return shaderName.EndsWith("FurCutout");
        }

        public static bool IsFurTwoPassShaderName(string shaderName)
        {
            return shaderName.EndsWith("FurTwoPass");
        }

        public static bool IsTessellationShaderName(string shaderName)
        {
            var separatorIndex = shaderName.LastIndexOf('/');
            if (separatorIndex == -1 || separatorIndex + 1 == shaderName.Length)
            {
                return false;
            }

            if (shaderName.IndexOf("Tessellation", separatorIndex + 1) > 0)
            {
                return true;
            }

            // For following custom shader names.
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/Opaque
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/Cutout
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/Transparent
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/OnePassTransparent
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/TwoPassTransparent
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/OpaqueOutline
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/CutoutOutline
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/TransparentOutline
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/OnePassTransparentOutline
            // - Hidden/*LIL_SHADER_NAME*/Tessellation/TwoPassTransparentOutline
            var partIndex = shaderName.LastIndexOf("/Tessellation/");
            if (partIndex != -1 && partIndex + 13 == separatorIndex)
            {
                return true;
            }

            return false;
        }

        public static bool IsGemShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "Gem");
        }

        public static bool IsFakeShadowShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "FakeShadow");
        }

        public static bool IsOnePassShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "OnePass");
        }

        public static bool IsTwoPassShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "TwoPass");
        }

        public static bool IsMultiShaderName(string shaderName)
        {
            return ContainsAfterLastSeparator(shaderName, "Multi");
        }

        public static bool IsOptionalShaderName(string shaderName)
        {
            return shaderName.Contains("/[Optional] ");
        }

        private static bool ContainsAfterLastSeparator(string shaderName, string subName)
        {
            var separatorIndex = shaderName.LastIndexOf('/');
            if (separatorIndex == -1 || separatorIndex + 1 == shaderName.Length)
            {
                return false;
            }

            return shaderName.IndexOf(subName, separatorIndex + 1) != -1;
        }
    }
}
#endif

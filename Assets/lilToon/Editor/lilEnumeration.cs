#if UNITY_EDITOR
namespace lilToon
{
    public enum EditorMode
    {
        Simple,
        Advanced,
        Preset,
        Settings
    }

    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Transparent,
        Refraction,
        RefractionBlur,
        Fur,
        FurCutout,
        FurTwoPass,
        Gem
    }

    public enum TransparentMode
    {
        Normal,
        OnePass,
        TwoPass
    }

    [System.Obsolete]
    public enum BlendMode
    {
        Alpha,
        Add,
        Screen,
        Mul
    }

    public enum LightingPreset
    {
        Default,
        SemiMonochrome
    }

    public enum PropertyBlock
    {
        Base,
        Lighting,
        UV,
        MainColor,
        MainColor1st,
        MainColor2nd,
        MainColor3rd,
        AlphaMask,
        Shadow,
        RimShade,
        Emission,
        Emission1st,
        Emission2nd,
        NormalMap,
        NormalMap1st,
        NormalMap2nd,
        Anisotropy,
        Reflections,
        Reflection,
        MatCaps,
        MatCap1st,
        MatCap2nd,
        RimLight,
        Glitter,
        Backlight,
        Gem,
        Outline,
        Parallax,
        DistanceFade,
        AudioLink,
        Dissolve,
        IDMask,
        UDIMDiscard,
        Refraction,
        Fur,
        Encryption,
        Stencil,
        Rendering,
        Tessellation,
        Other
    }

    public enum lilRenderPipeline
    {
        BRP,
        LWRP,
        URP,
        HDRP
    }
}
#endif
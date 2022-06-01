#if UNITY_EDITOR
using UnityEngine;

namespace lilToon
{
    public class lilShaderManager
    {
        public static Shader lts         = Shader.Find("lilToon");
        public static Shader ltsc        = Shader.Find("Hidden/lilToonCutout");
        public static Shader ltst        = Shader.Find("Hidden/lilToonTransparent");
        public static Shader ltsot       = Shader.Find("Hidden/lilToonOnePassTransparent");
        public static Shader ltstt       = Shader.Find("Hidden/lilToonTwoPassTransparent");

        public static Shader ltso        = Shader.Find("Hidden/lilToonOutline");
        public static Shader ltsco       = Shader.Find("Hidden/lilToonCutoutOutline");
        public static Shader ltsto       = Shader.Find("Hidden/lilToonTransparentOutline");
        public static Shader ltsoto      = Shader.Find("Hidden/lilToonOnePassTransparentOutline");
        public static Shader ltstto      = Shader.Find("Hidden/lilToonTwoPassTransparentOutline");

        public static Shader ltsoo       = Shader.Find("_lil/[Optional] lilToonOutlineOnly");
        public static Shader ltscoo      = Shader.Find("_lil/[Optional] lilToonCutoutOutlineOnly");
        public static Shader ltstoo      = Shader.Find("_lil/[Optional] lilToonTransparentOutlineOnly");

        public static Shader ltstess     = Shader.Find("Hidden/lilToonTessellation");
        public static Shader ltstessc    = Shader.Find("Hidden/lilToonTessellationCutout");
        public static Shader ltstesst    = Shader.Find("Hidden/lilToonTessellationTransparent");
        public static Shader ltstessot   = Shader.Find("Hidden/lilToonTessellationOnePassTransparent");
        public static Shader ltstesstt   = Shader.Find("Hidden/lilToonTessellationTwoPassTransparent");

        public static Shader ltstesso    = Shader.Find("Hidden/lilToonTessellationOutline");
        public static Shader ltstessco   = Shader.Find("Hidden/lilToonTessellationCutoutOutline");
        public static Shader ltstessto   = Shader.Find("Hidden/lilToonTessellationTransparentOutline");
        public static Shader ltstessoto  = Shader.Find("Hidden/lilToonTessellationOnePassTransparentOutline");
        public static Shader ltstesstto  = Shader.Find("Hidden/lilToonTessellationTwoPassTransparentOutline");

        public static Shader ltsl        = Shader.Find("Hidden/lilToonLite");
        public static Shader ltslc       = Shader.Find("Hidden/lilToonLiteCutout");
        public static Shader ltslt       = Shader.Find("Hidden/lilToonLiteTransparent");
        public static Shader ltslot      = Shader.Find("Hidden/lilToonLiteOnePassTransparent");
        public static Shader ltsltt      = Shader.Find("Hidden/lilToonLiteTwoPassTransparent");

        public static Shader ltslo       = Shader.Find("Hidden/lilToonLiteOutline");
        public static Shader ltslco      = Shader.Find("Hidden/lilToonLiteCutoutOutline");
        public static Shader ltslto      = Shader.Find("Hidden/lilToonLiteTransparentOutline");
        public static Shader ltsloto     = Shader.Find("Hidden/lilToonLiteOnePassTransparentOutline");
        public static Shader ltsltto     = Shader.Find("Hidden/lilToonLiteTwoPassTransparentOutline");

        public static Shader ltsref      = Shader.Find("Hidden/lilToonRefraction");
        public static Shader ltsrefb     = Shader.Find("Hidden/lilToonRefractionBlur");
        public static Shader ltsfur      = Shader.Find("Hidden/lilToonFur");
        public static Shader ltsfurc     = Shader.Find("Hidden/lilToonFurCutout");
        public static Shader ltsfurtwo   = Shader.Find("Hidden/lilToonFurTwoPass");
        public static Shader ltsfuro     = Shader.Find("_lil/[Optional] lilToonFurOnly");
        public static Shader ltsfuroc    = Shader.Find("_lil/[Optional] lilToonFurOnlyCutout");
        public static Shader ltsfurotwo  = Shader.Find("_lil/[Optional] lilToonFurOnlyTwoPass");

        public static Shader ltsgem      = Shader.Find("Hidden/lilToonGem");

        public static Shader ltsfs       = Shader.Find("_lil/lilToonFakeShadow");

        public static Shader ltsover     = Shader.Find("_lil/[Optional] lilToonOverlay");
        public static Shader ltsoover    = Shader.Find("_lil/[Optional] lilToonOverlayOnePass");
        public static Shader ltslover    = Shader.Find("_lil/[Optional] lilToonLiteOverlay");
        public static Shader ltsloover   = Shader.Find("_lil/[Optional] lilToonLiteOverlayOnePass");

        public static Shader ltsbaker    = Shader.Find("Hidden/ltsother_baker");
        public static Shader ltspo       = Shader.Find("Hidden/ltspass_opaque");
        public static Shader ltspc       = Shader.Find("Hidden/ltspass_cutout");
        public static Shader ltspt       = Shader.Find("Hidden/ltspass_transparent");
        public static Shader ltsptesso   = Shader.Find("Hidden/ltspass_tess_opaque");
        public static Shader ltsptessc   = Shader.Find("Hidden/ltspass_tess_cutout");
        public static Shader ltsptesst   = Shader.Find("Hidden/ltspass_tess_transparent");

        public static Shader ltsm        = Shader.Find("_lil/lilToonMulti");
        public static Shader ltsmo       = Shader.Find("Hidden/lilToonMultiOutline");
        public static Shader ltsmref     = Shader.Find("Hidden/lilToonMultiRefraction");
        public static Shader ltsmfur     = Shader.Find("Hidden/lilToonMultiFur");
        public static Shader ltsmgem     = Shader.Find("Hidden/lilToonMultiGem");
        public static Shader mtoon       = Shader.Find("VRM/MToon");

        public static void InitializeShaders()
        {
            lts         = Shader.Find("lilToon");
            ltsc        = Shader.Find("Hidden/lilToonCutout");
            ltst        = Shader.Find("Hidden/lilToonTransparent");
            ltsot       = Shader.Find("Hidden/lilToonOnePassTransparent");
            ltstt       = Shader.Find("Hidden/lilToonTwoPassTransparent");

            ltso        = Shader.Find("Hidden/lilToonOutline");
            ltsco       = Shader.Find("Hidden/lilToonCutoutOutline");
            ltsto       = Shader.Find("Hidden/lilToonTransparentOutline");
            ltsoto      = Shader.Find("Hidden/lilToonOnePassTransparentOutline");
            ltstto      = Shader.Find("Hidden/lilToonTwoPassTransparentOutline");

            ltsoo       = Shader.Find("_lil/[Optional] lilToonOutlineOnly");
            ltscoo      = Shader.Find("_lil/[Optional] lilToonCutoutOutlineOnly");
            ltstoo      = Shader.Find("_lil/[Optional] lilToonTransparentOutlineOnly");

            ltstess     = Shader.Find("Hidden/lilToonTessellation");
            ltstessc    = Shader.Find("Hidden/lilToonTessellationCutout");
            ltstesst    = Shader.Find("Hidden/lilToonTessellationTransparent");
            ltstessot   = Shader.Find("Hidden/lilToonTessellationOnePassTransparent");
            ltstesstt   = Shader.Find("Hidden/lilToonTessellationTwoPassTransparent");

            ltstesso    = Shader.Find("Hidden/lilToonTessellationOutline");
            ltstessco   = Shader.Find("Hidden/lilToonTessellationCutoutOutline");
            ltstessto   = Shader.Find("Hidden/lilToonTessellationTransparentOutline");
            ltstessoto  = Shader.Find("Hidden/lilToonTessellationOnePassTransparentOutline");
            ltstesstto  = Shader.Find("Hidden/lilToonTessellationTwoPassTransparentOutline");

            ltsl        = Shader.Find("Hidden/lilToonLite");
            ltslc       = Shader.Find("Hidden/lilToonLiteCutout");
            ltslt       = Shader.Find("Hidden/lilToonLiteTransparent");
            ltslot      = Shader.Find("Hidden/lilToonLiteOnePassTransparent");
            ltsltt      = Shader.Find("Hidden/lilToonLiteTwoPassTransparent");

            ltslo       = Shader.Find("Hidden/lilToonLiteOutline");
            ltslco      = Shader.Find("Hidden/lilToonLiteCutoutOutline");
            ltslto      = Shader.Find("Hidden/lilToonLiteTransparentOutline");
            ltsloto     = Shader.Find("Hidden/lilToonLiteOnePassTransparentOutline");
            ltsltto     = Shader.Find("Hidden/lilToonLiteTwoPassTransparentOutline");

            ltsref      = Shader.Find("Hidden/lilToonRefraction");
            ltsrefb     = Shader.Find("Hidden/lilToonRefractionBlur");
            ltsfur      = Shader.Find("Hidden/lilToonFur");
            ltsfurc     = Shader.Find("Hidden/lilToonFurCutout");
            ltsfurtwo   = Shader.Find("Hidden/lilToonFurTwoPass");

            ltsgem      = Shader.Find("Hidden/lilToonGem");

            ltsfs       = Shader.Find("_lil/lilToonFakeShadow");

            ltsbaker    = Shader.Find("Hidden/ltsother_baker");
            ltspo       = Shader.Find("Hidden/ltspass_opaque");
            ltspc       = Shader.Find("Hidden/ltspass_cutout");
            ltspt       = Shader.Find("Hidden/ltspass_transparent");
            ltsptesso   = Shader.Find("Hidden/ltspass_tess_opaque");
            ltsptessc   = Shader.Find("Hidden/ltspass_tess_cutout");
            ltsptesst   = Shader.Find("Hidden/ltspass_tess_transparent");

            ltsm        = Shader.Find("_lil/lilToonMulti");
            ltsmo       = Shader.Find("Hidden/lilToonMultiOutline");
            ltsmref     = Shader.Find("Hidden/lilToonMultiRefraction");
            ltsmfur     = Shader.Find("Hidden/lilToonMultiFur");
            ltsmgem     = Shader.Find("Hidden/lilToonMultiGem");

            mtoon       = Shader.Find("VRM/MToon");
        }
    }
}
#endif
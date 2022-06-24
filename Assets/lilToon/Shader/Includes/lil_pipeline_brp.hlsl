#define LIL_BRP
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"
#include "UnityMetaPass.cginc"

#if defined(UNITY_SHOULD_SAMPLE_SH)
    #undef UNITY_SHOULD_SAMPLE_SH
    #define UNITY_SHOULD_SAMPLE_SH 1
    #define LIL_FORCED_SH
#endif
#include "openlit_core.hlsl"
#undef UNITY_SHOULD_SAMPLE_SH
#define UNITY_SHOULD_SAMPLE_SH (defined(LIGHTPROBE_SH) && !defined(UNITY_PASS_FORWARDADD) && !defined(UNITY_PASS_PREPASSBASE) && !defined(UNITY_PASS_SHADOWCASTER) && !defined(UNITY_PASS_META))
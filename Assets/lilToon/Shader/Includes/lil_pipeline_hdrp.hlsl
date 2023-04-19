#define LIL_HDRP

#define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
#if !defined(SHADOW_LOW) && !defined(SHADOW_MEDIUM) && !defined(SHADOW_HIGH)
    #define SHADOW_LOW
#endif
#if defined(SHADOW_LOW)
    #define PUNCTUAL_SHADOW_LOW
    #define DIRECTIONAL_SHADOW_LOW
#elif defined(SHADOW_MEDIUM)
    #define PUNCTUAL_SHADOW_MEDIUM
    #define DIRECTIONAL_SHADOW_MEDIUM
#elif defined(SHADOW_HIGH)
    #define PUNCTUAL_SHADOW_HIGH
    #define DIRECTIONAL_SHADOW_HIGH
#endif
//#pragma multi_compile_fragment AREA_SHADOW_MEDIUM AREA_SHADOW_HIGH
#define AREA_SHADOW_MEDIUM

#if defined(LIL_PASS_SHADOWCASTER) && ((LIL_SRP_VERSION_MAJOR < 5) || (LIL_SRP_VERSION_MAJOR <= 5) && (LIL_SRP_VERSION_MINOR <= 10))
    #define USE_LEGACY_UNITY_MATRIX_VARIABLES
#endif

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightEvaluation.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialEvaluation.hlsl"
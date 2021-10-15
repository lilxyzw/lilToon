#ifndef LIL_PIPELINE_INCLUDED
#define LIL_PIPELINE_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// BRP Start
//
#define LIL_BRP
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"
#include "Includes/lil_common.hlsl"
#include "UnityMetaPass.cginc"
//
// BRP End

//------------------------------------------------------------------------------------------------------------------------------
// LWRP Start
/*
#define LIL_LWRP
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
#include "Includes/lil_common.hlsl"
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/MetaInput.hlsl"
*/
// LWRP End

//------------------------------------------------------------------------------------------------------------------------------
// URP Start
/*
#define LIL_URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Includes/lil_common.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
*/
// URP End

//------------------------------------------------------------------------------------------------------------------------------
// HDRP Start
/*
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

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightEvaluation.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialEvaluation.hlsl"

#include "Includes/lil_common.hlsl"
*/
// HDRP End

#endif
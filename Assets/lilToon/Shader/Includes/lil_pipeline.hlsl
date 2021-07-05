#ifndef LIL_PIPELINE_INCLUDED
#define LIL_PIPELINE_INCLUDED

//----------------------------------------------------------------------------------------------------------------------
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

//----------------------------------------------------------------------------------------------------------------------
// LWRP Start
/*
#define LIL_LWRP
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
#include "Includes/lil_common.hlsl"
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/MetaInput.hlsl"
float4 MetaVertexPosition(float4 positionOS, float2 uv1, float2 uv2, float4 lmst, float4 dlst) { return MetaVertexPosition(positionOS, uv1, uv2, lmst); }
*/
// LWRP End

//----------------------------------------------------------------------------------------------------------------------
// URP Start
/*
#define LIL_URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Includes/lil_common.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
*/
// URP End

#endif
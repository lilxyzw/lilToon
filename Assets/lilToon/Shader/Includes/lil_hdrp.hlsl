#ifndef LIL_HDRP_INCLUDED
#define LIL_HDRP_INCLUDED

#define LIGHT_SIMULATE_HQ

// Avoid Error
#define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
#if !defined(SHADOW_LOW) && !defined(SHADOW_MEDIUM) && !defined(SHADOW_HIGH)
    #define SHADOW_LOW
#endif

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightEvaluation.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialEvaluation.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialGBufferMacros.hlsl"

struct lilNPRLightingData
{
    float3 color;
    float3 direction;
};

uint lilGetRenderingLayer()
{
    #if defined(RENDERING_LIGHT_LAYERS_MASK)
        return _EnableLightLayers ? (asuint(unity_RenderingLayer.x) & RENDERING_LIGHT_LAYERS_MASK) >> RENDERING_LIGHT_LAYERS_MASK_SHIFT : DEFAULT_LIGHT_LAYERS;
    #else
        return _EnableLightLayers ? asuint(unity_RenderingLayer.x) : DEFAULT_LIGHT_LAYERS;
    #endif
}

LightLoopContext lilInitLightLoopContext()
{
    LightLoopContext lightLoopContext;
    lightLoopContext.shadowContext    = InitShadowContext();
    lightLoopContext.shadowValue      = 1;
    lightLoopContext.sampleReflection = 0;
    lightLoopContext.contactShadow    = 0;
    return lightLoopContext;
}

//------------------------------------------------------------------------------------------------------------------------------
// Direction Light
bool lilUseScreenSpaceShadow(int screenSpaceShadowIndex)
{
    #if defined(SCREEN_SPACE_SHADOW_INDEX_MASK) && defined(INVALID_SCREEN_SPACE_SHADOW)
        return (screenSpaceShadowIndex & SCREEN_SPACE_SHADOW_INDEX_MASK) != INVALID_SCREEN_SPACE_SHADOW;
    #else
        return screenSpaceShadowIndex >= 0;
    #endif
}

float4 lilGetDirectionalLightColor(PositionInputs posInput, DirectionalLightData light)
{
    LightLoopContext lightLoopContext = lilInitLightLoopContext();
    return EvaluateLight_Directional(lightLoopContext, posInput, light);
}

lilNPRLightingData lilGetNPRDirectionalLight(PositionInputs posInput, DirectionalLightData light)
{
    lilNPRLightingData lighting = (lilNPRLightingData)0;
    float3 L = -light.forward;
    if(light.lightDimmer > 0)
    {
        float4 lightColor = lilGetDirectionalLightColor(posInput, light);
        lightColor.rgb *= lightColor.a;

        lighting.direction = L;
        lighting.color = lightColor.rgb;
    }
    return lighting;
}

void lilBlendlilNPRLightingData(inout lilNPRLightingData dst, lilNPRLightingData src)
{
    dst.color += src.color;
    dst.direction += src.direction * Luminance(src.color);
}

float lilGetDirectionalShadow(PositionInputs posInput, float3 normalWS, uint featureFlags)
{
    float attenuation = 1.0;
    if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
    {
        HDShadowContext shadowContext = InitShadowContext();
        if(_DirectionalShadowIndex >= 0)
        {
            DirectionalLightData light = _DirectionalLightDatas[_DirectionalShadowIndex];
            #if defined(SCREEN_SPACE_SHADOWS_ON)
            if(lilUseScreenSpaceShadow(light.screenSpaceShadowIndex))
            {
                attenuation = GetScreenSpaceShadow(posInput, light.screenSpaceShadowIndex);
            }
            else
            #endif
            {
                float3 L = -light.forward;
                if((light.lightDimmer > 0) && (light.shadowDimmer > 0))
                {
                    attenuation = GetDirectionalShadowAttenuation(shadowContext, posInput.positionSS, posInput.positionWS, normalWS, light.shadowIndex, L);
                }
            }
        }
    }
    return attenuation;
}

lilNPRLightingData lilGetDirectionalLightSum(PositionInputs posInput, uint renderingLayers, uint featureFlags)
{
    lilNPRLightingData lightingData;
    lightingData.color = 0.0;
    lightingData.direction = float3(0.0, 0.001, 0.0);
    if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
    {
        for(uint i = 0; i < _DirectionalLightCount; ++i)
        {
            if((_DirectionalLightDatas[i].lightLayers & renderingLayers) != 0)
            {
                lilNPRLightingData lighting = lilGetNPRDirectionalLight(posInput, _DirectionalLightDatas[i]);
                lilBlendlilNPRLightingData(lightingData, lighting);
            }
        }
    }

    lightingData.direction = normalize(lightingData.direction);

    #ifdef LIGHT_SIMULATE_HQ
        lightingData.color = 0.0;
        if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
        {
            for(uint i = 0; i < _DirectionalLightCount; ++i)
            {
                if((_DirectionalLightDatas[i].lightLayers & renderingLayers) != 0)
                {
                    lilNPRLightingData lighting = lilGetNPRDirectionalLight(posInput, _DirectionalLightDatas[i]);
                    lightingData.color += lighting.color * saturate(dot(lightingData.direction, lighting.direction));
                }
            }
        }
    #endif

    return lightingData;
}

void lilGetLightDirectionAndColor(out float3 lightDirection, out float3 lightColor, PositionInputs posInput, uint renderingLayers, uint featureFlags)
{
    lilNPRLightingData lightingData = lilGetDirectionalLightSum(posInput, renderingLayers, featureFlags);
    lightDirection = lightingData.direction;
    lightColor = lightingData.color;
}

//------------------------------------------------------------------------------------------------------------------------------
// Punctual Light (Point / Spot)
lilNPRLightingData lilGetNPRPunctualLight(PositionInputs posInput, LightData light)
{
    lilNPRLightingData lighting = (lilNPRLightingData)0;
    float3 L;
    float4 distances;
    GetPunctualLightVectors(posInput.positionWS, light, L, distances);
    if(light.lightDimmer > 0)
    {
        LightLoopContext lightLoopContext;
        lightLoopContext.shadowContext    = InitShadowContext();
        lightLoopContext.shadowValue      = 1;
        lightLoopContext.sampleReflection = 0;
        lightLoopContext.contactShadow    = 0;

        float4 lightColor = EvaluateLight_Punctual(lightLoopContext, posInput, light, L, distances);
        lightColor.rgb *= lightColor.a * light.diffuseDimmer;

        lighting.direction = L;
        lighting.color = lightColor.rgb;
    }
    return lighting;
}

float3 lilGetPunctualLightColor(PositionInputs posInput, uint renderingLayers, uint featureFlags)
{
    float3 lightColor = 0.0;
    if(featureFlags & LIGHTFEATUREFLAGS_PUNCTUAL)
    {
        uint lightStart = 0;
        bool fastPath = false;
        #if SCALARIZE_LIGHT_LOOP
            uint lightStartLane0;
            fastPath = IsFastPath(lightStart, lightStartLane0);
            if(fastPath) lightStart = lightStartLane0;
        #endif

        uint lightListOffset = 0;
        while(lightListOffset < _PunctualLightCount)
        {
            uint v_lightIdx = FetchIndex(lightStart, lightListOffset);
            #if SCALARIZE_LIGHT_LOOP
                uint s_lightIdx = ScalarizeElementIndex(v_lightIdx, fastPath);
            #else
                uint s_lightIdx = v_lightIdx;
            #endif
            if(s_lightIdx == -1) break;

            LightData lightData = FetchLight(s_lightIdx);

            if(s_lightIdx >= v_lightIdx)
            {
                lightListOffset++;
                if((lightData.lightLayers & renderingLayers) != 0)
                {
                    lilNPRLightingData lighting = lilGetNPRPunctualLight(posInput, lightData);
                    lightColor += lighting.color;
                }
            }
        }
    }

    return lightColor;
}

//------------------------------------------------------------------------------------------------------------------------------
// Area Light (Line / Rectangle)
float3 lilGetLineLightColor(PositionInputs posInput, LightData lightData)
{
    float3 lightColor = 0.0;
    float intensity = EllipsoidalDistanceAttenuation(
        lightData.positionRWS - posInput.positionWS,
        lightData.right,
        saturate(lightData.range / (lightData.range + (0.5 * lightData.size.x))),
        lightData.rangeAttenuationScale,
        lightData.rangeAttenuationBias);
    lightColor = lightData.color * (intensity * lightData.diffuseDimmer);
    return lightColor;
}

float3 lilGetRectLightColor(PositionInputs posInput, LightData lightData)
{
    float3 lightColor = 0.0;
    #if SHADEROPTIONS_BARN_DOOR
        RectangularLightApplyBarnDoor(lightData, posInput.positionWS);
    #endif
    float3 unL = lightData.positionRWS - posInput.positionWS;
    if(dot(lightData.forward, unL) < FLT_EPS)
    {
        float3x3 lightToWorld = float3x3(lightData.right, lightData.up, -lightData.forward);
        unL = mul(unL, transpose(lightToWorld));
        float halfWidth  = lightData.size.x * 0.5;
        float halfHeight = lightData.size.y * 0.5;
        float3 invHalfDim = rcp(float3(lightData.range + halfWidth, lightData.range + halfHeight, lightData.range));
        #ifdef ELLIPSOIDAL_ATTENUATION
            float intensity = EllipsoidalDistanceAttenuation(unL, invHalfDim, lightData.rangeAttenuationScale, lightData.rangeAttenuationBias);
        #else
            float intensity = BoxDistanceAttenuation(unL, invHalfDim, lightData.rangeAttenuationScale, lightData.rangeAttenuationBias);
        #endif
        lightColor = lightData.color * (intensity * lightData.diffuseDimmer);
    }
    return lightColor;
}

float3 lilGetAreaLightColor(PositionInputs posInput, uint renderingLayers, uint featureFlags)
{
    float3 lightColor = 0.0;
    #if SHADEROPTIONS_AREA_LIGHTS
        if(featureFlags & LIGHTFEATUREFLAGS_AREA)
        {
            if(_AreaLightCount > 0)
            {
                uint i = 0;
                uint last = _AreaLightCount - 1;
                LightData lightData = FetchLight(_PunctualLightCount, i);

                while(i <= last && lightData.lightType == GPULIGHTTYPE_TUBE)
                {
                    lightData.lightType = GPULIGHTTYPE_TUBE;
                    #if defined(COOKIEMODE_NONE)
                        lightData.cookieMode = COOKIEMODE_NONE;
                    #endif
                    if((lightData.lightLayers & renderingLayers) != 0)
                    {
                        lightColor += lilGetLineLightColor(posInput, lightData);
                    }
                    lightData = FetchLight(_PunctualLightCount, min(++i, last));
                }

                while(i <= last)
                {
                    lightData.lightType = GPULIGHTTYPE_RECTANGLE;
                    if((lightData.lightLayers & renderingLayers) != 0)
                    {
                        lightColor += lilGetRectLightColor(posInput, lightData);
                    }
                    lightData = FetchLight(_PunctualLightCount, min(++i, last));
                }
            }
        }
    #endif
    return lightColor;
}

//------------------------------------------------------------------------------------------------------------------------------
// Reflection / Refraction
float3 lilGetReflectionColor(
    LightLoopContext lightLoopContext, PositionInputs posInput, float3 reflUVW, float perceptualRoughness, float3 normalDirection,
    EnvLightData lightData, int influenceShapeType, inout float hierarchyWeight)
{
    float weight = 1.0;
    EvaluateLight_EnvIntersection(posInput.positionWS, normalDirection, lightData, influenceShapeType, reflUVW, weight);
    #if VERSION_GREATER_EQUAL(10, 1)
        float4 preLD = SampleEnv(lightLoopContext, lightData.envIndex, reflUVW, PerceptualRoughnessToMipmapLevel(perceptualRoughness) * lightData.roughReflections, lightData.rangeCompressionFactorCompensation, posInput.positionNDC);
    #else
        float4 preLD = SampleEnv(lightLoopContext, lightData.envIndex, reflUVW, PerceptualRoughnessToMipmapLevel(perceptualRoughness), lightData.rangeCompressionFactorCompensation);
    #endif
    weight *= preLD.a;
    UpdateLightingHierarchyWeights(hierarchyWeight, weight);
    return preLD.rgb * weight * lightData.multiplier;
}

float3 lilGetReflectionSum(float3 viewDirection, float3 normalDirection, float perceptualRoughness, PositionInputs posInput, uint renderingLayers, uint featureFlags)
{
    float3 reflUVW = reflect(-viewDirection, normalDirection);
    LightLoopContext lightLoopContext = lilInitLightLoopContext();
    float3 specular = 0.0;
    if(featureFlags & (LIGHTFEATUREFLAGS_ENV | LIGHTFEATUREFLAGS_SKY))
    {
        float reflectionHierarchyWeight = 0.0;
        uint envLightStart = 0;

        bool fastPath = false;
        #if SCALARIZE_LIGHT_LOOP
            uint envStartFirstLane;
            fastPath = IsFastPath(envLightStart, envStartFirstLane);
        #endif

        EnvLightData envLightData;
        if(_EnvLightCount > 0)  envLightData = FetchEnvLight(envLightStart, 0);
        else                    envLightData = InitSkyEnvLightData(0);

        if(featureFlags & LIGHTFEATUREFLAGS_ENV)
        {
            lightLoopContext.sampleReflection = SINGLE_PASS_CONTEXT_SAMPLE_REFLECTION_PROBES;

            #if SCALARIZE_LIGHT_LOOP
                if(fastPath) envLightStart = envStartFirstLane;
            #endif

            uint v_envLightListOffset = 0;
            uint v_envLightIdx = envLightStart;
            while(v_envLightListOffset < _EnvLightCount)
            {
                v_envLightIdx = FetchIndex(envLightStart, v_envLightListOffset);
                #if SCALARIZE_LIGHT_LOOP
                    uint s_envLightIdx = ScalarizeElementIndex(v_envLightIdx, fastPath);
                #else
                    uint s_envLightIdx = v_envLightIdx;
                #endif
                if(s_envLightIdx == -1) break;

                EnvLightData s_envLightData = FetchEnvLight(s_envLightIdx);
                if(s_envLightIdx >= v_envLightIdx)
                {
                    v_envLightListOffset++;
                    if((reflectionHierarchyWeight < 1.0) && ((s_envLightData.lightLayers & renderingLayers) != 0))
                    {
                        specular += lilGetReflectionColor(lightLoopContext, posInput, reflUVW, perceptualRoughness, normalDirection, s_envLightData, s_envLightData.influenceShapeType, reflectionHierarchyWeight);
                    }
                }
            }
        }

        if((featureFlags & LIGHTFEATUREFLAGS_SKY) && _EnvLightSkyEnabled)
        {
            lightLoopContext.sampleReflection = SINGLE_PASS_CONTEXT_SAMPLE_SKY;
            EnvLightData envLightSky = InitSkyEnvLightData(0);
            if(reflectionHierarchyWeight < 1.0)
            {
                specular += lilGetReflectionColor(lightLoopContext, posInput, reflUVW, perceptualRoughness, normalDirection, envLightSky, envLightSky.influenceShapeType, reflectionHierarchyWeight);
            }
        }
    }
    return specular * GetCurrentExposureMultiplier();
}

//------------------------------------------------------------------------------------------------------------------------------
// Mix
float3 lilGetAdditionalLights(PositionInputs posInput, uint renderingLayers, uint featureFlags)
{
    float3 lightColor = 0.0;
    lightColor += lilGetPunctualLightColor(posInput, renderingLayers, featureFlags);
    lightColor += lilGetAreaLightColor(posInput, renderingLayers, featureFlags);
    return lightColor * 0.75;
}

#endif
#ifndef VRC_LIGHT_VOLUMES_INCLUDED
#define VRC_LIGHT_VOLUMES_INCLUDED
#define VRCLV_VERSION 2
#define VRCLV_MAX_VOLUMES_COUNT 32
#define VRCLV_MAX_LIGHTS_COUNT 128


#ifndef SHADER_TARGET_SURFACE_ANALYSIS
cbuffer LightVolumeUniforms {
#endif

// Are Light Volumes enabled on scene? can be 0 or 1
uniform float _UdonLightVolumeEnabled;
    
// Rreturns 1, 2 or other number if there are light volumes on the scene. Number represents the light volumes system internal version number.
uniform float _UdonLightVolumeVersion;

// All volumes count in scene
uniform float _UdonLightVolumeCount;

// Additive volumes max overdraw count
uniform float _UdonLightVolumeAdditiveMaxOverdraw;

// Additive volumes count
uniform float _UdonLightVolumeAdditiveCount;

// Should volumes be blended with lightprobes?
uniform float _UdonLightVolumeProbesBlend;

// Should volumes be with sharp edges when not blending with each other
uniform float _UdonLightVolumeSharpBounds;

// World to Local (-0.5, 0.5) UVW Matrix 4x4
uniform float4x4 _UdonLightVolumeInvWorldMatrix[VRCLV_MAX_VOLUMES_COUNT];

// L1 SH quaternion rotation (relative to baked rotation)
//uniform float4 _UdonLightVolumeRotationQuaternion[32];
uniform float4 _UdonLightVolumeRotation[VRCLV_MAX_VOLUMES_COUNT * 2]; // Legacy! Used in this version to have back compatibility with older worlds. Array commented above will be used in future releases! Legacy!

// Value that is needed to smoothly blend volumes ( BoundsScale / edgeSmooth )
uniform float3 _UdonLightVolumeInvLocalEdgeSmooth[VRCLV_MAX_VOLUMES_COUNT];

// AABB Bounds of islands on the 3D Texture atlas. XYZ: UvwMin, W: Scale per axis
// uniform float4 _UdonLightVolumeUvwScale[96];
uniform float3 _UdonLightVolumeUvw[VRCLV_MAX_VOLUMES_COUNT * 6]; // Legacy! AABB Bounds of islands on the 3D Texture atlas. Array commented above will be used in future releases! Legacy!

// XYZ: AABB Bounds of islands on the 3D Texture atlas storing occlusion. W: Scale factor for the occlusion volume UVW
// This is optional data. If the volume has no occlusion, the value will be (-1, -1, -1, -1).
uniform float4 _UdonLightVolumeOcclusionUvw[VRCLV_MAX_VOLUMES_COUNT];

// Color multiplier (RGB) | If we actually need to rotate L1 components at all (A)
uniform float4 _UdonLightVolumeColor[VRCLV_MAX_VOLUMES_COUNT];

// Point Lights count
uniform float _UdonPointLightVolumeCount;

// Cubemaps count in the custom textures array
uniform float _UdonPointLightVolumeCubeCount;

// For point light: XYZ = Position, W = Inverse squared range
// For spot light: XYZ = Position, W = Inverse squared range, negated
// For area light: XYZ = Position, W = Width
uniform float4 _UdonPointLightVolumePosition[VRCLV_MAX_LIGHTS_COUNT];

// For point light: XYZ = Color, W = Cos of angle (for LUT)
// For spot light: XYZ = Color, W = Cos of outer angle if no custom texture, tan of outer angle otherwise
// For area light: XYZ = Color, W = 2 + Height
uniform float4 _UdonPointLightVolumeColor[VRCLV_MAX_LIGHTS_COUNT];

// For point light: XYZW = Rotation quaternion
// For spot light: XYZ = Direction, W = Cone falloff
// For area light: XYZW = Rotation quaternion
uniform float4 _UdonPointLightVolumeDirection[VRCLV_MAX_LIGHTS_COUNT];

// X = Custom ID:
//   If parametric: X stores 0
//   If uses custom lut: X stores LUT ID with positive sign
//   If uses custom texture: X stores texture ID with negative sign
// Y = Shadowmask index. If light doesn't use shadowmask, the index will be negative.
// Z = Squared Culling Range. Just a precalculated culling range to not recalculate in in shader.
uniform float3 _UdonPointLightVolumeCustomID[VRCLV_MAX_LIGHTS_COUNT];

// If we are far enough from a light that the irradiance
// is guaranteed lower than the threshold defined by this value,
// we cull the light.
uniform float _UdonLightBrightnessCutoff;

// The number of volumes that provide occlusion data.
// We use this to take faster paths when no occlusion is needed.
uniform float _UdonLightVolumeOcclusionCount;

#ifndef SHADER_TARGET_SURFACE_ANALYSIS
}
#endif

#ifndef SHADER_TARGET_SURFACE_ANALYSIS

// Main 3D Texture atlas
uniform Texture3D _UdonLightVolume;
uniform SamplerState sampler_UdonLightVolume;
// First elements must be cubemap faces (6 face textures per cubemap). Then goes other textures
uniform Texture2DArray _UdonPointLightVolumeTexture;
// Samples a texture using mip 0, and reusing a single sampler
#define LV_SAMPLE(tex, uvw) tex.SampleLevel(sampler_UdonLightVolume, uvw, 0)

#else

// Dummy macro definition to satisfy MojoShader (surface shaders).
#define LV_SAMPLE(tex, uvw) float4(0,0,0,0)

#endif

#define LV_PI 3.141592653589793f
#define LV_PI2 6.283185307179586f

// Smoothstep to 0, 1 but cheaper
float LV_Smoothstep01(float x) {
    return x * x * (3 - 2 * x);
}

// Rotates vector by Quaternion
float3 LV_MultiplyVectorByQuaternion(float3 v, float4 q) {
    float3 t = 2.0 * cross(q.xyz, v);
    return v + q.w * t + cross(q.xyz, t);
}

// Rotates vector by Matrix 2x3
float3 LV_MultiplyVectorByMatrix2x3(float3 v, float3 r0, float3 r1) {
    float3 r2 = cross(r0, r1);
    return float3(dot(v, r0), dot(v, r1), dot(v, r2));
}

// Fast approximate inverse cosine. Max absolute error = 0.009.
// From https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/
float LV_FastAcos(float x) {
    float absX = abs(x); 
    float res = -0.156583f * absX + LV_PI * 0.5f;
    res *= sqrt(1.0f - absX); 
    return (x >= 0) ? res : (LV_PI - res);
}

// Forms specular based on roughness
float LV_DistributionGGX(float NoH, float roughness) {
    float f = (roughness - 1) * ((roughness + 1) * (NoH * NoH)) + 1;
    return (roughness * roughness) / ((float) LV_PI * f * f);
}

// Checks if local UVW point is in bounds from -0.5 to +0.5
bool LV_PointLocalAABB(float3 localUVW) {
    return all(abs(localUVW) <= 0.5);
}

// Calculates local UVW using volume ID
float3 LV_LocalFromVolume(uint volumeID, float3 worldPos) {
    return mul(_UdonLightVolumeInvWorldMatrix[volumeID], float4(worldPos, 1.0)).xyz;
}

// Linear single SH L1 channel evaluation
float LV_EvaluateSH(float L0, float3 L1, float3 n) {
    return L0 + dot(L1, n);
}

// Samples a cubemap from _UdonPointLightVolumeTexture array
float4 LV_SampleCubemapArray(uint id, float3 dir) {
    float3 absDir = abs(dir);
    float2 uv;
    uint face;
    if (absDir.x >= absDir.y && absDir.x >= absDir.z) {
        face = dir.x > 0 ? 0 : 1;
        uv = float2((dir.x > 0 ? -dir.z : dir.z), -dir.y) * rcp(absDir.x);
    } else if (absDir.y >= absDir.z) {
        face = dir.y > 0 ? 2 : 3;
        uv = float2(dir.x, (dir.y > 0 ? dir.z : -dir.z)) * rcp(absDir.y);
    } else {
        face = dir.z > 0 ? 4 : 5;
        uv = float2((dir.z > 0 ? dir.x : -dir.x), -dir.y) * rcp(absDir.z);
    }
    float3 uvid = float3(uv * 0.5 + 0.5, id * 6 + face);
    return LV_SAMPLE(_UdonPointLightVolumeTexture, uvid);
}

// Projects irradiance from a planar quad with uniform radiant exitance into L1 spherical harmonics.
// Based on "Analytic Spherical Harmonic Coefficients for Polygonal Area Lights" by Wang and Ramamoorthi.
// https://cseweb.ucsd.edu/~ravir/ash.pdf. Assumes that shadingPosition is not behind the quad.
float4 LV_ProjectQuadLightIrradianceSH(float3 shadingPosition, float3 lightVertices[4]) {
    // Transform the vertices into local space centered on the shading position,
    // project, the polygon onto the unit sphere.
    [unroll] for (uint edge0 = 0; edge0 < 4; edge0++) {
        lightVertices[edge0] = normalize(lightVertices[edge0] - shadingPosition);
    }

    // Precomputed directions of rotated zonal harmonics,
    // and associated weights for each basis function.
    // I.E. \omega_{l,d} and \alpha_{l,d}^m in the paper respectively.
    const float3 zhDir0 = float3(0.866025, -0.500001, -0.000004);
    const float3 zhDir1 = float3(-0.759553, 0.438522, -0.480394);
    const float3 zhDir2 = float3(-0.000002, 0.638694,  0.769461);
    const float3 zhWeightL1y = float3(2.1995339f, 2.50785367f, 1.56572711f);
    const float3 zhWeightL1z = float3(-1.82572523f, -2.08165037f, 0.00000000f);
    const float3 zhWeightL1x = float3(2.42459869f, 1.44790525f, 0.90397552f);

    float solidAngle = 0.0;
    float3 surfaceIntegral = 0.0;
    [loop] for (uint edge1 = 0; edge1 < 4; edge1++) {
        uint next = (edge1 + 1) % 4;
        uint prev = (edge1 + 4 - 1) % 4;
        float3 prevVert = lightVertices[prev];
        float3 thisVert = lightVertices[edge1];
        float3 nextVert = lightVertices[next];

        // Compute the solid angle subtended by the polygon at the shading position,
        // using Arvo's formula (5.1) https://dl.acm.org/doi/pdf/10.1145/218380.218467.
        // The L0 term is directly proportional to the solid angle.
        float3 a = cross(thisVert, prevVert);
        float3 b = cross(thisVert, nextVert);
        float lenA = length(a);
        float lenB = length(b);
        solidAngle += LV_FastAcos(clamp(dot(a, b) / (lenA * lenB), -1, 1));

        // Compute the integral of the legendre polynomials over the surface of the
        // projected polygon for each zonal harmonic direction (S_l in the paper).
        // Computed as a sum of line integrals over the edges of the polygon.
        float3 mu = b * rcp(lenB);
        float cosGamma = dot(thisVert, nextVert);
        float gamma = LV_FastAcos(clamp(cosGamma, -1, 1));
        surfaceIntegral.x += gamma * dot(zhDir0, mu);
        surfaceIntegral.y += gamma * dot(zhDir1, mu);
        surfaceIntegral.z += gamma * dot(zhDir2, mu);
    }
    solidAngle = solidAngle - LV_PI2;
    surfaceIntegral *= 0.5;

    // The L0 term is just the projection of the solid angle onto the L0 basis function.
    const float normalizationL0 = 0.5f * sqrt(1.0f / LV_PI);
    float l0 = normalizationL0 * solidAngle;
    
    // Combine each surface (sub)integral with the associated weights to get
    // full surface integral for each L1 SH basis function.
    float l1y = dot(zhWeightL1y, surfaceIntegral);
    float l1z = dot(zhWeightL1z, surfaceIntegral);
    float l1x = dot(zhWeightL1x, surfaceIntegral);

    // The l0, l1y, l1z, l1x are raw SH coefficients for radiance from the polygon.
    // We need to apply some more transformations before we are done:
    // (1) We want the coefficients for irradiance, so we need to convolve with the
    //     clamped cosine kernel, as detailed in https://cseweb.ucsd.edu/~ravir/papers/envmap/envmap.pdf.
    //     The kernel has coefficients PI and 2/3*PI for L0 and L1 respectively.
    // (2) Unity's area lights underestimate the irradiance by a factor of PI for historical reasons.
    //     We need to divide by PI to match this 'incorrect' behavior.
    // (3) Unity stores SH coefficients (unity_SHAr..unity_SHC) pre-multiplied with the constant
    //     part of each SH basis function, so we need to multiply by constant part to match it.
    const float cosineKernelL0 = LV_PI; // (1)
    const float cosineKernelL1 = LV_PI2 / 3.0f; // (1)
    const float oneOverPi = 1.0f / LV_PI; // (2)
    const float normalizationL1 = 0.5f * sqrt(3.0f / LV_PI); // (3)
    const float weightL0 = cosineKernelL0 * normalizationL0 * oneOverPi; // (1), (2), (3)
    const float weightL1 = cosineKernelL1 * normalizationL1 * oneOverPi; // (1), (2), (3)
    l0  *= weightL0;
    l1y *= weightL1;
    l1z *= weightL1;
    l1x *= weightL1;
    
    return float4(l1x, l1y, l1z, l0);
}

// Samples a quad light, including culling
void LV_QuadLight(float3 worldPos, float3 centroidPos, float4 rotationQuat, float2 size, float3 color, float sqMaxDist, float occlusion, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, inout uint count) {
    
    float3 lightToWorldPos = worldPos - centroidPos;
    
    // Normal culling
    float3 normal = LV_MultiplyVectorByQuaternion(float3(0, 0, 1), rotationQuat);
    [branch] if (dot(normal, lightToWorldPos) < 0.0) return;
    
    // Attenuate the light based on distance to the bounding sphere, so we don't get hard seam at the edge.
    float sqCutoffDist = sqMaxDist - dot(lightToWorldPos, lightToWorldPos);
    color.rgb *= saturate(sqCutoffDist / sqMaxDist) * LV_PI * occlusion;
        
    // Compute the vertices of the quad
    float2 halfSize = size * 0.5f;
    float3 xAxis = LV_MultiplyVectorByQuaternion(float3(1, 0, 0), rotationQuat);
    float3 yAxis = cross(normal, xAxis);
    float3 verts[4];
    verts[0] = centroidPos + (-halfSize.x * xAxis) + ( halfSize.y * yAxis);
    verts[1] = centroidPos + ( halfSize.x * xAxis) + ( halfSize.y * yAxis);
    verts[2] = centroidPos + ( halfSize.x * xAxis) + (-halfSize.y * yAxis);
    verts[3] = centroidPos + (-halfSize.x * xAxis) + (-halfSize.y * yAxis);

    // Project irradiance from the area light
    float4 areaLightSH = LV_ProjectQuadLightIrradianceSH(worldPos, verts);

    // If the magnitude of L1 is greater than L0, we may get negative values
    // when reconstructing. To avoid, normalize L1. This is effectively de-ringing.
    float lenL1 = length(areaLightSH.xyz);
    if (lenL1 > areaLightSH.w) areaLightSH.xyz *= areaLightSH.w / lenL1;
    
    L0  += areaLightSH.w * color.rgb;
    L1r += areaLightSH.xyz * color.r;
    L1g += areaLightSH.xyz * color.g;
    L1b += areaLightSH.xyz * color.b;
    
    count++;
}

// Calculates point light attenuation. Returns false if it's culled
float3 LV_PointLightAttenuation(float sqdist, float sqlightSize, float3 color, float brightnessCutoff, float sqMaxDist) {
    float mask = saturate(1 - sqdist / sqMaxDist);
    return mask * mask * color * sqlightSize / (sqdist + sqlightSize);
}

// Calculates point light solid angle coefficient
float LV_PointLightSolidAngle(float sqdist, float sqlightSize) {
    return saturate(sqrt(sqdist / (sqlightSize + sqdist)));
}

// Calculares a spherical light source
void LV_SphereLight(float sqdist, float3 dirN, float sqlightSize, float3 color, float occlusion, float sqMaxDist, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, inout uint count) {
    
    float3 att = LV_PointLightAttenuation(sqdist, sqlightSize, color, _UdonLightBrightnessCutoff, sqMaxDist);

    float3 l0 = att * occlusion;
    float3 l1 = dirN * LV_PointLightSolidAngle(sqdist, sqlightSize);
    
    L0 += l0;
    L1r += l0.r * l1;
    L1g += l0.g * l1;
    L1b += l0.b * l1;
    count++;
    
}

// Calculares a spherical spot light source
void LV_SphereSpotLight(float sqdist, float3 dirN, float sqlightSize, float3 att, float spotMask, float cosAngle, float coneFalloff, float occlusion, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, inout uint count) {
    
    float smoothedCone = LV_Smoothstep01(saturate(spotMask * coneFalloff));
    float3 l0 = att * (occlusion * smoothedCone);
    float3 l1 = dirN * LV_PointLightSolidAngle(sqdist, sqlightSize * saturate(1 - cosAngle));
    
    L0 += l0;
    L1r += l0.r * l1;
    L1g += l0.g * l1;
    L1b += l0.b * l1;
    count++;
    
}

// Calculares a spherical spot light source
void LV_SphereSpotLightCookie(float sqdist, float3 dirN, float sqlightSize, float3 att, float4 lightRot, float tanAngle, uint customId, float occlusion, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, inout uint count) {
    
    float3 localDir = LV_MultiplyVectorByQuaternion(-dirN, lightRot);
    float2 uv = localDir.xy * rcp(localDir.z * tanAngle);
    [branch] if (
        localDir.z <= 0.0 || // Culling by direction
        abs(uv.x) > 1.0 || abs(uv.y) > 1.0 // Culling by UV
    ) return;
        
    uint id = (uint) _UdonPointLightVolumeCubeCount * 5 - customId - 1;
    float3 uvid = float3(uv * 0.5 + 0.5, id);        
    float angleSize = saturate(rsqrt(1 + tanAngle * tanAngle));
    float4 cookie = LV_SAMPLE(_UdonPointLightVolumeTexture, uvid);
        
    float3 l0 = att * cookie.rgb * (cookie.a * occlusion);
    float3 l1 = dirN * LV_PointLightSolidAngle(sqdist, sqlightSize * (1 - angleSize));
    
    L0 += l0;
    L1r += l0.r * l1;
    L1g += l0.g * l1;
    L1b += l0.b * l1;
    count++;
    
}

// Calculares a spherical spot light source
void LV_SphereSpotLightAttenuationLUT(float sqdist, float3 dirN, float sqlightSize, float3 color, float spotMask, float cosAngle, uint customId, float occlusion, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, inout uint count) {
    
    float dirRadius = sqdist * abs(sqlightSize);
    float spot = 1 - saturate(spotMask * rcp(1 - cosAngle));
    uint id = (uint) _UdonPointLightVolumeCubeCount * 5 + customId - 1;
    float3 uvid = float3(sqrt(float2(spot, dirRadius)), id);
    float3 att = color.rgb * LV_SAMPLE(_UdonPointLightVolumeTexture, uvid).xyz * occlusion;
    
    L0 += att;
    L1r += dirN * att.r;
    L1g += dirN * att.g;
    L1b += dirN * att.b;
    
    count++;
    
}

// Samples a spot light, point light or quad/area light
void LV_PointLight(uint id, float3 worldPos, float4 occlusion, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, inout uint count) {
    
    // IDs and range data
    float3 customID_data = _UdonPointLightVolumeCustomID[id];
    int shadowId = (int) customID_data.y; // Shadowmask id
    int customId = (int) customID_data.x; // Custom Texture ID
    float sqrRange = customID_data.z; // Squared culling distance
    
    float4 pos = _UdonPointLightVolumePosition[id]; // Light position and inversed squared range 
    float3 dir = pos.xyz - worldPos;
    float sqlen = max(dot(dir, dir), 1e-6);
    [branch] if (sqlen > sqrRange) return; // Early distance based culling
    float3 dirN = dir * rsqrt(sqlen);
    
    // Processing lights occlusion
    float lightOcclusion = 1;
    if (_UdonLightVolumeOcclusionCount != 0 && shadowId >= 0) {
        lightOcclusion = dot(occlusion, float4(shadowId == 0, shadowId == 1, shadowId == 2, shadowId == 3));
    }
    
    float4 color = _UdonPointLightVolumeColor[id]; // Color, angle
    float4 ldir = _UdonPointLightVolumeDirection[id]; // Dir + falloff or Rotation
    
    [branch] if (pos.w < 0) { // It is a spot light

        float angle = color.w;
        float spotMask = dot(ldir, -dirN) - angle;
        [branch] if(customId >= 0 && spotMask < 0) return; // Spot cone based culling
        
        [branch] if (customId > 0) {  // If it uses Attenuation LUT
            
            LV_SphereSpotLightAttenuationLUT(sqlen, dirN, -pos.w, color.rgb, spotMask, angle, customId, lightOcclusion, L0, L1r, L1g, L1b, count);
            
        } else { // If it uses default parametric attenuation

            float3 att = LV_PointLightAttenuation(sqlen, -pos.w, color, _UdonLightBrightnessCutoff, sqrRange);

            [branch] if (customId < 0) { // If uses cookie
                
                LV_SphereSpotLightCookie(sqlen, dirN, -pos.w, att, ldir, angle, customId, lightOcclusion, L0, L1r, L1g, L1b, count);
                
            } else { // If it uses default parametric attenuation
                
                LV_SphereSpotLight(sqlen, dirN, -pos.w, att, spotMask, angle, ldir.w, lightOcclusion, L0, L1r, L1g, L1b, count);
                
            }
            
        }
        
    } else if (color.w <= 1.5f) { // It is a point light
        
        [branch] if (customId > 0) { // Using LUT
            
            float invSqRange = abs(pos.w); // Sign of range defines if it's point light (positive) or a spot light (negative)
            float dirRadius = sqlen * invSqRange;
            uint id = (uint) _UdonPointLightVolumeCubeCount * 5 + customId;
            float3 uvid = float3(sqrt(float2(0, dirRadius)), id);
            float3 att = color.rgb * LV_SAMPLE(_UdonPointLightVolumeTexture, uvid).xyz * lightOcclusion;
            
            L0 += att;
            L1r += dirN * att.r;
            L1g += dirN * att.g;
            L1b += dirN * att.b;
            
            count++;
            
        } else { // If it uses default parametric attenuation
            
            float3 l0 = 0, l1r = 0, l1g = 0, l1b = 0;
            LV_SphereLight(sqlen, dirN, pos.w, color.rgb, lightOcclusion, sqrRange, l0, l1r, l1g, l1b, count);

            float3 cubeColor = 1;
            [branch] if (customId < 0) { // If it uses a cubemap
                uint id = -customId - 1; // Cubemap ID starts from zero and should not take in count texture array slices count.
                cubeColor = LV_SampleCubemapArray(id, LV_MultiplyVectorByQuaternion(dirN, ldir)).xyz;
            }

            L0 += l0 * cubeColor;
            L1r += l1r * cubeColor.r;
            L1g += l1g * cubeColor.g;
            L1b += l1b * cubeColor.b;
        }
        
    } else { // It is an area light
        
        LV_QuadLight(worldPos, pos.xyz, ldir, float2(pos.w, color.w - 2.0f), color.rgb, sqrRange, lightOcclusion, L0, L1r, L1g, L1b, count);
        
    }

}

// Samples 3 SH textures and packing them into L1 channels
void LV_SampleLightVolumeTex(float3 uvw0, float3 uvw1, float3 uvw2, out float3 L0, out float3 L1r, out float3 L1g, out float3 L1b) {
    // Sampling 3D Atlas
    float4 tex0 = LV_SAMPLE(_UdonLightVolume, uvw0);
    float4 tex1 = LV_SAMPLE(_UdonLightVolume, uvw1);
    float4 tex2 = LV_SAMPLE(_UdonLightVolume, uvw2);
    // Packing final data
    L0 = tex0.rgb;
    L1r = float3(tex1.r, tex2.r, tex0.a);
    L1g = float3(tex1.g, tex2.g, tex1.a);
    L1b = float3(tex1.b, tex2.b, tex2.a);
}

// Bounds mask for a volume rotated in world space, using local UVW
float LV_BoundsMask(float3 localUVW, float3 invLocalEdgeSmooth) {
    float3 distToMin = (localUVW + 0.5) * invLocalEdgeSmooth;
    float3 distToMax = (0.5 - localUVW) * invLocalEdgeSmooth;
    float3 fade = saturate(min(distToMin, distToMax));
    return fade.x * fade.y * fade.z;
}

// Default light probes SH components
void LV_SampleLightProbe(inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b) {
    L0 += float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
    L1r += unity_SHAr.xyz;
    L1g += unity_SHAg.xyz;
    L1b += unity_SHAb.xyz;
}

// Applies deringing to light probes. Useful if they baked with Bakery L1
void LV_SampleLightProbeDering(inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b) {
    L0 += float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
    L1r += unity_SHAr.xyz * 0.565f;
    L1g += unity_SHAg.xyz * 0.565f;
    L1b += unity_SHAb.xyz * 0.565f;
}

// Samples a Volume with ID and Local UVW
void LV_SampleVolume(uint id, float3 localUVW, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, out float4 occlusion) {
    
    // Additive UVW
    //uint uvwID = id * 3;
    //float4 uvwPos0 = _UdonLightVolumeUvwScale[uvwID];
    //float4 uvwPos1 = _UdonLightVolumeUvwScale[uvwID + 1];
    //float4 uvwPos2 = _UdonLightVolumeUvwScale[uvwID + 2];
    //float3 uvwScale = float3(uvwPos0.w, uvwPos1.w, uvwPos2.w);
    
    //float3 uvwScaled = saturate(localUVW + 0.5) * uvwScale;
    //float3 uvw0 = uvwPos0.xyz + uvwScaled;
    //float3 uvw1 = uvwPos1.xyz + uvwScaled;
    //float3 uvw2 = uvwPos2.xyz + uvwScaled;
    
    // Legacy! Commented code above will be used in future releases! Legacy!
    uint uvwID = id * 6;
    float3 uvwScaled = saturate(localUVW + 0.5) * (_UdonLightVolumeUvw[uvwID + 1].xyz - _UdonLightVolumeUvw[uvwID].xyz);
    float3 uvw0 = uvwScaled + _UdonLightVolumeUvw[uvwID].xyz;
    float3 uvw1 = uvwScaled + _UdonLightVolumeUvw[uvwID + 2].xyz;
    float3 uvw2 = uvwScaled + _UdonLightVolumeUvw[uvwID + 4].xyz;
    
    // Sample additive
    float3 l0, l1r, l1g, l1b;
    LV_SampleLightVolumeTex(uvw0, uvw1, uvw2, l0, l1r, l1g, l1b);

    // Sample occlusion
    float4 uvwOcclusion = _UdonLightVolumeOcclusionUvw[id];
    [branch] if (uvwOcclusion.x >= 0) {
        occlusion = 1.0f - LV_SAMPLE(_UdonLightVolume, uvwOcclusion.xyz + uvwScaled * uvwOcclusion.w);
    } else {
        occlusion = 1;
    }
    
    // Color correction
    float4 color = _UdonLightVolumeColor[id];
    L0 += l0 * color.rgb;
    l1r *= color.r;
    l1g *= color.g;
    l1b *= color.b;
    
    // Rotate if needed
    if (color.a != 0) {
        //float4 r = _UdonLightVolumeRotationQuaternion[id];
        //L1r = LV_MultiplyVectorByQuaternion(L1r, r);
        //L1g = LV_MultiplyVectorByQuaternion(L1g, r);
        //L1b = LV_MultiplyVectorByQuaternion(L1b, r);
        
        // Legacy to support older light volumes worlds! Commented code above will be used in future releases! Legacy!
        float3 r0 = _UdonLightVolumeRotation[id * 2].xyz;
        float3 r1 = _UdonLightVolumeRotation[id * 2 + 1].xyz;
        L1r += LV_MultiplyVectorByMatrix2x3(l1r, r0, r1);
        L1g += LV_MultiplyVectorByMatrix2x3(l1g, r0, r1);
        L1b += LV_MultiplyVectorByMatrix2x3(l1b, r0, r1);
    } else {
        L1r += l1r;
        L1g += l1g;
        L1b += l1b;
    }
                
}

float4 LV_SampleVolumeOcclusion(uint id, float3 localUVW) {
    
    // Sample occlusion
    float4 uvwOcclusion = _UdonLightVolumeOcclusionUvw[id];
    
    [branch] if (uvwOcclusion.x >= 0) {
        //uint uvwID = id * 3;
        //float4 uvwPos0 = _UdonLightVolumeUvwScale[uvwID];
        //float4 uvwPos1 = _UdonLightVolumeUvwScale[uvwID + 1];
        //float4 uvwPos2 = _UdonLightVolumeUvwScale[uvwID + 2];
        //float3 uvwScale = float3(uvwPos0.w, uvwPos1.w, uvwPos2.w);
        //float3 uvwScaled = saturate(localUVW + 0.5) * uvwScale;
        
        // Legacy to support older light volumes worlds! Commented code above will be used in future releases! Legacy!
        uint uvwID = id * 6;
        float3 uvwScaled = saturate(localUVW + 0.5) * (_UdonLightVolumeUvw[uvwID + 1].xyz - _UdonLightVolumeUvw[uvwID].xyz);
        
        return 1.0f - LV_SAMPLE(_UdonLightVolume, uvwOcclusion.xyz + uvwScaled * uvwOcclusion.w);
    } else {
        return 1;
    }
    
}

// Calculates L1 SH based on the world position and occlusion factor. Only samples point lights, not light volumes.
void LV_PointLightVolumeSH(float3 worldPos, float4 occlusion, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b) {
    
    uint pointCount = min((uint) _UdonPointLightVolumeCount, VRCLV_MAX_LIGHTS_COUNT);
    [branch] if (pointCount == 0) return;
    
    uint maxOverdraw = min((uint) _UdonLightVolumeAdditiveMaxOverdraw, VRCLV_MAX_LIGHTS_COUNT);
    uint pcount = 0; // Point lights counter

    [loop] for (uint pid = 0; pid < pointCount && pcount < maxOverdraw; pid++) {
        LV_PointLight(pid, worldPos, occlusion, L0, L1r, L1g, L1b, pcount);
    }
    
}

// Calculates L1 SH and occlusion based on the world position. Only samples light volumes, not point lights.
void LV_LightVolumeSH(float3 worldPos, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, out float4 occlusion) {

    // Initializing output variables
    occlusion = 1;
    float4 mOcclusion = 1; // Multiplicative occlusion. Applies on top of regular occlusion
    
    // Clamping gloabal iteration counts
    uint volumesCount = min((uint) _UdonLightVolumeCount, VRCLV_MAX_VOLUMES_COUNT);
    
    //if (_UdonLightVolumeVersion < VRCLV_VERSION || volumesCount == 0 ) { // Fallback to default light probes if Light Volume are not enabled or a version is too old to have a support
    [branch] if (volumesCount == 0) { // Legacy! Fallback to default light probes if Light Volume are not enabled or a version is too old to have a support. Legacy!
        LV_SampleLightProbe(L0, L1r, L1g, L1b);
        return;
    }
    
    uint maxOverdraw = min((uint) _UdonLightVolumeAdditiveMaxOverdraw, VRCLV_MAX_VOLUMES_COUNT);
    uint additiveCount = min((uint) _UdonLightVolumeAdditiveCount, VRCLV_MAX_VOLUMES_COUNT);
    bool lightProbesBlend = _UdonLightVolumeProbesBlend;
    
    uint volumeID_A = -1; // Main, dominant volume ID
    uint volumeID_B = -1; // Secondary volume ID to blend main with

    float3 localUVW   = 0; // Last local UVW to use in disabled Light Probes mode
    float3 localUVW_A = 0; // Main local UVW
    float3 localUVW_B = 0; // Secondary local UVW
    
    // Are A and B volumes NOT found?
    bool isNoA = true;
    bool isNoB = true;
    
    // Additive volumes variables
    uint addVolumesCount = 0;
    
    // Iterating through all light volumes with simplified algorithm requiring Light Volumes to be sorted by weight in descending order
    [loop] for (uint id = 0; id < volumesCount; id++) {
        localUVW = LV_LocalFromVolume(id, worldPos);
        [branch] if (LV_PointLocalAABB(localUVW)) { // Intersection test
            [branch] if (id < additiveCount) { // Sampling additive volumes
                [branch] if (addVolumesCount < maxOverdraw) {
                    float4 occ; // Multiplicative occlusion
                    LV_SampleVolume(id, localUVW, L0, L1r, L1g, L1b, occ);
                    mOcclusion *= occ;
                    addVolumesCount++;
                } 
            } else if (isNoA) { // First, searching for volume A
                volumeID_A = id;
                localUVW_A = localUVW;
                isNoA = false;
            } else { // Next, searching for volume B if A found
                volumeID_B = id;
                localUVW_B = localUVW;
                isNoB = false;
                break;
            }
        }
    }

    // If no volumes found, using Light Probes as fallback
    [branch] if (isNoA && lightProbesBlend) {
        LV_SampleLightProbe(L0, L1r, L1g, L1b);
        occlusion *= mOcclusion;
        return;
    }
        
    // Fallback to lowest weight light volume if outside of every volume
    localUVW_A = isNoA ? localUVW : localUVW_A;
    volumeID_A = isNoA ? volumesCount - 1 : volumeID_A;

    // Volume A SH components, occlusion, and mask to blend volume sides
    float3 L0_A  = 0;
    float3 L1r_A = 0;
    float3 L1g_A = 0;
    float3 L1b_A = 0;
    float4 occlusion_A = 1;
    
    // Sampling Light Volume A
    LV_SampleVolume(volumeID_A, localUVW_A, L0_A, L1r_A, L1g_A, L1b_A, occlusion_A);
    
    float mask = LV_BoundsMask(localUVW_A, _UdonLightVolumeInvLocalEdgeSmooth[volumeID_A]);
    [branch] if (mask == 1 || isNoA || (_UdonLightVolumeSharpBounds && isNoB)) { // Returning SH A result if it's the center of mask or out of bounds
        L0  += L0_A;
        L1r += L1r_A;
        L1g += L1g_A;
        L1b += L1b_A;
        occlusion = occlusion_A;
        occlusion *= mOcclusion;
        return;
    }
    
    // Volume B SH components and occlusion
    float3 L0_B  = 0;
    float3 L1r_B = 0;
    float3 L1g_B = 0;
    float3 L1b_B = 0;
    float4 occlusion_B = 1;

    [branch] if (isNoB && lightProbesBlend) { // No Volume found and light volumes blending enabled

        // Sample Light Probes B
        LV_SampleLightProbe(L0_B, L1r_B, L1g_B, L1b_B);

    } else { // Blending Volume A and Volume B
            
        // If no volume b found, use last one found to fallback
        localUVW_B = isNoB ? localUVW : localUVW_B;
        volumeID_B = isNoB ? volumesCount - 1 : volumeID_B;
            
        // Sampling Light Volume B
        LV_SampleVolume(volumeID_B, localUVW_B, L0_B, L1r_B, L1g_B, L1b_B, occlusion_B);
        
    }

    // Lerping occlusion
    occlusion = lerp(occlusion_B, occlusion_A, mask);
    occlusion *= mOcclusion;

    // Lerping SH components
    L0  += lerp(L0_B,  L0_A,  mask);
    L1r += lerp(L1r_B, L1r_A, mask);
    L1g += lerp(L1g_B, L1g_A, mask);
    L1b += lerp(L1b_B, L1b_A, mask);

}

// Calculates L1 SH based on the world position from additive volumes only. Only samples light volumes, not point lights.
// Also returns an occlusion factor, which may be used for point light shadows.
void LV_LightVolumeAdditiveSH(float3 worldPos, inout float3 L0, inout float3 L1r, inout float3 L1g, inout float3 L1b, out float4 occlusion) {

    // Initializing output variables
    occlusion = 1;
    float4 mOcclusion = 1; // Multiplicative occlusion. Applies on top of regular occlusion
    
    // Clamping gloabal iteration counts
    uint additiveCount = min((uint) _UdonLightVolumeAdditiveCount, VRCLV_MAX_VOLUMES_COUNT);
    //if (_UdonLightVolumeVersion < VRCLV_VERSION || (additiveCount == 0 && pointCount == 0)) return;
    [branch] if (additiveCount == 0 && (uint) _UdonPointLightVolumeCount == 0) return; // Legacy!

    uint volumesCount = min((uint) _UdonLightVolumeCount, VRCLV_MAX_VOLUMES_COUNT);
    uint maxOverdraw = min((uint) _UdonLightVolumeAdditiveMaxOverdraw, VRCLV_MAX_VOLUMES_COUNT);
    
    uint volumeID_A = -1; // Main, dominant volume ID
    uint volumeID_B = -1; // Secondary volume ID to blend main with

    float3 localUVW   = 0; // Last local UVW to use in disabled Light Probes mode
    float3 localUVW_A = 0; // Main local UVW for Y Axis and Free rotations
    float3 localUVW_B = 0; // Secondary local UVW
    
    // Are A and B volumes NOT found?
    bool isNoA = true;
    bool isNoB = true;
    
    // Additive volumes variables
    uint addVolumesCount = 0;

    // Iterating through all light volumes with simplified algorithm requiring Light Volumes to be sorted by weight in descending order
    uint count = min(_UdonLightVolumeOcclusionCount == 0 ? additiveCount : volumesCount, VRCLV_MAX_VOLUMES_COUNT); // Only use all volumes if occlusion volumes are enabled
    [loop] for (uint id = 0; id < count; id++) {
        localUVW = LV_LocalFromVolume(id, worldPos);
        [branch] if (LV_PointLocalAABB(localUVW)) { // Intersection test
            [branch] if (id < additiveCount) { // Sampling additive volumes
                [branch] if (addVolumesCount < maxOverdraw) {
                    float4 occ; // Multiplicative occlusion
                    LV_SampleVolume(id, localUVW, L0, L1r, L1g, L1b, occ);
                    mOcclusion *= occ;
                    addVolumesCount++;
                } 
            } else if (isNoA) { // First, searching for volume A
                volumeID_A = id;
                localUVW_A = localUVW;
                isNoA = false;
            } else { // Next, searching for volume B if A found
                volumeID_B = id;
                localUVW_B = localUVW;
                isNoB = false;
                break;
            }
        }
    }

    // If no volumes found, or we don't need the occlusion data, we are done
    [branch] if (isNoA || _UdonLightVolumeOcclusionCount == 0) {
        occlusion *= mOcclusion;
        return;
    }
    
    // Fallback to lowest weight light volume if outside of every volume
    localUVW_A = isNoA ? localUVW : localUVW_A;
    volumeID_A = isNoA ? volumesCount - 1 : volumeID_A;

    // Sampling Light Volume A
    occlusion = LV_SampleVolumeOcclusion(volumeID_A, localUVW_A);
    float mask = LV_BoundsMask(localUVW_A, _UdonLightVolumeInvLocalEdgeSmooth[volumeID_A]);
    
    [branch] if (mask == 1 || (_UdonLightVolumeSharpBounds && isNoB)) {
        occlusion *= mOcclusion;
        return; // Returning A result if it's the center of mask or out of bounds
    }

    // Blending Volume A and Volume B
    [branch] if (isNoB) occlusion = lerp(1, occlusion, mask);
    else occlusion = lerp(LV_SampleVolumeOcclusion(volumeID_B, localUVW_B), occlusion, mask);

    occlusion *= mOcclusion;
    
}

// Calculates speculars for light volumes or any SH L1 data with privided f0
float3 LightVolumeSpecular(float3 f0, float smoothness, float3 worldNormal, float3 viewDir, float3 L0, float3 L1r, float3 L1g, float3 L1b) {
    
    float3 specColor = max(float3(dot(reflect(-L1r, worldNormal), viewDir), dot(reflect(-L1g, worldNormal), viewDir), dot(reflect(-L1b, worldNormal), viewDir)), 0);
    
    float3 rDir = normalize(normalize(L1r) + viewDir);
    float3 gDir = normalize(normalize(L1g) + viewDir);
    float3 bDir = normalize(normalize(L1b) + viewDir);
    
    float rNh = saturate(dot(worldNormal, rDir));
    float gNh = saturate(dot(worldNormal, gDir));
    float bNh = saturate(dot(worldNormal, bDir));
    
    float roughness = 1 - smoothness * 0.9f;
    float roughExp = roughness * roughness;
    
    float rSpec = LV_DistributionGGX(rNh, roughExp);
    float gSpec = LV_DistributionGGX(gNh, roughExp);
    float bSpec = LV_DistributionGGX(bNh, roughExp);
    
    float3 specs = (rSpec + gSpec + bSpec) * f0;
    float3 coloredSpecs = specs * specColor;
    
    float3 a = coloredSpecs + specs * L0;
    float3 b = coloredSpecs * 3;
    
    return max(lerp(a, b, smoothness) * 0.5f, 0.0);
    
}

// Calculates speculars for light volumes or any SH L1 data
float3 LightVolumeSpecular(float3 albedo, float smoothness, float metallic, float3 worldNormal, float3 viewDir, float3 L0, float3 L1r, float3 L1g, float3 L1b) {
    float3 specularf0 = lerp(0.04f, albedo, metallic);
    return LightVolumeSpecular(specularf0, smoothness, worldNormal, viewDir, L0, L1r, L1g, L1b);
}

// Calculates speculars for light volumes or any SH L1 data, but simplified, with only one dominant direction with provided f0
float3 LightVolumeSpecularDominant(float3 f0, float smoothness, float3 worldNormal, float3 viewDir, float3 L0, float3 L1r, float3 L1g, float3 L1b) {
    
    float3 dominantDir = L1r + L1g + L1b;
    float3 dir = normalize(normalize(dominantDir) + viewDir);
    float nh = saturate(dot(worldNormal, dir));
    
    float roughness = 1 - smoothness * 0.9f;
    float roughExp = roughness * roughness;
    
    float spec = LV_DistributionGGX(nh, roughExp);
    
    return max(spec * L0 * f0, 0.0) * 1.5f;
    
}

// Calculates speculars for light volumes or any SH L1 data, but simplified, with only one dominant direction
float3 LightVolumeSpecularDominant(float3 albedo, float smoothness, float metallic, float3 worldNormal, float3 viewDir, float3 L0, float3 L1r, float3 L1g, float3 L1b) {
    float3 specularf0 = lerp(0.04f, albedo, metallic);
    return LightVolumeSpecularDominant(specularf0, smoothness, worldNormal, viewDir, L0, L1r, L1g, L1b);
}

// Calculate Light Volume Color based on all SH components provided and the world normal
float3 LightVolumeEvaluate(float3 worldNormal, float3 L0, float3 L1r, float3 L1g, float3 L1b) {
    return float3(LV_EvaluateSH(L0.r, L1r, worldNormal), LV_EvaluateSH(L0.g, L1g, worldNormal), LV_EvaluateSH(L0.b, L1b, worldNormal));
}

// Calculates L1 SH based on the world position. Samples both light volumes and point lights.
void LightVolumeSH(float3 worldPos, out float3 L0, out float3 L1r, out float3 L1g, out float3 L1b, float3 worldPosOffset = 0) {
    L0 = 0; L1r = 0; L1g = 0; L1b = 0;
    if (_UdonLightVolumeEnabled == 0) {
        LV_SampleLightProbeDering(L0, L1r, L1g, L1b);
    } else {
        float4 occlusion = 1;
        LV_LightVolumeSH(worldPos + worldPosOffset, L0, L1r, L1g, L1b, occlusion);
        LV_PointLightVolumeSH(worldPos, occlusion, L0, L1r, L1g, L1b);
    }
}

// Calculates L1 SH based on the world position from additive volumes only. Samples both light volumes and point lights.
void LightVolumeAdditiveSH(float3 worldPos, out float3 L0, out float3 L1r, out float3 L1g, out float3 L1b, float3 worldPosOffset = 0) {
    L0 = 0; L1r = 0; L1g = 0; L1b = 0;
    if (_UdonLightVolumeEnabled != 0) {
        float4 occlusion = 1;
        LV_LightVolumeAdditiveSH(worldPos + worldPosOffset, L0, L1r, L1g, L1b, occlusion);
        LV_PointLightVolumeSH(worldPos, occlusion, L0, L1r, L1g, L1b);
    }
}

// Calculates L0 SH based on the world position. Samples both light volumes and point lights.
float3 LightVolumeSH_L0(float3 worldPos, float3 worldPosOffset = 0) {
    if (_UdonLightVolumeEnabled == 0) {
        return float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
    } else {
        float3 L0 = 0; float4 occlusion = 1;
        float3 unused_L1; // Let's just pray that compiler will strip everything x.x
        LV_LightVolumeSH(worldPos + worldPosOffset, L0, unused_L1, unused_L1, unused_L1, occlusion);
        LV_PointLightVolumeSH(worldPos, occlusion, L0, unused_L1, unused_L1, unused_L1);
        return L0;
    }
}

// Calculates L0 SH based on the world position from additive volumes only. Samples both light volumes and point lights.
float3 LightVolumeAdditiveSH_L0(float3 worldPos, float3 worldPosOffset = 0) {
    if (_UdonLightVolumeEnabled == 0) {
        return 0;
    } else {
        float3 L0 = 0; float4 occlusion = 1;
        float3 unused_L1; // Let's just pray that compiler will strip everything x.x
        LV_LightVolumeAdditiveSH(worldPos + worldPosOffset, L0, unused_L1, unused_L1, unused_L1, occlusion);
        LV_PointLightVolumeSH(worldPos, occlusion, L0, unused_L1, unused_L1, unused_L1);
        return L0;
    }
}

// Checks if Light Volumes are used in this scene. Returns 0 if not, returns 1 if enabled
float LightVolumesEnabled() {
    return _UdonLightVolumeEnabled;
}

// Returns the light volumes version
float LightVolumesVersion() {
    return _UdonLightVolumeVersion == 0 ? _UdonLightVolumeEnabled : _UdonLightVolumeVersion;
}

#endif

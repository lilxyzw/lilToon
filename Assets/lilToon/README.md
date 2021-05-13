# lilToon
Version 1.0

# Overview
A toon shader developed for Unity.  
It is compatible with all Unity lighting and adjusts the brightness to be similar to Standard Shader without losing the look of the material.

Supported Unity versions
- Unity 2017 - Unity 2021.1

Supported Shader Models
- Normal: SM4.0 / ES3.0 or later
- Lite: SM3.0 / ES2.0 or later
- Fur: SM4.0 / ES3.1+AEP / ES3.2 or later
- Tessellation: SM5.0 / ES3.1+AEP / ES3.2 or later

Supported Rendering Pipelines
- Built-in Render Pipeline (BRP)
- Lightweight Render Pipeline (LWRP)
- Universal Render Pipeline (URP)
- Refraction shaders are supported only for BRP

# Features
This shader was developed for services that use avatars and has the following features.
- Support for all lighting in Unity (The brightness calculation is almost identical to Standard Shader)
- Extensive support (Unity 2017 - 2021.1, BRP/LWRP/URP)
- Light synthesis to prevent overexposure
- Color correction function for easy avatar modification
- Show/hide switching for each material
- One-click settings with presets
- Combine up to 3 layers with main color, and Sublayer also supports GIF animation
- Features for toon (Outline, Custom shading, Rim light, MatCap, Stencil)
- Features for Photorealistic (Normal map, Smoothness, Metallic, Emission, Parallax)
- UV scrolling & rotation available for almost all textures
- Parallax compatible emission (x2 layers)
- Pseudo-fluorescence that glows in dark spaces and when there is a little light (Recommended for making eyes glow)
- Fur, Refraction, Tessellation

# License
It is available under the MIT License. Please refer to the "LICENSE" included in the package.

# Usage - Material Setup
1. Drag and drop unitypackage to the Unity window to import it.
2. Select a material from Project.
3. Select "lilToon" from "Shader" at the top of Inspector.
4. If no texture has been applied, set the texture to "Main Color".
5. To make the material transparent, change "Rendering Mode" to "Cutout" or "Transparent".

|Rendering Mode|Description|
|-|-|
|Opacue|Ignore transparency.|
|Cutout|Transparency is used, but translucent drawing is not possible.|
|Transparent|Transparency is used. If two transparent objects overlap, one of them may not be visible.|

# Common Problems
- The shadows on face are dirty.  
  → You can specify a mask texture in "Mask & Strength" to partially remove shadows.
- Outline becomes dirty.  
  → You can specify a mask texture in "Mask & Width" to partially remove outline or adjust the thickness.
- Shadows are weak in bright places.  
  → "Environment Strength" value affects the strength of shadows in bright places.

# Recommended settings outside the shader
The following settings improve the problem of different brightness in one part of the model. Also, texture transparency artifacts are removed.
- Select the mesh from the Hierarchy, unify the "Root Bone", "Bounds", and "Anchor Override" settings, and turn off "Recieve Shadows".
- Select a transparent texture from Project and check "Alpha Is Transparency" in Inspector.
- Check the "Mip Maps Preserve Coverage" checkbox for textures used in Cutout materials.

# About Lite version
The Lite version is a greatly optimized version that maintains the appearance of the normal version to some extent. It is recommended for use in crowded situations, avatar displays in world, mobile devices, etc. It is recommended to convert materials created with the normal version to the Lite version instead of setting materials directly from the Lite version for more intuitive material setting.

# Change log
## v1.0
- Opening to the public
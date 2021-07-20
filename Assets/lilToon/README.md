# lilToon
Version 1.0

# Overview
This shader is developed for services using avatars (VRChat, etc.) and has the following features.
- Easy to use (One-click settings from presets, Saving your own presets, Color correction and exporting textures)
- Beautiful (Prevent overexposure, Anti-aliased shading)
- Lightweight (Automatically rewrites shaders and turns features on / off)
- Available in various versions (Unity 2017-2021, BRP/LWRP/URP)
- Compatible with all lighting and similar in brightness to StandardShader

# Support
Supported Unity versions
- Unity 2017 - Unity 2021.2

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
- Main color x3 layers (Decal, Layer mask, Gif animation, Normal / Additive / Multiplicative / Screen blending)
- Color correction, UV Scrolling & Rotation
- Flexible shadows (2 shadows, SSS, Environment light compositing, AO mask to adjust the ease of shadowing)
- Emission x2 layers (Animation, Mask, Blinking, Color change over time, Parallax)
- Normal map x2 layers
- Specular reflection
- MatCap (Z-axis rotation cancellation, Normal / Additive / Multiplicative / Screen blending)
- Rim light
- Outline (Color specification by texture, Mask, Thickness based on vertex color and distance)
- Fur, Refraction
- Distance Clipping Canceler
- Distance Fade (Changes color according to distance)
- AudioLink (Animate materials in sync with sound in supported VRChat worlds)
- Tessellation (For video production due to high load)
- Mesh Encryption ([AvatarEncryption](https://github.com/lilxyzw/AvaterEncryption) is required)

# License
lilToon is available under the MIT License. Please refer to the `LICENSE` included in the package.

# Usage - Material Setup
1. Import lilToon into Unity using one of the following methods.  
    i. Drag and drop unitypackage to the Unity window to import it.  
    ii. Import ```https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master``` from UPM.  
2. Select a material from Project.
3. Select `lilToon` from `Shader` at the top of Inspector.
4. If no texture has been applied, set the texture to `Main Color`.
5. To make the material transparent, change `Rendering Mode` to `Cutout` or `Transparent`.
6. If you want to add more features, change the "[Shader Setting](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md#shader-setting)" in the Advanced settings.

Please refer to the [manual](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md) for more detailed settings.

# Usage - Update
1. Import lilToon into Unity using one of the following methods.  
    i. Drag and drop unitypackage to the Unity window to import it.  
    ii. Import ```https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master``` from UPM.  
2. Click `Assets/lilToon/Refresh Shaders` in the top menu bar.

# How to distribute your works that use lilToon
Shader settings are stored in the `lilToonSetting` folder, so it is recommended to include this folder in the unitypackage.  
If you have not changed shader settings, you do not need to include them.  
The following is the detailed procedure.
1. Select the folder of your works.
2. hold down ctrl and select `lilToonSetting` folder.
3. (only if you want to include shaders) hold down ctrl and select the `lilToon` folder.
4. Right click and select `Export package...`.
5. uncheck `Include Dependencies`.
6. Press `Export...` to save the unitypackage.

# Common Problems
- Material error has occurred.
  → Clicking `Assets/lilToon/Refresh Shaders` in the top menu bar may help.
- The shadows on face are dirty.  
  → You can specify a mask texture in `Mask & Strength` to partially remove shadows.
- Outline becomes dirty.  
  → You can specify a mask texture in `Mask & Width` to partially remove outline or adjust the thickness.
- Shadows are weak in bright places.  
  → `Environment Strength` value affects the strength of shadows in bright places.

# Recommended settings outside the shader
The following settings improve the problem of different brightness in one part of the model. Also, texture transparency artifacts are removed.
- Select the mesh from the Hierarchy, unify the `Root Bone`, `Bounds`, and `Anchor Override` settings, and turn off `Recieve Shadows`.
- Select a transparent texture from Project and check `Alpha Is Transparency` in Inspector.
- Check the `Mip Maps Preserve Coverage` checkbox for textures used in Cutout materials.

# About Lite version
Lite version is a greatly optimized version that maintains the appearance of the normal version to some extent. This version is recommended for avatar displays because it has no shader settings and its features are unified. It is recommended to convert materials created with Normal version to the Lite version instead of setting materials directly from Lite version for more intuitive material setting.

# References
- [UnlitWF (whiteflare)](https://github.com/whiteflare/Unlit_WF_ShaderSuite) / [MIT LICENCE](https://github.com/whiteflare/Unlit_WF_ShaderSuite/blob/master/LICENSE)  
I referred to many parts of both scripts and shaders, such as far shaders and property deletion.
- [Arktoon-Shaders (synqark)](https://github.com/synqark/Arktoon-Shaders) / [MIT LICENCE](https://github.com/synqark/Arktoon-Shaders/blob/master/LICENSE)  
I'm referred to the shadow setting part.
- [MToon (Santarh)](https://github.com/Santarh/MToon) / [MIT LICENCE](https://github.com/Santarh/MToon/blob/master/LICENSE)  
Comparing parameters when implementing `Convert to MToon (VRM)`
- [GTAvaCrypt (rygo6)](https://github.com/rygo6/GTAvaCrypt) / [MIT LICENCE](https://github.com/rygo6/GTAvaCrypt/blob/master/LICENSE)
- [Optimized inverse trigonometric function (seblagarde)](https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/)
- [視差オクルージョンマッピング(parallax occlution mapping) (コポコポ)](https://coposuke.hateblo.jp/entry/2019/01/20/043042)

# Change log
## v1.1.2
- Added custom normal map for MatCap
- Added customization of Rim Light by light direction
- One-click lighting settings for meshes and materials
- Outline width can now be set to any value
- Alpha masks can now be applied to outline
## v1.1.1
- Show warning when property alpha is 0
- Support for changing editor theme
- Added shader refresh
## v1.1
- Added Alpha Mask
- Added MatCap 2nd
- Added properties to adjust Lower brightness limit and vertex light strength
- Fixed a bug where CascadeShadow was not working properly in URP
- Fixed a bug in MatCap where "Apply Lighting" was not working properly
- Added warnings for high-load functions (refraction / POM)
- Added references
## v1.0
- Opening to the public
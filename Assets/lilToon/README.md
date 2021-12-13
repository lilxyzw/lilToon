# lilToon
Version 1.2.7

# Overview
This shader is developed for services using avatars (VRChat, etc.) and has the following features.
- Easy to use (One-click settings from presets, Saving your own presets, Color correction and exporting textures)
- Beautiful (Prevent overexposure, Anti-aliased shading)
- Lightweight (Automatically rewrites shaders and turns features on / off)
- Available in various versions (Unity 2017-2021, BRP/LWRP/URP/HDRP)
- Compatible with all lighting and similar in brightness to StandardShader

# Support
Supported Unity versions
- Unity 2017 - Unity 2021.2

Tested version
- Unity 2017.1.0f3
- Unity 2018.4.20f1 (Built-in RP / LWRP 4.0.0 / HDRP 4.0.0)
- Unity 2019.2.0f1 (Built-in RP / LWRP 6.9.0 / HDRP 6.9.0)
- Unity 2019.3.0f6  (Built-in RP / URP 7.1.8 / HDRP 7.1.8)
- Unity 2019.4.31f1 (Built-in RP / URP 7.7.1 / HDRP 7.7.1)
- Unity 2020.3.20f1 (Built-in RP / URP 10.6.0 / HDRP 10.6.0)
- Unity 2021.1.24f1 (Built-in RP / URP 11.0.0 / HDRP 11.0.0)
- Unity 2021.2.6f1 (Built-in RP / URP 12.1.2 / HDRP 12.1.2)

Supported Shader Models
- Normal: SM4.0 / ES3.0 or later
- Lite: SM3.0 / ES2.0 or later
- Fur: SM4.0 / ES3.1+AEP / ES3.2 or later
- Tessellation: SM5.0 / ES3.1+AEP / ES3.2 or later

Supported Rendering Pipelines
- Built-in Render Pipeline
- Lightweight Render Pipeline 4.0.0 - 6.9.1
- Universal Render Pipeline 7.0.0 - 12.1.1
- High Definition Render Pipeline 4.0.0 - 12.1.1

# Features
- Main color x3 layers (Decal, Layer mask, Gif animation, Normal / Additive / Multiplicative / Screen blending)
- Color correction, UV Scrolling & Rotation
- Flexible shadows (2 shadows, SSS, Environment light compositing, AO mask to adjust the ease of shadowing)
- Emission x2 layers (Animation, Mask, Blinking, Color change over time, Parallax)
- Normal map x2 layers
- Anisotropic reflection
- Specular reflection
- MatCap x2 (Z-axis rotation cancellation, Normal / Additive / Multiplicative / Screen blending)
- Rim light
- Backlight
- Outline (Color specification by texture, Mask, Thickness based on vertex color and distance)
- Fur, Refraction, Gem
- Distance Clipping Canceler
- Distance Fade (Changes color according to distance)
- AudioLink (Animate materials in sync with sound in supported VRChat worlds)
- Tessellation (For video production due to high load)
- Mesh Encryption ([AvatarEncryption](https://github.com/lilxyzw/AvaterEncryption) is required)

# License
lilToon is available under the MIT License. Please refer to the `LICENSE` included in the package.  
For more information about third party licenses, please see [Third Party Notices.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/Third%20Party%20Notices.md).

# Usage - Material Setup
1. Import lilToon into Unity using one of the following methods.  
    i. Drag and drop unitypackage to the Unity window to import it.  
    ii. Import ```https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master``` from UPM.
2. Select a material from Project.
3. Select `lilToon` from `Shader` at the top of Inspector.
4. If no texture has been applied, set the texture to `Main Color`.
5. To make the material transparent, change `Rendering Mode` to `Cutout` or `Transparent`.
6. If you want to add more features, change the [Shader Setting](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md#shader-setting) in the Advanced settings.

Please refer to the [manual](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md) for more detailed settings.

# Usage - Update
0. If you are updating from 1.1.8 or earlier to 1.2.0 or later, delete the lilToon folder before importing
1. Import lilToon into Unity using one of the following methods.  
    i. Drag and drop unitypackage to the Unity window to import it.  
    ii. Import ```https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master``` from UPM.  
2. Click `Assets/lilToon/Refresh Shaders` in the top menu bar.

# Shader variations
- lilToon : This is the normal version. Optimizes shaders by using the shader settings instead of the shader keywords.
- lilToonLite : This is a lightweight version with fixed and limited features. It is not affected by shader settings. [Details](#about-lite-version)
- lilToonMulti : This is the version that uses the local shader keyword. It is not affected by shader settings. [Details](#about-multi-version)

# How to distribute your works that use lilToon
I recommend right-clicking on the material and running `lilToon/Remove unused properties` so that the appearance does not change when shader settings are changed. It used to be necessary to include lilToonSetting, but now it is not necessary. `Shader Setting` is automatically optimized by scanning materials and animations when importing assets.  
1. Select the folder of your works.
2. (only if you want to include shaders) hold down ctrl and select the `lilToon` folder.
3. Right click and select `Export package...`.
4. uncheck `Include Dependencies`.
5. Press `Export...` to save the unitypackage.

# Common Problems
- Material error has occurred.  
  → Clicking `Assets/lilToon/Refresh Shaders` in the top menu bar may help.
- Editor error has occurred.  
  → Right-clicking the` lilToon` folder and `Reimport` may help.
- Cannot use alpha mask  
  → Please check the following
  - Check and apply the alpha mask in [Shader Setting](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md#shader-setting) in the Advanced settings.
  - Set rendering mode to `Cutout` or `Transparent`
  - Assign a texture to `Alpha Mask` in the Main Color menu
- Some functions seem to be missing.  
  → If you want to add more features, change the [Shader Setting](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md#shader-setting) in the Advanced settings.
- UI is not displayed when selecting a material / There is an error in the editor  
  → You may have a mix of older versions of lilToon. Delete the `lilToon` folder and then import unitypackage again.
- Different meshes have different lighting.  
  → Right click on your avatar and select `[lilToon] Fix Lighting` to automatically fix this.
- The shadows on face are dirty.  
  → You can specify a mask texture in `Mask & Strength` to partially remove shadows.
- Outline becomes dirty.  
  → You can specify a mask texture in `Mask & Width` to partially remove outline or adjust the thickness.
- Shadows are weak in bright places.  
  → `Environment Strength` value affects the strength of shadows in bright places.
- I don't know which ones to turn on in shader settings.  
  → The shader settings are automatically set by running `Assets/lilToon/Auto shader setting` from the top menu bar.
- Errors occur in a specific version of SRP  
  → SRP 7.0.0 or earlier cannot pick up the version number, so the shader cannot perform detailed version determination.  
  If the error occurs, you need to specify the detailed version in `lilToon/Shader/Includes/lil_common_macro.hlsl` or update to the latest version.  
  Example: HDRP 4.8.0
  ```HLSL
  #define SHADER_LIBRARY_VERSION_MAJOR 4
  #define SHADER_LIBRARY_VERSION_MINOR 8
  ```

If you have any other problems and suspect a bug, please contact me on [Twitter](https://twitter.com/lil_xyzw), [GitHub](https://github.com/lilxyzw/lilToon), or [BOOTH](https://lilxyzw.booth.pm/).  
Please refer to the following template when reporting a bug.
```
Bug: 
Reproduction method: 

# Optional
Unity version: 
Shader setting: 
VRChat World: 
Screenshots: 
Console logs: 
```

# Recommended settings outside the shader
The following settings improve the problem of different brightness in one part of the model. Also, texture transparency artifacts are removed.
- Select the mesh from the Hierarchy, unify the `Root Bone`, `Bounds`, and `Anchor Override` settings, and turn off `Recieve Shadows`.
- Select a transparent texture from Project and check `Alpha Is Transparency` in Inspector.
- Check the `Mip Maps Preserve Coverage` checkbox for textures used in Cutout materials.

# About Lite version
Lite version is a greatly optimized version that maintains the appearance of the normal version to some extent.  
This version is recommended for avatar displays because it has no shader settings and its features are unified.  
It is recommended to convert materials created with Normal version to the Lite version instead of setting materials directly from Lite version for more intuitive material setting.

# About Multi version
This is the version that uses the shader keyword and allows you to use all the features regardless of the shader settings.  
You can convert from the normal version to the Mutli version with one click.  
This also works well with the avatar display as it is not affected by shader settings.  
It is not available in Unity 2018 and earlier, but you can use it by rewriting `shader_feature_local` to `shader_feature` in the shader.  
If you use AvatarEncryption, replace `//#define LIL_FEATURE_ENCRYPTION` in `lil_replace_keywords.hlsl` with `#define LIL_FEATURE_ENCRYPTION`.

# Other
[Developer Documentation](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/DeveloperDocumentation_JP.md)
# lilToon
Version 1.7.2

# Overview
This shader is developed for services using avatars (VRChat, etc.) and has the following features.
- Easy to use (One-click settings from presets, Saving your own presets, Color correction and exporting textures)
- Beautiful (Prevent overexposure, Anti-aliased shading)
- Lightweight (Automatically rewrites shaders and turns features on / off)
- Available in various versions (Unity 2018-2023, BRP/LWRP/URP/HDRP)
- Compatible with all lighting and similar in brightness to StandardShader

# Support
Supported Unity versions
- Unity 2018.1 - Unity 2023.2

Tested version
- Unity 2018.1.0f2 (Built-in RP)
- Unity 2018.4.20f1 (Built-in RP / LWRP 4.10.0 / HDRP 4.10.0)
- Unity 2019.2.21f1 (Built-in RP / LWRP 6.9.2 / HDRP 6.9.2)
- Unity 2019.3.0f6 (Built-in RP / URP 7.1.8 / HDRP 7.1.8)
- Unity 2019.4.31f1 (Built-in RP / URP 7.7.1 / HDRP 7.7.1)
- Unity 2020.3.47f1 (Built-in RP / URP 10.10.1 / HDRP 10.10.1)
- Unity 2021.3.23f1 (Built-in RP / URP 12.1.11 / HDRP 12.1.11)
- Unity 2022.3.15f1 (Built-in RP / URP 14.0.8 / HDRP 14.0.8)
- Unity 2023.2.0a11 (Built-in RP / URP 16.0.1 / HDRP 16.0.1)

Some older versions of Unity 2021 and 2022 have problems such as not applying transforms or not rendering materials on URP/HDRP. If you have this issue, please update to the Unity version where the bug has been fixed. ( [GameObjects doesn't get rendered when using "Unlit.Unlit_UsePass" Shader](https://issuetracker.unity3d.com/issues/sphere-gameobject-doesnt-get-rendered-when-using-unlit-dot-unlit-usepass-shader) )

Some older versions of Unity 2022 and 2023 may crash when updating shaders. If you have this issue, please update to the Unity version where the bug has been fixed. ( [Crash on malloc_internal when recompiling a ShaderGraph used by another shader via UsePass](https://issuetracker.unity3d.com/issues/crash-on-malloc-internal-when-recompiling-a-shadergraph-used-by-another-shader-via-usepass) )

Supported Shader Models
- Normal: SM4.0 / ES3.0 or later
- Lite: SM3.0 / ES2.0 or later
- Fur: SM4.0 / ES3.1+AEP / ES3.2 or later
- Tessellation: SM5.0 / ES3.1+AEP / ES3.2 or later

Supported Rendering Pipelines
- Built-in Render Pipeline
- Lightweight Render Pipeline 4.0.0 - 6.9.2
- Universal Render Pipeline 7.0.0 - 16.0.1
- High Definition Render Pipeline 4.0.0 - 16.0.1

# Features
- Main color x3 layers (Decal, Layer mask, Gif animation, Normal / Additive / Multiplicative / Screen blending)
- Color correction, UV Scrolling & Rotation
- Flexible shadows (3 shadows, SSS, Environment light compositing, AO mask to adjust the ease of shadowing)
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
lilToon is available under the MIT License. Please refer to the `LICENSE` included in the package. For more information about third party licenses, please see [Third Party Notices.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/Third%20Party%20Notices.md).

# Usage - Material Setup
1. Import lilToon into Unity using one of the following methods.  
    i. Drag and drop unitypackage to the Unity window to import it.  
    ii. Import `https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master` from UPM.
2. Select a material from Project.
3. Select `lilToon` from `Shader` at the top of Inspector.
4. If no texture has been applied, set the texture to `Main Color`.
5. To make the material transparent, change `Rendering Mode` to `Cutout` or `Transparent`.

Please refer to the [manual](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md) for more detailed settings.

# Usage - Update
0. If you are updating from 1.1.8 or earlier to 1.2.0 or later, delete the lilToon folder before importing
1. Import lilToon into Unity using one of the following methods.  
    i. Drag and drop unitypackage to the Unity window to import it.  
    ii. Import `https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master` from UPM.

# How to distribute your works that use lilToon
- If you want to include shaders, it is recommended to include a shortcut to the [BOOTH](https://booth.pm/ja/items/3087170) or [GitHub](https://github.com/lilxyzw/lilToon/releases) download page, or to include the shader unitypackage as a separate file.
- The method of combining shaders and creations into a single unitypackage has been deprecated. (due to problems such as overwriting with an older version during import)

# Common Problems
- Material error has occurred.  
  → Clicking `Assets/lilToon/Refresh Shaders` in the top menu bar may help.
- Editor error has occurred.  
  → Right-clicking the` lilToon` folder and `Reimport` may help.
- Cannot use alpha mask  
  → Please check the following
  - Set rendering mode to `Cutout` or `Transparent`
  - Assign a texture to `Alpha Mask` in the Main Color menu
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

If you have any other problems and suspect a bug, please contact me on [Twitter](https://twitter.com/lil_xyzw), [GitHub](https://github.com/lilxyzw/lilToon), or [BOOTH](https://lilxyzw.booth.pm/). Please refer to the following template when reporting a bug.
```
Bug: 
Reproduction method: 

# Optional
Unity version: 
VRChat World: 
Screenshots: 
Console logs: 
```
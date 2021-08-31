# lilToon
Version 1.1.8

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
- Refraction and Gem shaders are supported only for BRP

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
6. If you want to add more features, change the [Shader Setting](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md#shader-setting) in the Advanced settings.

Please refer to the [manual](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL.md) for more detailed settings.

# Usage - Update
1. Import lilToon into Unity using one of the following methods.  
    i. Drag and drop unitypackage to the Unity window to import it.  
    ii. Import ```https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master``` from UPM.  
2. Click `Assets/lilToon/Refresh Shaders` in the top menu bar.

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
Lite version is a greatly optimized version that maintains the appearance of the normal version to some extent. This version is recommended for avatar displays because it has no shader settings and its features are unified. It is recommended to convert materials created with Normal version to the Lite version instead of setting materials directly from Lite version for more intuitive material setting.

# References
- [UnlitWF (whiteflare)](https://github.com/whiteflare/Unlit_WF_ShaderSuite) / [MIT LICENCE](https://github.com/whiteflare/Unlit_WF_ShaderSuite/blob/master/LICENSE)  
I referred to many parts of both scripts and shaders, such as far shaders and property deletion.
- [Arktoon-Shaders (synqark)](https://github.com/synqark/Arktoon-Shaders) / [MIT LICENCE](https://github.com/synqark/Arktoon-Shaders/blob/master/LICENSE)  
I'm referred to the shadow setting part.
- [MToon (Santarh)](https://github.com/Santarh/MToon) / [MIT LICENCE](https://github.com/Santarh/MToon/blob/master/LICENSE)  
Comparing parameters when implementing `Convert to MToon (VRM)`
- [GTAvaCrypt (rygo6)](https://github.com/rygo6/GTAvaCrypt) / [MIT LICENCE](https://github.com/rygo6/GTAvaCrypt/blob/master/LICENSE)
- [Multi-channel signed distance field generator](https://github.com/Chlumsky/msdfgen) / [MIT LICENCE](https://github.com/Chlumsky/msdfgen/blob/master/LICENSE.txt)
- [Optimized inverse trigonometric function (seblagarde)](https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/)
- [視差オクルージョンマッピング(parallax occlution mapping) (コポコポ)](https://coposuke.hateblo.jp/entry/2019/01/20/043042)

# Change log
## v1.1.8
- Added lilToonGem and lilToonFakeShadow
- Added UV1 for glitter
- Added property to adjust the strength of parallax during VR to glitter
- Fixed emission color space
- Fixed an issue where glitter properties were not scanned when importing
- Fixed an issue where meshes were not decrypted on some paths when using Avater Encryption
## v1.1.7
- Added color code next to HDR color picker (Unity 2019 or later)
- Added mask to main color correction
- Added distance fade to main color 2nd / 3rd
- Added glitter function
- Changed main color's color picker to HDR
- Fixed UPM import
- Fixed `Distance Fade` and `Dissolve` transparency
- Fixed error in Unity 2019 URP
- Moved some processing to the vertex shader for optimization
- Changed transform calculation
## v1.1.6
- Changed the default value of ZTest in outline from LessEqual to Less
- Fixed an issue where shadows were weakened when using `Lower brightness limit`
- Added `Fix Now` button to help box
## v1.1.5
- Added `When in trouble...`
- Improved transparency processing
- Organize the UI
- Added on / off of Z-axis rotation cancellation of MatCap
- Fixed shader folder to be movable
- Fixed an issue where opaque materials would show alpha mask properties
- Fixed some translations
## v1.1.4a
- Fixed an issue where `Setup from FBX` did not work in Unity 2017.3 or earlier, Unity 2019.3 or later
## v1.1.4
- Added auto-scan materials and animations when importing unitypackage
- Added `Auto shader setting`, which scans all materials and animations in the project and automatically optimizes Shader Setting
- Added `Remove unused properties`, optimizes materials so that turning on additional shader settings does not affect their appearance
- Added `Setup from FBX`, automatically generate materials from FBX files, apply presets, outline mask, and shadow mask
- Added lock for `Shader Setting`
- Changed the display name of some properties (`Environment Strength` → `Environment strength on shadow color`, `Fix Width` → `Fix width by distance`)
- Inspector will be preserved in play mode
- Changed the `As Unlit` parameter of the fur shader to a slider
- Added length mask to fur shader
## v1.1.3
- Fixed problem with Inspector not showing up when version check fails
- Fixed `[lilToon] Fix Lighting` breaking when an object has a Cloth component or no bones
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
# lil's Toon Shader
Version 1.0

## Download
[Github](https://github.com/lilxyzw/lil-s-Toon-Shader/releases/latest)

## How to use
1. Import the unitypackage
2. Select a material
3. Set shader to lilToon
4. If the texture is not reflected, specify the texture in "Main Color"
5. Change "Rendering Mode" to "Cutout" or "Transparent" to make the material transparent

## Feature
This shader developed for services that use avatars such as VRChat, and has the following features.
- Support Unity standard all lighting
- Adjust the shadow intensity according to the lighting to prevent failure
- Avoiding the issue of disappearing when an avatar is behind a translucent object
- Easy color change of avatar by color correction
- Show / Hide switching
- Editor can be changed according to the purpose
- One-click setting with preset
- 4 layers are combined with the main color, and the composition mode can be changed for each layer
- Features for toon shaders (Outline, Custom Shadow, Rim Light, Matcap, Stencil)
- PBR Material（Alpha Mask、Normal Map x2、Smoothness、Metallic、Emissive）
- Matcap Z-axis rotation cancellation
- 2 layers of Time change & Parallax Emission
- Shadow customization (mask, two-color shadow, gradation, color correction, blur, range, strength)
- Fur & Refraction
- Vertex color to material color or Outline thickness
- Customize rendering method (Cull, ZWrite, ZTest, Blend, Render Queue)
- No unique shader keywords, and automatic deletion of unnecessary shader keywords

## How to add editor language
Add lang_xx_xx.txt file in Editor folder

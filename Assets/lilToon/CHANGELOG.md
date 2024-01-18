# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.7.2] - 2024-01-18
### Fixed
- Toggle doesn't work well in Unity 2019

## [1.7.1] - 2024-01-17
### Fixed
- UI Error

## [1.7.0] - 2024-01-17
### Added
- UV Tile Discard feature

### Fixed
- Incorrect material versioning
- Add shader code text asset to lilcontainer

## [1.6.0] - 2023-12-31
### Added
- RimShade feature

### Fixed
- AssetPostprocessor not working properly
- Avoid problems caused by incompatibility of shader keywords

## [1.5.2] - 2023-12-30
### Fixed
- Fixed an issue where necessary vertex data might be deleted when building AssetBundle

## [1.5.1] - 2023-12-27
### Fixed
- Avoid crashes in certain Unity versions
- Support for material variants
- Reduced the frequency of material migration

## [1.5.0] - 2023-12-10
### Added
- Enabled to see property name with alt key
- Mode that treats IDMask as bitmap and Dissolve support
- `LILTOON_DISABLE_OPTIMIZATION` symbol that allows you to force disable optimization
- `Assets/lilToon/[Material] Run migration`

### Changed
- Temporary files are no longer generated under the Temp folder

### Fixed
- Wrong label in transparent mode
- MotionVector not being output correctly in URP
- Avoid Unity removing too many UV channels
- Problems when editing multiple materials
- Baking color correction does not work correctly

### Removed
- Optimization by `IPreprocessShaders`

## [1.4.1] - 2023-09-04
### Fixed
- `Fix Lighting` does not work properly when bones are missing
- Some properties cannot be saved in presets
- Duplicate TEXCOORD in custom shaders
- Tessellation does not work properly when changing scale
- Enable stencil in `DepthOnly`, `DepthNormals` and `MotionVectors` for URP

## [1.4.0] - 2023-05-12
### Added
- Mask by vertex id
- Dither
- Alpha replacement for main color 2nd/3rd
- Property search
- UV settings to AudioLink mask
- Rim light for distance fade

### Fixed
- Fixed lilToonMulti convert
- Fixed editor error in lilToonLite
- Fixed scan when animations and materials are sub-assets
- Changed to detect VRCSDK3 using Version Defines
- Fixed locking settings when using UPM
- Fixed save directory for presets when imported with UPM
- Fixed emission not working properly
- Fixed UV not being clamped when texture is not set for main color 2nd/3rd
- Fixed unable to get the correct version of SLZURP
- Worked around an issue that conflicted with URP's variant reduction feature
- Fixed possible shader error when setting alpha mask to replace mode
- Fixed script error in Unity 2018
- Fixed bounds becoming too large with `Fix Lighting`
- Fixed `_Color` to be clamped in MToon conversion
- Fixed incorrect depth output when `Disable in VR` is on

### Changed
- Moved csc.rsp to the same directory as asmdef

## [1.3.7] - 2023-01-17
### Added
- Support for `VRChat Package Manager`
- `Blending Mode` to rim light and emission
- Support for `SLZURP`
- `Anti-aliasing shading` property

### Fixed
- Fixed error in fakeshadow
- Fixed AO in cutout fur
- Fixed refraction blur strength depending on FOV
- Fixed value after optimization
- Fixed an error in Forward+ of URP 14.0.4

## [1.3.6] - 2022-09-10
### Fixed
- Fixed shader property stringification bug
- Fixed UV for reflection shader
- Fixed missing localization

## [1.3.5] - 2022-09-06
### Added
- LightMode override
- `Tiling & Offset` and `Blend Mode (Add / Subtract)` for alpha mask
- `LUT` for shadow color
- Support for `Forward+` (URP)
- Added option to reflect light emission in lightbake
- Lock for `Shader Setting`
- Bug report generator (`GameObject/lilToon/[Debug] Generate bug report`)

### Fixed
- Fixed error in VRChat World SDK
- Fixed `Vertex Light Strength` being ignored
- Fixed custom shaders not being scanned during optimization
- Fixed light layers in URP
- Fixed UV calculation when resolution is changed with `ScalableBufferManager.ResizeBuffers()`

## [1.3.4] - 2022-07-30
### Added
- Warning when using variants with GrabPass or geometry shader in project for cluster
- ChilloutVR avatar build optimization

### Fixed
- Fixed an issue that could cause script errors
- Fixed color value after optimization
- Fixed ForwardAdd lighting
- Fixed an issue where additional lights would not work after build on URP

## [1.3.3] - 2022-07-27
### Added
- `UV Mode` for glitter mask

### Fixed
- Fixed that the main color 2nd and 3rd dissolve may not work properly after optimization
- Fixed an issue where shaders might not work well after optimization under some conditions
- Fixed ambient lighting in URP
- Fixed an issue with mixed line feed codes on Mac
- Fixed an error in lilToonMulti
- Fixed shader rewriting at startup not working properly in OpenGL

## [1.3.2] - 2022-07-20
### Added
- `UV Mode` for normal map 2nd
- `Cull Mode` for main color 2nd / 3rd
- `Receive Shadow` to outline highlight
- Extended 2 pass transparent shader
- Function to make variables constant in VRChat avatar builds
- Dialog if old package is imported

### Changed
- Changed the sampler of the normal map 2nd to `Repeat`
- Changed to move the calculation of point light, spot light and area light to the pixel shader in HDRP and also calculate the light direction
- Adjusted GUI a little
- Optimized build time
- Formatted `lilToonSetting.json`
- Changed to use ForwardAdd pass in 2 pass transparent shader
- Unified file names for texture baking

### Fixed
- Fixed a error in gem shader's GUI
- Fixed to use RGB channel in matcap 2nd mask
- Fixed toolchips
- Fixed the material not being fixed in `Fix Lighting`
- Fixed dissolve sometimes being removed during shader optimization

## [1.3.1] - 2022-06-28
### Added
- Option to not optimize during VRChat avatar test build

### Fixed
- Fixed a problem that drawing cannot be done correctly when the texture import setting is Clamp in the main color 2nd and 3rd.
- Fixed an issue where the maximum number of textures was exceeded when using `OpenGLES2`,` OpenGLES3`, and `OpenGLCore` for the API on the editor.

## [1.3.0] - 2022-06-25
### Added
- `Receive Shadow` to 2nd / 3rd shadows
- `Flat` to shadow mask type
- `Z Bias`, `Highlight`, `Remove 0 width vertices` and `Disable in VR` to outline
- `Shadow Caster Bias` to rendering settings
- Fur mesh division type
- `GSAA` and `Blending Mode` to reflection
- `LOD` to strength / blur / AO mask of shadows
- `Main Color Power` to backlight, MatCaps, rim light, emissions
- `Mask` for shape and `Randomize` for particle size to glitter
- The function to fill the backface with a single color
- Metal MatCap and preset
- Circular tangent map
- Shape textures for glitter
- Avoid errors when overwriting with an older version
- Extension of custom shader function
- URP and HDRP compatible 2 pass fur shader

### Changed
- Improved alpha mask GUI
- Changed the `Fix width by distance` of the outline to be adjustable steplessly
- Adjusted the initial value of `Light Direction Override`
- Changed to set shader settings automatically
- Split `Contrast` into `Density` and `Sensitivity`
- Changed POM to parameter
- Expanded preset
- Changed to save `Setup from FBX` presets in shader settings
- Changed to preserve `Light Direction Override` property in `Fix lighting`
- Retune fur preset
- Improved lighting with Spot Light and Point Light in URP

### Fixed
- Fixed an issue where fog wasn't working well in URP
- Fixed baking process when using other than PNG / JPG format for texture
- Added `[MainTexture]` to the `_MainTex` property
- Fixed an issue where `Light Direction Override` was affecting ShadeSH9 calculations
- Fixed an issue where lilToonMulti shader variants were being reduced too much at build
- Fixed outline mask not working well in lilToonLite

### Removed
- Support for Unity 2017

## [1.2.12] - 2022-03-31
### Fixed
- Fixed an issue where custom shaders would give an error in `Unity 2019.4.10f1` and earlier
- Fixed an issue where the ForwardAdd path did not work well in custom shaders

## [1.2.11] - 2022-03-28
### Added
- `Ignore border properties` property for AO Map (blend AO Map after toon processing)
- `Post Contrast` property for glitter
- `UV Mode` property for main color 2nd / 3rd
- An option to determine the normal direction using vertex color for outline
- New format for custom shaders

### Changed
- Migrated the contents of developer documentation to online
- Removed template for legacy custom shaders

### Fixed
- Fixed shadows not being applied in the ForwardAdd pass of lilToonMulti
- Fixed the `Normal Map Strength` of rim light and MatCap not working well in gem
- Fixed the appearance of fur in ForwardAdd
- Fixed Z-fighting for 2 pass fur
- Fixed an issue where polygons might disappear at the edge of the field of view in the tessellation shader
- Fixed behavior in VR (URP / HDRP)
- Changed the method of`Remove unused properties` to via SerializedObject

## [1.2.10] - 2022-03-06
### Added
- Added an editor (`Window/_lil/[Beta] lilToon Multi-Editor`) that allows editing multiple shader variants at the same time

### Changed
- Disable the `Distance Clipping Canceller` when the near clip value is large
- Replace `UnpackNormalScale()` to `lilUnpackNormalScale()`
- Changed to correct the SH light direction

### Fixed
- Fixed an issue where the normal map scale was not applied in the fur shader
- Fixed the multi-editing of some properties
- Fixed an issue where RenderQueue changed in lilToonMulti would revert back when restarting UnityEditor
- Fixed an issue where `Custom Safety Fallback` settings were not copied in `Remove Unused Properties`
- Fixed `VRCFallback` tag not being saved in some cases

## [1.2.9] - 2022-02-04
### Added
- Added cubemap fallback / override
- Added preset button to MatCap UV settings

### Changed
- Moved shader settings to tab

### Fixed
- Fixed an issue where `Fix lighting` wasn't showing up in the right-click menu
- Fixed refraction shader getting too bright in ForwardAdd

## [1.2.8] - 2022-01-30
### Added
- 3rd shadow
- `Randomize` property for fur
- Add the same properties to the fur as a normal shader
- `Optimize for submission to the event` button and the function to automatically optimize shader settings before build (only available when VRCSDK for world is imported)
- Reduce the build size of shaders that use shader keywords
- 2 pass (cutout & transparent) fur shader
- Default settings for material parameters applied by`[lilToon] Fix Lighting`
- Added shader settings to reduce build size
- Added `Object following` to light direction override

### Changed
- Changed to use G channel of shadow blur map as strength of 2nd shadow blur
- Changed distance clipping canceller of lilToonMulti to parameter
- Changed to apply spotlight shape even with transparent fur shader
- Changed to apply the ForwardAdd pass to the outline
- Changed the default value of `Vertex Light Strength` to 0
- Changed the default value of `Lower brightness limit` to 0.05
- Changed the name of the lighting preset from `Stable` to `Semi-monochrome`
- Changed to apply shadow even in ForwardAdd pass
- Adjusted GUI a little
- Changed to apply shader settings automatically when the shader settings menu is closed
- Changed the cutout value range from -0.001 to 1.001
- Organized the right-click menu
- Changed the outline so that it is not blurred by DOF

### Fixed
- Fixed the `Upper brightness limit` not being able to be changed in ForwardAdd pass
- Fixed an issue where the refraction shader blur strength had changed with the Clipping distance

## [1.2.7] - 2021-12-13
### Added
- Normal map for adjusting the direction of pushing out the outline
- Outline / Fur only shader
- Property to adjust the strength of parallax during VR to rim light

### Changed
- Show render queue in base settings as well
- Disable stencil in HDRP
- Changed so that fur shadows appear stronger in HDRP

### Fixed
- Fixed refraction shader behavior in Single Pass Instanced
- Fixed render queue for transparent shaders in HDRP
- Fixed motion vector in HDRP

## [1.2.6] - 2021-12-01
### Added
- Custom UV for some textures
- `Normal Map Strength` to each function
- `Blur` properties for MatCap
- Adjustment properties to alpha mask and AO mask
- Properties for non-AudioLink compatible worlds
- Spectrum display for AudioLink
- A feature to customize shader safety fallback in VRChat

### Changed
- Changed to use G channel of AO map as range of 2nd shadow color
- Changed to use each RGB channel of a MatCap mask
- Changed to remove custom UVs that had little impact on performance from shader settings and always enable them
- Changed sampler to trilinear

### Fixed
- Fixed an issue where the ForwardAdd path did not reflect the transparency of the refraction shader
- Fixed refraction / gem shader rendering
- Fixed an issue where the tessellation shader wasn't handling fogs well
- Fixed an issue where MatCap uv calculation of lite version wasn't working well in HDRP

## [1.2.5] - 2021-11-21
### Fixed
- Fixed an issue where the fur shader wasn't handling fogs well

## [1.2.4] - 2021-11-20
### Added
- `Border` and `Blur` properties for toon specular
- UV Mode in Emission
- `uint vertexID : SV_VertexID` to the input of the appdata structure
- Support for Light Layers (URP)

### Changed
- Changed the display timing of the dialog for the function to scan shader settings from material animation
- Adjusted GUI a little
- Reduced the number of interpolators
- Changed algorithm for toon specular
- Changed function when skipping outline of lilToonMulti

### Fixed
- Fixed an issue where the worlds that do not support AudioLink could not be determined properly
- Fixed an issue where the environment light strength property was not applied in the fur shader
- Fixed ZWrite being turned off when switching rendering mode in transparent fur shader of lilToonMulti
- Fixed behavior when UV scrolling and rotation are used together

## [1.2.3] - 2021-10-30
### Added
- Extended UV settings for MatCap
- Anisotropic reflection
- Changed main color properties to be saved in _BaseMap, _BaseColorMap and _BaseColor
- `Multi Light Specular` property
- `Backface Mask` property to MatCap, rim light, glitter, and backlight

### Changed
- Changed the RenderQueue of the opaque material in the Stencil settings button to match AlphaTest

### Fixed
- Fixed an issue where the preset RenderQueue was not being applied
- Fixed fur masking to be applied after fur processing
- Fixed an issue where shader rewriting was not being done properly in the UPM version
- Fixed missing localization
- Fixed an issue where a material would become dark in Planer Reflection when using Exposure in HDRP (HDRP 11.0.0 or later)
- Fixed an issue that caused parallax in distance fade

## [1.2.2] - 2021-10-18
### Added
- Add a button to change lighting settings with one click

### Fixed
- Fixed an issue where MatCap's custom normal map was not working
- Fixed an issue with tessellation splitting at long range
- Fixed an issue where some properties might not be scanned in the auto shader setting

## [1.2.1] - 2021-10-17
### Fixed
- Fixed an issue where the fur length property was not working
- Fixed log date

## [1.2.0] - 2021-10-17
### Added
- New support for HDRP
- `Upper brightness limit` and `Monochrome lighting` and `Light Direction Override` properties
- Backlight function
- `Root Width` property to fur shader
- `Shadow Mask` and `VR Parallax Strength` property to MatCap
- Menu to the property blocks (multiple properties can now be copied and pasted at once)
- Function for shifting the backface UV
- One pass / two pass variations to the transparent shader
- Shader variation that use shader keywords
- Warning when selecting anything other than a material while the preset save window is open
- Macros for creating custom shaders
- Template of custom shader

### Changed
- Changed RenderQueue of transparent shader to 2460 (added space for stencil)
- Changed RenderQueue of refraction shader to 2900 (avoid hiding through transparent materials)
- Changed to allow negative values for refraction strength
- Changed MatCap UVs to face the front even at the edge of the screen
- Renamed `Main Color Power` to` Contrast` in Shadow Setting
- Supports switching of `AlphaToMask`
- Changed `RenderType` of transparent shader to` Transparent Cutout`
- Changed the default value of `Environment strength on shadow color` to 0
- Move change log to `CHANGELOG.md` and `CHANGELOG_JP.md`

### Fixed
- Fixed an issue where Parent Constraint could not be edited in play mode
- Fixed an issue where refraction shaders could interfere with some assets under certain conditions
- Fixed an issue where the SamplerState of the main texture was not being retrieved properly

### Removed
- Some hlsl files

## [1.1.8] - 2021-08-31
### Added
- lilToonGem and lilToonFakeShadow
- UV1 for glitter
- Property to adjust the strength of parallax during VR to glitter

### Fixed
- Fixed emission color space
- Fixed an issue where glitter properties were not scanned when importing
- Fixed an issue where meshes were not decrypted on some paths when using Avater Encryption

## [1.1.7] - 2021-08-29
### Added
- Color code next to HDR color picker (Unity 2019 or later)
- Mask to main color correction
- Distance fade to main color 2nd / 3rd
- Glitter function

### Changed
- Changed main color's color picker to HDR
- Changed transform calculation
- Moved some processing to the vertex shader for optimization

### Fixed
- Fixed UPM import
- Fixed `Distance Fade` and `Dissolve` transparency
- Fixed error in Unity 2019 URP

## [1.1.6] - 2021-08-17
### Added
- `Fix Now` button to help box

### Changed
- Changed the default value of ZTest in outline from LessEqual to Less

### Fixed
- Fixed an issue where shadows were weakened when using `Lower brightness limit`

## [1.1.5] - 2021-08-08
### Added
- Added `When in trouble...`
- Added on / off of Z-axis rotation cancellation of MatCap

### Changed
- Improved transparency processing
- Organize the UI

### Fixed
- Fixed shader folder to be movable
- Fixed an issue where opaque materials would show alpha mask properties
- Fixed some translations

## [1.1.4] - 2021-07-31
### Added
- Auto-scan materials and animations when importing unitypackage
- `Auto shader setting`, which scans all materials and animations in the project and automatically optimizes Shader Setting
- `Remove unused properties`, optimizes materials so that turning on additional shader settings does not affect their appearance
- `Setup from FBX`, automatically generate materials from FBX files, apply presets, outline mask, and shadow mask
- Length mask to fur shader
- Lock for `Shader Setting`

### Changed
- Changed the display name of some properties (`Environment Strength` → `Environment strength on shadow color`, `Fix Width` → `Fix width by distance`)
- Inspector will be preserved in play mode
- Changed the `As Unlit` parameter of the fur shader to a slider

## [1.1.3] - 2021-07-24
### Fixed
- Fixed problem with Inspector not showing up when version check fails
- Fixed `[lilToon] Fix Lighting` breaking when an object has a Cloth component or no bones

## [1.1.2] - 2021-07-20
### Added
- Custom normal map for MatCap
- Customization of Rim Light by light direction
- One-click lighting settings for meshes and materials

### Changed
- Outline width can now be set to any value
- Alpha masks can now be applied to outline

## [1.1.1] - 2021-07-16
### Added
- Shader Refresh in menu
- Show warning when property alpha is 0

### Fixed
- Support for changing editor theme

## [1.1] - 2021-07-15
### Added
- Alpha Mask
- MatCap 2nd
- Properties to adjust Lower brightness limit and vertex light strength
- Warnings for high-load functions (refraction / POM)

### Fixed
- Fixed a bug where CascadeShadow was not working properly in URP
- Fixed a bug in MatCap where "Apply Lighting" was not working properly
- Added references

## [1.0] - 2021-07-12 [YANKED]
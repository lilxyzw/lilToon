# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
<div class="window_info">&#x26a0; Translation is in progress</div>

# Overview

<div class="align-center">

## Download
Shader: [BOOTH](https://lilxyzw.booth.pm/items/3087170) / [GitHub](https://github.com/lilxyzw/lilToon/releases)  
Logo: [GitHub](https://github.com/lilxyzw/lilToon/tree/master/logo)

## Features
lilToon is a shader developed for avatar-based services and has the following features

</div>

<div class="flexwrapcontainer">
<div class="flex2">
<h3>Easy</h3>
<p>Supports one-click settings from presets and saving of your own presets. Once a material has been created, it can be set again with a single click. lilToon also includes a color correction function for avatar modification, and the created colors can be exported as textures.</p>
</div>

<div class="flex2">
<h3>Beautiful</h3>
<p>The features of modern illustrations have been thoroughly researched and a wide variety of functions are provided to express them on the shader. Anti-aliased shading enables smooth reproduction of shadows like those in animation. It also prevents overexposure and transparency over translucent objects such as water.</p>
</div>

<div class="flex2">
<h3>Lightweight</h3>
<p>The editor automatically rewrites shaders to turn functions on and off and exclude unnecessary functions to minimize the load. The build size is also minimized in an effort to reduce the avatar size in VRSNS.</p>
</div>

<div class="flex2">
<h3>Stable</h3>
<p>It is compatible with all types of Unity lighting, so the brightness is close to that of Standard Shader. It also supports a wide range of Unity versions for use in a variety of environments over a long period of time, reducing the need for shader migration. (Unity2017 - 2021, BRP/LWRP/URP/HDRP)</p>
</div>
</div>

<div class="bg-black">
<div class="align-center">

## Support
A wide range of Unity versions are supported for use in a variety of environments over a long period of time.

</div>

<div class="flexcontainer">
    <div class="flex3">
        <h3>Unity Version</h3>
        <ul>
            <li>2018.1 - 2018.4</li>
            <li>2019.1 - 2019.4</li>
            <li>2020.1 - 2020.3</li>
            <li>2021.1 - 2021.3</li>
            <li>2022.1</li>
        </ul>
    </div>
    <div class="flex3">
        <h3>Shader model</h3>
        <ul>
            <li>Normal : SM4.0・ES3.0</li>
            <li>Lite : SM3.0・ES2.0</li>
            <li>Fur : SM4.0・ES3.1+AEP・ES3.2</li>
            <li>Tessellation : SM5.0・ES3.1+AEP・ES3.2</li>
        </ul>
    </div>
    <div class="flex3">
        <h3>Render Pipeline</h3>
        <ul>
            <li>Built-in Render Pipeline</li>
            <li>Lightweight Render Pipeline</li>
            <li>Universal Render Pipeline</li>
            <li>High Definition Render Pipeline</li>
        </ul>
    </div>
</div>

<div class="small-container">

**Tested versions**
- Unity 2018.1.0f2 (Built-in RP)
- Unity 2018.4.20f1 (Built-in RP / LWRP 4.10.0 / HDRP 4.10.0)
- Unity 2019.2.21f1 (Built-in RP / LWRP 6.9.2 / HDRP 6.9.2)
- Unity 2019.3.0f6  (Built-in RP / URP 7.1.8 / HDRP 7.1.8)
- Unity 2019.4.31f1 (Built-in RP / URP 7.7.1 / HDRP 7.7.1)
- Unity 2020.3.36f1 (Built-in RP / URP 10.9.0 / HDRP 10.9.0)
- Unity 2021.3.4f1 (Built-in RP / URP 12.1.7 / HDRP 12.1.7)
- Unity 2022.1.5f1 (Built-in RP / URP 13.1.8 / HDRP 13.1.8)

</div>
</div>

## Main Features
- [UV Scroll & Rotate](/en-us/base/uv.md) - Various settings can be made, such as tiling, animation, and different UVs for the front and back of polygons.
- [Main Color](/en-us/color/maincolor.md) - Supports color change and texture export with color correction for avatar creation.
- [Main Color 2nd / 3rd](/en-us/color/maincolor_layer.md) - Decal, Detail, Layer Mask, Gif Animation, Distance Fade, and various compositing modes are supported.
- [Alpha Mask](/en-us/color/alphamask.md) - It can be burned into the alpha channel of the main texture and exported.
- [3 Shadows](/en-us/color/shadow.md) - It is possible to specify colors by texture, set border colors, and adjust the ease of shadow generation by AO masks.
- [2 Emissions](/en-us/color/emission.md) - It supports various expressions such as animation, masks, blinking, color change over time, parallax, etc.
- [2 Normal Maps](/en-us/reflections/normal.md) - The UV and scale can be adjusted individually, so that the first image can be used to adjust rough shadows, and the second image can be used to add fine details.
- [Anisotropic reflection](/en-us/reflections/anisotropy.md) - This can be applied to both reflection and MatCaps respectively. Complex textures such as hair and hairline finish can be expressed.
- [Backlight](/en-us/reflections/backlight.md) - It enables the expression of light shining in from behind that is often used in illustrations.
- [Reflections](/en-us/reflections/reflection.md) - It can express everything from illustration-style highlights to photorealistic reflections. CubeMap fallback and override are also supported.
- [2 MatCaps](/en-us/reflections/matcap.md) - It supports Z-axis rotation cancellation, perspective correction, UV1 blending for angel ring, and various blend modes.
- [Rim Light](/en-us/reflections/rimlight.md) - The color, blur amount, and range can be adjusted for the sun and shadow areas individually according to the direction of the light.
- [Glitter](/en-us/reflections/glitter.md) - It supports complex shining expressions such as glitter.
- [Gem](/en-us/reflections/gem.md) - It supports the expression of gems, which is difficult to express with ordinary shaders.
- [Outline](/en-us/advanced/outline.md) - It supports color specification by texture, masking, and distance-based thickness correction. Smooth rendering with vertex color is also possible for hard-edged models.
- [Parallax](/en-us/advanced/parallax.md) - By shifting the UV according to the direction of gaze, the image appears three-dimensional.
- [Distance Fade](/en-us/advanced/distancefade.md) - It can be used to express ambient occlusion by darkening the material when approaching, or it can be used like fog.
- [AudioLink](/en-us/advanced/audiolink.md) - Materials can be animated in sync with sound in supported VRChat worlds.
- [Dissolve](/en-us/advanced?id=dissolve.md) - It can be used in a variety of ways, including transitions and partial melting expressions.
- [AvatarEncryption](/en-us/advanced/encryption.md) - Ripping is prevented by encrypting the mesh using four keys.
- [Tessellation](/en-us/advanced/tessellation.md) - This function is mainly for video production, smoothing the model when approaching.
- [Refraction](/en-us/advanced/refraction.md) - Refraction expression like glass is possible.
- [Fur](/en-us/advanced/fur.md) - It allows for the expression of complex materials such as fur.
- Distance clipping canceller - Cancels culling if you get too close, especially effective in VR. It can be used by turning it on in the shader settings.
- [VRChat](/en-us/base/vrchat.md) - You can select the shader to fallback to if the shader is blocked by the safety system.

## License
This shader is published under the [MIT License](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/LICENSE). For third party licenses, see [Third Party Notices.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/Third%20Party%20Notices.md).
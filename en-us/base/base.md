# Base Setting

## Overview
These are settings related to basic rendering. This is a collection of properties that should be set first, such as shader type (opaque, translucent, refractive, etc.) and toggling between single-sided and double-sided rendering.

## Parameters

|Name|Description|
|-|-|
|Rendering Mode|The settings for drawing types such as opaque, cutout, transparent, and refractive.|
|Cutoff|Pixels are not rendered if alpha is less than this value.|
|Cull Mode|Draws only on the specified side. Single-sided rendering performs better than double-sided rendering.|
|Flip Backface Normal|Reverses lighting and other processes when the surface is on the backface.|
|Backface Force Shadow|This is the degree to force darkening of the backface. Use this setting when you feel that the backside of clothing is unnaturally bright.|
|Color|This is the color that fills the backface.|
|Invisible|When on, the material is hidden.|
|ZWrite|This is whether to write depth or not. Basically, this should be turned on, but turning it off may improve rendering problems in transparent materials.|
|Render Queue|This number is used to determine the order in which materials are drawn. The larger the value, the later the material is rendered. If transparent materials overlap each other and one of them disappears, increasing the Render Queue of the material displayed in the foreground may improve the situation.|

## Rendering Mode

|Rendering Mode|Description|
|-|-|
|Opaque|Ignore transparency.|
|Cutout|It uses transparency but cannot draw translucent.|
|Transparent|Use transparency. When transparent objects overlap each other, one of them may become invisible. Use for facial expressions, hair transparency, etc.|
|[high-load] Refraction|Transparent areas will appear distorted.|
|[high-load] Refraction Blur|In addition to refraction, frosted glass-like blurring can also be achieved.|
|[high-load] Fur|Renders like fur. This gives a softer impression, but may result in unnatural rendering of the front-back relationship.|
|[high-load] Fur (Cutout)|Renders like fur. The cutout rendering gives a hard impression, but this is somewhat improved in environments where anti-aliasing is applied.|
|[high-load] Fur (2 pass)|Renders like fur. Blending transparent and cutout fur mitigates the disadvantages of each.|
|[high-load] Gem|Renders complex refractions.|
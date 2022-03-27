# シェーダーの構造
## シェーダーバリエーションについて
基本的には`ltspass_xx.shader`がシェーダー本体になっていて、各シェーダーバリエーションからUsePassでパスを呼び出す形になっています。  
ただし以下のシェーダーは例外的に固有のパスを持っています。
- lts_fakeshadow.shader
- lts_fur.shader
- lts_fur_cutout.shader
- lts_gem.shader
- lts_ref.shader
- lts_ref_blur.shader
- ltsmulti.shader
- ltsmulti_fur.shader
- ltsmulti_gem.shader
- ltsmulti_ref.shader

## パスについて
非常に多くのバリエーションがありますが、シェーダー冒頭の`HLSLINCLUDE`でシェーダーバリエーションごとのマクロを宣言し、各パスで`#include "Includes/lil_pass_xx.hlsl"`を呼び出してマクロに応じた分岐をさせることでなるべくコードを共通化しています。また、Built-in RP / LWRP / URP / HDRPのシェーダーが共通になっていますが、スクリプトから各シェーダーと`lil_pipeline.hlsl`を書き換えることでパイプラインの切り替えをできるようになっています。

## includeについて
各パスで`#include "Includes/lil_pass_xx.hlsl"`を呼び出していますが、このパスごとのhlslファイルは基本的に以下の構造で統一されています。
- lil_pass_xx.hlsl
    - lil_common.hlsl
        - lil_setting.hlsl
        - lil_common_macro.hlsl
        - lil_common_input.hlsl
        - lil_common_functions.hlsl
    - lil_common_appdata.hlsl
    - 各パスごとのv2f構造体の宣言
    - lil_common_vert.hlsl
    - lil_common_frag.hlsl
    - ピクセルシェーダー

## シェーダーキーワードについて
lilToonMultiでは以下のシェーダーキーワードを使用しています。  
Built-inのシェーダーに合わせているのでシェーダーキーワードの枯渇を回避しつつVRCSDKでも警告が出ないようになっています。
```
ETC1_EXTERNAL_ALPHA
UNITY_UI_ALPHACLIP
UNITY_UI_CLIP_RECT
EFFECT_HUE_VARIATION
_COLORADDSUBDIFF_ON
_COLORCOLOR_ON
_SUNDISK_NONE
GEOM_TYPE_FROND
_COLOROVERLAY_ON
_REQUIRE_UV2
ANTI_FLICKER
_EMISSION
GEOM_TYPE_BRANCH
_SUNDISK_SIMPLE
_NORMALMAP
EFFECT_BUMP
_GLOSSYREFLECTIONS_OFF
_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
_SPECULARHIGHLIGHTS_OFF
GEOM_TYPE_MESH
_METALLICGLOSSMAP
GEOM_TYPE_LEAF
_SPECGLOSSMAP
_PARALLAXMAP
PIXELSNAP_ON
_FADING_ON
_MAPPING_6_FRAMES_LAYOUT
_SUNDISK_HIGH_QUALITY
GEOM_TYPE_BRANCH_DETAIL
_DETAIL_MULX2
```
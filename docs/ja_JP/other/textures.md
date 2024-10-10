# テクスチャのインポート設定について

## 概要

Unityではテクスチャのインポート時に`Wrap Mode`（テクスチャのタイリング方法）や`Filter Mode`（サンプリングする際の補間方法）などを設定できます。一部項目はAPIの制限で無視される場合があります。このページではそれについて触れています。

## 無視される場合がある項目

以下のインポート設定は`SamplerState`に関する設定で、シェーダーが持てる設定数に上限があるため無視される場合があります。

|名前|説明|
|-|-|
|Wrap Mode|テクスチャのタイリング方法の設定です。|
|Filter Mode|サンプリング時の補間方法の設定です。|

## 固有のSamplerStateを持つテクスチャ

以下のテクスチャは固有のサンプラーを持っているためテクスチャのインポート設定が利用されます。

|名前|SamplerState|
|-|-|
|`_MainTex`（メインカラー）|sampler_MainTex|
|`_Main2ndTex`（メインカラー2nd）|sampler_Main2ndTex|
|`_Main3rdTex`（メインカラー3rd）|sampler_Main3rdTex|
|`_EmissionMap`（発光テクスチャ）|sampler_EmissionMap|
|`_Emission2ndMap`（発光テクスチャ2nd）|sampler_Emission2ndMap|
|`_AudioLinkMask`（AudioLinkのマスク）|sampler_AudioLinkMask|
|`_OutlineTex`（輪郭線の色）|sampler_OutlineTex|

## SamplerStateが固定されたテクスチャ

以下テクスチャは`SamplerState`が固定されています。

|名前|SamplerState|
|-|-|
|`_MainGradationTex`（グラデーションマップ）|sampler_linear_clamp|
|`_EmissionGradTex`（発光のグラデーション）|sampler_linear_repeat|
|`_Emission2ndGradTex`（発光2ndのグラデーション）|sampler_linear_repeat|
|`_ShadowBlurMask`（影のぼかし）|sampler_linear_repeat|
|`_ShadowBorderMask`（影のAO）|sampler_linear_repeat|
|`_ShadowStrengthMask`（影の強度）|sampler_linear_repeat|
|`_ShadowColorTex` (影色1のLUT)|sampler_linear_clamp|
|`_Shadow2ndColorTex` (影色2のLUT)|sampler_linear_clamp|
|`_Shadow3rdColorTex` (影色3のLUT)|sampler_linear_clamp|
|`_Bump2ndMap`（ノーマルマップ2nd）|sampler_linear_repeat|
|`_MatCapTex`（マットキャップ）|sampler_linear_repeat|
|`_MatCap2ndTex`（マットキャップ2nd）|sampler_linear_repeat|
|`_GlitterShapeTex`（ラメの形状）|sampler_linear_clamp|
|`_ParallaxMap`（視差マップ）|sampler_linear_repeat|
|`_AudioLinkLocalMap`（AudioLinkのローカル）|sampler_linear_repeat|
|`_OutlineWidthMask`（輪郭線の幅）|sampler_linear_repeat|
|`_OutlineVectorTex`（輪郭線の法線）|sampler_linear_repeat|
|`_FurVectorTex`（ファーの方向）|sampler_linear_repeat|
|`_FurLengthMask`（ファーの長さ）|sampler_linear_repeat|

## その他のテクスチャ
`_MainTex`の設定が共有されます。
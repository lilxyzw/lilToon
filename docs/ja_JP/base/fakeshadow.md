# FakeShadow設定

## 概要
シェーダーバリエーション`_lil/[Optional] lilToonFakeShadow`の専用プロパティです。メッシュをライトの反対方向にずらして表示することで擬似的な影を生成することができます。

## パラメーター

|名前|説明|
|-|-|
|向き|ライトの向きに加えてずらす向きです。|
|Offset|ずらし量です。|

FakeShadowを使用する場合は以下のようにステンシル設定を変更します。髪以外でも影が重なって気になる場合はそのマテリアルも髪と同様に設定してください。Refの数値は仮で`51`としていますが、0以外の任意の値を使用できます。  

|名前|顔|髪|FakeShadow|
|-|-|-|-|
|Ref|51|0|51|
|ReadMask|255|255|255|
|WriteMask|255|255|255|
|Comp|Always|Always|Equal|
|Pass|Replace|Replace|Keep|
|Fail|Keep|Keep|Keep|
|ZFail|Keep|Keep|Keep|
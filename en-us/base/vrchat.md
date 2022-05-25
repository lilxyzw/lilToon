# VRChat

## 概要
`Custom Safety Fallback`でセーフティー発動時のフォールバック先のシェーダーをカスタマイズできます。セーフティーシステムの詳細については[こちら](https://docs.vrchat.com/docs/shader-fallback-system)をご確認ください。

<div class="window_info">&#x26a0; 本項目執筆時点では ToonTransparent は存在せず、代わりに Unlit/Transparent にフォールバックされるため注意が必要です。</div>

## パラメーター

|名前|説明|
|-|-|
|Shader Type|シェーダーの種類です。|
|Rendering Mode|透過の種類です。|
|Facing|表示する面です。片面描画と両面描画のどちらかを選択できます。|
|Result|上記の設定を元にマテリアルに設定されたタグです。これを元にフォールバック先のシェーダーが決定されます。|
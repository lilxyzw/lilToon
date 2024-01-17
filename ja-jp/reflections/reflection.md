# 光沢設定

## 概要
光沢（鏡面反射）に関する設定です。イラストで言うハイライト（つや）を表現したり、シーンに設定された環境光を反射するリアルな表現も行えます。Cubemapを指定することで反射光をカスタマイズできるため様々な使い方ができます。

## パラメーター

|名前|説明|
|-|-|
|滑らかさ|表面の滑らかさです。数値を下げるほど反射が柔らかくなります。テクスチャのRチャンネルが使用されます。|
|金属度|金属っぽさです。テクスチャのRチャンネルが使用されます。|
|反射率|環境光の反射率です。[数値のサンプル](https://forum.corona-renderer.com/index.php?topic=2359.0)|
|色|反射の色です。|
|光沢のタイプ|ライトを反射させる見た目です。|
|複数ライトから光沢を生成|オンにするとポイントライトやスポットライトからも光沢を生成するようになります。|
|環境光の反射|周りの色を反射させます。|
|合成モード|反射の合成方法です。通常、加算、スクリーン、乗算が選択できます。|
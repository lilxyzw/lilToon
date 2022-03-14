# 概要
カスタムシェーダー用のテンプレートです。基本的には以下の4つのファイルのみを編集するだけで完結するようになっています。

- Shaders/custom.hlsl
- Shaders/lilCustomShaderDatas.lilblock
- Shaders/lilCustomShaderProperties.lilblock
- Editor/CustomInspector.cs

バリエーション数が多いため、適宜不要なバリエーションを削除してください。

## custom.hlsl
改変用のマクロが含まれたものです。マクロは旧仕様から変更はありません。

## lilCustomShaderDatas.lilblock
1行目はシェーダー名、2行目はカスタムエディタ名になっています。

## lilCustomShaderProperties.lilblock
全バリエーションに挿入されるカスタムプロパティです。

## CustomInspector.cs
カスタムエディタ用のスクリプトです。
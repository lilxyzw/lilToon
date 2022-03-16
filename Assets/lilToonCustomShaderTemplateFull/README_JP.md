# 概要
カスタムシェーダー用のテンプレートです。基本的には以下の5つのファイルのみを編集するだけで完結するようになっています。

- Shaders/custom.hlsl
- Shaders/custom_insert.hlsl
- Shaders/lilCustomShaderDatas.lilblock
- Shaders/lilCustomShaderProperties.lilblock
- Editor/CustomInspector.cs

バリエーション数が多いため、適宜不要なバリエーションを削除してください。

## custom.hlsl
改変用のマクロが含まれたものです。マクロは旧仕様から変更はありません。

## custom_insert.hlsl
各パスでUnityのライブラリがincludeされた後に挿入されます。Unityの変数依存の関数などはこちらに記述します。

## lilCustomShaderDatas.lilblock
共通のシェーダー名やカスタムエディタ名、文字列置換などを記述できます。パスの挿入はテキストファイルを指定します。
|タグ|説明|
|-|-|
|ShaderName|共通のシェーダー名（`ShaderName "TemplateFull"`のような形で記述します。ここで指定した名前は`*LIL_SHADER_NAME*`から置き換えられます。）|
|EditorName|共通のエディタ名（`EditorName "lilToon.TemplateFullInspector"`のような形で記述します。ここで指定した名前は`*LIL_EDITOR_NAME*`から置き換えられます）|
|Replace|文字列置換（`Replace "From" "To"`のような形で記述します。）|
|InsertPassPre|前に挿入するPass（`InsertPassPre "PrePass.lilblock"`、もしくは`InsertPassPre "lite" "PrePass.lilblock"`のような形で記述します。後者はシェーダー名に文字列が含まれる場合にパスが挿入されます。`InsertPassPre "!lite" "PrePass.lilblock"`のように記述した場合は含まない場合に挿入されるようになります。）|
|InsertPassPost|後に挿入するPass|
|InsertUsePassPre|前に挿入するUsePass|
|InsertUsePassPost|後に挿入するUsePass|

InsertPassは後に記述したものが優先されます。以下の例では通常は`PrePass.lilblock`、liteでは`PrePassLite.lilblock`、Multiでは`PrePassMulti.lilblock`が挿入されるようになります。
```
InsertPassPre "PrePass.lilblock"
InsertPassPre "lite" "PrePassLite.lilblock"
InsertPassPre "Multi" "PrePassMulti.lilblock"
```

また、ファイル名の後ろに`BRP`、`URP`、`HDRP`を追加したファイルを配置しておくことで各RPごとに読み込むファイルを切り替えられます。  
例: `InsertPassPre "PrePass.lilblock"`と記述し、同一階層に`PrePassBRP.lilblock`、`PrePassURP.lilblock`、`PrePassHDRP.lilblock`を配置する

## lilCustomShaderProperties.lilblock
全バリエーションに挿入されるカスタムプロパティです。

## CustomInspector.cs
カスタムエディタ用のスクリプトです。
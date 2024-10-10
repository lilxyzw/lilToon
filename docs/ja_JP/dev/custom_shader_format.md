# カスタムシェーダーの仕様

## 概要
拡張子`.lilcontainer`のテキストファイルです。基本的にShaderLab同様に記述できますが、いくつか独自の仕様が含まれています。

```ShaderLab
Shader "*LIL_SHADER_NAME*"
{
    Properties
    {
        // lilToon標準プロパティが挿入される
        lilProperties "Default"
        lilProperties "DefaultOpaque"

        // カスタムプロパティは直接書くかテキストファイルから読み込む
        lilProperties "Properties.lilblock"
    }

    HLSLINCLUDE
    ENDHLSL

    // シェーダー設定挿入のスキップ
    //lilSkipSettings

    // SubShader内のTags
    lilSubShaderTags {"RenderType" = "Opaque" "Queue" = "Geometry"}

    // 各Pass内でUnityのライブラリをincludeした後に挿入されるテキスト
    //lilSubShaderInsert "SubShaderInsert.lilblock"

    // SubShaderをテキストファイルかキーワードから呼び出す
    lilSubShaderBRP "DefaultMulti"
    lilSubShaderURP "DefaultMulti"
    lilSubShaderHDRP "DefaultMulti"
    //lilSubShaderBRP "BRP.lilblock"
    //lilSubShaderURP "URP.lilblock"
    //lilSubShaderHDRP "HDRP.lilblock"

    // カスタムエディタの指定
    CustomEditor "*LIL_EDITOR_NAME*"
}
```

## 独自仕様の説明

|名前|説明|
|-|-|
|`lilProperties ""`|プロパティの挿入|
|`lilSkipSettings`|シェーダー設定の挿入のスキップ|
|`lilSubShaderTags {}`|SubShader内のTags|
|`lilSubShaderInsert ""`|Unityのライブラリのinclude後に処理を挿入|
|`lilPassShaderName ""`|UsePassの読み込み元のシェーダー名|
|`lilSubShaderBRP ""`|Built-in RP用のSubShader|
|`lilSubShaderURP ""`|URP用のSubShader|
|`lilSubShaderHDRP ""`|HDRP用のSubShader|
|`lilCustomShaderDatas.lilblock`|シェーダーと同一階層に置くことでその階層のシェーダーのデータをまとめて指定できます。|
|`*LIL_SHADER_NAME*`|上記ファイルで指定したシェーダー名に置き換えられる文字列|
|`*LIL_EDITOR_NAME*`|上記ファイルで指定したエディタ名に置き換えられる文字列|

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

## pragma
以下の独自のpragmaを利用できます。これらはRender Pipelineに関わらず使用できます。

|名前|説明|
|-|-|
|#pragma lil_multi_compile_forward|Forward用のmulti_compile|
|#pragma lil_multi_compile_forwardadd|ForwardAdd用のmulti_compile|
|#pragma lil_multi_compile_shadowcaster|ShadowCaster用のmulti_compile|
|#pragma lil_multi_compile_depthonly|DepthOnly用のmulti_compile|
|#pragma lil_multi_compile_depthnormals|DepthNormals用のmulti_compile|
|#pragma lil_multi_compile_motionvectors|MotionVectors用のmulti_compile|
|#pragma lil_multi_compile_sceneselection|SceneSelection用のmulti_compile|
|#pragma lil_multi_compile_meta|META用のmulti_compile|
|#pragma lil_skip_variants_shadows|影の受け取りのバリアントのスキップ|
|#pragma lil_skip_variants_lightmaps|ライトマップのバリアントのスキップ|
|#pragma lil_skip_variants_decals|デカールのバリアントのスキップ|
|#pragma lil_skip_variants_addlight|頂点ライトのバリアントのスキップ|
|#pragma lil_skip_variants_addlightshadows|ポイント/スポットライトの影の受け取りのバリアントのスキップ|
|#pragma lil_skip_variants_probevolumes|Probe Volumeのバリアントのスキップ|
|#pragma lil_skip_variants_ao|アンビエントオクルージョンのバリアントのスキップ|
|#pragma lil_skip_variants_lightlists|LightListのバリアントのスキップ|
|#pragma lil_skip_variants_reflections|Environment Reflectionのバリアントのスキップ|

## lilSubShaderのキーワード
ファイルパスを指定する代わりに以下のキーワードを記述することでデフォルトのSubShaderを挿入できます。

```
Default
DefaultFakeShadow
DefaultFur
DefaultGem
DefaultRefraction
DefaultRefractionBlur
DefaultTessellation

DefaultLite

DefaultMulti
DefaultMultiOutline
DefaultMultiFur
DefaultMultiRefraction
DefaultMultiGem

DefaultUsePass
DefaultUsePassOutline
DefaultUsePassFurOnly
DefaultUsePassOutlineOnly
DefaultUsePassOverlay
```

## lilPropertiesのキーワード
ファイルパスを指定する代わりに以下のキーワードを記述することでデフォルトのプロパティを挿入できます。

```
Default

DefaultLite

DefaultOpaque
DefaultCutout
DefaultTransparent

DefaultFurCutout
DefaultFurTransparent
DefaultRefraction
DefaultGem
DefaultFakeShadow
```
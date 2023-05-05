# カスタムシェーダーの作り方

## 基本的な流れ
[カスタムシェーダー用テンプレート（Template.zip）](https://github.com/lilxyzw/lilToon/raw/Document/files/Template.zip)を元に作成していきます。基本的には以下の5つのファイルのみを編集するだけで完結するようになっています。

- Shaders/custom.hlsl
- Shaders/custom_insert.hlsl
- Shaders/lilCustomShaderDatas.lilblock
- Shaders/lilCustomShaderProperties.lilblock
- Editor/CustomInspector.cs

バリエーション数が多いため、適宜不要なバリエーションを削除してください。またルートのフォルダ名（Template）はわかりやすいように任意の名前に変更してください。

## シェーダー名の設定
シェーダー名を変更するには以下の二箇所を編集します。

|ファイル|該当箇所|
|-|-|
|Shaders/lilCustomShaderDatas.lilblock|`ShaderName ""`タグ内|
|Editor/CustomInspector.cs|`shaderName`変数|

## マテリアル変数の追加
まず、`lilCustomShaderProperties.lilblock`内に任意のプロパティを追加してください。ShaderLabsの`Properties`ブロックと同様の書式で記述します。今回は例として以下のものを追加します。
```
        //----------------------------------------------------------------------------------------------------------------------
        // Custom
        [lilVec3]       _CustomVertexWaveScale      ("Vertex Wave Scale", Vector) = (10.0,10.0,10.0,0.0)
        [lilVec3]       _CustomVertexWaveStrength   ("Vertex Wave Strength", Vector) = (0.0,0.1,0.0,0.0)
                        _CustomVertexWaveSpeed      ("Vertex Wave Speed", float) = 10.0
        [NoScaleOffset] _CustomVertexWaveMask       ("Vertex Wave Mask", 2D) = "white" {}
```

続いて`custom.hlsl`を編集します。変数は`#define LIL_CUSTOM_PROPERTIES`、Texture2DやSamplerStateは`#define LIL_CUSTOM_TEXTURES`というマクロで挿入できます。

```hlsl
#define LIL_CUSTOM_PROPERTIES \
    float4  _CustomVertexWaveScale; \
    float4  _CustomVertexWaveStrength; \
    float   _CustomVertexWaveSpeed;

#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_CustomVertexWaveMask);
```

## 関数やincludeの追加
通常は`custom.hlsl`内に記述できますが、Unityの関数や変数に依存する場合は`custom_insert.hlsl`内に関数やincludeを記述してください。特定パスでエラーが出る場合はそのパスで`#undef`し、必要な場合は宣言し直すと良いでしょう。パスごとに以下のマクロ宣言されるため、`#if defined(LIL_PASS_FORWARD)`のような形で条件分岐できます。

|名前|パス名|
|-|-|
|LIL_PASS_FORWARD|ForwardBase、UniversalForward、ForwardOnly、SRPDefaultUnlit|
|LIL_PASS_FORWARDADD|ForwardAdd（BRPのみ）|
|LIL_PASS_SHADOWCASTER|ShadowCaster|
|LIL_PASS_DEPTHONLY|DepthOnly（URP、HDRPのみ）|
|LIL_PASS_DEPTHNORMALS|DepthNormals（URPのみ）|
|LIL_PASS_MOTIONVECTORS|MotionVectors（HDRPのみ）|
|LIL_PASS_META|META|

## 頂点シェーダーの入力の追加（appdata構造体）
以下のキーワードを`#define`することで対応した入力が追加されます。

|キーワード|追加される変数名|セマンティクス|
|-|-|-|
|LIL_REQUIRE_APP_POSITION|positionOS|POSITION|
|LIL_REQUIRE_APP_TEXCOORD0|uv|TEXCOORD0|
|LIL_REQUIRE_APP_TEXCOORD1|uv1|TEXCOORD1|
|LIL_REQUIRE_APP_TEXCOORD2|uv2|TEXCOORD2|
|LIL_REQUIRE_APP_TEXCOORD3|uv3|TEXCOORD3|
|LIL_REQUIRE_APP_TEXCOORD4|uv4|TEXCOORD4|
|LIL_REQUIRE_APP_TEXCOORD5|uv5|TEXCOORD5|
|LIL_REQUIRE_APP_TEXCOORD6|uv6|TEXCOORD6|
|LIL_REQUIRE_APP_TEXCOORD7|uv7|TEXCOORD7|
|LIL_REQUIRE_APP_COLOR|color|COLOR|
|LIL_REQUIRE_APP_NORMAL|normalOS|NORMAL|
|LIL_REQUIRE_APP_TANGENT|tangentOS|TANGENT|
|LIL_REQUIRE_APP_VERTEXID|vertexID|SV_VertexID|

## 頂点シェーダーの出力の追加（v2f構造体）
構造体のメンバーは`#define LIL_CUSTOM_V2F_MEMBER`から追加できます。もともと構造体に含まれるものは`#define LIL_V2F_FORCE_(キーワード)`で強制的にメンバーにさせることができます。

## 頂点シェーダーに処理を挿入する
以下のようなマクロで処理を挿入できます。
```hlsl
#define LIL_CUSTOM_VERTEX_OS \
    float3 customWaveStrength = LIL_SAMPLE_2D_LOD(_CustomVertexWaveMask, sampler_linear_repeat, input.uv0, 0).r * _CustomVertexWaveStrength.xyz; \
    positionOS.xyz += sin(LIL_TIME * _CustomVertexWaveSpeed + dot(positionOS.xyz, _CustomVertexWaveScale.xyz)) * customWaveStrength;
```

### LIL_CUSTOM_VERTEX_OS（オブジェクト空間での処理）

|利用できる変数|説明|
|-|-|
|inout appdata input|appdata構造体|
|inout float2 uvMain|計算済みUV座標|
|inout float4 positionOS|頂点のオブジェクト座標|

### LIL_CUSTOM_VERTEX_WS（ワールド空間での処理）

|利用できる変数|説明|
|-|-|
|inout appdata input|appdata構造体|
|inout float2 uvMain|計算済みUV座標|
|inout lilVertexPositionInputs vertexInput|頂点座標の構造体（positionWS、positionVS、positionCS、positionSS）|
|inout lilVertexNormalInputs vertexNormalInput|ワールド空間のベクトルの構造体（tangentWS、bitangentWS、normalWS）|

## ピクセルシェーダーに処理を挿入する
`BEFORE_(キーワード)`や`OVERRIDE_(キーワード)`で処理を挟んだり上書きしたりできます。
```hlsl
#define BEFORE_UNPACK_V2F \
    fd.uv0 = input.uv0;
```
現在対応しているキーワードは以下の通りです。

|名前|説明|
|-|-|
|UNPACK_V2F|v2f構造体の展開|
|ANIMATE_MAIN_UV|メインUVのアニメーション処理|
|ANIMATE_OUTLINE_UV|輪郭線UVのアニメーション処理|
|PARALLAX|視差マップの処理|
|MAIN|メインカラーの処理|
|OUTLINE_COLOR|輪郭線の色の処理|
|FUR|(ファーシェーダー用) ファーの処理|
|ALPHAMASK|アルファマスクの処理|
|DISSOLVE|Dissolveの処理|
|NORMAL_1ST|ノーマルマップの処理|
|NORMAL_2ND|ノーマルマップ2ndの処理|
|ANISOTROPY|異方性反射の処理|
|AUDIOLINK|AudioLinkの処理|
|MAIN2ND|メインカラー2ndの処理|
|MAIN3RD|メインカラー3rdの処理|
|SHADOW|影の処理|
|BACKLIGHT|逆光ライトの処理|
|REFRACTION|屈折の処理|
|REFLECTION|反射の処理|
|MATCAP|マットキャップの処理|
|MATCAP_2ND|マットキャップ2ndの処理|
|RIMLIGHT|リムライトの処理|
|GLITTER|ラメの処理|
|EMISSION_1ST|発光の処理|
|EMISSION_2ND|発光2ndの処理|
|DISSOLVE_ADD|Dissolveの境界の発光処理|
|BLEND_EMISSION|発光の合成|
|DISTANCE_FADE|距離フェードの処理|
|FOG|フォグの処理|
|OUTPUT|最終書き出し|

## 拡張Inspector
編集手順は以下の通りです。
1. `Editor/CustomInspector.cs`でクラス名を変更、`Shaders/lilCustomShaderDatas.lilblock`内の`EditorName`タグを設定したクラス名に置き換え
2. `MaterialProperty`を宣言
3. `LoadCustomProperties()`をオーバーライドして、`FindProperty`でプロパティを取得
4. `DrawCustomProperties()`をオーバーライドしてGUIを実装

lilToonのGUIStyleとして以下のものが利用できます。

|名前|説明|
|-|-|
|boxOuter|プロパティボックスの外側|
|boxInnerHalf|プロパティボックスの内側（ラベル描画用に上部が角ばっています）|
|boxInner|プロパティボックスの内側（ラベルなしの場合に綺麗になるように上部が角丸になっています）|
|customBox|Unityデフォルトのboxに近いですが視認性を上げるために縁取りがされています|
|customToggleFont|プロパティボックスのラベルに使われる太字のフォント|

またlilToon独自のEditor拡張として以下のような関数が使えます。

|名前|説明|
|-|-|
|bool Foldout(string title, string help, bool display)|Foldoutの描画|
|void DrawLine()|区切り線の描画|
|void DrawWebButton(string text, string URL)|ウェブボタンの描画|
|void LoadCustomLanguage(string langFileGUID)|カスタム言語ファイルの追加読み込み|

## 作例
- [ジオメトリシェーダー利用例（lilToonGeometryFX_1.0.1.unitypackage）](https://github.com/lilxyzw/lilToon/raw/Document/files/lilToonGeometryFX_1.0.1.unitypackage)
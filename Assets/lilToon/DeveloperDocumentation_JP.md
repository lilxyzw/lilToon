# 開発者向けドキュメント

# ファイル構成
- lilToon
    - Editor : エディタ関係のアセット
        - gui_xx : GUI用のアセット
        - lang.txt : 言語ファイル (tsv形式)
        - lilInspector.cs : ShaderGUIの拡張
        - lilStartup.cs : スタートアップ (シェーダー設定生成、バージョンチェック等)
        - lilToonAssetPostprocessor.cs : アセットに使われているシェーダー設定を分析して自動設定するAssetPostprocessor
        - lilToonEditorUtils.cs : MenuItemの追加
        - lilToonPreset.cs : プリセットのScriptableObject
        - lilToonPropertyDrawer.cs : MaterialPropertyDrawer
        - lilToonSetting.cs : シェーダー設定のScriptableObject
    - Presets : プリセット
    - Shader : シェーダー
        - Includes : hlslファイル (シェーダー本体)
            - lil_common.hlsl : 共通ファイル
            - lil_common_appdata.hlsl : appdata構造体の宣言 (頂点シェーダーのinput)
            - lil_common_frag.hlsl : ピクセルシェーダーの共通ファイル
            - lil_common_frag_alpha.hlsl : 透過のみ処理するピクセルシェーダーの共通ファイル
            - lil_common_functions.hlsl : 関数
            - lil_common_input.hlsl : マテリアル変数の宣言
            - lil_common_macro.hlsl : Unityのマクロを変換、パイプラインの差の吸収
            - lil_common_vert.hlsl : 頂点シェーダーの共通ファイル
            - lil_common_vert_fur.hlsl : ファーの頂点シェーダーの共通ファイル
            - lil_hdrp.hlsl : HDRP対応用
            - lil_pass_depthnormals.hlsl : DepthNormalsパス (URP用)
            - lil_pass_depthonly.hlsl : DepthOnlyパス (SRP用)
            - lil_pass_forward.hlsl : Forwardパス
            - lil_pass_forward_fur.hlsl : ファーシェーダーのForwardパス
            - lil_pass_forward_lite.hlsl : lilToonLiteのForwardパス
            - lil_pass_forward_normal.hlsl : 通常シェーダーのForwardパス
            - lil_pass_forward_gem.hlsl : 宝石シェーダーのForwardパス
            - lil_pass_forward_refblur : 屈折シェーダーのぼかしパス
            - lil_pass_forward_fakeshadow.hlsl : FakeShadow用のパス
            - lil_pass_meta.hlsl : ライトベイク用のパス
            - lil_pass_motionvectors.hlsl : MotionVectorsパス (HDRP用)
            - lil_pass_shadowcaster.hlsl : ShadowCasterのパス
            - lil_pass_universal2d.hlsl : Universal2Dパス
            - lil_pipeline.hlsl : パイプラインごとの分岐
            - lil_replace_keywords.hlsl : シェーダーキーワードをシェーダー設定として置き換え
            - lil_tessellation.hlsl : テッセレーション用のシェーダー
            - lil_vert_audiolink.hlsl : AudioLink用の頂点シェーダーの処理
            - lil_vert_encryption.hlsl : AvaterEncryption用の頂点シェーダーの処理
            - lil_vert_outline.hlsl : 輪郭線用の頂点シェーダーの処理
        - lts_xx.shader : 通常のシェーダー
        - ltsl.shader : lilToonLite
        - ltsmulti.shader : シェーダー設定の代わりにシェーダーキーワードを使ったもの
        - ltspass_baker.shader : テクスチャ焼き込み用のシェーダー
        - ltspass_xx.shader : 各バリエーションからUsePassされるシェーダー本体
            - _o : 輪郭線
            - _cutout : カットアウト
            - _trans : 透過
            - _fur : ファー
            - _ref : 屈折
            - _gem : 宝石
            - _tess : テッセレーション
            - _overlay : オーバーレイ (Forwardパス以外を省略した透過シェーダー)
            - _fakeshadow : メッシュをずらして擬似的に影を生成するシェーダー
            - _one : 1パス（ForwardAddを頂点シェーダーで計算)
            - _two : 2パス（ForwardAddを頂点シェーダーで計算かつ追加パスで背面を描画)
    - Texture
        - lil_emission_rainbow.png : 虹色のグラデーションテクスチャ
        - lil_noise_1d.png : 1Dノイズ
        - lil_noise_fur.png : ファーのノイズ
    - CHANGELOG.md : 変更履歴
    - DeveloperDocumentation.md : 開発者ドキュメント
    - LICENSE : ライセンス
    - MANUAL.md : マニュアル
    - package.json : UPM用のデータ
    - README.md : 説明書
    - Third Party Notices.md : 参考文献やサードパーティのライセンスとURL
- lilToonSetting : プロジェクトごとに自動生成されるシェーダー設定
    - lil_setting.hlsl : シェーダー設定から自動生成されるシェーダー用マクロ
    - ShaderSetting.asset : シェーダー設定保存用アセット

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
非常に多くのバリエーションがありますが、シェーダー冒頭の`HLSLINCLUDE`でシェーダーバリエーションごとのマクロを宣言し、各パスで`#include "Includes/lil_pass_xx.hlsl"`を呼び出してマクロに応じた分岐をさせることでなるべくコードを共通化しています。  
また、Built-in RP / LWRP / URP / HDRPのシェーダーが共通になっていますが、スクリプトから各シェーダーと`lil_pipeline.hlsl`を書き換えることでパイプラインの切り替えをできるようになっています。

## includeについて
各パスで`#include "Includes/lil_pass_xx.hlsl"`を呼び出していますが、このパスごとのhlslファイルは基本的に以下の構造で統一されています。
- lil_pass_xx.hlsl
    - lil_pipeline.hlsl
        - Unityのライブラリ
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
ETC1_EXTERNAL_ALPHA UNITY_UI_ALPHACLIP UNITY_UI_CLIP_RECT EFFECT_HUE_VARIATION _COLORADDSUBDIFF_ON _COLORCOLOR_ON _SUNDISK_NONE GEOM_TYPE_FROND _COLOROVERLAY_ON _REQUIRE_UV2 ANTI_FLICKER _EMISSION GEOM_TYPE_BRANCH _SUNDISK_SIMPLE _NORMALMAP EFFECT_BUMP _GLOSSYREFLECTIONS_OFF _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A _SPECULARHIGHLIGHTS_OFF GEOM_TYPE_MESH _METALLICGLOSSMAP GEOM_TYPE_LEAF _SPECGLOSSMAP _PARALLAXMAP PIXELSNAP_ON BILLBOARD_FACE_CAMERA_POS _FADING_ON _MAPPING_6_FRAMES_LAYOUT _SUNDISK_HIGH_QUALITY GEOM_TYPE_BRANCH_DETAIL _DETAIL_MULX2
```

# カスタムシェーダーの作り方

## 基本的な流れ
基本的にメインのシェーダーの`HLSLINCLUDE`内にマクロを記述する形になります。  
特定パスでエラーが出る場合はそのパスで`#undef`し、必要な場合は宣言し直すと良いでしょう。

## シェーダーファイルの作成
`ltspass_○○.shader`等にパスが記述されており各シェーダーバリエーションから`UsePass`を用いて参照しています。  
新規にShaderを作成しコードを`ScriptTemplates/99-lilToon__Custom Pass Shader-custom_ltspass_opaque.shader.txt`からコピーします。  
ここでは例として`custom_ltspass_opaque.shader`を作成します。  
また、ファイル冒頭の`Shader "Hidden/#NAME#"`を`Shader "Hidden/custom_ltspass_opaque"`に書き換えます。

## シェーダーバリエーションの作成
`lts.shader`を複製し`custom_lts.shader`に名前を変更します。  
ファイル冒頭の`Shader "lilToon"`を`Shader "lilToonCustomExample/Opaque"`に書き換えます。  
シェーダー内の`UsePass "Hidden/ltspass_opaque/xx"`の部分を先程作成した`UsePass "Hidden/custom_ltspass_opaque/xx"`に置き換えます。

## マテリアル変数の追加
まず`custom_lts.shader`内の`Properties`ブロックに任意のプロパティを追加しましょう。  
今回は以下のものを追加します。
```
        //----------------------------------------------------------------------------------------------------------------------
        // Custom
        [lilVec3]       _CustomVertexWaveScale      ("Vertex Wave Scale", Vector) = (10.0,10.0,10.0,0.0)
        [lilVec3]       _CustomVertexWaveStrength   ("Vertex Wave Strength", Vector) = (0.0,0.1,0.0,0.0)
                        _CustomVertexWaveSpeed      ("Vertex Wave Speed", float) = 10.0
        [NoScaleOffset] _CustomVertexWaveMask       ("Vertex Wave Mask", 2D) = "white" {}
```
これで`custom_lts.shader`の編集は終わりです。  
エディタをカスタマイズする場合は`CustomEditor "lilToon.lilToonInspector"`を任意のものに変更します。

続いて`custom_ltspass_opaque.shader`を編集します。  
変数は`#define LIL_CUSTOM_PROPERTIES`、Texture2DやSamplerStateは`#define LIL_CUSTOM_TEXTURES`というマクロで挿入できます。  
以下のようにマクロを記述しましょう。

```HLSL
#define LIL_CUSTOM_PROPERTIES \
    float4  _CustomVertexWaveScale; \
    float4  _CustomVertexWaveStrength; \
    float   _CustomVertexWaveSpeed;

#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_CustomVertexWaveMask);
```

## 関数やincludeの追加
一部ライブラリはUnityの関数や変数に依存します。  
その場合は各パスに存在する`#include "Includes/lil_pass_xx.hlsl"`の直前に関数やincludeを挿入してください。

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
構造体のメンバーは`#define LIL_CUSTOM_V2F_MEMBER`から追加できます。  
もともと構造体に含まれるものは`#define LIL_V2F_FORCE_(キーワード)`で強制的にメンバーにさせることができます。

## 頂点シェーダーに処理を挿入する
以下のマクロで処理を挿入できます。

|名前|説明|
|-|-|
|LIL_CUSTOM_VERTEX_OS|オブジェクト空間での処理|
|LIL_CUSTOM_VERTEX_WS|ワールド空間での処理|

今回の例では以下のように波のアニメーションを追加します。
```HLSL
#define LIL_CUSTOM_VERTEX_OS \
    float3 customWaveStrength = LIL_SAMPLE_2D_LOD(_CustomVertexWaveMask, sampler_linear_repeat, input.uv0, 0).r * _CustomVertexWaveStrength.xyz; \
    positionOS.xyz += sin(LIL_TIME * _CustomVertexWaveSpeed + dot(positionOS.xyz, _CustomVertexWaveScale.xyz)) * customWaveStrength;
```

今回は使いませんでしたがワールド空間の座標や法線は`vertexInput`や`vertexNormalInput`に格納されています。
```HLSL
#define LIL_CUSTOM_VERTEX_WS \
    vertexInput.positionWS = CustomSomething(vertexInput.positionWS);
```

## ピクセルシェーダーに処理を挿入する
`BEFORE_(キーワード)`や`OVERRIDE_(キーワード)`で処理を挟んだり上書きしたりできます。  
現在対応しているキーワードは以下の通りです。（今後要望に合わせて増やすかもしれません）

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

例:
```HLSL
#define BEFORE_UNPACK_V2F \
    fd.uv0 = input.uv0;
```

## 拡張Inspector
拡張Inspectorのテンプレートは`ScriptTemplates/99-lilToon__Custom Inspector-NewCustomInspector.cs.txt`にあります。  
編集手順は以下の通りです。
1. `MaterialProperty`を宣言
2. `LoadCustomProperties()`をオーバーライドして、その中で`isCustomShader`を`true`にしつつ`FindProperty`でプロパティを取得
3. `DrawCustomProperties()`をオーバーライドしてGUIを実装

lilToonのGUIStyleとして以下のものが渡されてくるのでご活用ください。

|名前|説明|
|-|-|
|boxOuter|プロパティボックスの外側|
|boxInnerHalf|プロパティボックスの内側（ラベル描画用に上部が角ばっています）|
|boxInner|プロパティボックスの内側（ラベルなしの場合に綺麗になるように上部が角丸になっています）|
|customBox|Unityデフォルトのboxに近いですが視認性を上げるために縁取りがされています|
|customToggleFont|プロパティボックスのラベルに使われる太字のフォント|
|offsetButton|プロパティボックスに合うようにインデントが加えられたボタン|

またlilToon独自のGUI拡張として以下のような関数が使えます。
|名前|説明|
|-|-|
|bool Foldout(string title, string help, bool display)|Foldoutの描画|
|void DrawLine()|区切り線の描画|
|void DrawWebButton(string text, string URL)|ウェブボタンの描画|
|void LoadCustomLanguage(string langFileGUID)|カスタム言語ファイルの追加読み込み|

今回はCustomInspectorExample.csを作成し以下のように拡張しました。
```C#
// This script should be placed in the lilToon/Editor folder

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class CustomInspectorExample : lilToonInspector
    {
        // Custom properties
        MaterialProperty customVertexWaveScale;
        MaterialProperty customVertexWaveStrength;
        MaterialProperty customVertexWaveSpeed;
        MaterialProperty customVertexWaveMask;

        private static bool isShowCustomProperties;

        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;
            //LoadCustomLanguage("");
            customVertexWaveScale = FindProperty("_CustomVertexWaveScale", props);
            customVertexWaveStrength = FindProperty("_CustomVertexWaveStrength", props);
            customVertexWaveSpeed = FindProperty("_CustomVertexWaveSpeed", props);
            customVertexWaveMask = FindProperty("_CustomVertexWaveMask", props);
        }

        protected override void DrawCustomProperties(
            MaterialEditor materialEditor,
            Material material,
            GUIStyle boxOuter,          // outer box
            GUIStyle boxInnerHalf,      // inner box
            GUIStyle boxInner,          // inner box without label
            GUIStyle customBox,         // box (similar to unity default box)
            GUIStyle customToggleFont,  // bold font
            GUIStyle offsetButton)      // button with indent
        {
            isShowCustomProperties = Foldout("Vertex Wave & Emission UV", "Vertex Wave & Emission UV", isShowCustomProperties);
            if(isShowCustomProperties)
            {
                // Vertex Wave
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(GetLoc("Vertex Wave"), customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);

                materialEditor.ShaderProperty(customVertexWaveScale, "Scale");
                materialEditor.ShaderProperty(customVertexWaveStrength, "Strength");
                materialEditor.ShaderProperty(customVertexWaveSpeed, "Speed");
                materialEditor.TexturePropertySingleLine(new GUIContent("Mask", "Strength (R)"), customVertexWaveMask);

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif
```

# マクロや関数
レンダリングパイプラインやAPIの差を吸収するためのマクロや関数が用意されています。  
より詳しくはHLSLファイルをご確認ください。

## テクスチャ
|名前|説明|
|-|-|
|TEXTURE2D(_Tex)|Texture2D _Tex|
|SAMPLER(sampler_Tex)|SamplerState sampler_Tex|
|LIL_SAMPLE_2D(Texture2D _Tex, SamplerState sampler_Tex, float2 uv)|_Tex.Sample(sampler_Tex, uv)|

## トランスフォーム
|名前|説明|
|-|-|
|LIL_MATRIX_M|ワールド行列|
|LIL_MATRIX_I_M|ワールドの転置行列|
|LIL_MATRIX_V|ビュー行列|
|LIL_MATRIX_VP|ビュー&プロジェクト行列|
|LIL_GET_VIEWDIR_WS(float3 positionWS)|normalizeされていないView Directionの取得|

## 座標のトランスフォーム (LIL_VERTEX_POSITION_INPUTS)
|名前|説明|
|-|-|
|LIL_VERTEX_POSITION_INPUTS(float4 positionOS, out vertexInput)|座標のトランスフォーム|
|LIL_RE_VERTEX_POSITION_INPUTS(vertexInput)|ワールド座標を変更した場合にトランスフォームを再計算|
|vertexInput.positionWS|ワールド座標|
|vertexInput.positionVS|ビュー座標|
|vertexInput.positionCS|クリップ座標 (SV_POSITION)|
|vertexInput.positionSS|画面座標|

## ベクターのトランスフォーム (LIL_VERTEX_NORMAL_TANGENT_INPUTS)
|名前|説明|
|-|-|
|LIL_VERTEX_NORMAL_TANGENT_INPUTS(float3 normalOS, float4 tangentOS, out vertexNormalInput)|ベクターのトランスフォーム|
|vertexNormalInput.tangentWS|ワールド空間のtangent|
|vertexNormalInput.bitangentWS|ワールド空間のbitangent|
|vertexNormalInput.normalWS|ワールド空間のnormal|

## ユーティリティ
|名前|説明|
|-|-|
|float2 lilCalcUV(float2 uv, float4 tex_ST, float4 tex_ScrollRotate)|UVの計算|
|float3 lilBlendColor(float3 dstCol, float3 srcCol, float srcA, uint blendMode)|ブレンドモードによる色の合成 (0: 通常 / 1: 加算 / 2: スクリーン / 3: 乗算)|
|float3 UnpackNormalScale(float4 normalTex, float scale)|ノーマルマップの展開|
|float lilTooning()|入力値をトゥーン化|

# ピクセルシェーダー
lilFragDataという共通の構造体を用いて変数を管理しています。  
詳しくはlil_common.hlslを確認してください。
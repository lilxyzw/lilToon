# その他

## 最適化

|名前|説明|
|-|-|
|全て焼き込み|メインカラーと色調補正、レイヤー2&3を統合して1枚のテクスチャにします。負荷を軽減しつつ他シェーダーに変更した際の見た目の変化を抑えることができます。|
|色調補正を焼き込み|色調補正したテクスチャを書き出します。他シェーダーに変更した際の見た目の変化を抑えることができます。|
|レイヤー2を焼き込み|メインカラーとレイヤー2を1枚のテクスチャに統合します。|
|レイヤー3を焼き込み|メインカラーとレイヤー3を1枚のテクスチャに統合します。|
|未使用のテクスチャを外す|マテリアルから使われていないテクスチャを外します。|
|lilToonLiteに変換|マテリアルを軽量版のlilToonLite用に変換します。|
|MToon(VRM)に変換|マテリアルをVRMで使われるMToon用に変換します。|

## シェーダー設定
全マテリアル共通の設定です。ここでオフにした機能はシェーダーから除去されます。不要な機能をオフにすることでアバターの容量を削減しつつ、負荷も抑えることができます。

## メニューの追加項目
上部メニューバーおよび右クリックメニューにいくつかツールを追加しています。

|名前|説明|
|-|-|
|[lilToon] Fix lighting|複数メッシュを持つオブジェクト向け。MeshRendererの設定の統一、マテリアルの頂点ライティングの無効化を行うことでメッシュごとの明るさの違いを緩和します。|
|Refresh shaders|レンダーパイプラインとシェーダー設定の再適用を行いエラーの自動修復を試みます。|
|Auto shader setting|プロジェクト内の全マテリアル・アニメーションをスキャンし自動でシェーダー設定を最適化します。|
|Remove unused properties|不要なプロパティを削除しビルドサイズを削減しつつ、シェーダー設定を追加でオンにしても見た目に影響が出ないようにマテリアルを最適化します。|
|Setup from FBX|FBXファイルから自動でマテリアルの生成、プリセットの適用、輪郭線マスク、影マスクの適用を行います。透過モードはマテリアルかテクスチャの名前に`cutout`が含まれている場合はカットアウト、`alpha`または`fade`または`transparent`が含まれている場合は半透明に変更されます。テクスチャ検索の命名規則は下記のリストをご覧ください。|
|Convert normal map (DirectX <-> OpenGL)|ノーマルマップをDirectX仕様とOpenGL仕様で相互変換します。|
|Convert Gif to Atlas|Gifからアトラステクスチャを生成します。処理内容はマテリアル設定の`Convert Gif`と同等です。|
|Dot texture reduction|ぼかしなしでドット絵の縮小を行います。|

<details><summary>テクスチャ命名規則</summary>

大文字小文字の違いは無視されます。また、マテリアルにテクスチャが割り当てられている場合は元のテクスチャが優先されます。

|プロパティ名|命名規則|
|-|-|
|メインテクスチャ|マテリアル名を含み、メインテクスチャではなさそうな名前ではないもの。<br/>(例: マテリアル名=faceの場合)<br/>- OK: face.png / texture_face.png<br/>- NG: face_outline_mask.png / face_smoothness.png|
|輪郭線マスク|マテリアル名と`outline`を含むもの。<br/>(例: マテリアル名=faceの場合)<br/>- OK: face_outline.png / face_outline_mask.png<br/>- NG: face_mask.png / face_ol_mask.png|
|影マスク|マテリアル名と`shadow`/`shade`どちらかを含み、`mask`/`strength`どちらかを含むもの。<br/>(例: マテリアル名=faceの場合)<br/>- OK: face_shadow_mask.png / face_shadow_strength_mask.png<br/>- NG: face_shadow_color.png / face_shadow.png|

メインテクスチャとして認識されないワード  
`mask`, `shadow`, `shade`, `outline`, `normal`, `bumpmap`, `matcap`, `rimlight`, `emittion`, `reflection`, `specular`, `roughness`, `smoothness`, `metallic`, `metalness`, `opacity`, `parallax`, `displacement`, `height`, `ambient`, `occlusion`

</details>
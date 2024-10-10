# メニューの追加項目

## 概要
上部メニューバーおよび右クリックメニューにいくつかツールを追加しています。

## 追加項目一覧

|名前|説明|
|-|-|
|GameObject/lilToon/[GameObject] Fix lighting|複数メッシュを持つオブジェクト向け。MeshRendererの設定の統一、マテリアルの頂点ライティングの無効化を行うことでメッシュごとの明るさの違いを緩和します。|
|Assets/lilToon/[Shader] Refresh shaders|レンダーパイプラインとシェーダー設定の再適用を行いエラーの自動修復を試みます。|
|Assets/lilToon/[Material] Remove unused properties|不要なプロパティを削除しビルドサイズを削減します。|
|Assets/lilToon/[Material] Run migration|全マテリアルのバージョン移行を手動で実行します。|
|Assets/lilToon/[Texture] Convert normal map (DirectX <-> OpenGL)|ノーマルマップをDirectX仕様とOpenGL仕様で相互変換します。|
|Assets/lilToon/[Texture] Pixel art reduction|ぼかしなしでドット絵の縮小を行います。|
|Assets/lilToon/[Texture] Convert Gif to Atlas|Gifからアトラステクスチャを生成します。処理内容はマテリアル設定の`Convert Gif`と同等です。|
|Assets/lilToon/[Model] Setup from FBX|FBXファイルから自動でマテリアルの生成、プリセットの適用、輪郭線マスク、影マスクの適用を行います。透過モードはマテリアルかテクスチャの名前に`cutout`が含まれている場合はカットアウト、`alpha`または`fade`または`transparent`が含まれている場合は半透明に変更されます。テクスチャ検索の命名規則は下記のリストをご覧ください。|

## テクスチャ命名規則（Setup from FBX）

大文字小文字の違いは無視されます。また、マテリアルにテクスチャが割り当てられている場合は元のテクスチャが優先されます。

|プロパティ名|命名規則|
|-|-|
|メインテクスチャ|マテリアル名を含み、メインテクスチャではなさそうな名前ではないもの。<br/>(例: マテリアル名=faceの場合)<br/>- OK: face.png / texture_face.png<br/>- NG: face_outline_mask.png / face_smoothness.png|
|輪郭線マスク|マテリアル名と`outline`を含むもの。<br/>(例: マテリアル名=faceの場合)<br/>- OK: face_outline.png / face_outline_mask.png<br/>- NG: face_mask.png / face_ol_mask.png|
|影マスク|マテリアル名と`shadow`/`shade`どちらかを含み、`mask`/`strength`どちらかを含むもの。<br/>(例: マテリアル名=faceの場合)<br/>- OK: face_shadow_mask.png / face_shadow_strength_mask.png<br/>- NG: face_shadow_color.png / face_shadow.png|

メインテクスチャとして認識されないワード  
`mask`, `shadow`, `shade`, `outline`, `normal`, `bumpmap`, `matcap`, `rimlight`, `emittion`, `reflection`, `specular`, `roughness`, `smoothness`, `metallic`, `metalness`, `opacity`, `parallax`, `displacement`, `height`, `ambient`, `occlusion`
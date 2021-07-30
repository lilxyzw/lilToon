# lilToon
Version 1.1.4a

# 概要
アバターを用いたサービス（VRChat等）向けに開発したシェーダーで以下のような特徴があります。
- 簡単（プリセットからワンクリック設定＆自作プリセットの保存、色調補正機能による色替え＆テクスチャ書き出し）
- 美麗（白飛び防止、水中などでの透け防止、アンチエイリアスシェーディング）
- 軽量（エディタが自動でシェーダーを書き換えて機能をオンオフ）
- 長期間、様々な環境で利用可能（Unity2017～2021、BRP/LWRP/URP）
- Unityの全ライティングに対応しStandardShaderに近い明るさに

# 対応状況
Unityバージョン
- Unity 2017 - Unity 2021.2

シェーダーモデル
- 通常版: SM4.0・ES3.0以降
- 軽量版: SM3.0・ES2.0以降
- ファーシェーダー: SM4.0・ES3.1+AEP・ES3.2以降
- テッセレーション: SM5.0・ES3.1+AEP・ES3.2以降

レンダリングパイプライン
- Built-in Render Pipeline (BRP)
- Lightweight Render Pipeline (LWRP)
- Universal Render Pipeline (URP)
- 屈折シェーダーはBRPのみ対応

# 主な機能
- メインカラーx3レイヤー（デカール、レイヤーマスク、Gifアニメーション、通常・加算・乗算・スクリーン合成対応）
- 色調補正、UVスクロール＆回転
- 自由度の高い影（2影、SSS、環境光の合成、AOマスクで影の付きやすさの調整可能）
- 発光x2レイヤー（アニメーション、マスク、点滅、色の時間変化、視差対応）
- ノーマルマップx2レイヤー
- 鏡面反射
- マットキャップ（Z軸回転キャンセル、通常・加算・乗算・スクリーン合成対応）
- リムライト
- アウトライン（テクスチャによる色指定、マスク、頂点カラー・距離に応じた太さ補正）
- ファー、屈折
- 距離クリッピングキャンセラー（近づきすぎても消えない）
- 距離フェード（距離に応じて色を変える）
- AudioLink（対応VRChatワールドで音に同期してマテリアルをアニメーション）
- テッセレーション（自動ハイポリ化、高負荷なので映像制作向け）
- メッシュの暗号化 (別途[AvatarEncryption](https://github.com/lilxyzw/AvaterEncryption)の導入が必要です)

# ライセンス
MIT Licenseで公開しています。同梱の`LICENSE`をご確認ください。

# 使い方 - 新規セットアップ
1. 下記いずれかの方法でUnityにlilToonをインポート  
    i. unitypackageをUnityウィンドウにドラッグ＆ドロップでインポート  
    ii. UPMから```https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master```をインポート  
2. Projectからマテリアルを選択
3. Inspector上部の`Shader`から`lilToon`を選択
4. (エディタの言語が異なる場合のみ) `Language`を`Japanese`に変更
5. テクスチャが反映されていない場合は`メインカラー`にテクスチャを設定
6. テクスチャを透過する場合は`Rendering Mode(透過モード)`を`カットアウト`または`半透明`に変更
7. 機能を追加したい場合は詳細設定内の[シェーダー設定](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL_JP.md#シェーダー設定)を変更

より詳しい設定については[マニュアル](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL_JP.md)を参照してください。

# 使い方 - アップデート
1. 下記いずれかの方法でUnityにlilToonをインポート  
    i. unitypackageをUnityウィンドウにドラッグ＆ドロップでインポート  
    ii. UPMから```https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master```をインポート  
2. 上部メニューバーの`Assets/lilToon/Refresh Shaders`をクリック

# lilToonを用いた制作物の配布手順について
シェーダー設定を変更された場合に見た目が変化しないように、マテリアルを右クリックし`Assets/lilToon/Remove unused properties`を実行しておくことをオススメします。また以前はlilToonSettingの同梱が必要でしたが、現在は不要になりました。アセットのインポート時にマテリアルとアニメーションをスキャンし自動で設定されます。  
1. 制作物のフォルダを選択
2. （シェーダーを同梱する場合のみ）ctrlを押しながら`lilToon`フォルダを選択
3. 右クリックし`Export package...`を選択
4. `Include Dependencies`のチェックを外す
5. `Export...`を押してunitypackageを保存

# よくあるトラブル
- マテリアルエラーが発生した  
  → 上部メニューバーの`Assets/lilToon/Refresh Shaders`をクリックすると改善される場合があります
- 顔にかかる影が気になる  
  → `マスクと強度`にマスクテクスチャを指定することで部分的に影を消すことができます。
- 輪郭線が汚くなる  
  → `マスクと太さ`にマスクテクスチャを指定することで部分的に輪郭線を消したり太さを調整したりすることができます。
- 明るい場所で影が薄くなる  
  → `環境光の強さ`の数値を下げると明るい場所でも影が強く出ます。
- 部位によって明るさが変わる  
  → アバターを右クリックし`[lilToon] Fix Lighting`を選択することで自動で修正されます。
- 一部機能が存在しないように見える  
  → 機能を追加したい場合は詳細設定の一番下のの[シェーダー設定](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL_JP.md#シェーダー設定)を変更してください
- シェーダー設定でどれをオンにすべきかわからない  
  → 上部メニューバーから`Assets/lilToon/Auto shader setting`を実行することで自動でシェーダー設定が行えます。

# シェーダー外のおすすめ設定
以下の設定で顔など一部分だけ明るさが違う現象を緩和できます。また、テクスチャの透過が綺麗になります。
- Projectから透過テクスチャを選択し、Inspectorから`Alpha Is Transparency`にチェックを入れる
- Cutoutマテリアルに使われるテクスチャは`Mip Maps Preserve Coverage`にチェックを入れる

# Liteバージョンについて
通常版の見た目をある程度維持しつつ大幅に軽量化したものです。こちらはシェーダー設定がなく機能が統一されているためアバター展示にオススメです。Lite版から直接マテリアルを設定せず、通常版で作成したものを変換するとより直感的にマテリアル設定が可能です。

# リファレンス
- [UnlitWF (whiteflare)](https://github.com/whiteflare/Unlit_WF_ShaderSuite) / [MIT LICENCE](https://github.com/whiteflare/Unlit_WF_ShaderSuite/blob/master/LICENSE)  
ファーシェーダーやプロパティ削除機能等、スクリプト・シェーダーともに非常に多くの部分を参考にさせていただきました。
- [Arktoon-Shaders (synqark)](https://github.com/synqark/Arktoon-Shaders) / [MIT LICENCE](https://github.com/synqark/Arktoon-Shaders/blob/master/LICENSE)  
影設定部分で参考にさせていただきました。
- [MToon (Santarh)](https://github.com/Santarh/MToon) / [MIT LICENCE](https://github.com/Santarh/MToon/blob/master/LICENSE)  
`MToon(VRM)に変換`の実装時に各種パラメータの比較を行いました。
- [GTAvaCrypt (rygo6)](https://github.com/rygo6/GTAvaCrypt) - [MIT LICENCE](https://github.com/rygo6/GTAvaCrypt/blob/master/LICENSE)
- [Optimized inverse trigonometric function (seblagarde)](https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/)
- [視差オクルージョンマッピング(parallax occlution mapping) (コポコポ)](https://coposuke.hateblo.jp/entry/2019/01/20/043042)

# 変更履歴
## v1.1.4a
- `Setup from FBX`がUnity2017.3以前、Unity2019.3以降で動かない問題の修正
## v1.1.4
- unitypackageインポート時のマテリアル・アニメーションの自動スキャン機能を追加
- `Auto shader setting`を追加、プロジェクト内の全マテリアル・アニメーションをスキャンし自動でシェーダー設定を最適化します
- `Remove unused properties`を追加、シェーダー設定を追加でオンにしても見た目に影響が出ないようにマテリアルを最適化します
- `Setup from FBX`を追加、FBXファイルから自動でマテリアルの生成、プリセットの適用、輪郭線マスク、影マスクの適用を行います
- `シェーダー設定`のロックを追加
- 一部プロパティの表示名の変更 (`環境光の強さ`→`影色への環境光影響度`、`太さを補正`→`距離に応じた太さ補正`)
- 再生ボタンを押してもInspectorの状態が維持されるように変更
- ファーシェーダーの`Unlit化`のプロパティをスライダー化
- ファーシェーダーに長さ調整用のマスクを追加
## v1.1.3
- バージョンチェック失敗時にInspectorが表示されない問題の修正
- `[lilToon] Fix Lighting`でClothコンポーネントがある、もしくはボーンが無い場合に壊れる問題の修正
## v1.1.2
- マットキャップにカスタムノーマルマップを追加
- リムライトにライト方向によるカスタマイズ機能を追加
- メッシュ・マテリアルのライティングを一括設定する機能を追加
- 輪郭線の太さに任意の値を設定可能に
- アルファマスクが輪郭線にも適用されるように変更
## v1.1.1
- プロパティのアルファが0のときに警告を表示
- エディターのテーマ変更に対応
- シェーダーリフレッシュの追加
## v1.1
- アルファマスクの追加
- マットキャップ2ndの追加
- 明るさの下限や頂点ライトの強度を調整可能に
- URPにおいてCascadeShadowがうまく動作していなかった点
- マットキャップの"ライティングを適用"が正しく動作していなかった点
- 負荷が比較的高い機能に警告を追加 (屈折・POM)
- 参考文献の追記
## v1.0
- 公開開始
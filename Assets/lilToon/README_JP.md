# lilToon
Version 1.0

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
4. テクスチャが反映されていない場合は`メインカラー`にテクスチャを設定
5. テクスチャを透過する場合は`Rendering Mode(透過モード)`を`カットアウト`または`半透明`に変更
6. 機能を追加したい場合は詳細設定内の"[シェーダー設定](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL_JP.md#シェーダー設定)"を変更

より詳しい設定については[マニュアル](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL_JP.md)を参照してください。

# lilToonを用いた制作物の配布手順について
`lilToonSetting`フォルダにシェーダー設定が保存されているためこちらも含めてunitypackage化することをオススメします。  
シェーダー設定を変更していない場合は同梱する必要はありません。  
詳細な手順は次の通りです。
1. 制作物のフォルダを選択
2. ctrlを押しながら`lilToonSetting`フォルダを選択
3. （シェーダーを同梱する場合のみ）ctrlを押しながら`lilToon`フォルダを選択
4. 右クリックし`Export package...`を選択
5. `Include Dependencies`のチェックを外す
6. `Export...`を押してunitypackageを保存

# よくあるトラブル
- 顔にかかる影が気になる  
  → `マスクと強度`にマスクテクスチャを指定することで部分的に影を消すことができます。
- 輪郭線が汚くなる  
  → `マスクと太さ`にマスクテクスチャを指定することで部分的に輪郭線を消したり太さを調整したりすることができます。
- 明るい場所で影が薄くなる  
  → `環境光の強さ`の数値を下げると明るい場所でも影が強く出ます。

# シェーダー外のおすすめ設定
以下の設定で顔など一部分だけ明るさが違う現象を緩和できます。また、テクスチャの透過が綺麗になります。
- Hierarchyからメッシュを選択し、`Root Bone`、`Bounds`、`Anchor Override`の設定を統一、`Recieve Shadows`をオフにする
- Projectから透過テクスチャを選択し、Inspectorから`Alpha Is Transparency`にチェックを入れる
- Cutoutマテリアルに使われるテクスチャは`Mip Maps Preserve Coverage`にチェックを入れる

# Liteバージョンについて
通常版の見た目をある程度維持しつつ大幅に軽量化したものです。こちらはシェーダー設定がなく機能が統一されているためアバター展示にオススメです。Lite版から直接マテリアルを設定せず、通常版で作成したものを変換するとより直感的にマテリアル設定が可能です。

# リファレンス
- [UnlitWF (whiteflare)](https://github.com/whiteflare/Unlit_WF_ShaderSuite)
- [Arktoon-Shaders (synqark)](https://github.com/synqark/Arktoon-Shaders)
- [MToon (Santarh)](https://github.com/Santarh/MToon)
- [Optimized inverse trigonometric function (seblagarde)](https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/)
- [視差オクルージョンマッピング(parallax occlution mapping) (コポコポ)](https://coposuke.hateblo.jp/entry/2019/01/20/043042)
- [GTAvaCrypt (rygo6)](https://github.com/rygo6/GTAvaCrypt)

# 変更履歴
## v1.0
- 公開開始
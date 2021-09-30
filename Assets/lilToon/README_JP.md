# lilToon
Version 1.2.0

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
- High Definition Render Pipeline 7.1.8 - 11.0.0 (HDRP)  
  HDRPではUnlit同様にForwardで描画しますが、ライト情報を受け取ります
- 屈折シェーダーと宝石シェーダーはBRPのみ対応

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
2. (SRPのみ) 上部メニューバーの`Assets/lilToon/Refresh Shaders`をクリック
3. Projectからマテリアルを選択
4. Inspector上部の`Shader`から`lilToon`を選択
5. (エディタの言語が異なる場合のみ) `Language`を`Japanese`に変更
6. テクスチャが反映されていない場合は`メインカラー`にテクスチャを設定
7. テクスチャを透過する場合は`Rendering Mode(透過モード)`を`カットアウト`または`半透明`に変更
8. 機能を追加したい場合は詳細設定内の[シェーダー設定](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL_JP.md#シェーダー設定)を変更

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
- エディタのエラーが発生した  
  → `lilToon`フォルダを右クリックし`Reimport`すると改善される場合があります
- アルファマスクが使えない  
  → 以下をご確認ください
  - 詳細設定の一番下の[シェーダー設定](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL_JP.md#シェーダー設定)で`アルファマスク`にチェックを入れて`Apply`されているか
  - 透過モードが`カットアウト`もしくは`半透明`になっているか
  - 基本色設定内のアルファマスクでテクスチャを割り当てているか
- 一部機能が存在しないように見える  
  → 機能を追加したい場合は詳細設定の一番下の[シェーダー設定](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/MANUAL_JP.md#シェーダー設定)を変更してください
- マテリアルを選択してもUIが表示されない・エディタにエラーがある  
  → 古いバージョンのlilToonが混在している可能性があります。`lilToon`フォルダを削除してから再度インポートし直して下さい。
- 部位によって明るさが変わる  
  → アバターを右クリックし`[lilToon] Fix Lighting`を選択することで自動で修正されます。
- 明るい場所で影が薄くなる  
  → `環境光の強さ`の数値を下げると明るい場所でも影が強く出ます。
- 顔にかかる影が気になる  
  → `マスクと強度`にマスクテクスチャを指定することで部分的に影を消すことができます。
- 輪郭線が汚くなる  
  → `マスクと太さ`にマスクテクスチャを指定することで部分的に輪郭線を消したり太さを調整したりすることができます。
- シェーダー設定でどれをオンにすべきかわからない  
  → 上部メニューバーから`Assets/lilToon/Auto shader setting`を実行することで自動でシェーダー設定が行えます。

これ以外でトラブルが発生し不具合であることが疑われる場合は[Twitter](https://twitter.com/lil_xyzw)、[GitHub](https://github.com/lilxyzw/lilToon)、[BOOTH](https://lilxyzw.booth.pm/)のいずれかにご連絡いただければ幸いです。  
以下にテンプレートも用意させていただきましたのでバグ報告の際の参考にご活用下さい。
```
バグ: 
再現方法: 

# 可能であれば
Unityバージョン: 
シェーダー設定: 
VRChatのワールド: 
スクリーンショット: 
コンソールログ: 
```

# シェーダー外のおすすめ設定
以下の設定で顔など一部分だけ明るさが違う現象を緩和できます。また、テクスチャの透過が綺麗になります。
- Projectから透過テクスチャを選択し、Inspectorから`Alpha Is Transparency`にチェックを入れる
- Cutoutマテリアルに使われるテクスチャは`Mip Maps Preserve Coverage`にチェックを入れる

# Liteバージョンについて
通常版の見た目をある程度維持しつつ大幅に軽量化したものです。こちらはシェーダー設定がなく機能が統一されているためアバター展示にオススメです。Lite版から直接マテリアルを設定せず、通常版で作成したものを変換するとより直感的にマテリアル設定が可能です。

# カスタムシェーダーの作成、シェーダーの書き換えについて
[DeveloperDocumentation_JP.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/DeveloperDocumentation_JP.md)を参照してください。

# 参考文献
[Third Party Notices.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/Third-Party-Notices.md)を参照してください。
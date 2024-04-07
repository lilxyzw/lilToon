# lilToon
Version 1.7.3

# 概要
アバターを用いたサービス（VRChat等）向けに開発したシェーダーで以下のような特徴があります。
- 簡単（プリセットからワンクリック設定＆自作プリセットの保存、色調補正機能による色替え＆テクスチャ書き出し）
- 美麗（白飛び防止、水中などでの透け防止、アンチエイリアスシェーディング）
- 軽量（エディタが自動でシェーダーを書き換えて機能をオンオフ）
- 長期間、様々な環境で利用可能（Unity2018～2023、BRP/LWRP/URP/HDRP）
- Unityの全ライティングに対応しStandardShaderに近い明るさに

# 対応状況
Unityバージョン
- Unity 2018.1 - Unity 2023.2

動作確認環境
- Unity 2018.1.0f2 (Built-in RP)
- Unity 2018.4.20f1 (Built-in RP / LWRP 4.10.0 / HDRP 4.10.0)
- Unity 2019.2.21f1 (Built-in RP / LWRP 6.9.2 / HDRP 6.9.2)
- Unity 2019.3.0f6 (Built-in RP / URP 7.1.8 / HDRP 7.1.8)
- Unity 2019.4.31f1 (Built-in RP / URP 7.7.1 / HDRP 7.7.1)
- Unity 2020.3.47f1 (Built-in RP / URP 10.10.1 / HDRP 10.10.1)
- Unity 2021.3.23f1 (Built-in RP / URP 12.1.11 / HDRP 12.1.11)
- Unity 2022.3.15f1 (Built-in RP / URP 14.0.8 / HDRP 14.0.8)
- Unity 2023.2.0a11 (Built-in RP / URP 16.0.1 / HDRP 16.0.1)

Unity 2021や2022の一部の古いバージョンではURP・HDRPでトランスフォームが適用されなかったりレンダリングされなかったりする等の不具合があります。問題が発生する場合は不具合が修正されたバージョンに更新してください。（ [GameObjects doesn't get rendered when using "Unlit.Unlit_UsePass" Shader](https://issuetracker.unity3d.com/issues/sphere-gameobject-doesnt-get-rendered-when-using-unlit-dot-unlit-usepass-shader) ）

Unity 2022や2023の一部の古いバージョンではシェーダー更新時にクラッシュが発生することがあります。問題が発生する場合は不具合が修正されたバージョンに更新してください。（ [Crash on malloc_internal when recompiling a ShaderGraph used by another shader via UsePass](https://issuetracker.unity3d.com/issues/crash-on-malloc-internal-when-recompiling-a-shadergraph-used-by-another-shader-via-usepass) ）

シェーダーモデル
- 通常版: SM4.0・ES3.0以降
- 軽量版: SM3.0・ES2.0以降
- ファーシェーダー: SM4.0・ES3.1+AEP・ES3.2以降
- テッセレーション: SM5.0・ES3.1+AEP・ES3.2以降

レンダリングパイプライン
- Built-in Render Pipeline
- Lightweight Render Pipeline 4.0.0 - 6.9.2
- Universal Render Pipeline 7.0.0 - 16.0.1
- High Definition Render Pipeline 4.0.0 - 16.0.1

# 主な機能
- メインカラーx3レイヤー（デカール、レイヤーマスク、Gifアニメーション、通常・加算・乗算・スクリーン合成対応）
- 色調補正、UVスクロール＆回転
- 自由度の高い影（3影、SSS、環境光の合成、AOマスクで影の付きやすさの調整可能）
- 発光x2レイヤー（アニメーション、マスク、点滅、色の時間変化、視差対応）
- ノーマルマップx2レイヤー
- 異方性反射
- 鏡面反射
- マットキャップx2（Z軸回転キャンセル、通常・加算・乗算・スクリーン合成対応）
- リムライト
- 逆光ライト
- アウトライン（テクスチャによる色指定、マスク、頂点カラー・距離に応じた太さ補正）
- ファー、屈折、宝石
- 距離クリッピングキャンセラー（近づきすぎても消えない）
- 距離フェード（距離に応じて色を変える）
- AudioLink（対応VRChatワールドで音に同期してマテリアルをアニメーション）
- テッセレーション（自動ハイポリ化、高負荷なので映像制作向け）
- メッシュの暗号化 (別途[AvatarEncryption](https://github.com/lilxyzw/AvaterEncryption)の導入が必要です)

# ライセンス
MIT Licenseで公開しています。同梱の`LICENSE`をご確認ください。サードパーティーのライセンスについては[Third Party Notices.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/Third%20Party%20Notices.md)をご確認ください。

# 使い方 - 新規セットアップ
1. 下記いずれかの方法でUnityにlilToonをインポート  
    i. unitypackageをUnityウィンドウにドラッグ＆ドロップでインポート  
    ii. UPMから`https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master`をインポート
2. Projectからマテリアルを選択
3. Inspector上部の`Shader`から`lilToon`を選択
4. (エディタの言語が異なる場合のみ) `Language`を`Japanese`に変更
5. テクスチャが反映されていない場合は`メインカラー`にテクスチャを設定
6. テクスチャを透過する場合は`Rendering Mode(透過モード)`を`カットアウト`または`半透明`に変更

より詳しい設定については[マニュアル](https://lilxyzw.github.io/lilToon/)を参照してください。

# 使い方 - アップデート
0. 1.1.8以前から1.2.0以降にアップデートする場合はインポート前にlilToonフォルダを削除
1. 下記いずれかの方法でUnityにlilToonをインポート  
    i. unitypackageをUnityウィンドウにドラッグ＆ドロップでインポート  
    ii. UPMから`https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master`をインポート

# lilToonを用いた制作物の配布手順について
- シェーダーを同梱する場合は、[BOOTH](https://booth.pm/ja/items/3087170)や[GitHub](https://github.com/lilxyzw/lilToon/releases)のダウンロードページへのショートカットを同梱する方法か、ダウンロードしてきたそのままのシェーダーのunitypackageを同梱する方法がオススメです
- シェーダー本体と制作物を1つのunitypackageにまとめる方法は非推奨となりました（インポート時に古いバージョンで上書きしてしまう問題が発生する可能性があるため）

# よくあるトラブル
- マテリアルエラーが発生した  
  → 上部メニューバーの`Assets/lilToon/Refresh Shaders`をクリックすると改善される場合があります
- エディタのエラーが発生した  
  → `lilToon`フォルダを右クリックし`Reimport`すると改善される場合があります
- アルファマスクが使えない  
  → 以下をご確認ください
  - 透過モードが`カットアウト`もしくは`半透明`になっているか
  - 基本色設定内のアルファマスクでテクスチャを割り当てているか
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

これ以外でトラブルが発生し不具合であることが疑われる場合は[Twitter](https://twitter.com/lil_xyzw)、[GitHub](https://github.com/lilxyzw/lilToon)、[BOOTH](https://lilxyzw.booth.pm/)のいずれかにご連絡いただければ幸いです。以下にテンプレートも用意させていただきましたのでバグ報告の際の参考にご活用下さい。
```
バグ: 
再現方法: 

# 可能であれば
Unityバージョン: 
VRChatのワールド: 
スクリーンショット: 
コンソールログ: 
```
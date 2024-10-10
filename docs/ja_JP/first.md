# はじめに

[[toc]]

## 登場する用語
Unity等で3DCGを扱う際に登場する用語です。これらを把握しておくことで、マニュアルや自分が求めている機能を理解しやすくなります。

|名前|説明|
|-|-|
|マテリアル|ものの見え方を決めるデータです。|
|テクスチャ|画像のことです。ものの色を決めるときなど、様々なことに使われます。|
|UV|テクスチャを貼る位置を決めるデータです。|
|マスク|処理を行う部分を指定するために使われるテクスチャです。|
|ノーマルマップ|表面に凹凸があるように見せかけるテクスチャです。影や光沢の出方に影響します。|
|マットキャップ|光の反射を描き込んだテクスチャです。|
|リムライト|逆光のように光が周り込み、ものの輪郭だけ明るくなるライトです。|
|ステンシル|画面上で行われるマスク表現です。髪の上に眉毛を描画するなどの表現ができます。|

## 導入手順と簡易的な使い方

<div class="timeline">

<div class="timeline_part">
<div class="timeline_label">STEP 1</div>
<div class="timeline_title">シェーダーのインストール</div>
<div class="timeline_text">

インストールは以下の手順のうち、**どれか1つ**を行ってください。

- [BOOTH](https://lilxyzw.booth.pm/items/3087170)からダウンロードしzipを展開、中の`.unitypackage`ファイルをUnityのProjectウィンドウ内にドラッグ&ドロップ
- [こちら](vcc://vpm/addRepo?url=https://lilxyzw.github.io/vpm-repos/vpm.json)をクリックしてlilToonをVCCまたはALCOMに追加し、Manage Projectのパッケージ一覧からlilToonをインストール
- [Unity Package Manager](https://docs.unity3d.com/ja/current/Manual/upm-ui-giturl.html)を用いて追加

::: info
**既に設定済みのモデルデータがある場合は STEP 2 以降の作業は不要**です。STEP 2以降はモデルを新規作成したりシェーダーを移行する場合の手順となっています。
:::

</div>
</div>

<div class="timeline_part timeline_part_sub">
<div class="timeline_label">STEP 2</div>
<div class="timeline_title">マテリアルの作成</div>
<div class="timeline_text">

マテリアルを選択し、Inspector上部の"Shader"を"lilToon"に変更してください。**"_lil"内にあるシェーダーは特殊なものなので、基本的には通常の"lilToon"を選択してください**。また、マテリアルが無い場合は手動で新規に作成するか、FBXのインポート設定でMaterialsタブを選択し<a href="https://docs.unity3d.com/ja/current/Manual/FBXImporter-Materials.html" target="_blank" rel="noopener noreferrer">インポート設定を変更</a>してください。

</div>
</div>

<div class="timeline_part timeline_part_sub">
<div class="timeline_label">STEP 3</div>
<div class="timeline_title">編集モードの変更</div>
<div class="timeline_text">

次に編集モードを**詳細設定**に変更してください。簡易設定は<u>既にセットアップ済みのモデルの色だけを変更したい初心者向けのモード</u>、詳細設定は<u>新規セットアップやシェーダー移行、細かい改変をしたい中・上級者向けのモード</u>となっています。

::: warning
一部環境でデフォルトで日本語表示になっていない場合があります。その場合はLanguageをJapaneseに変更してください。
:::

</div>
</div>

<div class="timeline_part timeline_part_sub">
<div class="timeline_label">STEP 4</div>
<div class="timeline_title">テクスチャの割り当て</div>
<div class="timeline_text">

テクスチャが割り当てられていない場合は基本色設定内の`メインカラー`にテクスチャを指定してください。ノーマルマップなど他の種類のテクスチャがある場合は以下のように対応した箇所に割り当てます。

|種類|割り当てる場所|
|-|-|
|メインテクスチャ|基本色設定 -> メインカラー -> テクスチャ|
|ノーマルマップ|ノーマルマップ設定 -> ノーマルマップ -> ノーマルマップ|
|アウトラインマスク|輪郭線設定 -> 輪郭線 -> マスクと太さ|
|影マスク|影設定 -> 影 -> マスクと強度|
|マットキャップ|マットキャップ設定 -> マットキャップ -> マットキャップ|
|マットキャップマスク|マットキャップ設定 -> マットキャップ -> マスク|
|リムライトマスク|リムライト設定 -> リムライト -> 色 / マスク|
|エミッション（マスク）|発光設定 -> 発光テクスチャ -> 色|

</div>
</div>

<div class="timeline_part timeline_part_sub">
<div class="timeline_label">STEP 5</div>
<div class="timeline_title">描画モードの変更</div>
<div class="timeline_text">

テクスチャを透過する場合は`描画モード`をカットアウトまたは半透明に変更してください。

</div>
</div>

<div class="timeline_part timeline_part_sub">
<div class="timeline_label">STEP 6</div>
<div class="timeline_title">プリセットの適用</div>
<div class="timeline_text">

編集モードをプリセットに変更して目的に合ったプリセットを適用することで簡易的にセットアップできます。基本的な使い方は以上になります。より細かく編集したい場合は各機能のマニュアルをご確認ください。

</div>
</div>

</div>

## 対応状況
長期間、様々な環境で利用できるように幅広いUnityバージョンに対応しています。

### Unityバージョン

- 2018.3 - 2018.4
- 2019.1 - 2019.4
- 2020.1 - 2020.3
- 2021.1 - 2021.3
- 2022.1 - 2022.3
- 2023.1 - 2023.3

### シェーダーモデル

- 通常版: SM4.0・ES3.0
- 軽量版: SM3.0・ES2.0
- ファー: SM4.0・ES3.1+AEP・ES3.2
- テッセレーション: SM5.0・ES3.1+AEP・ES3.2

### レンダリングパイプライン

- Built-in Render Pipeline
- Lightweight Render Pipeline
- Universal Render Pipeline
- High Definition Render Pipeline

※ 動作確認環境
- Unity 2018.1.0f2 (Built-in RP)
- Unity 2018.4.20f1 (Built-in RP / LWRP 4.10.0 / HDRP 4.10.0)
- Unity 2019.2.21f1 (Built-in RP / LWRP 6.9.2 / HDRP 6.9.2)
- Unity 2019.3.0f6 (Built-in RP / URP 7.1.8 / HDRP 7.1.8)
- Unity 2019.4.31f1 (Built-in RP / URP 7.7.1 / HDRP 7.7.1)
- Unity 2020.3.47f1 (Built-in RP / URP 10.10.1 / HDRP 10.10.1)
- Unity 2021.3.23f1 (Built-in RP / URP 12.1.11 / HDRP 12.1.11)
- Unity 2022.3.22f1 (Built-in RP / URP 14.0.8 / HDRP 14.0.8)
- Unity 2023.2.0a11 (Built-in RP / URP 16.0.1 / HDRP 16.0.1)

## lilToonを用いた制作物の配布について
- VRChat向けの配布物である場合、VCCからのインストールを案内することをオススメします
- シェーダーを同梱する場合は、[BOOTH](https://booth.pm/ja/items/3087170)のダウンロードページへのショートカットを同梱する方法か、ダウンロードしてきたそのままのシェーダーのunitypackageを同梱する方法がオススメです
- シェーダー本体と制作物を1つのunitypackageにまとめる方法は、ユーザーがインポート時に古いバージョンで上書きしてしまう問題が発生する可能性があるため非推奨です

## ロゴデータ
ロゴデータは[こちら](https://github.com/lilxyzw/lilToon/tree/master/logo)からダウンロードできます。BOOTH等の販売サイトでのサムネイルへの使用や作品への使用・同梱が可能で、商品ページでのライセンスの記載も不要です。色や解像度を変更し、作品にマッチしたイメージに変更することもできます。

## ライセンス
本シェーダーは[MIT License](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/LICENSE)で公開しています。日本語参考訳は[こちら](https://licenses.opensource.jp/MIT/MIT.html)で確認いただけます。サードパーティーのライセンスについては[Third Party Notices.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/Third%20Party%20Notices.md)をご確認ください。

## シェーダーバリエーション
パッケージに含まれるシェーダーのバリエーションの一覧です。

|名前|説明|
|-|-|
|lilToon|メインのシェーダーです。一般的な用途ではこれを使用します。|
|_lil/[Optional] lilToonOverlay|マテリアルの上に重ねて表示するための透過シェーダーです。不要なパスが除去されており、通常の透過シェーダーを重ねるより低負荷です。|
|_lil/[Optional] lilToonOutlineOnly|輪郭線のみのシェーダーです。例えば、ハードエッジのモデルで輪郭線を描画する場合に別途法線がスムーズなメッシュを用意してこのシェーダーを割り当てることにより、通常より綺麗な輪郭線を描画できます。|
|_lil/[Optional] lilToonFurOnly|ファーの毛の部分のみを描画するシェーダーです。通常のシェーダーに重ねる表現や、カットアウトのファーと透過のファーを組み合わせた表現をする場合にオススメです。|
|_lil/lilToonMulti|シェーダーキーワードを利用するバージョンです。マテリアルを大量に使用する場合などにビルドサイズが大きくなりやすいため注意が必要です。|
|Hidden/lilToonLite|通常版の見た目をある程度維持しつつ大幅に軽量化したものです。Lite版から直接マテリアルを設定せず、通常版で作成したものを変換するとより直感的にマテリアル設定が可能です。|
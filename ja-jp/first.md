# はじめに

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
    <div class="timeline_title">シェーダーのダウンロード</div>
    <div class="timeline_text">ダウンロードは<a href="https://lilxyzw.booth.pm/items/3087170">BOOTH</a>や<a href="https://github.com/lilxyzw/lilToon/releases">GitHub</a>から行えます。いくつかバリエーションがありますが、用途にあったもの1つだけをインポートしてください。
    <div class="window_info">&#x1f530; VRChatで使用する場合は<a href="vcc://vpm/addRepo?url=https://lilxyzw.github.io/vpm-repos/vpm.json">こちら</a>からVCCに追加することもできます。VCCに追加するとManage Projectのパッケージ一覧に表示されますので+ボタンを押すかパッケージのバージョンを選択して追加してください。<span>VCCから追加した場合はSTEP 2のunitypackageのインポートは不要</span>です。</div>

|種類|用途|
|-|-|
|無印|Built-in RP用のものです。VRChatで使用する場合はこちらをご利用ください。BOOTHからダウンロードできるファイルにはこのバリエーションのみ含まれています。|
|LWRP|Lightweight Render Pipeline（LWRP）用のものです。URPの旧バージョンにあたり、現在ではほぼ使われないと考えられますが対応コストがほぼ無いため残しています。将来的にサポートを削除する可能性があります。|
|URP|Universal Render Pipeline（URP）用のものです。|
|HDRP|High Definition Render Pipeline（HDRP）用のものです。|

</div>
</div>

<div class="timeline_part">
    <div class="timeline_label">STEP 2</div>
    <div class="timeline_title">unitypackageのインポート</div>
    <div class="timeline_text">次に、先程ダウンロードした拡張子が「<span>.unitypackage</span>」のファイルをUnity内の<span>Projectウィンドウ内にドラッグ&ドロップ</span>してください。<a href="https://docs.unity3d.com/ja/current/Manual/upm-ui-giturl.html" target="_blank" rel="noopener noreferrer">Unity Package Managerを用いる方法</a>もありますが、アップデートしにくいためVRChat用途ではunitypackageを用いる方法が無難かもしれません。
        <div class="window_info">&#x1f530; <span>既に設定済みのモデルデータがある場合は STEP 3 以降の作業は不要</span>です。STEP 3以降はモデルを新規作成したりシェーダーを移行する場合の手順となっています。</div>
    </div>
</div>
<div class="timeline_part timeline_part_sub">
    <div class="timeline_label">STEP 3</div>
    <div class="timeline_title">マテリアルの作成</div>
    <div class="timeline_text">マテリアルを選択し、Inspector上部の"Shader"を"lilToon"に変更してください。<span>"_lil"内にあるシェーダーは特殊なものなので、基本的には通常の"lilToon"を選択してください</span>。また、マテリアルが無い場合は手動で新規に作成するか、FBXのインポート設定でMaterialsタブを選択し<a href="https://docs.unity3d.com/ja/current/Manual/FBXImporter-Materials.html" target="_blank" rel="noopener noreferrer">インポート設定を変更</a>してください。</div>
</div>
<div class="timeline_part timeline_part_sub">
    <div class="timeline_label">STEP 4</div>
    <div class="timeline_title">編集モードの変更</div>
    <div class="timeline_text">次に<a href="#">編集モード</a>を<span>詳細設定</span>に変更してください。簡易設定は<span>既にセットアップ済みのモデルの色だけを変更したい初心者向けのモード</span>、詳細設定は<span>新規セットアップやシェーダー移行、細かい改変をしたい中・上級者向けのモード</span>となっています。
        <div class="window_info">&#x26a0; 一部環境でデフォルトで日本語表示になっていない場合があります。その場合はLanguageを<span>Japanese</span>に変更してください。</div>
    </div>
</div>
<div class="timeline_part timeline_part_sub">
    <div class="timeline_label">STEP 5</div>
    <div class="timeline_title">テクスチャの割り当て</div>
    <div class="timeline_text">テクスチャが割り当てられていない場合は基本色設定内の<span>メインカラー</span>にテクスチャを指定してください。ノーマルマップなど他の種類のテクスチャがある場合は以下のように対応した箇所に割り当てます。

|種類|割り当てる場所|
|-|-|
|メインテクスチャ|基本色設定 > メインカラー > テクスチャ|
|ノーマルマップ|ノーマルマップ設定 > ノーマルマップ > ノーマルマップ|
|アウトラインマスク|輪郭線設定 > 輪郭線 > マスクと太さ|
|影マスク|影設定 > 影 > マスクと強度|
|マットキャップ|マットキャップ設定 > マットキャップ > マットキャップ|
|マットキャップマスク|マットキャップ設定 > マットキャップ > マスク|
|リムライトマスク|リムライト設定 > リムライト > 色 / マスク|
|エミッション（マスク）|発光設定 > 発光テクスチャ > 色|

</div>
</div>
<div class="timeline_part timeline_part_sub">
    <div class="timeline_label">STEP 6</div>
    <div class="timeline_title">描画モードの変更</div>
    <div class="timeline_text">テクスチャを透過する場合は"Rendering Mode（透過モード）"を<span>カットアウト</span>または<span>半透明</span>に変更してください。</div>
</div>
<div class="timeline_part timeline_part_sub">
    <div class="timeline_label">STEP 7</div>
    <div class="timeline_title">プリセットの適用</div>
    <div class="timeline_text"><a href="#">編集モード</a>をプリセットに変更して目的に合ったプリセットを適用することで簡易的にセットアップできます。基本的な使い方は以上になります。より細かく編集したい場合は各機能のマニュアルをご確認ください。</div>
</div>
</div>

## lilToonを用いた制作物の配布について
- シェーダーを同梱する場合は、[BOOTH](https://booth.pm/ja/items/3087170)や[GitHub](https://github.com/lilxyzw/lilToon/releases)のダウンロードページへのショートカットを同梱する方法か、ダウンロードしてきたそのままのシェーダーのunitypackageを同梱する方法がオススメです
- シェーダー本体と制作物を1つのunitypackageにまとめる方法は、ユーザーがインポート時に古いバージョンで上書きしてしまう問題が発生する可能性があるため非推奨です

## ロゴデータ
ロゴデータは[こちら](https://github.com/lilxyzw/lilToon/tree/master/logo)からダウンロードできます。  
BOOTH等の販売サイトでのサムネイルへの使用や作品への使用・同梱が可能で、商品ページでのライセンスの記載も不要です。  
色や解像度を変更し、作品にマッチしたイメージに変更することもできます。

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
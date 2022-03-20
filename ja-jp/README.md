# lilToonについて

<div class="align-center">

## ダウンロード
シェーダー: [BOOTH](https://lilxyzw.booth.pm/items/3087170) / [GitHub](https://github.com/lilxyzw/lilToon/releases)  
ロゴ: [GitHub](https://github.com/lilxyzw/lilToon/tree/master/logo)

## 特徴
アバターを用いたサービス向けに開発したシェーダーで以下のような特徴があります。

</div>

<div class="flexwrapcontainer">
<div class="flex2">
<h3>簡単</h3>
<p>プリセットからのワンクリック設定、自作プリセットの保存に対応。一度作り込んだマテリアルは次回からワンクリックで設定できます。また、アバター改変向けに色調補正機能も搭載し、作った色はテクスチャとして書き出すこともできます。</p>
</div>

<div class="flex2">
<h3>美麗</h3>
<p>イラストの塗りを徹底的に研究し、シェーダー上で表現できるように多彩な機能を搭載。アンチエイリアスシェーディングにより、アニメ塗りも滑らかに再現できます。また、VRSNSの事情を考慮して徹底的な白飛び防止と、水などの半透明なオブジェクト越しの透け防止などの対策も行っています。</p>
</div>

<div class="flex2">
<h3>軽量</h3>
<p>エディタが自動でシェーダーを書き換えて機能をオンオフし、不要な機能を除外することで最低限の負荷に抑えられます。また、ビルドサイズも最低限に抑える工夫をしているため、VRSNSでのアバター容量を削減できます。</p>
</div>

<div class="flex2">
<h3>安定</h3>
<p>Unityの全ライティングに対応済みでStandard Shaderに近い明るさに。さらに長期間、様々な環境で利用できるように幅広いUnityバージョンに対応しており、シェーダー移行の手間を減らします。（Unity2017 - 2021、BRP/LWRP/URP/HDRP）</p>
</div>
</div>

<div class="bg-black">
<div class="align-center">

## 対応状況
長期間、様々な環境で利用できるように幅広いUnityバージョンに対応しています。

</div>

<div class="flexcontainer">
    <div class="flex3">
        <h3>Unityバージョン</h3>
        <ul>
            <li>2017.1 - 2017.4</li>
            <li>2018.1 - 2018.4</li>
            <li>2019.1 - 2019.4</li>
            <li>2020.1 - 2020.3</li>
            <li>2021.1 - 2021.2</li>
        </ul>
    </div>
    <div class="flex3">
        <h3>シェーダーモデル</h3>
        <ul>
            <li>通常版: SM4.0・ES3.0</li>
            <li>軽量版: SM3.0・ES2.0</li>
            <li>ファー: SM4.0・ES3.1+AEP・ES3.2</li>
            <li>テッセレーション: SM5.0・ES3.1+AEP・ES3.2</li>
        </ul>
    </div>
    <div class="flex3">
        <h3>レンダリングパイプライン</h3>
        <ul>
            <li>Built-in Render Pipeline</li>
            <li>Lightweight Render Pipeline</li>
            <li>Universal Render Pipeline</li>
            <li>High Definition Render Pipeline</li>
        </ul>
    </div>
</div>

<div class="small-container">

※ 動作確認環境
- Unity 2017.1.0f3
- Unity 2018.4.20f1 (Built-in RP / LWRP 4.0.0 / HDRP 4.0.0)
- Unity 2019.2.0f1 (Built-in RP / LWRP 6.9.0 / HDRP 6.9.0)
- Unity 2019.3.0f6 (Built-in RP / URP 7.1.8 / HDRP 7.1.8)
- Unity 2019.4.31f1 (Built-in RP / URP 7.7.1 / HDRP 7.7.1)
- Unity 2020.3.20f1 (Built-in RP / URP 10.6.0 / HDRP 10.6.0)
- Unity 2021.1.24f1 (Built-in RP / URP 11.0.0 / HDRP 11.0.0)
- Unity 2021.2.9f1 (Built-in RP / URP 12.1.4 / HDRP 12.1.4)

</div>
</div>

## 主な機能
- [UVスクロール＆回転](/ja-jp/color?id=uv設定)
- [メインカラー](/ja-jp/color?id=メインカラー-透過) - アバター改変を想定し色調補正による簡易色替え＆テクスチャ書き出しに対応
- [メインカラー2nd/3rd](/ja-jp/color?id=メインカラー2nd・3rd) - デカール・ディテール、レイヤーマスク、Gifアニメーション、距離フェード、通常・加算・乗算・スクリーン合成に対応
- [アルファマスク](/ja-jp/color?id=アルファマスク)
- [3影](/ja-jp/color?id=影設定) - テクスチャによる色指定、影の境界の色指定、AOマスクによって影の付きやすさを調整可能
- [発光x2](/ja-jp/color?id=発光設定) - アニメーション、マスク、点滅、色の時間変化、視差対応
- [ノーマルマップx2](/ja-jp/reflections?id=ノーマルマップ設定) - 個別にUV・強度を調整可能です。1枚目で大まかに法線を調整し2枚目でディテールを付与する、というような使い方ができます。
- [異方性反射](/ja-jp/reflections?id=異方性反射) - 反射・マットキャップそれぞれに適用可能です。髪の毛などの複雑な質感の表現ができます。
- [逆光ライト](/ja-jp/reflections?id=逆光ライト) - ライトが逆光になったときにリムライトのように光を加算できます。
- [鏡面反射](/ja-jp/reflections?id=光沢設定) - スペキュラ（ハイライト）・リフレクションに対応しフォトリアルな表現も加えることができます。CubeMapのフォールバック・オーバーライドにも対応しているため環境に影響されない表現もできます。
- [マットキャップx2](/ja-jp/reflections?id=マットキャップ設定) - Z軸回転キャンセル、パースの補正、UV1のブレンドによるエンジェルリング表現、通常・加算・乗算・スクリーン合成に対応しています。
- [リムライト](/ja-jp/reflections?id=リムライト設定)
- [ラメ](/ja-jp/reflections?id=ラメ設定)
- [宝石](/ja-jp/reflections?id=宝石設定)
- [輪郭線](/ja-jp/advanced?id=輪郭線設定) - テクスチャによる色指定、マスク、頂点カラー・距離に応じた太さ補正
- [視差マップ](/ja-jp/advanced?id=視差マップ)
- [距離フェード](/ja-jp/advanced?id=距離フェード) - 距離に応じて色を変える
- [AudioLink](/ja-jp/advanced?id=audiolink) - 対応VRChatワールドで音に同期してマテリアルをアニメーション
- [Dissolve](/ja-jp/advanced?id=dissolve)
- [AvatarEncryption](/ja-jp/advanced?id=暗号化) - メッシュの暗号化
- [テッセレーション](/ja-jp/advanced?id=テッセレーション) - 自動ハイポリ化、高負荷なので映像制作向け
- [ファー](/ja-jp/advanced?id=ファー設定)
- [屈折](/ja-jp/advanced?id=屈折設定)
- 距離クリッピングキャンセラー - 近づきすぎても消えない
- [VRChat向け機能](/ja-jp/base?id=vrchat) - シェーダーブロック時のフォールバック先のシェーダー選択機能

## ライセンス
本シェーダーは[MIT License](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/LICENSE)で公開しています。日本語参考訳は[こちら](https://licenses.opensource.jp/MIT/MIT.html)で確認いただけます。  
サードパーティーのライセンスについては[Third Party Notices.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/Third%20Party%20Notices.md)をご確認ください。
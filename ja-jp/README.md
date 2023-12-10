# 概要

<div class="align-center">

## ダウンロード
シェーダー: [BOOTH](https://lilxyzw.booth.pm/items/3087170) / [GitHub](https://github.com/lilxyzw/lilToon/releases) / [VRChat Creator Companion](vcc://vpm/addRepo?url=https://lilxyzw.github.io/vpm-repos/vpm.json)  
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
            <li>2018.1 - 2018.4</li>
            <li>2019.1 - 2019.4</li>
            <li>2020.1 - 2020.3</li>
            <li>2021.1 - 2021.3</li>
            <li>2022.1 - 2022.3</li>
            <li>2023.1 - 2023.2</li>
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
- Unity 2018.1.0f2 (Built-in RP)
- Unity 2018.4.20f1 (Built-in RP / LWRP 4.10.0 / HDRP 4.10.0)
- Unity 2019.2.21f1 (Built-in RP / LWRP 6.9.2 / HDRP 6.9.2)
- Unity 2019.3.0f6 (Built-in RP / URP 7.1.8 / HDRP 7.1.8)
- Unity 2019.4.31f1 (Built-in RP / URP 7.7.1 / HDRP 7.7.1)
- Unity 2020.3.47f1 (Built-in RP / URP 10.10.1 / HDRP 10.10.1)
- Unity 2021.3.23f1 (Built-in RP / URP 12.1.11 / HDRP 12.1.11)
- Unity 2022.3.15f1 (Built-in RP / URP 14.0.8 / HDRP 14.0.8)
- Unity 2023.2.0a11 (Built-in RP / URP 16.0.1 / HDRP 16.0.1)

</div>
</div>

## 主な機能
- [UVスクロール＆回転](/ja-jp/base/uv.md) - タイリングやアニメーションさせたり、ポリゴンの表・裏で異なるUVにしたりなど様々な変更を加えることができます。
- [メインカラー](/ja-jp/color/maincolor.md) - アバター改変を想定し色調補正による色替え＆テクスチャ書き出しに対応しています。
- [メインカラー2nd/3rd](/ja-jp/color/maincolor_layer.md) - デカール・ディテール、レイヤーマスク、Gifアニメーション、距離フェード、様々な合成モードに対応しています。
- [アルファマスク](/ja-jp/color/alphamask.md) - メインテクスチャのアルファチャンネルに焼き込んで書き出すこともできます。
- [3影](/ja-jp/color/shadow.md) - テクスチャによる色指定、影の境界の色指定、AOマスクによって影の付きやすさを調整可能です。
- [発光x2](/ja-jp/color/emission.md) - アニメーション、マスク、点滅、色の時間変化、視差対応で多彩な表現ができます。
- [ノーマルマップx2](/ja-jp/reflections/normal.md) - 個別にUV・強度を調整可能で、1枚目で大まかな影の付き方の調整し、2枚目で細かいディテールを加えるような使い方ができます。
- [異方性反射](/ja-jp/reflections/anisotropy.md) - 反射・マットキャップそれぞれに適用可能です。髪の毛やヘアライン仕上げのような複雑な質感の表現ができます。
- [逆光ライト](/ja-jp/reflections/backlight.md) - イラストでよく使われる後ろから光が差し込む表現ができます。
- [鏡面反射](/ja-jp/reflections/reflection.md) - イラスト調のハイライトからフォトリアルな反射まで表現できます。CubeMapのフォールバック・オーバーライドにも対応しているため環境に影響されない表現もできます。
- [マットキャップx2](/ja-jp/reflections/matcap.md) - Z軸回転キャンセル、パースの補正、UV1のブレンドによるエンジェルリング表現、様々な合成モードに対応しています。
- [リムライト](/ja-jp/reflections/rimlight.md) - ライト方向に応じて日向部分・影部分個別に色やぼかし量・範囲を調節することができます。
- [ラメ](/ja-jp/reflections/glitter.md) - ラメのような複雑に輝く質感の表現ができます。
- [宝石](/ja-jp/reflections/gem.md) - 通常のシェーダーでは表現が困難な宝石のキラキラした質感を表現できます。
- [輪郭線](/ja-jp/advanced/outline.md) - テクスチャによる色指定、マスク、距離に応じた太さ補正に対応しています。また、ハードエッジのモデルでも頂点カラーを用いた滑らかな描画が可能です。
- [視差マップ](/ja-jp/advanced/parallax.md) - 視線方向に応じてUVをずらすことで擬似的に立体感を出すことができます。
- [距離フェード](/ja-jp/advanced/distancefade.md) - 近づいたときに暗くすることで臨場感を出すような使い方や、フェードのかかり方を逆にしてフォグのように利用することもできます。
- [AudioLink](/ja-jp/advanced/audiolink.md) - 対応VRChatワールドで音に同期してマテリアルをアニメーション可能です。
- [Dissolve](/ja-jp/advanced?id=dissolve.md) - 出現・退場アニメーションや変身エフェクト、部分的に溶ける表現など様々な使い方ができます。
- [AvatarEncryption](/ja-jp/advanced/encryption.md) - 4つのキーを用いてメッシュを暗号化することでリッピングを防止します。
- [テッセレーション](/ja-jp/advanced/tessellation.md) - 近づいた際にモデルをなめらかにする、主に映像制作向けの機能です。
- [屈折](/ja-jp/advanced/refraction.md) - ガラスのような屈折表現を行えます。
- [ファー](/ja-jp/advanced/fur.md) - 毛のような複雑な質感を表現できます。
- 距離クリッピングキャンセラー - 近づきすぎても消えなくすることができます。VR等で特に効果を発揮します。シェーダー設定からオンにすることで利用できます。
- [VRChat向け機能](/ja-jp/base/vrchat.md) - シェーダーブロック時のフォールバック先のシェーダー選択機能です。

## ライセンス
本シェーダーは[MIT License](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/LICENSE)で公開しています。日本語参考訳は[こちら](https://licenses.opensource.jp/MIT/MIT.html)で確認いただけます。サードパーティーのライセンスについては[Third Party Notices.md](https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/Third%20Party%20Notices.md)をご確認ください。
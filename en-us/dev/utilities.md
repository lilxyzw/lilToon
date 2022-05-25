# ユーティリティ
レンダリングパイプラインやAPIの差を吸収するためのマクロや関数が用意されています。より詳しくはHLSLファイルをご確認ください。

## テクスチャ
|名前|説明|
|-|-|
|TEXTURE2D(_Tex)|Texture2D _Tex|
|SAMPLER(sampler_Tex)|SamplerState sampler_Tex|
|LIL_SAMPLE_2D(Texture2D _Tex, SamplerState sampler_Tex, float2 uv)|_Tex.Sample(sampler_Tex, uv)|

## トランスフォーム
|名前|説明|
|-|-|
|LIL_MATRIX_M|ワールド行列|
|LIL_MATRIX_I_M|ワールドの転置行列|
|LIL_MATRIX_V|ビュー行列|
|LIL_MATRIX_P|プロジェクト行列|
|LIL_MATRIX_VP|ビュー&プロジェクト行列|
|float3 lilTransformOStoWS(float4 positionOS)|オブジェクト座標からワールド座標への変換|
|float3 lilTransformOStoWS(float3 positionOS)|オブジェクト座標からワールド座標への変換|
|float3 lilTransformWStoOS(float3 positionWS)|ワールド座標からオブジェクト座標への変換|
|float3 lilTransformWStoVS(float3 positionWS)|ワールド座標からビュー座標への変換|
|float4 lilTransformWStoCS(float3 positionWS)|ワールド座標からクリップ座標への変換|
|float4 lilTransformVStoCS(float3 positionVS)|ビュー座標からクリップ座標への変換|
|float4 lilTransformCStoSS(float4 positionCS)|クリップ座標から画面座標への変換|
|float4 lilTransformCStoSSFrag(float4 positionCS)|クリップ座標から画面座標への変換（Fragmentシェーダー用）|
|float3 lilToAbsolutePositionWS(float3 positionRWS)|カメラからの相対座標から絶対座標への変換（HDRP用）|
|float3 lilTransformDirOStoWS(float3 directionOS, bool doNormalize)|ベクトルのオブジェクト空間からワールド空間への変換|
|float3 lilTransformDirWStoOS(float3 directionWS, bool doNormalize)|ベクトルのワールド空間からオブジェクト空間への変換|
|float3 lilTransformNormalOStoWS(float3 normalOS, bool doNormalize)|法線のオブジェクト空間からワールド空間への変換|
|float3 lilViewDirection(float3 positionWS)|normalizeされていないView Directionの取得|
|float3 lilHeadDirection(float3 positionWS)|normalizeされていないView Directionの取得（VRでは２つの目の間の座標を元に計算し視差を無効化）|
|float2 lilCStoGrabUV(float4 positionCS)|クリップ座標からGrabPass用UVを取得|

## 座標のトランスフォーム (LIL_VERTEX_POSITION_INPUTS)
|名前|説明|
|-|-|
|LIL_VERTEX_POSITION_INPUTS(float4 positionOS, out vertexInput)|座標のトランスフォーム|
|LIL_RE_VERTEX_POSITION_INPUTS(vertexInput)|ワールド座標を変更した場合にトランスフォームを再計算|
|vertexInput.positionWS|ワールド座標|
|vertexInput.positionVS|ビュー座標|
|vertexInput.positionCS|クリップ座標 (SV_POSITION)|
|vertexInput.positionSS|画面座標|

## ベクターのトランスフォーム (LIL_VERTEX_NORMAL_TANGENT_INPUTS)
|名前|説明|
|-|-|
|LIL_VERTEX_NORMAL_TANGENT_INPUTS(float3 normalOS, float4 tangentOS, out vertexNormalInput)|ベクターのトランスフォーム|
|vertexNormalInput.tangentWS|ワールド空間のtangent|
|vertexNormalInput.bitangentWS|ワールド空間のbitangent|
|vertexNormalInput.normalWS|ワールド空間のnormal|

## その他
|名前|説明|
|-|-|
|float2 lilCalcUV(float2 uv, float4 tex_ST, float4 tex_ScrollRotate)|UVの計算|
|float3 lilBlendColor(float3 dstCol, float3 srcCol, float srcA, uint blendMode)|ブレンドモードによる色の合成 (0: 通常 / 1: 加算 / 2: スクリーン / 3: 乗算)|
|float3 lilUnpackNormalScale(float4 normalTex, float scale)|ノーマルマップの展開|
|float lilTooning()|入力値をトゥーン化|

## ピクセルシェーダー
`lilFragData`という共通の構造体を用いて変数を管理しています。構造体の中身は以下の通りです。

|名前|説明|
|-|-|
|float4 col|最終的に出力される色|
|float3 albedo|マテリアルそのものの色（メインカラー）|
|float3 emissionColor|発光の色|
|float3 lightColor|ライトの色|
|float3 indLightColor|環境光の色（影色に利用）|
|float3 addLightColor|頂点ライトの色|
|float attenuation|ライトの減衰|
|float3 invLighting|ライトの色の逆（蛍光に利用）|
|float2 uv0|TEXCOORD0|
|float2 uv1|TEXCOORD1|
|float2 uv2|TEXCOORD2|
|float2 uv3|TEXCOORD3|
|float2 uvMain|メインカラーの計算済みUV|
|float2 uvMat|MatCapのUV|
|float2 uvRim|リムライトのUV|
|float2 uvPanorama|パノラマUV|
|float2 uvScn|画面座標のUV|
|bool isRightHand|Tangentのwが正の値でtrue、負の値でfalse|
|float3 positionOS|頂点のオブジェクト座標|
|float3 positionWS|頂点のワールド座標|
|float4 positionCS|頂点のクリップ座標|
|float4 positionSS|頂点の画面座標|
|float depth|深度|
|float3x3 TBN|Tangent、Bitangent、Normalの行列|
|float3 T|Tangent|
|float3 B|Bitangent|
|float3 N|Normal|
|float3 V|View Direction|
|float3 L|Light Direction|
|float3 origN|元のNormal|
|float3 origL|元のLight Direction|
|float3 headV|VR用に視差を無効化したView Direction|
|float3 reflectionN|反射用Normal|
|float3 matcapN|MatCap用Normal|
|float3 matcap2ndN|MatCap2nd用Normal|
|float facing|Cull Offのときに裏面かどうか|
|float vl|VdotL|
|float hl|HdotL|
|float ln|LdotN|
|float nv|NdotV|
|float nvabs|abs(NdotV)|
|float4 triMask|lilToonLite用マスク|
|float3 parallaxViewDirection|視差マップ用|
|float2 parallaxOffset|視差マップ用オフセット量|
|float anisotropy|異方性反射の強さ|
|float smoothness|Smoothness|
|float roughness|Roughness|
|float perceptualRoughness|Roughnessの2乗|
|float shadowmix|影の強さ|
|float audioLinkValue|AudioLinkの値|
|uint renderingLayers|URP、HDRP用|
|uint featureFlags|HDRP用|
|uint2 tileIndex|HDRP用|
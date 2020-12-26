// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------------------
// fragment shader
float4 frag(v2f i, fixed facing : VFACE) : SV_TARGET
{
    if(_Invisible)
    {
        //----------------------------------------------------------------------------------------------------------------------
        // 非表示
        discard;
        return 0.0;
    } else {
        //----------------------------------------------------------------------------------------------------------------------
        // 裏面の法線を反転
        i.normalDir = facing < (_FlipNormal-1) ? -i.normalDir : i.normalDir;

        //----------------------------------------------------------------------------------------------------------------------
        // 初期化用
        float4 col = 1.0;
        float4 colbuf = 1.0;
        #ifndef LIL_FOR_ADD
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz + float3(0.0, +0.001, 0.0));
        #else
            float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.worldPos.xyz, _WorldSpaceLightPos0.w));
        #endif
        float3 viewDirection = normalize(UnityWorldSpaceViewDir(i.worldPos.xyz));

        //----------------------------------------------------------------------------------------------------------------------
        // Lighting (光の当たり方と影の強さ)
        // 影の処理は完全に感覚
        float directContribution = 1.0;
        UNITY_BRANCH
        if(_UseShadow)
        {
            // 影
            directContribution = saturate(dot(lightDirection,i.normalDir)*0.5+0.5);

            // トゥーン処理
            float shadowBorder = _ShadowBorder * _ShadowBorder;
            float ShadowBorderMin = saturate(shadowBorder - _ShadowBlur*0.5);
            float ShadowBorderMax = saturate(shadowBorder + _ShadowBlur*0.5);
            directContribution = saturate((directContribution - ShadowBorderMin) / (ShadowBorderMax - ShadowBorderMin));

            // 背面を暗く
            directContribution = directContribution * (1 - (facing < 0) * _BackfaceForceShadow);

            // 影の濃さ (光が弱い場合は薄くする)
            float shadowStrength = GET_MASK(_ShadowStrengthMask) * saturate(lightColorL*1.25+ShadeSH9PlusL-ShadeSH9AveL);
            directContribution = 1.0 - mad(shadowStrength,-directContribution,shadowStrength);
        }

        //----------------------------------------------------------------------------------------------------------------------
        // Main Texture (メインの色)
        // Main
        colbuf = _MainTex.Sample(sampler_MainTex,i.uv*_MainTex_ST.xy+_MainTex_ST.zw);
        colbuf = pow(colbuf,_MainTexTonecurve);
        colbuf *= _Color;
        colbuf.rgb = colorShifter(colbuf.rgb, _MainTexHue, _MainTexSaturation, _MainTexValue);
        col = colbuf;
        // Main2nd
        UNITY_BRANCH
        if(_UseMain2ndTex)
        {
            colbuf = GET_TEXSUB(_Main2ndTex) * _Color2nd;
            col.rgb = mixSubCol(col.rgb, colbuf.rgb, colbuf.a, _Main2ndTexMix);
        }

        //----------------------------------------------------------------------------------------------------------------------
        // Vertex Color (頂点カラー)
        UNITY_BRANCH
        if(_UseVertexColor) col *= i.color;

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha Mask (アルファマスク)
        #if LIL_RENDER != 0
            // Alpha
            UNITY_BRANCH
            if(_UseAlphaMask) col.a *= GET_MASK(_AlphaMask);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Outline Color (輪郭線の色)
        #ifdef LIL_LITE_OUTLINE
            col *= _OutlineColor;
            clip(GET_MASK(_OutlineAlphaMask)-0.5);
        #endif

        // 透過関係
        #if LIL_RENDER == 0
            col.a = 1;
        #elif  LIL_RENDER == 1
            clip(col.a - _Cutoff);
            col.a = 1;
        #else
            clip(col.a - 0.001);
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Light Color (光の色を計算)
        float3 finalLight = 1.0;
        float3 directLighting = saturate(ShadeSH9Plus+lightColor);

        UNITY_BRANCH
        if(_UseShadow)
        {
            float3 indirectLighting = saturate(ShadeSH9Ave);
            indirectLighting = indirectLighting + _ShadowColor - indirectLighting * _ShadowColor;
            finalLight = lerp(indirectLighting,directLighting,directContribution)+i.sh;
        } else {
            finalLight = directLighting+i.sh;
        }
        col.rgb *= finalLight;

        //----------------------------------------------------------------------------------------------------------------------
        // ここから加算系（アウトラインには不要な処理）
        #ifndef LIL_LITE_OUTLINE
            //----------------------------------------------------------------------------------------------------------------------
            // Matcap (マットキャップ)
            float2 matUV = 0.0;
            float2x2 matRot = calcMatRot(i.isMirror);

            // Matcap
            UNITY_BRANCH
            if(_UseMatcap)
            {
                matUV = ComputeTransformCap(i.normalDir);
                matUV = uvRotate(matUV, matRot);
                if(i.isMirror > 0.5) matUV.x = 1-matUV.x;
                colbuf = UNITY_SAMPLE_TEX2D_SAMPLER(_MatcapTex, _linear_repeat, matUV) * _MatcapColor;
                colbuf.rgb *= 1 - _MatcapShadeMix + finalLight * _MatcapShadeMix;
                col.rgb = mixSubCol(col.rgb, colbuf.rgb, GET_MASK(_MatcapBlendMask) * _MatcapBlend * colbuf.a, _MatcapMix);
            }

            //----------------------------------------------------------------------------------------------------------------------
            // Emission (発光)
            UNITY_BRANCH
            if(_UseEmission)
            {
                float2 _EmissionMapParaTex = i.uv + _EmissionParallaxDepth * mul(float3x3(i.tangentDir, i.bitangentDir, i.normalDir), viewDirection).xy;
                colbuf = GET_EMITEX(_EmissionMap) * _EmissionColor;
                UNITY_BRANCH
                if(_EmissionUseGrad) colbuf *= getGradTex(_EmissionGradSpeed, _EmissionGradTex);
                col.rgb = mad(colbuf.rgb, GET_MASK(_EmissionBlendMask) * _EmissionBlend * _EmissionBlink * colbuf.a, col.rgb);
            }
        #endif

        //-------------------------------------------------------------------------------------------------------------------------
        // フォグを適用して書き出し
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
    }
}
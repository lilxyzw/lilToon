// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------------------
// vertex shader
v2f vert(appdata v)
{
    v2f o = (v2f)0;
    #ifdef LIL_LITE_OUTLINE
        if(_Invisible != 1 && _UseOutline)
    #else
        if(_Invisible != 1)
    #endif
    {
        //----------------------------------------------------------------------------------------------------------------------
        // シングルパスインスタンシングレンダリング用
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        //----------------------------------------------------------------------------------------------------------------------
        // 格納
        o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
        #ifndef LIL_LITE_OUTLINE
            o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
        #endif
        o.color = v.color;
        o.uv = v.uv;

        #ifdef LIL_LITE_OUTLINE
            //--------------------------------------------------------------------------------------------------------------
            // 輪郭線用: 法線方向に拡大
            float mask = _OutlineWidth * 0.01;
            if(_vc2w>=0) mask *= o.color[_vc2w];
            o.worldPos.xyz += o.normalDir * mask;
            o.pos = UnityWorldToClipPos(o.worldPos);
        #else
            o.pos = UnityObjectToClipPos(v.vertex);
            o.isMirror = dot(cross(UNITY_MATRIX_V[0], UNITY_MATRIX_V[1]), UNITY_MATRIX_V[2]) < 0 ? 0 : 1;
        #endif
        UNITY_TRANSFER_FOG(o,o.pos);
        TRANSFER_SHADOW(o)

        //----------------------------------------------------------------------------------------------------------------------
        // 頂点ライティング
        #if UNITY_SHOULD_SAMPLE_SH && defined(VERTEXLIGHT_ON)
            // 距離
            float4 toLightX = unity_4LightPosX0 - o.worldPos.x;
            float4 toLightY = unity_4LightPosY0 - o.worldPos.y;
            float4 toLightZ = unity_4LightPosZ0 - o.worldPos.z;
            // squared lengths
            float4 lengthSq = toLightX * toLightX + toLightY * toLightY + toLightZ * toLightZ;
            lengthSq = max(lengthSq, 0.000001);
            // ライト強度: 距離のみで計算
            float4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
            atten = pow(atten , max(0.05 / atten , 1.0)); // なめらか
            // 合成、saturateしてforward addに明るさを合わせる
            o.sh = saturate(unity_LightColor[0].rgb * 0.85 * atten.x) + 
                   saturate(unity_LightColor[1].rgb * 0.85 * atten.y) + 
                   saturate(unity_LightColor[2].rgb * 0.85 * atten.z) + 
                   saturate(unity_LightColor[3].rgb * 0.85 * atten.w);
        #else
            o.sh = 0.0;
        #endif
    }
    return o;
}
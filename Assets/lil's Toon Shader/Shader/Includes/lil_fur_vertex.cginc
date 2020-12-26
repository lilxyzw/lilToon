// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------------------
// vertex shader
v2g vert(appdata v)
{
    v2g o = (v2g)0;
    if(_Invisible != 1)
    {
        //----------------------------------------------------------------------------------------------------------------------
        // シングルパスインスタンシングレンダリング用
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2g, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        //----------------------------------------------------------------------------------------------------------------------
        // 格納
        o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
        o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
        o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
        o.color = v.color;
        o.uv = v.uv;
        o.pos = UnityObjectToClipPos(v.vertex);
        UNITY_TRANSFER_FOG(o,o.pos);

        //----------------------------------------------------------------------------------------------------------------------
        // 頂点ライティング
        #ifndef LIL_FOR_ADD
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
                // ライト方向: ピクセルシェーダーで影を付けるときに利用
                o.shNormal = normalize(float3(toLightX.x,toLightY.x,toLightZ.x)) * lilLuminance(unity_LightColor[0].rgb * atten.x) + 
                             normalize(float3(toLightX.y,toLightY.y,toLightZ.y)) * lilLuminance(unity_LightColor[1].rgb * atten.y) + 
                             normalize(float3(toLightX.z,toLightY.z,toLightZ.z)) * lilLuminance(unity_LightColor[2].rgb * atten.z) + 
                             normalize(float3(toLightX.w,toLightY.w,toLightZ.w)) * lilLuminance(unity_LightColor[3].rgb * atten.w);
                o.shNormal = normalize(o.shNormal);
                // 合成、saturateしてforward addに明るさを合わせる
                o.sh = saturate(unity_LightColor[0].rgb * 0.85 * atten.x) + 
                       saturate(unity_LightColor[1].rgb * 0.85 * atten.y) + 
                       saturate(unity_LightColor[2].rgb * 0.85 * atten.z) + 
                       saturate(unity_LightColor[3].rgb * 0.85 * atten.w);
            #else
                o.sh = 0.0;
            #endif
        #endif
    }
    return o;
}

//------------------------------------------------------------------------------------------------------------------------------
// geometry shader
// フィン法とシェル法を混合……せずにシェル法を視差マップで代用
[maxvertexcount(48)]
void geom(triangle v2g i[3], inout TriangleStream<g2f> outStream)
{
    if(_Invisible != 1)
    {
        g2f o;

        //----------------------------------------------------------------------------------------------------------------------
        // ファー
        // 向き
        float3x3 fur_vector;
        for(int fvi=0;fvi<3;fvi++)
        {
            fur_vector[fvi] = float3(_FurVectorX,_FurVectorY,_FurVectorZ);
            if(_VertexColor2FurVector) fur_vector[fvi] = mixNormal(fur_vector[fvi], i[fvi].color.xyz);
            fur_vector[fvi] = mixNormal(fur_vector[fvi], UnpackScaleNormal(getTexLite_V(_FurVectorTex, i[fvi].uv, sampler_linear_repeat), _FurVectorScale));
            fur_vector[fvi] = normalize(fur_vector[fvi].x * i[fvi].tangentDir + fur_vector[fvi].y * i[fvi].bitangentDir + fur_vector[fvi].z * i[fvi].normalDir);
            fur_vector[fvi].y -= _FurGravity;
        }

        //----------------------------------------------------------------------------------------------------------------------
        // 中間
        float3 fvc = (fur_vector[0]+fur_vector[1]+fur_vector[2])*0.333333333333;
        float3 wpc = (i[0].worldPos+i[1].worldPos+i[2].worldPos)*0.333333333333;
        float3 ndc = (i[0].normalDir+i[1].normalDir+i[2].normalDir)*0.333333333333;
        float2 uvc = (i[0].uv+i[1].uv+i[2].uv)*0.333333333333;
        #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
            float fcc = (i[0].fogCoord+i[1].fogCoord+i[2].fogCoord)*0.333333333333;
        #endif
        #ifndef LIL_FOR_ADD
            float3 shn = (i[0].shNormal+i[1].shNormal+i[2].shNormal)*0.333333333333;
            float3 shc = (i[0].sh+i[1].sh+i[2].sh)*0.333333333333;
        #endif

        //--------------------------------------------------------------------------------------------------------------
        // フィン
        for (int fl = 0; fl < _FurLayerNum; fl++)
        {
            float lpmix = fl*_InvFurLayerNum;
            for(int ii=0;ii<4;ii++)
            {
                int ii2 = ii==3 ? 0 : ii;
                o.worldPos = lerp(i[ii2].worldPos,wpc,lpmix);
                o.normalDir = lerp(i[ii2].normalDir,ndc,lpmix);
                o.uv = lerp(i[ii2].uv,uvc,lpmix);
                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    o.fogCoord = lerp(i[ii2].fogCoord,fcc,lpmix);
                #endif
                #ifndef LIL_FOR_ADD
                    o.shNormal = lerp(i[ii2].shNormal,shn,lpmix);
                    o.sh = lerp(i[ii2].sh,shc,lpmix);
                #endif
                o.pos = UnityWorldToClipPos(o.worldPos);
                o.furLayer = 0;
                outStream.Append(o);

                o.normalDir = lerp(fur_vector[ii2],fvc,lpmix);
                o.worldPos.xyz += o.normalDir * _FurLength;
                o.pos = UnityWorldToClipPos(o.worldPos);
                o.furLayer = 1;
                outStream.Append(o);
            }
            outStream.RestartStrip();
        }

        //--------------------------------------------------------------------------------------------------------------
        // シェル
        /*
        for (uint fl = 1; fl <= _FurLayerNum; fl++)
        {
            for (uint ffi = 0; ffi < 3; ffi++)
            {
                o.worldPos = i[ffi].worldPos;
                o.normalDir = i[ffi].normalDir;
                o.uv = i[ffi].uv;
                //o.pos = i[oi].pos;

                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    o.fogCoord = i[ffi].fogCoord;
                #endif

                #ifndef LIL_FOR_ADD
                    o.shNormal = i[ffi].shNormal;
                    o.sh = i[ffi].sh;
                #endif

                // ファー用: 法線方向に拡大
                o.furLayer = fl*_InvFurLayerNum;
                o.isShell = 1;
                o.worldPos.xyz += fur_vector[ffi] * _FurLength * o.furLayer;
                o.pos = UnityWorldToClipPos(o.worldPos);

                outStream.Append(o);
            }
            outStream.RestartStrip();
        }*/
    }
}
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
        #ifdef LIL_FULL
            o.uv0.xy = v.uv0;
            o.uv0.zw = v.uv1;
            o.uv1.xy = v.uv2;
            o.uv1.zw = v.uv3;
        #else
            o.uv = v.uv;
        #endif
        o.pos = UnityObjectToClipPos(v.vertex);
        #if !defined(LIL_FOR_ADD)
            o.isMirror = unity_CameraProjection._m20 != 0.0 || unity_CameraProjection._m21 != 0.0;
        #endif
        #ifdef LIL_REFRACTION
            o.projPos = ComputeGrabScreenPos(o.pos);
        #endif
        UNITY_TRANSFER_FOG(o,o.pos);
        TRANSFER_SHADOW(o)

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
                // 合成、影を付けない分の補正をしつつsaturateしてforward addに明るさを合わせる
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
[maxvertexcount(9)]
void geom(triangle v2g i[3], inout TriangleStream<g2f> outStream)
{
    if(_Invisible != 1)
    {
        g2f o;
        //----------------------------------------------------------------------------------------------------------------------
        // 輪郭線
        if (_UseOutline == 1)
        {
            for (int oi = 2; oi >= 0; oi--)
            {
                o.worldPos = i[oi].worldPos;
                o.normalDir = i[oi].normalDir;
                o.tangentDir = i[oi].tangentDir;
                o.bitangentDir = i[oi].bitangentDir;
                o.color = i[oi].color;
                #ifdef LIL_FULL
                    o.uv0 = i[oi].uv0;
                    o.uv1 = i[oi].uv1;
                #else
                    o.uv = i[oi].uv;
                #endif
                //o.pos = i[oi].pos;
                #ifndef LIL_FOR_ADD
                    o.isMirror = i[oi].isMirror;
                #endif
                o.isOutline = 1.0;
                o.facing = 1.0;
                o._ShadowCoord = i[oi]._ShadowCoord;

                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    o.fogCoord = i[oi].fogCoord;
                #endif

                #ifndef LIL_FOR_ADD
                    o.shNormal = i[oi].shNormal;
                    o.sh = i[oi].sh;
                #endif

                #ifdef LIL_REFRACTION
                    o.projPos = i[oi].projPos;
                #endif

                //--------------------------------------------------------------------------------------------------------------
                // 輪郭線用: 法線方向に拡大
                float3 viewDirection = normalize(UnityWorldSpaceViewDir(o.worldPos.xyz));
                #ifdef LIL_FULL
                    float2 uvs[5];
                    {
                        uvs[0] = o.uv0.xy;
                        uvs[1] = o.uv0.zw;
                        uvs[2] = o.uv1.xy;
                        uvs[3] = o.uv1.zw;
                        //uvs[4] = float2(0.5+lilAtan2(viewDirection.x,viewDirection.z)/UNITY_PI, 0.5-lilAsin(viewDirection.y)/UNITY_PI);
                        uvs[4] = o.uv0.xy; //普通使わないので省略
                    }
                #endif
                float mask = GET_MASK_V(_OutlineWidthMask) * _OutlineWidth * 0.01;
                if(_vc2w>=0) mask *= o.color[_vc2w];
                o.worldPos.xyz += o.normalDir * mask;
                o.pos = UnityWorldToClipPos(o.worldPos);

                outStream.Append(o);
            }

            outStream.RestartStrip();
        }

        //----------------------------------------------------------------------------------------------------------------------
        // 背面
        if (_CullMode <= 1)
        {
            for (int bi = 2; bi >= 0; bi--)
            {
                o.worldPos = i[bi].worldPos;
                o.normalDir = i[bi].normalDir;
                o.tangentDir = i[bi].tangentDir;
                o.bitangentDir = i[bi].bitangentDir;
                o.color = i[bi].color;
                #ifdef LIL_FULL
                    o.uv0 = i[bi].uv0;
                    o.uv1 = i[bi].uv1;
                #else
                    o.uv = i[bi].uv;
                #endif
                o.pos = i[bi].pos;
                #ifndef LIL_FOR_ADD
                    o.isMirror = i[bi].isMirror;
                #endif
                o.isOutline = 0.0;
                o.facing = -1.0;
                o._ShadowCoord = i[bi]._ShadowCoord;

                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    o.fogCoord = i[bi].fogCoord;
                #endif

                #ifndef LIL_FOR_ADD
                    o.shNormal = i[bi].shNormal;
                    o.sh = i[bi].sh;
                #endif

                #ifdef LIL_REFRACTION
                    o.projPos = i[bi].projPos;
                #endif

                    outStream.Append(o);
                }

            outStream.RestartStrip();
        }

        //----------------------------------------------------------------------------------------------------------------------
        // 前面
        if(_CullMode != 1)
        {
            for (int fi = 0; fi < 3; fi++)
            {
                o.worldPos = i[fi].worldPos;
                o.normalDir = i[fi].normalDir;
                o.tangentDir = i[fi].tangentDir;
                o.bitangentDir = i[fi].bitangentDir;
                o.color = i[fi].color;
                #ifdef LIL_FULL
                    o.uv0 = i[fi].uv0;
                    o.uv1 = i[fi].uv1;
                #else
                    o.uv = i[fi].uv;
                #endif
                o.pos = i[fi].pos;
                #ifndef LIL_FOR_ADD
                    o.isMirror = i[fi].isMirror;
                #endif
                o.isOutline = 0.0;
                o.facing = 1.0;
                o._ShadowCoord = i[fi]._ShadowCoord;

                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    o.fogCoord = i[fi].fogCoord;
                #endif

                #ifndef LIL_FOR_ADD
                    o.shNormal = i[fi].shNormal;
                    o.sh = i[fi].sh;
                #endif

                #ifdef LIL_REFRACTION
                    o.projPos = i[fi].projPos;
                #endif

                outStream.Append(o);
            }
            /*
            // 単純な裏面描画ならこれでいいが裏表の判定ができない、強引にやろうとしても汚くなる
            if (_CullMode <= 1)
            {
                o.worldPos = i[0].worldPos;
                o.normalDir = i[0].normalDir;
                o.tangentDir = i[0].tangentDir;
                o.bitangentDir = i[0].bitangentDir;
                o.color = i[0].color;
                #ifdef LIL_FULL
                    o.uv0 = i[0].uv0;
                    o.uv1 = i[0].uv1;
                #else
                    o.uv = i[0].uv;
                #endif
                o.pos = i[0].pos;
                #ifndef LIL_FOR_ADD
                    o.isMirror = i[0].isMirror;
                #endif
                o.isOutline = 0.0;
                o.facing = 1.0;
                o._ShadowCoord = i[0]._ShadowCoord;

                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    o.fogCoord = i[0].fogCoord;
                #endif

                #ifndef LIL_FOR_ADD
                    o.shNormal = i[0].shNormal;
                    o.sh = i[0].sh;
                #endif

                #ifdef LIL_REFRACTION
                    o.projPos = i[0].projPos;
                #endif

                outStream.Append(o);
            }
            */
        }

        outStream.RestartStrip();
    }
}
// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

//------------------------------------------------------------------------------------------------------------------
// Shadow Caster
struct VertexInput {
    float4 vertex   : POSITION;
    #if LIL_RENDER > 0
        #ifdef LIL_FULL
            float2 uv0      : TEXCOORD0;
            float2 uv1      : TEXCOORD1;
            float2 uv2      : TEXCOORD2;
            float2 uv3      : TEXCOORD3;
        #else
            float2 uv       : TEXCOORD0;
        #endif
    #endif
};

struct VertexOutput {
    V2F_SHADOW_CASTER;
    #if LIL_RENDER > 0
        #ifdef LIL_FULL
            float4 uv0      : TEXCOORD0;
            float4 uv1      : TEXCOORD1;
        #else
            float2 uv       : TEXCOORD0;
        #endif
    #endif
};

VertexOutput vert (VertexInput v) {
    VertexOutput o = (VertexOutput)0;
    if(_Invisible != 1)
    {
        #if LIL_RENDER > 0
            #ifdef LIL_FULL
                o.uv0.xy = v.uv0;
                o.uv0.zw = v.uv1;
                o.uv1.xy = v.uv2;
                o.uv1.zw = v.uv3;
            #else
                o.uv = v.uv;
            #endif
        #endif
        o.pos = UnityObjectToClipPos( v.vertex );
        TRANSFER_SHADOW_CASTER(o)
    }
    return o;
}

float4 frag(VertexOutput i) : COLOR {
    if(_Invisible == 1)
    {
        discard;
        return float4(0,0,0,0);
    } else {
        #if LIL_RENDER > 0
            float alpha = 1;
            #ifdef LIL_FULL
                float2 uvs[6];
                {
                    uvs[0] = i.uv0.xy;
                    uvs[1] = i.uv0.zw;
                    uvs[2] = i.uv1.xy;
                    uvs[3] = i.uv1.zw;
                    uvs[4] = i.uv0.xy;
                }
                alpha = GET_TEX_SAMP(_MainTex).a * _Color.a;
                if(_UseAlphaMask)
                {
                    float mask = GET_MASK(_AlphaMask) * _Alpha;
                    alpha = mad(mad(_AlphaMaskMixMain, -alpha, _AlphaMaskMixMain), -mask, mask);
                }
            #else
                alpha = GET_TEX_SAMP(_MainTex).a * _Color.a;
                if(_UseAlphaMask) alpha *= GET_MASK(_AlphaMask);
            #endif
            
            #if LIL_RENDER == 1
                clip(alpha - _Cutoff);
            #else
                clip(alpha - 0.5);
            #endif
        #endif
        SHADOW_CASTER_FRAGMENT(i)
    }
}
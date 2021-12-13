Shader "Hidden/ltspass_dummy"
{
//----------------------------------------------------------------------------------------------------------------------
// BRP Start
//
    SubShader
    {
        // ForwardAdd
        Pass
        {
            Name "FORWARD_ADD"
            Tags {"LightMode" = "ForwardAdd"}

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            float4 vert() : SV_POSITION {return float4(0.0,0.0,0.0,0.0);}
            float4 frag() : SV_Target {return float4(0.0,0.0,0.0,0.0);}

            ENDHLSL
        }
    }
//
// BRP End
}

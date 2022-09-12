//------------------------------------------------------------------------------------------------------------------------------
// GTAvaCrypt (https://github.com/rygo6/GTAvaCrypt/blob/master/LICENSE)
#if defined(LIL_MODIFY_PREVPOS)
    #define LIL_MODIFY_TARGET input.previousPositionOS
#else
    #define LIL_MODIFY_TARGET input.positionOS
#endif

#if !defined(LIL_LITE) && !defined(LIL_BAKER) && defined(LIL_FEATURE_ENCRYPTION)
    #if defined(GTMODELDECODE)
        if(!_IgnoreEncryption) LIL_MODIFY_TARGET.xyz = modelDecode(float4(LIL_MODIFY_TARGET.xyz,1), input.normalOS, input.uv6.xy, input.uv7.xy).xyz;
    #else
        if(!_IgnoreEncryption)
        {
            float4 keys = floor(_Keys + 0.5);
            keys = keys.x == 0 ? float4(0,0,0,0) : floor(keys / 3) * 3 + 1;

            keys.x *= 1;
            keys.y *= 2;
            keys.z *= 3;
            keys.w *= 4;

            LIL_MODIFY_TARGET.xyz -= input.normalOS * input.uv6.x * (sin((keys.z - keys.y) * 2) * cos(keys.w - keys.x));
            LIL_MODIFY_TARGET.xyz -= input.normalOS * input.uv6.y * (sin((keys.w - keys.x) * 3) * cos(keys.z - keys.y));
            LIL_MODIFY_TARGET.xyz -= input.normalOS * input.uv7.x * (sin((keys.x - keys.w) * 4) * cos(keys.y - keys.z));
            LIL_MODIFY_TARGET.xyz -= input.normalOS * input.uv7.y * (sin((keys.y - keys.z) * 5) * cos(keys.x - keys.w));
        }
    #endif
#endif

#undef LIL_MODIFY_TARGET
#ifndef LIL_COMMON_INCLUDED
#define LIL_COMMON_INCLUDED

#if !defined(LIL_CUSTOM_SHADER) && !defined(LIL_LITE) && !defined(LIL_MULTI) && !defined(LIL_IGNORE_SHADERSETTING)
#include "../../../lilToonSetting/lil_setting.hlsl"
#endif
#include "Includes/lil_common_macro.hlsl"
#include "Includes/lil_common_input.hlsl"

#if defined(LIL_MULTI)
    #define _UseMain2ndTex true
    #define _UseMain3rdTex true
    #define _UseShadow true
    #define _UseBacklight true
    #define _UseBumpMap true
    #define _UseBump2ndMap true
    #define _UseReflection true
    #define _UseMatCap true
    #define _UseMatCap2nd true
    #define _UseRim true
    #define _UseGlitter true
    #define _UseEmission true
    #define _UseEmission2nd true
    #define _UseParallax true
    #define _UseAudioLink true
    #define _AudioLinkAsLocal true
    #undef LIL_BRANCH
    #define LIL_BRANCH
#endif

#include "Includes/lil_common_functions.hlsl"

#endif
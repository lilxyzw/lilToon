#ifndef LIL_PASS_FORWARD_INCLUDED
#define LIL_PASS_FORWARD_INCLUDED

#if defined(LIL_LITE)
    #include "lil_pass_forward_lite.hlsl"
#else
    #include "lil_pass_forward_normal.hlsl"
#endif

#endif
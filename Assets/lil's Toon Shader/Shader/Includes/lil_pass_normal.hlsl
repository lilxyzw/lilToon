// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

#ifndef LIL_PASS_INCLUDED
#define LIL_PASS_INCLUDED

//------------------------------------------------------------------------------------------------------------------
// Pass for normal version (ForwardBase, ForwardAdd, Outline)

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_normal_struct.hlsl"
#include "Includes/lil_normal_vertex.hlsl"
#include "Includes/lil_normal_fragment.hlsl"
#if defined(LIL_TESSELLATION)
    #include "Includes/lil_tessellation.hlsl"
#endif

#endif
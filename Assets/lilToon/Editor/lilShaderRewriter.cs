#if UNITY_EDITOR
using System.IO;
using lilToon.lilRenderPipelineReader;
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class lilShaderRewriter
    {
        public static void RewriteReceiveShadow(string path, bool enable)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(enable)
            {
                s = s.Replace(
                    "        #pragma skip_variants SHADOWS_SCREEN _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN _ADDITIONAL_LIGHT_SHADOWS SCREEN_SPACE_SHADOWS_ON SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH",
                    "        //#pragma skip_variants SHADOWS_SCREEN _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN _ADDITIONAL_LIGHT_SHADOWS SCREEN_SPACE_SHADOWS_ON SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH");
            }
            else
            {
                s = s.Replace(
                    "        //#pragma skip_variants SHADOWS_SCREEN _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN _ADDITIONAL_LIGHT_SHADOWS SCREEN_SPACE_SHADOWS_ON SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH",
                    "        #pragma skip_variants SHADOWS_SCREEN _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN _ADDITIONAL_LIGHT_SHADOWS SCREEN_SPACE_SHADOWS_ON SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH");
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteForwardAdd(string path, bool enable)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(enable)
            {
                s = s.Replace(
                    "        // ForwardAdd Start\r\n        /*",
                    "        // ForwardAdd Start\r\n        //");
                s = s.Replace(
                    "        */\r\n        // ForwardAdd End",
                    "        //\r\n        // ForwardAdd End");
            }
            else
            {
                s = s.Replace(
                    "        // ForwardAdd Start\r\n        //",
                    "        // ForwardAdd Start\r\n        /*");
                s = s.Replace(
                    "        //\r\n        // ForwardAdd End",
                    "        */\r\n        // ForwardAdd End");
            }

            StreamWriter sw = new StreamWriter(path,false);
            string[] lines = s.Split('\n');
            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                line = line.Replace("\r", "");

                if(line.Contains("UsePass") && line.Contains("FORWARD_ADD"))
                {
                    if(enable)
                    {
                        line = line.Replace(
                            "        //UsePass",
                            "        UsePass");
                    }
                    else
                    {
                        line = line.Replace(
                            "        UsePass",
                            "        //UsePass");
                    }
                }
                if(i != lines.Length - 1) sw.WriteLine(line);
                else                      sw.Write(line);
                
            }
            sw.Close();
        }

        public static void RewriteVertexLight(string path, bool enable)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(enable)
            {
                s = s.Replace(
                    "        #pragma skip_variants VERTEXLIGHT_ON _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                    "        //#pragma skip_variants VERTEXLIGHT_ON _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS");
            }
            else
            {
                s = s.Replace(
                    "        //#pragma skip_variants VERTEXLIGHT_ON _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                    "        #pragma skip_variants VERTEXLIGHT_ON _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS");
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteLightmap(string path, bool enable)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(enable)
            {
                s = s.Replace(
                    "        #pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK DIRLIGHTMAP_COMBINED _MIXED_LIGHTING_SUBTRACTIVE",
                    "        //#pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK DIRLIGHTMAP_COMBINED _MIXED_LIGHTING_SUBTRACTIVE");
            }
            else
            {
                s = s.Replace(
                    "        //#pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK DIRLIGHTMAP_COMBINED _MIXED_LIGHTING_SUBTRACTIVE",
                    "        #pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK DIRLIGHTMAP_COMBINED _MIXED_LIGHTING_SUBTRACTIVE");
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteURPPass(string path, int version)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(version >= 12)
            {
                s = s.Replace(
                    "            //URP12U\r\n            //#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING\r\n            //#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION\r\n            //#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3\r\n            //#pragma multi_compile _ _LIGHT_LAYERS\r\n            //#pragma multi_compile_fragment _ _LIGHT_COOKIES\r\n            //#pragma multi_compile _ _CLUSTERED_RENDERING\r\n            //#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                    "            //URP12U\r\n            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING\r\n            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION\r\n            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3\r\n            #pragma multi_compile _ _LIGHT_LAYERS\r\n            #pragma multi_compile_fragment _ _LIGHT_COOKIES\r\n            #pragma multi_compile _ _CLUSTERED_RENDERING\r\n            #pragma multi_compile _ DYNAMICLIGHTMAP_ON");
            }
            else
            {
                s = s.Replace(
                    "            //URP12U\r\n            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING\r\n            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION\r\n            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3\r\n            #pragma multi_compile _ _LIGHT_LAYERS\r\n            #pragma multi_compile_fragment _ _LIGHT_COOKIES\r\n            #pragma multi_compile _ _CLUSTERED_RENDERING\r\n            #pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                    "            //URP12U\r\n            //#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING\r\n            //#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION\r\n            //#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3\r\n            //#pragma multi_compile _ _LIGHT_LAYERS\r\n            //#pragma multi_compile_fragment _ _LIGHT_COOKIES\r\n            //#pragma multi_compile _ _CLUSTERED_RENDERING\r\n            //#pragma multi_compile _ DYNAMICLIGHTMAP_ON");
            }
            if(version >= 11)
            {
                s = s.Replace(
                    "            //URP11U\r\n            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN",
                    "            //URP11U\r\n            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN");
                s = s.Replace(
                    "            //#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW",
                    "            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW");
            }
            else
            {
                s = s.Replace(
                    "            //URP11U\r\n            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN",
                    "            //URP11U\r\n            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN");
                s = s.Replace(
                    "            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW",
                    "            //#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW");
            }
            if(version >= 10)
            {
                s = s.Replace(
                    "            //URP10U\r\n            //#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION\r\n            //#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING\r\n            //#pragma multi_compile _ SHADOWS_SHADOWMASK",
                    "            //URP10U\r\n            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION\r\n            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING\r\n            #pragma multi_compile _ SHADOWS_SHADOWMASK");
            }
            else
            {
                s = s.Replace(
                    "            //URP10U\r\n            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION\r\n            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING\r\n            #pragma multi_compile _ SHADOWS_SHADOWMASK",
                    "            //URP10U\r\n            //#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION\r\n            //#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING\r\n            //#pragma multi_compile _ SHADOWS_SHADOWMASK");
            }
            if(version <= 10)
            {
                s = s.Replace(
                    "            //URP10D\r\n            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE",
                    "            //URP10D\r\n            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE");
            }
            else
            {
                s = s.Replace(
                    "            //URP10D\r\n            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE",
                    "            //URP10D\r\n            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE");
            }
            if(version <= 9)
            {
                s = s.Replace(
                    "            //URP9D\r\n            //#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE",
                    "            //URP9D\r\n            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE");
            }
            else
            {
                s = s.Replace(
                    "            //URP9D\r\n            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE",
                    "            //URP9D\r\n            //#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE");
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteHDRPPass(string path, int version)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(version >= 12)
            {
                s = s.Replace(
                    "//#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2",
                    "#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2");
                s = s.Replace(
                    "//#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT",
                    "#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT");
            }
            else
            {
                s = s.Replace(
                    "#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2",
                    "//#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2");
                s = s.Replace(
                    "#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT",
                    "//#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT");
            }
            if(version >= 10)
            {
                s = s.Replace(
                    "//#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON",
                    "#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON");
                s = s.Replace(
                    "//#pragma multi_compile _ WRITE_DECAL_BUFFER",
                    "#pragma multi_compile _ WRITE_DECAL_BUFFER");
            }
            else
            {
                s = s.Replace(
                    "#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON",
                    "//#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON");
                s = s.Replace(
                    "#pragma multi_compile _ WRITE_DECAL_BUFFER",
                    "//#pragma multi_compile _ WRITE_DECAL_BUFFER");
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteRPPass(string path, PackageVersionInfos version)
        {
            switch(version.RP)
            {
                case lilRenderPipeline.URP: RewriteURPPass(path, version.Major); break;
                case lilRenderPipeline.HDRP: RewriteHDRPPass(path, version.Major); break;
                default: break;
            }
        }

        public static void RewriteReceiveShadow(Shader shader, bool enable)
        {
            string path = AssetDatabase.GetAssetPath(shader);
            RewriteReceiveShadow(path, enable);
        }
    }
}
#endif
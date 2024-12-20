#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace lilToon
{
    public class lilOptimizer
    {
        private const int TYPE_OFFSET = 8;

        internal static void OptimizeInputHLSL(Material[] materials, AnimationClip[] clips)
        {
            try
            {
                var dicT = new Dictionary<string, TexProp>();
                var dicD = new Dictionary<string, STProp>();
                var dicF = new Dictionary<string, FloatProp>();
                var dicC = new Dictionary<string, ColorProp>();
                CheckMaterials(materials, dicT, dicD, dicF, dicC);
                CheckAnimationClips(clips, dicT, dicD, dicF, dicC);
                RewriteInputHLSL(dicT, dicD, dicF, dicC);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                Debug.Log("[lilToon] OptimizeInputHLSL() failed");
            }
        }

        internal static string GetOptimizedText(Material[] materials, AnimationClip[] clips)
        {
            try
            {
                var dicT = new Dictionary<string, TexProp>();
                var dicD = new Dictionary<string, STProp>();
                var dicF = new Dictionary<string, FloatProp>();
                var dicC = new Dictionary<string, ColorProp>();
                CheckMaterials(materials, dicT, dicD, dicF, dicC);
                CheckAnimationClips(clips, dicT, dicD, dicF, dicC);
                return RewriteInputHLSLText(dicT, dicD, dicF, dicC);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                Debug.Log("[lilToon] OptimizeInputHLSL() failed");
                return e.ToString();
            }
        }

        private static void CheckMaterials(IEnumerable<Material> materials, Dictionary<string, TexProp> dicT, Dictionary<string, STProp> dicD, Dictionary<string, FloatProp> dicF, Dictionary<string, ColorProp> dicC)
        {
            foreach(var m in materials) CheckMaterial(m, dicT, dicD, dicF, dicC);
        }

        private static void CheckMaterial(Material material, Dictionary<string, TexProp> dicT, Dictionary<string, STProp> dicD, Dictionary<string, FloatProp> dicF, Dictionary<string, ColorProp> dicC)
        {
            if(material == null || !CheckShaderIslilToon(material.shader)) return;
            var so = new SerializedObject(material);
            var savedProps = so.FindProperty("m_SavedProperties");

            var texs = savedProps.FindPropertyRelative("m_TexEnvs");
            Check(dicT, dicD, texs, material);

            var floats = savedProps.FindPropertyRelative("m_Floats");
            Check(dicF, floats, material);

            var colors = savedProps.FindPropertyRelative("m_Colors");
            Check(dicC, colors, material);
        }

        private static void CheckAnimationClips(IEnumerable<AnimationClip> clips, Dictionary<string, TexProp> dicT, Dictionary<string, STProp> dicD, Dictionary<string, FloatProp> dicF, Dictionary<string, ColorProp> dicC)
        {
            foreach(var c in clips) CheckAnimationClip(c, dicT, dicD, dicF, dicC);
        }

        private static void CheckAnimationClip(AnimationClip clip, Dictionary<string, TexProp> dicT, Dictionary<string, STProp> dicD, Dictionary<string, FloatProp> dicF, Dictionary<string, ColorProp> dicC)
        {
            if(clip == null) return;
            CheckMaterials(AnimationUtility.GetObjectReferenceCurveBindings(clip).SelectMany(b => AnimationUtility.GetObjectReferenceCurve(clip, b)).Select(f => f.value as Material).Distinct(), dicT, dicD, dicF, dicC);

            foreach(var propname in AnimationUtility.GetCurveBindings(clip).Select(b => b.propertyName).Where(n => !string.IsNullOrEmpty(n) && n.Contains("material.")))
            {
                if(propname.Contains("_ST."))
                {
                    string name = propname.Substring(9, propname.Length - 14);
                    dicD[name] = new STProp(){isVariable = true};
                }
                else if(propname.EndsWith(".r") || propname.EndsWith(".g") || propname.EndsWith(".b") || propname.EndsWith(".a") || propname.EndsWith(".x") || propname.EndsWith(".y") || propname.EndsWith(".z") || propname.EndsWith(".w"))
                {
                    string name = propname.Substring(9, propname.Length - 11);
                    dicC[name] = new ColorProp(){isVariable = true};
                }
                else
                {
                    string name = propname.Substring(9, propname.Length - 9);
                    dicF[name] = new FloatProp(){isVariable = true};
                }
            }
        }

        private static void Check(Dictionary<string, TexProp> dic, Dictionary<string, STProp> dicD, SerializedProperty props, Material material)
        {
            for(int i = 0; i < props.arraySize; i++)
            {
                var prop = props.GetArrayElementAtIndex(i);
                string name = prop.FindPropertyRelative("first").stringValue;
                if(!material.HasProperty(name)) continue;
                var prop2 = prop.FindPropertyRelative("second");
                Object tex = prop2.FindPropertyRelative("m_Texture").objectReferenceValue;
                Vector2 scale = prop2.FindPropertyRelative("m_Scale").vector2Value;
                Vector2 offset = prop2.FindPropertyRelative("m_Offset").vector2Value;

                if(dic.ContainsKey(name))
                {
                    if(!dic[name].isVariable && dic[name].t != tex) dic[name] = new TexProp(){isVariable = true};
                }
                else
                {
                    dic[name] = new TexProp(){
                        isVariable = false,
                        t = tex
                    };
                }

                if(dicD.ContainsKey(name))
                {
                    if(!dicD[name].isVariable)
                    {
                        var v = dicD[name];
                        if(v.s != scale || v.o != offset) dicD[name] = new STProp(){isVariable = true};
                    }
                }
                else
                {
                    dicD[name] = new STProp(){
                        isVariable = false,
                        s = scale,
                        o = offset
                    };
                }
            }
        }

        private static void Check(Dictionary<string, FloatProp> dic, SerializedProperty props, Material material)
        {
            for(int i = 0; i < props.arraySize; i++)
            {
                var prop = props.GetArrayElementAtIndex(i);
                string name = prop.FindPropertyRelative("first").stringValue;
                if(!material.HasProperty(name) || dic.ContainsKey(name) && dic[name].isVariable) continue;
                float fl = prop.FindPropertyRelative("second").floatValue;
                if(dic.ContainsKey(name))
                {
                    if(dic[name].f != fl) dic[name] = new FloatProp(){isVariable = true};
                    continue;
                }
                dic[name] = new FloatProp(){
                    isVariable = false,
                    f = fl
                };
            }
        }

        private static void Check(Dictionary<string, ColorProp> dic, SerializedProperty props, Material material)
        {
            for(int i = 0; i < props.arraySize; i++)
            {
                var prop = props.GetArrayElementAtIndex(i);
                string name = prop.FindPropertyRelative("first").stringValue;
                if(!material.HasProperty(name) || dic.ContainsKey(name) && dic[name].isVariable) continue;
                Color color = prop.FindPropertyRelative("second").colorValue;
                if(dic.ContainsKey(name))
                {
                    if(dic[name].c != color) dic[name] = new ColorProp(){isVariable = true};
                    continue;
                }
                dic[name] = new ColorProp(){
                    isVariable = false,
                    c = color
                };
            }
        }

        private static void RewriteInputHLSL(Dictionary<string, TexProp> dicT, Dictionary<string, STProp> dicD, Dictionary<string, FloatProp> dicF, Dictionary<string, ColorProp> dicC)
        {
            string optHLSL = RewriteInputHLSLText(dicT, dicD, dicF, dicC);
            string pathOpt = AssetDatabase.GUIDToAssetPath("571051a232e4af44a98389bda858df27");
            var sw = new StreamWriter(pathOpt, false);
            sw.Write(optHLSL);
            sw.Close();
        }

        private static string RewriteInputHLSLText(Dictionary<string, TexProp> dicT, Dictionary<string, STProp> dicD, Dictionary<string, FloatProp> dicF, Dictionary<string, ColorProp> dicC)
        {
            if(dicT.Count == 0 && dicD.Count == 0 && dicF.Count == 0 && dicC.Count == 0) return null;
            string pathBase = AssetDatabase.GUIDToAssetPath("8ff7f7d9c86e1154fb3aac5a8a8681bb");
            string pathOpt = AssetDatabase.GUIDToAssetPath("571051a232e4af44a98389bda858df27");
            if(string.IsNullOrEmpty(pathBase) || string.IsNullOrEmpty(pathOpt) || !File.Exists(pathBase) || !File.Exists(pathOpt)) return null;
            var shader = Shader.Find("Hidden/ltspass_proponly");
            var sb = new StringBuilder();
            var sr = new StreamReader(pathBase);
            string line;
            while((line = sr.ReadLine()) != null)
            {
                int indEND = line.IndexOf(";");
                if(indEND <= 0)
                {
                    sb.AppendLine(line);
                    continue;
                }

                int indF4 = line.IndexOf("float4  ");
                int indST = line.IndexOf("_ST;");
                int indF = line.IndexOf("float   ");
                int indI = line.IndexOf("uint    ");
                int indB = line.IndexOf("lilBool ");
                if(indF4 >= 0)
                {
                    indF4 += TYPE_OFFSET;
                    string name = line.Substring(indF4, indEND - indF4);
                    if(indST >= 0)
                    {
                        // Texture
                        string texname = name.Substring(0,name.Length - 3);
                        if(dicD.ContainsKey(texname) && !dicD[texname].isVariable)
                        {
                            var v = dicD[texname];
                            sb.AppendLine(GetIndent(indF4 - 8) + "#define " + name + " float4(" + LilF2S(v.s.x) + "," + LilF2S(v.s.y) + "," + LilF2S(v.o.x) + "," + LilF2S(v.o.y) + ")");
                            continue;
                        }
                    }
                    else if(dicC.ContainsKey(name) && !dicC[name].isVariable)
                    {
                        var v = dicC[name];
                        Color c = ShouldLinear(shader, name) ? v.c.linear : v.c;
                        sb.AppendLine(GetIndent(indF4 - 8) + "#define " + name + " float4(" + LilF2S(c.r) + "," + LilF2S(c.g) + "," + LilF2S(c.b) + "," + LilF2S(c.a) + ")");

                        if(name == "_Color" && (v.c.r + v.c.g + v.c.b <= 0.001f)) sb.AppendLine(GetIndent(indF4 - 8) + "#define sampler_MainTex lil_sampler_linear_repeat_");
                        continue;
                    }
                }
                else if(indF >= 0)
                {
                    // Float
                    indF += TYPE_OFFSET;
                    string name = line.Substring(indF, indEND - indF);
                    if(dicF.ContainsKey(name) && !dicF[name].isVariable)
                    {
                        float f = dicF[name].f;
                        f = ShouldLinear(shader, name) ? Mathf.GammaToLinearSpace(f) : f;
                        sb.AppendLine(GetIndent(indF - 8) + "#define " + name + " (" + LilF2S(f) + ")");
                        continue;
                    }
                }
                else if(indI >= 0)
                {
                    // Int
                    indI += TYPE_OFFSET;
                    string name = line.Substring(indI, indEND - indI);
                    if(dicF.ContainsKey(name) && !dicF[name].isVariable && name != "_AlphaMaskMode")
                    {
                        sb.AppendLine(GetIndent(indI - 8) + "#define " + name + " (" + (uint)dicF[name].f + ")");
                        continue;
                    }
                }
                else if(indB >= 0)
                {
                    // Bool
                    indB += TYPE_OFFSET;
                    string name = line.Substring(indB, indEND - indB);
                    if(dicF.ContainsKey(name) && !dicF[name].isVariable)
                    {
                        sb.AppendLine(GetIndent(indB - 8) + "#define " + name + " (" + (uint)dicF[name].f + ")");
                        continue;
                    }
                }
                sb.AppendLine(line);
            }
            sr.Close();

            sb.Replace("\r\n", "\r");
            sb.Replace("\n", "\r");
            sb.Replace("\r", "\r\n");
            return sb.ToString();
        }

        private static bool ShouldLinear(Shader shader, string name)
        {
            if(PlayerSettings.colorSpace != ColorSpace.Linear) return false;
            #if UNITY_2019_3_OR_NEWER
                int id = shader.FindPropertyIndex(name);
                if(id == -1) return false;
                var flag = shader.GetPropertyFlags(id);
                var type = shader.GetPropertyType(id);
                if(type == UnityEngine.Rendering.ShaderPropertyType.Color)
                {
                    return flag != UnityEngine.Rendering.ShaderPropertyFlags.HDR;
                }
                return flag == UnityEngine.Rendering.ShaderPropertyFlags.Gamma;
            #else
                //int id = shader.PropertyToID(name);
                return false;
            #endif
        }

        internal static void ResetInputHLSL()
        {
            string pathBase = AssetDatabase.GUIDToAssetPath("8ff7f7d9c86e1154fb3aac5a8a8681bb");
            string pathOpt = AssetDatabase.GUIDToAssetPath("571051a232e4af44a98389bda858df27");
            if(string.IsNullOrEmpty(pathBase) || string.IsNullOrEmpty(pathOpt) || !File.Exists(pathBase) || !File.Exists(pathOpt)) return;
            var sw = new StreamWriter(pathOpt, false);
            var sr = new StreamReader(pathBase);
            sw.Write(sr.ReadToEnd());
            sw.Close();
            sr.Close();
        }

        private static string GetIndent(int indent)
        {
            return new string(' ', indent);
        }

        private static bool CheckShaderIslilToon(Shader shader)
        {
            if(shader == null) return false;
            if(shader.name.Contains("lilToon")) return true;
            string shaderPath = AssetDatabase.GetAssetPath(shader);
            return !string.IsNullOrEmpty(shaderPath) && shaderPath.Contains(".lilcontainer");
        }

        private static string LilF2S(float f){ return f.ToString(CultureInfo.InvariantCulture); }

        private struct TexProp
        {
            public bool isVariable;
            public Object t;
        }

        private struct STProp
        {
            public bool isVariable;
            public Vector2 s;
            public Vector2 o;
        }

        private struct FloatProp
        {
            public bool isVariable;
            public float f;
        }

        private struct ColorProp
        {
            public bool isVariable;
            public Color c;
        }
    }
}
#endif
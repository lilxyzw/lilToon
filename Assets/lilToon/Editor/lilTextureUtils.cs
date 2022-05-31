#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class lilTextureUtils
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Gradient
        #region
        public static void GradientEditor(Material material, Gradient ingrad, MaterialProperty texprop, bool setLinear = false)
        {
            #if UNITY_2018_3_OR_NEWER
                ingrad = EditorGUILayout.GradientField(lilToonInspector.GetLoc("sGradColor"), ingrad);
            #else
                MethodInfo setMethod = typeof(EditorGUILayout).GetMethod(
                    "GradientField",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new [] {typeof(string), typeof(Gradient), typeof(GUILayoutOption[])},
                    null);
                if(setMethod != null) {
                    ingrad = (Gradient)setMethod.Invoke(null, new object[]{lilToonInspector.GetLoc("sGradColor"), ingrad, null});;
                }
            #endif
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Test"))
            {
                texprop.textureValue = GradientToTexture(ingrad, setLinear);
            }
            if(GUILayout.Button("Save"))
            {
                Texture2D tex = GradientToTexture(ingrad, setLinear);
                tex = SaveTextureToPng(material, tex, texprop.name);
                if(setLinear) SetLinear(tex);
                texprop.textureValue = tex;
            }
            GUILayout.EndHorizontal();
        }

        public static void GradientEditor(Material material, string emissionName, Gradient ingrad, MaterialProperty texprop, bool setLinear = false)
        {
            ingrad = MaterialToGradient(material, emissionName);
            #if UNITY_2018_3_OR_NEWER
                ingrad = EditorGUILayout.GradientField(lilToonInspector.GetLoc("sGradColor"), ingrad);
            #else
                MethodInfo setMethod = typeof(EditorGUILayout).GetMethod(
                    "GradientField",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new [] {typeof(string), typeof(Gradient), typeof(GUILayoutOption[])},
                    null);
                if(setMethod != null) {
                    ingrad = (Gradient)setMethod.Invoke(null, new object[]{GetLoc("sGradColor"), ingrad, null});;
                }
            #endif
            GradientToMaterial(material, emissionName, ingrad);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Test"))
            {
                texprop.textureValue = GradientToTexture(ingrad, setLinear);
            }
            if(GUILayout.Button("Save"))
            {
                Texture2D tex = GradientToTexture(ingrad, setLinear);
                tex = SaveTextureToPng(material, tex, texprop.name);
                if(setLinear) SetLinear(tex);
                texprop.textureValue = tex;
            }
            GUILayout.EndHorizontal();
        }

        private static void SetLinear(Texture2D tex)
        {
            if(tex != null)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
                textureImporter.sRGBTexture = false;
                AssetDatabase.ImportAsset(path);
            }
        }

        private static Gradient MaterialToGradient(Material material, string emissionName)
        {
            Gradient outgrad = new Gradient();
            GradientColorKey[] colorKey = new GradientColorKey[material.GetInt(emissionName + "ci")];
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[material.GetInt(emissionName + "ai")];
            for(int i=0;i<colorKey.Length;i++)
            {
                colorKey[i].color = material.GetColor(emissionName + "c" + i.ToString());
                colorKey[i].time = material.GetColor(emissionName + "c" + i.ToString()).a;
            }
            for(int j=0;j<alphaKey.Length;j++)
            {
                alphaKey[j].alpha = material.GetColor(emissionName + "a" + j.ToString()).r;
                alphaKey[j].time = material.GetColor(emissionName + "a" + j.ToString()).a;
            }
            outgrad.SetKeys(colorKey, alphaKey);
            return outgrad;
        }

        private static void GradientToMaterial(Material material, string emissionName, Gradient ingrad)
        {
            material.SetInt(emissionName + "ci", ingrad.colorKeys.Length);
            material.SetInt(emissionName + "ai", ingrad.alphaKeys.Length);
            for(int ic=0;ic<ingrad.colorKeys.Length;ic++)
            {
                material.SetColor(emissionName + "c" + ic.ToString(), new Color(ingrad.colorKeys[ic].color.r, ingrad.colorKeys[ic].color.g, ingrad.colorKeys[ic].color.b, ingrad.colorKeys[ic].time));
            }
            for(int ia=0;ia<ingrad.alphaKeys.Length;ia++)
            {
                material.SetColor(emissionName + "a" + ia.ToString(), new Color(ingrad.alphaKeys[ia].alpha, 0, 0, ingrad.alphaKeys[ia].time));
            }
        }

        private static Texture2D GradientToTexture(Gradient grad, bool setLinear = false)
        {
            Texture2D tex = new Texture2D(128, 4, TextureFormat.ARGB32, true, setLinear);

            // Set colors
            for(int w = 0; w < tex.width; w++)
            {
                for(int h = 0; h < tex.height; h++)
                {
                    tex.SetPixel(w, h, grad.Evaluate((float)w / tex.width));
                }
            }

            tex.Apply();
            return tex;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Load Texture
        #region
        private static void GetReadableTexture(ref Texture2D tex)
        {
            if(tex == null) return;

            #if UNITY_2018_3_OR_NEWER
            if(!tex.isReadable)
            #endif
            {
                RenderTexture bufRT = RenderTexture.active;
                RenderTexture texR = RenderTexture.GetTemporary(tex.width, tex.height);
                Graphics.Blit(tex, texR);
                RenderTexture.active = texR;
                tex = new Texture2D(texR.width, texR.height);
                tex.ReadPixels(new Rect(0, 0, texR.width, texR.height), 0, 0);
                tex.Apply();
                RenderTexture.active = bufRT;
                RenderTexture.ReleaseTemporary(texR);
            }
        }

        public static void LoadTexture(ref Texture2D tex, string path)
        {
            if(string.IsNullOrEmpty(path)) return;

            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            GetReadableTexture(ref tex);
            if(path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
                tex.LoadImage(bytes);
            }

            if(tex != null) tex.filterMode = FilterMode.Bilinear;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Save Texture
        #region
        public static Texture2D SaveTextureToPng(Material material, Texture2D tex, string texname, string customName = "")
        {
            string path = AssetDatabase.GetAssetPath(material.GetTexture(texname));
            if(string.IsNullOrEmpty(path)) path = AssetDatabase.GetAssetPath(material);

            string filename = Path.GetFileNameWithoutExtension(path);
            if(!string.IsNullOrEmpty(customName)) filename += customName;
            else                                  filename += "_2";
            if(!string.IsNullOrEmpty(path))  path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), filename, "png");
            else                 path = EditorUtility.SaveFilePanel("Save Texture", "Assets", tex.name + ".png", "png");
            if(!string.IsNullOrEmpty(path)) {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
            }
            else
            {
                return (Texture2D)material.GetTexture(texname);
            }
        }

        public static Texture2D SaveTextureToPng(Texture2D tex, Texture2D origTex)
        {
            string path = AssetDatabase.GetAssetPath(origTex);
            if(!string.IsNullOrEmpty(path))  path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)+"_alpha", "png");
            else                 path = EditorUtility.SaveFilePanel("Save Texture", "Assets", tex.name + "_alpha.png", "png");
            if(!string.IsNullOrEmpty(path)) {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
            }
            else
            {
                return origTex;
            }
        }

        public static Texture2D SaveTextureToPng(Texture2D tex, string path, string customName = "")
        {
            string filename = customName + Path.GetFileNameWithoutExtension(path);
            if(!string.IsNullOrEmpty(path)) path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), filename, "png");
            else                            path = EditorUtility.SaveFilePanel("Save Texture", "Assets", filename, "png");
            if(!string.IsNullOrEmpty(path))
            {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
            }
            else
            {
                return tex;
            }
        }

        public static string SaveTextureToPng(string path, string add, Texture2D tex)
        {
            string savePath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + add + ".png";
            File.WriteAllBytes(savePath, tex.EncodeToPNG());
            return savePath;
        }
        #endregion
    }
}
#endif
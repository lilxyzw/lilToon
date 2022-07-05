#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace lilToon
{
    public class lilTextureUtils
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Gradient
        #region
        internal static void GradientEditor(Material material, Gradient ingrad, MaterialProperty texprop, bool setLinear = false)
        {
            #if UNITY_2018_3_OR_NEWER
                ingrad = EditorGUILayout.GradientField(lilLanguageManager.GetLoc("sGradColor"), ingrad);
            #else
                MethodInfo setMethod = typeof(EditorGUILayout).GetMethod(
                    "GradientField",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new [] {typeof(string), typeof(Gradient), typeof(GUILayoutOption[])},
                    null);
                if(setMethod != null) {
                    ingrad = (Gradient)setMethod.Invoke(null, new object[]{lilLanguageManager.GetLoc("sGradColor"), ingrad, null});;
                }
            #endif
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 16);
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

        internal static void GradientEditor(Material material, string emissionName, Gradient ingrad, MaterialProperty texprop, bool setLinear = false)
        {
            ingrad = MaterialToGradient(material, emissionName);
            #if UNITY_2018_3_OR_NEWER
                ingrad = EditorGUILayout.GradientField(lilLanguageManager.GetLoc("sGradColor"), ingrad);
            #else
                MethodInfo setMethod = typeof(EditorGUILayout).GetMethod(
                    "GradientField",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new [] {typeof(string), typeof(Gradient), typeof(GUILayoutOption[])},
                    null);
                if(setMethod != null) {
                    ingrad = (Gradient)setMethod.Invoke(null, new object[]{lilLanguageManager.GetLoc("sGradColor"), ingrad, null});;
                }
            #endif
            GradientToMaterial(material, emissionName, ingrad);
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 16);
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
        internal static Texture2D SaveTextureToPng(Material material, Texture2D tex, string texname, string customName = "")
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
                Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
            }
            else
            {
                return (Texture2D)material.GetTexture(texname);
            }
        }

        internal static Texture2D SaveTextureToPng(Texture2D tex, Texture2D origTex)
        {
            string path = AssetDatabase.GetAssetPath(origTex);
            if(!string.IsNullOrEmpty(path))  path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)+"_alpha", "png");
            else                 path = EditorUtility.SaveFilePanel("Save Texture", "Assets", tex.name + "_alpha.png", "png");
            if(!string.IsNullOrEmpty(path)) {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
            }
            else
            {
                return origTex;
            }
        }

        internal static Texture2D SaveTextureToPng(Texture2D tex, string path, string customName = "")
        {
            string filename = customName + Path.GetFileNameWithoutExtension(path);
            if(!string.IsNullOrEmpty(path)) path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), filename, "png");
            else                            path = EditorUtility.SaveFilePanel("Save Texture", "Assets", filename, "png");
            if(!string.IsNullOrEmpty(path))
            {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Gif to Atlas
        #region
        #if SYSTEM_DRAWING
            public static string ConvertGifToAtlas(Object tex)
            {
                int frameCount, loopXY, duration;
                float xScale, yScale;
                return ConvertGifToAtlas(tex, out frameCount, out loopXY, out duration, out xScale, out yScale);
            }

            public static string ConvertGifToAtlas(Object tex, out int frameCount, out int loopXY, out int duration, out float xScale, out float yScale)
            {
                    string path = AssetDatabase.GetAssetPath(tex);
                    System.Drawing.Image origGif = System.Drawing.Image.FromFile(path);
                    System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(origGif.FrameDimensionsList[0]);
                    frameCount = origGif.GetFrameCount(dimension);
                    loopXY = Mathf.CeilToInt(Mathf.Sqrt(frameCount));
                    duration = BitConverter.ToInt32(origGif.GetPropertyItem(20736).Value, 0);
                    int finalWidth = 1;
                    int finalHeight = 1;
                    if(EditorUtility.DisplayDialog(lilLanguageManager.GetLoc("sDialogGifToAtlas"), lilLanguageManager.GetLoc("sUtilGif2AtlasPow2"), lilLanguageManager.GetLoc("sYes"), lilLanguageManager.GetLoc("sNo")))
                    {
                        while(finalWidth < origGif.Width * loopXY) finalWidth *= 2;
                        while(finalHeight < origGif.Height * loopXY) finalHeight *= 2;
                    }
                    else
                    {
                        finalWidth = origGif.Width * loopXY;
                        finalHeight = origGif.Height * loopXY;
                    }
                    Texture2D atlasTexture = new Texture2D(finalWidth, finalHeight);
                    xScale = (float)(origGif.Width * loopXY) / finalWidth;
                    yScale = (float)(origGif.Height * loopXY) / finalHeight;
                    for(int x = 0; x < finalWidth; x++)
                    {
                        for(int y = 0; y < finalHeight; y++)
                        {
                            atlasTexture.SetPixel(x, finalHeight - 1 - y, Color.clear);
                        }
                    }
                    for(int i = 0; i < frameCount; i++)
                    {
                        int offsetX = i%loopXY;
                        int offsetY = Mathf.FloorToInt(i/loopXY);
                        origGif.SelectActiveFrame(dimension, i);
                        System.Drawing.Bitmap frame = new System.Drawing.Bitmap(origGif.Width, origGif.Height);
                        System.Drawing.Graphics.FromImage(frame).DrawImage(origGif, System.Drawing.Point.Empty);

                        for(int x = 0; x < frame.Width; x++)
                        {
                            for(int y = 0; y < frame.Height; y++)
                            {
                                System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                                atlasTexture.SetPixel(x + (frame.Width * offsetX), finalHeight - (frame.Height * offsetY) - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                            }
                        }
                    }
                    atlasTexture.Apply();

                    // Save
                    string savePath = lilTextureUtils.SaveTextureToPng(path, "_gif2png_" + loopXY + "_" + frameCount + "_" + duration, atlasTexture);
                    AssetDatabase.Refresh();
                    return savePath;
            }
        #else
            public static string ConvertGifToAtlas(Object tex)
            {
                int frameCount, loopXY, duration;
                float xScale, yScale;
                return ConvertGifToAtlas(tex, out frameCount, out loopXY, out duration, out xScale, out yScale);
            }

            public static string ConvertGifToAtlas(Object tex, out int frameCount, out int loopXY, out int duration, out float xScale, out float yScale)
            {
                frameCount = 0;
                loopXY = 0;
                duration = 0;
                xScale = 1.0f;
                yScale = 1.0f;
                return "";
            }
        #endif
        #endregion
    }
}
#endif
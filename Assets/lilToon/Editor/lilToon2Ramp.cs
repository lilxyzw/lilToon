using System.IO;
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class lilToon2Ramp
    {
        public static Texture2D Convert(Material origin, int width = 128)
        {
            var material = new Material(origin);
            material.shader = Shader.Find("Hidden/ltsother_bakeramp");

            int height = 16;
            var currentRT = RenderTexture.active;
            var renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
            RenderTexture.active = renderTexture;
            Graphics.Blit(null, renderTexture, material);
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);
            return tex;
        }

        public static void ConvertAndSave(Material origin, int width = 128, string path = null)
        {
            var tex = Convert(origin, width);

            if(string.IsNullOrEmpty(path)) path = AssetDatabase.GetAssetPath(origin)[..^4] + "_ramp.png";
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(path);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SaveAndReimport();
        }
    }
}

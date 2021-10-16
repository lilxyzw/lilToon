#if UNITY_EDITOR
using UnityEngine;

public class lilToonPreset : ScriptableObject
{
    public lilPresetBase[] bases;
    public lilToon.lilToonInspector.lilPresetCategory category;
    public Shader shader;
    public lilPresetColor[] colors;
    public lilPresetVector4[] vectors;
    public lilPresetFloat[] floats;
    public lilPresetTexture[] textures;
    public int renderQueue;
    public bool outline;
    public bool outlineMainTex;
    public bool tessellation;

    [System.Serializable]
    public struct lilPresetBase
    {
        public string language;
        public string name;
    }

    [System.Serializable]
    public struct lilPresetColor
    {
        public string name;
        public Color value;
    }

    [System.Serializable]
    public struct lilPresetVector4
    {
        public string name;
        public Vector4 value;
    }

    [System.Serializable]
    public struct lilPresetFloat
    {
        public string name;
        public float value;
    }

    [System.Serializable]
    public struct lilPresetTexture
    {
        public string name;
        public Texture value;
        public Vector2 offset;
        public Vector2 scale;
    }
}
#endif
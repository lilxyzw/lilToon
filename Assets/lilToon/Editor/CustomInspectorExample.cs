#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class CustomInspectorExample : lilToonInspector
    {

        MaterialProperty customVertexWaveScale;
        MaterialProperty customVertexWaveStrength;
        MaterialProperty customVertexWaveSpeed;
        MaterialProperty customVertexWaveMask;
        MaterialProperty customEmissionUVMode;
        public override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;
            customVertexWaveScale = FindProperty("_CustomVertexWaveScale", props);
            customVertexWaveStrength = FindProperty("_CustomVertexWaveStrength", props);
            customVertexWaveSpeed = FindProperty("_CustomVertexWaveSpeed", props);
            customVertexWaveMask = FindProperty("_CustomVertexWaveMask", props);
            customEmissionUVMode = FindProperty("_CustomEmissionUVMode", props);
        }

        public override void DrawCustomProperties(MaterialEditor materialEditor, Material material)
        {
            materialEditor.ShaderProperty(customVertexWaveScale, "Vertex Wave Scale");
            materialEditor.ShaderProperty(customVertexWaveStrength, "Vertex Wave Strength");
            materialEditor.ShaderProperty(customVertexWaveSpeed, "Vertex Wave Speed");
            materialEditor.ShaderProperty(customVertexWaveMask, "Vertex Wave Mask");
            materialEditor.ShaderProperty(customEmissionUVMode, "Emission UV Mode|UV0|UV1|UV2|UV3");
        }
    }
}
#endif
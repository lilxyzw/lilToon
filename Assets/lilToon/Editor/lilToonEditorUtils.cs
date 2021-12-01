#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#if !UNITY_2018_1_OR_NEWER
    using System.Reflection;
#endif

namespace lilToon
{
    public class lilRefreshShaders : MonoBehaviour
    {
        [MenuItem("Assets/lilToon/Refresh shaders", false, 20)]
        static void RefreshShaders()
        {
            lilToonInspector.RewriteShaderRP();
            string shaderSettingPath = lilToonInspector.GetShaderSettingPath();
            lilToonSetting shaderSetting = (lilToonSetting)AssetDatabase.LoadAssetAtPath(shaderSettingPath, typeof(lilToonSetting));
            if(shaderSetting != null) lilToonInspector.ApplyShaderSetting(shaderSetting);

            string[] shaderFolderPaths = lilToonInspector.GetShaderFolderPaths();
            bool isShadowReceive = shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW || shaderSetting.LIL_FEATURE_BACKLIGHT;
            string[] shaderGuids = AssetDatabase.FindAssets("t:shader", shaderFolderPaths);
            foreach(string shaderGuid in shaderGuids)
            {
                string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                lilToonInspector.RewriteReceiveShadow(shaderPath, isShadowReceive);
                lilToonInspector.RewriteZClip(shaderPath);
            }

            lilToonInspector.ReimportPassShaders();
            AssetDatabase.Refresh();
        }
    }

    public class lilAutoShaderSetting : MonoBehaviour
    {
        [MenuItem("Assets/lilToon/Auto shader setting", false, 20)]
        static void AutoShaderSetting()
        {
            // Load shader setting
            lilToonSetting shaderSetting = null;
            lilToonInspector.InitializeShaderSetting(ref shaderSetting);

            if(shaderSetting == null)
            {
                EditorUtility.DisplayDialog("Auto Shader Setting",lilToonInspector.GetLoc("sUtilShaderNotFound"),lilToonInspector.GetLoc("sCancel"));
                return;
            }

            if(shaderSetting.isLocked)
            {
                EditorUtility.DisplayDialog("Auto Shader Setting",lilToonInspector.GetLoc("sUtilShaderSettingLocked"),lilToonInspector.GetLoc("sCancel"));
                return;
            }

            lilToonInspector.TurnOffAllShaderSetting(ref shaderSetting);

            // Get materials
            string[] materialGuids = AssetDatabase.FindAssets("t:material");
            foreach(string guid in materialGuids)
            {
                Material material = (Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Material));
                lilToonInspector.SetupShaderSettingFromMaterial(material, ref shaderSetting);
            }

            // Get animations
            string[] clipGuids = AssetDatabase.FindAssets("t:animationclip");
            foreach(string guid in clipGuids)
            {
                AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(AnimationClip));
                lilToonInspector.SetupShaderSettingFromAnimationClip(clip, ref shaderSetting);
            }

            if(shaderSetting == null)
            {
                EditorUtility.DisplayDialog("Auto Shader Setting",lilToonInspector.GetLoc("sUtilShaderSettingNotFound"),lilToonInspector.GetLoc("sCancel"));
                return;
            }

            // Apply
            EditorUtility.SetDirty(shaderSetting);
            AssetDatabase.SaveAssets();
            lilToonInspector.ApplyShaderSetting(shaderSetting);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Auto Shader Setting",lilToonInspector.GetLoc("sComplete"),lilToonInspector.GetLoc("sOK"));
        }
    }

    public class lilRemoveUnusedProperties : MonoBehaviour
    {
        [MenuItem("Assets/lilToon/Remove unused properties")]
        static void RemoveUnusedProperties()
        {
            if(Selection.objects.Length == 0) return;
            for(int i = 0; i < Selection.objects.Length; i++)
            {
                if(Selection.objects[i] is Material)
                {
                    Material material = (Material)Selection.objects[i];
                    lilToonInspector.RemoveUnusedTexture(material);
                }
            }
        }

        [MenuItem("Assets/lilToon/Remove unused properties", true, 20)]
        static bool CheckFormat()
        {
            if(Selection.activeObject == null) return false;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();
            return path.EndsWith(".mat");
        }
    }

    public class lilSetupFromFBX : MonoBehaviour
    {
        [MenuItem("Assets/lilToon/Setup from FBX")]
        static void SetupFromFBX()
        {
            if(Selection.objects.Length == 0) return;
            Shader lts = Shader.Find("lilToon");
            if(lts == null) EditorUtility.DisplayDialog("Setup From FBX",lilToonInspector.GetLoc("sUtilShaderNotFound"),lilToonInspector.GetLoc("sCancel"));
            bool isStandardPreset = EditorUtility.DisplayDialog("Setup From FBX",lilToonInspector.GetLoc("sUtilSelectPresets"),"Standard", "Anime");
            foreach(UnityEngine.Object selectionObj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(selectionObj);
                if(!path.ToLower().EndsWith(".fbx")) continue;

                ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath(path);
                #if UNITY_2019_3_OR_NEWER
                    importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                #else
                    importer.importMaterials = true;
                #endif

                string dirPath = Path.GetDirectoryName(path);
                string materialFolder = dirPath + "/Materials";
                if(!Directory.Exists(materialFolder))
                {
                    Directory.CreateDirectory(materialFolder);
                }
                else
                {
                    if(!EditorUtility.DisplayDialog("Setup From FBX",lilToonInspector.GetLoc("sUtilMaterialAlreadyExist"),lilToonInspector.GetLoc("sYes"), lilToonInspector.GetLoc("sNo"))) return;
                }

                // Materials in SerializedObject
                SerializedObject serializedObject = new SerializedObject(importer);
                SerializedProperty serializedObjects = serializedObject.FindProperty("m_ExternalObjects");
                for(int i = 0; i < serializedObjects.arraySize; i++)
                {
                    SerializedProperty serializedMaterial = serializedObjects.GetArrayElementAtIndex(i);
                    string propType = serializedMaterial.FindPropertyRelative("first.type").stringValue;
                    if(propType != "UnityEngine:Material") continue;

                    Material material = (Material)serializedMaterial.FindPropertyRelative("second").objectReferenceValue;
                    if(material == null)
                    {
                        material = new Material(lts);
                        material.name = serializedMaterial.FindPropertyRelative("first.name").stringValue;
                    }
                    SetUpMaterial(ref material, materialFolder, isStandardPreset);
                }

                // Materials in model
                UnityEngine.Object[] objectsInFBX = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach(UnityEngine.Object obj in objectsInFBX)
                {
                    if(obj == null || !(obj is Material)) continue;
                    Material material = new Material((Material)obj);
                    SetUpMaterial(ref material, materialFolder, isStandardPreset);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                #if UNITY_2017_3_OR_NEWER
                    importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Local);
                #endif
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Assets/lilToon/Setup from FBX", true, 20)]
        static bool CheckFormat()
        {
            if(Selection.activeObject == null) return false;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();
            return path.EndsWith(".fbx");
        }

        static void SetUpMaterial(ref Material material, string materialFolder, bool isStandardPreset)
        {
            if(String.IsNullOrEmpty(material.name)) return;
            string materialFileName = material.name;
            string materialLowerName = material.name.ToLower();
            if(!materialFileName.EndsWith(".mat")) materialFileName += ".mat";
            string materialPath = materialFolder + "/" + materialFileName;
            if(File.Exists(materialPath))
            {
                material = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
            }
            else
            {
                AssetDatabase.CreateAsset(material, materialPath);
            }
            Shader lts = Shader.Find("lilToon");
            if(lts != null) material.shader = lts;

            lilToonPreset presetSkin = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("dbec582958af3f340988b3ff86a12633"), typeof(lilToonPreset));
            lilToonPreset presetSkinAnime = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("322c901472f2b9a4d98da370ea954214"), typeof(lilToonPreset));
            lilToonPreset presetSkinFlat = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("125301c732c00f84091ef099d83833b7"), typeof(lilToonPreset));
            lilToonPreset presetHair = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("2357e878227675d4bade1cc9e4c2f8ca"), typeof(lilToonPreset));
            lilToonPreset presetHairAnime = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("13a5da17b9b512c45a20e627ef499e01"), typeof(lilToonPreset));
            lilToonPreset presetCloth = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("5132cf0fbee6ea540831dc73b68c8c25"), typeof(lilToonPreset));
            lilToonPreset presetClothAnime = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("72377412f6a548c459a5e79549f29dff"), typeof(lilToonPreset));
            if(isStandardPreset)
            {
                if(materialLowerName.Contains("face"))                                              lilToonInspector.ApplyPreset(material, presetSkinFlat);
                else if(materialLowerName.Contains("body") || materialLowerName.Contains("skin"))   lilToonInspector.ApplyPreset(material, presetSkin);
                else if(materialLowerName.Contains("hair"))                                         lilToonInspector.ApplyPreset(material, presetHair);
                else                                                                                lilToonInspector.ApplyPreset(material, presetCloth);
            }
            else
            {
                if(materialLowerName.Contains("face"))                                              lilToonInspector.ApplyPreset(material, presetSkinFlat);
                else if(materialLowerName.Contains("body") || materialLowerName.Contains("skin"))   lilToonInspector.ApplyPreset(material, presetSkinAnime);
                else if(materialLowerName.Contains("hair"))                                         lilToonInspector.ApplyPreset(material, presetHairAnime);
                else                                                                                lilToonInspector.ApplyPreset(material, presetClothAnime);
            }

            bool isOutl = material.shader.name.Contains("Outline");

            if(material.GetTexture("_MainTex") == null)
            {
                string[] texGUIDs = AssetDatabase.FindAssets("t:texture2d");
                foreach(string texGUID in texGUIDs)
                {
                    Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(texGUID), typeof(Texture2D));
                    if(tex == null) continue;
                    string texNameLow = tex.name.ToLower();
                    if(!texNameLow.Contains(materialLowerName)) continue;
                    if(lilToonInspector.CheckMainTextureName(texNameLow))
                    {
                        material.SetTexture("_MainTex", tex);
                        break;
                    }
                }
            }

            if(!material.HasProperty("_ShadowStrengthMask") || material.GetTexture("_ShadowStrengthMask") == null)
            {
                string[] texGUIDs = AssetDatabase.FindAssets("t:texture2d");
                foreach(string texGUID in texGUIDs)
                {
                    Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(texGUID), typeof(Texture2D));
                    if(tex == null) continue;
                    string texNameLow = tex.name.ToLower();
                    if(!texNameLow.Contains(materialLowerName)) continue;
                    if((texNameLow.Contains("shadow") || texNameLow.Contains("shade")) && (texNameLow.Contains("mask") || texNameLow.Contains("strength")))
                    {
                        material.SetTexture("_ShadowStrengthMask", tex);
                        break;
                    }
                }
            }

            if(isOutl && (!material.HasProperty("_OutlineWidthMask") || material.GetTexture("_OutlineWidthMask") == null))
            {
                string[] texGUIDs = AssetDatabase.FindAssets("t:texture2d");
                foreach(string texGUID in texGUIDs)
                {
                    Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(texGUID), typeof(Texture2D));
                    if(tex == null) continue;
                    string texNameLow = tex.name.ToLower();
                    if(texNameLow.Contains(materialLowerName) && texNameLow.Contains("outline"))
                    {
                        material.SetTexture("_OutlineWidthMask", tex);
                        break;
                    }
                }
            }

            string mainTexLowerName = material.GetTexture("_MainTex").name.ToLower();

            if(materialLowerName.Contains("cutout") || mainTexLowerName.Contains("cutout"))
            {
                lilToonInspector.SetupMaterialWithRenderingMode(material, lilToonInspector.RenderingMode.Cutout, lilToonInspector.TransparentMode.Normal, isOutl, false, false, false);
            }
            else if(materialLowerName.Contains("alpha") || mainTexLowerName.Contains("alpha"))
            {
                lilToonInspector.SetupMaterialWithRenderingMode(material, lilToonInspector.RenderingMode.Transparent, lilToonInspector.TransparentMode.Normal, isOutl, false, false, false);
            }
            else if(materialLowerName.Contains("fade") || mainTexLowerName.Contains("fade"))
            {
                lilToonInspector.SetupMaterialWithRenderingMode(material, lilToonInspector.RenderingMode.Transparent, lilToonInspector.TransparentMode.Normal, isOutl, false, false, false);
            }
            else if(materialLowerName.Contains("transparent") || mainTexLowerName.Contains("transparent"))
            {
                lilToonInspector.SetupMaterialWithRenderingMode(material, lilToonInspector.RenderingMode.Transparent, lilToonInspector.TransparentMode.Normal, isOutl, false, false, false);
            }

            EditorUtility.SetDirty(material);
        }
    }

    public class lilConvertNormal : MonoBehaviour
    {
        // Normal Map DirectX <-> OpenGL
        [MenuItem("Assets/lilToon/Convert normal map (DirectX <-> OpenGL)")]
        static void ConvertNormal()
        {
            Texture2D srcTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true, true);
            Material hsvgMaterial = new Material(Shader.Find("Hidden/ltsother_baker"));
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            srcTexture.LoadImage(bytes);
            hsvgMaterial.SetTexture("_MainTex", srcTexture);
            hsvgMaterial.EnableKeyword("_NORMAL_DXGL");

            RenderTexture dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

            Texture2D outTexture = new Texture2D(srcTexture.width, srcTexture.height, TextureFormat.ARGB32, true, true);
            outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
            outTexture.Apply();

            // Save
            string savePath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_conv" + ".png";
            File.WriteAllBytes(savePath, outTexture.EncodeToPNG());
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(savePath);

            UnityEngine.Object.DestroyImmediate(hsvgMaterial);
            UnityEngine.Object.DestroyImmediate(srcTexture);
            UnityEngine.Object.DestroyImmediate(dstTexture);
        }

        [MenuItem("Assets/lilToon/Convert normal map (DirectX <-> OpenGL)", true, 20)]
        static bool CheckFormat()
        {
            if(Selection.activeObject == null) return false;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();
            return path.EndsWith(".png");
        }
    }

    #if SYSTEM_DRAWING
        public class lilGifToAtlas : MonoBehaviour
        {
            // Gif to Atlas
            [MenuItem("Assets/lilToon/Convert Gif to Atlas")]
            static void ConvertGifToAtlas()
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                System.Drawing.Image origGif = System.Drawing.Image.FromFile(path);
                System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(origGif.FrameDimensionsList[0]);
                int frameCount = origGif.GetFrameCount(dimension);
                int loopXY = Mathf.CeilToInt(Mathf.Sqrt(frameCount));
                int finalWidth = 1;
                int finalHeight = 1;
                if(EditorUtility.DisplayDialog("Convert Gif to Atlas", lilToonInspector.GetLoc("sUtilGif2AtlasPow2"), lilToonInspector.GetLoc("sYes"), lilToonInspector.GetLoc("sNo")))
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
                int duration = BitConverter.ToInt32(origGif.GetPropertyItem(20736).Value, 0);
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
                            atlasTexture.SetPixel(x + frame.Width*offsetX, finalHeight - frame.Height * offsetY - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                        }
                    }
                }
                atlasTexture.Apply();

                // Save
                string savePath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_gif2png_" + loopXY + "_" + frameCount + "_" + duration + ".png";
                File.WriteAllBytes(savePath, atlasTexture.EncodeToPNG());
                AssetDatabase.Refresh();
            }

            [MenuItem("Assets/lilToon/Convert Gif to Atlas", true, 20)]
            static bool CheckFormat()
            {
                if(Selection.activeObject == null) return false;
                string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();
                return path.EndsWith(".gif");
            }
        }
    #endif

    public class lilDotTextureReduction : MonoBehaviour
    {
        // Dot Texture reduction
        [MenuItem("Assets/lilToon/Dot texture reduction")]
        static void DotTextureReduction()
        {
            Texture2D srcTexture = new Texture2D(2, 2);
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            srcTexture.LoadImage(bytes);
            int finalWidth = 0;
            int finalHeight = 0;
            int scale = 0;
            if(EditorUtility.DisplayDialog("Dot Texture reduction",lilToonInspector.GetLoc("sUtilDotTexRedRatio"),"1/2","1/4"))
            {
                finalWidth = srcTexture.width / 2;
                finalHeight = srcTexture.height / 2;
                scale = 2;
            }
            else
            {
                finalWidth = srcTexture.width / 4;
                finalHeight = srcTexture.height / 4;
                scale = 4;
            }
            Texture2D outTex = new Texture2D(finalWidth, finalHeight);
            for(int x = 0; x < finalWidth; x++)
            {
                for(int y = 0; y < finalHeight; y++)
                {
                    outTex.SetPixel(x, y, srcTexture.GetPixel(x*scale, y*scale));
                }
            }
            outTex.Apply();

            // Save
            string savePath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_resized" + ".png";
            File.WriteAllBytes(savePath, outTex.EncodeToPNG());
            AssetDatabase.Refresh();
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(savePath);
            textureImporter.filterMode = FilterMode.Point;
            AssetDatabase.ImportAsset(savePath);
        }

        [MenuItem("Assets/lilToon/Dot texture reduction", true, 20)]
        static bool CheckFormat()
        {
            if(Selection.activeObject == null) return false;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();
            return path.EndsWith(".png");
        }
    }

    public class lilFixLighting : MonoBehaviour
    {
        public const string anchorName = "AutoAnchorObject";
        // Dot Texture reduction
        [MenuItem("GameObject/[lilToon] Fix lighting", false, 20)]
        static void FixLighting()
        {
            GameObject gameObject = Selection.activeGameObject;
            if(gameObject == null)
            {
                EditorUtility.DisplayDialog("[lilToon] Fix lighting",lilToonInspector.GetLoc("sUtilSelectGameObject"),lilToonInspector.GetLoc("sOK"));
                return;
            }

            // Create Anchor
            if(gameObject.transform.Find(anchorName) != null)
            {
                DestroyImmediate(gameObject.transform.Find(anchorName).gameObject);
            }
            GameObject anchorObject = new GameObject(anchorName);

            // Calculate avatar size
            float minX =  10000.0f;
            float minY =  10000.0f;
            float minZ =  10000.0f;
            float maxX = -10000.0f;
            float maxY = -10000.0f;
            float maxZ = -10000.0f;
            Transform[] objTransforms = gameObject.GetComponentsInChildren<Transform>(true);
            foreach(Transform objTransform in objTransforms)
            {
                minX = minX < objTransform.position.x ? minX : objTransform.position.x;
                minY = minY < objTransform.position.y ? minY : objTransform.position.y;
                minZ = minZ < objTransform.position.z ? minZ : objTransform.position.z;
                maxX = maxX > objTransform.position.x ? maxX : objTransform.position.x;
                maxY = maxY > objTransform.position.y ? maxY : objTransform.position.y;
                maxZ = maxZ > objTransform.position.z ? maxZ : objTransform.position.z;
            }

            Vector3 centerPosition = new Vector3((minX + maxX) / 2.0f, (minY + maxY) / 2.0f, (minZ + maxZ) / 2.0f);

            //anchorObject.transform.position = centerPosition;
            anchorObject.transform.position = new Vector3(gameObject.transform.position.x, centerPosition.y, gameObject.transform.position.z);
            anchorObject.transform.parent = gameObject.transform;

            minX -= anchorObject.transform.position.x;
            minY -= anchorObject.transform.position.y;
            minZ -= anchorObject.transform.position.z;
            maxX -= anchorObject.transform.position.x;
            maxY -= anchorObject.transform.position.y;
            maxZ -= anchorObject.transform.position.z;

            float avatarWidth = -minX;
            avatarWidth = -minY > avatarWidth ? -minY : avatarWidth;
            avatarWidth = -minZ > avatarWidth ? -minZ : avatarWidth;
            avatarWidth =  maxX > avatarWidth ?  maxX : avatarWidth;
            avatarWidth =  maxY > avatarWidth ?  maxY : avatarWidth;
            avatarWidth =  maxZ > avatarWidth ?  maxZ : avatarWidth;
            avatarWidth *= 2.5f;

            // MeshRenderer
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            if(meshRenderers.Length != 0)
            {
                foreach(MeshRenderer meshRenderer in meshRenderers)
                {
                    // Fix vertex light
                    foreach(Material material in meshRenderer.sharedMaterials)
                    {
                        if(material.shader.name.Contains("lilToon"))
                        {
                            material.SetFloat("_VertexLightStrength", 0.0f);
                            EditorUtility.SetDirty(material);
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    // Fix renderer settings
                    meshRenderer.probeAnchor = anchorObject.transform;
                    meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                    meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;
                    if(meshRenderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off)
                    {
                        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                }
            }

            // SkinnedMeshRenderer
            SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            if(skinnedMeshRenderers.Length != 0)
            {
                foreach(SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    // Fix vertex light
                    foreach(Material material in skinnedMeshRenderer.sharedMaterials)
                    {
                        if(material.shader.name.Contains("lilToon"))
                        {
                            material.SetFloat("_VertexLightStrength", 0.0f);
                            EditorUtility.SetDirty(material);
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    // Fix renderer settings
                    skinnedMeshRenderer.probeAnchor = anchorObject.transform;
                    skinnedMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                    skinnedMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;
                    if(skinnedMeshRenderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off)
                    {
                        skinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }

                    // Fix bounds
                    if(skinnedMeshRenderer.gameObject.GetComponent<Cloth>() == null && skinnedMeshRenderer.bones != null && skinnedMeshRenderer.bones.Length != 0)
                    {
                        skinnedMeshRenderer.rootBone = anchorObject.transform;
                        skinnedMeshRenderer.localBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(avatarWidth, avatarWidth, avatarWidth));
                    }
                }
            }

            EditorUtility.DisplayDialog("[lilToon] Fix Lighting",lilToonInspector.GetLoc("sComplete"),lilToonInspector.GetLoc("sOK"));
        }
    }
}
#endif
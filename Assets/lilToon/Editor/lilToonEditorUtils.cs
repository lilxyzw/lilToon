#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
#if !UNITY_2018_1_OR_NEWER
    using System.Reflection;
#endif
#if VRC_SDK_VRCSDK3 && UDON
    using VRC.SDKBase.Editor.BuildPipeline;
#endif

namespace lilToon
{
    public static class lilToonEditorUtils
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Constant
        private const string menuPathAssets                 = "Assets/lilToon/";
        private const string menuPathGameObject             = "GameObject/lilToon/";
        private const string menuPathRefreshShaders         = menuPathAssets + "[Shader] Refresh shaders";
        private const string menuPathAutoShaderSetting      = menuPathAssets + "[Shader] Full scan and auto shader setting";
        private const string menuPathRemoveUnusedProperties = menuPathAssets + "[Material] Remove unused properties";
        private const string menuPathConvertNormal          = menuPathAssets + "[Texture] Convert normal map (DirectX <-> OpenGL)";
        private const string menuPathPixelArtReduction      = menuPathAssets + "[Texture] Pixel art reduction";
        private const string menuPathConvertGifToAtlas      = menuPathAssets + "[Texture] Convert Gif to Atlas";
        private const string menuPathSetupFromFBX           = menuPathAssets + "[Model] Setup from FBX";
        private const string menuPathFixLighting            = menuPathGameObject + "[GameObject] Fix lighting";

        private const int menuPriorityAssets = 1100;
        private const int menuPriorityGameObject = 21; // This must be 21 or less
        private const int menuPriorityRefreshShaders            = menuPriorityAssets + 0;
        private const int menuPriorityAutoShaderSetting         = menuPriorityAssets + 1;
        private const int menuPriorityRemoveUnusedProperties    = menuPriorityAssets + 20;
        private const int menuPriorityConvertNormal             = menuPriorityAssets + 21;
        private const int menuPriorityPixelArtReduction         = menuPriorityAssets + 22;
        private const int menuPriorityConvertGifToAtlas         = menuPriorityAssets + 23;
        private const int menuPrioritySetupFromFBX              = menuPriorityAssets + 24;
        private const int menuPriorityFixLighting               = menuPriorityGameObject;

        private const string anchorName = "AutoAnchorObject";

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Refresh shaders
        [MenuItem(menuPathRefreshShaders, false, menuPriorityRefreshShaders)]
        private static void RefreshShaders()
        {
            lilToonInspector.RewriteShaderRP();
            string shaderSettingPath = lilToonInspector.GetShaderSettingPath();
            lilToonSetting shaderSetting = AssetDatabase.LoadAssetAtPath<lilToonSetting>(shaderSettingPath);
            if(shaderSetting != null) lilToonInspector.ApplyShaderSetting(shaderSetting);

            string[] shaderFolderPaths = lilToonInspector.GetShaderFolderPaths();
            bool isShadowReceive = (shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) || shaderSetting.LIL_FEATURE_BACKLIGHT;
            foreach(string shaderGuid in AssetDatabase.FindAssets("t:shader", shaderFolderPaths))
            {
                string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                lilToonInspector.RewriteReceiveShadow(shaderPath, isShadowReceive);
                lilToonInspector.RewriteZClip(shaderPath);
            }

            lilToonInspector.ReimportPassShaders();
            AssetDatabase.Refresh();
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Auto shader setting
        [MenuItem(menuPathAutoShaderSetting, false, menuPriorityAutoShaderSetting)]
        private static void AutoShaderSetting()
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
            foreach(string guid in AssetDatabase.FindAssets("t:material"))
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                lilToonInspector.SetupShaderSettingFromMaterial(material, ref shaderSetting);
            }

            // Get animations
            foreach(string guid in AssetDatabase.FindAssets("t:animationclip"))
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid));
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Remove unused properties
        [MenuItem(menuPathRemoveUnusedProperties, false, menuPriorityRemoveUnusedProperties)]
        private static void RemoveUnusedProperties()
        {
            if(Selection.objects.Length == 0) return;
            for(int i = 0; i < Selection.objects.Length; i++)
            {
                if(Selection.objects[i] is Material)
                {
                    lilToonInspector.RemoveUnusedTexture((Material)Selection.objects[i]);
                }
            }
        }

        [MenuItem(menuPathRemoveUnusedProperties, true, menuPriorityRemoveUnusedProperties)]
        private static bool CheckRemoveUnusedProperties()
        {
            return CheckExtension(".mat");
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Convert normal map (DirectX <-> OpenGL)
        [MenuItem(menuPathConvertNormal, false, menuPriorityConvertNormal)]
        private static void ConvertNormal()
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
            lilToonInspector.SavePng(path, "_conv", outTexture);
            AssetDatabase.Refresh();

            UnityEngine.Object.DestroyImmediate(hsvgMaterial);
            UnityEngine.Object.DestroyImmediate(srcTexture);
            UnityEngine.Object.DestroyImmediate(dstTexture);
        }

        [MenuItem(menuPathConvertNormal, true, menuPriorityConvertNormal)]
        private static bool CheckConvertNormal()
        {
            return CheckImageExtension();
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Convert Gif to Atlas
        #if SYSTEM_DRAWING
            // Gif to Atlas
            [MenuItem(menuPathConvertGifToAtlas, false, menuPriorityConvertGifToAtlas)]
            private static void ConvertGifToAtlas()
            {
                lilToonInspector.ConvertGifToAtlas(Selection.activeObject);
            }

            [MenuItem(menuPathConvertGifToAtlas, true, menuPriorityConvertGifToAtlas)]
            private static bool CheckConvertGifToAtlas()
            {
                return CheckExtension(".gif");
            }
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Dot texture reduction
        [MenuItem(menuPathPixelArtReduction, false, menuPriorityPixelArtReduction)]
        private static void PixelArtReduction()
        {
            Texture2D srcTexture = new Texture2D(2, 2);
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            srcTexture.LoadImage(bytes);
            int finalWidth;
            int finalHeight;
            int scale;
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
            string savePath = lilToonInspector.SavePng(path, "_resized", outTex);
            AssetDatabase.Refresh();
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(savePath);
            textureImporter.filterMode = FilterMode.Point;
            AssetDatabase.ImportAsset(savePath);
        }

        [MenuItem(menuPathPixelArtReduction, true, menuPriorityPixelArtReduction)]
        private static bool CheckPixelArtReduction()
        {
            return CheckImageExtension();
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Setup from FBX
        [MenuItem(menuPathSetupFromFBX, false, menuPrioritySetupFromFBX)]
        private static void SetupFromFBX()
        {
            if(Selection.objects.Length == 0) return;
            Shader lts = Shader.Find("lilToon");
            if(lts == null) EditorUtility.DisplayDialog("Setup From FBX",lilToonInspector.GetLoc("sUtilShaderNotFound"),lilToonInspector.GetLoc("sCancel"));
            bool isStandardPreset = EditorUtility.DisplayDialog("Setup From FBX",lilToonInspector.GetLoc("sUtilSelectPresets"),"Standard", "Anime");
            foreach(UnityEngine.Object selectionObj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(selectionObj);
                if(!path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) continue;

                ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(path);
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
                        material = new Material(lts)
                        {
                            name = serializedMaterial.FindPropertyRelative("first.name").stringValue
                        };
                    }
                    SetUpMaterial(ref material, materialFolder, isStandardPreset);
                }

                // Materials in model
                foreach(UnityEngine.Object obj in AssetDatabase.LoadAllAssetsAtPath(path))
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

        [MenuItem(menuPathSetupFromFBX, true, menuPrioritySetupFromFBX)]
        private static bool CheckSetupFromFBX()
        {
            return CheckExtension(".fbx");
        }

        private static void SetUpMaterial(ref Material material, string materialFolder, bool isStandardPreset)
        {
            if(string.IsNullOrEmpty(material.name)) return;
            string materialFileName = material.name;
            string materialLowerName = material.name.ToLower();
            if(!materialFileName.EndsWith(".mat")) materialFileName += ".mat";
            string materialPath = materialFolder + "/" + materialFileName;
            if(File.Exists(materialPath))
            {
                material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            }
            else
            {
                AssetDatabase.CreateAsset(material, materialPath);
            }
            Shader lts = Shader.Find("lilToon");
            if(lts != null) material.shader = lts;

            lilToonPreset presetSkin        = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("dbec582958af3f340988b3ff86a12633"));
            lilToonPreset presetSkinAnime   = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("322c901472f2b9a4d98da370ea954214"));
            lilToonPreset presetSkinFlat    = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("125301c732c00f84091ef099d83833b7"));
            lilToonPreset presetHair        = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("2357e878227675d4bade1cc9e4c2f8ca"));
            lilToonPreset presetHairAnime   = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("13a5da17b9b512c45a20e627ef499e01"));
            lilToonPreset presetCloth       = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("5132cf0fbee6ea540831dc73b68c8c25"));
            lilToonPreset presetClothAnime  = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("72377412f6a548c459a5e79549f29dff"));
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
                foreach(string texGUID in AssetDatabase.FindAssets("t:texture2d"))
                {
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(texGUID));
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
                foreach(string texGUID in AssetDatabase.FindAssets("t:texture2d"))
                {
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(texGUID));
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
                foreach(string texGUID in AssetDatabase.FindAssets("t:texture2d"))
                {
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(texGUID));
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

        //------------------------------------------------------------------------------------------------------------------------------
        // GameObject/[lilToon] Fix lighting
        [MenuItem(menuPathFixLighting, false, menuPriorityFixLighting)]
        private static void FixLighting()
        {
            GameObject gameObject = Selection.activeGameObject;
            /*if(gameObject == null)
            {
                EditorUtility.DisplayDialog("[lilToon] Fix lighting",lilToonInspector.GetLoc("sUtilSelectGameObject"),lilToonInspector.GetLoc("sOK"));
                return;
            }*/

            // Create Anchor
            if(gameObject.transform.Find(anchorName) != null)
            {
                UnityEngine.Object.DestroyImmediate(gameObject.transform.Find(anchorName).gameObject);
            }
            GameObject anchorObject = new GameObject(anchorName);

            // Calculate avatar size
            float minX =  10000.0f;
            float minY =  10000.0f;
            float minZ =  10000.0f;
            float maxX = -10000.0f;
            float maxY = -10000.0f;
            float maxZ = -10000.0f;
            foreach(Transform objTransform in gameObject.GetComponentsInChildren<Transform>(true))
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

            string shaderSettingPath = lilToonInspector.GetShaderSettingPath();
            lilToonSetting shaderSetting = AssetDatabase.LoadAssetAtPath<lilToonSetting>(shaderSettingPath);

            // MeshRenderer
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            if(meshRenderers.Length != 0)
            {
                foreach(MeshRenderer meshRenderer in meshRenderers)
                {
                    // Fix vertex light
                    foreach(Material material in meshRenderer.sharedMaterials)
                    {
                        if(material.shader.name.Contains("lilToon") && shaderSetting != null)
                        {
                            material.SetFloat("_AsUnlit", shaderSetting.defaultAsUnlit);
                            material.SetFloat("_VertexLightStrength", shaderSetting.defaultVertexLightStrength);
                            material.SetFloat("_LightMinLimit", shaderSetting.defaultLightMinLimit);
                            material.SetFloat("_LightMaxLimit", shaderSetting.defaultLightMaxLimit);
                            material.SetFloat("_BeforeExposureLimit", shaderSetting.defaultBeforeExposureLimit);
                            material.SetFloat("_MonochromeLighting", shaderSetting.defaultMonochromeLighting);
                            material.SetFloat("_lilDirectionalLightStrength", shaderSetting.defaultlilDirectionalLightStrength);
                            material.SetVector("_LightDirectionOverride", shaderSetting.defaultLightDirectionOverride);
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
                        if(material != null && material.shader != null && material.shader.name.Contains("lilToon") && shaderSetting != null)
                        {
                            material.SetFloat("_AsUnlit", shaderSetting.defaultAsUnlit);
                            material.SetFloat("_VertexLightStrength", shaderSetting.defaultVertexLightStrength);
                            material.SetFloat("_LightMinLimit", shaderSetting.defaultLightMinLimit);
                            material.SetFloat("_LightMaxLimit", shaderSetting.defaultLightMaxLimit);
                            material.SetFloat("_BeforeExposureLimit", shaderSetting.defaultBeforeExposureLimit);
                            material.SetFloat("_MonochromeLighting", shaderSetting.defaultMonochromeLighting);
                            material.SetFloat("_lilDirectionalLightStrength", shaderSetting.defaultlilDirectionalLightStrength);
                            material.SetVector("_LightDirectionOverride", shaderSetting.defaultLightDirectionOverride);
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

        [MenuItem(menuPathFixLighting, true, menuPriorityFixLighting)]
        private static bool CheckFixLighting()
        {
            return Selection.activeGameObject != null;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Format checker
        private static bool CheckExtension(string extension)
        {
            if(Selection.activeObject == null) return false;
            return AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }

        private static bool CheckImageExtension()
        {
            if(Selection.activeObject == null) return false;
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            return assetPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                   assetPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   assetPath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase);
        }
    }

#if UNITY_2019_3_OR_NEWER
    //------------------------------------------------------------------------------------------------------------------------------
    // Build size optimization
    public class lilToonPreprocessShaders : UnityEditor.Build.IPreprocessShaders
    {
        public int callbackOrder { get { return default(int); } }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if(!shader.name.Contains("lilToon") && !shader.name.Contains("ltspass")) return;

            lilToonInspector.lilRenderPipeline lilRP = lilToonInspector.CheckRP();

            if(shader.name.Contains("lilToonMulti"))
            {
                string[] keywords = GatherKeywords(shader, data);
                Material[] materials = GatherMaterials(shader);

                for(int i = data.Count - 1; i >= 0; i--)
                {
                    bool isMatch = false;
                    if(ShouldRemoveShadowsScreen(shader, data[i].shaderKeywordSet, lilRP))
                    {
                        data.RemoveAt(i);
                        continue;
                    }
                    foreach(Material material in materials)
                    {
                        if(IsMatchKeywords(material, data[i].shaderKeywordSet, shader, keywords))
                        {
                            isMatch = true;
                            break;
                        }
                    }
                    if(isMatch) continue;
                    data.RemoveAt(i);
                }
            }
        }

        private Material[] GatherMaterials(Shader shader)
        {
            List<Material> materialList = new List<Material>();
            foreach(string guid in AssetDatabase.FindAssets("t:material"))
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if(material.shader == shader) materialList.Add(material);
            }
            return materialList.ToArray();
        }

        private bool IsMatchKeywords(Material material, ShaderKeywordSet shaderKeywordSet, Shader shader, string[] keywords)
        {
            foreach(string keyword in keywords)
            {
                bool materialHasKeyword = System.Array.IndexOf(material.shaderKeywords, keyword) >= 0;
                ShaderKeyword keyword2 = new ShaderKeyword(shader, keyword);
                if(materialHasKeyword && shaderKeywordSet.IsEnabled(keyword2))
                {
                    continue;
                }
                if(!materialHasKeyword && !shaderKeywordSet.IsEnabled(keyword2))
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        private bool ShouldRemoveShadowsScreen(Shader shader, ShaderKeywordSet shaderKeywordSet, lilToonInspector.lilRenderPipeline lilRP)
        {
            ShaderKeyword _REQUIRE_UV2 = new ShaderKeyword(shader, "_REQUIRE_UV2");
            ShaderKeyword ANTI_FLICKER = new ShaderKeyword(shader, "ANTI_FLICKER");
            if(shaderKeywordSet.IsEnabled(_REQUIRE_UV2) || shaderKeywordSet.IsEnabled(ANTI_FLICKER)) return false;
            if(lilRP == lilToonInspector.lilRenderPipeline.BRP)
            {
                ShaderKeyword SHADOWS_SCREEN                = new ShaderKeyword(shader, "SHADOWS_SCREEN");
                return shaderKeywordSet.IsEnabled(SHADOWS_SCREEN);
            }
            else if(lilRP == lilToonInspector.lilRenderPipeline.LWRP || lilRP == lilToonInspector.lilRenderPipeline.URP)
            {
                ShaderKeyword _MAIN_LIGHT_SHADOWS           = new ShaderKeyword(shader, "_MAIN_LIGHT_SHADOWS");
                ShaderKeyword _MAIN_LIGHT_SHADOWS_CASCADE   = new ShaderKeyword(shader, "_MAIN_LIGHT_SHADOWS_CASCADE");
                ShaderKeyword _MAIN_LIGHT_SHADOWS_SCREEN    = new ShaderKeyword(shader, "_MAIN_LIGHT_SHADOWS_SCREEN");
                ShaderKeyword _SHADOWS_SOFT                 = new ShaderKeyword(shader, "_SHADOWS_SOFT");
                return shaderKeywordSet.IsEnabled(_MAIN_LIGHT_SHADOWS) || shaderKeywordSet.IsEnabled(_MAIN_LIGHT_SHADOWS_CASCADE) || shaderKeywordSet.IsEnabled(_MAIN_LIGHT_SHADOWS_SCREEN) || shaderKeywordSet.IsEnabled(_SHADOWS_SOFT);
            }
            else if(lilRP == lilToonInspector.lilRenderPipeline.HDRP)
            {
                ShaderKeyword SCREEN_SPACE_SHADOWS_OFF      = new ShaderKeyword(shader, "SCREEN_SPACE_SHADOWS_OFF");
                ShaderKeyword SCREEN_SPACE_SHADOWS_ON       = new ShaderKeyword(shader, "SCREEN_SPACE_SHADOWS_ON");
                ShaderKeyword SHADOW_LOW                    = new ShaderKeyword(shader, "SHADOW_LOW");
                ShaderKeyword SHADOW_MEDIUM                 = new ShaderKeyword(shader, "SHADOW_MEDIUM");
                ShaderKeyword SHADOW_HIGH                   = new ShaderKeyword(shader, "SHADOW_HIGH");
                return shaderKeywordSet.IsEnabled(SCREEN_SPACE_SHADOWS_OFF) || shaderKeywordSet.IsEnabled(SCREEN_SPACE_SHADOWS_ON) || shaderKeywordSet.IsEnabled(SHADOW_LOW) || shaderKeywordSet.IsEnabled(SHADOW_MEDIUM) || shaderKeywordSet.IsEnabled(SHADOW_HIGH);
            }
            return false;
        }

        private string[] GatherKeywords(Shader shader, IList<ShaderCompilerData> data)
        {
            List<string> keywordList = new List<string>();
            foreach(ShaderCompilerData part in data)
            {
                foreach(ShaderKeyword keyword in part.shaderKeywordSet.GetShaderKeywords())
                {
                    #if UNITY_2021_2_OR_NEWER
                        if(!ShaderKeyword.IsKeywordLocal(keyword) || keywordList.Contains(keyword.name)) continue;
                        keywordList.Add(keyword.name);
                    #else
                        if(!ShaderKeyword.IsKeywordLocal(keyword) || keywordList.Contains(ShaderKeyword.GetKeywordName(shader, keyword))) continue;
                        keywordList.Add(ShaderKeyword.GetKeywordName(shader, keyword));
                    #endif
                }
            }
            return keywordList.ToArray();
        }
    }
#endif

#if VRC_SDK_VRCSDK3 && UDON
    //------------------------------------------------------------------------------------------------------------------------------
    // VRChat
    public class lilToonVRCBuildPreprocess : IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => 0;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            // Load shader setting
            lilToonSetting shaderSetting = null;
            lilToonInspector.InitializeShaderSetting(ref shaderSetting);

            if(shaderSetting == null || shaderSetting.isLocked) return true;

            lilToonInspector.TurnOffAllShaderSetting(ref shaderSetting);

            // Get materials
            foreach(string guid in AssetDatabase.FindAssets("t:material"))
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                lilToonInspector.SetupShaderSettingFromMaterial(material, ref shaderSetting);
            }

            // Get animations
            foreach(string guid in AssetDatabase.FindAssets("t:animationclip"))
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid));
                lilToonInspector.SetupShaderSettingFromAnimationClip(clip, ref shaderSetting);
            }

            // Apply
            EditorUtility.SetDirty(shaderSetting);
            AssetDatabase.SaveAssets();
            lilToonInspector.ApplyShaderSetting(shaderSetting);
            AssetDatabase.Refresh();
            return true;
        }
    }
#endif
}
#endif
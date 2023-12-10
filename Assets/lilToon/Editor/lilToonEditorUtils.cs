#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Object = UnityEngine.Object;
using System.Text;
using System.Reflection;

namespace lilToon
{
    public static class lilToonEditorUtils
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Constant
        private const string menuPathAssets                 = "Assets/lilToon/";
        private const string menuPathGameObject             = "GameObject/lilToon/";
        private const string menuPathRefreshShaders         = menuPathAssets + "[Shader] Refresh shaders";
        private const string menuPathRemoveUnusedProperties = menuPathAssets + "[Material] Remove unused properties";
        private const string menuPathRunMigration           = menuPathAssets + "[Material] Run migration";
        private const string menuPathConvertNormal          = menuPathAssets + "[Texture] Convert normal map (DirectX <-> OpenGL)";
        private const string menuPathPixelArtReduction      = menuPathAssets + "[Texture] Pixel art reduction";
        private const string menuPathConvertGifToAtlas      = menuPathAssets + "[Texture] Convert Gif to Atlas";
        private const string menuPathConvertLUTToPNG        = menuPathAssets + "[Texture] Convert LUT to PNG";
        private const string menuPathSetupFromFBX           = menuPathAssets + "[Model] Setup from FBX";
        private const string menuPathFixLighting            = menuPathGameObject + "[GameObject] Fix lighting";

        private const int menuPriorityAssets = 1100;
        private const int menuPriorityGameObject = 21; // This must be 21 or less
        private const int menuPriorityRefreshShaders            = menuPriorityAssets + 0;
        private const int menuPriorityRemoveUnusedProperties    = menuPriorityAssets + 20;
        private const int menuPriorityRunMigration              = menuPriorityAssets + 21;
        private const int menuPriorityConvertNormal             = menuPriorityAssets + 22;
        private const int menuPriorityPixelArtReduction         = menuPriorityAssets + 23;
        private const int menuPriorityConvertGifToAtlas         = menuPriorityAssets + 24;
        private const int menuPriorityConvertLUTToPNG           = menuPriorityAssets + 25;
        private const int menuPrioritySetupFromFBX              = menuPriorityAssets + 26;
        private const int menuPriorityFixLighting               = menuPriorityGameObject;

        private const string anchorName = "AutoAnchorObject";

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Refresh shaders
        [MenuItem(menuPathRefreshShaders, false, menuPriorityRefreshShaders)]
        private static void RefreshShaders()
        {
            lilEditorParameters.instance.modifiedShaders = "";
            lilToonSetting shaderSetting = null;
            lilToonSetting.InitializeShaderSetting(ref shaderSetting);
            if(shaderSetting.isDebugOptimize)
            {
                lilToonSetting.ApplyShaderSettingOptimized();
                return;
            }
            if(lilShaderAPI.IsTextureLimitedAPI())
            {
                lilToonSetting.TurnOffAllShaderSetting(ref shaderSetting);
                lilToonSetting.CheckTextures(ref shaderSetting);
            }

            lilToonSetting.TurnOnAllShaderSetting(ref shaderSetting);
            lilToonSetting.ApplyShaderSetting(shaderSetting);

            AssetDatabase.Refresh();
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Remove unused properties
        [MenuItem(menuPathRemoveUnusedProperties, false, menuPriorityRemoveUnusedProperties)]
        private static void RemoveUnusedProperties()
        {
            if(Selection.objects.Length == 0) return;
            Undo.RecordObjects(Selection.objects, "Remove unused properties");
            foreach(var m in Selection.objects.Where(o => o is Material).Select(o => (Material)o))
            {
                lilMaterialUtils.RemoveUnusedTexture(m);
            }
        }

        [MenuItem(menuPathRemoveUnusedProperties, true, menuPriorityRemoveUnusedProperties)]
        private static bool CheckRemoveUnusedProperties()
        {
            return CheckExtension(".mat");
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Run migration
        [MenuItem(menuPathRunMigration, false, menuPriorityRunMigration)]
        private static void RunMigration()
        {
            lilStartup.MigrateMaterials();
            EditorUtility.DisplayDialog("[lilToon] Run migration",GetLoc("sComplete"),GetLoc("sOK"));
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Convert normal map (DirectX <-> OpenGL)
        [MenuItem(menuPathConvertNormal, false, menuPriorityConvertNormal)]
        private static void ConvertNormal()
        {
            var srcTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true, true);
            var hsvgMaterial = new Material(Shader.Find("Hidden/ltsother_baker"));
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            lilTextureUtils.LoadTexture(ref srcTexture, path);
            hsvgMaterial.SetTexture("_MainTex", srcTexture);
            hsvgMaterial.EnableKeyword("_NORMAL_DXGL");

            Texture2D outTexture = null;
            lilToonInspector.RunBake(ref outTexture, srcTexture, hsvgMaterial);

            // Save
            lilTextureUtils.SaveTextureToPng(path, "_conv", outTexture);
            AssetDatabase.Refresh();

            Object.DestroyImmediate(hsvgMaterial);
            Object.DestroyImmediate(srcTexture);
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
                lilTextureUtils.ConvertGifToAtlas(Selection.activeObject);
            }

            [MenuItem(menuPathConvertGifToAtlas, true, menuPriorityConvertGifToAtlas)]
            private static bool CheckConvertGifToAtlas()
            {
                return CheckExtension(".gif");
            }
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Convert LUT to PNG
        [MenuItem(menuPathConvertLUTToPNG, false, menuPriorityConvertLUTToPNG)]
        private static void ConvertLUTToPNG()
        {
            foreach(var o in Selection.objects)
            {
                lilTextureUtils.ConvertLUTToPNG(o);
            }
        }

        [MenuItem(menuPathConvertLUTToPNG, true, menuPriorityConvertLUTToPNG)]
        private static bool CheckConvertLUTToPNG()
        {
            return CheckExtension(".cube");
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Assets/lilToon/Dot texture reduction
        [MenuItem(menuPathPixelArtReduction, false, menuPriorityPixelArtReduction)]
        private static void PixelArtReduction()
        {
            var srcTexture = new Texture2D(2, 2);
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var bytes = File.ReadAllBytes(Path.GetFullPath(path));
            srcTexture.LoadImage(bytes);
            lilTextureUtils.LoadTexture(ref srcTexture, path);
            int finalWidth;
            int finalHeight;
            int scale;
            if(EditorUtility.DisplayDialog("Dot Texture reduction",GetLoc("sUtilDotTexRedRatio"),"1/2","1/4"))
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
            var outTex = new Texture2D(finalWidth, finalHeight);
            for(int x = 0; x < finalWidth; x++)
            {
                for(int y = 0; y < finalHeight; y++)
                {
                    outTex.SetPixel(x, y, srcTexture.GetPixel(x*scale, y*scale));
                }
            }
            outTex.Apply();

            // Save
            string savePath = lilTextureUtils.SaveTextureToPng(path, "_resized", outTex);
            AssetDatabase.Refresh();
            var textureImporter = (TextureImporter)AssetImporter.GetAtPath(savePath);
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
            var lts = Shader.Find("lilToon");
            if(lts == null) EditorUtility.DisplayDialog("Setup From FBX",GetLoc("sUtilShaderNotFound"),GetLoc("sCancel"));
            Undo.RecordObjects(Selection.objects, "Setup From FBX");
            foreach(var selectionObj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(selectionObj);
                if(!path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) continue;

                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
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
                    if(!EditorUtility.DisplayDialog("Setup From FBX",GetLoc("sUtilMaterialAlreadyExist"),GetLoc("sYes"),GetLoc("sNo"))) return;
                }

                lilToonSetting shaderSetting = null;
                lilToonSetting.InitializeShaderSetting(ref shaderSetting);

                // Materials in SerializedObject
                var serializedObject = new SerializedObject(importer);
                var serializedObjects = serializedObject.FindProperty("m_ExternalObjects");
                for(int i = 0; i < serializedObjects.arraySize; i++)
                {
                    var serializedMaterial = serializedObjects.GetArrayElementAtIndex(i);
                    string propType = serializedMaterial.FindPropertyRelative("first.type").stringValue;
                    if(propType != "UnityEngine:Material") continue;

                    var material = (Material)serializedMaterial.FindPropertyRelative("second").objectReferenceValue;
                    if(material == null)
                    {
                        material = new Material(lts)
                        {
                            name = serializedMaterial.FindPropertyRelative("first.name").stringValue
                        };
                    }
                    SetUpMaterial(ref material, materialFolder, shaderSetting);
                }

                // Materials in model
                foreach(var obj in AssetDatabase.LoadAllAssetsAtPath(path).Where(o => o is Material))
                {
                    var material = new Material((Material)obj);
                    SetUpMaterial(ref material, materialFolder, shaderSetting);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Local);
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
            }
        }

        [MenuItem(menuPathSetupFromFBX, true, menuPrioritySetupFromFBX)]
        private static bool CheckSetupFromFBX()
        {
            return CheckExtension(".fbx");
        }

        private static void SetUpMaterial(ref Material material, string materialFolder, lilToonSetting shaderSetting)
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
            var lts = Shader.Find("lilToon");
            if(lts != null) material.shader = lts;

            if(material.GetTexture("_MainTex") == null)
            {
                foreach(var tex in lilDirectoryManager.FindAssets<Texture2D>("t:texture2d"))
                {
                    string texNameLow = tex.name.ToLower();
                    if(!texNameLow.Contains(materialLowerName)) continue;
                    if(lilMaterialUtils.CheckMainTextureName(texNameLow))
                    {
                        material.SetTexture("_MainTex", tex);
                        break;
                    }
                }
            }

            lilToonPreset presetSkin = null;
            lilToonPreset presetFace = null;
            lilToonPreset presetHair = null;
            lilToonPreset presetCloth = null;

            if(shaderSetting != null)
            {
                presetSkin    = shaderSetting.presetSkin;
                presetFace    = shaderSetting.presetFace;
                presetHair    = shaderSetting.presetHair;
                presetCloth   = shaderSetting.presetCloth;
            }

            if(presetSkin  == null) presetSkin  = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("44e146d270da72d4cb21a0a3b8658d1a"));
            if(presetFace  == null) presetFace  = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("125301c732c00f84091ef099d83833b7"));
            if(presetHair  == null) presetHair  = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("b66bf1309c6d60847ae978e0a54ac5fa"));
            if(presetCloth == null) presetCloth = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath("193de7d9d533d4841842d8c5ed740259"));
            if(materialLowerName.Contains("face"))                                              lilToonPreset.ApplyPreset(material, presetFace, false);
            else if(materialLowerName.Contains("body") || materialLowerName.Contains("skin"))   lilToonPreset.ApplyPreset(material, presetSkin, false);
            else if(materialLowerName.Contains("hair"))                                         lilToonPreset.ApplyPreset(material, presetHair, false);
            else                                                                                lilToonPreset.ApplyPreset(material, presetCloth, false);

            bool isOutl = material.shader.name.Contains("Outline");

            if(!material.HasProperty("_ShadowStrengthMask") || material.GetTexture("_ShadowStrengthMask") == null)
            {
                foreach(var tex in lilDirectoryManager.FindAssets<Texture2D>("t:texture2d"))
                {
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
                foreach(var tex in lilDirectoryManager.FindAssets<Texture2D>("t:texture2d"))
                {
                    string texNameLow = tex.name.ToLower();
                    if(texNameLow.Contains(materialLowerName) && texNameLow.Contains("outline"))
                    {
                        material.SetTexture("_OutlineWidthMask", tex);
                        break;
                    }
                }
            }

            string mainTexLowerName = "";
            if(material.GetTexture("_MainTex") != null) mainTexLowerName = material.GetTexture("_MainTex").name.ToLower();

            if(materialLowerName.Contains("cutout") || mainTexLowerName.Contains("cutout"))
            {
                lilMaterialUtils.SetupMaterialWithRenderingMode(material, RenderingMode.Cutout, TransparentMode.Normal, isOutl, false, false, false);
            }
            else if(materialLowerName.Contains("alpha") || mainTexLowerName.Contains("alpha") || materialLowerName.Contains("fade") || mainTexLowerName.Contains("fade") || materialLowerName.Contains("transparent") || mainTexLowerName.Contains("transparent"))
            {
                lilMaterialUtils.SetupMaterialWithRenderingMode(material, RenderingMode.Transparent, TransparentMode.Normal, isOutl, false, false, false);
            }

            EditorUtility.SetDirty(material);
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // GameObject/[lilToon] Fix lighting
        [MenuItem(menuPathFixLighting, false, menuPriorityFixLighting)]
        private static void FixLighting()
        {
            var gameObject = Selection.activeGameObject;
            var anchorTransform = gameObject.transform.Find(anchorName);
            var anchorObject = anchorTransform != null ? anchorTransform.gameObject : null;
            var meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            var skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            var recordObjects = new List<Object>{gameObject};
            recordObjects.AddRange(meshRenderers);
            recordObjects.AddRange(skinnedMeshRenderers);

            // Create Anchor
            if(anchorObject == null)
            {
                anchorObject = new GameObject(anchorName);
            }
            recordObjects.Add(anchorObject);
            Undo.RecordObjects(recordObjects.ToArray(), "[lilToon] Fix lighting");

            // Calculate avatar size
            float minX =  10000.0f;
            float minY =  10000.0f;
            float minZ =  10000.0f;
            float maxX = -10000.0f;
            float maxY = -10000.0f;
            float maxZ = -10000.0f;
            //foreach(var objTransform in gameObject.GetComponentsInChildren<Transform>(true))
            foreach(var objTransform in skinnedMeshRenderers.SelectMany(s=>s.bones).Union(meshRenderers.Select(m=>m.transform)).Where(t=>t!=null))
            {
                minX = minX < objTransform.position.x ? minX : objTransform.position.x;
                minY = minY < objTransform.position.y ? minY : objTransform.position.y;
                minZ = minZ < objTransform.position.z ? minZ : objTransform.position.z;
                maxX = maxX > objTransform.position.x ? maxX : objTransform.position.x;
                maxY = maxY > objTransform.position.y ? maxY : objTransform.position.y;
                maxZ = maxZ > objTransform.position.z ? maxZ : objTransform.position.z;
            }

            var centerPosition = new Vector3((minX + maxX) / 2.0f, (minY + maxY) / 2.0f, (minZ + maxZ) / 2.0f);

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

            lilToonSetting shaderSetting = null;
            lilToonSetting.InitializeShaderSetting(ref shaderSetting);

            // MeshRenderer
            if(meshRenderers.Length != 0)
            {
                foreach(var meshRenderer in meshRenderers)
                {
                    // Fix vertex light
                    foreach(var material in meshRenderer.sharedMaterials.Where(m => lilMaterialUtils.CheckShaderIslilToon(m) && shaderSetting != null))
                    {
                        Undo.RecordObject(material, "[lilToon] Fix lighting");
                        material.SetFloat("_AsUnlit", shaderSetting.defaultAsUnlit);
                        material.SetFloat("_VertexLightStrength", shaderSetting.defaultVertexLightStrength);
                        material.SetFloat("_LightMinLimit", shaderSetting.defaultLightMinLimit);
                        material.SetFloat("_LightMaxLimit", shaderSetting.defaultLightMaxLimit);
                        material.SetFloat("_BeforeExposureLimit", shaderSetting.defaultBeforeExposureLimit);
                        material.SetFloat("_MonochromeLighting", shaderSetting.defaultMonochromeLighting);
                        material.SetFloat("_lilDirectionalLightStrength", shaderSetting.defaultlilDirectionalLightStrength);
                        EditorUtility.SetDirty(material);
                    }

                    // Fix renderer settings
                    meshRenderer.probeAnchor = anchorObject.transform;
                    meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
                    meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
                    if(meshRenderer.shadowCastingMode == ShadowCastingMode.Off)
                    {
                        meshRenderer.shadowCastingMode = ShadowCastingMode.On;
                    }
                }
            }

            // SkinnedMeshRenderer
            if(skinnedMeshRenderers.Length != 0)
            {
                foreach(var skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    // Fix vertex light
                    foreach(var material in skinnedMeshRenderer.sharedMaterials.Where(m => lilMaterialUtils.CheckShaderIslilToon(m) && shaderSetting != null))
                    {
                        Undo.RecordObject(material, "[lilToon] Fix lighting");
                        material.SetFloat("_AsUnlit", shaderSetting.defaultAsUnlit);
                        material.SetFloat("_VertexLightStrength", shaderSetting.defaultVertexLightStrength);
                        material.SetFloat("_LightMinLimit", shaderSetting.defaultLightMinLimit);
                        material.SetFloat("_LightMaxLimit", shaderSetting.defaultLightMaxLimit);
                        material.SetFloat("_BeforeExposureLimit", shaderSetting.defaultBeforeExposureLimit);
                        material.SetFloat("_MonochromeLighting", shaderSetting.defaultMonochromeLighting);
                        material.SetFloat("_lilDirectionalLightStrength", shaderSetting.defaultlilDirectionalLightStrength);
                        EditorUtility.SetDirty(material);
                    }

                    // Fix renderer settings
                    skinnedMeshRenderer.probeAnchor = anchorObject.transform;
                    skinnedMeshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
                    skinnedMeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
                    if(skinnedMeshRenderer.shadowCastingMode == ShadowCastingMode.Off)
                    {
                        skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.On;
                    }

                    // Fix bounds
                    if(skinnedMeshRenderer.gameObject.GetComponent<Cloth>() == null && skinnedMeshRenderer.bones != null && skinnedMeshRenderer.bones.Length != 0)
                    {
                        skinnedMeshRenderer.rootBone = anchorObject.transform;
                        skinnedMeshRenderer.localBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(avatarWidth, avatarWidth, avatarWidth));
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("[lilToon] Fix Lighting",GetLoc("sComplete"),GetLoc("sOK"));
        }

        [MenuItem(menuPathFixLighting, true, menuPriorityFixLighting)]
        private static bool CheckFixLighting()
        {
            return Selection.activeGameObject != null;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Debug
        [MenuItem("GameObject/lilToon/[Debug] Generate bug report", false, 22)]
        public static void GenerateBugReport()
        {
            GenerateBugReport(null, null, null);
        }

        internal static void GenerateBugReport(List<Material> materialsIn, List<AnimationClip> clipsIn, string addText)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Shader Information");
            sb.AppendLine("lilToon " + lilConstants.currentVersionName);
            sb.AppendLine();

            if(!string.IsNullOrEmpty(addText))
            {
                sb.AppendLine(addText);
                sb.AppendLine();
            }
            sb.AppendLine("# Platform Information");
            sb.AppendLine("Unity: " + Application.unityVersion);
            sb.AppendLine("Platform: " + Application.platform.ToString());
            sb.AppendLine("Language: " + Application.systemLanguage.ToString());
            sb.AppendLine("Shader API: " + SystemInfo.graphicsDeviceType.ToString());
            sb.AppendLine();

            sb.AppendLine("# Editor Settings");
            foreach(var prop in typeof(EditorSettings).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                sb.AppendLine("EditorSettings." + prop.Name + " = " + prop.GetValue(null,null) + ";");
            }
            sb.AppendLine();

            sb.AppendLine("# Graphics Settings");
            foreach(var prop in typeof(GraphicsSettings).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                sb.AppendLine("GraphicsSettings." + prop.Name + " = " + prop.GetValue(null,null) + ";");
            }
            sb.AppendLine();

            sb.AppendLine("# Tier Settings");
            sb.AppendLine("Active Tier: " + Graphics.activeTier);
            foreach(var prop in typeof(TierSettings).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                sb.AppendLine("TierSettings." + prop.Name + " = " + prop.GetValue(null,null) + ";");
            }
            sb.AppendLine();

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            #if UNITY_2021_3_OR_NEWER
                var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            #else
                var namedBuildTarget = buildTargetGroup;
                var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            #endif

            sb.AppendLine("# Player Settings");
            sb.AppendLine("Color Space: " + PlayerSettings.colorSpace.ToString());
            sb.AppendLine("Graphics APIs: ");
            foreach(var api in PlayerSettings.GetGraphicsAPIs(buildTarget))
            {
                sb.AppendLine("    " + api.ToString());
            }
            sb.AppendLine("Scripting Backend: " + PlayerSettings.GetScriptingBackend(namedBuildTarget));
            sb.AppendLine("Api Compatibility Level: " + PlayerSettings.GetApiCompatibilityLevel(namedBuildTarget));
            sb.AppendLine("C++ Compiler Configuration: " + PlayerSettings.GetIl2CppCompilerConfiguration(namedBuildTarget));
            sb.AppendLine("Scripting Define Symbols: " + scriptingDefineSymbols);
            sb.AppendLine();

            sb.AppendLine("# Quality Settings");
            foreach(var prop in typeof(QualitySettings).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                sb.AppendLine("QualitySettings." + prop.Name + " = " + prop.GetValue(null,null) + ";");
            }
            sb.AppendLine();

            sb.AppendLine("# SRP Information");
            if(GraphicsSettings.renderPipelineAsset != null)
            {
                sb.AppendLine("Current RP: " + GraphicsSettings.renderPipelineAsset.ToString());
            }
            else
            {
                sb.AppendLine("Current RP: " + "Built-in Render Pipeline");
            }
            string versionURP = ReadVersion("30648b8d550465f4bb77f1e1afd0b37d");
            if(versionURP != null) sb.AppendLine("URP: " + versionURP);
            string versionHDRP = ReadVersion("6f54db4299717fc4ca37866c6afa0905");
            if(versionHDRP != null) sb.AppendLine("HDRP: " + versionHDRP);
            sb.AppendLine();

            sb.AppendLine("# VRCSDK Information");
            #if UDON
                sb.AppendLine("UDON defined");
            #endif
            string versionVRCSDKBase = ReadVersion("1f872e4d36d785e409479da1c5fcde4c");
            if(versionVRCSDKBase != null) sb.AppendLine("VRChat SDK - Base: " + versionVRCSDKBase);
            string versionVRCSDKAvatars = ReadVersion("bd7510fb5fa478f43a81e9c74b72cb6f");
            if(versionVRCSDKAvatars != null) sb.AppendLine("VRChat SDK - Avatars: " + versionVRCSDKAvatars);
            string versionVRCSDKWorlds = ReadVersion("067f9b5cc16a52649985a5947e355556");
            if(versionVRCSDKWorlds != null) sb.AppendLine("VRChat SDK - Worlds: " + versionVRCSDKWorlds);
            string versionVRCSDKPath = AssetDatabase.GUIDToAssetPath("2cdbe2e71e2c46e48951c13df254e5b1");
            if(!string.IsNullOrEmpty(versionVRCSDKPath)) sb.AppendLine("VRChat SDK - Unitypackage: " + File.ReadAllText(versionVRCSDKPath));
            sb.AppendLine();

            sb.AppendLine("# CVRCCK Information");
            sb.AppendLine();

            sb.AppendLine("# GameObject Information");
            List<Material> materialList;
            List<AnimationClip> clipList;
            if(Selection.activeGameObject == null)
            {
                materialList = lilDirectoryManager.FindAssets<Material>("t:material").ToList();
                clipList = lilDirectoryManager.FindAssets<AnimationClip>("t:animationclip").ToList();
            }
            else
            {
                materialList = Selection.activeGameObject.GetComponentsInChildren<Renderer>(true).SelectMany(r => r.sharedMaterials).ToList();
                clipList = Selection.activeGameObject.GetComponentsInChildren<Animator>(true).Where(a => a.runtimeAnimatorController != null).SelectMany(a => a.runtimeAnimatorController.animationClips).ToList();
                var meshRenderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>(true);
                var skinnedMeshRenderers = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                var animators = Selection.activeGameObject.GetComponentsInChildren<Animator>(true);

                if(meshRenderers == null)   sb.AppendLine("MeshRenderer is not found");
                else                        sb.AppendLine("MeshRenderer Count: " + meshRenderers.Length);
                if(skinnedMeshRenderers == null)    sb.AppendLine("SkinnedMeshRenderer is not found");
                else                                sb.AppendLine("SkinnedMeshRenderer Count: " + skinnedMeshRenderers.Length);
                if(animators == null)   sb.AppendLine("Animator is not found");
                else                    sb.AppendLine("Animator Count: " + animators.Length);
            }

            if(materialsIn != null) materialList.AddRange(materialsIn);
            if(clipsIn != null) clipList.AddRange(clipsIn);

            var materials = materialList.ToArray();
            var clips = clipList.ToArray();

            if(materials == null)   sb.AppendLine("Material is not found");
            else                    sb.AppendLine("Material Count: " + materials.Length);
            if(clips == null)   sb.AppendLine("AnimationClip is not found");
            else                sb.AppendLine("AnimationClip Count: " + clips.Length);
            sb.AppendLine();

            string usedShaders, optimizedHLSL, shaderSettingText;
            lilToonSetting.GetOptimizedSetting(materials, clips, out usedShaders, out optimizedHLSL, out shaderSettingText);

            sb.AppendLine("# Shader List");
            if(!string.IsNullOrEmpty(usedShaders))  sb.AppendLine(usedShaders);
            else                                    sb.AppendLine("Shader is not found");
            sb.AppendLine();

            sb.AppendLine("# Shader Setting");
            if(!string.IsNullOrEmpty(shaderSettingText))    sb.AppendLine(shaderSettingText);
            else                                            sb.AppendLine("Shader setting is empty");
            sb.AppendLine();

            sb.AppendLine("# Optimized Input HLSL");
            if(!string.IsNullOrEmpty(optimizedHLSL))    sb.AppendLine(optimizedHLSL);
            else                                        sb.AppendLine("Optimization is failed");
            sb.AppendLine();

            string date = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
            string path = EditorUtility.SaveFilePanel("Save Bug Report", "", "lilToonBugReport-" + date, "txt");
            if(string.IsNullOrEmpty(path)) return;
            var sw = new StreamWriter(path, false);
            sw.Write(sb.ToString());
            sw.Close();
        }

        private static string ReadVersion(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if(!string.IsNullOrEmpty(path))
            {
                var package = JsonUtility.FromJson<PackageInfos>(File.ReadAllText(path));
                return package.version;
            }
            return null;
        }

        private class PackageInfos
        {
            public string version = "";
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

        public static string GetLoc(string value) { return lilLanguageManager.GetLoc(value); }
    }

#if LILTOON_DISABLE_ASSET_MODIFICATION == false
    public class lilToonBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 100; } }

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            lilToonSetting.SetShaderSettingBeforeBuild();
            EditorApplication.delayCall -= lilToonSetting.SetShaderSettingAfterBuild;
            EditorApplication.delayCall += lilToonSetting.SetShaderSettingAfterBuild;
        }

        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            lilToonSetting.SetShaderSettingAfterBuild();
        }
    }
#endif //LILTOON_DISABLE_ASSET_MODIFICATION
}
#endif
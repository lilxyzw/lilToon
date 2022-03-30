#if UNITY_EDITOR && UNITY_2019_4_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
    using UnityEditor.AssetImporters;
#else
    using UnityEditor.Experimental.AssetImporters;
#endif

namespace lilToon
{
    [ScriptedImporter(0, "lilcontainer")]
    public class lilShaderContainerImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            #if UNITY_2019_4_0 || UNITY_2019_4_1 || UNITY_2019_4_2 || UNITY_2019_4_3 || UNITY_2019_4_4 || UNITY_2019_4_5 || UNITY_2019_4_6 || UNITY_2019_4_7 || UNITY_2019_4_8 || UNITY_2019_4_9 || UNITY_2019_4_10
                Shader shader = ShaderUtil.CreateShaderAsset(lilShaderContainer.UnpackContainer(ctx.assetPath, ctx), false);
            #else
                Shader shader = ShaderUtil.CreateShaderAsset(ctx, lilShaderContainer.UnpackContainer(ctx.assetPath, ctx), false);
            #endif

            ctx.AddObjectToAsset("main obj", shader);
            ctx.SetMainObject(shader);
        }
    }

    [CustomEditor(typeof(lilShaderContainerImporter))]
    public class lilShaderContainerImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Export Shader"))
            {
                string assetPath = AssetDatabase.GetAssetPath(target);
                string shaderText = lilShaderContainer.UnpackContainer(assetPath, null);
                string exportPath = EditorUtility.SaveFilePanel("Export Shader", Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath), "shader");
                if(string.IsNullOrEmpty(exportPath)) return;
                File.WriteAllText(exportPath, shaderText);
            }
            ApplyRevertGUI();
        }
    }

    public class lilShaderContainerAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach(string path in importedAssets)
            {
                if(!path.EndsWith("lilcontainer", StringComparison.InvariantCultureIgnoreCase)) continue;

                var mainobj = AssetDatabase.LoadMainAssetAtPath(path);
                if(mainobj is Shader) ShaderUtil.RegisterShader((Shader)mainobj);

                foreach(var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                {
                    if(obj is Shader) ShaderUtil.RegisterShader((Shader)obj);
                }
            }
        }
    }

    public class lilShaderContainer
    {
        private const string MULTI_COMPILE_FORWARD          = "#pragma lil_multi_compile_forward";
        private const string MULTI_COMPILE_FORWARDADD       = "#pragma lil_multi_compile_forwardadd";
        private const string MULTI_COMPILE_SHADOWCASTER     = "#pragma lil_multi_compile_shadowcaster";
        private const string MULTI_COMPILE_DEPTHONLY        = "#pragma lil_multi_compile_depthonly";
        private const string MULTI_COMPILE_DEPTHNORMALS     = "#pragma lil_multi_compile_depthnormals";
        private const string MULTI_COMPILE_MOTIONVECTORS    = "#pragma lil_multi_compile_motionvectors";
        private const string MULTI_COMPILE_SCENESELECTION   = "#pragma lil_multi_compile_sceneselection";
        private const string MULTI_COMPILE_META             = "#pragma lil_multi_compile_meta";
        private const string MULTI_COMPILE_INSTANCING       = "#pragma multi_compile_instancing";
        private const string SKIP_VARIANTS_SHADOWS          = "#pragma lil_skip_variants_shadows";
        private const string SKIP_VARIANTS_LIGHTMAPS        = "#pragma lil_skip_variants_lightmaps";
        private const string SKIP_VARIANTS_DECALS           = "#pragma lil_skip_variants_decals";
        private const string SKIP_VARIANTS_ADDLIGHT         = "#pragma lil_skip_variants_addlight";
        private const string SKIP_VARIANTS_ADDLIGHTSHADOWS  = "#pragma lil_skip_variants_addlightshadows";
        private const string SKIP_VARIANTS_PROBEVOLUMES     = "#pragma lil_skip_variants_probevolumes";
        private const string SKIP_VARIANTS_AO               = "#pragma lil_skip_variants_ao";
        private const string SKIP_VARIANTS_LIGHTLISTS       = "#pragma lil_skip_variants_lightlists";
        private const string SKIP_VARIANTS_REFLECTIONS      = "#pragma lil_skip_variants_reflections";

        private const string LIL_SHADER_NAME                = "*LIL_SHADER_NAME*";
        private const string LIL_EDITOR_NAME                = "*LIL_EDITOR_NAME*";
        private const string LIL_SUBSHADER_INSERT           = "*LIL_SUBSHADER_INSERT*";
        private const string LIL_SHADER_SETTING             = "*LIL_SHADER_SETTING*";
        private const string LIL_PASS_SHADER_NAME           = "*LIL_PASS_SHADER_NAME*";
        private const string LIL_SUBSHADER_TAGS             = "*LIL_SUBSHADER_TAGS*";
        private const string LIL_DOTS_SM_TAGS               = "*LIL_DOTS_SM_TAGS*";
        private const string LIL_DOTS_SM_4_5                = "*LIL_DOTS_SM_4_5*";
        private const string LIL_DOTS_SM_4_5_OR_3_5         = "*LIL_DOTS_SM_4_5_OR_3_5*";
        private const string LIL_INSERT_PASS_PRE            = "*LIL_INSERT_PASS_PRE*";
        private const string LIL_INSERT_PASS_POST           = "*LIL_INSERT_PASS_POST*";
        private const string LIL_INSERT_USEPASS_PRE         = "*LIL_INSERT_USEPASS_PRE*";
        private const string LIL_INSERT_USEPASS_POST        = "*LIL_INSERT_USEPASS_POST*";

        private const string csdShaderNameTag                   = "ShaderName";
        private const string csdEditorNameTag                   = "EditorName";
        private const string csdReplaceTag                      = "Replace";
        private const string csdInsertPassPreTag                = "InsertPassPre";
        private const string csdInsertPassPostTag               = "InsertPassPost";
        private const string csdInsertUsePassPreTag             = "InsertUsePassPre";
        private const string csdInsertUsePassPostTag            = "InsertUsePassPost";

        private static Dictionary<string, string> replaces      = new Dictionary<string, string>();

        private const string customShaderDataFile               = "lilCustomShaderDatas.lilblock";
        private const string customShaderResourcesFolderGUID    = "1acd4e79a7d2c6c44aa8b97a1e33f20b"; // "Assets/lilToon/CustomShaderResources"
        private static string GetCustomShaderResourcesFolderPath() { return AssetDatabase.GUIDToAssetPath(customShaderResourcesFolderGUID); }

        private static string passShaderName = "";
        private static string subShaderTags = "";
        private static string insertText = "";
        private static string shaderSettingText = "";
        private static string resourcesFolderPath = "";
        private static string assetFolderPath = "";
        private static string shaderLibsPath = "";
        private static string assetName = "";
        private static string shaderName = "";
        private static string editorName = "";
        private static string origShaderName = "";
        private static bool isOrigShaderNameLoaded = false;

        private static string insertPassPre = "";
        private static string insertPassPost = "";
        private static string insertUsePassPre = "";
        private static string insertUsePassPost = "";

        private static PackageVersionInfos version = new PackageVersionInfos();
        private static int indent = 12;

        public static string UnpackContainer(string assetPath, AssetImportContext ctx)
        {
            passShaderName = "";
            subShaderTags = "";
            insertText = "";
            resourcesFolderPath = GetCustomShaderResourcesFolderPath() + "/";
            assetFolderPath = Path.GetDirectoryName(assetPath) + "/";
            shaderLibsPath = lilToonInspector.GetShaderFolderPath() + "/Includes";
            assetName = Path.GetFileName(assetPath);
            shaderSettingText = lilToonInspector.BuildShaderSettingString(false).Replace("\r\n", "\r\n            ");
            shaderName = "";
            editorName = "";
            origShaderName = "";
            insertPassPre = "";
            insertPassPost = "";
            insertUsePassPre = "";
            insertUsePassPost = "";
            isOrigShaderNameLoaded = false;
            replaces = new Dictionary<string, string>();

            StringBuilder sb;

            version = new PackageVersionInfos()
            {
                RP = RenderPipeline.BRP,
                Major = 12,
                Minor = 0,
                Patch = 0
            };

            string renderPipelineName = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset?.ToString() ?? "";
            if(renderPipelineName.Contains("Universal"))
            {
                version = GetURPVersion();
            }
            else if(renderPipelineName.Contains("HDRenderPipeline"))
            {
                version = GetHDRPVersion();
            }

            switch(version.RP)
            {
                case RenderPipeline.BRP:
                    sb = ReadContainerFile(assetPath, "BRP", ctx);
                    ReplaceMultiCompiles(ref sb, version, indent, false);
                    break;
                case RenderPipeline.URP:
                    sb = ReadContainerFile(assetPath, "URP", ctx);
                    break;
                case RenderPipeline.HDRP:
                    sb = ReadContainerFile(assetPath, "HDRP", ctx);
                    ReplaceMultiCompiles(ref sb, version, indent, false);
                    subShaderTags.Replace("\"RenderType\" = \"Opaque\"",            "\"RenderType\" = \"HDLitShader\"");
                    subShaderTags.Replace("\"RenderType\" = \"Transparent\"",       "\"RenderType\" = \"HDLitShader\"");
                    subShaderTags.Replace("\"RenderType\" = \"TransparentCutout\"", "\"RenderType\" = \"HDLitShader\"");
                    subShaderTags.Replace("\"Queue\" = \"AlphaTest+10\"",           "\"Queue\" = \"Transparent\"");
                    subShaderTags.Replace("\"Queue\" = \"AlphaTest+55\"",           "\"Queue\" = \"Transparent\"");
                    subShaderTags.Replace("\"Queue\" = \"Transparent-100\"",        "\"Queue\" = \"Transparent\"");
                    break;
                default:
                    sb = ReadContainerFile(assetPath, "BRP", ctx);
                    ReplaceMultiCompiles(ref sb, version, indent, false);
                    break;
            }

            ReadDataFile(ctx);
            ReplaceMultiCompiles(ref insertPassPre, version, indent, false);
            ReplaceMultiCompiles(ref insertPassPost, version, indent, false);
            ReplaceMultiCompiles(ref insertUsePassPre, version, indent, false);
            ReplaceMultiCompiles(ref insertUsePassPost, version, indent, false);
            sb.Replace(LIL_INSERT_PASS_PRE,     insertPassPre);
            sb.Replace(LIL_INSERT_PASS_POST,    insertPassPost);
            sb.Replace(LIL_INSERT_USEPASS_PRE,  insertUsePassPre);
            sb.Replace(LIL_INSERT_USEPASS_POST, insertUsePassPost);

            sb.Replace("\"Includes",            "\"" + shaderLibsPath);
            sb.Replace(LIL_SUBSHADER_INSERT,    insertText);
            sb.Replace(LIL_SHADER_SETTING,      shaderSettingText);
            sb.Replace(LIL_PASS_SHADER_NAME,    passShaderName);
            sb.Replace(LIL_SUBSHADER_TAGS,      subShaderTags);

            sb.Replace(LIL_SHADER_NAME,         shaderName);
            sb.Replace(LIL_EDITOR_NAME,         editorName);

            sb.Replace(SKIP_VARIANTS_SHADOWS,           GetSkipVariantsShadows());
            sb.Replace(SKIP_VARIANTS_LIGHTMAPS,         GetSkipVariantsLightmaps());
            sb.Replace(SKIP_VARIANTS_DECALS,            GetSkipVariantsDecals());
            sb.Replace(SKIP_VARIANTS_ADDLIGHT,          GetSkipVariantsAddLight());
            sb.Replace(SKIP_VARIANTS_ADDLIGHTSHADOWS,   GetSkipVariantsAddLightShadows());
            sb.Replace(SKIP_VARIANTS_PROBEVOLUMES,      GetSkipVariantsProbeVolumes());
            sb.Replace(SKIP_VARIANTS_AO,                GetSkipVariantsAO());
            sb.Replace(SKIP_VARIANTS_LIGHTLISTS,        GetSkipVariantsLightLists());
            sb.Replace(SKIP_VARIANTS_REFLECTIONS,       GetSkipVariantsReflections());

            foreach(KeyValuePair<string,string> replace in replaces)
            {
                sb.Replace(replace.Key, replace.Value);
            }

            FixIncludeForOldUnity(ref sb);

            return sb.ToString();
        }

        private static void ReadDataFile(AssetImportContext ctx)
        {
            string path = assetFolderPath + customShaderDataFile;

            if(!File.Exists(path))
            {
                Debug.LogWarning("[" + assetName + "] " + "File not found: " + path);
                return;
            }
            ctx?.DependsOnSourceAsset(path);
            StreamReader sr = new StreamReader(path);
            string line = "";

            while((line = sr.ReadLine()) != null)
            {
                if(line.Contains(csdShaderNameTag))
                {
                    shaderName = GetTags(line);
                    origShaderName = origShaderName.Replace(LIL_SHADER_NAME, shaderName);
                    continue;
                }
                if(line.Contains(csdEditorNameTag))
                {
                    editorName = GetTags(line);
                    continue;
                }
                if(line.Contains(csdReplaceTag))
                {
                    GetReplaces(line);
                    continue;
                }
                if(line.Contains("Insert"))
                {
                    GetInsert(line, ctx);
                    continue;
                }
            }

            sr.Close();
        }

        private static string GetTags(string line)
        {
            int first = line.IndexOf('"') + 1;
            int second = line.IndexOf('"', first);
            return line.Substring(first, second - first);
        }

        private static void GetReplaces(string line)
        {
            int first = line.IndexOf('"') + 1;
            int second = line.IndexOf('"', first);
            int third = line.IndexOf('"', second + 1) + 1;
            int fourth = line.IndexOf('"', third);
            string from = line.Substring(first, second - first);
            string to = line.Substring(third, fourth - third);
            replaces[from] = to;
        }

        private static void GetInsert(string line, AssetImportContext ctx)
        {
            string rpname = "BRP";
            if(version.RP == RenderPipeline.URP) rpname = "URP";
            if(version.RP == RenderPipeline.HDRP) rpname = "HDRP";
            if(line.Contains(csdInsertPassPreTag))
            {
                GetInsert(ref insertPassPre, line, rpname, ctx);
            }
            if(line.Contains(csdInsertPassPostTag))
            {
                GetInsert(ref insertPassPost, line, rpname, ctx);
            }
            if(line.Contains(csdInsertUsePassPreTag))
            {
                GetInsert(ref insertUsePassPre, line, rpname, ctx);
            }
            if(line.Contains(csdInsertUsePassPostTag))
            {
                GetInsert(ref insertUsePassPost, line, rpname, ctx);
            }
        }

        private static void GetInsert(ref string insertPass, string line, string rpname, AssetImportContext ctx)
        {
            string subpath = "";
            int first = line.IndexOf('"') + 1;
            int second = line.IndexOf('"', first);
            int third = line.IndexOf('"', second + 1) + 1;
            if(third > 1)
            {
                int fourth = line.IndexOf('"', third);
                string name = line.Substring(first, second - first);
                if(name.StartsWith("!"))
                {
                    if(origShaderName.Contains(name.Substring(1)))
                    {
                        return;
                    }
                }
                else if(!origShaderName.Contains(name))
                {
                    return;
                }
                subpath = line.Substring(third, fourth - third);
            }
            else
            {
                subpath = line.Substring(first, second - first);
            }

            // for render pipeline
            string pathForRP = assetFolderPath + Path.GetFileNameWithoutExtension(subpath) + rpname + Path.GetExtension(subpath);
            if(File.Exists(pathForRP))
            {
                ctx?.DependsOnSourceAsset(pathForRP);
                insertPass = ReadTextFile(pathForRP);
                return;
            }

            // normal
            subpath = assetFolderPath + subpath;
            if(!File.Exists(subpath))
            {
                Debug.LogWarning("[" + assetName + "] " + "File not found: " + subpath);
                return;
            }

            ctx?.DependsOnSourceAsset(subpath);
            insertPass = ReadTextFile(subpath);
        }

        private static StringBuilder ReadContainerFile(string path, string rpname, AssetImportContext ctx)
        {
            StringBuilder sb = new StringBuilder();
            StreamReader sr = new StreamReader(path);
            string line = "";

            while((line = sr.ReadLine()) != null)
            {
                if(!isOrigShaderNameLoaded && line.StartsWith("Shader"))
                {
                    int first = line.IndexOf('"') + 1;
                    int second = line.IndexOf('"', first);
                    if(line.Substring(0, first).Contains("//"))
                    {
                        sb.AppendLine(line);
                        continue;
                    }

                    origShaderName = line.Substring(first, second - first);
                    isOrigShaderNameLoaded = true;
                }
                if(line.Contains("lil"))
                {
                    if(line.Contains("lilSkipSettings"))
                    {
                        if(line.Substring(0, line.IndexOf("lilSkipSettings") + 1).Contains("//"))
                        {
                            sb.AppendLine(line);
                            continue;
                        }

                        shaderSettingText = "";
                        continue;
                    }
                    if(line.Contains("lilProperties"))
                    {
                        GetProperties(path, rpname, ctx, sb, line);
                        continue;
                    }
                    if(line.Contains("lilSubShader"))
                    {
                        if(line.Contains(rpname))
                        {
                            GetSubShader(path, rpname, ctx, sb, line);
                        }
                        if(line.Contains("lilSubShaderInsert"))
                        {
                            GetSubShaderInsert(path, rpname, ctx, sb, line);
                        }
                        if(line.Contains("lilSubShaderTags"))
                        {
                            GetSubShaderTags(path, rpname, ctx, sb, line);
                        }
                        continue;
                    }
                    if(line.Contains("lilPassShaderName"))
                    {
                        GetPassShaderName(path, rpname, ctx, sb, line);
                        continue;
                    }
                }
                sb.AppendLine(line);
            }

            sr.Close();
            return sb;
        }
    
        private static void GetSubShader(string path, string rpname, AssetImportContext ctx, StringBuilder sb, string line)
        {
            int first = line.IndexOf('"') + 1;
            int second = line.IndexOf('"', first);
            if(line.Substring(0, first).Contains("//"))
            {
                sb.AppendLine(line);
                return;
            }

            string subpath = line.Substring(first, second - first);
            if(subpath.Contains("Default") && !subpath.Contains(".lilblock"))
            {
                subpath = resourcesFolderPath + rpname + "/" + subpath + ".lilblock";
            }
            else
            {
                subpath = assetFolderPath + subpath;
            }

            if(!File.Exists(subpath))
            {
                Debug.LogWarning("[" + assetName + "] " + "File not found: " + subpath);
                return;
            }
            ctx?.DependsOnSourceAsset(subpath);

            if(rpname == "URP" && !subpath.Contains("UsePass"))
            {
                StringBuilder sb1 = new StringBuilder(ReadTextFile(subpath));
                StringBuilder sb2 = new StringBuilder(sb1.ToString());

                sb1.Replace(LIL_DOTS_SM_TAGS, " \"ShaderModel\" = \"4.5\"");
                sb1.Replace(LIL_DOTS_SM_4_5, "#pragma target 4.5\r\n            #pragma exclude_renderers gles gles3 glcore");
                sb1.Replace(LIL_DOTS_SM_4_5_OR_3_5, "#pragma target 4.5\r\n            #pragma exclude_renderers gles gles3 glcore");
                ReplaceMultiCompiles(ref sb1, version, indent, true);
                sb.AppendLine(sb1.ToString());
                sb.AppendLine();

                sb2.Replace(LIL_DOTS_SM_TAGS, "");
                sb2.Replace(LIL_DOTS_SM_4_5, "#pragma only_renderers gles gles3 glcore d3d11");
                sb2.Replace(LIL_DOTS_SM_4_5_OR_3_5, "#pragma target 3.5\r\n            #pragma only_renderers gles gles3 glcore d3d11");
                ReplaceMultiCompiles(ref sb2, version, indent, false);
                sb.AppendLine(sb2.ToString());
            }
            else
            {
                sb.AppendLine(ReadTextFile(subpath));
            }
        }
    
        private static void GetSubShaderInsert(string path, string rpname, AssetImportContext ctx, StringBuilder sb, string line)
        {
            int first = line.IndexOf('"') + 1;
            int second = line.IndexOf('"', first);
            if(line.Substring(0, first).Contains("//"))
            {
                sb.AppendLine(line);
                return;
            }

            string subpath = assetFolderPath + line.Substring(first, second - first);
            if(!File.Exists(subpath))
            {
                Debug.LogWarning("[" + assetName + "] " + "File not found: " + subpath);
                return;
            }
            ctx?.DependsOnSourceAsset(subpath);
            insertText = ReadTextFile(subpath);
        }
    
        private static void GetProperties(string path, string rpname, AssetImportContext ctx, StringBuilder sb, string line)
        {
            int first = line.IndexOf('"') + 1;
            int second = line.IndexOf('"', first);
            if(line.Substring(0, first).Contains("//"))
            {
                sb.AppendLine(line);
                return;
            }

            string subpath = line.Substring(first, second - first);
            if(subpath.Contains("Default") && !subpath.Contains(".lilblock"))
            {
                subpath = resourcesFolderPath + "Properties/" + subpath + ".lilblock";
            }
            else
            {
                subpath = assetFolderPath + subpath;
            }

            if(!File.Exists(subpath))
            {
                Debug.LogWarning("[" + assetName + "] " + "File not found: " + subpath);
                return;
            }
            ctx?.DependsOnSourceAsset(subpath);
            sb.AppendLine(ReadTextFile(subpath));
        }

        private static void GetPassShaderName(string path, string rpname, AssetImportContext ctx, StringBuilder sb, string line)
        {
            int first = line.IndexOf('"') + 1;
            int second = line.IndexOf('"', first);
            if(line.Substring(0, first).Contains("//"))
            {
                sb.AppendLine(line);
                return;
            }

            passShaderName = line.Substring(first, second - first);
        }

        private static void GetSubShaderTags(string path, string rpname, AssetImportContext ctx, StringBuilder sb, string line)
        {
            int first = line.IndexOf('{') + 1;
            int second = line.IndexOf('}', first);
            if(line.Substring(0, first).Contains("//"))
            {
                sb.AppendLine(line);
                return;
            }

            subShaderTags = line.Substring(first, second - first);
        }

        private static string ReadTextFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string text = sr.ReadToEnd();
            sr.Close();
            return text;
        }

        private static void FixIncludeForOldUnity(ref StringBuilder sb)
        {
            #if UNITY_2019_4_0 || UNITY_2019_4_1 || UNITY_2019_4_2 || UNITY_2019_4_3 || UNITY_2019_4_4 || UNITY_2019_4_5 || UNITY_2019_4_6 || UNITY_2019_4_7 || UNITY_2019_4_8 || UNITY_2019_4_9 || UNITY_2019_4_10
                string additionalPath = assetFolderPath.Replace("\\", "/");
                char[] escapes = Environment.NewLine.ToCharArray();
                string[] text = sb.ToString().Split(escapes[0]);
                sb.Clear();

                if(escapes.Length >= 1)
                {
                    foreach(char escape in escapes)
                    {
                        string escapeStr = escape.ToString();
                        for(int i = 0; i < text.Length; i++)
                        {
                            text[i] = text[i].Replace(escapeStr, "");
                        }
                    }
                }

                foreach(string line in text)
                {
                    if(line.Contains("#include \"") && !line.Contains("\"Assets/") && !line.Contains("\"Packages/"))
                    {
                        sb.AppendLine(line.Replace("#include \"", "#include \"" + additionalPath));
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
            #endif
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Utils
        private static string GetIndent(int indent)
        {
            return new string(' ', indent);
        }

        private static string GenerateIndentText(int indent, params string[] texts)
        {
            string ind = "\r\n" + GetIndent(indent);
            return string.Join(ind, texts);
        }

        private static string GetRelativePath(string fromPath, string toPath)
        {
            Uri fromUri = new Uri(Path.GetFullPath(fromPath));
            Uri toUri = new Uri(Path.GetFullPath(toPath));

            string relativePath = Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, '/');

            return relativePath;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Render Pipeline
        private static PackageVersionInfos GetURPVersion()
        {
            PackageVersionInfos version = ReadVersion("30648b8d550465f4bb77f1e1afd0b37d");
            version.RP = RenderPipeline.URP;
            return version;
        }

        private static PackageVersionInfos GetHDRPVersion()
        {
            PackageVersionInfos version = ReadVersion("6f54db4299717fc4ca37866c6afa0905");
            version.RP = RenderPipeline.HDRP;
            return version;
        }

        private static PackageVersionInfos ReadVersion(string guid)
        {
            string version = "";
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if(!string.IsNullOrEmpty(path))
            {
                PackageInfos package = JsonUtility.FromJson<PackageInfos>(File.ReadAllText(path));
                version = package.version;
            }

            PackageVersionInfos infos;
            infos.RP = RenderPipeline.BRP;
            if(string.IsNullOrEmpty(version))
            {
                infos.Major = 12;
                infos.Minor = 0;
                infos.Patch = 0;
            }
            else
            {
                string[] parts = version.Split('.');
                infos.Major = int.Parse(parts[0]);
                infos.Minor = int.Parse(parts[1]);
                infos.Patch = int.Parse(parts[2]);
            }
            return infos;
        }

        private enum RenderPipeline
        {
            BRP,
            URP,
            HDRP
        }

        private struct PackageVersionInfos
        {
            public RenderPipeline RP;
            public int Major;
            public int Minor;
            public int Patch;
        }

        private class PackageInfos
        {
            public string version;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Multi Compile
        private static void ReplaceMultiCompiles(ref StringBuilder sb, PackageVersionInfos version, int indent, bool isDots)
        {
            sb.Replace(MULTI_COMPILE_FORWARDADD, GetMultiCompileForwardAdd(version, indent));
            sb.Replace(MULTI_COMPILE_FORWARD, GetMultiCompileForward(version, indent));
            sb.Replace(MULTI_COMPILE_SHADOWCASTER, GetMultiCompileShadowCaster(version, indent));
            sb.Replace(MULTI_COMPILE_DEPTHONLY, GetMultiCompileDepthOnly(version, indent));
            sb.Replace(MULTI_COMPILE_DEPTHNORMALS, GetMultiCompileDepthNormals(version, indent));
            sb.Replace(MULTI_COMPILE_MOTIONVECTORS, GetMultiCompileMotionVectors(version, indent));
            sb.Replace(MULTI_COMPILE_SCENESELECTION, GetMultiCompileSceneSelection(version, indent));
            sb.Replace(MULTI_COMPILE_META, GetMultiCompileMeta(version, indent));
            sb.Replace(MULTI_COMPILE_INSTANCING, GetMultiCompileInstancingLayer(version, indent, isDots));
        }

        private static void ReplaceMultiCompiles(ref string sb, PackageVersionInfos version, int indent, bool isDots)
        {
            sb = sb.Replace(MULTI_COMPILE_FORWARDADD, GetMultiCompileForwardAdd(version, indent))
                   .Replace(MULTI_COMPILE_FORWARD, GetMultiCompileForward(version, indent))
                   .Replace(MULTI_COMPILE_SHADOWCASTER, GetMultiCompileShadowCaster(version, indent))
                   .Replace(MULTI_COMPILE_DEPTHONLY, GetMultiCompileDepthOnly(version, indent))
                   .Replace(MULTI_COMPILE_DEPTHNORMALS, GetMultiCompileDepthNormals(version, indent))
                   .Replace(MULTI_COMPILE_MOTIONVECTORS, GetMultiCompileMotionVectors(version, indent))
                   .Replace(MULTI_COMPILE_SCENESELECTION, GetMultiCompileSceneSelection(version, indent))
                   .Replace(MULTI_COMPILE_META, GetMultiCompileMeta(version, indent))
                   .Replace(MULTI_COMPILE_INSTANCING, GetMultiCompileInstancingLayer(version, indent, isDots));
        }

        private static string GetMultiCompileForward(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                if(version.Major >= 12)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN",
                        "#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                        "#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS",
                        "#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING",
                        "#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION",
                        "#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                        "#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION",
                        "#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3",
                        "#pragma multi_compile_fragment _ _LIGHT_LAYERS",
                        "#pragma multi_compile_fragment _ _LIGHT_COOKIES",
                        "#pragma multi_compile _ _CLUSTERED_RENDERING",
                        "#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING",
                        "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                        "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                        "#pragma multi_compile_instancing",
                        "#define LIL_PASS_FORWARD");
                }
                if(version.Major >= 11)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN",
                        "#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                        "#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS",
                        "#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                        "#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION",
                        "#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING",
                        "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                        "#pragma multi_compile_instancing",
                        "#define LIL_PASS_FORWARD");
                }
                if(version.Major >= 10)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE",
                        "#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                        "#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS",
                        "#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                        "#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION",
                        "#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING",
                        "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                        "#pragma multi_compile_instancing",
                        "#define LIL_PASS_FORWARD");
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE",
                        "#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                        "#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS",
                        "#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                        "#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                        "#pragma multi_compile_instancing",
                        "#define LIL_PASS_FORWARD");
                }
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                if(version.Major >= 12)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                        "#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2",
                        "#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON",
                        "#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT",
                        "#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT",
                        "#pragma multi_compile_fragment SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH",
                        "#pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_FORWARD",
                        "#define HAS_LIGHTLOOP",
                        "#define LIL_PASS_FORWARD");
                }
                if(version.Major >= 10)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                        "#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON",
                        "#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT",
                        "#pragma multi_compile_fragment SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH",
                        "#pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_FORWARD",
                        "#define HAS_LIGHTLOOP",
                        "#define LIL_PASS_FORWARD");
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                        "#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT",
                        "#pragma multi_compile_fragment SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH",
                        "#pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_FORWARD",
                        "#define HAS_LIGHTLOOP",
                        "#define LIL_PASS_FORWARD");
                }
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_fwdbase",
                    "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_FORWARD");
            }
        }

        private static string GetMultiCompileForwardAdd(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_FORWARDADD");
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_FORWARDADD");
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_fragment POINT DIRECTIONAL SPOT POINT_COOKIE DIRECTIONAL_COOKIE",
                    "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_FORWARDADD");
            }
        }

        private static string GetMultiCompileShadowCaster(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                if(version.Major >= 11)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW",
                        "#pragma multi_compile_instancing",
                        "#define LIL_PASS_SHADOWCASTER");
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_instancing",
                        "#define LIL_PASS_SHADOWCASTER");
                }
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define SHADERPASS SHADERPASS_SHADOWS",
                    "#define LIL_PASS_SHADOWCASTER");
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_shadowcaster",
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_SHADOWCASTER");
            }
        }

        private static string GetMultiCompileDepthOnly(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_DEPTHONLY");
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                if(version.Major >= 10)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ WRITE_NORMAL_BUFFER",
                        "#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH",
                        "#pragma multi_compile _ WRITE_DECAL_BUFFER",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_DEPTH_ONLY",
                        "#define LIL_PASS_DEPTHONLY");
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ WRITE_NORMAL_BUFFER",
                        "#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_DEPTH_ONLY",
                        "#define LIL_PASS_DEPTHONLY");
                }
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_DEPTHONLY");
            }
        }

        private static string GetMultiCompileDepthNormals(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_DEPTHNORMALS");
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_DEPTHNORMALS");
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_DEPTHNORMALS");
            }
        }

        private static string GetMultiCompileMotionVectors(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_MOTIONVECTORS");
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                if(version.Major >= 10)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ WRITE_NORMAL_BUFFER",
                        "#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH",
                        "#pragma multi_compile _ WRITE_DECAL_BUFFER",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_MOTION_VECTORS",
                        "#define LIL_PASS_MOTIONVECTORS");
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ WRITE_NORMAL_BUFFER",
                        "#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_MOTION_VECTORS",
                        "#define LIL_PASS_MOTIONVECTORS");
                }
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_MOTIONVECTORS");
            }
        }

        private static string GetMultiCompileSceneSelection(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_SCENESELECTION");
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#pragma editor_sync_compilation",
                    "#define SHADERPASS SHADERPASS_DEPTH_ONLY",
                    "#define SCENESELECTIONPASS",
                    "#define LIL_PASS_SCENESELECTION");
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define LIL_PASS_SCENESELECTION");
            }
        }

        private static string GetMultiCompileMeta(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma shader_feature EDITOR_VISUALIZATION",
                    "#define LIL_PASS_META");
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#pragma shader_feature EDITOR_VISUALIZATION",
                    "#define SHADERPASS SHADERPASS_LIGHT_TRANSPORT",
                    "#define LIL_PASS_META");
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma shader_feature EDITOR_VISUALIZATION",
                    "#define LIL_PASS_META");
            }
        }

        private static string GetMultiCompileInstancingLayer(PackageVersionInfos version, int indent, bool isDots = false)
        {
            if(version.RP == RenderPipeline.URP)
            {
                if(version.Major >= 12)
                {
                    if(isDots)
                    {
                        return GenerateIndentText(indent,
                            "#pragma multi_compile _ DOTS_INSTANCING_ON",
                            "#pragma multi_compile_instancing",
                            "#pragma instancing_options renderinglayer");
                    }
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_instancing",
                        "#pragma instancing_options renderinglayer");
                }
                else if(version.Major >= 9)
                {
                    if(isDots)
                    {
                        return GenerateIndentText(indent,
                            "#pragma multi_compile _ DOTS_INSTANCING_ON",
                            "#pragma multi_compile_instancing");
                    }
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_instancing");
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_instancing");
                }
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                if(version.Major >= 9 && isDots)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ DOTS_INSTANCING_ON",
                        "#pragma multi_compile_instancing",
                        "#pragma instancing_options renderinglayer");
                }
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#pragma instancing_options renderinglayer");
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Skip Variants
        private static string GetSkipVariantsShadows()
        {
            return "#pragma skip_variants SHADOWS_SCREEN _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN _ADDITIONAL_LIGHT_SHADOWS SCREEN_SPACE_SHADOWS_ON SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH";
        }

        private static string GetSkipVariantsLightmaps()
        {
            return "#pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK DIRLIGHTMAP_COMBINED _MIXED_LIGHTING_SUBTRACTIVE";
        }

        private static string GetSkipVariantsDecals()
        {
            return "#pragma skip_variants DECALS_OFF DECALS_3RT DECALS_4RT DECAL_SURFACE_GRADIENT _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3";
        }

        private static string GetSkipVariantsAddLight()
        {
            return "#pragma skip_variants VERTEXLIGHT_ON _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS";
        }

        private static string GetSkipVariantsAddLightShadows()
        {
            return "#pragma skip_variants _ADDITIONAL_LIGHT_SHADOWS";
        }

        private static string GetSkipVariantsProbeVolumes()
        {
            return "#pragma skip_variants PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2";
        }

        private static string GetSkipVariantsAO()
        {
            return "#pragma skip_variants _SCREEN_SPACE_OCCLUSION";
        }

        private static string GetSkipVariantsLightLists()
        {
            return "#pragma skip_variants USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST";
        }

        private static string GetSkipVariantsReflections()
        {
            return "#pragma skip_variants _REFLECTION_PROBE_BLENDING _REFLECTION_PROBE_BOX_PROJECTION";
        }
    }
}
#endif
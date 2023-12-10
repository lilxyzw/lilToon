#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace lilToon
{
    public class lilRenderPipelineReader
    {
        public static lilRenderPipeline GetRP()
        {
            // Render Pipeline
            // BRP : null
            // LWRP : LightweightPipeline.LightweightRenderPipelineAsset
            // URP : Universal.UniversalRenderPipelineAsset
            // HDRP : HighDefinition.HDRenderPipelineAsset
            string renderPipelineName = "";
            #if UNITY_2019_3_OR_NEWER
                if(GraphicsSettings.currentRenderPipeline != null)
                {
                    renderPipelineName = GraphicsSettings.currentRenderPipeline.ToString();
                }
            #else
                if(GraphicsSettings.renderPipelineAsset != null)
                {
                    renderPipelineName = GraphicsSettings.renderPipelineAsset.ToString();
                }
            #endif
            if(renderPipelineName.Contains("Universal"))
            {
                return lilRenderPipeline.URP;
            }
            else if(renderPipelineName.Contains("Lightweight"))
            {
                return lilRenderPipeline.LWRP;
            }
            else if(renderPipelineName.Contains("HDRenderPipeline"))
            {
                return lilRenderPipeline.HDRP;
            }
            return lilRenderPipeline.BRP;
        }

        public static PackageVersionInfos GetRPInfos()
        {
            string renderPipelineName = "";
            #if UNITY_2019_3_OR_NEWER
                if(GraphicsSettings.currentRenderPipeline != null)
                {
                    renderPipelineName = GraphicsSettings.currentRenderPipeline.ToString();
                }
            #else
                if(GraphicsSettings.renderPipelineAsset != null)
                {
                    renderPipelineName = GraphicsSettings.renderPipelineAsset.ToString();
                }
            #endif
            if(renderPipelineName.Contains("Universal"))
            {
                return GetURPVersion();
            }
            else if(renderPipelineName.Contains("Lightweight"))
            {
                return GetLWRPVersion();
            }
            else if(renderPipelineName.Contains("HDRenderPipeline"))
            {
                return GetHDRPVersion();
            }
            return new PackageVersionInfos()
            {
                RP = lilRenderPipeline.BRP,
                Major = 0,
                Minor = 0,
                Patch = 0
            };
        }

        private static PackageVersionInfos GetURPVersion()
        {
            string path = AssetDatabase.GUIDToAssetPath("30648b8d550465f4bb77f1e1afd0b37d");
            var package = JsonUtility.FromJson<PackageInfos>(File.ReadAllText(path));
            string guid =
                package.displayName.Contains("SLZ") ?
                "753d1ac2429a21a44ac5f937cbbb409f" : // Core
                "30648b8d550465f4bb77f1e1afd0b37d";  // URP
            var version = ReadVersion(guid);
            version.RP = lilRenderPipeline.URP;
            return version;
        }

        private static PackageVersionInfos GetLWRPVersion()
        {
            var version = ReadVersion("30648b8d550465f4bb77f1e1afd0b37d");
            version.RP = lilRenderPipeline.LWRP;
            return version;
        }

        private static PackageVersionInfos GetHDRPVersion()
        {
            var version = ReadVersion("6f54db4299717fc4ca37866c6afa0905");
            version.RP = lilRenderPipeline.HDRP;
            return version;
        }

        private static PackageVersionInfos ReadVersion(string guid)
        {
            string version = "";
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if(!string.IsNullOrEmpty(path))
            {
                var package = JsonUtility.FromJson<PackageInfos>(File.ReadAllText(path));
                version = package.version;
            }

            PackageVersionInfos infos;
            infos.RP = lilRenderPipeline.BRP;
            if(string.IsNullOrEmpty(version))
            {
                infos.Major = 0;
                infos.Minor = 0;
                infos.Patch = 0;
            }
            else
            {
                var parser = new SemVerParser(version);
                infos.Major = parser.major;
                infos.Minor = parser.minor;
                infos.Patch = parser.patch;
            }
            return infos;
        }

        private class PackageInfos
        {
            public string displayName = "";
            public string version = "";
        }
    }

    public struct PackageVersionInfos
    {
        public lilRenderPipeline RP;
        public int Major;
        public int Minor;
        public int Patch;
    }
}
#endif
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
            if(GraphicsSettings.renderPipelineAsset != null)
            {
                renderPipelineName = GraphicsSettings.renderPipelineAsset.ToString();
            }
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
            if(GraphicsSettings.renderPipelineAsset != null)
            {
                renderPipelineName = GraphicsSettings.renderPipelineAsset.ToString();
            }
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
            PackageVersionInfos version = ReadVersion("30648b8d550465f4bb77f1e1afd0b37d");
            version.RP = lilRenderPipeline.URP;
            return version;
        }

        private static PackageVersionInfos GetLWRPVersion()
        {
            PackageVersionInfos version = ReadVersion("30648b8d550465f4bb77f1e1afd0b37d");
            version.RP = lilRenderPipeline.LWRP;
            return version;
        }

        private static PackageVersionInfos GetHDRPVersion()
        {
            PackageVersionInfos version = ReadVersion("6f54db4299717fc4ca37866c6afa0905");
            version.RP = lilRenderPipeline.HDRP;
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
            infos.RP = lilRenderPipeline.BRP;
            if(string.IsNullOrEmpty(version))
            {
                infos.Major = 0;
                infos.Minor = 0;
                infos.Patch = 0;
            }
            else
            {
                string[] parts = version.Split('.');
                infos.Major = int.Parse(parts[0]);
                infos.Minor = int.Parse(parts[1]);
                infos.Patch = int.Parse(parts[2].Replace("-preview", ""));
            }
            return infos;
        }

        private class PackageInfos
        {
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
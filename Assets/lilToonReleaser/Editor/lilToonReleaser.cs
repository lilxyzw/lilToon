#if UNITY_EDITOR
using System.IO;
using System.IO.Compression;
using UnityEditor;

namespace lilToon
{
    public class lilToonReleaser
    {
        private const string packageName = "jp.lilxyzw.liltoon";

        [MenuItem("Assets/lilToon/[Release] Export packages", false, 1080)]
        public static void ExportPackages()
        {
            // Select directory
            string pathO = EditorUtility.SaveFolderPanel("Select directory to output", "", "");
            if(string.IsNullOrEmpty(pathO)) return;

            // Root path
            string pathI = AssetDatabase.GUIDToAssetPath("05d1d116436047941ad97d1b9064ee05");
            string version = lilConstants.currentVersionName;
            string commonName = "lilToon_" + version;
            pathO += "/" + commonName;

            // BOOTH path
            string pathDistribution = AssetDatabase.GUIDToAssetPath("2d8dcdfb497e01a4b93841f3993e7636");
            string pathLinkBooth = AssetDatabase.GUIDToAssetPath("c4bdc26d81a3bf148af4714c277e0cab");
            string pathLinkGitHub = AssetDatabase.GUIDToAssetPath("58b9f84a9414dfb4a97806ea5c618f68");
            string pathReadme = AssetDatabase.GUIDToAssetPath("3c433e0dc2c01824c98f95e1e0bcda79");
            string pathBoothFolder = pathO + "/" + commonName;
            string pathPackage = pathBoothFolder + "/" + commonName + ".unitypackage";
            string pathBoothZip = pathBoothFolder + ".zip";

            // VPM path
            string pathVPM = pathO + "/" + packageName + "-" + version + ".zip";

            // Markdown path
            string pathChangelog = AssetDatabase.GUIDToAssetPath("3e475cdc396583e41b50cc858691ceb7");
            string pathMarkdownI = AssetDatabase.GUIDToAssetPath("e3c0a3abc7c6c0540ab6ce28d4e24927");
            string pathMarkdownO = pathO + "/" + Path.GetFileName(pathMarkdownI);

            // Create directory
            if(!Directory.Exists(pathO)) Directory.CreateDirectory(pathO);
            if(!Directory.Exists(pathBoothFolder)) Directory.CreateDirectory(pathBoothFolder);
            pathBoothFolder += "/";

            // Create BOOTH package
            CopyToDirectory(pathDistribution, pathBoothFolder);
            CopyToDirectory(pathLinkBooth, pathBoothFolder);
            CopyToDirectory(pathLinkGitHub, pathBoothFolder);
            CopyToDirectory(pathReadme, pathBoothFolder);
            CreatePackage(pathI, pathPackage);
            CreateZip(pathBoothFolder, pathBoothZip, true);

            // Create VPM package
            CreateZip(pathI, pathVPM, false);

            // Write Markdown
            var sw = new StreamWriter(pathMarkdownO, false);
            var sr = new StreamReader(pathMarkdownI);
            bool shouldRead = false;
            string line;

            while((line = sr.ReadLine()) != null)
            {
                // Write changelog
                if(line.Contains("CHANGELOG"))
                {
                    var srChangelog = new StreamReader(pathChangelog);
                    while((line = srChangelog.ReadLine()) != null)
                    {
                        if(!shouldRead)
                        {
                            shouldRead = line.Contains("## [" + version + "]");
                            continue;
                        }
                        if(line.Contains("## [")) break;
                        sw.WriteLine(line);
                    }
                    srChangelog.Close();
                    continue;
                }

                // Write line
                sw.WriteLine(
                    line
                    .Replace("VERSION", version)
                    .Replace("PACKAGENAME", packageName)
                );
            }

            sw.Close();
            sr.Close();

            EditorUtility.DisplayDialog("[lilToon] Export packages", "Complete!", "OK");
        }

        private static void CopyToDirectory(string path, string folder)
        {
            File.Copy(path, folder + Path.GetFileName(path), true);
        }

        private static void CreatePackage(string pathI, string pathO)
        {
            if(File.Exists(pathO)) File.Delete(pathO);
            AssetDatabase.ExportPackage(pathI, pathO, ExportPackageOptions.Recurse);
        }

        private static void CreateZip(string pathI, string pathO, bool includeDirectory)
        {
            if(File.Exists(pathO)) File.Delete(pathO);
            ZipFile.CreateFromDirectory(pathI, pathO, CompressionLevel.Optimal, includeDirectory);
        }
    }
}
#endif
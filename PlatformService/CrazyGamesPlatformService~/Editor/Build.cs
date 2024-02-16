using System.IO;
using Modules.PlatformService.Editor.Build;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEngine;

namespace Modules.PlatformService.CrazyGamesPlatformService.Editor
{ 
    public static class Build
    {
        private const string CrazyDefine = "CRAZY";
        private const string WebGLTemplate = "PROJECT:Crazy_2020";

        [MenuItem("Game/Build/Crazy/BuildDev")]
        public static void BuildDev()
        {
            SetFbDebugDefines();
            DoBuild();
        }
        
        [MenuItem("Game/Build/Crazy/BuildProd")]
        public static void BuildProd()
        {
            SetFbProdDefines();
            DoBuild();
        }
        
        private static void DoBuild()
        {
            BuildBase.IncrementBuildNumber();
            PlayerSettings.WebGL.template = WebGLTemplate;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;

            var path = Application.dataPath.Replace("/Assets", $"/Builds/crazy/v{PlayerSettings.bundleVersion}");
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }

            Directory.CreateDirectory(path);

            if (string.IsNullOrEmpty(path) != false)
            {
                return;
            }

            BuildBase.BuildAddressables();
            BuildPipeline.BuildPlayer(BuildBase.GetScenes(), path, BuildTarget.WebGL, BuildOptions.CleanBuildCache);
            ZipBuild(path);
        }

        private static string ZipBuild(string directoryPath)
        {
            var zipFile = directoryPath + ".zip";
            ZipUtility.CompressFolderToZip(zipFile, null, directoryPath);
            return zipFile;
        }
        
        [MenuItem("Game/Build/Crazy/Set Defines Dev")]
        public static void SetFbDebugDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, DebugDefines);
        }
        
        [MenuItem("Game/Build/Crazy/Set Defines Prod")]
        public static void SetFbProdDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, ProdDefines);
        }
        
        private static string DebugDefines => $"{BuildBase.DebugDefines};{CrazyDefine}";
        private static string ProdDefines => $"{BuildBase.ProdDefines};{CrazyDefine}";
    }
}
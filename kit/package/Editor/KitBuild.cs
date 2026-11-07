// Kit v0 build entry points: one call per target, line defaults applied, guards enforced.
// Driven from the command line by kit/tools/build_unity.ps1, so a session can build,
// check and install without anyone opening the editor.
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Gabella.Kit.Editor
{
    public class KitBuildSettings
    {
        public string companyName = "Gabella Games";
        public string productName;
        public string applicationId;
        public string version = "0.1.0";
        public int versionCode = 1;
        public string bootScene = "Assets/Scenes/Boot.unity";
        public string apkPath;
        public string windowsExePath;
        public bool development;
    }

    public static class KitBuild
    {
        /// <summary>The scene every kit game boots into: empty; the game spawns itself from code.</summary>
        public static void EnsureBootScene(string path)
        {
            if (File.Exists(path)) return;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log("[kit] created boot scene " + path);
        }

        public static void ConfigureCommon(KitBuildSettings s)
        {
            PlayerSettings.companyName = s.companyName;
            PlayerSettings.productName = s.productName;
            PlayerSettings.bundleVersion = s.version;
            // Unity 6 made the splash optional on Personal; verified on Gian's licence 2026-09-13.
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.runInBackground = false;
        }

        public static void ConfigureAndroid(KitBuildSettings s)
        {
            ConfigureCommon(s);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, s.applicationId);
            PlayerSettings.Android.bundleVersionCode = s.versionCode;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            // Spike: Unity raised a requested 24 to 25 without a warning. Ask for 25 openly.
            PlayerSettings.Android.minSdkVersion = ApiLevel(25, (AndroidSdkVersions)25);
            PlayerSettings.Android.targetSdkVersion = ApiLevel(36, AndroidSdkVersions.AndroidApiLevelAuto);
            PlayerSettings.Android.forceInternetPermission = false;
            PlayerSettings.Android.forceSDCardPermission = false;
            PlayerSettings.Android.startInFullscreen = true;
            PlayerSettings.Android.renderOutsideSafeArea = true;   // the game fits its own safe area
            EditorUserBuildSettings.buildAppBundle = false;        // debug APK for sideloading; AAB comes at M4
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
        }

        public static bool Android(KitBuildSettings s)
        {
            var guard = KitGuards.Load();
            EnsureBootScene(s.bootScene);
            ConfigureAndroid(s);
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(s.apkPath)));
            string rejected = s.apkPath + ".REJECTED";
            if (File.Exists(rejected)) File.Delete(rejected);

            var options = new BuildPlayerOptions
            {
                scenes = new[] { s.bootScene },
                locationPathName = s.apkPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = s.development ? BuildOptions.Development : BuildOptions.None,
            };
            if (!Build(options)) return Finish(false);

            var failures = KitGuards.CheckApkPermissions(s.apkPath, guard, s.applicationId, out var found);
            Debug.Log("[kit] APK permissions: " + (found.Count == 0 ? "none" : string.Join(", ", found)));
            KitGuards.Report("permissions", failures);
            if (failures.Count > 0)
            {
                // never leave an untrusted APK where an install script would pick it up
                File.Move(s.apkPath, rejected);
                Debug.LogError("[kit] APK moved to " + rejected);
                return Finish(false);
            }
            Debug.Log($"[kit] APK OK {s.apkPath} {new FileInfo(s.apkPath).Length / (1024f * 1024f):0.00} MB");
            return Finish(true);
        }

        /// <summary>A windowed Windows player, portrait-proportioned, for screenshot checks on the desktop.</summary>
        public static bool Windows(KitBuildSettings s)
        {
            KitGuards.Load();
            EnsureBootScene(s.bootScene);
            ConfigureCommon(s);
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 1170;
            PlayerSettings.resizableWindow = false;
            PlayerSettings.runInBackground = true;   // the desktop check runs unattended, often unfocused
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            var options = new BuildPlayerOptions
            {
                scenes = new[] { s.bootScene },
                locationPathName = s.windowsExePath,
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.None,
            };
            return Finish(Build(options));
        }

        private static bool Build(BuildPlayerOptions options)
        {
            try
            {
                var summary = BuildPipeline.BuildPlayer(options).summary;
                Debug.Log($"[kit] build {options.target} result={summary.result} time={summary.totalTime.TotalSeconds:0.0}s errors={summary.totalErrors}");
                return summary.result == BuildResult.Succeeded;
            }
            catch (BuildFailedException e)
            {
                Debug.LogError("[kit] build refused: " + e.Message);
                return false;
            }
        }

        private static bool Finish(bool ok)
        {
            Debug.Log(ok ? "[kit] RESULT OK" : "[kit] RESULT FAILED");
            if (Application.isBatchMode) EditorApplication.Exit(ok ? 0 : 1);
            return ok;
        }

        // The AndroidApiLevelNN enum gains members every year; resolve by name so this
        // compiles on an editor newer or older than expected.
        private static AndroidSdkVersions ApiLevel(int level, AndroidSdkVersions fallback)
        {
            try { return (AndroidSdkVersions)Enum.Parse(typeof(AndroidSdkVersions), "AndroidApiLevel" + level); }
            catch (ArgumentException)
            {
                Debug.LogWarning($"[kit] API level {level} not in this editor's enum; using {fallback}");
                return fallback;
            }
        }
    }
}

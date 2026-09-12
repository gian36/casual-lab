// Batch-mode entry point: builds the scene in code, configures Android, produces the
// APK. Lets the whole Unity slice be driven from the command line, the same way the
// Godot and Capacitor ones were, so the three are measured comparably. THROWAWAY.
//
//   Unity.exe -quit -batchmode -nographics -projectPath <this dir> \
//             -executeMethod SpikeBuild.BuildAndroid -logFile -
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SpikeBuild
{
    private const string ScenePath = "Assets/Scenes/Spike.unity";
    private const string OutputPath = "build/cl01-spike-unity.apk";

    [MenuItem("Spike/Build Android")]
    public static void BuildAndroid()
    {
        CreateScene();
        ConfigurePlayer();

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputPath)));

        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.Development,
        };

        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;
        Debug.Log($"[spike] result={summary.result} size={summary.totalSize / (1024f * 1024f):0.00} MB " +
                  $"time={summary.totalTime.TotalSeconds:0.0}s errors={summary.totalErrors}");

        if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            EditorApplication.Exit(1);
        }
    }

    private static void CreateScene()
    {
        Directory.CreateDirectory("Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var go = new GameObject("Spike");
        go.AddComponent<SpikeBootstrap>();
        EditorSceneManager.SaveScene(scene, ScenePath);
    }

    private static void ConfigurePlayer()
    {
        PlayerSettings.companyName = "Gabella Games";
        PlayerSettings.productName = "CL-01 Spike Unity";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.gabella.cl01spikeunity");

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;

        // arm64 only, matching the Godot preset, so the size numbers compare like for like.
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);

        PlayerSettings.Android.minSdkVersion = ParseApiLevel(24, AndroidSdkVersions.AndroidApiLevel24);
        PlayerSettings.Android.targetSdkVersion = ParseApiLevel(36, AndroidSdkVersions.AndroidApiLevelAuto);

        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
    }

    // The AndroidApiLevelNN enum gains members every year; resolve by name so this does
    // not fail to compile on an editor that is newer or older than expected.
    private static AndroidSdkVersions ParseApiLevel(int level, AndroidSdkVersions fallback)
    {
        try
        {
            return (AndroidSdkVersions)Enum.Parse(typeof(AndroidSdkVersions), "AndroidApiLevel" + level);
        }
        catch (ArgumentException)
        {
            Debug.LogWarning($"[spike] API level {level} not in this editor's enum; using {fallback}");
            return fallback;
        }
    }
}

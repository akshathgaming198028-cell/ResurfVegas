// Editor-only helper for the 64-bit CI test build. Not part of the phone build.
// Called by game-ci via buildMethod; the normal Android build does not use it.
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CIArm64Build
{
    public static void Build()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        // IL2CPP stripping can remove classes the game only finds at runtime (UI, state scripts).
        PlayerSettings.stripEngineCode = false;
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        int code = 1;
        string run = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER");
        if (!string.IsNullOrEmpty(run))
        {
            int.TryParse(run, out code);
        }
        PlayerSettings.Android.bundleVersionCode = Mathf.Max(code, 1);

        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        Directory.CreateDirectory("build/Android");
        string outPath = "build/Android/ResurfVegas-arm64.apk";
        var report = BuildPipeline.BuildPlayer(scenes, outPath, BuildTarget.Android, BuildOptions.None);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new Exception("ARM64 build failed: " + report.summary.result);
        }
        Debug.Log("ARM64 build written to " + outPath);
    }
}

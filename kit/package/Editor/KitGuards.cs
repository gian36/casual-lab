// The three build-time conditions attached to the Unity ruling (LINE_CHARTER.md §3):
//  (a) the package manifest is pinned;
//  (b) the APK's permissions are asserted at build time;
//  (c) the build fails on any package the editor added unrequested.
// During the engine spike Unity silently added IAP and analytics packages on open, and they
// shipped BILLING and ACCESS_NETWORK_STATE into the APK. For an ads-funded line that has to
// declare Play Data safety, that is a governance failure, so these checks fail the build.
//
// The source of truth is the game's kit-guards.json (project root, next to Assets/), which
// Unity never writes:
//   { "packages":    ["com.unity.ugui@2.0.0", "com.gabella.kit@file:../../../kit/package", ...],
//     "permissions": ["{appId}.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION"] }
// Widening either list is a deliberate, reviewable edit (M4 adds the ads SDK this way).
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Gabella.Kit.Editor
{
    [Serializable]
    public class GuardFile
    {
        public string[] packages = new string[0];
        public string[] permissions = new string[0];
    }

    public static class KitGuards
    {
        public const string GuardPath = "kit-guards.json";
        private static readonly Regex Exact = new Regex(@"^\d+\.\d+\.\d+(-[0-9A-Za-z.\-]+)?$");

        public static GuardFile Load()
        {
            if (!File.Exists(GuardPath))
                throw new BuildFailedException($"[kit] {GuardPath} is missing: a kit game must declare its packages and permissions.");
            return JsonUtility.FromJson<GuardFile>(File.ReadAllText(GuardPath));
        }

        public static Dictionary<string, string> Requested(GuardFile g)
        {
            var d = new Dictionary<string, string>();
            foreach (var p in g.packages)
            {
                int at = p.IndexOf('@');
                if (at <= 0) throw new BuildFailedException($"[kit] {GuardPath}: '{p}' is not name@version");
                d[p.Substring(0, at)] = p.Substring(at + 1);
            }
            return d;
        }

        /// <summary>(a) + (c), on disk and as resolved by the Package Manager. Empty = pass.</summary>
        public static List<string> CheckPackages(GuardFile g)
        {
            var fail = new List<string>();
            var want = Requested(g);

            string manifestText = File.ReadAllText("Packages/manifest.json");
            var manifest = FlatObject(manifestText, "dependencies");

            // (a) pinned: exact versions or local paths, nothing floating
            foreach (var kv in manifest)
            {
                bool local = kv.Value.StartsWith("file:", StringComparison.Ordinal);
                if (!local && !Exact.IsMatch(kv.Value))
                    fail.Add($"(a) {kv.Key}@{kv.Value} is not pinned to an exact version");
            }
            if (manifestText.Contains("\"scopedRegistries\""))
                fail.Add("(a) manifest declares scopedRegistries; kit v0 allows none");

            // (a) the manifest is exactly the requested set; (c) nothing extra
            foreach (var kv in manifest)
            {
                if (!want.TryGetValue(kv.Key, out string v))
                    fail.Add($"(c) {kv.Key}@{kv.Value} is in the manifest but was never requested");
                else if (v != kv.Value)
                    fail.Add($"(a) {kv.Key} is {kv.Value} in the manifest, pinned to {v}");
            }
            foreach (var kv in want)
                if (!manifest.ContainsKey(kv.Key))
                    fail.Add($"(a) {kv.Key}@{kv.Value} is requested but missing from the manifest");

            // (a) the lock file agrees with the manifest at depth 0
            if (File.Exists("Packages/packages-lock.json"))
            {
                string lockText = File.ReadAllText("Packages/packages-lock.json");
                foreach (Match m in Regex.Matches(lockText,
                             "\"([^\"]+)\"\\s*:\\s*\\{\\s*\"version\"\\s*:\\s*\"([^\"]*)\"\\s*,\\s*\"depth\"\\s*:\\s*(\\d+)"))
                {
                    if (m.Groups[3].Value != "0") continue;
                    string name = m.Groups[1].Value;
                    if (!want.ContainsKey(name))
                        fail.Add($"(c) {name} is a direct dependency in packages-lock.json but was never requested");
                }
            }

            // (c) what the Package Manager actually resolved: every package must be requested,
            // or be a dependency (transitively) of something requested.
            var registered = PackageInfo.GetAllRegisteredPackages();
            var byName = registered.ToDictionary(p => p.name, p => p);
            var allowed = new HashSet<string>(want.Keys);
            var queue = new Queue<string>(want.Keys);
            while (queue.Count > 0)
            {
                string n = queue.Dequeue();
                if (!byName.TryGetValue(n, out var info)) continue;
                foreach (var dep in info.dependencies)
                    if (allowed.Add(dep.name)) queue.Enqueue(dep.name);
            }
            foreach (var p in registered)
            {
                if (!allowed.Contains(p.name))
                    fail.Add($"(c) {p.name}@{p.version} is resolved in the project but nothing requested it");
                else if (p.isDirectDependency && !want.ContainsKey(p.name))
                    fail.Add($"(c) {p.name}@{p.version} is a direct dependency but was never requested");
            }
            return fail;
        }

        /// <summary>(b): every uses-permission in the APK must be on the allow list. Empty = pass.</summary>
        public static List<string> CheckApkPermissions(string apk, GuardFile g, string appId, out List<string> found)
        {
            found = new List<string>();
            var fail = new List<string>();
            string aapt2 = FindAapt2();
            if (aapt2 == null)
            {
                fail.Add("(b) aapt2 not found: cannot read the APK's permissions, so the build is not trusted");
                return fail;
            }
            var psi = new ProcessStartInfo(aapt2, $"dump permissions \"{Path.GetFullPath(apk)}\"")
            {
                UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true,
            };
            string output;
            using (var p = Process.Start(psi))
            {
                output = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
                p.WaitForExit();
                if (p.ExitCode != 0) { fail.Add($"(b) aapt2 failed ({p.ExitCode}): {output.Trim()}"); return fail; }
            }
            var allowed = new HashSet<string>(g.permissions.Select(x => x.Replace("{appId}", appId)));
            foreach (Match m in Regex.Matches(output, @"uses-permission(?:-sdk-23)?: name='([^']+)'"))
            {
                string perm = m.Groups[1].Value;
                found.Add(perm);
                if (!allowed.Contains(perm)) fail.Add($"(b) the APK uses {perm}, which is not on the allow list");
            }
            return fail;
        }

        public static string FindAapt2()
        {
            var roots = new List<string>
            {
                Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/SDK"),
                Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"),
                Environment.GetEnvironmentVariable("ANDROID_HOME"),
                @"C:\Android\Sdk",
            };
            foreach (var root in roots.Where(r => !string.IsNullOrEmpty(r)))
            {
                string bt = Path.Combine(root, "build-tools");
                if (!Directory.Exists(bt)) continue;
                foreach (var dir in Directory.GetDirectories(bt).OrderByDescending(d => d))
                {
                    string exe = Path.Combine(dir, Application.platform == RuntimePlatform.WindowsEditor ? "aapt2.exe" : "aapt2");
                    if (File.Exists(exe)) return exe;
                }
            }
            return null;
        }

        /// <summary>The string-to-string pairs of one flat JSON object member (Unity's manifest shape).</summary>
        public static Dictionary<string, string> FlatObject(string json, string member)
        {
            var d = new Dictionary<string, string>();
            var block = Regex.Match(json, "\"" + Regex.Escape(member) + "\"\\s*:\\s*\\{([^}]*)\\}");
            if (!block.Success) return d;
            foreach (Match m in Regex.Matches(block.Groups[1].Value, "\"([^\"]+)\"\\s*:\\s*\"([^\"]*)\""))
                d[m.Groups[1].Value] = m.Groups[2].Value;
            return d;
        }

        public static void Report(string what, List<string> failures)
        {
            if (failures.Count == 0) { Debug.Log($"[kit] guard {what}: PASS"); return; }
            foreach (var f in failures) Debug.LogError($"[kit] guard {what}: FAIL {f}");
        }
    }

    /// <summary>Runs (a) and (c) before every player build, however it was started.</summary>
    public class KitPreBuildGuard : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;

        public void OnPreprocessBuild(BuildReport report)
        {
            var failures = KitGuards.CheckPackages(KitGuards.Load());
            KitGuards.Report("packages", failures);
            if (failures.Count > 0)
                throw new BuildFailedException($"[kit] {failures.Count} package guard failure(s); see the log. Fix the manifest, never the guard.");
        }
    }
}

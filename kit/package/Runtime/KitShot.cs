// Kit v0 desktop self-check: a player launched with --shot can save its own frames.
// It only ever sees the game's own frame, never the desktop. Inert without the flag.
// Spike lesson: six of Unity's seven defects compiled clean and were caught only on a
// screenshot or the phone, so every build must be looked at, not just compiled.
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Gabella.Kit
{
    public static class KitShot
    {
        public static bool Enabled => Array.IndexOf(Environment.GetCommandLineArgs(), "--shot") >= 0;

        /// <summary>Value after a command-line flag, or null.</summary>
        public static string Arg(string flag)
        {
            var args = Environment.GetCommandLineArgs();
            int i = Array.IndexOf(args, flag);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
        }

        public static string Dir
        {
            get
            {
                string d = Arg("--shot-dir") ?? Path.Combine(Application.persistentDataPath, "shots");
                Directory.CreateDirectory(d);
                return d;
            }
        }

        /// <summary>Yield this from a coroutine: saves the current frame as name.png.</summary>
        public static IEnumerator Capture(string name)
        {
            yield return new WaitForEndOfFrame();
            Texture2D frame = ScreenCapture.CaptureScreenshotAsTexture();
            string path = Path.Combine(Dir, name + ".png");
            File.WriteAllBytes(path, frame.EncodeToPNG());
            UnityEngine.Object.Destroy(frame);
            Debug.Log("[kit] shot " + path);
        }
    }
}

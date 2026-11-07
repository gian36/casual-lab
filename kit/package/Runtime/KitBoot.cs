// Kit v0 boot: the line's device defaults, applied before the first scene loads.
// Portrait only (every Toybox game is portrait), frame rate matched to the display
// (spike: a hard 60 capped a 120 Hz phone), screen kept awake during play, one finger.
using UnityEngine;

namespace Gabella.Kit
{
    public static class KitBoot
    {
        public static bool Booted { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;

            // On Android -1 means the platform default of 30, so set it explicitly.
            QualitySettings.vSyncCount = 0;
            int hz = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
            Application.targetFrameRate = Mathf.Max(60, hz);

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Input.multiTouchEnabled = false;   // a second finger must not hijack a drag
            Booted = true;
            Debug.Log($"[kit] boot v0.1.0 screen={Screen.width}x{Screen.height} safe={Screen.safeArea} fps={Application.targetFrameRate}");
        }

        /// <summary>Create the game's root object, with a camera that clears the screen.</summary>
        public static T Spawn<T>(Color clear) where T : MonoBehaviour
        {
            var camGo = new GameObject("KitCamera");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = clear;
            cam.orthographic = true;
            cam.cullingMask = 0;
            Object.DontDestroyOnLoad(camGo);

            var go = new GameObject(typeof(T).Name);
            Object.DontDestroyOnLoad(go);
            return go.AddComponent<T>();
        }
    }
}

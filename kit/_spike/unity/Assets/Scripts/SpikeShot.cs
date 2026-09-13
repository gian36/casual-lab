// Engine-side capture for the Unity slice, the counterpart of Godot's `-- --shot`: it can
// only ever see this game's own frame, never the desktop. Self-installing, and inert unless
// the player is launched with --shot, so it changes nothing about the slice itself.
// THROWAWAY.
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class SpikeShot : MonoBehaviour
{
    private const int FramesBeforeCapture = 90;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "--shot") < 0) return;
        var go = new GameObject("SpikeShot");
        DontDestroyOnLoad(go);
        go.AddComponent<SpikeShot>();
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < FramesBeforeCapture; i++) yield return null;

        // CaptureScreenshotAsTexture must run at end of frame, and includes Screen Space -
        // Overlay canvases, which is where the whole slice is drawn.
        yield return new WaitForEndOfFrame();
        Texture2D frame = ScreenCapture.CaptureScreenshotAsTexture();
        string path = Path.Combine(Application.persistentDataPath, "shot.png");
        File.WriteAllBytes(path, frame.EncodeToPNG());
        Destroy(frame);

        Debug.Log("[spike] shot saved: " + path);
        Application.Quit();
    }
}

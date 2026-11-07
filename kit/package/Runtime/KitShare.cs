// Kit v0: hand text to the phone's own share sheet (Gmail, Drive, Notes ...), so the player
// chooses where it goes. No permission, no network, no SDK: Android's ACTION_SEND does the
// sending, and only when the player picks a target. On desktop the text goes to a file and
// the clipboard instead.
using System;
using System.IO;
using UnityEngine;

namespace Gabella.Kit
{
    public static class KitShare
    {
        public static bool Text(string subject, string body, string chooserTitle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var intentClass = new AndroidJavaClass("android.content.Intent"))
                using (var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.SEND"))
                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    intent.Call<AndroidJavaObject>("setType", "text/plain").Dispose();
                    intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.SUBJECT", subject).Dispose();
                    intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", body).Dispose();
                    using (var chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, chooserTitle))
                        activity.Call("startActivity", chooser);
                }
                Debug.Log($"[kit] share sheet opened ({body.Length} chars)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[kit] share failed: " + ex.Message);
                return false;
            }
#else
            try
            {
                string dir = Path.Combine(Application.persistentDataPath, "share");
                Directory.CreateDirectory(dir);
                string path = Path.Combine(dir, DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt");
                File.WriteAllText(path, subject + "\n\n" + body);
                GUIUtility.systemCopyBuffer = body;
                Debug.Log($"[kit] share (desktop): {body.Length} chars written to {path} and copied to the clipboard");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[kit] share failed: " + ex.Message);
                return false;
            }
#endif
        }
    }
}

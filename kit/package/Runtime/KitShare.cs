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
        /// <summary>
        /// Save text as a file in the phone's Downloads/{Application.productName} folder and offer
        /// it to the share sheet as an attachment. Android 10+ only (MediaStore, no permission);
        /// returns false on older phones or any failure, so the caller can fall back to Text().
        /// </summary>
        public static bool File(string fileName, string mime, string content, string subject, string note, string chooserTitle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                    if (version.GetStatic<int>("SDK_INT") < 29) return false;

                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var resolver = activity.Call<AndroidJavaObject>("getContentResolver"))
                using (var values = new AndroidJavaObject("android.content.ContentValues"))
                using (var downloads = new AndroidJavaClass("android.provider.MediaStore$Downloads"))
                using (var collection = downloads.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI"))
                {
                    values.Call("put", "_display_name", fileName);
                    values.Call("put", "mime_type", mime);
                    values.Call("put", "relative_path", "Download/" + Application.productName);
                    using (var uri = resolver.Call<AndroidJavaObject>("insert", collection, values))
                    {
                        if (uri == null) return false;
                        using (var os = resolver.Call<AndroidJavaObject>("openOutputStream", uri))
                        {
                            os.Call("write", (sbyte[])(Array)new System.Text.UTF8Encoding(false).GetBytes(content));
                            os.Call("close");
                        }

                        using (var intentClass = new AndroidJavaClass("android.content.Intent"))
                        using (var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.SEND"))
                        using (var clipClass = new AndroidJavaClass("android.content.ClipData"))
                        using (var clip = clipClass.CallStatic<AndroidJavaObject>("newRawUri", fileName, uri))
                        {
                            intent.Call<AndroidJavaObject>("setType", mime).Dispose();
                            intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.STREAM", uri).Dispose();
                            intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.SUBJECT", subject).Dispose();
                            intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", note).Dispose();
                            intent.Call("setClipData", clip);
                            intent.Call<AndroidJavaObject>("addFlags", 1).Dispose();   // FLAG_GRANT_READ_URI_PERMISSION
                            using (var chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, chooserTitle))
                            {
                                chooser.Call<AndroidJavaObject>("addFlags", 1).Dispose();
                                activity.Call("startActivity", chooser);
                            }
                        }
                    }
                }
                Debug.Log($"[kit] share file {fileName} ({content.Length} chars), saved in Download/{Application.productName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[kit] share file failed, falling back: " + ex.Message);
                return false;
            }
#else
            try
            {
                string dir = Path.Combine(Application.persistentDataPath, "share");
                Directory.CreateDirectory(dir);
                string path = Path.Combine(dir, fileName);
                System.IO.File.WriteAllText(path, content);
                Debug.Log($"[kit] share file (desktop): {content.Length} chars written to {path}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[kit] share file failed: " + ex.Message);
                return false;
            }
#endif
        }

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
                System.IO.File.WriteAllText(path, subject + "\n\n" + body);
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

// Kit v0 play log: one JSON object per line, written to the phone's app folder.
// LOCAL ONLY. No network, no SDK, no identifiers: it exists so a session can read what
// happened in Gian's own play (pull it with adb). It is not analytics; analytics comes
// from the kit at M4, behind consent.
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gabella.Kit
{
    public static class EventLog
    {
        private const int Keep = 40;
        private static StreamWriter _w;
        private static float _t0;
        public static string FilePath { get; private set; }

        public static void Open()
        {
            if (_w != null) return;
            try
            {
                string dir = Path.Combine(Application.persistentDataPath, "logs");
                Directory.CreateDirectory(dir);
                foreach (var old in Directory.GetFiles(dir, "*.jsonl").OrderByDescending(f => f).Skip(Keep - 1))
                    File.Delete(old);
                FilePath = Path.Combine(dir, DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture) + ".jsonl");
                _w = new StreamWriter(FilePath, false, new UTF8Encoding(false)) { AutoFlush = true };
                _t0 = Time.realtimeSinceStartup;
                Application.quitting += Close;
                Debug.Log("[kit] event log " + FilePath);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[kit] event log unavailable: " + ex.Message);
                _w = null;
            }
        }

        /// <summary>Write one event. fields: "key", value, "key", value ... (numbers, bools, strings).</summary>
        public static void Write(string kind, params object[] fields)
        {
            if (_w == null) return;
            var sb = new StringBuilder(96);
            sb.Append("{\"rt\":").Append((Time.realtimeSinceStartup - _t0).ToString("0.000", CultureInfo.InvariantCulture));
            sb.Append(",\"ev\":\"").Append(Escape(kind)).Append('"');
            for (int i = 0; i + 1 < fields.Length; i += 2)
            {
                sb.Append(",\"").Append(Escape(fields[i].ToString())).Append("\":");
                object v = fields[i + 1];
                switch (v)
                {
                    case null: sb.Append("null"); break;
                    case bool b: sb.Append(b ? "true" : "false"); break;
                    case float f: sb.Append(f.ToString("0.###", CultureInfo.InvariantCulture)); break;
                    case double d: sb.Append(d.ToString("0.###", CultureInfo.InvariantCulture)); break;
                    case int n: sb.Append(n.ToString(CultureInfo.InvariantCulture)); break;
                    default: sb.Append('"').Append(Escape(v.ToString())).Append('"'); break;
                }
            }
            sb.Append('}');
            try { _w.WriteLine(sb.ToString()); } catch (Exception) { /* a full disk must not stop play */ }
        }

        /// <summary>
        /// Every kept log file as one block of text, for the player to send by hand (KitShare).
        /// Plain JSONL with a "# file" line before each file when it fits in maxChars; otherwise
        /// gzip + base64 (first line "GZIP-BASE64", wrapped at 76 so mail clients leave it alone).
        /// Oldest files are dropped until it fits. files = how many went in.
        /// </summary>
        public static string Bundle(int maxChars, out int files)
        {
            var parts = new System.Collections.Generic.List<string>();   // newest first
            try
            {
                string dir = Path.Combine(Application.persistentDataPath, "logs");
                foreach (var f in Directory.GetFiles(dir, "*.jsonl").OrderByDescending(x => x))
                {
                    using (var fs = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var r = new StreamReader(fs, Encoding.UTF8))
                        parts.Add("# file " + Path.GetFileName(f) + "\n" + r.ReadToEnd());
                }
            }
            catch (Exception ex) { Debug.LogWarning("[kit] reading logs failed: " + ex.Message); }

            for (int n = parts.Count; n > 0; n--)
            {
                string plain = string.Join("\n", parts.Take(n).Reverse());
                if (plain.Length <= maxChars) { files = n; return plain; }
                string packed = Pack(plain);
                if (packed.Length <= maxChars) { files = n; return packed; }
            }
            files = 0;
            return "";
        }

        private static string Pack(string text)
        {
            using (var ms = new MemoryStream())
            {
                using (var gz = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionLevel.Optimal, true))
                {
                    var bytes = new UTF8Encoding(false).GetBytes(text);
                    gz.Write(bytes, 0, bytes.Length);
                }
                string b64 = Convert.ToBase64String(ms.ToArray());
                var sb = new StringBuilder(b64.Length + b64.Length / 76 + 16).Append("GZIP-BASE64\n");
                for (int i = 0; i < b64.Length; i += 76) sb.Append(b64, i, Math.Min(76, b64.Length - i)).Append('\n');
                return sb.ToString();
            }
        }

        public static void Close()
        {
            _w?.Dispose();
            _w = null;
        }

        private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}

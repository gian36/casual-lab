// Procedurally generated one-shots — no audio asset, same shapes as the Godot and
// Phaser slices, so the three are comparable. THROWAWAY.
using System;
using UnityEngine;

public static class SpikeSfx
{
    private const int MixRate = 22050;

    public static AudioClip CompletePop()
    {
        return Tone("pop", 0.14f, t =>
        {
            float env = Mathf.Exp(-t * 26f);
            float freq = 760f + 620f * Mathf.Exp(-t * 16f);
            return Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.55f;
        });
    }

    public static AudioClip Reject()
    {
        return Tone("reject", 0.10f, t =>
            Mathf.Sin(2f * Mathf.PI * 190f * t) * Mathf.Exp(-t * 30f) * 0.35f);
    }

    private static AudioClip Tone(string name, float duration, Func<float, float> shape)
    {
        int frames = Mathf.FloorToInt(MixRate * duration);
        var data = new float[frames];
        for (int i = 0; i < frames; i++)
        {
            data[i] = Mathf.Clamp(shape((float)i / MixRate), -1f, 1f);
        }
        var clip = AudioClip.Create(name, frames, 1, MixRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}

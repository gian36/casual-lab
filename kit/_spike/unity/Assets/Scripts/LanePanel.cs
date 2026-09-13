// One gate lane: aircraft, outstanding need icons, the bar for the running service,
// and the drop target for the tray. THROWAWAY.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanePanel : MonoBehaviour
{
    public int LaneIndex;
    public List<int> Needs = new List<int>();
    public int ActiveService = -1;
    public float ActiveElapsed;
    public float RefuseFlash;

    private Image _background;
    private Image _gateLight;
    private PolygonGraphic _aircraft;
    private readonly List<Image> _icons = new List<Image>();
    private readonly List<Image> _fills = new List<Image>();
    private readonly List<Text> _labels = new List<Text>();
    private readonly List<Pop> _pops = new List<Pop>();
    private readonly List<Image> _popImages = new List<Image>();

    private struct Pop
    {
        public int Service;
        public float Age;
    }

    public RectTransform Rect => (RectTransform)transform;

    public bool IsClean => Needs.Count == 0 && ActiveService == -1;

    public bool Accepts(int service) => ActiveService == -1 && Needs.Contains(service);

    public void Build(Image background, Image gateLight, PolygonGraphic aircraft,
        List<Image> icons, List<Image> fills, List<Text> labels, List<Image> popImages)
    {
        _background = background;
        _gateLight = gateLight;
        _aircraft = aircraft;
        _icons.AddRange(icons);
        _fills.AddRange(fills);
        _labels.AddRange(labels);
        _popImages.AddRange(popImages);
    }

    public void ResetLane(List<int> needs)
    {
        Needs = new List<int>(needs);
        ActiveService = -1;
        ActiveElapsed = 0f;
        RefuseFlash = 0f;
        _pops.Clear();
    }

    public void StartService(int service)
    {
        ActiveService = service;
        ActiveElapsed = 0f;
    }

    public void Refuse() => RefuseFlash = 1f;

    // Returns the service that just finished, or -1.
    public int Tick(float dt, float serviceSeconds)
    {
        int completed = -1;
        if (ActiveService != -1)
        {
            ActiveElapsed += dt;
            if (ActiveElapsed >= serviceSeconds)
            {
                completed = ActiveService;
                Needs.Remove(completed);
                _pops.Add(new Pop { Service = completed, Age = 0f });
                ActiveService = -1;
                ActiveElapsed = 0f;
            }
        }

        for (int i = _pops.Count - 1; i >= 0; i--)
        {
            var pop = _pops[i];
            pop.Age += dt;
            if (pop.Age >= 0.55f) _pops.RemoveAt(i);
            else _pops[i] = pop;
        }

        if (RefuseFlash > 0f) RefuseFlash = Mathf.Max(0f, RefuseFlash - dt * 3f);
        return completed;
    }

    public void Redraw(float serviceSeconds, Color[] serviceColours, string[] serviceNames)
    {
        Color bg = new Color(0.13f, 0.15f, 0.19f);
        if (RefuseFlash > 0f) bg = Color.Lerp(bg, new Color(0.26f, 0.15f, 0.16f), RefuseFlash);
        _background.color = bg;

        _gateLight.color = IsClean
            ? new Color(0.35f, 0.85f, 0.45f)
            : new Color(0.85f, 0.65f, 0.25f);
        _aircraft.color = IsClean
            ? new Color(0.55f, 0.9f, 0.65f)
            : new Color(0.82f, 0.85f, 0.9f);

        for (int slot = 0; slot < _icons.Count; slot++)
        {
            bool used = slot < Needs.Count;
            _icons[slot].gameObject.SetActive(used);
            _labels[slot].gameObject.SetActive(used);
            if (!used)
            {
                // FIXED 2026-09-13: the fill image was left active on unused slots, and an
                // Image with no sprite draws solid white.
                _fills[slot].gameObject.SetActive(false);
                continue;
            }

            int service = Needs[slot];
            _labels[slot].text = serviceNames[service];

            if (service == ActiveService)
            {
                // Active service: the icon becomes a filling bar.
                _icons[slot].color = serviceColours[service] * 0.35f;
                _fills[slot].gameObject.SetActive(true);
                _fills[slot].color = serviceColours[service];
                _fills[slot].fillAmount = Mathf.Clamp01(ActiveElapsed / serviceSeconds);
            }
            else
            {
                _icons[slot].color = serviceColours[service];
                _fills[slot].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _popImages.Count; i++)
        {
            bool live = i < _pops.Count;
            _popImages[i].gameObject.SetActive(live);
            if (!live) continue;
            var pop = _pops[i];
            float t = Mathf.Clamp01(pop.Age / 0.55f);
            Color c = serviceColours[pop.Service];
            c.a = 1f - t;
            _popImages[i].color = c;
            float scale = 1f + 0.5f * (1f - t) + 1.6f * t;
            _popImages[i].rectTransform.localScale = Vector3.one * scale;
        }
    }
}

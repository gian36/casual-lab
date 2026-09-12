// CL-01 engine spike — Unity slice, per HANDOFF_KIT_ENGINE_SPIKE.md. THROWAWAY.
//
// The whole UI is built in code rather than authored in a .unity scene, so the slice
// stays text-reviewable and diffable in the same way the Godot and Phaser ones are.
// Unity scene YAML is GUID-bound and not sensibly hand-written.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpikeBootstrap : MonoBehaviour
{
    private static readonly string[] ServiceNames = { "FUEL", "BAGS", "CATER", "PUSH" };

    private static readonly Color[] ServiceColours =
    {
        new Color(0.98f, 0.75f, 0.25f),  // FUEL
        new Color(0.42f, 0.72f, 0.98f),  // BAGS
        new Color(0.72f, 0.55f, 0.95f),  // CATER
        new Color(0.45f, 0.88f, 0.62f),  // PUSH
    };

    // Same unit-space outline as the Godot and Phaser slices.
    private static readonly Vector2[] Plane =
    {
        new Vector2(0.00f, 0.50f), new Vector2(0.09f, 0.36f), new Vector2(0.34f, 0.33f),
        new Vector2(0.40f, 0.06f), new Vector2(0.48f, 0.06f), new Vector2(0.49f, 0.33f),
        new Vector2(0.78f, 0.33f), new Vector2(0.90f, 0.10f), new Vector2(0.97f, 0.10f),
        new Vector2(0.94f, 0.36f), new Vector2(1.00f, 0.44f), new Vector2(1.00f, 0.56f),
        new Vector2(0.94f, 0.64f), new Vector2(0.97f, 0.90f), new Vector2(0.90f, 0.90f),
        new Vector2(0.78f, 0.67f), new Vector2(0.49f, 0.67f), new Vector2(0.48f, 0.94f),
        new Vector2(0.40f, 0.94f), new Vector2(0.34f, 0.67f), new Vector2(0.09f, 0.64f),
    };

    private const int W = 1080;
    private const int H = 1920;
    private const int LaneCount = 4;
    private const float ServiceSeconds = 3f;
    private const float ShiftSeconds = 45f;
    private const string BestKey = "cl01.best";

    // FLAGGED TO GIAN 2026-09-12, UNRULED — same flag as the other two slices. -1 keeps
    // the briefed behaviour (unlimited vehicles). Set to 2 to feel contention.
    private const int VehiclePool = -1;

    private const float LaneTop = 230f;
    private const float LaneH = 300f;
    private const float LaneGap = 16f;
    private const float TrayTop = 1580f;
    private const float TrayH = 300f;
    private const float Icon = 64f;
    private const float IconGap = 14f;

    private readonly List<LanePanel> _lanes = new List<LanePanel>();
    private readonly List<TrayButton> _tray = new List<TrayButton>();
    private int[] _available;

    private Canvas _canvas;
    private Font _font;
    private Text _hud;
    private Text _perf;
    private GameObject _failOverlay;
    private Text _failDetail;
    private AudioSource _audio;
    private AudioClip _pop;
    private AudioClip _reject;
    private RectTransform _dragPreview;
    private Transform _stressRoot;

    private float _timeLeft = ShiftSeconds;
    private int _served;
    private int _best;
    private bool _running = true;

    private float _accumulated;
    private float _startedAt;
    private float _frameMax;
    private float _dragFrameMax;
    private float _perfWindow;
    private bool _dragging;
    private int _stress;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.Portrait;

        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _best = PlayerPrefs.GetInt(BestKey, 0);
        _pop = SpikeSfx.CompletePop();
        _reject = SpikeSfx.Reject();
        _audio = gameObject.AddComponent<AudioSource>();

        BuildCanvas();
        BuildLanes();
        BuildTray();
        BuildHud();
        Restart();
    }

    private void BuildCanvas()
    {
        var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        _canvas = canvasGo.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(W, H);
        scaler.matchWidthOrHeight = 1f;  // portrait: match height

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        var bg = NewImage("Bg", _canvas.transform, 0, 0, W, H);
        bg.color = new Color(0.075f, 0.086f, 0.11f);

        _stressRoot = new GameObject("StressLayer", typeof(RectTransform)).transform;
        _stressRoot.SetParent(_canvas.transform, false);

        _dragPreview = NewImage("DragPreview", _canvas.transform, 0, 0, 120, 120).rectTransform;
        _dragPreview.anchorMin = _dragPreview.anchorMax = new Vector2(0.5f, 0.5f);
        _dragPreview.gameObject.SetActive(false);
    }

    private void BuildLanes()
    {
        for (int i = 0; i < LaneCount; i++)
        {
            float y = LaneTop + i * (LaneH + LaneGap);
            var laneGo = new GameObject("Lane" + i, typeof(RectTransform));
            laneGo.transform.SetParent(_canvas.transform, false);
            Place((RectTransform)laneGo.transform, 24, y, W - 48, LaneH);

            var background = NewImage("Bg", laneGo.transform, 0, 0, W - 48, LaneH);
            var light = NewImage("Light", laneGo.transform, 18, LaneH / 2 - 12, 24, 24);

            var planeGo = new GameObject("Aircraft", typeof(RectTransform), typeof(PolygonGraphic));
            planeGo.transform.SetParent(laneGo.transform, false);
            Place((RectTransform)planeGo.transform, 58, LaneH * 0.22f, (W - 48) * 0.42f, LaneH * 0.56f);
            var aircraft = planeGo.GetComponent<PolygonGraphic>();
            aircraft.Points = Plane;

            var icons = new List<Image>();
            var fills = new List<Image>();
            var labels = new List<Text>();
            var pops = new List<Image>();
            float ix = (W - 48) * 0.52f;
            float iy = LaneH / 2 - Icon / 2;
            for (int slot = 0; slot < ServiceNames.Length; slot++)
            {
                icons.Add(NewImage("Icon" + slot, laneGo.transform, ix, iy, Icon, Icon));
                var fill = NewImage("Fill" + slot, laneGo.transform, ix, iy, Icon, Icon);
                fill.type = Image.Type.Filled;
                fill.fillMethod = Image.FillMethod.Vertical;
                fill.fillOrigin = (int)Image.OriginVertical.Bottom;
                fills.Add(fill);
                labels.Add(NewText("Label" + slot, laneGo.transform, ix - 2, iy + Icon + 4, 120, 26, 20,
                    new Color(0.75f, 0.78f, 0.82f)));
                var pop = NewImage("Pop" + slot, laneGo.transform, ix, iy, Icon, Icon);
                pop.raycastTarget = false;
                pops.Add(pop);
                ix += Icon + IconGap;
            }

            var lane = laneGo.AddComponent<LanePanel>();
            lane.LaneIndex = i;
            lane.Build(background, light, aircraft, icons, fills, labels, pops);
            _lanes.Add(lane);
        }
    }

    private void BuildTray()
    {
        _available = new int[ServiceNames.Length];
        float slotW = (W - 48 - 3 * 16) / 4f;
        for (int s = 0; s < ServiceNames.Length; s++)
        {
            float x = 24 + s * (slotW + 16);
            var go = new GameObject("Tray" + s, typeof(RectTransform));
            go.transform.SetParent(_canvas.transform, false);
            Place((RectTransform)go.transform, x, TrayTop, slotW, TrayH);

            var body = NewImage("Body", go.transform, 0, 0, slotW, TrayH);
            body.color = ServiceColours[s];
            NewText("Label", go.transform, 18, TrayH / 2 - 22, slotW, 44, 34, new Color(0.08f, 0.09f, 0.11f));

            var button = go.AddComponent<TrayButton>();
            button.Service = s;
            button.Available = VehiclePool;
            button.Build(body, _dragPreview, _canvas);
            button.OnDragStarted = _ => _dragging = true;
            button.OnDropped = OnDropped;
            _tray.Add(button);
        }
    }

    private void BuildHud()
    {
        _hud = NewText("Hud", _canvas.transform, 32, 24, 700, 190, 64, Color.white);
        _perf = NewText("Perf", _canvas.transform, W - 380, 24, 348, 190, 28,
            new Color(0.5f, 0.75f, 0.55f));
        _perf.alignment = TextAnchor.UpperRight;

        _failOverlay = new GameObject("FailOverlay", typeof(RectTransform));
        _failOverlay.transform.SetParent(_canvas.transform, false);
        Place((RectTransform)_failOverlay.transform, 0, 0, W, H);
        var dim = NewImage("Dim", _failOverlay.transform, 0, 0, W, H);
        dim.color = new Color(0, 0, 0, 0.72f);
        NewText("FailLabel", _failOverlay.transform, 80, 760, 920, 140, 88,
            new Color(1f, 0.42f, 0.38f)).alignment = TextAnchor.MiddleCenter;
        _failDetail = NewText("FailDetail", _failOverlay.transform, 80, 910, 920, 90, 40,
            new Color(0.78f, 0.8f, 0.84f));
        _failDetail.alignment = TextAnchor.MiddleCenter;
        _failOverlay.transform.Find("FailLabel").GetComponent<Text>().text = "SHIFT FAILED";

        MakeButton("RetryButton", _failOverlay.transform, 320, 1060, 440, 140, "RETRY", Restart);
        MakeButton("StressButton", _canvas.transform, 700, 1480, 356, 80, "stress +100", () =>
        {
            _stress += 100;
            RebuildStress();
        });
    }

    private void OnDropped(int service, Vector2 screenPosition)
    {
        _dragging = false;
        if (!_running) return;

        foreach (var lane in _lanes)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(lane.Rect, screenPosition, null)) continue;

            if (lane.Accepts(service) && Take(service))
            {
                lane.StartService(service);
            }
            else
            {
                lane.Refuse();
                _audio.PlayOneShot(_reject);
            }
            return;
        }
    }

    private bool Take(int service)
    {
        if (VehiclePool < 0) return true;
        if (_available[service] <= 0) return false;
        _available[service] -= 1;
        _tray[service].Available = _available[service];
        _tray[service].Refresh(ServiceColours[service]);
        return true;
    }

    private void Restart()
    {
        _timeLeft = ShiftSeconds;
        _served = 0;
        _running = true;
        _accumulated = 0f;
        _startedAt = Time.realtimeSinceStartup;
        for (int s = 0; s < ServiceNames.Length; s++)
        {
            _available[s] = VehiclePool;
            _tray[s].Available = VehiclePool;
            _tray[s].Refresh(ServiceColours[s]);
        }
        foreach (var lane in _lanes) lane.ResetLane(RandomNeeds());
        _failOverlay.SetActive(false);
    }

    private List<int> RandomNeeds()
    {
        var pool = new List<int> { 0, 1, 2, 3 };
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }
        return pool.GetRange(0, Random.Range(2, 4));
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        Measure(dt);
        if (!_running) return;

        _accumulated += dt;
        _timeLeft = Mathf.Max(0f, ShiftSeconds - _accumulated);

        foreach (var lane in _lanes)
        {
            int completed = lane.Tick(dt, ServiceSeconds);
            if (completed != -1)
            {
                _served += 1;
                if (VehiclePool >= 0)
                {
                    _available[completed] = Mathf.Min(_available[completed] + 1, VehiclePool);
                    _tray[completed].Available = _available[completed];
                    _tray[completed].Refresh(ServiceColours[completed]);
                }
                _audio.PlayOneShot(_pop);
                if (lane.IsClean) lane.ResetLane(RandomNeeds());
            }
            lane.Redraw(ServiceSeconds, ServiceColours, ServiceNames);
        }

        if (_timeLeft <= 0f) Fail();
        _hud.text = string.Format("{0:0.0}\nSERVED {1}\nBEST {2}", _timeLeft, _served, _best);
    }

    private void Fail()
    {
        _running = false;
        if (_served > _best)
        {
            _best = _served;
            PlayerPrefs.SetInt(BestKey, _best);
            PlayerPrefs.Save();
        }
        _failDetail.text = string.Format("served {0}   ·   best {1}", _served, _best);
        _failOverlay.SetActive(true);
    }

    private void Measure(float dt)
    {
        float frameMs = dt * 1000f;
        _frameMax = Mathf.Max(_frameMax, frameMs);
        if (_dragging) _dragFrameMax = Mathf.Max(_dragFrameMax, frameMs);
        _perfWindow += dt;
        if (_perfWindow < 1f) return;

        float wall = Time.realtimeSinceStartup - _startedAt;
        float drift = _accumulated - wall;
        _perf.text = string.Format(
            "fps {0:0}\nframe max {1:0.0} ms\ndrag max {2:0.0} ms\ndrift {3:+0.000;-0.000} s\nplanes {4}",
            1f / Mathf.Max(dt, 0.0001f), _frameMax, _dragFrameMax, drift, LaneCount + _stress);
        _frameMax = 0f;
        _dragFrameMax = 0f;
        _perfWindow = 0f;
    }

    // Extra aircraft, to find where frame time degrades. UGUI draws each as its own
    // Graphic, which is exactly the cost model worth measuring.
    private void RebuildStress()
    {
        for (int i = _stressRoot.childCount - 1; i >= 0; i--) Destroy(_stressRoot.GetChild(i).gameObject);
        var rng = new System.Random(12345);
        for (int i = 0; i < _stress; i++)
        {
            var go = new GameObject("S" + i, typeof(RectTransform), typeof(PolygonGraphic));
            go.transform.SetParent(_stressRoot, false);
            Place((RectTransform)go.transform, rng.Next(0, W - 160), rng.Next((int)LaneTop, 1500), 150, 60);
            var graphic = go.GetComponent<PolygonGraphic>();
            graphic.Points = Plane;
            graphic.color = new Color(0.82f, 0.85f, 0.9f, 0.18f);
            graphic.raycastTarget = false;
        }
    }

    // --- small UGUI helpers -------------------------------------------------------

    private static void Place(RectTransform rt, float x, float y, float w, float h)
    {
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(w, h);
    }

    private static Image NewImage(string name, Transform parent, float x, float y, float w, float h)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Place((RectTransform)go.transform, x, y, w, h);
        return go.GetComponent<Image>();
    }

    private Text NewText(string name, Transform parent, float x, float y, float w, float h,
        int size, Color colour)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        Place((RectTransform)go.transform, x, y, w, h);
        var text = go.GetComponent<Text>();
        text.font = _font;
        text.fontSize = size;
        text.color = colour;
        text.raycastTarget = false;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        return text;
    }

    private void MakeButton(string name, Transform parent, float x, float y, float w, float h,
        string label, UnityEngine.Events.UnityAction onClick)
    {
        var image = NewImage(name, parent, x, y, w, h);
        image.color = new Color(0.23f, 0.25f, 0.32f);
        var button = image.gameObject.AddComponent<Button>();
        button.onClick.AddListener(onClick);
        var text = NewText("Label", image.transform, 0, 0, w, h, 40, Color.white);
        text.alignment = TextAnchor.MiddleCenter;
        text.text = label;
    }
}

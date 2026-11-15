// Kit v0 drag helper: one finger picks something up, a ghost follows it, the game decides
// what the drop means. Drag is the line's physical action (one-mechanic games, C02 §3).
//
// Rules learned in 005's prototype and the engine spike:
//  - the ghost lives on its own top canvas, so a drag never dies when the game re-lays out
//    or re-parents the thing being dragged (005 prototype: a drag on an arrival row died
//    when that aircraft landed);
//  - the game is asked for its verdict on every move, so valid targets can light up
//    while the finger is still down (feedback in under a second).
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gabella.Kit
{
    public class DragSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>Whatever the game wants to know about what is being dragged.</summary>
        public object Payload;

        public Func<DragSource, bool> CanDrag;
        public Action<DragSource, Vector2> Began;
        public Action<DragSource, Vector2> Moved;
        public Action<DragSource, Vector2> Dropped;

        /// <summary>Size and colour of the ghost, in reference units.</summary>
        public Vector2 GhostSize = new Vector2(120, 120);
        public Color GhostColor = new Color(1, 1, 1, 0.85f);
        public string GhostText = "";

        public static DragSource Active { get; private set; }

        /// <summary>How far the ghost floats above the finger, in reference units.</summary>
        public const float Lift = 90f;

        /// <summary>
        /// Screen position of the ghost's centre on the last move: what the player sees and aims
        /// with. The finger is Lift below it, so a game that resolves drops under the finger alone
        /// misses drops aimed with the ghost (005 M1: 6 "nowhere" drops on a phone).
        /// </summary>
        public static Vector2 GhostScreen { get; private set; }

        private static Canvas _ghostCanvas;
        private static Image _ghost;
        private static Text _ghostLabel;

        public bool Dragging => Active == this;

        public void OnBeginDrag(PointerEventData e)
        {
            if (Active != null) return;
            if (CanDrag != null && !CanDrag(this)) return;
            Active = this;
            EnsureGhost();
            ((RectTransform)_ghost.transform).sizeDelta = GhostSize;
            _ghost.color = GhostColor;
            _ghostLabel.text = GhostText;
            _ghost.gameObject.SetActive(true);
            MoveGhost(e.position);
            Began?.Invoke(this, e.position);
        }

        public void OnDrag(PointerEventData e)
        {
            if (Active != this) return;
            MoveGhost(e.position);
            Moved?.Invoke(this, e.position);
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (Active != this) return;
            End();
            Dropped?.Invoke(this, e.position);
        }

        private void OnDisable()
        {
            if (Active == this) End();
        }

        private static void End()
        {
            Active = null;
            if (_ghost != null) _ghost.gameObject.SetActive(false);
        }

        private static void EnsureGhost()
        {
            if (_ghost != null) return;
            _ghostCanvas = KitUi.CreateCanvas("DragGhostCanvas", 1000);
            _ghostCanvas.GetComponent<GraphicRaycaster>().enabled = false;
            _ghost = KitUi.Box("Ghost", _ghostCanvas.transform, 0, 0, 120, 120, Color.white);
            var rt = (RectTransform)_ghost.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            _ghostLabel = KitUi.Label("Label", _ghost.transform, 0, 0, 0, 0, 44, new Color(0.08f, 0.09f, 0.11f), TextAnchor.MiddleCenter);
            var lrt = (RectTransform)_ghostLabel.transform;
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one; lrt.pivot = new Vector2(0.5f, 0.5f);
            lrt.offsetMin = lrt.offsetMax = Vector2.zero;
            _ghost.gameObject.SetActive(false);
        }

        private static void MoveGhost(Vector2 screen)
        {
            var root = (RectTransform)_ghostCanvas.transform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screen, null, out Vector2 local);
            // lift the ghost above the finger so the finger does not hide what is carried
            ((RectTransform)_ghost.transform).anchoredPosition = local + new Vector2(0, Lift);
            GhostScreen = screen + new Vector2(0, Lift * _ghostCanvas.scaleFactor);
        }
    }
}

// One service vehicle in the bottom tray. Drag source. THROWAWAY.
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrayButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int Service;
    public int Available = -1;  // -1 == unlimited

    public Action<int, Vector2> OnDropped;
    public Action<int> OnDragStarted;
    public Action OnDragMoved;

    private RectTransform _preview;
    private Image _body;
    private Canvas _canvas;

    public void Build(Image body, RectTransform preview, Canvas canvas)
    {
        _body = body;
        _preview = preview;
        _canvas = canvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Available == 0) return;
        _preview.gameObject.SetActive(true);
        _preview.GetComponent<Image>().color = _body.color;
        MovePreview(eventData);
        OnDragStarted?.Invoke(Service);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_preview.gameObject.activeSelf) return;
        MovePreview(eventData);
        OnDragMoved?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_preview.gameObject.activeSelf) return;
        _preview.gameObject.SetActive(false);
        OnDropped?.Invoke(Service, eventData.position);
    }

    private void MovePreview(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)_canvas.transform,
            eventData.position,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 local);
        _preview.anchoredPosition = local;
    }

    public void Refresh(Color colour)
    {
        _body.color = Available == 0 ? colour * 0.35f : colour;
    }
}

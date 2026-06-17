using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DraggableBuildItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector, SerializeField] private BuildingData data;

    [Header("World Ghost")]
    [Tooltip("Sorting Layer used for the ghost preview")]
    [SerializeField] private string ghostSortingLayer = "Default";

    [SerializeField] private int ghostSortingOrder = 100;

    private Camera _cam;
    private GameObject _ghost;
    private SpriteRenderer _ghostRenderer;
    private BuildSlot _hoveredSlot;

    public void SetData(BuildingData newData)
    {
        data = newData;

        Image image = GetComponent<Image>();

        if (data != null && data.icon != null)
            image.sprite = data.icon;
    }

    private void Awake()
    {
        _cam = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data == null)
        {
            Debug.LogWarning($"{name}: Cannot start drag. BuildingData is missing.");
            return;
        }

        if (_cam == null)
            _cam = Camera.main;

        if (_cam == null)
        {
            Debug.LogWarning($"{name}: Cannot start drag. Camera is missing.");
            return;
        }

        _ghost = new GameObject("BuildGhost");
        _ghostRenderer = _ghost.AddComponent<SpriteRenderer>();

        _ghostRenderer.sprite = data.worldSprite != null ? data.worldSprite : data.icon;
        _ghostRenderer.color = new Color(1f, 1f, 1f, 0.6f);
        _ghostRenderer.sortingLayerName = ghostSortingLayer;
        _ghostRenderer.sortingOrder = ghostSortingOrder;

        MoveGhost(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_ghost == null)
            return;

        MoveGhost(eventData.position);

        BuildSlot slot = SlotUnder(eventData.position);

        if (slot != _hoveredSlot)
        {
            if (_hoveredSlot != null)
                _hoveredSlot.ResetHighlight();

            _hoveredSlot = slot;
        }

        if (_hoveredSlot != null)
            _hoveredSlot.ShowHighlight(_hoveredSlot.CanAccept(data));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        BuildSlot slot = SlotUnder(eventData.position);

        if (slot != null && slot.CanAccept(data))
        {
            bool paid = ResourceManager.Instance == null || ResourceManager.Instance.TrySpend(data.costs);

            if (paid)
            {
                GameObject placedObject = slot.Place(data);

                if (placedObject != null)
                    Debug.Log($"Build item placed: {data.displayName}");
                else
                    Debug.LogWarning($"Failed to place build item: {data.displayName}");
            }
            else
            {
                Debug.LogWarning($"Not enough resources to build: {data.displayName}");
            }
        }

        if (_hoveredSlot != null)
            _hoveredSlot.ResetHighlight();

        _hoveredSlot = null;

        if (_ghost != null)
            Destroy(_ghost);

        _ghost = null;
    }

    private void MoveGhost(Vector2 screenPosition)
    {
        if (_ghost == null)
            return;

        _ghost.transform.position = ScreenToWorld(screenPosition);
    }

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        if (_cam == null)
            _cam = Camera.main;

        if (_cam == null)
            return Vector3.zero;

        float depth = _cam.orthographic ? 10f : -_cam.transform.position.z;

        Vector3 worldPosition = _cam.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, depth)
        );

        worldPosition.z = 0f;
        return worldPosition;
    }

    private BuildSlot SlotUnder(Vector2 screenPosition)
    {
        Vector3 worldPosition = ScreenToWorld(screenPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        return hit != null ? hit.GetComponentInParent<BuildSlot>() : null;
    }
}

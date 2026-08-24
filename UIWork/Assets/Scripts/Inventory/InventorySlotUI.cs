using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour,
    IPointerClickHandler,
    IInitializePotentialDragHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    // Добавлено для подсветки источника и возможной цели переноса.
    private static readonly Color NormalColor = new(0.15f, 0.15f, 0.15f, 1f);
    private static readonly Color DragSourceColor = new(0.95f, 0.65f, 0.15f, 1f);
    private static readonly Color ValidTargetColor = new(0.20f, 0.70f, 0.25f, 1f);
    private static readonly Color InvalidTargetColor = new(0.75f, 0.20f, 0.20f, 1f);

    private InventoryUI _inventoryUI;
    private InventorySlotArea _area;
    private int _index;

    private Image _background;

    // Нужен, чтобы OnDrag и OnEndDrag работали только после успешного BeginDrag.
    private bool _dragStartedHere;

    public InventorySlotArea Area => _area;
    public int Index => _index;

    public void Initialize(InventoryUI inventoryUI, InventorySlotArea area, int index)
    {
        _inventoryUI = inventoryUI;
        _area = area;
        _index = index;

        // Image используется только для изменения цвета слота.
        _background = GetComponent<Image>();
        ResetHighlight();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_inventoryUI == null)
        {
            return;
        }

        bool shiftPressed =
            Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

        bool exactSplit =
            eventData.button == PointerEventData.InputButton.Right &&
            shiftPressed;

        _inventoryUI.HandleSlotClick(this, eventData.button, exactSplit);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Начинаем drag при первом реальном движении мыши,
            // не ожидая стандартного порога EventSystem.
            eventData.useDragThreshold = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStartedHere = false;

        if (_inventoryUI == null ||
            eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        // false означает, что слот пустой или перенос из него запрещён.
        _dragStartedHere = _inventoryUI.BeginDrag(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_dragStartedHere || _inventoryUI == null)
        {
            return;
        }

        _inventoryUI.Drag(this, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_dragStartedHere || _inventoryUI == null)
        {
            return;
        }

        _dragStartedHere = false;
        _inventoryUI.EndDrag(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_inventoryUI == null || eventData.pointerDrag == null)
        {
            return;
        }

        // pointerDrag хранит слот, с которого началось перетаскивание.
        InventorySlotUI source =
            eventData.pointerDrag.GetComponent<InventorySlotUI>();

        if (source != null)
        {
            _inventoryUI.Drop(source, this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Во время drag подсвечиваем слот под курсором.
        if (_inventoryUI != null && _inventoryUI.IsDragging)
        {
            _inventoryUI.SetDragTarget(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_inventoryUI != null)
        {
            _inventoryUI.ClearDragTarget(this);
        }
    }

    public void SetSourceHighlight(bool active)
    {
        if (_background != null)
        {
            _background.color = active ? DragSourceColor : NormalColor;
        }
    }

    public void SetTargetHighlight(bool active, bool valid)
    {
        if (_background == null)
        {
            return;
        }

        if (!active)
        {
            _background.color = NormalColor;
            return;
        }

        _background.color = valid ? ValidTargetColor : InvalidTargetColor;
    }

    public void ResetHighlight()
    {
        if (_background != null)
        {
            _background.color = NormalColor;
        }
    }
}
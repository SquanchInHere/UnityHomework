using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private const int InventoryColumns = 9;
    private const int InventoryRows = 3;
    private const int CraftingColumns = 2;
    private const int CraftingRows = 2;
    private const float SlotSize = 56f;
    private const float SlotSpacing = 6f;

    private InventorySystem _inventorySystem;

    private GameObject _canvasObject;
    private GameObject _background;
    private GameObject _inventoryPanel;
    private GameObject _splitPanel;

    private InventorySlotUI[] _inventorySlotUI;
    private InventorySlotUI[] _craftingSlotUI;
    private InventorySlotUI _resultSlotUI;

    private Text[] _inventoryItemNames;
    private Text[] _inventoryAmounts;

    private Text[] _craftingItemNames;
    private Text[] _craftingAmounts;

    private Text _resultItemName;
    private Text _resultAmount;

    private Text _cursorItemName;
    private Text _cursorAmount;

    private InputField _splitInput;

    private InventorySlotUI _splitSource;

    private bool _isOpen;

    // Эти три поля добавлены только для drag-and-drop.
    private InventorySlotUI _dragSource;
    private InventorySlotUI _dragTarget;
    private GameObject _dragPreview;
    private bool _isInitialized;

    public bool IsOpen => _isOpen;
    public bool IsDragging => _dragSource != null;


    public void Initialize(InventorySystem inventorySystem)
    {
        if (inventorySystem == null)
        {
            return;
        }

        _inventorySystem = inventorySystem;

        CreateCanvas();
        CreateInventoryWindow();
        CreateCursorUI();
        CreateSplitWindow();

        Refresh();
        SetOpen(false);

        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            SetOpen(!_isOpen);
        }

        if (_isOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_splitPanel != null && _splitPanel.activeSelf)
            {
                CloseSplitWindow();
            }
            else
            {
                SetOpen(false);
            }
        }

        if (_isOpen)
        {
            UpdateCursorUI();
        }
    }

    private void LateUpdate()
    {
        if (!_isInitialized || !_isOpen)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HandleSlotClick(InventorySlotUI slot, PointerEventData.InputButton button, bool exactSplit)
    {
        if (slot == null || _inventorySystem == null)
        {
            return;
        }

        if (exactSplit)
        {
            OpenSplitWindow(slot);
            return;
        }

        _inventorySystem.HandleSlotClick(
            slot.Area,
            slot.Index,
            button,
            false
        );
        Refresh();
    }

    public bool BeginDrag(InventorySlotUI slot, PointerEventData eventData)
    {
        if (!_isOpen || slot == null || _inventorySystem == null)
            return false;

        if (slot.Area == InventorySlotArea.Result)
            return false;

        InventorySlotData data = _inventorySystem.GetSlot(slot.Area, slot.Index);

        if (data == null || data.IsEmpty)
        {
            return false;
        }

        // Очищаем возможный незавершённый предыдущий перенос.
        FinishDrag();

        _dragSource = slot;
        _dragSource.SetSourceHighlight(true);
        CreateDragPreview(data, eventData.position);
        return true;
    }

    public void Drag(InventorySlotUI slot, PointerEventData eventData)
    {
        if (_dragSource == null || slot != _dragSource || _dragPreview == null)
        {
            return;
        }

        // Preview следует за экранной позицией курсора.
        _dragPreview.transform.position = eventData.position;
    }

    public void EndDrag(InventorySlotUI slot)
    {
        if (_dragSource != slot)
        {
            return;
        }

        // Не обнуляем _dragSource раньше: FinishDrag должен снять с него подсветку.
        FinishDrag();
    }

    public void Drop(InventorySlotUI source, InventorySlotUI target)
    {
        if (_inventorySystem == null ||
            source == null ||
            target == null ||
            source != _dragSource)
        {
            return;
        }

        bool transferred = _inventorySystem.TransferSlot(
            source.Area,
            source.Index,
            target.Area,
            target.Index);

        if (transferred)
        {
            Refresh();
        }
    }

    public void SetDragTarget(InventorySlotUI target)
    {
        if (_dragSource == null || target == null || target == _dragSource)
        {
            return;
        }

        if (_dragTarget != null && _dragTarget != target)
        {
            _dragTarget.ResetHighlight();
        }

        _dragTarget = target;

        bool valid = _inventorySystem.CanTransferSlot(
            _dragSource.Area,
            _dragSource.Index,
            target.Area,
            target.Index);

        _dragTarget.SetTargetHighlight(true, valid);
    }

    public void ClearDragTarget(InventorySlotUI target)
    {
        if (_dragTarget == null || _dragTarget != target)
        {
            return;
        }

        _dragTarget.ResetHighlight();
        _dragTarget = null;
    }

    private void CreateDragPreview(InventorySlotData data, Vector2 screenPosition)
    {
        _dragPreview = CreateImage(
            "DragPreview",
            _canvasObject.transform,
            new Color(0.07f, 0.07f, 0.07f, 0.92f));

        RectTransform previewRect = _dragPreview.GetComponent<RectTransform>();
        previewRect.sizeDelta = new Vector2(76f, 76f);
        previewRect.position = screenPosition;

        // Preview не должен перехватывать raycast, иначе целевой слот не получит OnDrop.
        Image previewImage = _dragPreview.GetComponent<Image>();
        previewImage.raycastTarget = false;

        CanvasGroup canvasGroup = _dragPreview.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0.92f;

        Text itemName = CreateText(
            "ItemName",
            _dragPreview.transform,
            GetShortName(data.Item.DisplayName),
            18,
            TextAnchor.MiddleCenter,
            Vector2.zero,
            new Vector2(70f, 48f));

        itemName.color = data.Item.IconColor;

        CreateText(
            "Amount",
            _dragPreview.transform,
            data.Amount.ToString(),
            18,
            TextAnchor.LowerRight,
            new Vector2(-4f, 4f),
            new Vector2(45f, 28f));

        _dragPreview.transform.SetAsLastSibling();
    }

    private void FinishDrag()
    {
        if (_dragSource != null)
        {
            _dragSource.ResetHighlight();
        }

        if (_dragTarget != null)
        {
            _dragTarget.ResetHighlight();
        }

        if (_dragPreview != null)
        {
            Destroy(_dragPreview);
        }

        _dragSource = null;
        _dragTarget = null;
        _dragPreview = null;
    }

    private void CreateCanvas()
    {
        _canvasObject = new GameObject("InventoryCanvas");

        Canvas canvas = _canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = _canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        _canvasObject.AddComponent<GraphicRaycaster>();

        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();

        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
        }

        // Drag начнётся после движения на 4 пикселя, а не после стандартных 10.
        eventSystem.pixelDragThreshold = 4;

        StandaloneInputModule standaloneInputModule =
            eventSystem.GetComponent<StandaloneInputModule>();

        if (standaloneInputModule != null)
        {
            // Destroy выполняется только в конце кадра, поэтому сначала
            // отключаем старый модуль сразу, чтобы он не конфликтовал с новым.
            standaloneInputModule.enabled = false;
            Destroy(standaloneInputModule);
        }

        InputSystemUIInputModule inputSystemModule =
            eventSystem.GetComponent<InputSystemUIInputModule>();

        if (inputSystemModule == null)
        {
            inputSystemModule =
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }

        // Принудительно включаем модуль и назначаем Position, Click и Drag.
        // Без этих actions OnBeginDrag, OnDrag и OnDrop не вызываются.
        inputSystemModule.enabled = true;
        inputSystemModule.AssignDefaultActions();
    }

    private void CreateInventoryWindow()
    {
        _background = CreateImage(
            "Background",
            _canvasObject.transform,
            new Color(0f, 0f, 0f, 0.65f));

        SetRect(_background.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero);

        _inventoryPanel = CreateImage(
            "InventoryPanel",
            _canvasObject.transform,
            new Color(0.08f, 0.08f, 0.08f, 0.98f));

        RectTransform panelRect = _inventoryPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(1000f, 600f);
        panelRect.anchoredPosition = Vector2.zero;

        CreateText(
            "InventoryTitle",
            _inventoryPanel.transform,
            "ИНВЕНТАРЬ",
            28,
            TextAnchor.MiddleCenter,
            new Vector2(0f, 235f),
            new Vector2(600f, 50f));

        CreateText(
            "InventoryHint",
            _inventoryPanel.transform,
            "ЛКМ — взять/положить   ПКМ — половина/1   Shift + ПКМ — точное количество",
            15,
            TextAnchor.MiddleCenter,
            new Vector2(0f, -260f),
            new Vector2(900f, 40f));

        CreateInventoryGrid();
        CreateCraftingArea();
    }

    private void CreateInventoryGrid()
    {
        GameObject gridObject = new GameObject("InventoryGrid");
        gridObject.transform.SetParent(_inventoryPanel.transform, false);

        RectTransform rect = gridObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(552f, 180f);
        rect.anchoredPosition = new Vector2(-185f, -35f);

        GridLayoutGroup grid = gridObject.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(SlotSize, SlotSize);
        grid.spacing = new Vector2(SlotSpacing, SlotSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = InventoryColumns;
        grid.childAlignment = TextAnchor.MiddleCenter;

        _inventorySlotUI = new InventorySlotUI[27];
        _inventoryItemNames = new Text[27];
        _inventoryAmounts = new Text[27];

        for (int i = 0; i < 27; i++)
        {
            InventorySlotUI slot = CreateSlot(
                gridObject.transform,
                InventorySlotArea.Inventory,
                i,
                out Text itemName,
                out Text amount);

            _inventorySlotUI[i] = slot;
            _inventoryItemNames[i] = itemName;
            _inventoryAmounts[i] = amount;
        }
    }

    private void CreateCraftingArea()
    {
        CreateText(
            "CraftingTitle",
            _inventoryPanel.transform,
            "КРАФТ",
            28,
            TextAnchor.MiddleCenter,
            new Vector2(280f, 205f),
            new Vector2(300f, 50f));

        GameObject gridObject = new GameObject("CraftingGrid");
        gridObject.transform.SetParent(_inventoryPanel.transform, false);

        RectTransform rect = gridObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(118f, 118f);
        rect.anchoredPosition = new Vector2(250f, 60f);

        GridLayoutGroup grid = gridObject.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(SlotSize, SlotSize);
        grid.spacing = new Vector2(SlotSpacing, SlotSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = CraftingColumns;
        grid.childAlignment = TextAnchor.MiddleCenter;

        _craftingSlotUI = new InventorySlotUI[4];
        _craftingItemNames = new Text[4];
        _craftingAmounts = new Text[4];

        for (int i = 0; i < 4; i++)
        {
            InventorySlotUI slot = CreateSlot(
                gridObject.transform,
                InventorySlotArea.Crafting,
                i,
                out Text itemName,
                out Text amount);

            _craftingSlotUI[i] = slot;
            _craftingItemNames[i] = itemName;
            _craftingAmounts[i] = amount;
        }

        CreateText(
            "CraftArrow",
            _inventoryPanel.transform,
            "→",
            40,
            TextAnchor.MiddleCenter,
            new Vector2(350f, 60f),
            new Vector2(60f, 60f));

        GameObject resultObject = CreateImage(
            "CraftingResult",
            _inventoryPanel.transform,
            new Color(0.16f, 0.16f, 0.16f, 1f));

        RectTransform resultRect = resultObject.GetComponent<RectTransform>();
        resultRect.anchorMin = new Vector2(0.5f, 0.5f);
        resultRect.anchorMax = new Vector2(0.5f, 0.5f);
        resultRect.pivot = new Vector2(0.5f, 0.5f);
        resultRect.sizeDelta = new Vector2(80f, 80f);
        resultRect.anchoredPosition = new Vector2(430f, 60f);

        _resultSlotUI = resultObject.AddComponent<InventorySlotUI>();
        _resultSlotUI.Initialize(this, InventorySlotArea.Result, 0);

        _resultItemName = CreateText(
            "ResultName",
            resultObject.transform,
            "",
            15,
            TextAnchor.MiddleCenter,
            Vector2.zero,
            new Vector2(70f, 45f));

        _resultAmount = CreateText(
            "ResultAmount",
            resultObject.transform,
            "",
            18,
            TextAnchor.LowerRight,
            new Vector2(-3f, 3f),
            new Vector2(35f, 25f));
    }

    private InventorySlotUI CreateSlot(
        Transform parent,
        InventorySlotArea area,
        int index,
        out Text itemName,
        out Text amount)
    {
        GameObject slotObject = CreateImage(
            $"Slot_{area}_{index}",
            parent,
            new Color(0.15f, 0.15f, 0.15f, 1f));

        InventorySlotUI slot = slotObject.AddComponent<InventorySlotUI>();
        slot.Initialize(this, area, index);

        itemName = CreateText(
            "ItemName",
            slotObject.transform,
            "",
            18,
            TextAnchor.MiddleCenter,
            Vector2.zero,
            new Vector2(52f, 38f));

        amount = CreateText(
            "Amount",
            slotObject.transform,
            "",
            18,
            TextAnchor.LowerRight,
            new Vector2(-3f, 3f),
            new Vector2(38f, 24f));

        return slot;
    }

    private void CreateCursorUI()
    {
        GameObject cursorObject = CreateImage(
            "CursorStack",
            _canvasObject.transform,
            new Color(0.1f, 0.1f, 0.1f, 0.9f));

        // Декоративный блок не должен мешать выбору слота под мышью.
        cursorObject.GetComponent<Image>().raycastTarget = false;

        RectTransform rect = cursorObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);
        rect.sizeDelta = new Vector2(110f, 55f);
        rect.anchoredPosition = new Vector2(20f, 20f);

        _cursorItemName = CreateText(
            "CursorItem",
            cursorObject.transform,
            "",
            16,
            TextAnchor.MiddleLeft,
            new Vector2(8f, 0f),
            new Vector2(70f, 55f));

        _cursorAmount = CreateText(
            "CursorAmount",
            cursorObject.transform,
            "",
            18,
            TextAnchor.MiddleRight,
            new Vector2(-5f, 0f),
            new Vector2(30f, 55f));
    }

    private void CreateSplitWindow()
    {
        _splitPanel = CreateImage(
            "SplitPanel",
            _canvasObject.transform,
            new Color(0.06f, 0.06f, 0.06f, 1f));

        RectTransform panelRect = _splitPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(350f, 220f);
        panelRect.anchoredPosition = Vector2.zero;

        CreateText(
            "SplitTitle",
            _splitPanel.transform,
            "РАЗДЕЛИТЬ СТАК",
            24,
            TextAnchor.MiddleCenter,
            new Vector2(0f, 65f),
            new Vector2(300f, 45f));

        _splitInput = CreateInputField(
            _splitPanel.transform,
            new Vector2(0f, 10f),
            new Vector2(180f, 50f));

        CreateButton(
            _splitPanel.transform,
            "ВЗЯТЬ",
            new Vector2(-75f, -60f),
            new Vector2(130f, 45f),
            ConfirmSplit);

        CreateButton(
            _splitPanel.transform,
            "ОТМЕНА",
            new Vector2(75f, -60f),
            new Vector2(130f, 45f),
            CloseSplitWindow);
    }

    private void OpenSplitWindow(InventorySlotUI slot)
    {
        if (slot.Area == InventorySlotArea.Result)
        {
            return;
        }

        InventorySlotData data = _inventorySystem.GetSlot(slot.Area, slot.Index);

        if (data == null || data.IsEmpty)
        {
            return;
        }

        if (!_inventorySystem.CursorSlot.IsEmpty)
        {
            return;
        }

        if (data.Amount <= 1)
        {
            return;
        }

        _splitSource = slot;
        _splitInput.text = Mathf.CeilToInt(data.Amount / 2f).ToString();
        _splitPanel.SetActive(true);
        _splitInput.Select();
        _splitInput.ActivateInputField();
    }

    private void ConfirmSplit()
    {
        if (_splitSource == null)
        {
            CloseSplitWindow();
            return;
        }

        if (!int.TryParse(_splitInput.text, out int amount))
        {
            return;
        }

        InventorySlotData source = _inventorySystem.GetSlot(
            _splitSource.Area,
            _splitSource.Index);

        if (source == null || source.IsEmpty)
        {
            CloseSplitWindow();
            return;
        }

        amount = Mathf.Clamp(amount, 1, source.Amount);

        if (_inventorySystem.TakeExactFromSlot(
            _splitSource.Area,
            _splitSource.Index,
            amount))
        {
            CloseSplitWindow();
            Refresh();
        }
    }

    private void CloseSplitWindow()
    {
        _splitSource = null;
        _splitPanel.SetActive(false);
    }

    private void Refresh()
    {
        for (int i = 0; i < _inventorySlotUI.Length; i++)
        {
            InventorySlotData slot = _inventorySystem.InventorySlots[i];
            RefreshSlot(_inventoryItemNames[i], _inventoryAmounts[i], slot);
        }

        for (int i = 0; i < _craftingSlotUI.Length; i++)
        {
            InventorySlotData slot = _inventorySystem.CraftingSlots[i];
            RefreshSlot(_craftingItemNames[i], _craftingAmounts[i], slot);
        }

        InventorySlotData result = _inventorySystem.GetCraftingResult();
        RefreshSlot(_resultItemName, _resultAmount, result);
    }

    private void RefreshSlot(Text itemName, Text amount, InventorySlotData slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            itemName.text = "";
            amount.text = "";
            return;
        }

        itemName.text = GetShortName(slot.Item.DisplayName);
        itemName.color = slot.Item.IconColor;
        amount.text = slot.Amount.ToString();
    }

    private void UpdateCursorUI()
    {
        if (!_isInitialized)
        {
            return;
        }

        if (_inventorySystem == null)
        {
            return;
        }

        if (_cursorItemName == null || _cursorAmount == null)
        {
            return;
        }

        InventorySlotData cursor = _inventorySystem.CursorSlot;

        if (cursor == null || cursor.IsEmpty || cursor.Item == null)
        {
            _cursorItemName.text = "";
            _cursorAmount.text = "";
            return;
        }

        _cursorItemName.text = GetShortName(cursor.Item.DisplayName);
        _cursorItemName.color = cursor.Item.IconColor;
        _cursorAmount.text = cursor.Amount.ToString();
    }

    private string GetShortName(string displayName)
    {
        if (string.IsNullOrEmpty(displayName))
        {
            return "";
        }

        if (displayName.Length <= 3)
        {
            return displayName;
        }

        return displayName.Substring(0, 3);
    }

    private GameObject CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject objectInstance = new GameObject(objectName);
        objectInstance.transform.SetParent(parent, false);

        Image image = objectInstance.AddComponent<Image>();
        image.color = color;

        return objectInstance;
    }

    private Text CreateText(
        string objectName,
        Transform parent,
        string text,
        int fontSize,
        TextAnchor alignment,
        Vector2 position,
        Vector2 size)
    {
        GameObject objectInstance = new GameObject(objectName);
        objectInstance.transform.SetParent(parent, false);

        RectTransform rect = objectInstance.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Text textComponent = objectInstance.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = Color.white;
        textComponent.raycastTarget = false;

        return textComponent;
    }

    private InputField CreateInputField(
        Transform parent,
        Vector2 position,
        Vector2 size)
    {
        GameObject objectInstance = CreateImage(
            "SplitInput",
            parent,
            new Color(0.18f, 0.18f, 0.18f, 1f));

        RectTransform rect = objectInstance.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Text text = CreateText(
            "Text",
            objectInstance.transform,
            "",
            22,
            TextAnchor.MiddleCenter,
            Vector2.zero,
            size - new Vector2(10f, 10f));

        InputField inputField = objectInstance.AddComponent<InputField>();
        inputField.textComponent = text;
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.lineType = InputField.LineType.SingleLine;
        inputField.characterLimit = 3;

        return inputField;
    }

    private void CreateButton(
        Transform parent,
        string label,
        Vector2 position,
        Vector2 size,
        UnityEngine.Events.UnityAction action)
    {
        GameObject objectInstance = CreateImage(
            label,
            parent,
            new Color(0.18f, 0.18f, 0.18f, 1f));

        RectTransform rect = objectInstance.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Button button = objectInstance.AddComponent<Button>();
        button.onClick.AddListener(action);

        CreateText(
            "Label",
            objectInstance.transform,
            label,
            16,
            TextAnchor.MiddleCenter,
            Vector2.zero,
            size);
    }

    private void SetRect(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 position)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = position;
    }

    private void SetOpen(bool value)
    {
        _isOpen = value;

        if (!value)
        {
            FinishDrag();
        }

        if (_background != null)
        {
            _background.SetActive(value);
        }

        if (_inventoryPanel != null)
        {
            _inventoryPanel.SetActive(value);
        }

        if (!value && _splitPanel != null)
        {
            CloseSplitWindow();
        }

        if (_isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Refresh();
            UpdateCursorUI();
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
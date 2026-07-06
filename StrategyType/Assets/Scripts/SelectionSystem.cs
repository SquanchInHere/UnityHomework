using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SelectionSystem : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _dragThreshold = 10f;

    private List<Unit> _selectedUnits = new List<Unit>();
    private Vector3 _dragStartScreenPos;
    private bool _isDragging;
    private bool _isSelecting;
    private Texture2D _boxTexture;

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;

        _boxTexture = new Texture2D(1, 1);
        _boxTexture.SetPixel(0, 0, Color.white);
        _boxTexture.Apply();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleLeftDown();

        if (Input.GetMouseButtonUp(0))
            HandleLeftUp();

        if (_isDragging)
            UpdateDrag();

        if (Input.GetMouseButtonDown(1))
            HandleRightClick();
    }

    private void HandleLeftDown()
    {
        if (BuildingPlacer.IsAnyPlacing) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        _dragStartScreenPos = Input.mousePosition;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var building = hit.collider.GetComponentInParent<Building>();
            if (building != null)
            {
                ClearSelection();
                building.SpawnUnits();
                return;
            }

            var unit = hit.collider.GetComponentInParent<Unit>();
            if (unit != null)
            {
                ClearSelection();
                SelectUnit(unit);
                return;
            }
        }

        ClearSelection();
        _isDragging = true;
        _isSelecting = false;
    }

    private void HandleLeftUp()
    {
        if (!_isDragging) return;

        _isDragging = false;

        if (_isSelecting)
        {
            var rect = GetScreenRect(_dragStartScreenPos, Input.mousePosition);
            SelectUnitsInRect(rect);
        }

        _isSelecting = false;
    }

    private void UpdateDrag()
    {
        if (Vector3.Distance(_dragStartScreenPos, Input.mousePosition) > _dragThreshold)
            _isSelecting = true;
    }

    private void HandleRightClick()
    {
        if (_selectedUnits.Count == 0) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var resource = hit.collider.GetComponentInParent<ResourcePoint>();
            if (resource != null && resource.HasResources)
            {
                foreach (var unit in _selectedUnits)
                    unit.StartGathering(resource);
                return;
            }

            foreach (var unit in _selectedUnits)
                unit.MoveToCommand(hit.point);
        }
    }

    private void SelectUnitsInRect(Rect rect)
    {
        ClearSelection();

        var units = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        foreach (var unit in units)
        {
            var screenPos = _camera.WorldToScreenPoint(unit.transform.position);
            screenPos.y = Screen.height - screenPos.y;

            if (rect.Contains(screenPos))
                SelectUnit(unit);
        }
    }

    private void SelectUnit(Unit unit)
    {
        if (_selectedUnits.Contains(unit)) return;

        _selectedUnits.Add(unit);
        unit.SetSelected(true);
    }

    private void ClearSelection()
    {
        foreach (var unit in _selectedUnits)
            unit.SetSelected(false);

        _selectedUnits.Clear();
    }

    private void OnGUI()
    {
        if (!_isDragging || !_isSelecting) return;

        var rect = GetScreenRect(_dragStartScreenPos, Input.mousePosition);

        GUI.color = new Color(0, 1, 0, 0.2f);
        GUI.DrawTexture(rect, _boxTexture);

        GUI.color = new Color(0, 1, 0, 0.8f);
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 2f), _boxTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), _boxTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y, 2f, rect.height), _boxTexture);
        GUI.DrawTexture(new Rect(rect.xMax - 2f, rect.y, 2f, rect.height), _boxTexture);

        GUI.color = Color.white;
    }

    private static Rect GetScreenRect(Vector3 start, Vector3 end)
    {
        start.y = Screen.height - start.y;
        end.y = Screen.height - end.y;

        var topLeft = Vector3.Min(start, end);
        var bottomRight = Vector3.Max(start, end);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
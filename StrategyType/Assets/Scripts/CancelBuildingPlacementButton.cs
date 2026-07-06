using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CancelBuildingPlacementButton : MonoBehaviour
{
    [SerializeField] private BuildingPlacer _buildingPlacer;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(CancelPlacement);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(CancelPlacement);
    }

    private void CancelPlacement()
    {
        if (_buildingPlacer != null)
            _buildingPlacer.CancelCurrentPlacement();
    }
}

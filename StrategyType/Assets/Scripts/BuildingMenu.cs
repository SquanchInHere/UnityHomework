using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [Header("Building Placer")]
    [SerializeField] private BuildingPlacer _buildingPlacer;

    [Header("Building Recipes")]
    [SerializeField] private BuildingProductionRecipe _barracksRecipe;
    [SerializeField] private BuildingProductionRecipe _houseRecipe;

    [Header("UI Buttons")]
    [SerializeField] private Button _barracksButton;
    [SerializeField] private Button _houseButton;
    [SerializeField] private Button _cancelButton;

    private void Awake()
    {
        if (_barracksButton != null)
            _barracksButton.onClick.AddListener(StartBarracksPlacement);

        if (_houseButton != null)
            _houseButton.onClick.AddListener(StartHousePlacement);

        if (_cancelButton != null)
            _cancelButton.onClick.AddListener(CancelPlacement);
    }

    private void OnDestroy()
    {
        if (_barracksButton != null)
            _barracksButton.onClick.RemoveListener(StartBarracksPlacement);

        if (_houseButton != null)
            _houseButton.onClick.RemoveListener(StartHousePlacement);

        if (_cancelButton != null)
            _cancelButton.onClick.RemoveListener(CancelPlacement);
    }

    private void StartBarracksPlacement()
    {
        StartPlacement(_barracksRecipe);
    }

    private void StartHousePlacement()
    {
        StartPlacement(_houseRecipe);
    }

    private void StartPlacement(BuildingProductionRecipe recipe)
    {
        if (_buildingPlacer == null)
        {
            Debug.LogWarning("BuildingPlacer is missing.");
            return;
        }

        if (recipe == null)
        {
            Debug.LogWarning("Building recipe is missing.");
            return;
        }

        _buildingPlacer.StartPlacing(recipe);
    }

    private void CancelPlacement()
    {
        if (_buildingPlacer != null)
            _buildingPlacer.CancelCurrentPlacement();
    }
}

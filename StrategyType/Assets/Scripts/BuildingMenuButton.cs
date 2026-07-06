using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuildingMenuButton : MonoBehaviour
{
    [SerializeField] private BuildingPlacer _buildingPlacer;
    [SerializeField] private BuildingProductionRecipe _buildingRecipe;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(StartPlacement);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(StartPlacement);
    }

    private void StartPlacement()
    {
        if (_buildingPlacer == null)
        {
            Debug.LogWarning("BuildingPlacer is missing.");
            return;
        }

        if (_buildingRecipe == null)
        {
            Debug.LogWarning("Building recipe is missing.");
            return;
        }

        _buildingPlacer.StartPlacing(_buildingRecipe);
    }
}
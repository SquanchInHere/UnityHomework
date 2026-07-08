using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [Header("Building Placer")]
    [SerializeField] private BuildingPlacer _buildingPlacer;

    [Header("Building Recipes")]
    [SerializeField] private BuildingProductionRecipe[] _recipes;

    [Header("Generated Menu")]
    [SerializeField] private Transform _buttonsRoot;
    [SerializeField] private Button _buttonPrefab;

    [SerializeField] private Button _cancelButton;

    private void Awake()
    {
        GenerateMenu();

        if (_cancelButton != null)
            _cancelButton.onClick.AddListener(CancelPlacement);
    }

    private void OnDestroy()
    {
        if (_cancelButton != null)
            _cancelButton.onClick.RemoveListener(CancelPlacement);
    }

    private void GenerateMenu()
    {
        if (_buttonsRoot == null)
        {
            Debug.LogWarning("Buttons root is missing.");
            return;
        }

        if (_buttonPrefab == null)
        {
            Debug.LogWarning("Button prefab is missing.");
            return;
        }

        ClearMenu();

        if (_recipes == null || _recipes.Length == 0)
        {
            Debug.LogWarning("Building recipes are empty.");
            return;
        }

        foreach (var recipe in _recipes)
        {
            if (recipe == null)
                continue;

            if (recipe.Building == null)
            {
                Debug.LogWarning("Building recipe has no BuildingData.");
                continue;
            }

            CreateButton(recipe);
        }
    }

    private void CreateButton(BuildingProductionRecipe recipe)
    {
        var button = Instantiate(_buttonPrefab, _buttonsRoot);
        button.gameObject.SetActive(true);

        SetButtonText(button, GetRecipeName(recipe));

        button.onClick.AddListener(() =>
        {
            StartPlacement(recipe);
        });
    }

    private string GetRecipeName(BuildingProductionRecipe recipe)
    {
        if (recipe == null || recipe.Building == null)
            return "Unknown";

        if (!string.IsNullOrWhiteSpace(recipe.Building.DisplayName))
            return recipe.Building.DisplayName;

        if (!string.IsNullOrWhiteSpace(recipe.Building.Id))
            return recipe.Building.Id;

        return recipe.Building.Type.ToString();
    }

    private void SetButtonText(Button button, string text)
    {
        var label = button.GetComponentInChildren<Text>();

        if (label != null)
            label.text = text;
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

    private void ClearMenu()
    {
        for (var i = _buttonsRoot.childCount - 1; i >= 0; i--)
            Destroy(_buttonsRoot.GetChild(i).gameObject);
    }
}
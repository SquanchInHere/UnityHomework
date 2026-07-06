using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _gridSize = 1f;

    private bool _isPlacing;
    private BuildingProductionRecipe _recipe;
    private GameObject _previewInstance;

    private readonly Plane _groundPlane = new Plane(Vector3.up, 0f);

    public static bool IsAnyPlacing { get; private set; }

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    private void Update()
    {
        if (!_isPlacing)
            return;

        UpdatePreview();

        if (Input.GetMouseButtonDown(0) && _previewInstance != null)
            PlaceBuilding();

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacement();
    }

    public void StartPlacing(BuildingProductionRecipe recipe)
    {
        if (_isPlacing)
            CancelPlacement();

        _recipe = recipe;
        _previewInstance = Instantiate(recipe.Prefab);

        PreparePreview(_previewInstance);

        _isPlacing = true;
        IsAnyPlacing = true;
    }

    private void UpdatePreview()
    {
        if (_camera == null || _previewInstance == null)
            return;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (!_groundPlane.Raycast(ray, out var distance))
            return;

        var point = ray.GetPoint(distance);

        if (_gridSize > 0f)
        {
            point.x = Mathf.Round(point.x / _gridSize) * _gridSize;
            point.z = Mathf.Round(point.z / _gridSize) * _gridSize;
        }

        if (GroundBounds.Instance != null)
            point = GroundBounds.Instance.ClampPosition(point);

        _previewInstance.transform.position = point;
    }

    private void PlaceBuilding()
    {
        if (_recipe == null || _recipe.Prefab == null)
            return;

        var position = _previewInstance.transform.position;

        if (GroundBounds.Instance != null)
            position = GroundBounds.Instance.ClampPosition(position);

        Instantiate(_recipe.Prefab, position, Quaternion.identity);

        CancelPlacement();
    }

    private void CancelPlacement()
    {
        _isPlacing = false;
        IsAnyPlacing = false;

        if (_previewInstance != null)
            Destroy(_previewInstance);

        _previewInstance = null;
        _recipe = null;
    }

    public void CancelCurrentPlacement()
    {
        if (!_isPlacing)
            return;

        CancelPlacement();
    }

    private void PreparePreview(GameObject preview)
    {
        var colliders = _previewInstance.GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
            col.enabled = false;

        var renderers = _previewInstance.GetComponentsInChildren<Renderer>();

        foreach (var rend in renderers)
        {
            var mats = rend.materials;

            for (var i = 0; i < mats.Length; i++)
            {
                var mat = mats[i];
                var color = mat.color;

                color.a = 0.5f;
                mat.color = color;

                if (!mat.HasProperty("_Mode"))
                    continue;

                mat.SetFloat("_Mode", 2);
                mat.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetFloat("_ZWrite", 0);
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}
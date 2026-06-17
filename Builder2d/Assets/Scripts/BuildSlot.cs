using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BuildSlot : MonoBehaviour
{
    [Tooltip("Slot accepts item only if acceptedCategory matches BuildingData.category. Empty = any item")]
    [SerializeField] private string acceptedCategory = "";

    [Header("Highlight")]
    [Tooltip("SpriteRenderer used for highlight")]
    [SerializeField] private SpriteRenderer highlightRenderer;

    [SerializeField] private Color idleColor = new(1f, 1f, 1f, 0.15f);
    [SerializeField] private Color validColor = new(0.4f, 1f, 0.4f, 0.6f);
    [SerializeField] private Color invalidColor = new(1f, 0.4f, 0.4f, 0.6f);

    [Header("Unit Path")]
    [SerializeField] private Path unitPath;

    public bool IsOccupied { get; private set; }
    public GameObject CurrentBuilding { get; private set; }
    public BuildingData CurrentData { get; private set; }

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        ResetHighlight();
    }

    public bool CanAccept(BuildingData data)
    {
        if (IsOccupied)
            return false;

        if (data == null)
            return false;

        if (string.IsNullOrEmpty(acceptedCategory))
            return true;

        return acceptedCategory == data.category;
    }

    public void ShowHighlight(bool valid)
    {
        if (highlightRenderer != null)
            highlightRenderer.color = valid ? validColor : invalidColor;
    }

    public void ResetHighlight()
    {
        if (highlightRenderer != null)
            highlightRenderer.color = idleColor;
    }

    public GameObject Place(BuildingData data)
    {
        if (!CanAccept(data))
        {
            Debug.LogWarning($"{name}: Slot cannot accept item.");
            return null;
        }

        if (data.worldPrefab == null)
        {
            Debug.LogWarning($"{name}: World prefab is missing for {data.displayName}.");
            return null;
        }

        CurrentBuilding = Instantiate(data.worldPrefab, transform.position, Quaternion.identity, transform);
        CurrentData = data;
        IsOccupied = true;

        ResetHighlight();

        Building building = CurrentBuilding.GetComponent<Building>();

        if (building == null)
            building = CurrentBuilding.AddComponent<Building>();

        building.Init(this);

        Debug.Log($"{name}: Placed item {data.displayName}.");

        SpawnUnit unitSpawner = CurrentBuilding.GetComponent<SpawnUnit>();

        if (unitSpawner != null)
        {
            if (unitPath != null)
            {
                unitSpawner.SetPath(unitPath);
                Debug.Log($"{name}: Path passed to UnitProducer on {data.displayName}.");
            }
            else
            {
                Debug.LogWarning($"{name}: UnitProducer found on {data.displayName}, but unitPath is missing.");
            }
        }

        Debug.Log($"{name}: Placed item {data.displayName}.");

        return CurrentBuilding;
    }

    public void Clear()
    {
        if (CurrentBuilding != null)
            Destroy(CurrentBuilding);

        CurrentBuilding = null;
        CurrentData = null;
        IsOccupied = false;

        ResetHighlight();

        Debug.Log($"{name}: Slot cleared.");
    }

    public void Demolish()
    {
        if (!IsOccupied)
            return;

        if (CurrentData != null && ResourceManager.Instance != null)
        {
            ResourceManager.Instance.Add(CurrentData.costs, CurrentData.refundFraction);
            Debug.Log($"{name}: Demolished item {CurrentData.displayName}. Refund fraction: {CurrentData.refundFraction}");
        }

        Clear();
    }
}

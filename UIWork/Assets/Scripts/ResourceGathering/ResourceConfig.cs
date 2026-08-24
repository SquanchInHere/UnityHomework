using UnityEngine;

[CreateAssetMenu(fileName = "ResourceConfig", menuName = "Resources/ResourceConfig")]
public class ResourceConfig : ScriptableObject
{
    [SerializeField] private string _resourceId;
    [SerializeField] private string _resourceName;
    [SerializeField] private ResourceType _resourceType = ResourceType.Wood;


    [Header("Dropped item")]
    [SerializeField] private ItemDefinition _droppedItem;


    [Header("Total resource amount")]
    [Min(1)]
    [SerializeField] private int _minResourceAmount = 10;

    [Min(1)]
    [SerializeField] private int _maxResourceAmount = 20;

    [Header("Resource amount per hit")]
    [Min(1)]
    [SerializeField] private int _minAmountPerHit = 1;

    [Min(1)]
    [SerializeField] private int _maxAmountPerHit = 5;

    public string ResourceId => _resourceId;
    public string ResourceName => _resourceName;
    public ResourceType resourceType => _resourceType;
    public ItemDefinition droppedItem => _droppedItem;

    public int CreateInitAmount()
    {
        return Random.Range(_minResourceAmount, _maxResourceAmount + 1);
    }


    public int CreateAmountPerHit(int harvestPower)
    {
        int baseAmount = Random.Range(_minAmountPerHit, _maxAmountPerHit + 1);

        harvestPower = Mathf.Max(1, harvestPower);

        return baseAmount * harvestPower;
    }

}

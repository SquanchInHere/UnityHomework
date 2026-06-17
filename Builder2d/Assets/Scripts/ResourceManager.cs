using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Gold, Wood, Stone, Food, Fish, Unit }

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Serializable]
    public struct StartingResource
    {
        public ResourceType type;
        public int amount;
    }

    [Tooltip("Resources available at game start")]
    [SerializeField] private List<StartingResource> startingResources = new();

    private readonly Dictionary<ResourceType, int> _amounts = new();

    public event Action OnChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate ResourceManager found. Destroying duplicate instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            _amounts[type] = 0;

        foreach (var resource in startingResources)
            _amounts[resource.type] = Mathf.Max(0, resource.amount);
    }

    public int Get(ResourceType type)
    {
        return _amounts.TryGetValue(type, out int value) ? value : 0;
    }

    public bool CanAfford(ResourceType type, int cost)
    {
        if (cost <= 0)
            return true;

        return Get(type) >= cost;
    }

    public bool CanAfford(List<ResourceCost> costs)
    {
        if (costs == null || costs.Count == 0)
            return true;

        foreach (ResourceCost cost in costs)
        {
            if (cost == null)
                continue;

            if (cost.amount <= 0)
                continue;

            if (Get(cost.type) < cost.amount)
                return false;
        }

        return true;
    }

    public void Add(ResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        _amounts[type] = Get(type) + amount;

        Debug.Log($"Resource added: {type} +{amount}. Current amount: {_amounts[type]}");

        OnChanged?.Invoke();
    }

    public void Add(List<ResourceCost> costs, float multiplier = 1f)
    {
        if (costs == null || costs.Count == 0)
            return;

        bool changed = false;

        foreach (ResourceCost cost in costs)
        {
            if (cost == null)
                continue;

            if (cost.amount <= 0)
                continue;

            int amount = Mathf.RoundToInt(cost.amount * multiplier);

            if (amount <= 0)
                continue;

            _amounts[cost.type] = Get(cost.type) + amount;
            changed = true;

            Debug.Log($"Resource refunded: {cost.type} +{amount}. Current amount: {_amounts[cost.type]}");
        }

        if (changed)
            OnChanged?.Invoke();
    }

    public bool TrySpend(ResourceType type, int amount)
    {
        if (amount <= 0)
            return true;

        if (!CanAfford(type, amount))
        {
            Debug.LogWarning($"Not enough resource: {type}. Required: {amount}, Available: {Get(type)}");
            return false;
        }

        _amounts[type] = Get(type) - amount;

        Debug.Log($"Resource spent: {type} -{amount}. Current amount: {_amounts[type]}");

        OnChanged?.Invoke();
        return true;
    }

    public bool TrySpend(List<ResourceCost> costs)
    {
        if (costs == null || costs.Count == 0)
            return true;

        if (!CanAfford(costs))
        {
            Debug.LogWarning("Not enough resources to complete purchase.");
            return false;
        }

        bool changed = false;

        foreach (ResourceCost cost in costs)
        {
            if (cost == null)
                continue;

            if (cost.amount <= 0)
                continue;

            _amounts[cost.type] = Get(cost.type) - cost.amount;
            changed = true;

            Debug.Log($"Resource spent: {cost.type} -{cost.amount}. Current amount: {_amounts[cost.type]}");
        }

        if (changed)
            OnChanged?.Invoke();

        return true;
    }
}

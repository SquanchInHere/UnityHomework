using System;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] private ResourceConfig _config;

    [SerializeField] private bool _disableWhenDepleted = true;

    [SerializeField] private int _remainingAmount;

    private bool _isInitialized;

    public ResourceConfig Config => _config;
    public int RemainingAmount => _remainingAmount;
    public bool IsDepleted => _remainingAmount <= 0;

    public event Action<HarvestResult> Harvested;
    public event Action<ResourceNode> Depleted;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_isInitialized)
            return;

        if (_config == null || _config.droppedItem == null)
            return;

        _remainingAmount = _config.CreateInitAmount();
        _isInitialized = true;
    }

    public bool TryHarvest(int harvestPower, IResourceReceiver receiver, out HarvestResult result)
    {
        result = default;

        if (!_isInitialized)
            Initialize();

        if (!_isInitialized || IsDepleted)
            return false;

        if (receiver == null)
            return false;

        int requestedAmount = _config.CreateAmountPerHit(harvestPower);

        requestedAmount = Mathf.Min(requestedAmount, _remainingAmount);

        int acceptedAmount = receiver.Add(
            _config.droppedItem,
            requestedAmount
        );

        acceptedAmount = Mathf.Clamp(acceptedAmount, 0, requestedAmount);

        if (acceptedAmount <= 0)
            return false;

        _remainingAmount -= acceptedAmount;

        result = new HarvestResult(
            _config.droppedItem,
            acceptedAmount,
            _remainingAmount,
            IsDepleted
            );

        Harvested?.Invoke(result);

        if (IsDepleted)
            HandleDepleted();

        return true;
    }

    private void HandleDepleted()
    {
        Depleted?.Invoke(this);

        if (_disableWhenDepleted)
            gameObject.SetActive(false);
    }

    public void Refill()
    {
        if (_config == null)
            return;

        _remainingAmount = _config.CreateInitAmount();
        _isInitialized = true;
        gameObject.SetActive(true);
    }

    public void RestoreState(int remainingAmount)
    {
        _remainingAmount = Mathf.Max(0, remainingAmount);
        _isInitialized = true;

        if (IsDepleted && _disableWhenDepleted)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }
}

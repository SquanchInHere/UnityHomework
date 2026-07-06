using UnityEngine;

public class ResourcePoint : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ResourceData _data;

    [Header("Amount")]
    [SerializeField] private int _maxResources = 100;

    private int _currentResources;

    public ResourceData Data => _data;

    public ResourceType Type =>
        _data != null ? _data.Type : ResourceType.Wood;

    public bool HasResources => _currentResources > 0;

    public float ResourcePercent =>
        _maxResources <= 0 ? 0f : (float)_currentResources / _maxResources;

    private void Awake()
    {
        if (_data != null)
            _maxResources = _data.DefaultAmount;

        _currentResources = _maxResources;
    }

    public int Gather(int amount)
    {
        if (amount <= 0)
            return 0;

        if (_currentResources <= 0)
            return 0;

        var gathered = Mathf.Min(amount, _currentResources);
        _currentResources -= gathered;

        if (_currentResources <= 0)
            MarkEmpty();

        return gathered;
    }

    private void MarkEmpty()
    {
        var rend = GetComponent<Renderer>();

        if (rend != null)
            rend.material.color = Color.gray;
    }
}
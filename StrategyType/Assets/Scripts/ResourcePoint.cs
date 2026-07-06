using UnityEngine;

public class ResourcePoint : MonoBehaviour
{
    [SerializeField] private int _maxResources = 100;

    private int _currentResources;

    private void Awake()
    {
        _currentResources = _maxResources;
    }

    public bool HasResources => _currentResources > 0;

    public int Gather(int amount)
    {
        if (_currentResources <= 0)
            return 0;

        var gathered = Mathf.Min(amount, _currentResources);
        _currentResources -= gathered;

        if (_currentResources <= 0)
        {
            var rend = GetComponent<Renderer>();

            if (rend != null)
                rend.material.color = Color.gray;
        }

        return gathered;
    }

    public float ResourcePercent => (float)_currentResources / _maxResources;
}
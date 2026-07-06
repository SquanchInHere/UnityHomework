using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _spawnCount = 1;
    [SerializeField] private BuildingData _data;

    private int _storedResources;
    private float _currentHealth;

    public int StoredResources => _storedResources;
    public BuildingData Data => _data;

    public BuildingType Type =>
        _data != null ? _data.Type : BuildingType.TownCenter;

    public bool IsSpawnUnit =>
        _data != null && _data.CanProduceUnits;

    public UnitData[] Units =>
        _data != null ? _data.ProducedUnits : null;

    public float MaxHealth =>
        _data != null ? _data.MaxHealth : 300f;

    public float Armor =>
        _data != null ? _data.Armor : 0f;

    public float CurrentHealth => _currentHealth;

    private void Awake()
    {
        if (_spawnPoint == null)
            _spawnPoint = transform;

        _currentHealth = MaxHealth;
    }

    public void SpawnUnits()
    {
        if (!IsSpawnUnit || Units == null)
            return;

        foreach (UnitData unit in Units)
        {
            if (unit == null || unit.Prefab == null)
                continue;

            for (var i = 0; i < _spawnCount; i++)
            {
                var offset = Random.insideUnitSphere * 2f;
                offset.y = 0;

                Instantiate(unit.Prefab, _spawnPoint.position + offset, Quaternion.identity);
            }
        }
    }

    public void Deposit(int amount)
    {
        if (amount <= 0)
            return;

        _storedResources += amount;
    }

    public void TakeDamage(float rawDamage)
    {
        var finalDamage = Mathf.Max(0f, rawDamage - Armor);

        _currentHealth -= finalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);

        if (_currentHealth <= 0f)
            Destroy(gameObject);
    }
}
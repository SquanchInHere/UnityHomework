using UnityEngine;

public class Building : MonoBehaviour
{
    //[SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _spawnCount = 1;
    [SerializeField] private BuildingData _data;

    private int _storedResources;
    public int StoredResources => _storedResources;

    public BuildingData Data => _data;

    public BuildingType Type => _data.Type;

    public bool IsSpawnUnit => _data.CanProduceUnits;

    public UnitData[] Units => _data.ProducedUnits;

    private void Awake()
    {
        if (_spawnPoint == null)
            _spawnPoint = transform;
    }

    public void SpawnUnits()
    {
        foreach (UnitData unit in Units)
        {
            if (unit.Prefab == null)
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
        _storedResources += amount;
    }
}
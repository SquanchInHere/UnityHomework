using UnityEngine;

public class SpawnUnit : MonoBehaviour
{
    [Header("Unit")]
    [SerializeField] private MoveUnit _unitPrefab;
    [SerializeField] private Transform _spawnPoint;

    [Header("Spawn Settings")]
    [SerializeField] private bool _spawnByTimer = true;
    [SerializeField] private float _secondsPerSpawn = 3f;
    [SerializeField] private int _spawnLimit = 3;

    private Path _path;
    private float _timer;
    private int _spawnedCount = 0;

    private bool HasPath => _path != null && _path.Points != null && _path.Points.Length > 0;

    private void Update()
    {
        if (!_spawnByTimer)
            return;

        if (_spawnedCount >= _spawnLimit)
            return;

        if (!HasPath)
            return;

        if (_secondsPerSpawn <= 0f)
        {
            Debug.LogWarning($"{name}: Seconds per spawn must be greater than 0.");
            return;
        }

        _timer += Time.deltaTime;

        while (_timer >= _secondsPerSpawn)
        {
            _timer -= _secondsPerSpawn;
            Spawn();
        }
    }

    public void SetPath(Path path)
    {
        _path = path;

        if (HasPath)
            Debug.Log($"{name}: Path assigned.");
        else
            Debug.LogWarning($"{name}: Assigned path is missing or empty.");
    }

    public void Spawn()
    {
        if (_spawnedCount >= _spawnLimit)
        {
            Debug.Log($"{name}: Spawn limit reached: {_spawnedCount}/{_spawnLimit}");
            return;
        }

        if (_unitPrefab == null)
        {
            Debug.LogWarning($"{name}: Unit prefab is missing.");
            return;
        }

        if (_spawnPoint == null)
        {
            Debug.LogWarning($"{name}: Spawn point is missing.");
            return;
        }

        if (!HasPath)
        {
            Debug.LogWarning($"{name}: Path is missing. Unit was not spawned.");
            return;
        }

        MoveUnit unit = Instantiate(
            _unitPrefab,
            _spawnPoint.position,
            Quaternion.identity
        );

        unit.SetPath(_path.Points);

        _spawnedCount++;

        Debug.Log($"{name}: Spawned unit {_spawnedCount}/{_spawnLimit}.");
    }
}

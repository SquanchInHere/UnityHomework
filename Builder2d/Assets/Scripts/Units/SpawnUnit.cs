using UnityEngine;

public class SpawnUnit : MonoBehaviour
{
    [Header("Unit")]
    [SerializeField] private MoveUnit _unitPrefab;
    [SerializeField] private Transform _spawnPoint;

    [Header("Path")]
    [SerializeField] private Path _path;

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        MoveUnit unit = Instantiate(
            _unitPrefab,
            _spawnPoint.position,
            Quaternion.identity
        );

        unit.SetPath(_path.Points);
    }
}

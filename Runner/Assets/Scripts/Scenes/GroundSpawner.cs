using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [Header("Runner Object")]
    [SerializeField] private GameObject _roadPrefab;
    [SerializeField] private Transform _player;

    [Header("Obstacles")]
    [SerializeField] private GameObject _obstaclePrefabs;
    [SerializeField] private int _obstaclesPerRoad = 2;

    [Header("Coins")]
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private int _coinsPerRoad = 2;

    private float spawnZ = 0f;
    private float roadLength = 10f;

    [Header("Items spawn position")]
    [SerializeField] private float minX = -4.5f;
    [SerializeField] private float stepX = 1.5f;
    [SerializeField] private int xCells = 7;
    [SerializeField] private float[] pointsZ = { 2f, 4f, 6f, 8f };

    private int roadCount = 0;

    public void Start()
    {
        for (int i = 0; i < 10; i++)
            SpawnerRoad();
    }

    public void Update()
    {
        if (_player.position.z + 50 > spawnZ)
        {
            SpawnerRoad();
        }

        DeleteOldRoads();
    }

    public void SpawnerRoad()
    {
        GameObject road = Instantiate(_roadPrefab, new Vector3(0, 0, spawnZ), Quaternion.identity, transform);

        List<string> usedCells = new List<string>();

        if (roadCount > 2)
        {
            SpawnItemsOnRoad(road.transform);
        }

        roadCount++;
        spawnZ += roadLength;
    }

    private void DeleteOldRoads()
    {
        foreach (Transform road in transform)
        {
            if (road.position.z + roadLength < _player.position.z)
            {
                Destroy(road.gameObject);
            }
        }
    }

    private void SpawnObstacles(Transform road, float x, float z)
    {
        Instantiate(
             _obstaclePrefabs,
                new Vector3(x, 0.5f, road.position.z + z),
                Quaternion.identity,
                road
            );

    }

    private void SpawnCoins(Transform road, float x, float z)
    {
            Instantiate(
                _coinPrefab,
                new Vector3(x, 1f, road.position.z + z),
                Quaternion.identity,
                road
            );

    }

    private void SpawnItemsOnRoad(Transform road)
    {
        int obstaclesOnRoad = 0;
        int coinsOnRoad = 0;

        foreach (float z in pointsZ)
        {
            int obstaclesInRow = 0;

            for (int i = 0; i < xCells; i++)
            {
                float x = minX + i * stepX;
                float random = Random.value;

                if (
                    random < 0.15f &&
                    obstaclesOnRoad < _obstaclesPerRoad &&
                    obstaclesInRow < 2
                )
                {
                    SpawnObstacles(road, x, z);

                    obstaclesOnRoad++;
                    obstaclesInRow++;
                }
                else if (
                    random < 0.35f &&
                    coinsOnRoad < _coinsPerRoad
                )
                {
                    SpawnCoins(road, x, z);

                    coinsOnRoad++;
                }
            }
        }
    }
}

using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    [SerializeField] private Transform[] waypoints;

    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float spawnRate = 2f;

    private float timer;

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        if(timer >= spawnRate)
        {
            timer = 0f;
        }
    }

    private void SpawnerEnemy()
    {
        GameObject gameObject = Instantiate(enemy, spawnPoint.position, Quaternion.identity);

        EnemyMovement enemMovement = gameObject.GetComponent<EnemyMovement>();

        if (enemMovement != null)
        {
            enemMovement.Initilize(waypoints);
        }
    }
}

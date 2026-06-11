using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    [SerializeField] private float range = 3f;

    [SerializeField] private float fireRate = 1f;

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Transform firePoint;

    private float fireCountdown;

    private void FixedUpdate()
    {
        fireCountdown -= Time.deltaTime;

        GameObject target = GetComponent<GameObject>();

        if (target != null && fireCountdown <= 0f)
        {
            Shoot(target);
            fireCountdown = 1f / fireRate;
        }
    }

    private GameObject FindEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject nearestEnemy = null;

        float shortestDistance = Mathf.Infinity;
        

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance && distance <= range)
            {
                shortestDistance = distance;

                nearestEnemy = enemy;
            }

        }
        
        return nearestEnemy;
    }

    private void Shoot(GameObject target)
    {
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(target.transform);
        }
    }
}

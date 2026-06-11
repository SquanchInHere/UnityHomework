using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    [SerializeField] private int damage = 10;

    private Transform target;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);

            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) <= 0.1f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        EnemyHealth enemyHealth =
            target.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}

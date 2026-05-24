using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class AiBot : MonoBehaviour
{
    public Transform pointA, pointB, player;

    public float detectionRadius = 5f,
                   patrolSpeed = 2f,
                   chaseSpeed = 4f,
                   pointReachDistance = .2f;

    Rigidbody2D rb;
    Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = pointB;
    }

    void FixedUpdate()
    {
        if (player && Vector2.Distance(transform.position, player.position) <= detectionRadius)
            Move(player.position, chaseSpeed);
        else if (pointA && pointB)
        {
            Move(target.position, patrolSpeed);

            if (Vector2.Distance(transform.position, target.position) <= pointReachDistance)
                target = target == pointA ? pointB : pointA;
        }
    }

    void Move(Vector2 pos, float speed) => rb.MovePosition(Vector2.MoveTowards(rb.position, pos, speed * Time.fixedDeltaTime));

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (pointA && pointB)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
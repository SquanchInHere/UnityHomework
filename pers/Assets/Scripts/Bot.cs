using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private Transform _pointA;

    [SerializeField] private Transform _pointB;


    [SerializeField] private float _speed = 3f;


    private Rigidbody2D rb;

    private Transform target;


    private void Start()

    {

        rb = GetComponent<Rigidbody2D>();

        target = _pointB;

    }


    private void FixedUpdate()

    {

        Vector2 dir = ((Vector2)target.position - rb.position).normalized;

        rb.MovePosition(rb.position + dir * _speed * Time.fixedDeltaTime);


        if (Vector2.Distance(rb.position, target.position) < 0.1f)

        {

            target = target == _pointA ? _pointB : _pointA;

        }

    }
}

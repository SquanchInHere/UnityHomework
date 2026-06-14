using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private bool _destroyOnCollision = true;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_destroyOnCollision)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_destroyOnCollision)
        {
            return;
        }

        Destroy(gameObject);
    }
}

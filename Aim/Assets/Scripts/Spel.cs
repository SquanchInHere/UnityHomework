using UnityEngine;

public class Spel : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _fireBallPrefab;

    [Header("Spawn")]
    [SerializeField] private Transform _spawnPoint;


    [Header("Fire Ball")]
    [SerializeField] private float _fireBallSpeed = 20f;
    [SerializeField] private KeyCode _fireBallKey = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(_fireBallKey))
        {
            CastFireBall();
        }
    }

    private void CastFireBall()
    {
        GameObject fireBall = Instantiate(_fireBallPrefab, _spawnPoint.position, _spawnPoint.rotation);

        if (fireBall.TryGetComponent(out Rigidbody rb))
            rb.linearVelocity = _spawnPoint.forward * _fireBallSpeed;
    }
}

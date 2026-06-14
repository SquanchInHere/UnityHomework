using UnityEngine;
using System.Collections;

public class Spel : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _fireBallPrefab;
    [SerializeField] private GameObject _spikePrefab;

    [Header("Spawn")]
    [SerializeField] private Transform _spawnPoint;

    [Header("Fire Ball")]
    [SerializeField] private float _fireBallSpeed = 20f;

    //[Header("Spike")]
    //[SerializeField] private float _spikeSpawnDistance = 3f;

    [Header("Spike Chain")]
    [SerializeField] private int _spikeCount = 8;
    [SerializeField] private float _spikeSpacing = 1.2f;
    [SerializeField] private float _spikeSpawnDelay = 0.08f;
    [SerializeField] private float _spikeYOffset = 0f;

    [Header("Hook")]
    [SerializeField] private float _hookDistance = 20f;
    [SerializeField] private float _hookPullSpeed = 20f;
    [SerializeField] private string _hookTag = "isHook";
    [SerializeField] private LayerMask _hookLayerMask = ~0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            CastFireBall();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CastSpike();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            CastHook();
        }
    }

    private void CastFireBall()
    {
        GameObject fireball = Instantiate(
            _fireBallPrefab,
            _spawnPoint.position,
            _spawnPoint.rotation
        );

        if (fireball.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = _spawnPoint.forward * _fireBallSpeed;
        }
    }

    private void CastSpike()
    {
        StartCoroutine(SpawnSpikeChain());
    }

    private void CastHook()
    {
        bool hasHit = Physics.Raycast(
            _spawnPoint.position,
            _spawnPoint.forward,
            out RaycastHit hit,
            _hookDistance,
            _hookLayerMask
        );

        if (!hasHit)
        {
            return;
        }

        if (!hit.collider.CompareTag(_hookTag))
        {
            return;
        }

        if (!hit.collider.TryGetComponent(out Rigidbody rb))
        {
            Debug.LogWarning("Hook target does not have Rigidbody.");
            return;
        }

        Vector3 pullDirection = (transform.position - hit.collider.transform.position).normalized;

        rb.linearVelocity = pullDirection * _hookPullSpeed;
    }

    private IEnumerator SpawnSpikeChain()
    {
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.forward;
        direction.y = 0f;
        direction.Normalize();

        for (int i = 1; i <= _spikeCount; i++)
        {
            Vector3 position = startPosition + direction * (_spikeSpacing * i);
            position.y += _spikeYOffset;

            if (Physics.Raycast(position, Vector3.down, out RaycastHit groundHit, 20f))
            {
                position = groundHit.point;
                position.y += _spikeYOffset;
            }

            Quaternion rotation = Quaternion.LookRotation(direction);

            Instantiate(_spikePrefab, position, rotation);

            yield return new WaitForSeconds(_spikeSpawnDelay);
        }
    }
}

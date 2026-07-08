using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class DestructibleObject : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _damageThreshold = 150f;
    [SerializeField] private int _chunkCount = 8;
    [SerializeField] private int _fractureSeed = 42;

    [Header("Chunk Physics")]
    [SerializeField] private float _chunkMass = 1f;
    [SerializeField] private float _explosionForce = 500f;
    [SerializeField] private float _explosionRadius = 3f;
    [SerializeField] private PhysicsMaterial _chunkPhysicMaterial;

    [Header("Cleanup")]
    [SerializeField] private float _debrisLifetime = 5f;
    [SerializeField] private bool _destroyAfterCleanup = true;

    [Header("Effects")]
    [SerializeField] private GameObject _destroyEffectPrefab;
    [SerializeField] private AudioClip _destroySound;

    [Header("Events")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDestroyed;
    public UnityEvent<Vector3> OnChunkBreak;

    [Header("Progressive Damage Decals")]
    [SerializeField] private GameObject _crackDecalPrefab;
    [SerializeField] private int _maxCrackDecals = 6;
    [SerializeField] private float _crackSurfaceOffset = 0.025f;
    [SerializeField] private float _minCrackScale = 0.25f;
    [SerializeField] private float _maxCrackScale = 0.55f;

    [SerializeField] private bool _spawnCracksOnlyAfterHealthLoss = true;
    [SerializeField] private float _firstCrackHealthPercent = 0.75f;
    [SerializeField] private float _secondCrackHealthPercent = 0.5f;
    [SerializeField] private float _thirdCrackHealthPercent = 0.25f;

    private readonly List<GameObject> _crackDecals = new List<GameObject>();
    private int _damageStage = 0;

    private float _currentHealth;
    private bool _isDestroyed = false;
    private List<GameObject> _chunks = new List<GameObject>();
    private Transform _chunksParent;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDestroyed => _isDestroyed;
    public float HealthPercent => _currentHealth / _maxHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        GameObject parent = new GameObject($"{gameObject.name}_Chunks");
        parent.transform.SetParent(transform.parent);
        parent.transform.position = transform.position;
        parent.transform.rotation = transform.rotation;
        _chunksParent = parent.transform;
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitForce)
    {
        if (_isDestroyed) return;

        _currentHealth -= amount;
        OnDamageTaken.Invoke();

        UpdateDamageVisual(hitPoint);

        if (_currentHealth <= 0 || amount >= _damageThreshold)
        {
            Fracture(hitPoint, hitForce);
        }
    }

    public void Fracture(Vector3 originPoint, Vector3 force)
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        ClearCrackDecals();

        if (_destroyEffectPrefab != null)
            Instantiate(_destroyEffectPrefab, transform.position, Quaternion.identity);

        if (_destroySound != null && TryGetComponent(out AudioSource src))
            src.PlayOneShot(_destroySound);

        GenerateChunks();

        for (int i = 0; i < _chunks.Count; i++)
        {
            if (_chunks[i] == null) continue;

            if (!_chunks[i].TryGetComponent(out Rigidbody rb)) continue;

            rb.isKinematic = false;
            rb.useGravity = true;
            _chunks[i].transform.SetParent(null);

            Vector3 dir = (_chunks[i].transform.position - originPoint).normalized;
            if (dir.magnitude < 0.01f)
                dir = Random.onUnitSphere;

            dir += Random.onUnitSphere * 0.6f;
            dir.Normalize();

            float distanceFactor = Mathf.Clamp01(
                1f - Vector3.Distance(_chunks[i].transform.position, originPoint) / _explosionRadius);
            rb.AddForce(dir * _explosionForce * (0.5f + distanceFactor * 0.5f), ForceMode.Impulse);
            rb.AddTorque(Random.onUnitSphere * _explosionForce * 0.2f, ForceMode.Impulse);
            rb.AddForce(force * 0.5f, ForceMode.Impulse);

            OnChunkBreak.Invoke(_chunks[i].transform.position);
        }

        DisableOriginal();
        OnDestroyed.Invoke();
    }

    public void Fracture()
    {
        Fracture(transform.position, Vector3.zero);
    }

    private void GenerateChunks()
    {
        if (!TryGetComponent(out MeshFilter mf) || mf.sharedMesh == null)
        {
            Debug.LogError("[DestructibleObject] Missing MeshFilter or mesh", gameObject);
            return;
        }

        MeshRenderer mr = GetComponent<MeshRenderer>();
        Mesh originalMesh = mf.sharedMesh;
        Material material = mr != null ? mr.sharedMaterial : null;

        List<Mesh> chunkMeshes = MeshFracturer.FractureMesh(originalMesh, _chunkCount, _fractureSeed);
        if (chunkMeshes.Count == 0)
        {
            Debug.LogWarning("[DestructibleObject] Fracture produced no chunks", gameObject);
            return;
        }

        Vector3 originalCenter = originalMesh.bounds.center;

        for (int i = 0; i < chunkMeshes.Count; i++)
        {
            GameObject chunk = new GameObject($"Chunk_{i:D3}");
            chunk.transform.SetParent(_chunksParent);
            chunk.transform.rotation = transform.rotation;
            chunk.transform.localScale = transform.lossyScale;

            Vector3 chunkCenter = chunkMeshes[i].bounds.center;
            Vector3 offset = chunkCenter - originalCenter;
            Vector3 worldOffset = transform.rotation * Vector3.Scale(offset, transform.lossyScale);
            chunk.transform.position = transform.position + worldOffset;

            MeshFilter cf = chunk.AddComponent<MeshFilter>();
            cf.sharedMesh = chunkMeshes[i];

            MeshRenderer cr = chunk.AddComponent<MeshRenderer>();
            cr.sharedMaterial = material;

            MeshCollider cc = chunk.AddComponent<MeshCollider>();
            cc.sharedMesh = chunkMeshes[i];
            cc.convex = true;
            if (_chunkPhysicMaterial != null)
                cc.material = _chunkPhysicMaterial;

            Rigidbody rb = chunk.AddComponent<Rigidbody>();
            rb.mass = _chunkMass;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            DebrisCleanup cleanup = chunk.AddComponent<DebrisCleanup>();
            cleanup.Setup(_debrisLifetime, _destroyAfterCleanup);

            _chunks.Add(chunk);
        }
    }

    private void UpdateDamageVisual(Vector3 hitPoint)
    {
        if (_crackDecalPrefab == null)
            return;

        float hp = HealthPercent;

        int newStage = 0;

        if (hp <= _thirdCrackHealthPercent)
            newStage = 3;
        else if (hp <= _secondCrackHealthPercent)
            newStage = 2;
        else if (hp <= _firstCrackHealthPercent)
            newStage = 1;

        if (_spawnCracksOnlyAfterHealthLoss)
        {
            if (newStage <= _damageStage)
                return;

            _damageStage = newStage;
        }

        SpawnCrackDecal(hitPoint);
    }


    private void SpawnCrackDecal(Vector3 hitPoint)
    {
        if (_crackDecalPrefab == null)
        {
            Debug.LogError("[DestructibleObject] Crack Decal Prefab is NULL.", gameObject);
            return;
        }

        if (_crackDecals.Count >= _maxCrackDecals)
            return;

        Vector3 hitNormal = GetSurfaceNormal(hitPoint);
        Vector3 surfacePoint = GetSurfacePoint(hitNormal);

        Vector3 position = surfacePoint + hitNormal * _crackSurfaceOffset;

        Quaternion rotation = Quaternion.LookRotation(-hitNormal, Vector3.up);

        GameObject decal = Instantiate(_crackDecalPrefab, position, rotation);

        float scale = Random.Range(_minCrackScale, _maxCrackScale);
        decal.transform.localScale = Vector3.one * scale;

        decal.transform.Rotate(0f, 0f, Random.Range(0f, 360f), Space.Self);

        decal.transform.SetParent(transform, true);

        _crackDecals.Add(decal);

        Debug.Log($"[DestructibleObject] Decal position: {position}", decal);
    }

    private Vector3 GetSurfacePoint(Vector3 normal)
    {
        if (TryGetComponent(out Collider col))
        {
            Bounds bounds = col.bounds;

            Vector3 center = bounds.center;

            float radius = Mathf.Min(bounds.extents.x, bounds.extents.y, bounds.extents.z);

            return center + normal.normalized * radius;
        }

        return transform.position + normal.normalized * 0.5f;
    }

    private Vector3 GetSurfaceNormal(Vector3 hitPoint)
    {
        Vector3 center = transform.position;

        if (TryGetComponent(out Collider col))
            center = col.bounds.center;

        Vector3 normal = hitPoint - center;

        if (normal.sqrMagnitude < 0.0001f)
            normal = transform.forward;

        return normal.normalized;
    }

    private void ClearCrackDecals()
    {
        for (int i = 0; i < _crackDecals.Count; i++)
        {
            if (_crackDecals[i] != null)
                Destroy(_crackDecals[i]);
        }

        _crackDecals.Clear();
    }



    private void DisableOriginal()
    {
        if (TryGetComponent(out MeshRenderer mr))
            mr.enabled = false;

        if (TryGetComponent(out MeshCollider mc))
            mc.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, _explosionRadius);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Vector3 testPoint = transform.position + transform.forward * 0.6f;
            SpawnCrackDecal(testPoint);
        }
    }
}

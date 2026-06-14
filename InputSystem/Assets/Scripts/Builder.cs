using UnityEngine;

public class Builder : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _firstPrefab;
    [SerializeField] private GameObject _secondPrefab;

    [Header("Build Settings")]
    [SerializeField] private float _buildDistance = 3f;
    [SerializeField] private float _buildYOffset = 0.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Build(_firstPrefab);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Build(_secondPrefab);
        }
    }

    private void Build(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Build prefab is not assigned.");
            return;
        }

        Vector3 buildPosition = transform.position + transform.forward * _buildDistance;
        buildPosition.y += _buildYOffset;

        Instantiate(prefab, buildPosition, Quaternion.identity);
    }
}

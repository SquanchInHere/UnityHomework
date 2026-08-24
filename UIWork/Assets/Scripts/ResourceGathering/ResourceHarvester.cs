using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceHarvester : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera _playerCamera;

    [Min(0.1f)]
    [SerializeField] private float _harvestDistance = 3f;

    [SerializeField] private LayerMask _resourceLayerMask;

    [Header("Harvest")]
    [Min(0f)]
    [SerializeField] private float _harvestCooldown = 0.5f;

    [Min(1)]
    [SerializeField] private int _harvestPower = 1;

    [Header("Receiver")]
    [SerializeField] private MonoBehaviour _receiverBehaviour;

    private IResourceReceiver _receiver;
    private float _nextHarvestTime;

    private void Awake()
    {
        if (_playerCamera == null)
            _playerCamera = Camera.main;

        ResolveReceiver();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryHarvest();
        }
    }

    private void ResolveReceiver()
    {
        if (_receiverBehaviour != null)
        {
            _receiver = _receiverBehaviour as IResourceReceiver;

            if (_receiver != null)
                return;
        }

        MonoBehaviour[] localComponents = GetComponentsInParent<MonoBehaviour>(true);

        foreach (MonoBehaviour component in localComponents)
        {
            if (component is IResourceReceiver receiver)
            {
                _receiverBehaviour = component;
                _receiver = receiver;
                return;
            }
        }

        MonoBehaviour[] sceneComponents = FindObjectsByType<MonoBehaviour>(
            FindObjectsSortMode.None);

        foreach (MonoBehaviour component in sceneComponents)
        {
            if (component is IResourceReceiver receiver)
            {
                _receiverBehaviour = component;
                _receiver = receiver;
                return;
            }
        }
    }

    public bool TryHarvest()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return false;

        if (Time.time < _nextHarvestTime)
            return false;

        if (_playerCamera == null)
            _playerCamera = Camera.main;

        if (_receiver == null)
            ResolveReceiver();

        if (_playerCamera == null || _receiver == null)
            return false;

        Ray ray = new(
            _playerCamera.transform.position,
            _playerCamera.transform.forward);

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                _harvestDistance,
                _resourceLayerMask,
                QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        ResourceNode resourceNode = hit.collider.GetComponentInParent<ResourceNode>();

        if (resourceNode == null)
            return false;

        if (!resourceNode.TryHarvest(
                _harvestPower,
                _receiver,
                out _))
        {
            return false;
        }

        _nextHarvestTime = Time.time + _harvestCooldown;
        return true;
    }
}

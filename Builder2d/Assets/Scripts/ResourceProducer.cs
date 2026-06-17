using Unity.VisualScripting;
using UnityEngine;

public class ResourceProducer : MonoBehaviour
{
    [Tooltip("Який ресурс виробляє будівля")]
    [SerializeField] private ResourceType resourceType = ResourceType.Gold;

    [Tooltip("Скільки додавати за один тік")]
    [SerializeField] private int amountPerTick = 1;

    [Tooltip("Інтервал між тіками в секундах")]
    [SerializeField] private float secondsPerTick = 2f;

    [Tooltip("How many create ")]
    [SerializeField] private int _limitUnit = 3;
    private int unitCount = 0;

    private float _timer;

    private void Update()
    {
        if (ResourceManager.Instance == null) return;

        _timer += Time.deltaTime;
        while (_timer >= secondsPerTick)
        {
            _timer -= secondsPerTick;

            if (resourceType == ResourceType.Unit)
            {
                if (unitCount >= _limitUnit)
                    return;

                ResourceManager.Instance.Add(resourceType, amountPerTick);
                unitCount += amountPerTick;

                Debug.Log($"{name}: Produced {amountPerTick} unit. Current units: {unitCount}/{_limitUnit}");
            }
            else
            {
                ResourceManager.Instance.Add(resourceType, amountPerTick);

                Debug.Log($"{name}: Produced {amountPerTick} {resourceType}.");
            }
        }
    }
}

using UnityEngine;

public class DestroySpel : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }
}

using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [SerializeField] private float speed = 100f;

    private void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}

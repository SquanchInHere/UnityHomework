using UnityEngine;

public class FireBall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if (_ownerCollider != null && other == _ownerCollider)
        //{
        //    return;
        //}

        if (other.CompareTag("Player"))
        {
            return;
        }

        Destroy(gameObject);
    }
}

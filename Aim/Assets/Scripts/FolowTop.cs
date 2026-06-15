using UnityEngine;

public class FolowTop : MonoBehaviour
{
    [SerializeField] private Transform _player;


    [SerializeField] private float _height = 40f;


    [SerializeField] private bool _folowRotation = false;

    private void LateUpdate()
    {
        if (_player == null)
        {
            Debug.LogWarning("Player not found! ");
            return;
        }

        transform.position = _player.position + Vector3.up * _height;

        if (_folowRotation)
        {
            transform.rotation = Quaternion.Euler(90f, _player.eulerAngles.y, 0f);
        }

    }
}

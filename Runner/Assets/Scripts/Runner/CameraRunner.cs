using UnityEngine;

public class CameraRunner : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private Vector3 _offset;


    public void Start()
    {
        _offset = transform.position - _player.position;
    }

    public void LateUpdate()
    {
        transform.position = _player.position + _offset;
    }
}

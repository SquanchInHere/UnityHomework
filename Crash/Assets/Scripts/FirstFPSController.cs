using UnityEngine;

public class FirstFPSController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _gravity = -20f;

    [Header("Mouse Look")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private float _minLookAngle = -80f;
    [SerializeField] private float _maxLookAngle = 80f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _cameraPitch;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        if (_cameraTransform == null)
            _cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Move();
        Look();
    }

    private void Move()
    {
        bool isGrounded = _controller.isGrounded;

        if (isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection.Normalize();

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _moveSpeed;

        _controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        _velocity.y += _gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, _minLookAngle, _maxLookAngle);

        _cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }
}

using UnityEngine;

public class RunnerController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 50;
    [SerializeField] private float sideSpeed = 5;
    [SerializeField] private float jumpSpeed = 5;

    private Rigidbody rb;
    private bool isGrounded;
    private float moveX;
    private bool jumpPressed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        float moveX = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * moveX * sideSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpPressed = true;
        }

    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(
            moveX * sideSpeed,
            rb.linearVelocity.y,
            forwardSpeed
        );

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpSpeed,
                rb.linearVelocity.z
            );

            isGrounded = false;
            jumpPressed = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}

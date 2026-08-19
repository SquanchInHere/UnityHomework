using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerInvulnerability))]
[RequireComponent(typeof(PlayerMovementTracker))]
public class PlayerRespawn : MonoBehaviour
{

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Respawn Protection")]
    [SerializeField] private float invulnerabilityAfterRespawn = 2f;

    private PlayerHealth playerHealth;
    private PlayerInvulnerability playerInvulnerability;
    private PlayerMovementTracker playerMovementTracker;

    private void Awake()
    {
        playerHealth =
            GetComponent<PlayerHealth>();

        playerInvulnerability =
            GetComponent<PlayerInvulnerability>();

        playerMovementTracker =
            GetComponent<PlayerMovementTracker>();
    }

    private void OnEnable()
    {
        playerHealth.Died += Respawn;
    }

    private void OnDisable()
    {
        playerHealth.Died -= Respawn;
    }

    private void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning(
                "Respawn Point is not assigned!"
            );

            return;
        }

        CharacterController characterController =
            GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        playerHealth.RestoreFullHealth();

        playerMovementTracker.ResetMovementTracking();

        playerInvulnerability.StartInvulnerability(
            invulnerabilityAfterRespawn
        );

        Debug.Log(
            "Player respawned at: " +
            respawnPoint.position
        );
    }
}

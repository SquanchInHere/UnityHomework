using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMovementTracker))]
public class PlayerRegeneration : MonoBehaviour
{
    [Header("Regeneration")]
    [SerializeField] private float timeToStartRegeneration = 5f;
    [SerializeField] private float regenerationPerSecond = 10f;

    private PlayerHealth playerHealth;
    private PlayerMovementTracker playerMovementTracker;

    private void Awake()
    {
        playerHealth =
            GetComponent<PlayerHealth>();

        playerMovementTracker =
            GetComponent<PlayerMovementTracker>();
    }

    private void Update()
    {
        HandleRegeneration();
    }

    private void HandleRegeneration()
    {
        if (playerHealth.GetCurrentHealth() >=
            playerHealth.GetMaxHealth())
        {
            return;
        }

        if (playerMovementTracker.GetStationaryTimer() <
            timeToStartRegeneration)
        {
            return;
        }

        playerHealth.Heal(
            regenerationPerSecond * Time.deltaTime
        );
    }
}

using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInvulnerability))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private PlayerInvulnerability playerInvulnerability;

    public event Action Died;

    private void Awake()
    {
        playerInvulnerability =
            GetComponent<PlayerInvulnerability>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (playerInvulnerability.GetInvulnerabilityTimer() > 0f)
            return;

        if (currentHealth <= 0f)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float health)
    {
        currentHealth += health;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );
    }

    private void Die()
    {
        Debug.Log("Player died!");

        Died?.Invoke();
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
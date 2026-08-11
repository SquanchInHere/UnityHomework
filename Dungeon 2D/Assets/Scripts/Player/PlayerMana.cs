using System;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerConfigSO _config;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnManaRegen;

    public event Action<int, int> OnManaChanged;

    private int _currentMana;

    public int CurrentMana => _currentMana;
    public int MaxMana => _config != null ? _config.MaxMana : 0;

    private void Start()
    {
        ResetMana();
    }

    public void ResetMana()
    {
        _currentMana = MaxMana;

        OnManaChanged?.Invoke(_currentMana, MaxMana);
    }

    public void RegenMana(int amount)
    {
        if (amount <= 0) return;

        _currentMana += amount;

        OnManaChanged?.Invoke(_currentMana, MaxMana);
        OnManaRegen?.Invoke();
    }
}

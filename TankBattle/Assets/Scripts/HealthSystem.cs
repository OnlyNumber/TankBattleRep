using System;
using UnityEngine;

public class HealthSystem
{
    public int MaxHealth
    {
        get;
        private set;
    }
    private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            
            if (_currentHealth > MaxHealth)
                _currentHealth = MaxHealth;

            if (_currentHealth < 0)
                _currentHealth = 0;

            OnHealthChanged?.Invoke();
        }
    }
    public event Action OnHealthChanged;

    public HealthSystem(int MaxHealth)
    {
        this.MaxHealth = MaxHealth;
        _currentHealth = MaxHealth;

    }
}

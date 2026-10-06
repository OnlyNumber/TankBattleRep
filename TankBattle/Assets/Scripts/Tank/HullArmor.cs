using System;
using System.Collections.Generic;
using UnityEngine;

public class HullArmor : MonoBehaviour, ITankPart
{

    public int MaxHealth;
    private HealthSystem _health;
    
    public HullStats HullStats;
    
    public List<ArmorStat> armorStats = new();
    [SerializeField] private PartContainer _partContainer;

    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;

    void Start()
    {
        _health = new HealthSystem(MaxHealth);
    }

    public int GetCurrentHealth()
    {
        return _health.CurrentHealth;
    }
    
    public void Changehealth(int changedHealth)
    {
        _health.CurrentHealth += changedHealth;
    }

    public void AddOnTakeDamage(Action action)
    {
        _health.OnHealthChanged += action;
    }

    public ArmorStat GetArmorStat(Collider collider)
    {
        foreach (var armorStat in armorStats)
        {
            if (armorStat.Collider == collider)
                return armorStat;
        }

        return new ArmorStat();
    }
}

[System.Serializable]
public struct ArmorStat
{
    public Collider Collider;
    public float ArmorThickness;
    public ArmorSide Side;
}


public enum ArmorSide
{
    Front,
    Side,
    rear
}
using System.Collections.Generic;
using UnityEngine;

public class TankStat
{
    //private float _baseValue;
    private List<StatsModifier> _statsModifiers = new();
//    private bool _isDirty = true;
//    private float _cachedValue;
/*
    public float StatValue
    {
        get
        {
            if(_isDirty)
            {
                _isDirty = true;
                _cachedValue = CalculatePercentModifiers();
            }

            return _cachedValue;
        }
    }*/

    public void AddModifier(StatsModifier modifier)
    {
        _statsModifiers.Add(modifier);
    }

    
    public void RemoveModifier(StatsModifier modifier)
    {
        _statsModifiers.Remove(modifier);
    }

    private float CalculatePercentModifiers()
    {
        float totalPercent = 0f;

        // Додаємо всі відсотки один до одного (наприклад: +15% + 5% = +20%)
        foreach (var mod in _statsModifiers)
        {
            totalPercent += mod.Value;
        }

        // Кінцева формула: База * (1 + загальний відсоток)
        //float finalValue = _baseValue * (1f + totalPercent);
        
        return 1 +(float)System.Math.Round(totalPercent, 4);
    }

    public float CalculateFinalValue(float baseValue)
    {
        return CalculatePercentModifiers() * baseValue;
    }

}

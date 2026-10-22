using System.Collections.Generic;
using UnityEngine;

public class TankStatsController
{
    public TankStat ReloadStat = new();
    public TankStat ArmorStat = new();
    public TankStat SpeedStat = new();
    public TankStat AccelerationStat = new();
    public TankStat DamageStat = new();

    public List<TemporaryModifier> TempModifiers = new();

    public bool IsContainModifier(TemporaryModifierData checkModifier)
    {
        foreach (var item in TempModifiers)
        {
            if (checkModifier == item.ModifierData)
                return true;
        }

        return false;
    }

    public TankStat GetStat(ModifiedStat modifiedStat)
    {
        switch (modifiedStat)
        {
            case ModifiedStat.Reload:
                return ReloadStat;
            case ModifiedStat.Armor:
                return ArmorStat;
            case ModifiedStat.Speed:
                return SpeedStat;
            case ModifiedStat.Acceleration:
                return AccelerationStat;
            case ModifiedStat.Damage:
                return DamageStat;
        }

        return null;

    }



    public enum ModifiedStat
    {
        Reload,
        Armor,
        Speed,
        Acceleration,
        Damage
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class HullArmor : MonoBehaviour, ITankPart
{

    public int MaxHealth;
    public HullStats HullStats;

    public List<ArmorStat> armorStats = new();
    [SerializeField] private PartContainer _partContainer;

    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;

    public ArmorStat GetArmorStat(Collider collider)
    {
        foreach (var armorStat in armorStats)
        {
            if (armorStat.Collider == collider)
                return armorStat;
        }

        return new ArmorStat();
    }

    public List<Description> GetDescriptions()
    {
        List<Description> descriptions = new()
        {
            new Description("Health", MaxHealth.ToString()),
            new Description("Acceleration", HullStats.Acceleration.ToString()),
            new Description("Decceleration", HullStats.Decceleration.ToString()),
            new Description("MaxSpeed", HullStats.MaxSpeed.ToString()),
            new Description("RotationSpeed", HullStats.RotationSpeed.ToString()),

            GetArmorDescription(ArmorSide.Front),
            GetArmorDescription(ArmorSide.Side),
            GetArmorDescription(ArmorSide.Rear),
/*
            new Description("Front armor", HullStats.Decceleration.ToString()),
            new Description("Side armor", HullStats.MaxSpeed.ToString()),
            new Description("Rear armor", HullStats.RotationSpeed.ToString()),
*/
        };

        return descriptions;
    }

    private Description GetArmorDescription(ArmorSide armorSide)
    {
        foreach (var item in armorStats)
        {
            if (item.Side == armorSide)
                return new Description(item.Side.ToString(), item.ArmorThickness.ToString());
        }

        return null;
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
    Rear
}
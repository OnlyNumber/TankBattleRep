using System;
using System.Collections.Generic;
using UnityEngine;

public class HullArmor : MonoBehaviour, ITankPart
{

    public int MaxHealth;
    public HullStats HullStats = new();
    
    public List<ArmorStat> armorStats = new();

    [field: SerializeField]
    public Transform AimPosition
    {
        get;
        private set;
    }


    #region  ITankPart implementation
    [SerializeField] private PartContainer _partContainer;
    [SerializeField] private Collider _partCollider;

    public Collider PartCollider => _partCollider;
    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;

    [SerializeField] private bool _isBuiltIn;
    public bool IsBuiltIn => _isBuiltIn;
    #endregion
    
    [SerializeField] private List<StatsModifier> _statsModifiers;

    private TankStatsController _tankStatsController;


    public ArmorStat GetArmorStat(Collider collider)
    {
        ArmorStat calculatedArmourStat = new();

        foreach (var armorStat in armorStats)
        {
            if (armorStat.Collider == collider)
                calculatedArmourStat = armorStat;
        }

        calculatedArmourStat.ArmorThickness = _tankStatsController.ArmorStat.CalculateFinalValue(calculatedArmourStat.ArmorThickness);

        return calculatedArmourStat;
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
        };

        return descriptions;
    }

    public List<StatsModifier> GetStatsModifiers()
    {
        return _statsModifiers;
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

    public void SetTankStatController(TankStatsController tankStatsController)
    {
        _tankStatsController = tankStatsController;
        HullStats.SetTankStatController(tankStatsController);
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
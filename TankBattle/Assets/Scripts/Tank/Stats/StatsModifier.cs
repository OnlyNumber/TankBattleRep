using UnityEngine;
using static TankStatsController;

[System.Serializable]
public class StatsModifier
{
    public float Value;
    
    public ModifiedStat ModifierStat;

    public StatsModifier(float value, ModifiedStat modifiedStat)
    {
        Value = value;
        ModifierStat = modifiedStat; 
    }
}


public enum StatsMofierType
{
    Flat,
    Percent,
    Multiplier
}
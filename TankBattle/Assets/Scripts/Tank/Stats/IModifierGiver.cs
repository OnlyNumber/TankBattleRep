using System.Collections.Generic;
using UnityEngine;

public interface IModifierGiver
{
    public List<StatsModifier> GetStatsModifiers();
}

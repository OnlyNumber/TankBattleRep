using System.Collections.Generic;
using UnityEngine;

public interface ITankPart : IModifierGiver
{
    public PartContainer PartContainer
    {
        get;
    }
    public GameObject GameObject
    {
        get;
    }

    public List<Description> GetDescriptions();

    public void SetTankStatController(TankStatsController tankStatsController);

}

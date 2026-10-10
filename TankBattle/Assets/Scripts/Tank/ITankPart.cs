using System.Collections.Generic;
using UnityEngine;

public interface ITankPart
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
}

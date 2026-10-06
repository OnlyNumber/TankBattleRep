using System.Collections.Generic;
using UnityEngine;

public class PartContainer : MonoBehaviour
{
    public List<PartContainerSlot> TurretPlace = new();
    public List<PartContainerSlot> GunPlace = new();

}

[System.Serializable]
public class PartContainerSlot
{
    public Transform Parent;
    public ITankPart Part;
}


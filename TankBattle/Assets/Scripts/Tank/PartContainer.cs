using System.Collections.Generic;
using UnityEngine;

public class PartContainer : MonoBehaviour
{
    public List<PartContainerSlot> TurretPlace = new();
    public List<PartContainerSlot> GunPlace = new();

    [ContextMenu("FindMyParts")]
    public void FindMyParts()
    {
        foreach (var slot in TurretPlace)
            slot.Part = slot.Parent.GetComponentInChildren<ITankPart>();
        
        foreach (var slot in GunPlace)
            slot.Part = slot.Parent.GetComponentInChildren<ITankPart>();

    }


    private void SetParts(List<PartContainerSlot> slots)
    {
        foreach (var slot in slots)
        {
            slot.Part = slot.Parent.GetComponentInChildren<ITankPart>();
            if(slot.Part != null)
            {
                Debug.Log(slot.Part.GameObject.name);
            }

        }
    }

}

[System.Serializable]
public class PartContainerSlot
{
    public Transform Parent;
    public ITankPart Part;
}


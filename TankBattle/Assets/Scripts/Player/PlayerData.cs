using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int MoneyCount;
    public PartSave CurrentTank; 

    public List<PartSave> InventoryParts = new(); 
}

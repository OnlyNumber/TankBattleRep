using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartSave
{
    public PartType PartType;

    //For loading from resources
    public string PartName;

    public List<PartSaveSlotInfo> PartSlotsTurret = new();
    public List<PartSaveSlotInfo> PartSlotsGuns = new();

    public void AddToList(List<PartSaveSlotInfo> list, int partIndex, PartSave partSave)
    {
        var hz = new PartSaveSlotInfo();
        hz.partIndex = partIndex;
        hz.partSave = partSave;


        list.Add(hz);
    }

}

[System.Serializable]
public struct PartSaveSlotInfo
{
    public int partIndex;
    public PartSave partSave;
}
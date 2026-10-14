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

    public void RemovePartFromTurrets(PartSave partSave)
    {
        foreach (var item in PartSlotsTurret)
        {
            if(item.partSave == partSave)
            {
                PartSlotsTurret.Remove(item);
                break;
            } 
        }
    }

    public void RemovePartFromGuns(PartSave partSave)
    {
        foreach (var item in PartSlotsGuns)
        {
            if(item.partSave == partSave)
            {
                PartSlotsGuns.Remove(item);
                break;
            } 
        }
    }

    public void RemoveAll()
    {
        PartSlotsTurret.Clear();
        PartSlotsGuns.Clear();

    }

}

[System.Serializable]
public struct PartSaveSlotInfo
{
    public int partIndex;
    public PartSave partSave;
}
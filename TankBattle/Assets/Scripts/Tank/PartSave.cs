using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartSave
{
    public PartType PartType;

    //For loading from resources
    public string PartName;

    public List<IDK> PartSlotsTurret = new();
    public List<IDK> PartSlotsGuns = new();

    public void AddToList(List<IDK> list, int partIndex, PartSave partSave)
    {
        var hz = new IDK();
        hz.partIndex = partIndex;
        hz.partSave = partSave;


        list.Add(hz);
    }

}

[System.Serializable]
public struct IDK
{
    public int partIndex;
    public PartSave partSave;
}
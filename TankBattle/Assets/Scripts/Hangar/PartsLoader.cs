using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PartsLoader
{
    private const string Hulls_Path = "Parts/Hulls";
    private const string Turrets_Path = "Parts/Turrets";
    private const string Guns_Path = "Parts/Guns";



    private static T[] LoadParts<T>(string path)
    {
        var list = Resources.LoadAll(path, typeof(T)).Cast<T>().ToArray();
        return list;
    }

    public static T[] LoadParts<T>()
    {
        switch (typeof(T))
        {
            case System.Type t when t == typeof(HullArmor):
                return LoadParts<T>(Hulls_Path);

            case System.Type t when t == typeof(Gun):
                return LoadParts<T>(Guns_Path);

            case System.Type t when t == typeof(TurretMovement):
                return LoadParts<T>(Turrets_Path);
        }

        return null;
    }

    public static GameObject LoadPart(string name, PartType partType)
    {
        

        switch (partType)
        {
            case PartType.Hull:
                return Resources.Load<HullArmor>(Hulls_Path + "/" + name).gameObject;
            case PartType.Turret:
                return Resources.Load<TurretMovement>(Turrets_Path + "/" + name).gameObject;
            case PartType.Gun:
                return Resources.Load<Gun>(Guns_Path + "/" + name).gameObject;
        }

        return null;
    }
}
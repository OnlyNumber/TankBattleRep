using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PartsLoader
{
    private const string Hulls_Path = "Parts/Hulls";
    private const string Turrets_Path = "Parts/Turrets";
    private const string Guns_Path = "Parts/Guns";

    private static Dictionary<string, GameObject> loadedParts = new();

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
        string path = "";

        switch (partType)
        {
            case PartType.Hull:

                path = Hulls_Path + "/" + name;

                if (!loadedParts.Keys.Contains(path))
                    loadedParts.Add(path, Resources.Load<HullArmor>(path).gameObject);

                return loadedParts[path];
            case PartType.Turret:

                path = Turrets_Path + "/" + name;

                if (!loadedParts.Keys.Contains(path))
                    loadedParts.Add(path, Resources.Load<TurretMovement>(path).gameObject);

                return loadedParts[path];
            case PartType.Gun:

                path = Guns_Path + "/" + name;

                if (!loadedParts.Keys.Contains(path))
                    loadedParts.Add(path, Resources.Load<Gun>(path).gameObject);

                return loadedParts[path];
        }


        return null;
    }
}
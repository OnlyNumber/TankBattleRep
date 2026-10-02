using System.Collections.Generic;
using UnityEngine;

public static class ControllerGameObjectPooler
{
    /*
    public static ControllerGameObjectPooler _instance;
    public static ControllerGameObjectPooler Instance
    {
        get
        {
            if (Instance == null)
            {
                _instance = new();
            }

            return

        }
    }
    */

    private static Dictionary<IPooled, GameObjectPool> _dictionary = new();

    public static IPooled GetFromPrefab(IPooled prefab)
    {
        if(!_dictionary.ContainsKey(prefab))
            _dictionary.Add(prefab, new GameObjectPool(prefab));

        var pool = _dictionary[prefab];

        return pool.ObjectPool.Get();
    }
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TemporaryModifierData", menuName = "Scriptable Objects/TemporaryModifierData")]
public class TemporaryModifierData : ScriptableObject
{
    [field: SerializeField]
    public Sprite Icon
    {
        get;
        private set;
    }

    [field: SerializeField]
    public float Duration
    {
        get;
        private set;
    }
    
    public List<StatsModifier> StatsModifier;

    public TemporaryModifier GetModifier()
    {
        return new TemporaryModifier(this);
    }

}

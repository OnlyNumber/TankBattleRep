using System;
using UnityEngine;

public enum PartType
{
    Hull,
    Turret,
    Gun
}

public static class PartTypeSwitcher
{
    public static Type GetTypeByEnum(PartType typeName)
    {
        switch (typeName)
        {
            case PartType.Hull:
                return typeof(HullArmor);
            case PartType.Turret:
                return typeof(TurretMovement);

            case PartType.Gun:
                return typeof(Gun);

        }
        return null;
    }

    public static PartType GetEnumByType(Type typeName)
    {
        switch (typeName)
        {
            case System.Type t when t == typeof(HullArmor):
                return PartType.Hull;
            case System.Type t when t == typeof(TurretMovement):
                return PartType.Turret;
            case System.Type t when t == typeof(Gun):
                return PartType.Gun;
        }

        return PartType.Hull;
    }
}


using System;
using UnityEngine;

[System.Serializable]
public class ShellInfo
{
    [SerializeField] private int _damage;
    public int Damage
    {
        get
        {
            if (_tankStatsController == null)
                return _damage;

            return (int)_tankStatsController.DamageStat.CalculateFinalValue((float)_damage);

        }
    }
    public float Penetration;
    public float Speed;
    private TankStatsController _tankStatsController;

    public void SetTankStatController(TankStatsController tankStatsController)
    {
        _tankStatsController = tankStatsController;
    }
}

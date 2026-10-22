using UnityEngine;

[System.Serializable]
public class HullStats
{
    [SerializeField] private float _acceleration;
    public float Acceleration
    {
        get => (int)_tankStatsController.AccelerationStat.CalculateFinalValue(_acceleration);
    }

    [SerializeField] private float _decceleration;
    public float Decceleration
    {
        get => (int)_tankStatsController.AccelerationStat.CalculateFinalValue(_decceleration);
    }
    
    public float RotationSpeed;

    [SerializeField] private float _maxSpeed;
    public float MaxSpeed
    {
        get => (int)_tankStatsController.SpeedStat.CalculateFinalValue(_maxSpeed);
    }

    private TankStatsController _tankStatsController = new();

    public void SetTankStatController(TankStatsController tankStatsController)
    {
        _tankStatsController = tankStatsController;
    } 
}

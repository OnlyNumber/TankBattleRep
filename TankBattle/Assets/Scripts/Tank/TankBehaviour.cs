using UnityEngine;

public class TankBehaviour : MonoBehaviour
{

    public TankMovement TankMovement;
    public HullArmor HullArmor;
    public TurretMovement TankTurret;
    public Gun TankCannon;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        TankMovement.Initialize(
            HullArmor.HullStats.Acceleration,
            HullArmor.HullStats.Decceleration,
            HullArmor.HullStats.MaxSpeed,
            HullArmor.HullStats.RotationSpeed);
    }

    public void MoveTank(Vector3 direction)
    {
        MoveTank(new Vector2(direction.x, direction.z));
    }

    public void MoveTank(Vector2 direction)
    {
        TankMovement.SetMoveDirection(direction);
    }


    public void RotateTurret(Vector3 direction)
    {
        TankTurret.RotateTurret(TankTurret.TurretStats.TurretRotationSpeed, new Vector2(direction.x, direction.z));
    }

    public void RotateTurret(Vector2 direction)
    {
        TankTurret.RotateTurret(TankTurret.TurretStats.TurretRotationSpeed, direction);
    }
}

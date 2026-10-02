using UnityEngine;

public class PlayerTank : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private TankMovement _tankMovement;
    [SerializeField] private TurretMovement _tankTurret;
    [SerializeField] private Gun _tankCannon;



    #region Tank movement (Move later)
    public float Acceleration;
    public float Decceleration;
    public float RotationSpeed;
    public float MaxSpeed;
    #endregion

    #region Turret movement (Move later)
    public float TurretRotationSpeed;

    #endregion
    
    private void Start()
    {
        _tankTurret.OnTurretRotationComplete += _tankCannon.TryShoot;
    }

    void Update()
    {
        Vector2 leftJoystickValue = _playerInput.GetLeftJoystickValues();
        //Debug.Log("Values " + joystickValue);

        if (Mathf.Abs(leftJoystickValue.y) > 0)
        {
            //if (joystickValue.y > 0)
            _tankMovement.MoveTank(Acceleration, MaxSpeed, RotationSpeed, leftJoystickValue);
            //else
            //    _tankMovement.MoveTowards(-Acceleration, MaxSpeed, Vector3.zero);

        }
        else
            _tankMovement.Decelerate(Decceleration);

        Vector2 rightJoystickValue = _playerInput.GetRightJoystickValues();

        if (Mathf.Abs(rightJoystickValue.y) > 0 && Mathf.Abs(rightJoystickValue.x) > 0)
            _tankTurret.RotateTurret(TurretRotationSpeed, rightJoystickValue);
        else
            _tankTurret.ClearLine();
    }

}

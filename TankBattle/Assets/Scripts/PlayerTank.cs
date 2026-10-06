using UnityEngine;

public class PlayerTank : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private TankBehaviour _playerTank;

    private void Start()
    {
        _playerTank.TankTurret.OnTurretRotationComplete += _playerTank.TankCannon.TryShoot;
    }

    void Update()
    {
        Vector2 leftJoystickValue = _playerInput.GetLeftJoystickValues();

        if (Mathf.Abs(leftJoystickValue.y) > 0)
        {
            _playerTank.MoveTank(leftJoystickValue);

        }
        else
            _playerTank.Decelerate();


        Vector2 rightJoystickValue = _playerInput.GetRightJoystickValues();

        if (Mathf.Abs(rightJoystickValue.y) > 0 && Mathf.Abs(rightJoystickValue.x) > 0)
            _playerTank.RotateTurret(rightJoystickValue);
        else
            _playerTank.TankTurret.ClearLine();
    }

}

using UnityEngine;

public class PlayerTank : MonoBehaviour
{
    private PlayerInput _playerInput;
    private TankBehaviour _playerTank;

    private bool _isInitialize;

    public void Initialize(TankBehaviour playerTank, PlayerInput playerInput)
    {
        _playerInput = playerInput;
        _playerTank = playerTank;

        _playerTank.TankTurret.OnTurretRotationComplete += _playerTank.TankCannon.TryShoot;

        _isInitialize = true;
    }


    void Update()
    {
        if (!_isInitialize)
            return;

        Vector2 leftJoystickValue = _playerInput.GetLeftJoystickValues();

        _playerTank.MoveTank(leftJoystickValue);

        #region RightJoystick
        Vector2 rightJoystickValue = _playerInput.GetRightJoystickValues();

        if (Mathf.Abs(rightJoystickValue.y) > 0 && Mathf.Abs(rightJoystickValue.x) > 0)
            _playerTank.RotateTurret(rightJoystickValue);
        else
            _playerTank.TankTurret.ClearLine();
        #endregion
    }

}

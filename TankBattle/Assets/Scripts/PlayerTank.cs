using UnityEngine;

public class PlayerTank : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private TankMovement _tankMovement;

    #region Move later
    public float Acceleration;
    public float Decceleration;

    public float RotationSpeed;


    public float MaxSpeed;
    #endregion

    void Update()
    {
        Vector2 joystickValue = _playerInput.GetLeftJoystickValues();
        //Debug.Log("Values " + joystickValue);

        if (Mathf.Abs(joystickValue.y) > 0)
        {
            //if (joystickValue.y > 0)
            _tankMovement.MoveTank(Acceleration, MaxSpeed, RotationSpeed, joystickValue);
            //else
            //    _tankMovement.MoveTowards(-Acceleration, MaxSpeed, Vector3.zero);

        }
        else
            _tankMovement.Decelerate(Decceleration);

    }

}

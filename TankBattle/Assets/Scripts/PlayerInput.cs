using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Joystick _leftJoystick;
    [SerializeField] private Joystick _rightJoystick;

    public Vector2 GetLeftJoystickValues()
    {
        return new Vector2(_leftJoystick.Horizontal, _leftJoystick.Vertical);
    }
    
    public Vector2 GetRightJoystickValues()
    {
        return new Vector2(_rightJoystick.Horizontal, _rightJoystick.Vertical);
    }
}

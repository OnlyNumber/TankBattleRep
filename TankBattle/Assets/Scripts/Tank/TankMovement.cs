using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    private float _acceleration;
    private float _maxSpeed;
    private float _rotationSpeed;


    private float _currentSpeed;

    public void Initialize(float acceleration, float maxSpeed, float rotationSpeed)
    {
        _acceleration = acceleration;
        _maxSpeed = maxSpeed;
        _rotationSpeed = rotationSpeed;

    }

    public void MoveTank(float acceleration, float maxSpeed, float rotationSpeed, Vector2 moveDirection)
    {
        if (Vector3.Dot(transform.forward, new Vector3(moveDirection.x, 0, moveDirection.y)) >= 0)
        {
            _currentSpeed += acceleration * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.y), Vector3.up);


            _rb.MoveRotation(Quaternion.RotateTowards(
                    _rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime));


            _rb.MovePosition(transform.position + transform.forward * _currentSpeed * Time.deltaTime);
        }
        else
        {
            _currentSpeed -= acceleration * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(-new Vector3(moveDirection.x, 0, moveDirection.y), Vector3.up);

            _rb.MoveRotation(Quaternion.RotateTowards(
                    _rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime));


            _rb.MovePosition(transform.position + transform.forward * _currentSpeed * Time.deltaTime);
        }

        Math.Clamp(_currentSpeed, maxSpeed / 2, maxSpeed);
    }

    public void Decelerate(float acceleration)
    {

        //Math.Clamp(_currentSpeed, maxSpeed / 2, maxSpeed);
        if (_currentSpeed < 0)
            _currentSpeed += acceleration * Time.deltaTime;
        if (_currentSpeed > 0)
            _currentSpeed -= acceleration * Time.deltaTime;

        if (_currentSpeed > 0.1f && _currentSpeed < 0.1f)
            _currentSpeed = 0;

        _rb.MovePosition(transform.position + transform.forward * _currentSpeed * Time.deltaTime);
    }

    public static float NormalizeAngle360(float angle)
    {
        angle = angle % 360f;
        if (angle < 0)
        {
            angle += 360f;
        }
        return angle;
    }
}

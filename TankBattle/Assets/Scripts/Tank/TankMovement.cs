using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    private float _acceleration;
    private float _deceleration;

    private float _maxSpeed;
    private float _rotationSpeed;

    private float _currentSpeed;
    private Vector3 _currentMoveDirection;


    public void Initialize(float acceleration, float deceleration, float maxSpeed, float rotationSpeed)
    {
        _acceleration = acceleration;
        _deceleration = deceleration;

        _maxSpeed = maxSpeed;
        _rotationSpeed = rotationSpeed;

    }

    void Update()
    {
        if (Mathf.Abs(_currentMoveDirection.normalized.x) > 0 || Mathf.Abs(_currentMoveDirection.normalized.y) > 0)
            MoveTank(_currentMoveDirection.normalized);
        else
            Decelerate(_deceleration);

    }

    public void SetMoveDirection(Vector3 direciton)
    {
        _currentMoveDirection = direciton;
    }

    private void MoveTank(Vector2 moveDirection)
    {
        if (Vector3.Dot(transform.forward, new Vector3(moveDirection.x, 0, moveDirection.y)) >= 0)
        {
            _currentSpeed += _acceleration * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.y), Vector3.up);


            _rb.MoveRotation(Quaternion.RotateTowards(
                    _rb.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime));

            if (_currentSpeed < 0)
            {
                Decelerate(_deceleration);
                return;
            }

            _rb.MovePosition(transform.position + transform.forward * _currentSpeed * Time.deltaTime);
        }
        else
        {
            _currentSpeed -= _acceleration * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(-new Vector3(moveDirection.x, 0, moveDirection.y), Vector3.up);

            _rb.MoveRotation(Quaternion.RotateTowards(
                    _rb.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime));

            if (_currentSpeed > 0)
            {
                Decelerate(_deceleration);
                return;
            }

            _rb.MovePosition(transform.position + transform.forward * _currentSpeed * Time.deltaTime);
        }

        Math.Clamp(_currentSpeed, _maxSpeed / 2, _maxSpeed);
    }

    private void Decelerate(float acceleration)
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
}

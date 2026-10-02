using System;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    [SerializeField] private TrajectoryLine _trajectoryLine;
    [SerializeField] private float _lineLength;

    public event Action OnTurretRotationComplete;

    void Start()
    {
        //OnTurretRotationComplete += () => Debug.Log("Shoot");
    }

    public void RotateTurret(float rotationSpeed, Vector2 moveDirection)
    {
        _trajectoryLine.SetPositions(transform.position, transform.position + new Vector3(moveDirection.x, 0, moveDirection.y).normalized * _lineLength);

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.y), Vector3.up);

        transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) < 3)
            OnTurretRotationComplete?.Invoke();
    }

    public void ClearLine()
    {
        _trajectoryLine.SetPositions(Vector3.zero, Vector3.zero);
    }


}

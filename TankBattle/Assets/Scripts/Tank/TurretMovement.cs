using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour, ITankPart
{
    [SerializeField] private TrajectoryLine _trajectoryLine;
    [SerializeField] private float _lineLength;
    [SerializeField] private PartContainer _partContainer;
    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;


    public TurretStats TurretStats;

    public event Action OnTurretRotationComplete;

    public void RotateTurret(float rotationSpeed, Vector3 moveDirection)
    {
        _trajectoryLine.SetPositions(transform.position, transform.position + moveDirection.normalized * _lineLength);

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) < 3)
            OnTurretRotationComplete?.Invoke();
    }

    public void RotateTurret(float rotationSpeed, Vector2 moveDirection)
    {
        RotateTurret(rotationSpeed, new Vector3(moveDirection.x, 0, moveDirection.y));
    }

    public void ClearLine()
    {
        _trajectoryLine.SetPositions(Vector3.zero, Vector3.zero);
    }

    public List<Description> GetDescriptions()
    {
        List<Description> descriptions = new()
        {
            new Description("Rotation speed", TurretStats.TurretRotationSpeed.ToString())
        };

        return descriptions;
    }
}

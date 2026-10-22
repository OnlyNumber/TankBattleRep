using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour, ITankPart
{
    [SerializeField] private List<StatsModifier> _statsModifiers;
    [SerializeField] private TrajectoryLine _trajectoryLine;
    [SerializeField] private float _lineLength;
    [SerializeField] private PartContainer _partContainer;
    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;

    [SerializeField] private bool _isBuiltIn;
    public bool IsBuiltIn => _isBuiltIn;

    [SerializeField] private TurretStats _turretStats;
    private TankStatsController _tankStatsController;

    public event Action OnTurretRotationComplete;
    private float currentClampedAngle = 0f;

    public void RotateTurret(Vector3 rotateDirection)
    {
        RotateTurret(_turretStats.RotationSpeed, rotateDirection);
    }

    private void RotateTurret(float rotationSpeed, Vector3 rotateDirection)
    {
        _trajectoryLine.SetPositions(transform.position, transform.position + rotateDirection.normalized * _lineLength);

        Vector3 forwardRef = transform.parent.forward;
        forwardRef.y = 0f;
        forwardRef.Normalize();

        Vector3 targetDir = rotateDirection;
        targetDir.y = 0f;
        targetDir.Normalize();

        float targetAngle = Vector3.SignedAngle(forwardRef, targetDir, Vector3.up);

        float clampedTargetAngle = Mathf.Clamp(targetAngle, -_turretStats.MaxRotationAngle / 2, _turretStats.MaxRotationAngle / 2);

        currentClampedAngle = Mathf.MoveTowardsAngle(
            currentClampedAngle, 
            clampedTargetAngle, 
            _turretStats.RotationSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.LookRotation(forwardRef) * Quaternion.Euler(0f, currentClampedAngle, 0f);

        if (Mathf.Abs(transform.rotation.eulerAngles.y - currentClampedAngle) < 3)
            OnTurretRotationComplete?.Invoke();
    }

    public void ClearLine()
    {
        _trajectoryLine.SetPositions(Vector3.zero, Vector3.zero);
    }

    public List<Description> GetDescriptions()
    {
        List<Description> descriptions = new()
        {
            new Description("Rotation speed", _turretStats.RotationSpeed.ToString()),
            new Description("Max rotate angle", _turretStats.MaxRotationAngle.ToString())
        };

        return descriptions;
    }

    public List<StatsModifier> GetStatsModifiers()
    {
        return _statsModifiers;
    }

    public void SetTankStatController(TankStatsController tankStatsController)
    {
        _tankStatsController = tankStatsController;
    }
}

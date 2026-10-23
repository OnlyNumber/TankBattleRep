using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour, ITankPart
{
    #region ITankPart implementation
    [SerializeField] private PartContainer _partContainer;
    [SerializeField] private Collider _partCollider;
    public Collider PartCollider => _partCollider;
    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;

    [SerializeField] private bool _isBuiltIn;
    public bool IsBuiltIn => _isBuiltIn;
    #endregion
 
    [SerializeField] private List<StatsModifier> _statsModifiers;
    [SerializeField] private TrajectoryLine _trajectoryLine;
    [SerializeField] private float _lineLength;
    [SerializeField] private TurretStats _turretStats;

    private TankStatsController _tankStatsController;
    private float currentClampedAngle = 0f;

    public event Action OnTurretRotationComplete;
    
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
            rotationSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.LookRotation(forwardRef) * Quaternion.Euler(0f, currentClampedAngle, 0f);

        float angle = Mathf.Atan2(rotateDirection.x, rotateDirection.z) * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360;

        if (Mathf.Abs(transform.rotation.eulerAngles.y - angle) < 3)
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

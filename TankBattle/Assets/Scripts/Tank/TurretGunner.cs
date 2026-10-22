using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretGunner
{
    private List<TurretMovement> _tankTurrets = new();
    private Transform _aimPosition;
    private float _defaultDistance = 40;
    private List<Collider> _ignoreColliders;

    private GameObject checkPoint;

    public void Initialize(List<TurretMovement> tankTurrets, Transform aimPosition, List<Collider> ignoreColliders = null)
    {
        foreach (var item in tankTurrets)
            _tankTurrets.Add(item);

        _aimPosition = aimPosition;

        _ignoreColliders = ignoreColliders;

        checkPoint = new GameObject("Check");
    }
    Vector3 endPos;
    public void RotationInput(Vector3 inputDirection)
    {
        Vector3 hittedPosition = Vector3.zero;
        
        /*
        var hits = Physics.RaycastAll(_aimPosition.position, inputDirection.normalized, _defaultDistance);
        foreach (var item in hits)
        {
            if (_ignoreColliders.Contains(item.collider))
                continue;

            hittedPosition = item.point;
            break;
        }

        if (hittedPosition == Vector3.zero)*/
        hittedPosition = _aimPosition.position + inputDirection.normalized * _defaultDistance;

        endPos = hittedPosition;

        checkPoint.transform.position = endPos;

        foreach (var turret in _tankTurrets)
        {
            Vector3 turretDirection = hittedPosition - turret.transform.position;
            turretDirection.Normalize();

            turret.RotateTurret(turretDirection);
        }
    }
}

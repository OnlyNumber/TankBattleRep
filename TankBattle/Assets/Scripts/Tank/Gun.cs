using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Gun : MonoBehaviour, ITankPart
{
    [SerializeField] private ShellInfo _shellInfo;
    [SerializeField] private float _reloadTime;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private PartContainer _partContainer;

    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;


    private bool _isReadyForShoot = true;

    public void TryShoot()
    {
        if (!_isReadyForShoot)
            return;

        var projectile = (Projectile)ControllerGameObjectPooler.GetFromPrefab(_projectilePrefab);

        projectile.transform.position = _firePoint.position;
        projectile.Initialize(_shellInfo.Speed);
        projectile.transform.LookAt(_firePoint.position + _firePoint.forward);
        _isReadyForShoot = false;

        projectile.OnHit += ReactionCollision;

        Reload(this.GetCancellationTokenOnDestroy()).Forget();

    }

    private void ReactionCollision(Projectile projectile, RaycastHit hit, Vector3 direction)
    {
        UtilitiesMath.CalculateHit(hit, _shellInfo, direction);

        projectile.ReturnToPool();
    }

    public async UniTaskVoid Reload(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_reloadTime), cancellationToken: ct);

        _isReadyForShoot = true;
    }

}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float _reloadTime;
    [SerializeField] private float _projectileSpeed;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private Projectile _projectilePrefab;

    private bool _isReadyForShoot = true;

    public void Dispose()
    {

    }

    public void TryShoot()
    {
        if (!_isReadyForShoot)
            return;

        var projectile = (Projectile)ControllerGameObjectPooler.GetFromPrefab(_projectilePrefab);

        projectile.transform.position = _firePoint.position;
        projectile.SetSpeed(_projectileSpeed);
        projectile.transform.LookAt(_firePoint.position + _firePoint.forward);
        _isReadyForShoot = false;

        projectile.ProjectileCollision.OnCollision += ReactionCollision;

        Reload(this.GetCancellationTokenOnDestroy()).Forget();

    }

    private void ReactionCollision(GameObject reactedObject, Collision collision)
    {
        var projectile = reactedObject.GetComponent<Projectile>();
        projectile.ReturnToPool();
    }

    public async UniTaskVoid Reload(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_reloadTime), cancellationToken: ct);

        _isReadyForShoot = true;
    }

}

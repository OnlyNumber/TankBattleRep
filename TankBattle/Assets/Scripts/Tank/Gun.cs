using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Gun : MonoBehaviour, ITankPart
{
    [SerializeField] private List<StatsModifier> _statsModifiers;

    [SerializeField] private ShellInfo _shellInfo;
    [SerializeField] private float _reloadTime;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private PartContainer _partContainer;

    [SerializeField] private bool _isBuiltIn;
    public bool IsBuiltIn => _isBuiltIn;

    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;

    private bool _isReadyForShoot = true;
    private Stopwatch stopwatch = new();
    private TankStatsController _tankStatsController;

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

        UniTask task = Reload(this.GetCancellationTokenOnDestroy());
    }

    private void ReactionCollision(Projectile projectile, RaycastHit hit, Vector3 direction)
    {
        UtilitiesMath.CalculateHit(hit, _shellInfo, direction);

        projectile.ReturnToPool();
    }

    private float _currentReloadTime;

    void Update()
    {
        _currentReloadTime = GetCurretTime();
    }

    public float GetCurretTime()
    {
        return (float)stopwatch.Elapsed.TotalSeconds / _reloadTime;
    }

    public async UniTask Reload(CancellationToken ct)
    {
        stopwatch.Restart();
        await UniTask.Delay(TimeSpan.FromSeconds(_tankStatsController.ReloadStat.CalculateFinalValue(_reloadTime)), cancellationToken: ct);

        _isReadyForShoot = true;
        stopwatch.Stop();
    }

    public List<Description> GetDescriptions()
    {
        List<Description> descriptions = new()
        {
            new Description("Reload", _reloadTime.ToString()),
            new Description("Speed", _shellInfo.Speed.ToString()),
            new Description("Penetration", _shellInfo.Penetration.ToString()),
            new Description("Damage", _shellInfo.Damage.ToString())
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
        _shellInfo.SetTankStatController(_tankStatsController);
    }
}

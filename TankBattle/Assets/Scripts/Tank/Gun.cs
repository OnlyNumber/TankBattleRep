using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Gun : MonoBehaviour, ITankPart
{
    private const float Spawn_Hight = 0.5f;

    [SerializeField] private List<StatsModifier> _statsModifiers;

    [SerializeField] private ShellInfo _shellInfo;
    [SerializeField] private float _baseReloadTime = 1;

    public float ReloadTime
    {
        get
        {
            return _tankStatsController.ReloadStat.CalculateFinalValue(_baseReloadTime);
        }
    }


    [SerializeField] private Transform _firePoint;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private PartContainer _partContainer;

    #region ITankPart implementation
    [SerializeField] private bool _isBuiltIn;
    public bool IsBuiltIn => _isBuiltIn;

    [SerializeField] private Collider _partCollider;
    public Collider PartCollider => _partCollider;

    public GameObject GameObject => gameObject;
    public PartContainer PartContainer => _partContainer;
    #endregion

    private bool _isReadyForShoot = true;
    private Stopwatch stopwatch = new();
    private TankStatsController _tankStatsController;
    private List<Collider> _tankColliders = new();

    public void Initialize(List<Collider> tankColliders)
    {
        _tankColliders.Clear();
        _tankColliders.AddRange(tankColliders);
    }

    public void TryShoot()
    {
        if (!_isReadyForShoot)
            return;

        var projectile = (Projectile)ControllerGameObjectPooler.GetFromPrefab(_projectilePrefab);

        var spawnPosition = _firePoint.position;
        spawnPosition.y = Spawn_Hight;

        projectile.transform.position = spawnPosition;
        projectile.Initialize(_shellInfo.Speed, _tankColliders);
        projectile.transform.LookAt(spawnPosition + _firePoint.forward);
        _isReadyForShoot = false;

        projectile.OnHit += ReactionCollision;

        UniTask task = Reload(this.GetCancellationTokenOnDestroy());
    }

    private void ReactionCollision(Projectile projectile, RaycastHit hit, Vector3 direction)
    {
        UtilitiesMath.CalculateHit(hit, _shellInfo, direction);

        projectile.ReturnToPool();
    }

    public float GetCurretTime()
    {
        return (float)stopwatch.Elapsed.TotalSeconds / ReloadTime;
    }

    public async UniTask Reload(CancellationToken ct)
    {
        stopwatch.Restart();
        await UniTask.Delay(TimeSpan.FromSeconds(ReloadTime), cancellationToken: ct);

        _isReadyForShoot = true;
        stopwatch.Stop();
    }

    public List<Description> GetDescriptions()
    {
        List<Description> descriptions = new()
        {
            new Description("Reload", _baseReloadTime.ToString()),
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

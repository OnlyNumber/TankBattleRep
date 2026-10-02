using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooled
{
    private float _speed;
    [field: SerializeField]
    public CollisionController ProjectileCollision
    {
        get;
        private set;
    }

    GameObject IPooled.GameObject => gameObject;

    private GameObjectPool _gameObjectPool;

    GameObjectPool IPooled.MyObjectPool { get => _gameObjectPool; }

    public void Initialize(GameObjectPool gameObjectPool)
    {
        _gameObjectPool = gameObjectPool;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;

        LifeTimeDeath(this.GetCancellationTokenOnDestroy()).Forget();
    }

    public void Dispose()
    {
        _speed = 0;
    }

    public virtual void ReturnToPool()
    {
        _speed = 0;
        ProjectileCollision.ClearAllActions();
        _gameObjectPool.ObjectPool.Release(this);
    }

    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private async UniTask LifeTimeDeath(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: ct);

        Destroy(gameObject);
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooled
{
    private float _speed;

    GameObject IPooled.GameObject => gameObject;

    private GameObjectPool _gameObjectPool;

    GameObjectPool IPooled.MyObjectPool { get => _gameObjectPool; }

    public event Action<Projectile, RaycastHit, Vector3> OnHit;

    private CancellationTokenSource _cts;

    [SerializeField] private float _rayDistance = 1;
    private List<Collider> _tankColliders = new();

    public void InitializePool(GameObjectPool gameObjectPool)
    {
        _gameObjectPool = gameObjectPool;
    }

    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, _rayDistance))
        {
            foreach (var collider in _tankColliders)
            {
                if (hitInfo.collider == collider)
                    return;
            }
            OnHit?.Invoke(this, hitInfo, transform.forward);
        }
    }

    private async UniTask LifeTimeDeath(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: ct);

        ReturnToPool();
    }

    public void Initialize(float speed, List<Collider> tankColliders)
    {
        _tankColliders.AddRange(tankColliders);
        _speed = speed;

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        LifeTimeDeath(_cts.Token).Forget();
    }

    public void Dispose()
    {
        _speed = 0;
    }

    public virtual void ReturnToPool()
    {
        _tankColliders.Clear();

        _speed = 0;
        OnHit = null;

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = null;
        _gameObjectPool.ObjectPool.Release(this);
    }


}

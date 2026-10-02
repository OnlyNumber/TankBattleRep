using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool
{
    private const string Pool_Parent_Name = "PoolParent"; 
    protected static Transform Pool_Parent;
    protected Transform _parentObjectPool;

    public IObjectPool<IPooled> ObjectPool;

    private IPooled _prefab;

    public GameObjectPool(IPooled prefab)
    {
        if(Pool_Parent == null)
            Pool_Parent = new GameObject(Pool_Parent_Name).transform;

        _prefab = prefab;

        _parentObjectPool = new GameObject(_prefab.GameObject.name).transform;
        _parentObjectPool.transform.SetParent(Pool_Parent);

        ObjectPool = new ObjectPool<IPooled>(Create, Get, Release, Destroy);
    }

    private IPooled Create()
    {
        var createdPooledObject = GameObject.Instantiate(_prefab.GameObject).GetComponent<IPooled>();
        createdPooledObject.GameObject.transform.SetParent(_parentObjectPool);
        createdPooledObject.Initialize(this);

        return createdPooledObject;
    }

    private void Get(IPooled pooledObject)
    {
        pooledObject.GameObject.SetActive(true);
    }

    private void Release(IPooled pooledObject)
    {
        pooledObject.GameObject.SetActive(false);
    }

    private void Destroy(IPooled pooledObject)
    {
        pooledObject.GameObject.SetActive(false);
    }
}

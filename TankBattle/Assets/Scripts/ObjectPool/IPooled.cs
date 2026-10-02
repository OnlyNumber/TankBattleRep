using UnityEngine;

public interface IPooled 
{
    public GameObject GameObject
    {
        get;
    }

    protected GameObjectPool MyObjectPool
    {
        get;
    }

    public abstract void Initialize(GameObjectPool gameObjectPool);

    public abstract void ReturnToPool();
}

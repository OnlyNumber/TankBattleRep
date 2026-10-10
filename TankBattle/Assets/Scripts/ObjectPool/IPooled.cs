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

    public abstract void InitializePool(GameObjectPool gameObjectPool);

    public void ReturnToPool();
}

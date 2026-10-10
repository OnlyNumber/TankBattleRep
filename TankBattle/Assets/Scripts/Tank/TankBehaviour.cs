using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

public class TankBehaviour : MonoBehaviour, IPooled
{

    public TankMovement TankMovement;
    public HullArmor HullArmor;
    public TurretMovement TankTurret;
    public Gun TankCannon;

    public int MaxHealth;
    public HealthSystem TankHealthSystem
    {
        get;
        private set;
    }

    GameObject IPooled.GameObject => gameObject;

    private GameObjectPool _gameObjectPool;
    GameObjectPool IPooled.MyObjectPool { get => _gameObjectPool; }

    public Action<TankBehaviour> OnDeath;

    public void Initialize()
    {
        TankHealthSystem = new HealthSystem(MaxHealth);

        TankMovement.Initialize(
            HullArmor.HullStats.Acceleration,
            HullArmor.HullStats.Decceleration,
            HullArmor.HullStats.MaxSpeed,
            HullArmor.HullStats.RotationSpeed);

        TankHealthSystem.OnHealthChanged += Death;
    }

    public void SetUpParts(ITankPart mainHullPart)
    {
        HullArmor = mainHullPart as HullArmor;
        TankTurret = FindTurrets(HullArmor)[0];
        TankCannon = FindGuns(TankTurret)[0];
    }

    private List<TurretMovement> FindTurrets(ITankPart tankPart)
    {
        List<TurretMovement> turretParts = new();

        foreach (var item in tankPart.PartContainer.TurretPlace)
        {
            if (item.Part != null && item.Part is TurretMovement turret)
                turretParts.Add(turret);

        }

        return turretParts;
    }

    private List<Gun> FindGuns(ITankPart tankPart)
    {
        List<Gun> gunParts = new();

        foreach (var item in tankPart.PartContainer.GunPlace)
        {
            if (item.Part != null && item.Part is Gun gun)
                gunParts.Add(gun);

        }

        return gunParts;
    }



    private void Death()
    {
        if (TankHealthSystem.CurrentHealth <= 0)
        {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void MoveTank(Vector3 direction)
    {
        MoveTank(new Vector2(direction.x, direction.z));
    }

    public void MoveTank(Vector2 direction)
    {
        TankMovement.SetMoveDirection(direction);
    }

    public void RotateTurret(Vector3 direction)
    {
        TankTurret.RotateTurret(TankTurret.TurretStats.TurretRotationSpeed, new Vector2(direction.x, direction.z));
    }

    public void RotateTurret(Vector2 direction)
    {
        TankTurret.RotateTurret(TankTurret.TurretStats.TurretRotationSpeed, direction);
    }

    public virtual void InitializePool(GameObjectPool gameObjectPool)
    {
        _gameObjectPool = gameObjectPool;
    }

    public virtual void ReturnToPool()
    {
        _gameObjectPool.ObjectPool.Release(this);
    }
}

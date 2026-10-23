using System;
using System.Collections.Generic;
using ModestTree;
using Unity.VisualScripting;
using UnityEngine;

public class TankBehaviour : MonoBehaviour, IPooled
{

    public TankMovement TankMovement;
    public HullArmor HullArmor;
    public List<TurretMovement> TankTurrets = new();
    public List<Gun> TankCannon = new();

    private TankStatsController _tankStatsController = new();

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

    private TurretGunner _turretGunner = new();

    public Action<TemporaryModifier> OnAddingTemporaryModifier;

    public void Initialize()
    {
        TankHealthSystem = new HealthSystem(MaxHealth);
        TankHealthSystem.OnHealthChanged += Death;

        TankMovement.Initialize(
            HullArmor.HullStats);

        List<Collider> ignoreColliders = new();

        foreach (var item in HullArmor.armorStats)
        {
            ignoreColliders.Add(item.Collider);
        }

        var parts = TankCreator.GetPartsList(HullArmor);

        foreach (var item in parts)
            item.PartCollider.enabled = false;

        _turretGunner.Initialize(TankTurrets, HullArmor.AimPosition, ignoreColliders);
        GetModifiersFromParts();
    }

    public void SetUpParts(ITankPart mainHullPart)
    {
        HullArmor = mainHullPart as HullArmor;
        TankTurrets = FindParts<TurretMovement>(HullArmor.PartContainer.TurretPlace);

        foreach (var item in TankTurrets)
            TankCannon.AddRange(FindParts<Gun>(item.PartContainer.GunPlace));

        List<Collider> tankColliders = new();

        foreach (var item in HullArmor.armorStats)
        {
            tankColliders.Add(item.Collider);
        }

        foreach (var cannon in TankCannon)
        {
            if (cannon != null)
                cannon.Initialize(tankColliders);
        }
    }

    public void AddTemporaryModifier(TemporaryModifier temporaryModifier)
    {
        OnAddingTemporaryModifier?.Invoke(temporaryModifier);
        temporaryModifier.ActivateModifier(_tankStatsController);
    }

    public bool IsContainThisModifier(TemporaryModifierData temporaryModifier)
    {
        return _tankStatsController.IsContainModifier(temporaryModifier);
    }

    private void GetModifiersFromParts()
    {
        HashSet<ITankPart> tankParts = new();
        tankParts.Add(HullArmor);
        tankParts.AddRange(TankTurrets.ToArray());
        tankParts.AddRange(TankCannon);

        foreach (var part in tankParts)
        {
            if (part == null)
                continue;

            part.SetTankStatController(_tankStatsController);

            foreach (var modifier in part.GetStatsModifiers())
            {
                _tankStatsController.GetStat(modifier.ModifierStat).AddModifier(modifier);
            }
        }
    }

    public static List<T> FindParts<T>(List<PartContainerSlot> slots) where T : ITankPart
    {
        List<T> parts = new();

        foreach (var item in slots)
        {
            if (item.Part != null && item.Part is T needPart)
            {
                parts.Add(needPart);
                continue;
            }

            var child = item.Parent.GetChild(0);

            if (child != null)
            {
                item.Part = child.GetComponent<ITankPart>();

                if (item.Part is T needPart1)
                {
                    parts.Add(needPart1);
                }

            }

        }

        return parts;
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
        _turretGunner.RotationInput(direction);
        //TankTurret.RotateTurret(TankTurret.TurretStats.TurretRotationSpeed, new Vector2(direction.x, direction.z));
    }

    public void RotateTurret(Vector2 direction)
    {
        RotateTurret(new Vector3(direction.x, 0, direction.y));
        //TankTurret.RotateTurret(TankTurret.TurretStats.TurretRotationSpeed, direction);
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

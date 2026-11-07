using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using Zenject.SpaceFighter;

[System.Serializable]
public class EnemySpawner
{
    [SerializeField] private Transform ParentForAI;

    [SerializeField] private List<TankBehaviour> enemyTankPrefabs;
    [SerializeField] private EnemyAI enemyAIPrefab;
    [SerializeField] private FollowingHealthBar healthBarPrefab;

    //TODO: Change to Zenject

    [SerializeField] private List<Transform> _spawnPositions;

    [SerializeField] private int waveCount;
    [field: SerializeField]
    public int EnemiesPerWave
    {
        get;
        private set;
    }

    private TankBehaviour _targetTank;
    private int currentWave;
    private int _currentDeadEnemies;
    public int CurrentDeadEnemies
    {
        get => _currentDeadEnemies;
        private set
        {
            _currentDeadEnemies = value;
            OnChangedDeadEnemiesCount?.Invoke();
        }
    }
    public Action OnChangedDeadEnemiesCount;
    public Action<TankBehaviour> OnDeathSpawnedUnit;
    public Action OnEndWaves;

    public void Initialize(TankBehaviour targetTank)
    {
        _targetTank = targetTank;
    }

    public void ActivateWaves()
    {
        if (currentWave >= waveCount)
        {
            currentWave = 0;
            OnEndWaves?.Invoke();
            return;
        }

        CurrentDeadEnemies = 0;
        currentWave++;

        PlayerInstance.Instance.StartCoroutine(SpawnWave());
    }

    public IEnumerator SpawnWave()
    {
        for (int i = 0; i < EnemiesPerWave; i++)
        {
            var position = CheckPosition(_spawnPositions);
            if (position == UtilitiesMath.NoAccessibleVector)
                yield return null;

            var tank = GameObject.Instantiate(enemyTankPrefabs[UnityEngine.Random.Range(0, enemyTankPrefabs.Count)]);
            var tankAI = GameObject.Instantiate(enemyAIPrefab);
            var tankHealthBar = GameObject.Instantiate(healthBarPrefab);

            tank.SetUpParts(tank.GetComponentInChildren<HullArmor>());
            tank.Initialize();

            tank.transform.position = position;

            tankAI.transform.SetParent(ParentForAI);
            tankAI.Initialize(tank, _targetTank);
            tankAI.ActivateAI();

            tankHealthBar.Initialize(tank.transform, tank);

            tank.OnDeath += DeathOfEnemy;

            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
    }

    private Vector3 CheckPosition(List<Transform> transforms)
    {
        foreach (var item in transforms.Shuffle())
        {
            if (!Physics.Raycast(item.position, Vector3.up))
                return item.position;
        }

        return UtilitiesMath.NoAccessibleVector;
    }

    private void DeathOfEnemy(TankBehaviour tankBehaviour)
    {
        OnDeathSpawnedUnit?.Invoke(tankBehaviour);

        CurrentDeadEnemies++;
        if (CurrentDeadEnemies >= EnemiesPerWave)
            ActivateWaves();

    }

}

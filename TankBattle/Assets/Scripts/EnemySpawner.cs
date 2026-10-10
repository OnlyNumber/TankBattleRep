using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

[System.Serializable]
public class EnemySpawner
{
    [SerializeField] private Transform ParentForAI;

    [SerializeField] private TankBehaviour enemyTankPrefab;
    [SerializeField] private EnemyAI enemyAIPrefab;
    [SerializeField] private FollowingHealthBar healthBarPrefab ;

    //TODO: Change to Zenject

    [SerializeField] private List<Transform> _spawnPositions;

    [SerializeField] private int waveCount;
    [field: SerializeField] public int EnemiesPerWave
    {
        get;
        private set;
    }

    private TankBehaviour _targetTank;
    private int currentWave;
    public int currentDeadEnemies
    {
        get;
        private set;
    }

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

        currentWave++;

        SpawnWave();

    }

    public void SpawnWave()
    {
        for (int i = 0; i < EnemiesPerWave; i++)
        {
            var tank = GameObject.Instantiate(enemyTankPrefab);
            var tankAI = GameObject.Instantiate(enemyAIPrefab);
            var tankHealthBar = GameObject.Instantiate(healthBarPrefab);


            tank.Initialize();
            tank.transform.position = _spawnPositions[UnityEngine.Random.Range(0, _spawnPositions.Count)].position;
            
            tankAI.transform.SetParent(ParentForAI);
            tankAI.Initialize(tank, _targetTank);
            tankAI.ActivateAI();

            tankHealthBar.Initialize(tank.transform, tank.TankHealthSystem);

            tank.OnDeath +=DeathOfEnemy;
        }
    }

    private void DeathOfEnemy(TankBehaviour tankBehaviour)
    {
        OnDeathSpawnedUnit?.Invoke(tankBehaviour);

        currentDeadEnemies++;
        if (currentDeadEnemies >= EnemiesPerWave)
        {
            currentDeadEnemies = 0;
            ActivateWaves();
        }

    }

}

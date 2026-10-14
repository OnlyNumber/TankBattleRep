using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject.SpaceFighter;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private RewardGiver _rewardGiver;
    [SerializeField] private CompleteWaveWindow _completeWaveWindow;


    [SerializeField] private PlayerUI _playerUI;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerTank _playerTankControlling;

    [SerializeField] private TankBehaviour _baseTankBehaviour;
    [SerializeField] private GunReloadIndicator _gunReloadIndicatorPrefab;



    void Start()
    {
        Delay().Forget();
    }

    private void Initialize()
    {
        var playerTank = CreatePlayer();


        playerTank.Initialize();
        _playerTankControlling.Initialize(playerTank, _playerInput);
        _playerUI.Initialize(playerTank.TankHealthSystem);

        foreach (var turret in playerTank.TankTurrets)
        {
            foreach (var cannon in TankBehaviour.FindParts<Gun>(turret.PartContainer.GunPlace))
            {
                turret.OnTurretRotationComplete += cannon.TryShoot;
            }
        }


        /*
                var reloadIndicator = Instantiate(_gunReloadIndicatorPrefab);
                reloadIndicator.Initialize(playerTank.TankCannon);
                reloadIndicator.transform.SetParent(playerTank.TankCannon.transform);
                reloadIndicator.transform.localPosition = new Vector3(0, 0.5f, 0);
        */

        _enemySpawner.Initialize(playerTank);
        _enemySpawner.OnDeathSpawnedUnit += _rewardGiver.GetRewardForEnemy;
        _enemySpawner.OnEndWaves += () => _rewardGiver.RewardMessage(playerTank.transform.position, 200);
        _enemySpawner.OnEndWaves += _completeWaveWindow.Show;

        _enemySpawner.ActivateWaves();



    }

    private async UniTaskVoid Delay()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        Initialize();
    }

    private TankBehaviour CreatePlayer()
    {
        //Change to creation from parts later
        var mainHull = TankCreator.PartSaveToTankParts(PlayerInstance.Instance.PlayerData.CurrentTank);
        var tankBehaviour = ControllerGameObjectPooler.GetFromPrefab(_baseTankBehaviour) as TankBehaviour;//.GameObject.GetComponent<TankBehaviour>();

        mainHull.GameObject.transform.SetParent(tankBehaviour.transform);
        mainHull.GameObject.transform.localPosition = Vector3.zero;

        tankBehaviour.SetUpParts(mainHull);

        return tankBehaviour;
    }
}

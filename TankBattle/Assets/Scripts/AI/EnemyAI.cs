using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private TankBehaviour MyTank;
    [SerializeField] private TankBehaviour TargetTank;

    private CancellationTokenSource _cts;

    public EnemySettings enemySettings;
    private NavMeshPath _currentPath;
    private int _currentPathIndex;

    public void Initialize(TankBehaviour controllingTank, TankBehaviour targetTank)
    {
        MyTank = controllingTank;
        TargetTank = targetTank;
    }

    [ContextMenu("ActivateAI")]
    public void ActivateAI()
    {
        _cts?.Cancel();
        _cts?.Dispose();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        CalculatePath(MyTank.transform.position, TargetTank.transform.position);
        ActivityAsync().Forget();
    }

    private async UniTask ActivityAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            if (MyTank == null)
                _cts.Cancel();

            GoToTheTarget();
            TryShootTarget();

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    public void GoToTheTarget()
    {
        if (MyTank == null || MyTank.transform == null)
            return;

        Vector3 myPosition = MyTank.transform.position;
        Vector3 targetPosition = TargetTank.transform.position;


        if (Vector3.Distance(myPosition, targetPosition) < enemySettings.StopMovingRange)
        {
            MyTank.MoveTank(Vector2.zero);
            return;
        }

        if (_currentPath == null || _currentPath.corners[_currentPath.corners.Length - 1] != targetPosition)
            CalculatePath(myPosition, targetPosition);

        if (Vector3.Distance(myPosition, _currentPath.corners[_currentPathIndex]) < 1)
            _currentPathIndex++;

        MyTank.MoveTank((_currentPath.corners[_currentPathIndex] - myPosition).normalized);
    }

    public void TryShootTarget()
    {

        if (MyTank == null || MyTank.transform == null)
            return;
        Vector3 myPosition = MyTank.transform.position;
        Vector3 targetPosition = TargetTank.transform.position;

        if (Vector3.Distance(myPosition, targetPosition) < enemySettings.ShootRange)
        {
            MyTank.RotateTurret((targetPosition - myPosition).normalized);

            MyTank.TankCannon.TryShoot();
        }
    }

    private void CalculatePath(Vector3 startPosition, Vector3 endPosition)
    {
        _currentPath = new NavMeshPath();
        _currentPathIndex = 0;

        NavMesh.CalculatePath(startPosition, endPosition, NavMesh.AllAreas, _currentPath);
    }
}

[System.Serializable]
public struct EnemySettings
{
    public float StopMovingRange;

    public float ShootRange;

}
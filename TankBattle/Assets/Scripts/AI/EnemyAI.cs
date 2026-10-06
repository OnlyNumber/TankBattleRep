using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    public TankBehaviour MyTank;
    public TankBehaviour TargetTank;

    private CancellationTokenSource _cts;

    public EnemySettings enemySettings;
    private NavMeshPath _currentPath;
    private int _currentPathIndex;

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
            GoToTheTarget();
            TryShootTarget();

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    public void GoToTheTarget()
    {
        Vector3 myPosition = MyTank.transform.position;
        Vector3 targetPosition = TargetTank.transform.position;


        if (Vector3.Distance(myPosition, targetPosition) < enemySettings.StopMovingRange)
            return;

        if (_currentPath == null || _currentPath.corners[_currentPath.corners.Length - 1] != targetPosition)
            CalculatePath(myPosition, targetPosition);

        if (Vector3.Distance(myPosition, _currentPath.corners[_currentPathIndex]) < 1)
            _currentPathIndex++;

        MyTank.MoveTank((_currentPath.corners[_currentPathIndex] - myPosition).normalized);
    }

    public void TryShootTarget()
    {

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

    private void OnDrawGizmos()
    {
        //Gizmos.DrawRay(MyTank.transform.position, (_currentPath.corners[_currentPathIndex] - transform.position).normalized);
    }

}

[System.Serializable]
public struct EnemySettings
{
    public float StopMovingRange;

    public float ShootRange;

}
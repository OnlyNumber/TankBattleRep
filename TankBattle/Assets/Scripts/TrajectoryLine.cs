using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryLine : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    //[SerializeField] private float _lineLength;
    //private CancellationTokenSource _cTSource;

    /*public void Activate()
    {
        _cTSource = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        RenderTrajectoryLine(_cTSource.Token).Forget();
    }

    public void Deactivate()
    {
        _cTSource?.Cancel();
        _cTSource?.Dispose();

        _cTSource = null;

    }*/

    public void SetPositions(Vector3 startPosition, Vector3 endPosition )
    {
        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);

    }

    /*private async UniTaskVoid RenderTrajectoryLine(CancellationToken cT)
    {
        while(!cT.IsCancellationRequested)
        {
            _lineRenderer.SetPosition(0, )
            
            await UniTask.Yield(PlayerLoopTiming.Update, cT, true);
        }

    }*/
    
}

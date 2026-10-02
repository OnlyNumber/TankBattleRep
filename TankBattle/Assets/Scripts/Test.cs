using UnityEngine;

public class Test : MonoBehaviour
{
    public Projectile projectile;

    [ContextMenu("TestFunc")]
    public void TestFunc()
    {
        ControllerGameObjectPooler.GetFromPrefab(projectile);
    }
}

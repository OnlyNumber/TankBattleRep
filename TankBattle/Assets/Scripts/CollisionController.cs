using System;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public event Action<GameObject, Collider> OnTrigger;
    public event Action<GameObject, Collision> OnCollision;


    private void OnggerEnter(Collider other)
    {
        OnTrigger?.Invoke(gameObject, other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(gameObject, collision);
    }

    public void ClearAllActions()
    {
        OnTrigger = null;
        OnCollision = null;
    }
}

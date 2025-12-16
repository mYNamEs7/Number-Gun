using System;
using System.Linq;
using Dreamteck;
using UnityEngine;

public abstract class BulletTarget : MonoBehaviour
{
    protected enum State
    {
        Enable, Disable, Collect
    }

    protected BoxCollider[] thisColliders;
    [NonSerialized] public Transform thisTransform;
    protected State state = State.Disable;

    protected virtual void Awake()
    {
        thisTransform = transform;
        thisColliders = GetComponents<BoxCollider>();
        thisColliders.ForEach(x => x.enabled = false);
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Disable:
                if (PlayerController.Instance.thisTransform.position.z + 30 >= thisTransform.position.z) EnableCollision();
                break;
            case State.Enable:
                if (thisTransform.position.z < PlayerController.Instance.thisTransform.position.z) DisableCollision();
                break;
        }
    }

    public bool CheckCollision(Vector3 startPoint, Vector3 endPoint) => thisColliders.Any(x => x.bounds.IntersectRay(new Ray(startPoint, endPoint), out float dist) && dist < Vector3.Distance(startPoint, endPoint));
    public abstract void BulletHit(int damage, int multiplyDamage = 1);

    protected void EnableCollision()
    {
        if (state == State.Enable) return;
        thisColliders.ForEach(x => x.enabled = true);
        PhysicManager.AddBulletTarget(this);
        state = State.Enable;
    }

    protected virtual void DisableCollision()
    {
        if (state == State.Disable) return;
        thisColliders.ForEach(x => x.enabled = false);
        PhysicManager.RemoveBulletTarget(this);
        state = State.Disable;
    }

    protected virtual void Collect()
    {
        if (state == State.Collect) return;
        DisableCollision();
        state = State.Collect;
        IMovingObject[] movingObject = GetComponents<IMovingObject>();
        if (movingObject != null) movingObject.ForEach(x => x.Destroy());
    }

    public virtual void Destroy()
    {
        DisableCollision();
        Destroy(gameObject);
    }
}
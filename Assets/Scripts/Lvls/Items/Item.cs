using System;
using UnityEngine;

public class Item : MonoBehaviour, IWeaponTarget
{
    public Collider thisCollider;
    protected Transform thisTransform;
    [NonSerialized] public Rigidbody thisRigidbody;

    void Awake()
    {
        thisTransform = transform;
        if (TryGetComponent(out Rigidbody rb)) thisRigidbody = rb;
        else thisRigidbody = gameObject.AddComponent<Rigidbody>();
        thisRigidbody.constraints = ~RigidbodyConstraints.FreezePositionY;
    }

    public void SetKinematic(bool enable = true)
    {
        thisRigidbody.isKinematic = enable;
        thisRigidbody.useGravity = !enable;
        thisCollider.enabled = !enable;
    }

    public void Drop()
    {
        thisTransform.parent = Level.Instance.thisTransform;
        SetKinematic(false);
        thisRigidbody.velocity += Vector3.up * 4;
    }

    public void UnFreezePos() => thisRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

    public virtual void WeaponHit(Collider collider)
    {
        thisRigidbody.isKinematic = true;
    }
}

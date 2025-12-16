using UnityEngine;

public class CashGun : ScaleBulletTarget
{
    [SerializeField] int maxCount;
    [SerializeField] Vector2 shootStrength;
    [SerializeField] Vector3 gunPoint;
    [SerializeField] Renderer thisRenderer;
    Material material;

    protected override void Awake()
    {
        base.Awake();
        material = new(thisRenderer.sharedMaterial);
        thisRenderer.sharedMaterial = material;
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        if (maxCount <= 0) return;
        maxCount--;

        Cash cash = Instantiate(GameData.Default.cash, thisTransform.TransformPoint(gunPoint), Quaternion.Euler(0, Random.Range(-180f, 180f), 0), thisTransform.parent);
        Physics.IgnoreCollision(cash.thisCollider, thisColliders[0]);
        cash.UnFreezePos();
        cash.Drop();
        cash.thisRigidbody.velocity += (-thisTransform.forward + thisTransform.right * Random.Range(-1f, 1f)) * Random.Range(shootStrength.x, shootStrength.y);

        if (maxCount == 0) Disable();
    }

    void Disable()
    {
        material.SetTexture("_BaseMap", null);
        material.SetColor("_BaseColor", Color.gray);
    }
}

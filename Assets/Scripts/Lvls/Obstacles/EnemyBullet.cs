using DG.Tweening;
using UnityEngine;

public class EnemyBullet : MonoBehaviour, IWeaponTarget
{
    Rigidbody thisRigidbody;
    Vector3 dir;
    float speed;
    int damage;
    bool hit;
    Tween destroyTween;

    public void Shoot(Vector3 dir, float speed, int damage, float time = 7)
    {
        transform.parent = Level.Instance.thisTransform;
        thisRigidbody = GetComponent<Rigidbody>();
        this.dir = dir;
        this.speed = speed;
        this.damage = damage;
        destroyTween = DOTween.Sequence().SetDelay(time).OnComplete(() => Destroy(gameObject));
    }

    public void WeaponHit(Collider collider)
    {
        if (hit) return;
        hit = true;
        PlayerController.Instance.Damage(damage);
        destroyTween.Kill(true);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out PlayerController player)) destroyTween.Kill(true);
    }

    void FixedUpdate() => thisRigidbody.position += dir * (Time.fixedDeltaTime * speed);
}

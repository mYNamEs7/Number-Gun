using UnityEngine;

public class DamageObstacle : MonoBehaviour, IWeaponTarget
{
    [SerializeField] int damage;
    [SerializeField] float pushStrength;
    bool invisable;

    public void WeaponHit(Collider collider)
    {
        if (invisable) return;
        invisable = true;
        PlayerController.Instance.Damage(damage);
        PlayerController.Instance.Push(pushStrength);
    }
}
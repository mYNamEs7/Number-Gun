using UnityEngine;

public class DeathObstacle : MonoBehaviour, IWeaponTarget
{
    public void WeaponHit(Collider collider)
    {
        GameManager.Instance.Finish(false);
    }
}
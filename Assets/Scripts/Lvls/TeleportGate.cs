using System;
using UnityEngine;

public class TeleportGate : BulletTarget, IWeaponTarget
{
    public event Action OnCollect;
    
    [SerializeField] Vector3 point;
    [SerializeField] Material skyboxMaterial;
    bool collect;

    public override void BulletHit(int damage, int multiplyDamage = 1) { }

    public virtual void WeaponHit(Collider collider)
    {
        if (collect) return;
        collect = true;
        OnCollect?.Invoke();
        
        RenderSettings.skybox = skyboxMaterial;
        PlayerController.Instance.thisTransform.position = point;
        Instantiate(GameData.Default.teleportParticles, point, Quaternion.identity);
        Destroy();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.2f, 1, 0.3f);
        Gizmos.DrawSphere(point + Vector3.up * 0.5f + Vector3.forward, 0.5f);
        Gizmos.DrawCube(point + Vector3.up * 0.5f, Vector3.one);
        Gizmos.DrawWireSphere(point + Vector3.up * 0.5f + Vector3.forward, 0.5f);
        Gizmos.DrawWireCube(point + Vector3.up * 0.5f, Vector3.one);
        Gizmos.DrawLine(transform.position, point + Vector3.up * 0.5f);
    }
#endif
}

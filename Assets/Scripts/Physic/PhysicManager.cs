using System.Collections.Generic;
using UnityEngine;

public class PhysicManager : MonoBehaviour
{
    private static PhysicManager _instance;
    public static PhysicManager Instance { get => _instance; }
    public PhysicManager()
    {
        _instance = this;
        BulletTargets = new();
    }

    public static List<BulletTarget> BulletTargets;
    public static void AddBulletTarget(BulletTarget bulletTarget)
    {
        int index = 0;
        float z = bulletTarget.thisTransform.position.z;
        for (; index < BulletTargets.Count; index++) if (BulletTargets[index].thisTransform.position.z > z) break;
        BulletTargets.Insert(index, bulletTarget);
    }
    public static void RemoveBulletTarget(BulletTarget bulletTarget) => BulletTargets.Remove(bulletTarget);

    void Awake() => GameManager.OnRestartEvent += () => BulletTargets.Clear();
}
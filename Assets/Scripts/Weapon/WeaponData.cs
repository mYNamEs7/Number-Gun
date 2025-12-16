using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public int lvl;
    public Bullet bullet;
    public int damage = 1;
}

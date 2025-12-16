using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Card Data")]
public class CardData : ScriptableObject
{
    [Serializable]
    public class Lvl
    {
        public int needBullets;
        public string ruName, enName, trName;
    }

    [Serializable]
    public class Upgrade
    {
        public WeaponUpgrade upgrade;
        public string ruName, enName, trName;
    }

    public Lvl[] lvls;
    public Upgrade[] upgrades;
}
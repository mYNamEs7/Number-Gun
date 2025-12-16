using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Block Upgrade")]
public class BlockUpgrade : Upgrade
{
    public static readonly int[] maxValues = { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, };

    public override int CurPrice => 15 + (CurLvl + 1) * (CurLvl + 1 > 2 ? 5 : 10);

    protected override float value
    {
        get
        {
            var lvl = CurLvl + 1;
            for (int i = 0; i < maxValues.Length; i++)
            {
                if (lvl - maxValues[i] < 0) return i;
                lvl -= maxValues[i];
            }

            return maxValues.Length;
        }
    }
}

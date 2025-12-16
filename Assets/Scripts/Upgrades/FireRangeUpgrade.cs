using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/FireRange Upgrade")]
public class FireRangeUpgrade : Upgrade
{
    // protected override float value => 1 + (CurLvl + 2) * 0.01f;
    protected override float value
    {
        get
        {
            var value = 1f;
            for (int i = 0; i < CurLvl + 1; i++)
            {
                value += value * 0.05f;
            }

            return value;
        }
    }
}
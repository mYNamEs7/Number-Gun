using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollarSign : Clock
{
    [SerializeField] private int hp = 20;
    [SerializeField] private Vector3 cashOffset;

    private int curHP;

    protected override void Awake()
    {
        base.Awake();
        curHP = hp;
    }

    protected override void ActionOnBulletHit(int damage, int multiplyDamage = 1)
    {
        curHP -= damage;
        if (curHP <= 0)
        {
            DropMoney();
            curHP = hp;
        }
    }

    private void DropMoney()
    {
        Cash cash = Instantiate(GameData.Default.cash, thisTransform.TransformPoint(cashOffset), Quaternion.Euler(0, Random.Range(-180f, 180f), 0), thisTransform.parent);
        cash.UnFreezePos();
        cash.Drop();
        cash.thisRigidbody.velocity += (Vector3.back + Vector3.right * Random.Range(-1f, 1f)) * Random.Range(4, 6);
    }
}

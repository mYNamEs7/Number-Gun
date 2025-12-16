using UnityEngine;

public class ResizePodium : Podium
{
    public const float MinHeight = 1.25f, MaxHeight = 20.0f;
    public const float AddHPHeight = -0.55f, AddItemHeight = 0.025f;
    public const float ColliderWidth = 3.75f, ColliderLength = 2.25f;

    [SerializeField] Transform podiumModel;
    [SerializeField, HideInInspector, Range(MinHeight, MaxHeight)] float height = 1.0f;
    [SerializeField, HideInInspector, Range(0.1f, 5.0f)] float width = 1.0f;
    Transform itemTransform;
    float curHeight;

    protected override void Awake()
    {
        if (item) itemTransform = item.transform;
        curHeight = height;
        base.Awake();
    }

    protected override void UpdateHP()
    {
        base.UpdateHP();
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        base.BulletHit(damage);
    }

    void Update()
    {
        float height = Mathf.Lerp(this.height, MinHeight, 1 - (float)curHP / hp);
        curHeight = Mathf.Lerp(curHeight, height, Time.deltaTime * 9);
        podiumModel.localScale = new Vector3(podiumModel.localScale.x, curHeight, podiumModel.localScale.z);
        hpTxtTransform.localPosition = new Vector3(hpTxtTransform.localPosition.x, curHeight + AddHPHeight, hpTxtTransform.localPosition.z);
        if (item) itemTransform.localPosition = new Vector3(itemTransform.localPosition.x, curHeight + AddItemHeight, itemTransform.localPosition.z);
    }
}
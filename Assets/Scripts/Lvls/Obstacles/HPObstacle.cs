using UnityEngine;
using UnityEngine.UI;

public class HPObstacle : BulletTarget
{
    private enum UpdateSliderType
    {
        LocalPosition,
        Image_FillAmount
    }
    
    [SerializeField] protected int hp;
    [SerializeField] GameObject hpGameObject;
    [SerializeField] Transform hpSlider;
    [SerializeField] private UpdateSliderType updateSliderType;
    protected int curHP;
    private Image hpSliderImage;

    protected override void Awake()
    {
        base.Awake();
        curHP = hp;
        if (updateSliderType == UpdateSliderType.Image_FillAmount)
            hpSliderImage = hpSlider.GetComponent<Image>();
    }

    protected void EnableHp() => hpGameObject.gameObject.SetActive(true);

    protected void DisableHp()
    {
        if (hpGameObject)
            hpGameObject.gameObject.SetActive(false);
    }

    void UpdateHP()
    {
        if (updateSliderType == UpdateSliderType.LocalPosition)
            hpSlider.localPosition = new Vector3(1 - (float)curHP / hp, 0, 0);
        else
            hpSliderImage.fillAmount = (float)curHP / hp;
    }

    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        curHP -= damage;
        UpdateHP();
        if (curHP <= 0) Broke();
    }

    protected virtual void Broke()
    {
        Collect();
        Destroy(hpGameObject);
    }
}

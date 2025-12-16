using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class UIUpgrade : MonoBehaviour
{
    readonly static Dictionary<Language, string> LvlName = new Dictionary<Language, string> { { Language.RU, "УРОВЕНЬ " }, { Language.EN, "LEVEL " }, { Language.TR, "SEVİYE " } };
    readonly static Dictionary<Language, string> maxLvlName = new Dictionary<Language, string> { { Language.RU, "МАКС" }, { Language.EN, "MAX" }, { Language.TR, "MAKS" } };
    readonly static Color LockColor = new Color(0, 0, 0, 0.675f);
    readonly static Color UpgradeColor = new Color(1, 0.85f, 0.1f, 0.8f);

    [SerializeField] UpgradeType type;
    [SerializeField] Image maskImage;
    Tween colorTween;
    Tween shakeTween;

    [Space]
    [SerializeField] GameObject price;
    [SerializeField] protected TextMeshProUGUI priceTxt;
    [SerializeField] protected TextMeshProUGUI lvlTxt;
    [SerializeField] RectTransform pricePanelTransform;
    [SerializeField] GameObject watchAD;
    RectTransform thisTransform;
    Tween adDelayTween;
    protected bool canShowAd = true;

    protected virtual void Awake()
    {
        if (GameManager.GameEnabled) Init();
        else GameManager.OnInitEvent += Init;
    }

    protected virtual void Init()
    {
        thisTransform = GetComponent<RectTransform>();
        GameData.UpdateCashEvent += UpdateCash;
        UpdateCash();
        UpdatePrice();
        UpdateLvl();
    }

    void UpdateCash()
    {
        if (Data.MaxLvl <= Data.CurLvl) return;
        if (GameManager.CanWatchAD && canShowAd)
        {
            bool lack = Data.CurPrice > GameData.Cash;
            if (!watchAD.activeInHierarchy && lack)
            {
                adDelayTween.Kill();
                adDelayTween = DOTween.Sequence().SetDelay(0.5f).OnComplete(() => { });
            }
            watchAD.SetActive(lack);
            price.SetActive(!lack);
        }
        else
        {
            colorTween.Kill(true);
            if(maskImage) maskImage.color = Data.CurPrice > GameData.Cash ? LockColor : Color.clear;
        }
        
        shakeTween.Kill(true);
        if (Data.CurPrice <= GameData.Cash) shakeTween = thisTransform.DOShakeRotation(1, 9).SetDelay(Random.Range(0.5f, 1.5f)).SetUpdate(true).SetAutoKill(false).OnComplete(() => shakeTween.Restart());
    }
    protected virtual void UpdatePrice() => priceTxt.text = Data.MaxLvl > Data.CurLvl ? $"{Data.CurPrice}" /*+ "<sprite=0>"*/ : maxLvlName[GameData.Language];
    protected virtual void UpdateLvl() => lvlTxt.text = LvlName[GameData.Language] + (Data.CurLvl + 2);

    public void Buy()
    {
        if (Data.MaxLvl <= Data.CurLvl) return;
        if (GameData.Default.PayCash(Data.CurPrice)) GetUpgrade();
        else if (!adDelayTween.IsActive() && GameManager.CanWatchAD && canShowAd) GameManager.ShowRewardVideo(GetUpgrade);
    }

    void GetUpgrade()
    {
        Data.LvlUp();
        YandexGame.SaveProgress();

        UpdatePrice();
        UpdateLvl();
        UpdateCash();

        colorTween.Kill(true);
        if(maskImage) colorTween = maskImage.DOColor(UpgradeColor, 0.33f).From();
        SoundHolder.Default.PlayFromSoundPack("Buy");

        int count = Random.Range(2, 5);
        for (int i = 0; i < count; i++)
        {
            GameObject arrow = Instantiate(GameData.Default.upgradeArrowUI, thisTransform);
            Transform arrowTransform = arrow.transform;
            arrowTransform.localPosition = pricePanelTransform.localPosition + Vector3.right * Random.Range(-90f, 90f);
            arrowTransform.localScale *= Random.Range(0.75f, 1.25f);

            float time = Random.Range(0.7f, 1.1f);
            arrowTransform.DOLocalMoveY(Random.Range(120f, 300f), time);
            arrowTransform.DOScale(0, 0.33f).SetDelay(time - 0.33f);
            arrow.GetComponent<Image>().DOFade(0, 0.33f).SetDelay(time - 0.33f).OnComplete(() => Destroy(arrow));
        }
    }

    protected Upgrade Data => GameData.Default.GetUpgrade(type);
}
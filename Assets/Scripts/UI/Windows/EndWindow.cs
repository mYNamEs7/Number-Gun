using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Dreamteck;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class EndWindow : Window
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI endCaption;

    [SerializeField] UIRewardIndicator rewardIndicator;
    [SerializeField] GameObject getButton, noThanksButton;
    [SerializeField] private Image gloveImg;
    [SerializeField] private TMP_Text gloveFillTxt;
    [SerializeField] private HandPopup handPopup;
    [SerializeField] private BlockUpgradePopup blockPopup;

    [SerializeField] private TMP_Text blockTxt;
    [SerializeField] private Image blockFiller;
    [SerializeField] private Image blockImg;
    [SerializeField] private Sprite[] blockIcons;
    int reward;

    [SerializeField] UIUpgrade[] upgrades;
    [SerializeField] GameObject nextButton;

    float tweenDelay;
    protected float TweenDelay { get { float curTweenDelay = tweenDelay; tweenDelay += 0.1f; return curTweenDelay; } }

    public override void Show()
    {
        reward = UIManager.Instance.curLvlReward;
        gameObject.SetActive(true);
        background.DOFade(0, 0.33f).SetDelay(TweenDelay).From();
        
        blockImg.sprite = blockIcons[LevelManager.CurrentLevel / 20];
        var fill = LevelManager.CurrentLevel % 20;
        if (fill == 0 && LevelManager.CurrentLevel / 20 != 0)
        {
            fill = 20;
            StartCoroutine(ShowBlockPopup());
        }
        blockTxt.text = $"{fill}/20";
        blockFiller.fillAmount = fill / 20f;
        
        if (GameData.FillHandId == 0)
            GameData.FillHandId = GameData.Default.hands.ToList()
                .FindIndex(x => !GameData.AvailableHandIds.Contains(GameData.Default.hands.ToList().IndexOf(x)));
        
        GameData.FillHandPercent += 25;
        if (GameData.FillHandPercent >= 100)
        {
            StartCoroutine(ShowHandPopup(GameData.FillHandId));
            
            GameData.FillHandId = GameData.Default.hands.ToList()
                .FindIndex(x => GameData.Default.hands.ToList().IndexOf(x) > GameData.FillHandId && !GameData.AvailableHandIds.Contains(GameData.Default.hands.ToList().IndexOf(x)));
            GameData.FillHandPercent = 0;
        }
        gloveImg.sprite = GameData.Default.handsUI[GameData.FillHandId];
        gloveImg.fillAmount = (float)GameData.FillHandPercent / 100;
        gloveFillTxt.text = $"{GameData.FillHandPercent}%";

        rewardIndicator.Play(reward);
        UIManager.ShowElement(blockFiller.transform.parent, TweenDelay);
        UIManager.ShowElement(gloveImg.transform.parent, TweenDelay);
        UIManager.ShowElement(rewardIndicator, TweenDelay);
        UIManager.ShowElement(getButton, TweenDelay);
        UIManager.ShowElement(noThanksButton, TweenDelay);
    }
    
    private IEnumerator ShowBlockPopup()
    {
        yield return new WaitForSeconds(TweenDelay + 0.2f);
        var popup = Instantiate(blockPopup, UIManager.Instance.popupCanvas);
        popup.blockIcon = blockIcons[LevelManager.CurrentLevel / 20 - 1];
        popup.Init();
    }

    private IEnumerator ShowHandPopup(int handId)
    {
        yield return new WaitForSeconds(TweenDelay + 0.2f);
        var popup = Instantiate(handPopup, UIManager.Instance.popupCanvas);
        popup.newHandId = handId;
        popup.Init();
    }

    public void ShowRewarded()
    {
        rewardIndicator.StopRewardOutput();
        GameManager.ShowRewardVideo(GetBonus);
    }

    void GetBonus()
    {
        UIManager.GetReward(getButton.transform.localPosition, reward * rewardIndicator.Bonus);
        NextPage();
    }

    public void NoThanks()
    {
        UIManager.GetReward(noThanksButton.transform.localPosition, reward);
        NextPage();
    }

    void NextPage()
    {
        tweenDelay = 0;

        UIManager.HideElement(blockFiller.transform.parent, TweenDelay);
        UIManager.HideElement(gloveImg.transform.parent, TweenDelay);
        UIManager.HideElement(rewardIndicator, TweenDelay);
        UIManager.HideElement(getButton, TweenDelay);
        UIManager.HideElement(noThanksButton, TweenDelay);

        UIManager.ShowElement(endCaption, TweenDelay);
        upgrades.ForEach(x => UIManager.ShowElement(x, TweenDelay));
        UIManager.ShowElement(nextButton, TweenDelay);
    }

    public override void Hide()
    {
        upgrades.ForEach(x => x.gameObject.SetActive(false));
        nextButton.SetActive(false);
        blockFiller.transform.parent.gameObject.SetActive(true);
        gloveImg.transform.parent.gameObject.SetActive(true);
        rewardIndicator.gameObject.SetActive(true);
        endCaption.gameObject.SetActive(false);
        getButton.SetActive(true);
        noThanksButton.SetActive(true);
        gameObject.SetActive(false);
        base.Hide();
    }
}

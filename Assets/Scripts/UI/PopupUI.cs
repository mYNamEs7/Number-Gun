using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupUI : MonoBehaviour
{
    [Header("Refs")] 
    [SerializeField] private TMP_Text titleTxt;
    [SerializeField] private Button rewardedBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TMP_Text closeBtnText;

    protected virtual void OnEnable() => GameManager.OnRestartEvent += Destroy;
    protected virtual void OnDisable() => GameManager.OnRestartEvent -= Destroy;

    public virtual void Init()
    {
        rewardedBtn.onClick.AddListener(() => GameManager.ShowRewardVideo(() =>
        {
            OnReward();
            
            titleTxt.text = GameData.Language == Language.EN ? "RECEIVED!" : "ПОЛУЧЕНО!";
            closeBtnText.text = GameData.Language == Language.EN ? "CLOSE" : "ЗАКРЫТЬ";
            Destroy(rewardedBtn.gameObject);
        }));
        closeBtn.onClick.AddListener(Close);
        
        var transform = this.transform;
        transform.DOScale(0, 0.33f).From();
    }

    protected abstract void OnReward();

    public void Close() => transform.DOScale(0, 0.33f).OnComplete(() => Destroy());

    protected void Destroy() => Destroy(gameObject);
}

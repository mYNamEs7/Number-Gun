using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class ChestWindow : Window
{
    [SerializeField] Transform frame;
    [SerializeField] float cardStartScale;
    [SerializeField] float spaceBetweenCards;
    [SerializeField] float cardMoveTime;
    [SerializeField] float cardMoveDelay;
    [SerializeField] Transform chestLockPoint;
    [SerializeField] Transform cardsSpawnPoint;
    [SerializeField] Transform cardsList;
    Image background;

    [SerializeField] Chest chest;
    [SerializeField] ChestData data;
    [SerializeField] GameObject confirmButton;

    List<ChestCard> cards, chooseCards;
    int maxCountCards = 1;

    void OnEnable()
    {
        chest.EndRollEvent += OpenChest;
        chest.OnOpenEvent += GetCards;
    }

    void OnDisable()
    {
        chest.EndRollEvent -= OpenChest;
        chest.OnOpenEvent -= GetCards;
    }

    void Awake()
    {
        background = GetComponent<Image>();
        cards = new();
        chooseCards = new();
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        frame.DOScale(0, 0.33f).From();
        background.DOFade(0, 0.33f).From();
        
        chest.CurState = Chest.State.Roll;
    }

    void OpenChest()
    {
        Pay();
        DOTween.Sequence().SetDelay(1.0f).OnComplete(() =>
        {
            chest.CurState = Chest.State.Open;
            SoundHolder.Default.PlayFromSoundPack("Open Chest");
        });
    }

    void Pay()
    {
        Vector2 pos = UIMoney.Instance.KeysCounterPos;
        for (int i = 0; i < 10; i++)
        {
            int n = i;
            DOTween.Sequence().SetDelay(n * 0.05f).OnComplete(() =>
            {
                GameData.Default.TryPayKeys(1);
                GameObject key = Instantiate(GameData.Default.keyUI, UIManager.Instance.canvas);
                Transform keyTransform = key.transform;
                keyTransform.DOScale(0, 0.15f).From();
                keyTransform.localPosition = pos;
                keyTransform.DOLocalMove(pos + UnityEngine.Random.insideUnitCircle * 100, 0.15f).OnComplete(() =>
                keyTransform.DOLocalMove(chestLockPoint.localPosition, 0.25f).SetDelay(0.3f + (9 - n) * 0.05f).OnComplete(() => Destroy(key))
                );
            });
        }
    }

    void GetCards()
    {
        int curPack = YandexGame.savesData.CurChestCardPack;
        YandexGame.savesData.CurChestCardPack = (YandexGame.savesData.CurChestCardPack + 1) % data.packs.Length;
        YandexGame.SaveProgress();

        for (int i = 0; i < data.packs[curPack].cards.Length; i++)
        {
            int index = i;
            ChestCard card = Instantiate(data.packs[curPack].cards[index], cardsList);
            card.OnClickEvent += ClickOnCard;
            Transform cardTransform = card.transform;
            cardTransform.SetAsFirstSibling();
            cardTransform.localPosition = cardsSpawnPoint.localPosition;
            cardTransform.DOScale(Vector3.one * cardStartScale, cardMoveTime).From().SetDelay(cardMoveDelay * index);
            cardTransform.DOLocalMove(Vector3.right * ((index - 1) * spaceBetweenCards), cardMoveTime).SetDelay(cardMoveDelay * index).OnComplete(() => card.Enable());
            cards.Add(card);
        }
        DOTween.Sequence().SetDelay(cardMoveTime + 2 * cardMoveDelay).OnComplete(() => chest.CurState = Chest.State.Reduce);
    }

    void ClickOnCard(ChestCard card)
    {
        if (chooseCards.Contains(card)) return;
        if (chooseCards.Count >= maxCountCards)
        {
            chooseCards[0].Unchoose();
            chooseCards.RemoveAt(0);
        }
        chooseCards.Add(card);
        card.Choose();

        if (chooseCards.Count >= maxCountCards && !confirmButton.activeInHierarchy) UIManager.ShowElement(confirmButton);
    }

    public void Confirm()
    {
        chooseCards.ForEach(x => x.Get());
        UIManager.HideElement(confirmButton);
        Hide();
    }

    public override void Hide()
    {
        Vector3 startScale = frame.localScale;
        Color startColor = background.color;
        background.DOFade(0, 0.33f).OnComplete(() => background.color = startColor);
        frame.DOScale(0, 0.33f).OnComplete(() =>
        {
            chest.CurState = Chest.State.Idle;
            frame.localScale = startScale;
            gameObject.SetActive(false);
            cards.ForEach(x => Destroy(x.gameObject));
            cards.Clear();
            chooseCards.Clear();
        });

        base.Hide();
    }
}
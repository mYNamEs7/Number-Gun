using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UpgradeZone : MonoBehaviour
{
    public readonly static Vector3 CardSupineRotation = new Vector3(90, 0, 0);

    [NonSerialized] public Transform thisTransform;

    [SerializeField] Vector3 playerPoint;
    public Vector3 PlayerPoint => thisTransform.TransformPoint(playerPoint);
    
    [SerializeField] Vector3 secondGunPoint;
    public Vector3 SecondGunPoint => thisTransform.TransformPoint(secondGunPoint);
    [SerializeField] Vector3 cameraPoint;
    public Vector3 CameraPoint => thisTransform.TransformPoint(cameraPoint);
    [SerializeField] Vector3 cardsPoint;
    public Vector3 CardsPoint => thisTransform.TransformPoint(cardsPoint);

    [Header("Cards")]
    [SerializeField] float cardJumpPower;
    [SerializeField] float cardJumpDuration;
    [SerializeField] float cardJumpDelay;
    [SerializeField] Vector2 cardsSpace;
    [NonSerialized] public List<Card> cards;

    [SerializeField] GameObject gates;
    [SerializeField] GameObject grid;

    void Start()
    {
        thisTransform = transform;
    }

    private void OnEnable()
    {
        Level.OnEndUpgrade += LevelOnOnEndUpgrade;
    }

    private void OnDisable()
    {
        Level.OnEndUpgrade -= LevelOnOnEndUpgrade;
    }

    private void LevelOnOnEndUpgrade()
    {
        grid.transform.DOScale(0, 0.33f).SetEase(Ease.InBack);
    }

    public void Get()
    {
        // gates.SetActive(false);
        var gridTransform = grid.transform;
        var targetScale = gridTransform.localScale;
        gridTransform.localScale = Vector3.zero;
        grid.SetActive(true);
        gridTransform.DOScale(targetScale, 0.8f).SetEase(Ease.OutBack);
        
        gates.transform.DOScale(0, 0.8f).SetEase(Ease.InBack);

        cards = new(Level.Instance.conveyorBelt.GetCards());
        cards.Sort(delegate(Card x, Card y)
        {
            if (x is SuperCard && !(y is SuperCard)) return -1;
            return 0;
        });
        cards.Reverse();
        Vector3 cardsPoint = CardsPoint;
        int maxRow = Mathf.Max(cards.Count - 1, 0) / 3;
        for (int i = 0; i < cards.Count; i++)
        {
            int index = i;
            Card card = cards[index];
            int row = index / 3;
            int maxColumn = Mathf.Min(cards.Count - row * 3, 3) - 1;
            Vector3 offset = new Vector3(-cardsSpace.x * maxColumn * 0.5f + cardsSpace.x * (index % 3), 0, cardsSpace.y * maxRow * 0.25f - row * cardsSpace.y + (card.scale.y - 1) / 2);

            card.thisTransform.position = cardsPoint + offset;
            card.thisTransform.eulerAngles = CardSupineRotation;
            card.thisTransform.DOScale(1, 0.7f).SetEase(Ease.InOutBack).SetDelay(1.5f + index * cardJumpDelay)
                .OnStart(() => card.HideText())
                .OnComplete(() => card.EnableUsage());

            // card.thisTransform.DOJump(cardsPoint + offset, cardJumpPower, 1, cardJumpDuration)
            // .SetDelay(0.33f + index * cardJumpDelay)
            // .Join(card.thisTransform.DORotate(CardSupineRotation, cardJumpDuration))
            // .OnComplete(() => card.EnableUsage());
        }
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        if (cards.Count == 0)
        {
            // Level.Instance.Stage = LevelStage.Second;
        }
        else if (cards.Count == 1 && cards[0] is SuperCard && !(cards[0] as SuperCard).unlock)
        {
            card = cards[0];
            card.thisTransform.DOScale(0, 0.33f);
            card.thisTransform.DOLocalRotate(CardSupineRotation + Vector3.up * 360, 0.33f, RotateMode.FastBeyond360)/*.OnComplete(() => Destroy(card.gameObject))*/;
            cards.RemoveAt(0);
            // Level.Instance.Stage = LevelStage.Second;
        }
    }
}

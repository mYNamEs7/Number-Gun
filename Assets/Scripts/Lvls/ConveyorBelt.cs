using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    readonly static Vector3 CardSupineRotation = new Vector3(-90, 180, 0);
    
    [SerializeField] float speed = 1;
    [SerializeField] float cardJumpPower;
    [SerializeField] float cardJumpTime;
    [SerializeField] Material material;

    List<Card> cards;
    Dictionary<Card, Tween> cardsTween;

    Transform thisTransform;

    void Start()
    {
        thisTransform = transform;
        cards = new();
        cardsTween = new();
    }

    private void OnEnable()
    {
        Level.OnStartUpgrade += LevelOnOnStartUpgrade;
    }

    private void OnDisable()
    {
        Level.OnStartUpgrade -= LevelOnOnStartUpgrade;
    }

    private void LevelOnOnStartUpgrade()
    {
        thisTransform.DOMoveZ(40f, 1f).SetEase(Ease.InBack, 5);
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
        cardsTween[card] = card.thisTransform.DOJump(new Vector3(thisTransform.position.x, thisTransform.position.y + 0.2f, card.thisTransform.position.z), cardJumpPower, 1, cardJumpTime)
        .Join(card.thisTransform.DORotate(CardSupineRotation, cardJumpTime))
        .OnComplete(() =>
        {
            cardsTween[card] = card.thisTransform
                .DOMoveZ(thisTransform.position.z + thisTransform.lossyScale.y * 0.5f - 6, speed)
                .SetEase(Ease.Linear).SetSpeedBased(true)
                .OnComplete(() =>
                    card.thisTransform.DOJump(
                        new Vector3(card.thisTransform.position.x, thisTransform.position.y + 1.45f,
                            thisTransform.position.z + thisTransform.lossyScale.y * 0.5f - 2), cardJumpPower, 1,
                        cardJumpTime).OnComplete(() => card.thisTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutBack)));
        });
    }

    public List<Card> GetCards()
    {
        cards.ForEach(x => { if (cardsTween.TryGetValue(x, out Tween tween)) tween.Kill(); x.thisTransform.localScale = Vector3.zero; });
        cardsTween.Clear();
        return cards;
    }

    void Update()
    {
        material.SetTextureOffset("_BaseMap", new Vector2(0, -Time.time * speed * 0.2f));
    }
}

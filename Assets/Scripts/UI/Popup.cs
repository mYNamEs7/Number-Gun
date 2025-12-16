using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Popup : MonoBehaviour
{
    [SerializeField] Vector2 time;
    [SerializeField] Vector2 yRange;
    [SerializeField] Vector2 xRange;
    [SerializeField] TMP_Text txt;

    [NonSerialized] public bool isYearsPopup;

    void OnEnable() => GameManager.OnRestartEvent += Destroy;
    void OnDisable() => GameManager.OnRestartEvent -= Destroy;

    public void Init(string text, Color color = default, bool isYearsPopup = false)
    {
        this.isYearsPopup = isYearsPopup;
        
        txt.SetText(text);
        Transform transform = this.transform;
        transform.DOScale(0, 0.33f).From();
        transform.DOLocalMove(transform.localPosition + Vector3.up * Random.Range(yRange.x, yRange.y) + Vector3.right * Random.Range(xRange.x, xRange.y), Random.Range(time.x, time.y))
        .OnComplete(() => txt.DOFade(0, 0.33f).OnComplete(Destroy));

        if (color != default) txt.color = color;;
    }

    void Destroy() => Destroy(gameObject);
}

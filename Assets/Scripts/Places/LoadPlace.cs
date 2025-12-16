using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadPlace : MonoBehaviour
{
    [SerializeField] Component[] triggers;

    [SerializeField] Transform bar;
    [SerializeField] protected SpriteRadialFill fillSprite;

    [SerializeField] float delay;
    Tween delayTween;
    [SerializeField] protected float time;
    protected Coroutine coroutine;
    Tween barHeightTween, barScaleTween;
    float barStartHeight;
    Vector3 barStartScale;

    void Start()
    {
        fillSprite.Init();
        if (bar)
        {
            barStartHeight = bar.localPosition.z;
            barStartScale = bar.localScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            delayTween = DOTween.Sequence().SetDelay(delay).OnComplete(() => StartLoad());
            
            if (bar)
            {
                barHeightTween.Kill();
                barScaleTween.Kill();
                barHeightTween = bar.DOLocalMoveZ(barStartHeight - 1.5f, 0.33f);
                barScaleTween = bar.DOScale(barStartScale * 1.25f, 0.33f);
            }
        }
    }

    protected virtual void StartLoad()
    {
        coroutine = StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        float t = 0;
        while (t < time)
        {
            yield return null;
            t += Time.deltaTime;
            LoadUpdate(t / time);
        }

        for (int i = 0; i < triggers.Length; i++) (triggers[i] as ILoadPlace)?.Load();
    }

    protected void LoadUpdate(float t)
    {
        fillSprite.Fill(t);
        for (int i = 0; i < triggers.Length; i++) (triggers[i] as ILoadPlace)?.LoadUpdate(t);
    }

    protected virtual void EndLoad()
    {
        fillSprite.Fill(0);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            delayTween.Kill();
            if (coroutine != null) StopCoroutine(coroutine);
            EndLoad();

            if (bar)
            {
                barHeightTween.Kill();
                barScaleTween.Kill();
                barHeightTween = bar.DOLocalMoveZ(barStartHeight, 0.33f);
                barScaleTween = bar.DOScale(barStartScale, 0.33f);
            }
        }
    }
}

public interface ILoadPlace
{
    void Load() { }

    void LoadUpdate(float t) { }
}
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITransition : MonoBehaviour
{
    [SerializeField] float duration = 0.33f;
    [SerializeField] Ease ease = Ease.OutQuad;

    public static event Action CloseEvent;
    public static event Action OpenEvent;

    public void Close(Action action)
    {
        Material material = GetComponent<Image>().material;
        material.SetFloat("_Fill", 0);
        material.DOFloat(1, "_Fill", duration).SetEase(ease).OnComplete(() =>
        {
            CloseEvent?.Invoke();
            action?.Invoke();
        });
    }

    public void Open(Action action)
    {
        Material material = GetComponent<Image>().material;
        material.SetFloat("_Fill", 1);
        material.DOFloat(0, "_Fill", duration).SetEase(ease).OnComplete(() =>
        {
            OpenEvent?.Invoke();
            action?.Invoke();
        });
    }
}

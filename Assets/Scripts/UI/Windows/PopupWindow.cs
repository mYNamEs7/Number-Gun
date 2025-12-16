using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : Window
{
    [SerializeField] Transform frame;
    [SerializeField] GameObject claimButton;
    Image background;

    void Awake()
    {
        background = GetComponent<Image>();
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        frame.DOScale(0, 0.33f).From();
        background.DOFade(0, 0.33f).From();
        UIManager.ShowElement(claimButton, 0.33f);
    }

    public override void Hide()
    {
        UIManager.HideElement(claimButton);
        Vector3 startScale = frame.localScale;
        Color startColor = background.color;
        background.DOFade(0, 0.33f).OnComplete(() => background.color = startColor);
        frame.DOScale(0, 0.33f).OnComplete(() =>
        {
            frame.localScale = startScale;
            gameObject.SetActive(false);
        });

        base.Hide();
    }
}
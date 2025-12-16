using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameWindow : Window
{
    public Button restartButton;
    public Button doubleGunBtn;
    public static event Action OnRestartEvent;

    void OnEnable() => GameManager.OnRestartEvent += Hide;
    void OnDisable() => GameManager.OnRestartEvent -= Hide;

    public override void Show()
    {
        gameObject.SetActive(true);
        restartButton.transform.localScale = Vector3.one * 1.5f;
        UIManager.ShowElement(restartButton, 1.0f);
        if (PlayerController.Instance.Weapons.Count == 1 && LevelManager.CompleteLevelCount > 2)
            UIManager.ShowElement(doubleGunBtn, 1.0f);
    }

    public override void Hide()
    {
        base.Hide();
        UIManager.HideElement(restartButton, 0, () => gameObject.SetActive(false));
        UIManager.HideElement(doubleGunBtn, 0, () => gameObject.SetActive(false));
    }

    public void DoubleGun()
    {
        if (PlayerController.Instance.Weapons.Count == 1)
            GameManager.ShowRewardVideo(() => PlayerController.Instance.Upgrade(UpgradeType.GunAmount, 1));
    }

    public static void Restart() => OnRestartEvent?.Invoke();
}
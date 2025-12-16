using UnityEngine;

public class StartWindow : Window
{
    [SerializeField] GameObject[] objects;

    public override void Show()
    {
        for (int i = 0; i < objects.Length; i++) UIManager.ShowElement(objects[i], i * 0.1f);
    }

    public override void Hide()
    {
        base.Hide();
        for (int i = 0; i < objects.Length; i++) UIManager.HideElement(objects[i], i * 0.1f);
    }

    public void AddYearsReward() => print("недоступно");
}
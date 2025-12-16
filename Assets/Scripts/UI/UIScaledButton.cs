using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScaledButton : Button
{
    bool click;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!click)
        {
            transform.localScale -= Vector3.one * 0.1f;
            click = true;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (click)
        {
            transform.localScale += Vector3.one * 0.1f;
            click = false;
        }
    }
}

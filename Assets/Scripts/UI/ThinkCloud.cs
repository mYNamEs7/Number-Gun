using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThinkCloud : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TextMeshPro text;

    public void Init(Sprite sprite, string txt)
    {
        spriteRenderer.sprite = sprite;
        text.text = txt;
    }
}

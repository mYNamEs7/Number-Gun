using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class TMProDithering : MonoBehaviour
{
    [SerializeField] float startDistance = 10;
    [SerializeField] float range = 2;
    TextMeshPro text;
    float startAlpha;

    Transform thisTransform;
    Transform camTransform;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        startAlpha = text.color.a;

        thisTransform = transform;
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(startAlpha, 0, (startDistance - Vector3.Distance(thisTransform.position, camTransform.position)) / range));
    }
}
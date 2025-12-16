using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererDithering : MonoBehaviour
{
    [SerializeField] float startDistance = 5;
    [SerializeField] float range = 1;
    SpriteRenderer spriteRenderer;
    float startAlpha;

    Transform thisTransform;
    Transform camTransform;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startAlpha = spriteRenderer.color.a;

        thisTransform = transform;
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(startAlpha, 0, (startDistance - Vector3.Distance(thisTransform.position, camTransform.position)) / range));
    }
}
using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Billboard))]
public class Emote : MonoBehaviour
{

    [SerializeField] float time = -1;
    [SerializeField] float showTime = 0.2f;
    [SerializeField] AnimationCurve scaleCurve;
    [SerializeField] bool autoStart;

    Transform thisTransform, camTransform;
    Transform parent;
    Vector3 offset;
    [SerializeField] float thisRadius;
    float parentRadius;

    Coroutine coroutine;

    void Awake()
    {
        if (autoStart) Init();
    }

    public void Init(Transform parent = null, Vector3 offset = new Vector3(), float radius = 0)
    {
        thisTransform = transform;
        camTransform = Camera.main.transform;
        if (parent) this.parent = parent;
        this.offset = offset;
        parentRadius = radius;

        coroutine = StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateCoroutine()
    {
        thisTransform.DOScale(0, showTime).From().SetEase(scaleCurve);
        float r = thisRadius * 1.33f + parentRadius;

        float t = 0;
        while (t < time || time < 0)
        {
            t = Mathf.Min(t + Time.deltaTime, float.MaxValue);

            float flyAnimTime = Time.time * 0.5f;
            float flyAnimR = Mathf.Sin(flyAnimTime * 3) * 0.1f;
            Vector3 flyAnimOffset = thisTransform.up * (Mathf.Cos(flyAnimTime) * flyAnimR) + thisTransform.right * (Mathf.Sin(flyAnimTime) * flyAnimR);
            thisTransform.position = parent.TransformPoint(offset) + (parent.up + camTransform.right) * r + flyAnimOffset;
            yield return null;
        }

        End();
    }

    public void EndShow()
    {
        StopCoroutine(coroutine);
        End();
    }

    void End()
    {
        thisTransform.DOScale(0, showTime).OnComplete(() => Destroy(gameObject));
    }
}

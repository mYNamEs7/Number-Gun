using System.Collections;
using UnityEngine;

public class Key : Item
{
    [SerializeField] float collectTime;
    [SerializeField] AnimationCurve collectCurve;
    bool collect;

    public override void WeaponHit(Collider collider)
    {
        if (collect) return;
        collect = true;

        base.WeaponHit(collider);
        StartCoroutine(Get());
    }

    IEnumerator Get()
    {
        Vector3 startPos = thisTransform.position;
        Camera cam = Camera.main;
        Vector3 keysCounterPos = UIMoney.Instance.KeysCounterPos;
        float startScale = thisTransform.localScale.x;

        float t = 0;
        while (t < collectTime)
        {
            yield return null;
            t += Time.deltaTime;
            float tCurve = collectCurve.Evaluate(t / collectTime);

            Vector3 endPos = cam.ScreenToWorldPoint(keysCounterPos + Vector3.forward);
            Vector3 wayPoint = new Vector3(startPos.x, endPos.y, Mathf.Lerp(startPos.z, endPos.z, 0.5f));
            thisTransform.position = Mathf.Pow(1 - tCurve, 2) * startPos + 2 * (1 - tCurve) * tCurve * wayPoint + Mathf.Pow(tCurve, 2) * endPos;
            thisTransform.localScale = Vector3.one * Mathf.Lerp(startScale, startScale * 0.066f, tCurve);
        }

        GameData.Default.AddKeys(1);
        Destroy(gameObject);
    }
}

using DG.Tweening;
using UnityEngine;

public class JumpRamp : MonoBehaviour, IWeaponTarget
{
    [SerializeField] float height;
    [SerializeField] float time;
    bool get;

    [SerializeField] SpriteRenderer[] arrows;
    [SerializeField] Color[] colors;
    float t;

    public void WeaponHit(Collider collider)
    {
        if (get) return;
        get = true;

        PlayerController.Instance.thisTransform.DOLocalMoveY(transform.position.y + height, time).SetEase(Ease.OutBack);
    }

    void Update()
    {
        t = (t + Time.deltaTime * 12) % 5;
        for (int i = 0; i < arrows.Length; i++) arrows[i].color = colors[(int)t == i ? 2 : ((int)t == ((i + 1) % 5) ? 1 : 0)];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 point = transform.position + Vector3.up * height + Vector3.forward * 5;
        Gizmos.color = new Color(1, 0.2f, 1, 0.3f);
        Gizmos.DrawSphere(point + Vector3.up * 0.5f + Vector3.forward, 0.5f);
        Gizmos.DrawCube(point + Vector3.up * 0.5f, Vector3.one);
        Gizmos.DrawWireSphere(point + Vector3.up * 0.5f + Vector3.forward, 0.5f);
        Gizmos.DrawWireCube(point + Vector3.up * 0.5f, Vector3.one);
        Gizmos.DrawLine(transform.position, point + Vector3.up * 0.5f);
    }
#endif
}

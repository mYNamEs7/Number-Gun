using UnityEngine;
using DG.Tweening;

public class FallGate : MonoBehaviour, IWeaponTarget
{
    [SerializeField] float time = 1.5f;

    public void WeaponHit(Collider collider)
    {       
        PlayerController.Instance.thisTransform.DOLocalMoveY(0, time).SetEase(Ease.InBack);
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position + Vector3.up * 1.5f;
        Gizmos.color = new Color(0.1f, 1, 0.1f, 0.2f);
        Gizmos.DrawCube(pos, new Vector3(12, 3, 0));
        Gizmos.color = new Color(0.1f, 1, 0.1f, 0.5f);
        Gizmos.DrawWireCube(pos, new Vector3(12, 3, 0));
    }
#endif
}

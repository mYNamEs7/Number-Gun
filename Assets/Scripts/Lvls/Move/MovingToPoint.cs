using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MovingToPoint : MonoBehaviour, IMovingObject
{
    [SerializeField] Vector3 point;
    [SerializeField] float speed;
    [SerializeField] float startMoveOffset;
    [SerializeField] UnityEvent endMoveEvent;
    Tween moveTween;
    bool move;

    Transform thisTransform;

    void Start()
    {
        thisTransform = transform;
    }

    void Move()
    {
        move = true;
        moveTween = thisTransform.DOMove(point, speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() => endMoveEvent?.Invoke());
    }

    void Update()
    {
        if (move) return;
        if (thisTransform.position.z - startMoveOffset < PlayerController.Instance.thisTransform.position.z) Move();
    }

    public void Destroy()
    {
        moveTween.Kill();
        Destroy(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point, 0.5f);

        Vector3 pos = Vector3.forward * (transform.position.z - startMoveOffset) + Vector3.up * 1.5f;
        Gizmos.color = new Color(1, 0.1f, 0.1f, 0.2f);
        Gizmos.DrawCube(pos, new Vector3(12, pos.y * 2, 0));
        Gizmos.color = new Color(1, 0.1f, 0.1f, 0.5f);
        Gizmos.DrawWireCube(pos, new Vector3(12, pos.y * 2, 0));
    }
#endif
}

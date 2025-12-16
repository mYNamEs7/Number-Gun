using UnityEngine;

public class HorizontalMoving : MonoBehaviour, IMovingObject
{
    [SerializeField] Vector2 clamp;
    [SerializeField] float speed;
    [SerializeField] bool autoStart = true;
    bool move;
    float startMoveTime;

    Transform thisTransform;

    void Start()
    {
        thisTransform = transform;
        if (autoStart) StartMove();
    }

    public void StartMove()
    {
        move = true;
        startMoveTime = Time.time;
    }

    void Update()
    {
        if (!move) return;
        thisTransform.localPosition = new Vector3(Mathf.Lerp(clamp.x, clamp.y, Mathf.Abs(-1 + (Time.time - startMoveTime) * speed % 2)), thisTransform.localPosition.y, thisTransform.localPosition.z);
    }

    public void Destroy() => Destroy(this);
}

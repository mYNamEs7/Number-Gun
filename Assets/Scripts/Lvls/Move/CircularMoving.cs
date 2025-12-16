using System.Collections.Generic;
using UnityEngine;

public class CircularMoving : MonoBehaviour, IMovingObject
{
    [SerializeField] float radius = 3;
    [SerializeField] float startAngle;
    [SerializeField] float speed = 1;
    [SerializeField] bool autoStart = true;
    bool move;
    float startMoveTime;
    Vector3 startPoint;

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
        startPoint = thisTransform.localPosition;
    }

    void Update()
    {
        if (!move) return;
        float t = (Time.time - startMoveTime) * speed;
        thisTransform.localPosition = new Vector3(startPoint.x + Mathf.Sin(startAngle * Mathf.Deg2Rad + t) * radius, thisTransform.localPosition.y, startPoint.z + Mathf.Cos(startAngle * Mathf.Deg2Rad + t) * radius);
    }

    public void Destroy() => Destroy(this);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 point = Application.isPlaying ? startPoint : transform.position;
        Gizmos.color = Color.red;
        Vector3[] points = new Vector3[24];
        for (int i = 0; i < 24; i++) points[i] = point + Quaternion.Euler(0, i * 15, 0) * Vector3.forward * radius;
        Gizmos.DrawLineList(points);
        Gizmos.DrawSphere(point + Quaternion.Euler(0, startAngle, 0) * Vector3.forward * radius, 0.2f);
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Line : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    [SerializeField] private bool update;

    [Range(4, 200), SerializeField] private int smooth;
    [Range(0, 100), SerializeField] private float pointsProximity = 1;

    private float length;
    private LineRenderer lineRenderer;
    private Transform thisTransform;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] Color debugSpheresColor;
    [SerializeField] float debugSpheresScale;
    [SerializeField] bool debug;
#endif

    void Start()
    {
#if UNITY_EDITOR
        if (points == null || points.Length != 4) return;
#endif
        if (update) Init();
    }

    public void Init()
    {
        thisTransform = transform;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (update)
        {
#if UNITY_EDITOR
            if (points == null || points.Length != 4) return;
            if (thisTransform == null) Init();
#endif
            UpdateLine(points[0].position, points[1].position, points[2].position, points[3].position);
        }
    }

    public void SetVisible(bool visible)
    {
        lineRenderer.enabled = visible;
    }

    public void UpdateLine(Vector3 firstPoint, Vector3 secondPoint, Vector3 thirdPoint, Vector3 fourthPoint)
    {
        int vertexCount = smooth;
        Vector3[] positions = new Vector3[vertexCount];
        for (int i = 0; i < vertexCount; i++)
            positions[i] = Quaternion.Inverse(thisTransform.rotation) * (Curve(firstPoint, secondPoint, thirdPoint, fourthPoint, (float)i / (vertexCount - 1)) - thisTransform.position);

        lineRenderer.positionCount = vertexCount;
        lineRenderer.SetPositions(positions);
    }

    Vector3 Curve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float P0 = Mathf.Pow(1 - t, 3);
        float P1 = 3 * Mathf.Pow(1 - t, 2) * t * pointsProximity;
        float P2 = 3 * (1 - t) * Mathf.Pow(t, 2) * pointsProximity;
        float P3 = Mathf.Pow(t, 3);
        return (P0 * p0 + P1 * p1 + P2 * p2 + P3 * p3) / (P0 + P1 + P2 + P3);
    }

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.InSelectionHierarchy | GizmoType.Pickable)]
    private void OnDrawGizmos()
    {
        if (debug)
            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.color = debugSpheresColor;
                Gizmos.DrawWireSphere(points[i].position, debugSpheresScale);
                Gizmos.color = Gizmos.color * new Vector4(1, 1, 1, 0.3f);
                Gizmos.DrawSphere(points[i].position, debugSpheresScale);
            }
    }
#endif
}
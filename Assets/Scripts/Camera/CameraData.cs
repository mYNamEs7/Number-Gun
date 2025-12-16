using UnityEngine;

[CreateAssetMenu(menuName = "Data/Camera Data")]
public class CameraData : ScriptableObject
{
    public LayerMask camCollisionLayerMask;

    [Header("Move")]
    public float smoothSpeed = 0.015f;
    public float smoothAngle = 0.015f;

    [Header("In Game")]
    public Vector3 inGameOffset;
    public Vector3 inGameAngle;
}
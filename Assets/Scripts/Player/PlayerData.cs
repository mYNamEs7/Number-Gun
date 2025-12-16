using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Controll")]
    public float horizontalSensetive = 1;
    public float horizontalSensetivePow = 1.1f;
    public float horizontalSensetiveRatio = 0.5f;

    [Header("Move")]
    public float forwardSpeed = 2;
    public float horizontalSpeed = 6;
    public Vector2 horizontalClamp;
}

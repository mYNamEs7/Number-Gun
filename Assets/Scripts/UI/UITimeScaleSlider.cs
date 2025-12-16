using UnityEngine;

public class UITimeScaleSlider : MonoBehaviour
{
    [SerializeField] float maxTimeScale;

    public void SetTimeScale(float t) => Time.timeScale = 1 + t * maxTimeScale;
}

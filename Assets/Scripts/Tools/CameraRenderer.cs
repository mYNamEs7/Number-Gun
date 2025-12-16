using UnityEngine;

[ExecuteInEditMode]
public class CameraRenderer : MonoBehaviour
{
    [SerializeField] int frameCount = 2;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Time.frameCount % frameCount == 0) return;
        RenderTexture.active = cam.targetTexture;
        GL.Clear(true, true, Color.clear);
        cam.Render();
    }
}

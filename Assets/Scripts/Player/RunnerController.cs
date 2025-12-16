using System;
using MyInputManager;
using UnityEngine;

public class RunnerController : MonoBehaviour
{
    #region Singletone
    private static RunnerController _instance;
    public static RunnerController Instance { get => _instance; }
    public RunnerController() => _instance = this;
    #endregion

    float oldXPos;
    bool down;
    bool touchLock;

    [Header("Tutorial")]
    [SerializeField] GameObject tutorial;
    bool _tutorial;

    public static event Action<float> OnControllEvent;

    void OnEnable()
    {
        InputManager.DownEvent += TouchDown;
        InputManager.UpEvent += TouchUp;
    }

    void OnDisable()
    {
        InputManager.DownEvent -= TouchDown;
        InputManager.UpEvent -= TouchUp;
    }

    void TouchDown()
    {
        if (touchLock) return;

        down = true;
        oldXPos = (Input.mousePosition.x - Screen.width / 2f) * (1920f / Screen.height);
        if (!_tutorial)
        {
            Destroy(tutorial);
            _tutorial = true;
        }
    }

    void TouchUp()
    {
        if (touchLock) return;
        down = false;
    }

    public void LockControll()
    {
        TouchUp();
        touchLock = true;
    }

    public void UnLockControll() => touchLock = false;

    void Update() => OnControllEvent?.Invoke(Controll());

    public float Controll()
    {
        if (down)
        {
            float mousePos = (Input.mousePosition.x - Screen.width / 2f) * (1920f / Screen.height);

            if (Input.GetMouseButtonDown(0)) oldXPos = mousePos;
            else if (Input.GetMouseButtonUp(0)) TouchUp();
            if (Input.GetMouseButton(0) && Input.touchCount < 2)
            {
                float diff = mousePos - oldXPos;
                oldXPos = mousePos;
                return diff;
            }
        }
        return 0;
    }
}

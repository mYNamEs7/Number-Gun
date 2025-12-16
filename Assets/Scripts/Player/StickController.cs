using System;
using MyInputManager;
using UnityEngine;

public class StickController : MonoBehaviour
{
    #region Singletone
    private static StickController _instance;
    public static StickController Instance { get => _instance; }
    public StickController() => _instance = this;
    #endregion

    [Header("Stick")]
    [SerializeField] GameObject stickObj;
    [SerializeField] Transform stick;
    [SerializeField] float stickBorder = 100;
    [SerializeField] AnimationCurve stickFollowCurve;
    Vector2 startStickPos;
    bool down;
    bool touchLock;

    [Header("Tutorial")]
    [SerializeField] GameObject tutorial;
    bool _tutorial;

    Transform stickTransform;

    public event Action<Vector2> OnControllEvent;

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

    void Awake()
    {
        stickTransform = stickObj.transform;
    }

    void TouchDown()
    {
        if (touchLock) return;

        down = true;
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
        stickObj.SetActive(false);
    }

    public void LockControll()
    {
        TouchUp();
        touchLock = true;
    }

    public void UnLockControll() => touchLock = false;

    void Update() => OnControllEvent?.Invoke(Controll());

    public Vector3 Controll()
    {
        if (down)
        {
            Vector2 mousePos = ((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2f) * (1920f / Screen.height);

            if (Input.GetMouseButtonDown(0))
            {
                stickTransform.localPosition = mousePos;
                startStickPos = mousePos;
            }
            else if (Input.GetMouseButtonUp(0)) TouchUp();

            if (Input.GetMouseButton(0) && Input.touchCount < 2)
            {
                stickObj.SetActive(true);

                Vector2 curStickPos = mousePos - startStickPos;
                float magnitude = curStickPos.magnitude;
                float diff = curStickPos.magnitude - stickBorder;
                if (diff > 0)
                {
                    if (magnitude >= stickBorder)
                    {
                        Vector2 normalized = curStickPos / magnitude;
                        startStickPos += new Vector2(diff * normalized.x, diff * normalized.y);
                        stickTransform.localPosition = startStickPos;
                    }
                    curStickPos = Vector3.ClampMagnitude(mousePos - startStickPos, stickBorder);
                }
                stick.localPosition = curStickPos;
                //Stick Controll

                curStickPos = curStickPos.normalized * (Mathf.Min(magnitude, stickBorder) / stickBorder);
                return new Vector3(curStickPos.x, 0, curStickPos.y);
            }
            else stickObj.SetActive(false);
        }
        return Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UI.Windows;

public class CameraController : MonoBehaviour
{
    #region Singletone
    private static CameraController _instance;
    public static CameraController Instance { get => _instance; }
    public CameraController() => _instance = this;
    #endregion

    [SerializeField] CameraData data;
    [SerializeField] private Vector3 positionOnFinish;
    [SerializeField] private Vector3 angleOnFinish;

    public static Vector3 MainOffset;
    public static Vector3 MainAngle;

    public class FollowTarget
    {
        public Transform transform;
        public Vector3 offset;
        public Quaternion angle;
        public float speed;
        public int priority;
        public int hash;

        public FollowTarget(Transform transform, Vector3 offset, Quaternion angle, float speed, int priority, int hash)
        {
            this.transform = transform;
            this.offset = offset;
            this.angle = angle;
            this.speed = speed;
            this.priority = priority;
            this.hash = hash;
        }
    }
    [NonSerialized] public List<FollowTarget> followTargets;
    float curFollowSpeed;

    Camera cam;
    [NonSerialized] public Transform thisTransform, camTransform;
    Vector3 startCamLocalPos;
    float startFOV, addFOV;
    bool lockUpdate;
    Tween FOVTween;

    Coroutine returnCamCoroutine;
    private bool isSecondLoop;

    void OnEnable()
    {
        GameManager.OnStartMenu += StartMenu;
        GameManager.OnStartGame += StartGame;
        GameManager.OnRestartEvent += EndGame;
        Level.OnStartUpgrade += StartUpgrade;
        Level.OnEndUpgrade += EndUpgrade;
        FinishZone.OnEnter += FinishZoneOnOnEnter;
        
        BaseUpgradeWindow.OnStartUpgrade += BaseUpgradeWindowOnOnStartUpgrade;
        BaseUpgradeWindow.OnEndUpgrade += BaseUpgradeWindowOnOnEndUpgrade;
    }

    void OnDisable()
    {
        GameManager.OnStartMenu -= StartMenu;
        GameManager.OnStartGame -= StartGame;
        GameManager.OnRestartEvent -= EndGame;
        Level.OnStartUpgrade -= StartUpgrade;
        Level.OnEndUpgrade -= EndUpgrade;
        FinishZone.OnEnter -= FinishZoneOnOnEnter;
        
        BaseUpgradeWindow.OnStartUpgrade -= BaseUpgradeWindowOnOnStartUpgrade;
        BaseUpgradeWindow.OnEndUpgrade -= BaseUpgradeWindowOnOnEndUpgrade;
    }

    private void FinishZoneOnOnEnter()
    {
        SetFollowTarget(PlayerController.Instance.thisTransform, true, 2, 0, 0, positionOnFinish, angleOnFinish);
    }

    public void Init(CameraData data = null)
    {
        if (data != null) this.data = data;

        thisTransform = transform;
        cam = Camera.main;
        camTransform = cam.transform;
        startCamLocalPos = camTransform.localPosition;

        followTargets = new();

        startFOV = cam.fieldOfView;
    }
    
    private void BaseUpgradeWindowOnOnStartUpgrade()
    {
        isSecondLoop = true;
        SetFollowTarget(null, false, 3, 0, 0, new Vector3(100f,44f,-104.5f), Vector3.right * 90f, isOrthographic: true, orthographicSize: 11.6f);
    }
    
    private void BaseUpgradeWindowOnOnEndUpgrade()
    {
        isSecondLoop = false;
        cam.orthographic = false;
        RemoveTargetByPriority(3, false);
    }

    void StartMenu() => SetFollowTarget(PlayerController.Instance.thisTransform, false);

    void StartGame() => SetFollowTarget(PlayerController.Instance.thisTransform, true, 1, 0, 0, data.inGameOffset, data.inGameAngle);

    void EndGame()
    {
        followTargets.Clear();
        StartCoroutine(DisableOrthographic());
    }

    private IEnumerator DisableOrthographic()
    {
        var routine = StartCoroutine(DisableOrthographicRoutine());
        yield return new WaitForSeconds(3f);
        StopCoroutine(routine);
        if (!isSecondLoop)
            cam.orthographic = false;
    }

    private IEnumerator DisableOrthographicRoutine()
    {
        yield return new WaitUntil(() => cam.orthographic || returnCamCoroutine == null);
        if (!isSecondLoop)
            cam.orthographic = false;
    }

    void StartUpgrade()
    {
        SetFollowTarget(Level.Instance.upgradeZone.CameraPoint, true, 2, 0, 0, Vector3.zero, Vector3.right * 92.2f, true);
    }

    void EndUpgrade()
    {
        cam.orthographic = false;
        RemoveTargetByPriority(2);
    }

    public void Lock() => lockUpdate = true;
    public void Unlock() => lockUpdate = false;

    public void SetFOV(float addFOV)
    {
        FOVTween.Kill();
        FOVTween = DOTween.To(() => this.addFOV, x => this.addFOV = x, addFOV, 3).SetSpeedBased();
    }

    public int SetFollowTarget(Vector3 targetPos, bool transition = true, int priority = 0, float speed = 0, float time = 0, Vector3 offset = default, Vector3 angle = default, bool isOrthographic = false, float orthographicSize = 11f) => SetFollowTarget(null, transition, priority, speed, time, targetPos + offset, angle, isOrthographic);
    public int SetFollowTarget(Transform target, bool transition = true, int priority = 0, float speed = 0, float time = 0, Vector3 offset = default, Vector3 angle = default, bool isOrthographic = false, float orthographicSize = 11f)
    {
        int index = followTargets.FindLastIndex(x => x.priority >= priority);
        if (index == -1) index = 0;
        else index++;

        if (offset == default) offset = MainOffset;
        if (angle == default) angle = MainAngle;

        int hash = target ? target.GetHashCode() : offset.GetHashCode();
        FollowTarget followTarget = new FollowTarget(target, offset, Quaternion.Euler(angle), speed, priority, hash);
        followTargets.Insert(index, followTarget);
        if (time > 0) DOTween.Sequence().SetDelay(time).OnComplete(() => RemoveTargetByHash(hash));

        if (transition)
        {
            if (returnCamCoroutine != null) StopCoroutine(returnCamCoroutine);
            returnCamCoroutine = StartCoroutine(SmoothCamera(isOrthographic, orthographicSize));
        }
        else if (isOrthographic)
        {
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
        }

        return hash;
    }

    public void RemoveTargetByPriority(int priority, bool smooth = true)
    {
        followTargets.RemoveAll(x => x.priority == priority);
        if (smooth) TrySmoothCamera();
    }

    public void RemoveTargetByHash(int hash, bool smooth = true)
    {
        followTargets.RemoveAll(x => x.hash == hash);
        if (smooth) TrySmoothCamera();
    }

    void TrySmoothCamera()
    {
        if (followTargets.Count == 0) return;

        if (returnCamCoroutine != null) StopCoroutine(returnCamCoroutine);
        returnCamCoroutine = StartCoroutine(SmoothCamera());
    }

    IEnumerator SmoothCamera(bool isOrthographic = false, float orthographicSize = 11f)
    {
        lockUpdate = true;

        Vector3 startPos = thisTransform.position;
        Quaternion startAngle = thisTransform.rotation;
        float time = Vector3.Distance(startPos, GetTargetPos()) * data.smoothSpeed + Quaternion.Angle(startAngle, followTargets[0].angle) * data.smoothAngle;
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            thisTransform.position = Vector3.Slerp(startPos, GetTargetPos(), t / time);
            thisTransform.rotation = Quaternion.Slerp(startAngle, followTargets[0].angle, t / time);
            yield return null;
        }

        if (isOrthographic)
        {
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
        }
        
        lockUpdate = false;
    }

    void LateUpdate()
    {
        cam.fieldOfView = startFOV + addFOV;
        
        MoveFollow();
    }

    private void MoveFollow()
    {
        if (lockUpdate) return;
        float deltaTime = Time.deltaTime;

        if (followTargets.Count > 0)
        {
            Vector3 targetPos = GetTargetPos();
            if (followTargets[0].speed != 0) curFollowSpeed = Mathf.Lerp(curFollowSpeed, followTargets[0].speed, deltaTime * 6);

            if (followTargets[0].speed != 0)
                thisTransform.position = Vector3.Lerp(thisTransform.position, targetPos, deltaTime * curFollowSpeed);
            else thisTransform.position = targetPos;
            thisTransform.rotation = followTargets[0].angle;
        }
    }

    void CollisionCam()
    {
        Vector3 camTargetPos = startCamLocalPos;
        if (Physics.SphereCast(thisTransform.position, 0.5f, camTransform.position - thisTransform.position, out RaycastHit hit, 100, data.camCollisionLayerMask))
        {
            float dist = Vector3.Scale(thisTransform.position - hit.point, thisTransform.TransformDirection(startCamLocalPos).normalized).magnitude;
            camTargetPos = new Vector3(startCamLocalPos.x, startCamLocalPos.y, Mathf.Max(-dist, startCamLocalPos.z));
        }
        camTransform.localPosition = Vector3.Slerp(camTransform.localPosition, camTargetPos, Time.deltaTime * 9);
    }

    Vector3 GetTargetPos() => (followTargets[0].transform ? followTargets[0].transform.position : Vector3.zero) + followTargets[0].offset;
}
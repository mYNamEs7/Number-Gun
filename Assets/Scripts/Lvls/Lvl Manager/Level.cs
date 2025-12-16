using Dreamteck.Splines;
using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    #region Singletone
    private static Level _instance;
    public static Level Instance { get => _instance; }
    public Level() => _instance = this;
    #endregion

    LevelStage stage;
    public LevelStage Stage
    {
        get => stage;
        set
        {
            if (stage == value) return;
            //
            switch (stage)
            {
                case LevelStage.Upgrade:
                    {
                        OnEndUpgrade?.Invoke();
                    }
                    break;
            }
            //
            switch (value)
            {
                case LevelStage.Upgrade:
                    {
                        OnStartUpgrade?.Invoke();
                        upgradeZone.Get();
                    }
                    break;
            }
            //
            stage = value;
        }
    }

    [SerializeField] private LevelType type;
    [SerializeField] public Transform playerSpawnPoint;
    [NonSerialized] public ConveyorBelt conveyorBelt;
    [NonSerialized] public UpgradeZone upgradeZone;
    [NonSerialized] public FinishZone finishZone;
    [NonSerialized] public Transform thisTransform;

    public static event Action OnStartUpgrade, OnEndUpgrade;

    void OnEnable()
    {
        GameManager.OnStartMenu += SetPlayerPos;
        GameManager.OnStartGame += GameStart;
    }

    void OnDisable()
    {
        GameManager.OnStartMenu -= SetPlayerPos;
        GameManager.OnStartGame -= GameStart;
    }

    void Awake()
    {
        thisTransform = transform;
        conveyorBelt = GetComponentInChildren<ConveyorBelt>();
        upgradeZone = GetComponentInChildren<UpgradeZone>();
        finishZone = GetComponentInChildren<FinishZone>();
        stage = LevelStage.None;
        Init();
    }

    public void Init() { }

    void SetPlayerPos() => PlayerController.Instance.thisTransform.position = playerSpawnPoint ? playerSpawnPoint.position : Vector3.zero;

    void GameStart()
    {
        enabled = true;
        stage = LevelStage.First;
    }

    void Update()
    {
        switch (stage)
        {
            case LevelStage.First:
                {
                    if (PlayerController.Instance.thisTransform.position.z >= upgradeZone.thisTransform.position.z) Stage = LevelStage.Upgrade;
                }
                break;
            case LevelStage.Second:
                {
                    if (PlayerController.Instance.thisTransform.position.z >= finishZone.thisTransform.position.z)
                    {
                        Stage = LevelStage.Third;
                        finishZone.Active();
                    }
                }
                break;
            case LevelStage.Third:
                {
                    if (PlayerController.Instance.thisTransform.position.z >= finishZone.finishPoint.position.z)
                    {
                        Stage = LevelStage.Finish;
                        GameManager.Instance.Finish(true);
                    }
                }
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (playerSpawnPoint != null)
        {
            Gizmos.color = Color.magenta;
            var m = Gizmos.matrix;
            Gizmos.matrix = playerSpawnPoint.localToWorldMatrix;
            Gizmos.DrawSphere(Vector3.up * 0.5f + Vector3.forward, 0.5f);
            Gizmos.DrawCube(Vector3.up * 0.5f, Vector3.one);
            Gizmos.matrix = m;
        }
    }
#endif
}

public enum LevelStage
{
    None, First, Upgrade, Second, Third, Finish
}

public enum LevelType
{
    Default, Tutorial
}
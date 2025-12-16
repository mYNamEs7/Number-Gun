using System;
using DG.Tweening;
using UnityEngine;
using YG;

public class FinishZone : MonoBehaviour
{
    public static event Action OnEnter;
    
    [SerializeField] Transform record;
    [SerializeField] Transform weapon;
    [SerializeField] private Transform[] sideHighScores;
    [SerializeField] private Transform highScore;
    public Transform finishPoint;
    [SerializeField] MeshFilter weaponMesh;
    [SerializeField] Renderer weaponRenderer;
    [NonSerialized] public Transform thisTransform;
    bool active;

    private float startRecordPosZ;

    void OnEnable() => GameManager.OnFinishEvent += Finish;
    void OnDisable() => GameManager.OnFinishEvent -= Finish;

    void Start()
    {
        thisTransform = transform;
        record.localPosition = new Vector3(record.localPosition.x, record.localPosition.y, Mathf.Max(YandexGame.savesData.Record, record.localPosition.z));
        startRecordPosZ = record.position.z;
        Weapon newWeapon = GameData.Default.weapons[Mathf.Min((int)GameData.Default.GetUpgrade(UpgradeType.Years).CurValue / 50 + 1, GameData.Default.weapons.Length - 1)];
        // weaponMesh.sharedMesh = newWeapon.meshFilter.sharedMesh;
        // weaponRenderer.sharedMaterial = newWeapon.thisRenderer.sharedMaterial;
    }

    public void Active()
    {
        OnEnter?.Invoke();
        active = true;
        UIManager.Instance.inGameWindow.Hide();
    }

    void Update()
    {
        if (!active) return;
        record.position = new Vector3(record.position.x, record.position.y, Mathf.Lerp(record.position.z, Mathf.Max(PlayerController.Instance.thisTransform.position.z + 8, record.position.z), Time.deltaTime * 9));
        weapon.position = new Vector3(weapon.position.x, weapon.position.y, PlayerController.Instance.thisTransform.position.z + 68.7014f);
    }

    public void Finish(bool finish)
    {
        if (record.position.z > startRecordPosZ)
        {
            foreach (var sideHighScore in sideHighScores)
            {
                sideHighScore.DORotate(Vector3.forward * (sideHighScore.localScale.x * -180), 0.5f);
            }
            
            highScore.gameObject.SetActive(true);
            highScore.position = new Vector3(PlayerController.Instance.thisTransform.position.x, highScore.position.y, PlayerController.Instance.thisTransform.position.z);
            highScore.DOMoveY(0, 1f).SetEase(Ease.OutElastic, 2f);
            PlayerController.Instance.MoveSide();
        }
        
        YandexGame.savesData.Record = record.localPosition.z;
        YandexGame.SaveProgress();
    }
}

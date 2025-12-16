using DG.Tweening;
using UnityEngine;

public class Tank : HPObstacle
{
    [SerializeField] TeleportGate teleportGate;
    [SerializeField] float offsetBetweenPlayer;
    [SerializeField] int cashCount = 9;
    [SerializeField] int reward = 500;
    bool isActive;
    Tween scaleTween;

    [Header("Shoot")]
    [SerializeField] int damage;
    [SerializeField] float fireRate, bulletSpeed;
    [SerializeField] Vector3 gunPoint;
    float shootT;

    [Space, Header("Refs")] 
    [SerializeField] private Animator animator;
    [SerializeField] private Transform weapon;
    [SerializeField] EnemyBullet bullet;

    private UIYearsSlider yearsSlider;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");

    public void Enable()
    {
        yearsSlider = FindObjectOfType<UIYearsSlider>();
        yearsSlider.gameObject.SetActive(false);
        
        isActive = true;
        EnableHp();
        
        animator.SetBool(IsWalking, true);
        animator.SetFloat(Vertical, -1);

        teleportGate.OnCollect += OnTeleportGateCollect;
    }

    private void OnTeleportGateCollect()
    {
        DisableHp();
        yearsSlider.gameObject.SetActive(true);
    }

    void Update()
    {
        if (state == State.Collect || !isActive) return;
        shootT += Time.deltaTime * fireRate;
        if (shootT > 1)
        {
            shootT -= 1;
            Shoot();
        }

        Vector3 playerPos = PlayerController.Instance.thisTransform.position;
        var sign = Mathf.Sign(Time.time % 8 - 4) * 3;
        if (!Mathf.Approximately(animator.GetFloat(Horizontal), -sign))
            animator.SetFloat(Horizontal, -sign);
        Vector3 targetPos = new Vector3(sign, thisTransform.position.y, Mathf.Max(playerPos.z + offsetBetweenPlayer, thisTransform.position.z));
        thisTransform.position = Vector3.Lerp(thisTransform.position, targetPos, Time.deltaTime * 1);
        thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, Quaternion.LookRotation(-(targetPos + Vector3.forward * 2 - thisTransform.position)), Time.deltaTime * 3);
    }

    void Shoot() => Instantiate(bullet, weapon.TransformPoint(gunPoint), Quaternion.LookRotation(-weapon.right), Level.Instance.thisTransform).Shoot(-weapon.right, bulletSpeed, damage);
    
    public override void BulletHit(int damage, int multiplyDamage = 1)
    {
        base.BulletHit(damage);

        if (!scaleTween.IsActive()) scaleTween = thisTransform.DOPunchScale(Vector3.one * 0.05f, GameData.Default.cardHitDuration, 0).SetEase(GameData.Default.cardHitEase);
    }

    protected override void Broke()
    {
        OnTeleportGateCollect();
        teleportGate.thisTransform.DOMoveZ(thisTransform.position.z + 4, 0.5f);
        for (int i = 0; i < cashCount; i++)
        {
            Cash cash = Instantiate(GameData.Default.cash, thisTransform.position, Quaternion.Euler(0, Random.Range(-180f, 180f), 0), thisTransform.parent);
            cash.reward = reward / cashCount;
            cash.UnFreezePos();
            cash.Drop();
            cash.thisRigidbody.velocity += (Vector3.back + Vector3.right * Random.Range(-1f, 1f)) * Random.Range(4, 6);
        }
        base.Broke();
    }
}

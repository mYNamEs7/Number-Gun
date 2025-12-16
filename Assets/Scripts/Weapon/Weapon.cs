using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Grid;
using UnityEngine;
using UnityEngine.Pool;
using YG;

public class Weapon : MonoBehaviour
{
    const int FireRateToMultiplyBullets = 10;
    readonly static Color UpgradeTargetColor = new Color(0.016f, 0.78f, 0);

    [NonSerialized] public Modifier detailsParent;
    private Vector3 startDetailsPosition = new Vector3(0f, 0.342f, 0.859f); /*new Vector3(0f, 0.342f, 0.678f);*/
    private Vector3 targetGunPoint = new Vector3(0, 0.42f, 1.12f);

    [HideInInspector, SerializeField] WeaponState state;
    public WeaponState State
    {
        get => state;
        set
        {
            if (state == value) return;
            switch (value)
            {
                case WeaponState.Shoot:
                    // yearsTag.gameObject.SetActive(false);
                    bulletData = new()
                    {
                        speed = GameData.Default.bulletSpeed,
                        range = GameData.Default.fireRange * GameData.Default.GetUpgrade(UpgradeType.FireRange).CurValue + GameData.AddedFireRange,
                        size = GameData.Default.bulletSize,
                        fireRate = GameData.Default.fireRate * GameData.Default.GetUpgrade(UpgradeType.FireRate).CurValue + GameData.AddedFireRate
                    };
                    break;
            }
            state = value;
        }
    }

    public WeaponData data;

    public Vector3 GunPoint => gunPoint;
    [SerializeField] Vector3 gunPoint;
    public List<Transform> gunPoints = new List<Transform>();
    
    [SerializeField] private Vector3 yearsPoint;
    [SerializeField] private Vector3 handPoint;

    [HideInInspector] public Transform thisTransform;
    Tween shootRecoilTween;
    public ObjectPool<Bullet> bulletsPool;
    Transform levelTransform;

    [HideInInspector] public BulletData bulletData;
    [HideInInspector, SerializeField] public int bulletCount = 1;
    [HideInInspector, SerializeField] int addDamage;
    [HideInInspector, SerializeField] int permanentDamage;
    [HideInInspector, SerializeField] int months;
    [NonSerialized] public float shootT;
    [HideInInspector, SerializeField] bool bulletsCheckCollision = true;

    public MeshFilter meshFilter;
    public Renderer thisRenderer;
    public BoxCollider boxCollider;
    [HideInInspector, SerializeField] public YearsTag yearsTag;
    Material material;
    private Hand hand;

    [SerializeField] private bool isCardWeapon;
    [SerializeField] private ParticleSystem gunChangingEffect;

    GameObject upgradeTargetParticles;

    private RotatedCard rotatedCard;

    private float startGunPointZ;
    public static float fireRateByYears;
    private Coroutine rotatedRoutine;
    private Vector3 endRotation;
    
    [HideInInspector] public GameObject mainPoint;
    
    public float PosX { get; private set; }

    void OnEnable() => GameManager.OnRestartEvent += Destroy;
    void OnDisable() => GameManager.OnRestartEvent -= Destroy;

    public Weapon Init()
    {
        gunPoints.Clear();
        mainPoint = new GameObject
        {
            transform =
            {
                localPosition = gunPoint
            }
        };
        gunPoints.Add(mainPoint.transform);
        
        thisTransform = transform;
        levelTransform = Level.Instance.thisTransform;
        bulletsPool = NewBulletsPool();

        material = new(thisRenderer.sharedMaterial);
        thisRenderer.sharedMaterial = material;
        
        if (!yearsTag) yearsTag = Instantiate(GameData.Default.yearsTag, thisTransform);
        SpawnHand();
        
        UpdateLvl();
        
        startGunPointZ = gunPoint.z;
        
        detailsParent = Instantiate(new GameObject(), thisTransform).AddComponent<Modifier>();
        RecalculateDetailsParent();
        
        fireRateByYears = GameData.Default.fireRateByYears[data.lvl];
        CheckForGolden(data.lvl);

        return this;
    }

    public void UpdatePosX() => PosX = thisTransform.localPosition.x;

    public void TransformRightGunPointZ()
    {
        var offset = 0.38f;
        if (gunPoint.z + offset <= startGunPointZ + offset)
            gunPoint.z += offset;
    }
    
    public void TransformLeftGunPointZ()
    {
        var offset = 0.38f;
        if (gunPoint.z - offset >= startGunPointZ)
            gunPoint.z -= offset;
    }

    public void RespawnHand(bool withRotation)
    {
        if (hand)
        {
            Destroy(hand.gameObject);
            hand = null;
        }
        SpawnHand(withRotation);
    }

    private void SpawnHand(bool withRotation = true)
    {
        if (hand) return;
        
        hand = Instantiate(GameData.Default.hands[GameData.CurHandId], thisTransform);
        hand.thisTransform.localPosition = handPoint;

        GameData.HandIncome = hand.data.income;
        GameData.HandFireRate = hand.data.fireRate;
        GameData.HandFireRange = hand.data.fireRange;

        if (PlayerController.Instance.Weapons.Count >= 1 && withRotation)
        {
            var scale = hand.thisTransform.localScale;
            scale.x *= -1;
            hand.thisTransform.localScale = scale;
        }
    }

    public void StartUpgrade()
    {
        boxCollider.enabled = false;
        hand.gameObject.SetActive(false);
        if (yearsTag) yearsTag.gameObject.SetActive(false);
        bulletsCheckCollision = false;
        
        rotatedCard = detailsParent.gameObject.AddComponent<RotatedCard>();
        rotatedCard.enabled = false;
    }

    public void EndUpgrade()
    {
        boxCollider.enabled = true;
        hand?.gameObject.SetActive(true);
        if (yearsTag) StartCoroutine(EnableYears());
        bulletsCheckCollision = true;
        if (rotatedCard) rotatedCard.enabled = true;
    }

    private IEnumerator EnableYears()
    {
        yield return new WaitForSeconds(1f);
        yearsTag.gameObject.SetActive(true);
    }

    public Weapon SetUpgradeTarget()
    {
        material.SetColor("_EmissionColor", UpgradeTargetColor);
        if (!upgradeTargetParticles) upgradeTargetParticles = Instantiate(GameData.Default.upgradeTargetParticles, thisTransform.position, Quaternion.identity).gameObject;
        return this;
    }
    public void ReleaseUpgradeTarget()
    {
        material.SetColor("_EmissionColor", Color.clear);
        if (upgradeTargetParticles) Destroy(upgradeTargetParticles);
    }

    public void Upgrade(UpgradeType upgrade, float value)
    {
        switch (upgrade)
        {
            case UpgradeType.Multiplier:
                {
                    bulletData.fireRate *= value;
                }
                break;
            case UpgradeType.Increase:
                {
                    bulletData.fireRate += value;
                }
                break;
            case UpgradeType.FireRate:
                {
                    bulletData.fireRate += value * GameData.Default.fireRateUIToUpgradeMultiplier;
                }
                break;
            case UpgradeType.FireRange:
                {
                    bulletData.range += value * GameData.Default.fireRangeUIToUpgradeMultiplier;
                }
                break;
            case UpgradeType.BulletSize:
                {
                    bulletData.size += value * GameData.Default.bulletSize;
                }
                break;
            case UpgradeType.Years:
                {
                    months += Mathf.RoundToInt(value * 12);
                    UpdateMonths();
                }
                break;
            case UpgradeType.Months:
                {
                    months += (int)value;
                    UpdateMonths();
                }
                break;
            case UpgradeType.Damage:
                {
                    addDamage += (int)value;
                }
                break;
            case UpgradeType.MultiShot:
            {
                bulletCount = value < 1 ? 1 : bulletCount * (int)value;
            }
                break;
            case UpgradeType.GunAmount:
                {
                    hand.gameObject.SetActive(false);
                    for (int i = 0; i < value; i++) PlayerController.Instance.InstantiateWeapon(this);
                    hand.gameObject.SetActive(true);
                    PlayerController.Instance.YearsCenter();
                }
                break;
            case UpgradeType.GunAmountMultiply:
                {
                    hand.gameObject.SetActive(false);
                    for (int i = 1; i < value; i++) PlayerController.Instance.InstantiateWeapon(this);
                    hand.gameObject.SetActive(true);
                }
                break;
        }
    }

    void UpdateMonths()
    {
        int addLvl = Mathf.Min(months / 600, GameData.Default.weapons.Length - data.lvl - 1);
        if (months < 0)
        {
            addLvl = -1;
            months += 600;
        }
        else months -= addLvl * 600;
        if (GameData.Default.weapons.Length <= addLvl || (addLvl < 0 && data.lvl == 0)) return;
        if (addLvl != 0)
        {
            // var year = 1800 + data.lvl * 50 + months / 12;
            // var StepCnt = (year-1800) / 50;
            // var result = 1;
            // for (int i = 0; i < StepCnt; i++)
            // {
            //     result = result + 2 + (i / 4);
            // }
            
            print(addLvl);
            print(data.lvl);
            var weaponIndex = data.lvl + addLvl;
            SetSkin(GameData.Default.weapons[weaponIndex]);
            bulletsPool.Dispose();
            bulletsPool = NewBulletsPool();
            UpdateLvl();
            
            fireRateByYears = GameData.Default.fireRateByYears[weaponIndex];
            PlayerController.Instance.MultiplyFireRate(1);
        }
        yearsTag.Set(1800 + data.lvl * 50 + months / 12);
        // yearsTag.Set(1800 + (int)GameData.Default.GetUpgrade(UpgradeType.Years).CurValue);
    }

    public void SetSkin(Weapon newWeapon)
    {
        meshFilter.transform.localRotation = newWeapon.meshFilter.transform.localRotation;
        meshFilter.sharedMesh = newWeapon.meshFilter.sharedMesh;
        material = new(newWeapon.thisRenderer.sharedMaterial);
        thisRenderer.sharedMaterial = material;
        boxCollider.center = newWeapon.boxCollider.center;
        boxCollider.size = newWeapon.boxCollider.size;
        thisTransform.localScale = newWeapon.transform.localScale;
        data = new() { lvl = newWeapon.data.lvl, bullet = newWeapon.data.bullet, damage = newWeapon.data.damage };
        gunPoint = newWeapon.gunPoint;
        handPoint = newWeapon.handPoint;
        
        CheckForGolden(data.lvl);
        
        hand.thisTransform.localPosition = handPoint;
        RecalculateDetailsParent();

        if (rotatedRoutine != null)
        {
            StopCoroutine(rotatedRoutine);
            thisTransform.localEulerAngles = endRotation;
        }
        rotatedRoutine = StartCoroutine(GunRotate(0.3f));
    }

    public void SetGolden(int curLvl)
    {
        GameData.GoldenWeapons[curLvl] = true;
        CheckForGolden(curLvl);
    }

    private void CheckForGolden(int curLvl)
    {
        if (GameData.GoldenWeapons[curLvl])
        {
            thisRenderer.material = GameData.Default.goldenGunsMaterial;
            permanentDamage = data.damage;
            GameData.GoldenGunFireRate = 3;
            GameData.GoldenGunFireRange = 3;
            PlayerController.Instance.UpdateFireRate();
        }
        else
        {
            thisRenderer.material = GameData.Default.gunsMaterial;
            permanentDamage = 0;
            GameData.GoldenGunFireRate = 0;
            GameData.GoldenGunFireRange = 0;
            PlayerController.Instance.UpdateFireRate();
        }
    }

    private void RecalculateDetailsParent()
    {
        var diff = gunPoint - targetGunPoint;
        detailsParent.transform.localPosition = startDetailsPosition + diff;
    }

    private IEnumerator GunRotate(float time)
    {
        gunChangingEffect?.Play();
        
        var startScale = thisTransform.localEulerAngles;
        var endScale = endRotation = startScale + Vector3.down * 360;
        float currentTime = 0.0f;

        while (currentTime <= time)
        {
            thisTransform.localEulerAngles = Vector3.Lerp(startScale, endScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }

        thisTransform.localEulerAngles = endScale;
        rotatedRoutine = null;
    }

    void UpdateLvl()
    {
        if (yearsTag)
            yearsTag.transform.localPosition = yearsPoint;
    }

    ObjectPool<Bullet> NewBulletsPool() => new ObjectPool<Bullet>(() => NewBullet(), (obj) => obj.thisGameObject.SetActive(true), (obj) => obj.thisGameObject.SetActive(false), (obj) => Destroy(obj.thisGameObject), false, 10, 1000);

    Bullet NewBullet()
    {
        Bullet bullet = Instantiate(data.bullet, levelTransform);
        bullet.Init(bulletsPool);
        return bullet;
    }

    public void Shoot()
    {
        float offset = bulletCount > 1 ? (PlayerController.Instance.thisTransform.position.x - thisTransform.position.x) * 0.5f : 0;
        
        for (int k = 0; k < gunPoints.Count; k++)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                Bullet bullet = bulletsPool.Get();
                int childCount = (int)bulletData.fireRate / FireRateToMultiplyBullets;
                Bullet[] childs = childCount > 0 ? new Bullet[childCount] : null;
                for (int j = 0; j < childCount; j++) do childs[j] = bulletsPool.Get(); while (childs[j] == bullet);

                var gunPointPosition = gunPoints[k].position.z > 10
                    ? gunPoints[k].position
                    : thisTransform.TransformPoint(gunPoints[k].position);
                
                bullet.thisTransform.position = gunPointPosition + Vector3.right * ((-(bulletCount - 1) / 2f + i) * PlayerController.SpaceBetweenWeapons * 0.75f + offset);
                bullet.Shoot(Quaternion.Euler(0, (-(bulletCount - 1) * 0.5f + i) * 9, 0) * (isCardWeapon? thisTransform.right : thisTransform.forward), bulletData, (data.damage + addDamage + permanentDamage) * (childCount + 1), childCount + 1, bulletsCheckCollision, childs);
            }
        }
    }

    public void CustomUpdate()
    {
        if (state != WeaponState.Shoot) return;

        shootT += Time.deltaTime * bulletData.fireRate / ((int)bulletData.fireRate / FireRateToMultiplyBullets + 1);
        for (int i = 0; i < (int)shootT; i++) Shoot();
        shootT -= (int)shootT;
    }

    public void Fall()
    {
        if (rotatedCard) rotatedCard.enabled = false;
        
        if (TryGetComponent(out Rigidbody _)) return;

        foreach (Transform detail in detailsParent.thisTransform)
        {
            detail.parent = Level.Instance.thisTransform;
        }
        
        // if (rotatedCard) rotatedCard.enabled = false;
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.AddForce(Vector3.right * (hand.transform.localScale.x * 100f));
        
        // if (!isReal)
        //     thisTransform.parent = null;
    }

    public void Destroy()
    {
        if (bulletsPool != null) bulletsPool.Dispose();
        Destroy(gameObject);
    }
    
    public class Modifier : MonoBehaviour
    {
        [NonReorderable] public Transform thisTransform;
        
        public Transform[] points;

        private void Awake()
        {
            thisTransform = transform;
        }
    }
}

public interface IWeaponTarget
{
    public void WeaponHit(Collider collider) { }
}

public enum WeaponState
{
    Idle, Shoot
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    [SerializeField] Collider thisCollider;
    [HideInInspector] public Transform thisTransform;
    Rigidbody thisRigidbody;
    [System.NonSerialized] public int value;
    Tween colliderTween;
    bool forPlayer;

    public void Init(int value, bool forPlayer = true)
    {
        this.value = value;
        this.forPlayer = forPlayer;
        thisTransform = transform;
        thisRigidbody = GetComponent<Rigidbody>();
        thisRigidbody.isKinematic = false;
        thisCollider.enabled = true;

        SphereCollider collider = GetComponent<SphereCollider>();
        collider.enabled = true;
        colliderTween = DOTween.To(() => collider.radius, x => collider.radius = x, 0, 1).From();

        Vector2 randomCircle = Random.insideUnitCircle;
        thisRigidbody.AddForce(new Vector3(randomCircle.x, Random.Range(0.66f, 2f), randomCircle.y).normalized * Random.Range(200f, 300f));
    }

    public void AddValue(int value) => this.value += value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            thisCollider.enabled = false;
            thisRigidbody.isKinematic = true;
            colliderTween.Kill();
            // StartCoroutine(Get(player));
        }
    }

    // IEnumerator Get(PlayerController player)
    // {
    //     Vector3 startPos = thisTransform.position;
    //     float randomOffset = Random.Range(3f, 5f);
    //     Vector3 dir = Vector3.Scale(thisTransform.position - player.thisTransform.position, new Vector3(1, 0, 1));

    //     float t = 0;
    //     while (dir.magnitude > 1)
    //     {
    //         yield return null;
    //         t += Time.deltaTime;
    //         float tCurve = GameData.Default.moneyCollectAnimCurve.Evaluate(t / GameData.Default.moneyCollectTime);

    //         Vector3 endPos = hero.thisTransform.position + Vector3.up;
    //         dir = Vector3.Scale(thisTransform.position - endPos, new Vector3(1, 0, 1));
    //         Vector3 offset = Vector3.up + dir.normalized;
    //         Vector3 wayPoint = startPos + offset * randomOffset;
    //         thisTransform.position = Mathf.Pow(1 - tCurve, 2) * startPos + 2 * (1 - tCurve) * tCurve * wayPoint + Mathf.Pow(tCurve, 2) * endPos;
    //     }

    //     if (hero.side == Side.Ally) Collect();
    //     else ((EnemyHero)hero).AddCoins(value);

    //     Instantiate(GameData.Default.coinCollectParticles, hero.thisTransform.position + Vector3.up, Quaternion.identity);
    //     Destroy(gameObject);
    // }

    public void Collect()
    {
        SoundHolder.Default.PlayFromSoundPack("Coin");
        GameData.Default.AddKeys(value);
    }

    public void Destroy()
    {
        colliderTween.Kill();
        Destroy(gameObject);
    }
}

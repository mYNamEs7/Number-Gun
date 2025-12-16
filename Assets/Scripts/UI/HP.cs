// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// using TMPro;
// using DG.Tweening;

// [RequireComponent(typeof(Billboard))]
// public class HP : MonoBehaviour
// {

//     [HideInInspector] public int maxHP;
//     [HideInInspector] public int curHP;
//     [SerializeField] private TextMeshPro txt;
//     [SerializeField] protected Transform slider;
//     [SerializeField] private float zeroSliderPos;
//     [SerializeField] private GameObject deductibleHP;

//     [HideInInspector] public UnityAction deathAction;
//     protected Transform thisTransform;
//     private Tween addHPTween;

//     [SerializeField] private Transform oldHPSlider;
//     private Tween oldHPTween;

//     public virtual void Init(int hp, UnityAction deathAction)
//     {
//         thisTransform = transform;

//         maxHP = hp;
//         curHP = hp;

//         this.deathAction += deathAction;
//         UpdateHP();
//     }

//     protected virtual void UpdateHP()
//     {
//         txt.text = curHP.ToString();
//         slider.localPosition = Vector3.Lerp(Vector3.right * zeroSliderPos, Vector3.zero, (float)curHP / maxHP);
//         oldHPTween.Kill();
//         oldHPTween = oldHPSlider.DOLocalMoveX(slider.localPosition.x, 0.66f);
//     }

//     public void NotAnimAddHP(int count)
//     {
//         maxHP += count;
//         curHP += count;
//         UpdateHP();
//     }

//     public void AddHP(int count)
//     {
//         maxHP += count;
//         addHPTween.Kill();
//         addHPTween = DOTween.To(() => curHP, x => curHP = x, maxHP, GameData.Default.deductibleHPTime).OnUpdate(() => UpdateHP());
//         DeductibleHP(count);
//         UpdateHP();
//     }

//     public void RegenHP(int count)
//     {
//         curHP = Mathf.Min(curHP + count, maxHP);
//         DeductibleHP(count);
//         UpdateHP();
//     }

//     public void SubHP(int count)
//     {
//         maxHP -= count;
//         curHP -= count;
//         DeductibleHP(-count);
//         UpdateHP();
//     }

//     public virtual bool TakeDamage(int count)
//     {
//         curHP -= count;
//         UpdateHP();
//         DeductibleHP(-count);

//         if (curHP <= 0)
//         {
//             Death();
//             return true;
//         }
//         return false;
//     }

//     protected virtual void Death()
//     {
//         oldHPTween.Kill();
//         deathAction.Invoke();
//         Destroy(gameObject);
//     }

//     protected void DeductibleHP(int count)
//     {
//         GameObject hp = Instantiate(deductibleHP, thisTransform.position, Quaternion.identity);
//         TextMeshPro txt = hp.GetComponent<TextMeshPro>();
//         txt.text = (count > 0 ? "+" : "-") + Mathf.Abs(count);
//         txt.color = count > 0 ? new Color(0.07f, 1, 0.07f, 1) : new Color(1, 0.07f, 0.07f, 1);

//         Transform hpTransform = hp.transform;
//         hpTransform.localScale = Vector3.zero;
//         hpTransform.DOScale(GameData.Default.deductibleHPScale, GameData.Default.deductibleHPTime).SetEase(GameData.Default.deductibleHPAnimCurve);
//         Vector3 addPos = (thisTransform.right * Random.Range(0.8f, 1.2f) + thisTransform.up * Random.Range(0.8f, 1.2f)) * (GameData.Default.deductibleHPDist + Random.Range(-0.33f, 0.33f));
//         hpTransform.DOMove(hpTransform.position + addPos, GameData.Default.deductibleHPTime)
//         .SetEase(Ease.OutSine)
//         .OnComplete(() => Destroy(hp));
//     }
// }

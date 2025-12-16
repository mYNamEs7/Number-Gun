using System;
using Lvls.Upgrades;
using TMPro;
using UnityEngine;

namespace Lvls.Obstacles
{
    public class Human : ScaleBulletTarget
    {
        [SerializeField] int hp;

        [Space, Header("Refs")]
        [SerializeField] GameObject hpTag;
        [SerializeField] TextMeshPro hpTxt;
        [SerializeField] private Cash cash;
        [SerializeField] private Renderer renderer;
        [SerializeField] private Animator animator;
        [SerializeField] private string targetAnim;
        [SerializeField] private float range = 10;
        bool death;

        private DamageObstacle damageObstacle;
        private static readonly int Death1 = Animator.StringToHash("Death");

        protected override void Awake()
        {
            base.Awake();
            UpdateHP();
            damageObstacle = GetComponent<DamageObstacle>();
        }

        void UpdateHP() => hpTxt.text = hp.ToString();

        public override void BulletHit(int damage, int multiplyDamage = 1)
        {
            if (death) return;
            hp -= damage;
            UpdateHP();

            if (hp <= 0) Death();
        }

        protected virtual void Death()
        {
            if (TryGetComponent(out ObjectMover mover))
                Destroy(mover);
            if(TryGetComponent(out ShootingObstacle shootingObstacle))
                Destroy(shootingObstacle);
            
            animator.SetTrigger(Death1);
            death = true;
            
            Destroy(damageObstacle);
            Destroy(hpTag);
            Destroy(this);
            gameObject.layer = 5;
            cash.gameObject.SetActive(true);
            renderer.material.color = Color.gray;
        }

        private void Update()
        {
            if(thisTransform.position.z - PlayerController.Instance.thisTransform.position.z < range && !animator.GetBool(targetAnim))
                animator.SetBool(targetAnim, true);
        }
    }
}

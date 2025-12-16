using System;
using System.Collections;
using UnityEngine;

namespace Lvls.Upgrades
{
    public class ObjectMover : MonoBehaviour
    {
        [NonSerialized] public Transform thisTransform;
        
        [SerializeField] private Vector3 pointA;
        [SerializeField] private Vector3 pointB;
        [SerializeField] private float speed;

        private Vector3 positionA;
        private Vector3 positionB;

        private void Awake()
        {
            thisTransform = transform;
            
            positionA = thisTransform.TransformPoint(pointA);
            positionB = thisTransform.TransformPoint(pointB);

            StartCoroutine(Move(positionA, positionB));
        }

        private IEnumerator Move(Vector3 a, Vector3 b)
        {
            while (true) // Бесконечный цикл
            {
                // Двигаем объект в точку A
                yield return StartCoroutine(MoveToPoint(a));

                // Двигаем объект в точку B
                yield return StartCoroutine(MoveToPoint(b));
            }
        }
        
        private IEnumerator MoveToPoint(Vector3 target)
        {
            // Пока объект не достигнет цели
            while (Vector3.Distance(thisTransform.position, target) > 0.1f)
            {
                // Перемещаем объект ближе к цели
                thisTransform.position = Vector3.MoveTowards(thisTransform.position, target, speed * Time.deltaTime);
                yield return null;  // Ждем следующий кадр
            }
        }
    }
}

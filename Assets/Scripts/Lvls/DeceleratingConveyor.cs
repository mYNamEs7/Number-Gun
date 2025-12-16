using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeceleratingConveyor : MonoBehaviour
{    
    [SerializeField] float speed = 1;
    [SerializeField] Material material;
    bool move;
    Transform thisTransform;

    void Start()
    {
        thisTransform = transform;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Weapon weapon)) move = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent(out Weapon weapon)) move = false;
    }

    void Update()
    {
        material.SetTextureOffset("_BaseMap", new Vector2(0, Time.time * speed * 0.2f));
        if (move) PlayerController.Instance.thisTransform.localPosition += thisTransform.up * (Time.deltaTime * speed);
    }
}

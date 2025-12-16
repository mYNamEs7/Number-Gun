using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpArrow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer arrow;
    [SerializeField] private TextMeshPro txt;

    Transform thisTransform, playerTransform;

    void Start()
    {
        thisTransform = transform;
        playerTransform = PlayerController.Instance.transform;
    }

    public void SetText(string text)
    {
        txt.text = text;
    }

    void Update()
    {
        Vector3 diff = thisTransform.position - playerTransform.position;
        float dist = new Vector3(diff.x, 0, diff.z).magnitude - 1.5f;
        arrow.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, dist));
        txt.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, dist));
    }
}

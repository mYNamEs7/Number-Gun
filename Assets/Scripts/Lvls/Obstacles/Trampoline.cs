using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Trampoline : MonoBehaviour, IWeaponTarget
{
    [Header("Params")] 
    [SerializeField] private float jumpPower = 3;
    [SerializeField] private float jumpTime = 0.5f;
    
    [Space]
    [SerializeField] private float jumpOffPower = 10;
    [SerializeField] private float jumpOffTime = 0.7f;
    
    [Header("Refs")]
    [SerializeField] private Transform road;
    
    public void WeaponHit(Collider collider)
    {
        var playerTransform = PlayerController.Instance.thisTransform;
        playerTransform.DOJump(
            new Vector3(playerTransform.position.x, road.position.y + road.lossyScale.y * 0.5f,
                road.position.z - road.lossyScale.z * 0.5f), jumpPower, 1, jumpTime);
        
        PlayerController.Instance.HorizontalClamp.x = road.position.x - road.lossyScale.x * 0.5f;
        PlayerController.Instance.HorizontalClamp.y = road.position.x + road.lossyScale.x * 0.5f;

        StartCoroutine(JumpOff());
    }

    private IEnumerator JumpOff()
    {
        yield return new WaitUntil(() =>
            PlayerController.Instance.thisTransform.position.z >= road.position.z + road.lossyScale.z * 0.5f);
        
        var playerTransform = PlayerController.Instance.thisTransform;
        playerTransform.DOJump(
            new Vector3(playerTransform.position.x, transform.position.y,
                playerTransform.position.z + 2f), jumpOffPower, 1, jumpOffTime);
        
        PlayerController.Instance.HorizontalClamp.x = PlayerController.Instance.data.horizontalClamp.x;
        PlayerController.Instance.HorizontalClamp.y = PlayerController.Instance.data.horizontalClamp.y;
    }
}

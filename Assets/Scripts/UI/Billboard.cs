using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class Billboard : MonoBehaviour
{

    public Vector3Int space = new Vector3Int(1, 1, 1);
    
    private bool isYearsPopup;
    Transform thisTransform;
    Transform cam;

    void Start()
    {
        isYearsPopup = GetComponent<Popup>().isYearsPopup;
        
        thisTransform = transform;
        cam = Camera.main.transform;
        if (isYearsPopup && PlayerController.Instance.Weapons[0].transform.localEulerAngles != Vector3.zero)
            space.z *= -1;
        LateUpdate();
    }

    public void LateUpdate()
    {
        thisTransform.rotation = Quaternion.LookRotation(Vector3.Scale(cam.position - thisTransform.position, space));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Hand Data")]
public class HandData : ScriptableObject
{
    public int income;
    public int fireRate;
    public int fireRange = 1;
}

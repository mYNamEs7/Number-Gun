using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Lvls List")]
public class LvlsList : ScriptableObject
{
    [System.Serializable]
    public class Lvl
    {
        public Level data;
    }

    public bool randomizedLvls;
    public List<Lvl> lvls;
}

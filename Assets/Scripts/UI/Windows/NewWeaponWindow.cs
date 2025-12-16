using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class NewWeaponWindow : PopupWindow
{
    [SerializeField] MeshFilter weaponMesh;
    [SerializeField] Renderer weaponRenderer;

    public override void Show()
    {
        base.Show();
        GameData.Default.GetUpgrade(UpgradeType.Years).CurLvl += 10;
        Weapon newWeapon = GameData.Default.weapons[Mathf.Min((int)GameData.Default.GetUpgrade(UpgradeType.Years).CurValue / 50, GameData.Default.weapons.Length - 1)];
        weaponMesh.transform.rotation = newWeapon.meshFilter.transform.rotation;
        weaponMesh.sharedMesh = newWeapon.meshFilter.sharedMesh;
        weaponRenderer.sharedMaterial = newWeapon.thisRenderer.sharedMaterial;
    }
}
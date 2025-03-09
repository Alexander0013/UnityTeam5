using System.Collections.Generic;
using UnityEngine;

public class WeaponTestList : MonoBehaviour
{
    public WeaponController weaponController;
    // No need for separate newWeaponPrefab variable, since our list is in WeaponController.
    // Instead, assign the list of weapon prefabs in the WeaponController in the Inspector.

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponController.playerAttackData.currentWeaponIndex = 0;
            weaponController.SwitchWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponController.playerAttackData.currentWeaponIndex = 1;
            weaponController.SwitchWeapon();
        }
    }
}

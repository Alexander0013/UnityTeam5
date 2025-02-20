using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    // Assign your weapon GameObject via the Inspector
    public GameObject weapon;

    public void ShowWeapon() {
        weapon.SetActive(true);
    }

    public void HideWeapon() {
        weapon.SetActive(false);
    }
}

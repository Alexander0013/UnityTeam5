using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // Assign these in the Inspector.
    public GameObject idleWeapon;   // Weapon floating behind the player (idle)
    public GameObject attackWeapon; // Weapon held in hand (attack)

    // Called in IdleState to show the floating weapon.
    public void ShowIdleWeapon()
    {
        if (idleWeapon != null) idleWeapon.SetActive(true);
        if (attackWeapon != null) attackWeapon.SetActive(false);
    }

    // Called in AttackState to show the in-hand weapon.
    public void ShowAttackWeapon()
    {
        if (attackWeapon != null) attackWeapon.SetActive(true);
        if (idleWeapon != null) idleWeapon.SetActive(false);
    }

    // Called in movement states (Walk, Run, Jump) to hide both weapons.
    public void HideBothWeapons()
    {
        if (idleWeapon != null) idleWeapon.SetActive(false);
        if (attackWeapon != null) attackWeapon.SetActive(false);
    }
    // Optional: Hide the attack weapon (for cleanup).
    public void HideAttackWeapon()
    {
        if (attackWeapon != null) attackWeapon.SetActive(false);
    }

    public void HideIdleWeapon()
    {
        if (idleWeapon != null) idleWeapon.SetActive(false);
    }
}

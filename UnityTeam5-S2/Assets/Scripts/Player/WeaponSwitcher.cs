using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public Transform weaponHolder;  // The parent object for weapons
    public GameObject newWeaponPrefab;  // The new weapon prefab

    private GameObject currentWeapon;

    void Start() {
        // Initialize currentWeapon with the first child under weaponHolder if available.
        if (weaponHolder.childCount > 0) {
            currentWeapon = weaponHolder.GetChild(0).gameObject;
        } else {
            Debug.LogWarning("No weapon found in weaponHolder at Start!");
        }
    }

    public void SwitchWeapon()
    {
        if(currentWeapon == null) {
            Debug.LogWarning("Current weapon is null. Instantiating new weapon directly.");
            currentWeapon = Instantiate(newWeaponPrefab, weaponHolder);
            return;
        }

        // Save the current weapon's local transform values.
        Vector3 storedLocalPos = currentWeapon.transform.localPosition;
        Quaternion storedLocalRot = currentWeapon.transform.localRotation;

        // Remove the old weapon.
        Destroy(currentWeapon);

        // Instantiate the new weapon as a child of the weaponHolder.
        currentWeapon = Instantiate(newWeaponPrefab, weaponHolder);

        // Apply the stored transform values.
        currentWeapon.transform.localPosition = storedLocalPos;
        currentWeapon.transform.localRotation = storedLocalRot;
    }
}

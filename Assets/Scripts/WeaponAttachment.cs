using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    // Reference to your hand socket (assign via the Inspector)
    public Transform rightHandSocket;
    // The weapon prefab you want to equip
    public GameObject weaponPrefab;
    private GameObject currentWeapon;

    public void EquipWeapon()
    {
        if (weaponPrefab != null && rightHandSocket != null)
        {
            // Instantiate and parent the weapon to the hand socket
            currentWeapon = Instantiate(weaponPrefab, rightHandSocket);
            // Reset local position/rotation so it aligns with the socket
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }
}

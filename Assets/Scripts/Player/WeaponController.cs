using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Gender playerGender;

    [Header("Weapons References")]
    // List of weapon prefabs stored in the Inspector.
    public List<GameObject> weaponPrefabs = new List<GameObject>();
    // The currently active weapon instance.
    public GameObject currentWeapon;

    [Header("Attachment Points")]
    public Transform idleAttach;   // e.g., HipAttach
    public Transform attackAttach; // e.g., HandAttach

    [Header("Slash VFX Spawn Point")]
    public Transform slashSpawn;   // Should be set on your weapon prefab.

    
    [HideInInspector]
    public AttackData playerAttackData;
    void Awake() 
    {
        PlayerHealth ph = GetComponentInParent<PlayerHealth>();
        if(ph != null)
            playerAttackData = ph.playerAttackData;
    }
    void OnEnable() 
    {
        InventoryManager.instance.onEquipmentChanged += OnEquipmentChanged;
    }
    void OnDisable() 
    {
        InventoryManager.instance.onEquipmentChanged -= OnEquipmentChanged;
    }
    private void OnEquipmentChanged(Equipment newItem, Equipment oldItem, int genderIndex)
    {
        // Process only if this change is for a weapon and matches our player's gender.
        if (playerGender == Gender.Female && genderIndex != 0)
            return;
        if (playerGender == Gender.Male && genderIndex != 1)
            return;

        if (newItem != null && newItem.type == EquipmentType.Weapon)
        {
            if (oldItem != null)
                playerAttackData.baseDamage -= oldItem.damageModifier;
            playerAttackData.baseDamage += newItem.damageModifier;

            int newWeaponIndex = (playerGender == Gender.Female) ? 0 : 1;
            SwitchWeapon(newWeaponIndex);
        }
    }
    
    // Show the current weapon.
    public void ShowWeapon()
    {
        if (currentWeapon != null)
            currentWeapon.SetActive(true);
    }

    // Hide the current weapon.
    public void HideWeapon()
    {
        if (currentWeapon != null)
            currentWeapon.SetActive(false);
    }

    // Attach the current weapon to the idle (hip) attachment.
    public void AttachWeaponToIdle()
    {
        if (currentWeapon == null || idleAttach == null)
            return;

        currentWeapon.transform.SetParent(idleAttach);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        //Debug.Log("[WeaponController] Attached to Idle position.");
    }

    // Attach the current weapon to the attack (hand) attachment.
    public void AttachWeaponToAttack()
    {
        if (currentWeapon == null || attackAttach == null)
            return;

        currentWeapon.transform.SetParent(attackAttach);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        //Debug.Log("[WeaponController] Attached to Attack position.");
    }

    // Fade in the current weapon.
    public IEnumerator FadeInWeapon(float duration, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        if (currentWeapon != null)
        {
            ShowWeapon();
            Renderer rend = currentWeapon.GetComponent<Renderer>();
            if (rend != null)
            {
                float timer = 0f;
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    float dissolve = Mathf.Lerp(1f, 0f, timer / duration);
                    rend.material.SetFloat("_DissolveAmount", dissolve);
                    yield return null;
                }
                rend.material.SetFloat("_DissolveAmount", 0f);
            }
        }
    }

    // Fade out the current weapon.
    public IEnumerator FadeOutWeapon(float duration, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        if (currentWeapon != null)
        {
            Renderer rend = currentWeapon.GetComponent<Renderer>();
            if (rend != null)
            {
                float timer = 0f;
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    float dissolve = Mathf.Lerp(0f, 1f, timer / duration);
                    rend.material.SetFloat("_DissolveAmount", dissolve);
                    yield return null;
                }
                rend.material.SetFloat("_DissolveAmount", 1f);
            }
            HideWeapon();
        }
    }

    /// <summary>
    /// Switches the current weapon based on the currentWeaponIndex in playerAttackData.
    /// It fades out and destroys the old weapon, then instantiates the new weapon prefab
    /// as a child of the idle attachment point.
    /// </summary>
    public void SwitchWeapon(int newWeaponIndex)
    {
        if (playerAttackData == null)
        {
            return;
        }
        // destroy current weapon.
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }
        playerAttackData.currentWeaponIndex = newWeaponIndex;

        if (weaponPrefabs != null || weaponPrefabs.Count > newWeaponIndex)
        {
            if (idleAttach != null)
            {
                // Instantiate the new weapon as a child of idleAttach.
                GameObject newWeapon = Instantiate(weaponPrefabs[newWeaponIndex], idleAttach);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localRotation = Quaternion.identity;
                newWeapon.transform.localScale = Vector3.one;
                currentWeapon = newWeapon;

                //Debug.Log("[WeaponController] Switched to new weapon at index " + index);
                StartCoroutine(FadeInWeapon(0.2f));

                // Update the transparency controller so it caches the new weapon's renderer.
                PlayerTransparencyController ptc = GetComponentInParent<PlayerTransparencyController>();
                ptc?.UpdateRenderers();
            }
        }
        else
        {
            Debug.LogWarning("[WeaponController] IdleAttach is not assigned!");
        }
    }
}

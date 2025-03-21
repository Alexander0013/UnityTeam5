using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapons References")]
    // List of weapon prefabs stored in the Inspector.
    public List<GameObject> weaponPrefabs = new List<GameObject>();

    // The current weapon index is stored in the AttackData ScriptableObject.
    // Make sure this same AttackData asset is assigned in all scenes.
    public AttackData playerAttackData;

    // The currently active weapon instance.
    public GameObject currentWeapon;

    [Header("Attachment Points")]
    public Transform idleAttach;   // e.g., HipAttach
    public Transform attackAttach; // e.g., HandAttach

    [Header("Slash VFX Spawn Point")]
    public Transform slashSpawn;   // Should be set on your weapon prefab.

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
    public void SwitchWeapon()
    {
        if (playerAttackData == null)
        {
            //Debug.LogWarning("[WeaponController] PlayerAttackData is not assigned.");
            return;
        }
        int index = playerAttackData.currentWeaponIndex;

        if (weaponPrefabs == null || weaponPrefabs.Count <= index)
        {
            //Debug.LogWarning("[WeaponController] No weapon prefab found at index: " + index);
            return;
        }

        // Fade out and destroy current weapon.
        if (currentWeapon != null)
        {
            StartCoroutine(FadeOutWeapon(0.2f));
            Destroy(currentWeapon, 0.25f);
        }

        if (idleAttach != null)
        {
            // Instantiate the new weapon as a child of idleAttach.
            GameObject newWeapon = Instantiate(weaponPrefabs[index], idleAttach);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;
            newWeapon.transform.localScale = Vector3.one;
            currentWeapon = newWeapon;

            //Debug.Log("[WeaponController] Switched to new weapon at index " + index);
            StartCoroutine(FadeInWeapon(0.2f));

            // Update the transparency controller so it caches the new weapon's renderer.
            PlayerTransparencyController ptc = GetComponentInParent<PlayerTransparencyController>();
            if (ptc != null)
            {
                ptc.UpdateRenderers();
            }
        }
        else
        {
            Debug.LogWarning("[WeaponController] IdleAttach is not assigned!");
        }
    }
}

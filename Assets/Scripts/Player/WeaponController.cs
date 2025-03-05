using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Single Weapon Reference")]
    public GameObject weapon; // The single weapon prefab.

    [Header("Attachment Points")]
    public Transform idleAttach;   // e.g., HipAttach
    public Transform attackAttach; // e.g., HandAttach

    public Transform slashSpawn; // Assign this in the Inspector on your weapon prefab.

    // Show / Hide the entire weapon GameObject
    public void ShowWeapon()
    {
        if (weapon != null) 
        {
            weapon.SetActive(true);
        }
    }

    public void HideWeapon()
    {
        if (weapon != null) 
        {
            weapon.SetActive(false);
        }
    }

    // Switch to idle position (hip)
    public void AttachWeaponToIdle()
    {
        if (weapon == null || idleAttach == null) return;
        weapon.transform.SetParent(idleAttach);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;

        Debug.Log("[WeaponController] Attached to Idle position.");
    }

    // Switch to attack position (hand)
    public void AttachWeaponToAttack()
    {
        if (weapon == null || attackAttach == null) return;
        weapon.transform.SetParent(attackAttach);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;

        Debug.Log("[WeaponController] Attached to Attack position.");
    }

    // Example fade in/out routines for a single weapon
    public IEnumerator FadeInWeapon(float duration, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        if (weapon != null)
        {
            ShowWeapon(); // Ensure it's active
            Renderer rend = weapon.GetComponent<Renderer>();
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

    public IEnumerator FadeOutWeapon(float duration, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        if (weapon != null)
        {
            Renderer rend = weapon.GetComponent<Renderer>();
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
}

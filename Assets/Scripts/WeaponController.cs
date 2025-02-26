using System.Collections;
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
    //Hide the attack weapon (for cleanup).
    public void HideAttackWeapon()
    {
        if (attackWeapon != null) attackWeapon.SetActive(false);
    }

    public void HideIdleWeapon()
    {
        if (idleWeapon != null) idleWeapon.SetActive(false);
    }
    //Fade In idle weapon (dissolve from fully dissolved to visible).
    public IEnumerator FadeInIdleWeapon(float duration, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        if (idleWeapon != null)
        {
            idleWeapon.SetActive(true);
            Renderer rend = idleWeapon.GetComponent<Renderer>();
            if (rend != null)
            {
                // Assume the material has _DissolveAmount property: 1 = invisible, 0 = visible.
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
    
    //Fade Out idle weapon (dissolve from visible to fully dissolved).
    public IEnumerator FadeOutIdleWeapon(float duration, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        if (idleWeapon != null)
        {
            Renderer rend = idleWeapon.GetComponent<Renderer>();
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
            idleWeapon.SetActive(false);
        }
    }
}

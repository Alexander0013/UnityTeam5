using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 10f;
    public GameObject deathEffectPrefab;
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {health}");
        
        StartCoroutine(GetHit());

        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }
    IEnumerator GetHit()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("getHit", true);
        yield return new WaitForSeconds(0.53f);
        animator.SetBool("getHit", false);
    }


    IEnumerator Die()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Die");

        // wait for 1.2 seconds for animation finished
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);

        //生成死亡特效
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}

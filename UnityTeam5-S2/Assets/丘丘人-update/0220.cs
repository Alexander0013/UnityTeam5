using UnityEngine;
using System.Collections;

public class ChurlController : MonoBehaviour
{
    private Animator animator;
    private bool isTakingDamage=false;
    public GameObject deathEffectPrefab; // 砞竚疭
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("getHit", false);
    }

    void Update()
    {
        // フ龄秨﹍计
        if (Input.GetKeyDown(KeyCode.Space)&&!isTakingDamage)
        {
            StartCoroutine(TakeDamageSequence());
        }
    }

    IEnumerator TakeDamageSequence()
    {
        isTakingDamage = true;
        // ち传阑笆礶
        animator.SetBool("getHit", true);

        // 单5
        yield return new WaitForSeconds(3f);

        // ち传笆礶
        animator.SetBool("getHit", false);
        animator.SetTrigger("Die");

        // 单0.5綪反
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);

        //ネΘ疭
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}

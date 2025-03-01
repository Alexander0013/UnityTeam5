using UnityEngine;
using System.Collections;

//public class churlBattle : churlBase
//{
//    public GameObject deathEffectPrefab;
//    private Animator animator;
//    private bool isTakingDamage = false;
//    void Start()
//    {
//        animator = GetComponent<Animator>();
//        animator.SetBool("getHit", false);
//    }

//    void Update()
//    {
//        // フ龄秨﹍计
//        if (Input.GetKeyDown(KeyCode.Space) && !isTakingDamage)
//        {
//            StartCoroutine(TakeDamageSequence());
//        }
//    }

//    IEnumerator TakeDamageSequence()
//    {
//        isTakingDamage = true;
//        // ち传阑笆礶
//        animator.SetBool("getHit", true);

//        // 单5
//        yield return new WaitForSeconds(3f);

//        // ち传笆礶
//        animator.SetBool("getHit", false);
//        animator.SetTrigger("Die");

//        // 单0.5綪反
//        yield return new WaitForSeconds(1f);
//        Destroy(gameObject);

//        //ネΘ疭
//        if (deathEffectPrefab != null)
//        {
//            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
//        }
//    }
    //void Start()
    //{       
    //    animator = GetComponent<Animator>();
    //    animator.SetBool("getHit", false);
    //}

    //public void TakeDamage(int damage)
    //{
    //    if (!isTakingDamage)
    //    {
    //        StartCoroutine(TakeDamageSequence(damage));
    //    }
    //}

    //private IEnumerator TakeDamageSequence(int damage)
    //{
    //    isTakingDamage = true;
    //    health -= damage;
    //    animator.SetBool("getHit", true);

    //    yield return new WaitForSeconds(1f); 
    //    animator.SetBool("getHit", false);

    //    if (health <= 0)
    //    {
    //        Die();
    //    }

    //    isTakingDamage = false;
    //}
    //private void Die()
    //{
    //    animator.SetTrigger("Die");
    //    StartCoroutine(DestroyAfterAnimation());
    //}

    //private IEnumerator DestroyAfterAnimation()
    //{
    //    yield return new WaitForSeconds(1f);

    //    if (deathEffectPrefab != null)
    //    {
    //        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    //    }
    //    Destroy(gameObject);
    //}
//}

    


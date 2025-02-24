using UnityEngine;
using System.Collections;

public class StoneHideController : MonoBehaviour
{
    private Animator animator;
    private bool isTakingDamage = false;
    public GameObject deathEffectPrefab; // �]�m���`�S��
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("getHit", false);
    }

    void Update()
    {
        // ���U�ť���}�l�˼�
        if (Input.GetKeyDown(KeyCode.Space) && !isTakingDamage)
        {
            StartCoroutine(TakeDamageSequence());
        }
    }

    IEnumerator TakeDamageSequence()
    {
        isTakingDamage = true;
        // ����������ʵe
        animator.SetBool("Hit", true);

        // ����5��
        yield return new WaitForSeconds(3f);

        // �����즺�`�ʵe
        animator.SetBool("Hit", false);
        animator.SetTrigger("Die");

        // ����0.5����P��
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);

        //�ͦ����`�S��
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}

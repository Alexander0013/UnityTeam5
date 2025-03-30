using UnityEngine;
using System.Collections;

public class BossSwipingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss �i�J Swiping ���A");
        if (boss.playerTarget != null)
        {
            boss.StartCoroutine(SmoothRotateToTarget(boss, boss.playerTarget.position, 0.3f));
        }
        // ���M���έ��m���ݭn���Ѽ�
        boss.animator.ResetTrigger("Walk");
        boss.animator.SetBool("Walk", false);
        boss.animator.SetBool("isSwiping", false);
        boss.animator.ResetTrigger("Dance");
        // ���ƹL��� Swiping �ʵe
        boss.animator.CrossFade("Swiping", 0.1f, 0, 0f);

        // �]�m swiping �Ѽơ]�T�O�b�L�秹����^
        boss.animator.SetBool("isSwiping", true);
    }
    private IEnumerator SmoothRotateToTarget(BossFSM boss, Vector3 targetPos, float duration)
    {
        float elapsed = 0f;
        Quaternion startRot = boss.transform.rotation;
        // �Ȧb����������
        Vector3 direction = (targetPos - boss.transform.position).normalized;
        direction.y = 0f;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        while (elapsed < duration)
        {
            boss.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.transform.rotation = targetRot;
    }
    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            Debug.Log("���a���}�����d��A�����^ Walk ���A");
            boss.TransitionToState(boss.walkState);
            
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss ���} Swiping ���A");
        boss.animator.SetBool("isSwiping", false);
    }

    // �Ѱʵe�ƥ�I�s�A�ˬd�O�_�R�����a
    public void OnAttackHit(BossFSM boss)
    {
        boss.ApplyAttackDamage();
  
    }

    // �����ʵe������A�^�� StandBy ���A�� Boss �M�w�U�@�B�ʧ@
    public void OnAttackAnimationFinished(BossFSM boss)
    {
        boss.StartCoroutine(DelayedSwipingTransition(boss));
    }
    private IEnumerator DelayedSwipingTransition(BossFSM boss)
    {
        // ����0.2���A�T�O�ʵe���������B Animator �Ѽ�í�w
        yield return new WaitForSeconds(0.2f);
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);
        if (distance <= boss.attackRadius)
        {
            Debug.Log("���a���b swiping �d��A���� swiping");
            boss.TransitionToState(boss.swipingState);
        }
        else
        {
            Debug.Log("���a�w���} swiping �d��A��^ WalkState");
            boss.TransitionToState(boss.walkState);
        }
    }
  
}
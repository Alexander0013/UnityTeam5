    using UnityEngine;
    using System.Collections;

    public class BossAttackState : BossBaseState
    {
        private float attackCooldown = 1f; 

        public override void EnterState(BossFSM boss)
        {
            Debug.Log("Enter Attack State");
            boss.animator.SetBool("isAttacking", true);
            boss.animator.SetBool("isWalking", false);
        }

        public override void UpdateState(BossFSM boss)
        {
            //if(boss.playerTarget == null) return;
            float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

            // If out of attack range, do something else
            if (distance > boss.attackRadius)
            {
                Debug.Log("玩家在攻擊範圍外，chase");
                boss.TransitionToState(boss.chaseState);
                return;
            }
            else
            {
                Debug.Log("玩家在攻擊範圍內，performAttack");
                boss.StartCoroutine(PerformAttack(boss));
            }
        }

        public override void ExitState(BossFSM boss)
        {
            boss.animator.SetBool("isAttacking", false);
        }
    private IEnumerator PerformAttack(BossFSM boss)
    {
        // 觸發攻擊動畫
        boss.animator.SetTrigger("Swiping");
        yield return new WaitForSeconds(0.2f);
    }

    //private IEnumerator PerformAttack(BossFSM boss)
    //{
    //    // Trigger animation
    //    boss.animator.SetTrigger("Swiping");
    //    yield return new WaitForSeconds(1f);
    //    // Delay to line up with the animation's impact frame
    //    //yield return new WaitForSeconds(2.667f);

    //    //// OverlapSphere or direct check for your "old logic" of dealing damage
    //    //float damage = boss.npcData.baseDamage * boss.npcData.comboMultiplier;
    //    //float radius = boss.npcData.hitRadius; // or boss.attackRadius, whichever you used
    //    //// Use the attackHitPoint (weapon position) as the center for hit detection.
    //    //Vector3 attackCenter = boss.attackHitPoint.position;
    //    //// Example OverlapSphere
    //    //Collider[] hits = Physics.OverlapSphere(attackCenter, radius, boss.npcData.playerLayers);
    //    //foreach (Collider c in hits)
    //    //{
    //    //    IDamageable dmg = c.GetComponent<IDamageable>();
    //    //    if (dmg != null)
    //    //    {
    //    //        dmg.TakeDamage(damage);
    //    //    }
    //    //}


    //    //isAttacking = false;
    //    //// Could go directly back to idle or do random logic again
    //    //boss.TransitionToState(boss.idleState);
    //}

    /// <summary>
    /// Finds any GameObject(s) tagged "Player" that is activeInHierarchy and has PlayerHealth > 0.
    /// Returns the first valid player's transform, or null if none found.
    /// </summary>
    private Transform FindActiveLivingPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                if (p.activeInHierarchy)
                {
                    PlayerHealth ph = p.GetComponent<PlayerHealth>();
                    if (ph != null && ph.CurrentHealth > 0)
                    {
                        return p.transform;
                    }
                }
            }
            return null;
        }
        public void OnAttackHit(BossFSM boss)
        {
            // Apply damage only if still in attack state
            boss.ApplyAttackDamage();
        }
        // Called by an Animation Event at the end of the attack animation.
        public void OnAttackAnimationFinished(BossFSM boss)
        {

            Debug.Log("Leaving BossAttack State via animation event");
            boss.TransitionToState(boss.idleState);
        }
    }

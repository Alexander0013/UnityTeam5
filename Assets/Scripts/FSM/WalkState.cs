using UnityEngine;
using StarterAssets;

public class WalkState : PlayerBaseState
{
    private float walkTimer = 0f;
    private bool weaponHidden = false;

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Walk State");
        walkTimer = 0f;
        weaponHidden = false;
        if (player.Animator != null)
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0.5f);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        walkTimer += Time.deltaTime;
        if (!weaponHidden && walkTimer >= 2f)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.HideBothWeapons();
                Debug.Log("Both weapons hidden after 2 seconds in Walk State.");
                weaponHidden = true;
            }
        }
        
        if (player.Input.move == Vector2.zero)
        {
            player.SwitchState(new IdleState());
        }
        else if (player.Input.sprint)
        {
            player.SwitchState(new RunState());
        }
        else if (player.Input.jump)
        {
            player.SwitchState(new JumpState());
        }
        else if (player.Input.attack)
        {
            Debug.Log("Switching to Attack State from Walk");
            player.SwitchState(new AttackState());
            player.Input.attack = false;
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Optional cleanup.
    }
}

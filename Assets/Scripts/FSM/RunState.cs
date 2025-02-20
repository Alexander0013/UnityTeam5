using UnityEngine;
using StarterAssets;

public class RunState : PlayerBaseState
{
    private float runTimer = 0f;
    private bool weaponHidden = false;

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Run State");
        runTimer = 0f;
        weaponHidden = false;
        if (player.Animator != null)
            player.Animator.SetFloat("Speed", 1f);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        runTimer += Time.deltaTime;
        if (!weaponHidden && runTimer >= 2f)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.HideWeapon();
                Debug.Log("Weapon hidden after 2 seconds in Run State.");
                weaponHidden = true;
            }
        }
        
        if (player.Input.move == Vector2.zero)
        {
            player.SwitchState(new IdleState());
        }
        else if (!player.Input.sprint)
        {
            player.SwitchState(new WalkState());
        }
        else if (player.Input.jump)
        {
            player.SwitchState(new JumpState());
        }
        else if (player.Input.attack)
        {
            player.SwitchState(new AttackState());
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Optional cleanup.
    }
}

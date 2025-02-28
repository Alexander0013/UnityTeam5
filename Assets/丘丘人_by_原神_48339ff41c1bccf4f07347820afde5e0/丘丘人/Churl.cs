using UnityEngine.Rendering;
using UnityEngine;

public class  Churl : MonoBehaviour
{
    private ChurlBase currentState;
    private ChurlPatrolSrate patrolState;
    //private CombatState combatState;
    //private DeathState deathState;

    private Rigidbody rb;

    void Start()
    {
        patrolState = new ChurlPatrolSrate(this);
        //combatState = new CombatState(this);
        //deathState = new DeathState(this);

        rb = GetComponent<Rigidbody>();

        // 設定初始狀態為巡邏
        ChangeState(patrolState);
    }

    void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(ChurlBase newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("churl"))
    //    {
    //        // 進入戰鬥狀態
    //        ChangeState(combatState);
    //    }
    //}

    //public void Die()
    //{
    //    ChangeState(deathState);
    //}
}


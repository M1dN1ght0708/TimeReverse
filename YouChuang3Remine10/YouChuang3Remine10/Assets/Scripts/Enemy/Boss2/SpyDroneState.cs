using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpyDroneStateEnum
{
    Idle,
    Patrol,
    Hurt,
    Dead,
}
public class SpyDroneIdle : BaseState
{
    private SpyDrone spyDrone;
    private float idleTimeCounter;
    public override void OnEnter(Enemy enemy)
    {
        spyDrone = enemy as SpyDrone;
        spyDrone.animator.Play("Idle");
        this.idleTimeCounter = spyDrone.idleTime;
    }
    public override void LogicUpdate()
    {

        if (spyDrone.isDead)
        {
            spyDrone.SwitchState(SpyDroneStateEnum.Dead);
            return;
        }
        if (spyDrone.beStop)
            return;
        idleTimeCounter -= Time.deltaTime;
        if (spyDrone.isHurt)
        {
            spyDrone.SwitchState(SpyDroneStateEnum.Hurt);
            return;
        }
        if (idleTimeCounter <= 0)
        {
            spyDrone.SwitchState(SpyDroneStateEnum.Patrol);
        }
    }
    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {

    }


}
public class SpyDronePatrol : BaseState
{
    private SpyDrone spyDrone;
    private Vector3 targetPos;
    private AnimatorStateInfo info;
    private bool canMove;
    private Vector2 moveDir;
    public override void OnEnter(Enemy enemy)
    {
        spyDrone = enemy as SpyDrone;
        targetPos = spyDrone.GetNewPoint();
        Debug.Log(targetPos);
        spyDrone.FlipTo(targetPos);
        spyDrone.animator.Play("StartMove");
    }
    public override void LogicUpdate()
    {
        if (spyDrone.isDead)
        {
            spyDrone.SwitchState(SpyDroneStateEnum.Dead);
            return;
        }
        if (spyDrone.beStop)
            return;
        if (spyDrone.isHurt)
        {
            spyDrone.SwitchState(SpyDroneStateEnum.Hurt);
            return;
        }
        info = spyDrone.animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("StartMove") && info.normalizedTime >= 0.95f)
        {
            canMove = true;
        }

        if (canMove)
        {
            spyDrone.animator.Play("MoveDrone");
            this.Move();
        }
        if (Mathf.Abs(targetPos.x - spyDrone.transform.position.x) < 0.1f && Mathf.Abs(targetPos.y - spyDrone.transform.position.y) < 0.1f)
        {
            canMove = false;
            spyDrone.animator.Play("EndMove");
            if (info.IsName("EndMove") && info.normalizedTime >= 0.95f)
                spyDrone.SwitchState(SpyDroneStateEnum.Idle);
        }
    }
    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        canMove = false;
    }

    private void Move()
    {
        moveDir = (targetPos - spyDrone.transform.position).normalized;
        spyDrone.transform.Translate(moveDir * spyDrone.patrolSpeed * Time.deltaTime, Space.World);
    }
}
public class SpyDroneHurt : BaseState
{
    private SpyDrone spyDrone;
    private AnimatorStateInfo info;
    public override void OnEnter(Enemy enemy)
    {
        spyDrone = enemy as SpyDrone;
        spyDrone.animator.Play("Hurt");
    }
    public override void LogicUpdate()
    {
        if (spyDrone.isDead)
        {
            spyDrone.SwitchState(SpyDroneStateEnum.Dead);
            return;
        }
        if (spyDrone.beStop)
            return;
        info = spyDrone.animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Hurt") && info.normalizedTime >= 0.95f)
        {
            spyDrone.SwitchState(SpyDroneStateEnum.Patrol);
        }
    }
    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        spyDrone.isHurt = false;
    }
}

public class SpyDroneDead : BaseState
{
    private SpyDrone spyDrone;
    public override void OnEnter(Enemy enemy)
    {
        spyDrone = enemy as SpyDrone;
        spyDrone.animator.Play("Die");
    }
    public override void LogicUpdate()
    {

    }
    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        spyDrone.isDead = false;
    }
}
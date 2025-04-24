using BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;

namespace Boos2Tasks
{
    public class StageSkillTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;
        private Boss2Character b2Character;
        private bool isAttack;
        private bool hasStageTwo;

        private Transform playerTrans;
        private Vector3 targetPos;
        private float deltaTime;
        private Vector3 moveDir;
        private bool hasTrigger;
        public StageSkillTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = b2Trans.GetComponent<Animator>();
            this.b2Character=b2Trans.GetComponent<Boss2Character>();
            this.playerTrans = b2Tree.playerTrans;
            this.deltaTime = b2Tree.skill2DeltaTime;
        }
        public override NodeState Evaluate()
        {
            if(Boss2Character.hasStageTwo||b2Character.currentHp>b2Character.maxHp/2||b2Tree.skillID!=0)
            {
                return NodeState.Failure;
            }
            if ((Mathf.Abs(b2Tree.stageSkillTarget.position.x - this.b2Trans.position.x) < 0.5f
                && Mathf.Abs(b2Tree.stageSkillTarget.position.y - this.b2Trans.position.y) < 0.5f) ||
                isAttack)
            {               
                if (moveDir.x == 0)
                {
                    if (!isAttack)
                    {
                        b2Tree.animator.Play("PlaneUptoIdle");
                        isAttack = true;
                    }
                }
                else
                {
                    if (!isAttack)
                    {
                        b2Tree.animator.Play("PlaneLefttoIdle");
                        isAttack = true;
                    }
                }
                if (!Boss2Character.isStageTwo)
                {
                    Boss2Character.isStageTwo = true;
                }
                if (!hasTrigger)
                {
                    hasTrigger = true;
                    this.StageSkill();
                }               
            }
            else
            {
                if (!isAttack)
                    this.MoveToTarget();
            }
            return NodeState.Success;
        }
        private void MoveToTarget()
        {
            moveDir = (b2Tree.stageSkillTarget.position - this.b2Trans.position).normalized;
            b2Trans.Translate(moveDir * b2Tree.stageSkillMoveSpeed * Time.deltaTime, Space.Self);
            if (moveDir.x < 0)
            {
                b2Tree.sprite.flipX = false;
                b2Tree.animator.Play("PlaneLeft");
            }
            else if (moveDir.x > 0)
            {
                b2Tree.sprite.flipX = true;
                b2Tree.animator.Play("PlaneLeft");
            }
            else
            {
                b2Tree.animator.Play("PlaneUp");
            }
        }
        private void StageSkill()
        {
            b2Tree.ShowWarn();
        }
    }     
    public class MoveTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;
        private float distance;

        private Vector3 moveDir;
        private Vector3 targetPos;
        private int targetIndex;
        private Vector3 moveTarget;
        private bool hasTarget;

        public MoveTask() { }
        public MoveTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = this.b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = this.b2Trans.GetComponent<Animator>();
        }
        public override NodeState Evaluate()
        {
            /*this.Move();
            if (Mathf.Abs(targetPos.x - this.b2Trans.position.x) < 0.5f && Mathf.Abs(targetPos.y - this.b2Trans.position.y) < 0.5f)
            {
                b2Tree.skillID = 0;
                b2Tree.canMove = false;
                b2Tree.nowIndex = this.targetIndex;
                if(moveDir.x==0)
                {
                    b2Tree.animator.Play("PlaneDowntoIdle");
                }
                else
                {
                    b2Tree.animator.Play("PlaneLefttoIdle");
                }

            }*/
            if (Boss2Character.isStageTwo && !Boss2Character.hasStageTwo)
                return NodeState.Failure;
            this.TracePlayerMove();
            if ((Mathf.Abs(moveTarget.x - this.b2Trans.position.x) < 0.5f
                && Mathf.Abs(moveTarget.y - this.b2Trans.position.y) < 0.5f))
            {
                if (moveDir.x != 0)
                {
                    b2Tree.animator.Play("PlaneLefttoIdle");
                }
                else if (moveDir.x == 0 && moveDir.y > 0)
                {
                    b2Tree.animator.Play("PlaneUptoIdle");
                }
                else if (moveDir.x == 0 && moveDir.y < 0)
                {
                    b2Tree.animator.Play("PlaneDowntoIdle");
                }
                b2Tree.skillID = 0;
                b2Tree.canMove = false;
                this.hasTarget = false;
            }

            return NodeState.Running;
        }

        private void TracePlayerMove()
        {
            if (!b2Tree.canMove)
                return;
            if (!hasTarget)
            {
                moveTarget = new Vector3(b2Tree.playerTrans.position.x, b2Tree.playerTrans.position.y + 5, 0);
                hasTarget = true;
            }
            moveDir = (moveTarget - this.b2Trans.position).normalized;
            b2Trans.Translate(moveDir * b2Tree.skill2MoveSpeed * Time.deltaTime, Space.Self);
            if (moveDir.x < 0)
            {
                b2Tree.sprite.flipX = false;
                b2Tree.animator.Play("PlaneLeft");
            }
            else if (moveDir.x > 0)
            {
                b2Tree.sprite.flipX = true;
                b2Tree.animator.Play("PlaneLeft");
            }
            else if (moveDir.x == 0 && moveDir.y > 0)
            {
                b2Tree.animator.Play("PlaneUp");
            }
            else if (moveDir.x == 0 && moveDir.y < 0)
            {
                b2Tree.animator.Play("PlaneDown");
            }
        }
        private void Move()
        {
            if (b2Tree.canMove)
            {
                while (this.targetIndex == b2Tree.nowIndex)
                {
                    this.targetIndex = UnityEngine.Random.Range(0, 5);
                }
                b2Tree.targetIndex = this.targetIndex;
                this.targetPos = b2Tree.targets[this.targetIndex].position;
                //移动：
                moveDir = (targetPos - this.b2Trans.position).normalized;
                b2Trans.Translate(moveDir * b2Tree.speed * Time.deltaTime, Space.Self);
                //根据方向不同播放移动动画：
                if (moveDir.x < 0)
                {
                    b2Tree.sprite.flipX = false;
                    b2Tree.animator.Play("PlaneLeft");
                }
                else if (moveDir.x > 0)
                {
                    b2Tree.sprite.flipX = true;
                    b2Tree.animator.Play("PlaneLeft");
                }
                else
                {
                    b2Tree.animator.Play("PlaneDown");
                }

            }
        }

    }
    public class SkillIdTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;


        private Transform playerTrans;
        private Vector3 targetPos;
        private float aimTime;

        public SkillIdTask() { }
        public SkillIdTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = b2Trans.GetComponent<Animator>();
            this.playerTrans = b2Tree.playerTrans;
        }
        public override NodeState Evaluate()
        {
            if (Boss2Character.isStageTwo && !Boss2Character.hasStageTwo)
                return NodeState.Failure;
            if (b2Tree.canMove)
            {
                b2Tree.skillID = 0;
                return NodeState.Failure;
            }
            if (b2Tree.skillID == 0)
            {
                int skillID = UnityEngine.Random.Range(1, 41);
                //Debug.Log("SkillID: "+skillID);
                if (skillID <= 10)
                    b2Tree.skillID = 1;
                else if (skillID <= 21 && skillID > 10)
                    b2Tree.skillID = 2;
                else if (skillID <= 31 && skillID > 20)
                    b2Tree.skillID = 3;
                else
                    b2Tree.skillID = 4;
                if (b2Tree.testSkillID != 0)
                    b2Tree.skillID = b2Tree.testSkillID;
            }

            return NodeState.Success;
        }

    }
    public class SkillOneTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;


        private Transform playerTrans;
        private Vector3 targetPos;
        private float aimTime;
        private Vector3 moveTarget;
        private Vector3 moveDir;
        private bool isTrace;
        private bool isAim;


        public SkillOneTask() { }
        public SkillOneTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = b2Trans.GetComponent<Animator>();
            this.playerTrans = b2Tree.playerTrans;

        }
        public override NodeState Evaluate()
        {
            if (Boss2Character.isStageTwo && !Boss2Character.hasStageTwo)
                return NodeState.Failure;
            if (b2Tree.skillID != 1)
                return NodeState.Failure;

            if ((Mathf.Abs(moveTarget.x - this.b2Trans.position.x) < 0.5f
                && Mathf.Abs(moveTarget.y - this.b2Trans.position.y) < 0.5f)
                || isAim)
            {
                if (!isAim)
                {
                    isAim = true;
                    if (moveDir.x != 0)
                    {
                        b2Tree.animator.Play("PlaneLefttoIdle");
                    }
                    else if (moveDir.x == 0 && moveDir.y > 0)
                    {
                        b2Tree.animator.Play("PlaneUptoIdle");
                    }
                    else if (moveDir.x == 0 && moveDir.y < 0)
                    {
                        b2Tree.animator.Play("PlaneDowntoIdle");
                    }
                }
                if (isAim)
                    this.AimPlayer();
            }
            else
            {
                if (!isAim)
                    this.SkillOneMove();
            }
            return NodeState.Success;
        }

        private void SkillOneMove()
        {
            if (!isTrace)
            {
                moveTarget = new Vector3(b2Tree.playerTrans.position.x, b2Tree.playerTrans.position.y + 5, 0);
                isTrace = true;
            }
            moveDir = (moveTarget - this.b2Trans.position).normalized;
            b2Trans.Translate(moveDir * b2Tree.skill2MoveSpeed * Time.deltaTime, Space.Self);
            if (moveDir.x < 0)
            {
                b2Tree.sprite.flipX = false;
                b2Tree.animator.Play("PlaneLeft");
            }
            else if (moveDir.x > 0)
            {
                b2Tree.sprite.flipX = true;
                b2Tree.animator.Play("PlaneLeft");
            }
            else if (moveDir.x == 0 && moveDir.y > 0)
            {
                b2Tree.animator.Play("PlaneUp");
            }
            else if (moveDir.x == 0 && moveDir.y < 0)
            {
                b2Tree.animator.Play("PlaneDown");
            }
        }

        private void AimPlayer()
        {

            if (b2Tree.nowAimTime1 > 0)
            {
                b2Tree.nowAimTime1 -= Time.deltaTime;
                //b2Tree.aimEffect.transform.position=playerTrans.position+new Vector3(0,2,0);
                if (!b2Tree.isAim)
                {
                    b2Tree.isAim = true;
                    b2Tree.StartAim();
                }
                targetPos = b2Tree.aimEffect.transform.position;
                b2Tree.aimEffect.SetActive(true);
            }
            else
            {
                b2Tree.EndAim();
                b2Tree.animator.Play("PlaneShoot");
                if (b2Tree.nowExplosionTime > 0)
                {
                    b2Tree.nowExplosionTime -= Time.deltaTime;
                }
                else
                {
                    b2Tree.aimEffect.SetActive(false);
                    b2Tree.explosionEffect.transform.position = targetPos;
                    b2Tree.explosionEffect.SetActive(true);
                    b2Tree.canMove = true;
                    //b2Tree.skillID = 0;
                    b2Tree.nowAimTime1 = b2Tree.skill1AimTime;
                    b2Tree.nowExplosionTime = b2Tree.skill1ExplosionTime;
                    b2Tree.HideExplosion();
                    b2Tree.isAim = false;
                    isTrace = false;
                    isAim = false;
                }
            }

        }
    }
    public class SkillTwoTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;
        private bool isAttack;

        private Transform playerTrans;
        private Vector3 targetPos;
        private float deltaTime;
        private Vector3 moveDir;


        public SkillTwoTask() { }
        public SkillTwoTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = b2Trans.GetComponent<Animator>();
            this.playerTrans = b2Tree.playerTrans;
            this.deltaTime = b2Tree.skill2DeltaTime;
        }
        public override NodeState Evaluate()
        {
            if (Boss2Character.isStageTwo && !Boss2Character.hasStageTwo)
                return NodeState.Failure;
            if (b2Tree.skillID != 2)
                return NodeState.Failure;
            if ((Mathf.Abs(b2Tree.skill2Target.position.x - this.b2Trans.position.x) < 0.5f
                && Mathf.Abs(b2Tree.skill2Target.position.y - this.b2Trans.position.y) < 0.5f) ||
                isAttack)
            {
                if (moveDir.x == 0)
                {
                    if (!isAttack)
                    {
                        b2Tree.animator.Play("PlaneUptoIdle");
                        isAttack = true;
                    }
                }
                else
                {
                    if (!isAttack)
                    {
                        b2Tree.animator.Play("PlaneLefttoIdle");
                        isAttack = true;
                    }
                }
                this.AimPlayer();
            }
            else
            {
                if (!isAttack)
                    this.SkillTwoMove();
            }
            if (isAttack)
                this.TracePlayerMove();
            return NodeState.Success;
        }

        public void SkillTwoMove()
        {
            moveDir = (b2Tree.skill2Target.position - this.b2Trans.position).normalized;
            b2Trans.Translate(moveDir * b2Tree.skill2MoveSpeed * Time.deltaTime, Space.Self);
            if (moveDir.x < 0)
            {
                b2Tree.sprite.flipX = false;
                b2Tree.animator.Play("PlaneLeft");
            }
            else if (moveDir.x > 0)
            {
                b2Tree.sprite.flipX = true;
                b2Tree.animator.Play("PlaneLeft");
            }
            else
            {
                b2Tree.animator.Play("PlaneUp");
            }


        }
        private void TracePlayerMove()
        {
            moveDir = new Vector3(playerTrans.position.x - b2Trans.position.x, 0, 0);
            b2Trans.Translate(moveDir * b2Tree.skill2MoveSpeed * Time.deltaTime, Space.Self);
        }
        private void AimPlayer()
        {
            if (b2Tree.skill2NowCount > 0)
            {
                if (deltaTime >= b2Tree.skill2DeltaTime)
                {

                    targetPos = playerTrans.position;
                    deltaTime = 0;
                    b2Tree.skill2NowCount--;
                    b2Tree.animator.Play("PlaneRocket");
                    GameObject rocketObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/STRocket");
                    rocketObj.transform.position = new Vector3(b2Trans.position.x, b2Trans.position.y - 3, -1.2f);
                    rocketObj.GetComponent<STRocket>().landSpeed = b2Tree.skill2BulletSpeed;
                }
                else
                {
                    deltaTime += Time.deltaTime;
                }

            }
            else
            {
                deltaTime = b2Tree.skill2DeltaTime;
                b2Tree.skillID = 0;
                b2Tree.skill2NowCount = b2Tree.skill2MaxCount;
                b2Tree.canMove = true;
                isAttack = false;
            }

        }


    }
    public class SkillThreeTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;

        private float deltaTime = 0;
        private Vector3 moveDir;
        private Vector3 moveTargetPos;
        private int moveTargetIndex = 0;
        private float nowMoveDis = 0;
        private float dashDir;
        private bool isDash;
        private int dashCount;

        public SkillThreeTask() { }
        public SkillThreeTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = b2Trans.GetComponent<Animator>();
            this.deltaTime = b2Tree.skill3BulletDeltaTime;
        }
        public override NodeState Evaluate()
        {
            if (Boss2Character.isStageTwo && !Boss2Character.hasStageTwo)
                return NodeState.Failure;
            if (b2Tree.skillID != 3)
                return NodeState.Failure;
            if (this.moveTargetIndex == 0)
            {
                this.moveTargetIndex = UnityEngine.Random.Range(1, 3);
                if (this.moveTargetIndex == 1)
                {
                    this.moveTargetPos = b2Tree.skill3TargetL.position;
                    this.dashDir = 1;
                }
                else
                {
                    this.moveTargetPos = b2Tree.skill3TargetR.position;
                    this.dashDir = -1;
                }
            }

            if ((Mathf.Abs(this.moveTargetPos.x - this.b2Trans.position.x) < 0.1f
                && Mathf.Abs(this.moveTargetPos.y - this.b2Trans.position.y) < 0.1f)
                || isDash)
            {
                //b2Tree.animator.Play("PlaneLefttoIdle");
                isDash = true;
                this.DashAndBullet();
            }
            else
            {
                if (!isDash)
                    this.SkillThreeMove();
            }

            return NodeState.Success;
        }

        public void SkillThreeMove()
        {
            moveDir = (this.moveTargetPos - this.b2Trans.position).normalized;
            b2Trans.Translate(moveDir * b2Tree.skill3MoveSpeed * Time.deltaTime, Space.Self);
            if (moveDir.x < 0)
            {
                b2Tree.sprite.flipX = false;
            }
            else if (moveDir.x > 0)
            {
                b2Tree.sprite.flipX = true;
            }
            b2Tree.animator.Play("PlaneLeft");

        }

        private void Skill3Dash()
        {
            if (dashDir < 0)
            {
                b2Tree.sprite.flipX = false;
            }
            else
            {
                b2Tree.sprite.flipX = true;
            }
            b2Tree.animator.Play("PlaneDash");
            b2Trans.Translate(this.dashDir * Vector3.right * b2Tree.skill3DashSpeed * Time.deltaTime, Space.Self);
            //Debug.Log(dashDir);
            this.nowMoveDis += b2Tree.skill3DashSpeed * Time.deltaTime;
        }
        private void DashAndBullet()
        {
            if (this.nowMoveDis < b2Tree.skill3DashDistance)
            {
                this.Skill3Dash();
                if (deltaTime >= b2Tree.skill3BulletDeltaTime
                    &&this.b2Trans.position.x<=15
                    &&this.b2Trans.position.x >= -15)
                {

                    deltaTime = 0;
                    GameObject rocketObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/STRocket");
                    rocketObj.transform.position = new Vector3(this.b2Trans.transform.position.x, this.b2Trans.transform.position.y, -1.2f);
                    rocketObj.GetComponent<STRocket>().landSpeed = b2Tree.skill2BulletSpeed;
                }
                else
                {
                    deltaTime += Time.deltaTime;
                }

            }
            else
            {
                this.dashCount++;
                if(Boss2Character.hasStageTwo&&this.dashCount<2)
                {
                    this.nowMoveDis = 0;
                    dashDir = -dashDir;
                    deltaTime = b2Tree.skill3BulletDeltaTime;
                    return;
                }
                this.moveTargetIndex = 0;
                this.dashDir = 0;
                deltaTime = b2Tree.skill3BulletDeltaTime;
                b2Tree.skillID = 0;
                this.nowMoveDis = 0;
                b2Tree.canMove = true;
                this.isDash = false;
                this.dashCount=0;
            }

        }


    }

    public class SkillFourTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;

        private Vector3 moveDir;
        private Vector3 moveTargetPos;
        private int moveTargetIndex = 0;
        private float nowMoveDis = 0;
        private float dashDir;
        private bool isDash;
        private float nowWarnTime = 0;
        private bool isTriggerShadow;
        private float dashY;
        private int dashCount;
        public SkillFourTask() { }
        public SkillFourTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = b2Trans.GetComponent<Animator>();
        }
        public override NodeState Evaluate()
        {
            if (Boss2Character.isStageTwo && !Boss2Character.hasStageTwo)
                return NodeState.Failure;
            if (b2Tree.skillID != 4)
                return NodeState.Failure;
            if (this.moveTargetIndex == 0)
            {
                this.moveTargetIndex = UnityEngine.Random.Range(1, 3);
                if (this.moveTargetIndex == 1)
                {
                    this.moveTargetPos = b2Tree.skill4TargetL.position;
                    this.dashDir = 1;
                }
                else
                {
                    this.moveTargetPos = b2Tree.skill4TargetR.position;
                    this.dashDir = -1;
                }
            }

            if ((Mathf.Abs(this.moveTargetPos.x - this.b2Trans.position.x) < 0.1f
                && Mathf.Abs(this.moveTargetPos.y - this.b2Trans.position.y) < 0.1f)
                || isDash)
            {
                //b2Tree.animator.Play("PlaneLefttoIdle");
                if (!isDash)
                {
                    isDash = true;
                    dashY = b2Tree.playerTrans.position.y + 3;
                    if (dashY > b2Tree.maxDashY)
                    {
                        dashY = b2Tree.maxDashY;
                    }
                    else if (dashY < b2Tree.minDashY)
                    {
                        dashY = b2Tree.minDashY;
                    }
                    b2Trans.position = new Vector3(b2Trans.position.x, dashY, b2Trans.position.z);
                }
                this.DashAndAttack();
            }
            else
            {
                if (!isDash)
                    this.SkillFourMove();
            }

            return NodeState.Success;
        }

        public void SkillFourMove()
        {
            moveDir = (this.moveTargetPos - this.b2Trans.position).normalized;
            b2Trans.Translate(moveDir * b2Tree.skill3MoveSpeed * Time.deltaTime, Space.Self);
            if (moveDir.x < 0)
            {
                b2Tree.sprite.flipX = false;
            }
            else if (moveDir.x > 0)
            {
                b2Tree.sprite.flipX = true;
            }
            b2Tree.animator.Play("PlaneLeft");

        }

        private void Skill4Dash()
        {

            b2Trans.Translate(this.dashDir * Vector3.right * b2Tree.skill4DashSpeed * Time.deltaTime, Space.Self);
            //Debug.Log(dashDir);
            this.nowMoveDis += b2Tree.skill4DashSpeed * Time.deltaTime;
            if (dashDir < 0)
            {
                b2Tree.sprite.flipX = false;
                b2Tree.dashAttackL.SetActive(true);
                b2Tree.dashAttackR.SetActive(false);
            }
            else
            {
                b2Tree.sprite.flipX = true;
                b2Tree.dashAttackR.SetActive(true);
                b2Tree.dashAttackL.SetActive(false);
            }
            if (!isTriggerShadow)
            {
                isTriggerShadow = true;
                b2Tree.TriggerShadow();
            }
            b2Tree.animator.Play("PlaneDash");

        }
        private void DashAndAttack()
        {
            if (this.nowMoveDis < b2Tree.skill4DashDistance)
            {
                if (this.nowWarnTime < b2Tree.skill4WarnTime)
                {
                    b2Tree.skill4WarnEffect.GetComponent<PlaneWarningMovement>().moveDir = this.dashDir;
                    b2Tree.skill4WarnEffect.GetComponent<PlaneWarningMovement>().warnningY = this.dashY;
                    SpriteRenderer[] sr = b2Tree.skill4WarnEffect.GetComponentsInChildren<SpriteRenderer>();
                    for (int i = 0; i < sr.Length; i++)
                    {
                        sr[i].flipX = this.dashDir == 1 ? false : true;
                    }
                    b2Tree.skill4WarnEffect.SetActive(true);

                    nowWarnTime += Time.deltaTime;
                }
                else
                {
                    b2Tree.skill4WarnEffect.SetActive(false);
                    this.Skill4Dash();
                }
            }
            else
            {
                this.dashCount++;
                if(Boss2Character.hasStageTwo&&dashCount<2)
                {
                    nowWarnTime = 0;
                    nowMoveDis = 0;
                    dashDir = -dashDir;
                    return;
                }
                b2Tree.dashAttackL.SetActive(false);
                b2Tree.dashAttackR.SetActive(false);
                b2Tree.EndShadow();
                this.nowWarnTime = 0;
                this.moveTargetIndex = 0;
                this.dashDir = 0;
                b2Tree.skillID = 0;
                this.nowMoveDis = 0;
                b2Tree.canMove = true;
                this.isDash = false;
                this.isTriggerShadow = false;
                this.dashCount = 0;
            }

        }


    }
    
}

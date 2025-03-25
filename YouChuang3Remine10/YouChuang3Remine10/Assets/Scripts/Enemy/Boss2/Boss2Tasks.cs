using BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Boos2Tasks
{
    public class MoveTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;
        private float distance;

        private Vector3 moveDir;
        private Vector3 targetPos;
        private int targetIndex;

        public MoveTask() { }
        public MoveTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = this.b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = this.b2Trans.GetComponent<Animator>();
        }
        public override NodeState Evaluate()
        {
            this.Move();
            if (Mathf.Abs(targetPos.x - this.b2Trans.position.x) < 0.5f && Mathf.Abs(targetPos.y - this.b2Trans.position.y) < 0.5f)
            {
                b2Tree.skillID = 0;
                b2Tree.canMove = false;
                b2Tree.nowIndex = this.targetIndex;               
            }
            return NodeState.Running;
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
                //根据方向不同播放移动动画：

                //移动：
                moveDir = (targetPos - this.b2Trans.position).normalized;
                b2Trans.Translate(moveDir * b2Tree.speed * Time.deltaTime, Space.Self);
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
            if (b2Tree.canMove)
            {
                b2Tree.skillID= 0;
                return NodeState.Failure;
            }
            if(b2Tree.skillID==0)
            {
                int skillID = UnityEngine.Random.Range(1, 41);
                //Debug.Log("SkillID: "+skillID);
                if (skillID <= 10)
                    b2Tree.skillID = 1;
                else if(skillID <= 21&&skillID>10)
                    b2Tree.skillID = 2;
                else if(skillID <= 31&&skillID>20)
                    b2Tree.skillID = 3;
                else
                    b2Tree.skillID = 4;
                //b2Tree.skillID = 4;
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
            if (b2Tree.skillID!=1)
                return NodeState.Failure;

            this.AimPlayer();
            return NodeState.Success;
        }

        private void AimPlayer()
        {
            
            if (b2Tree.nowAimTime1>0)
            {
                b2Tree.nowAimTime1 -= Time.deltaTime;
                b2Tree.aimEffect.transform.position=playerTrans.position+new Vector3(0,2,0);
                targetPos = b2Tree.aimEffect.transform.position;
                b2Tree.aimEffect.SetActive(true);
            }
            else
            {                
                if(b2Tree.nowExplosionTime>0)
                {
                    b2Tree.nowExplosionTime-=Time.deltaTime;
                }
                else
                {
                    b2Tree.aimEffect.SetActive(false);
                    b2Tree.explosionEffect.transform.position = targetPos;
                    b2Tree.explosionEffect.SetActive(true);
                    b2Tree.canMove = true;
                    //b2Tree.skillID = 0;
                    b2Tree.nowAimTime1 = b2Tree.skill1AimTime;
                    b2Tree.nowExplosionTime=b2Tree.skill1ExplosionTime;
                    b2Tree.HideExplosion();
                }                
            }
            
        }


    }
    public class SkillTwoTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;

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
            if (b2Tree.skillID != 2)
                return NodeState.Failure;
            if (Mathf.Abs(b2Tree.skill2Target.position.x - this.b2Trans.position.x) < 0.5f && Mathf.Abs(b2Tree.skill2Target.position.y - this.b2Trans.position.y) < 0.5f)
            {
                this.AimPlayer();
            }
            else
            {
                this.SkillTwoMove();
            }
           
            return NodeState.Success;
        }

        public void SkillTwoMove()
        {
            moveDir=(b2Tree.skill2Target.position-this.b2Trans.position).normalized;
            b2Trans.Translate(moveDir*b2Tree.skill2MoveSpeed*Time.deltaTime,Space.Self);

        }
        private void AimPlayer()
        {
            if(b2Tree.skill2NowCount>0)
            {
                if(deltaTime >= b2Tree.skill2DeltaTime)
                {
                    if (b2Tree.nowAimTime2 > 0)
                    {
                        b2Tree.nowAimTime2 -= Time.deltaTime;
                        b2Tree.aimEffect.transform.position = playerTrans.position + new Vector3(0, 2, 0);
                        targetPos = playerTrans.position;
                        b2Tree.aimEffect.SetActive(true);
                    }
                    else
                    {
                        deltaTime = 0;
                        b2Tree.skill2NowCount--;
                        b2Tree.aimEffect.SetActive(false);
                        GameObject rocketObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/STRocket");
                        rocketObj.transform.position = new Vector3(targetPos.x, targetPos.y + 20f, targetPos.z);
                        rocketObj.GetComponent<STRocket>().landSpeed = b2Tree.skill2BulletSpeed;
                        b2Tree.nowAimTime2 = b2Tree.skill2AimTime;
                    }
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
            }                

        }


    }
    public class SkillThreeTask : Node
    {
        private Boss2Tree b2Tree;
        private Animator b2Animator;
        private Transform b2Trans;

        private float deltaTime=0;
        private Vector3 moveDir;
        private Vector3 moveTargetPos;
        private int moveTargetIndex = 0;
        private float nowMoveDis = 0;
        private float dashDir;
        private bool isDash;

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
            if (b2Tree.skillID != 3)
                return NodeState.Failure;
            if(this.moveTargetIndex==0)
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
                ||isDash)
            {
                isDash = true;
                this.DashAndBullet();
            }
            else
            {   
                if(!isDash)
                    this.SkillThreeMove();
            }

            return NodeState.Success;
        }

        public void SkillThreeMove()
        {
            moveDir = (this.moveTargetPos - this.b2Trans.position).normalized;
            b2Trans.Translate(moveDir * b2Tree.skill3MoveSpeed * Time.deltaTime, Space.Self);

        }

        private void Skill3Dash()
        {

            b2Trans.Translate(this.dashDir*Vector3.right*b2Tree.skill3DashSpeed * Time.deltaTime, Space.Self);
            //Debug.Log(dashDir);
            this.nowMoveDis += b2Tree.skill3DashSpeed * Time.deltaTime;
        }
        private void DashAndBullet()
        {
            if (this.nowMoveDis<b2Tree.skill3DashDistance)
            {
                this.Skill3Dash();
                if (deltaTime >= b2Tree.skill3BulletDeltaTime)
                {

                    deltaTime = 0;
                    GameObject rocketObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/STRocket");
                    rocketObj.transform.position =this.b2Trans.transform.position;
                    rocketObj.GetComponent<STRocket>().landSpeed = b2Tree.skill2BulletSpeed;
                }
                else
                {
                    deltaTime += Time.deltaTime;
                }

            }
            else
            {
                this.moveTargetIndex = 0;
                this.dashDir = 0;
                deltaTime = b2Tree.skill3BulletDeltaTime;
                b2Tree.skillID = 0;
                this.nowMoveDis = 0;
                b2Tree.canMove = true;
                this.isDash=false;
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

        public SkillFourTask() { }
        public SkillFourTask(Transform boss2Trans)
        {
            this.b2Trans = boss2Trans;
            this.b2Tree = b2Trans.GetComponent<Boss2Tree>();
            this.b2Animator = b2Trans.GetComponent<Animator>();
        }
        public override NodeState Evaluate()
        {
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
                isDash = true;
                this.DashAndAttack();
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

        }

        private void Skill4Dash()
        {

            b2Trans.Translate(this.dashDir * Vector3.right * b2Tree.skill4DashSpeed * Time.deltaTime, Space.Self);
            //Debug.Log(dashDir);
            this.nowMoveDis += b2Tree.skill4DashSpeed * Time.deltaTime;
        }
        private void DashAndAttack()
        {
            if (this.nowMoveDis < b2Tree.skill4DashDistance)
            {
                if(this.nowWarnTime < b2Tree.skill4WarnTime)
                {
                    b2Tree.skill4WarnEffect.GetComponent<PlaneWarningMovement>().moveDir = this.dashDir;
                    SpriteRenderer[] sr= b2Tree.skill4WarnEffect.GetComponentsInChildren<SpriteRenderer>();
                    for (int i = 0;i<sr.Length;i++)
                    {
                        sr[i].flipX=this.dashDir==1?false:true;
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
                this.nowWarnTime = 0;
                this.moveTargetIndex = 0;
                this.dashDir = 0;
                b2Tree.skillID = 0;
                this.nowMoveDis = 0;
                b2Tree.canMove = true;
                this.isDash = false;
            }

        }


    }
}

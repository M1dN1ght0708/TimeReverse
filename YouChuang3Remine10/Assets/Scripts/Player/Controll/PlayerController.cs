using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private bool hasEnableST;
    public Vector2 nowSpeed; 
    [HideInInspector]public PlayerControllerInput playerInput;
    #region 物体组件
    public Rigidbody rb;
    public PhysicsCheck phyCheck;
    private PlayerAnimation pAnimation;
    #endregion
    [Header("移动相关")]
    public float currentSpeed;
    public float runSpeed;
    public Vector2 moveDirection;
    private Vector3 rightDirection=new Vector3(1,1,1);
    private Vector3 leftDirection=new Vector3(-1, 1, 1);
    bool isMaxSpeed;
    float time = 0;
    [Header("跳跃相关")]
    public float jumpForce;
    public bool isCanJump2;
    public int maxJupmCount;
    public int currentJumpCount;

    [Header("攻击相关")]
    //v01
    public bool isAttack;
    private Vector3 targetPos;
    //攻击蹭步
    public float attackForce;

    //v02
    public float normalizedTime;
    public int comboCunter;
    [HideInInspector]
    public List<AttackSO> comboList;
    [HideInInspector]
    float lastInputTime;
    [HideInInspector]
    float lastComboEndTime;   
    [HideInInspector]
    public float time1;
    [HideInInspector]
    public float time2;   
    [HideInInspector]
    public float endComboTime;

    //v03(借用v02中的comboCunter和normalizedTime)
    private AnimatorStateInfo animStateInfo;   
   
    
    [Header("冲刺相关")]
    public float dashDistance;
    public float dashTime;
    public float nowDashTime;
    public bool isDash;   
    public float dashSpeed;
    public GameObject shadowObj;
    public int dashCountMax;
    public int dashCounter;
    public float dashCD;
    public float dashCDCounter;
    [Header("时停相关")]
    public float stopTime;
    public float nowStopTime;
    public float stopTimeNeed;
    public float stopTimeCounter;
    public bool isCounterMax;
    public static bool isStopTime;    
    public bool testIsStopTime;
    //public UnityEvent bulletTimeEnemy;
    //范围时停
    public bool testIsRangeStopTime;
    public static bool isRangeStopTime;
    public  float rangeStopTimeNeed;
    public GameObject stopRangeObj;
    [Header("子弹相关")]
    public Vector2 bulletBeginOffset;
    public float drawBulletPosR;
    public float bulletBeginOffsetX;
    public bool isShoot;
    public bool isReadyShoot;
    public bool canShootEnd;
    //最大射击次数
    public int maxShootCount;
    public int shootCounter;
    [Header("空中攻击")]
    public float jumpAttackForceX;
    public float jumpAttackForceY;
    public int jumpAttackMax;
    public int jumpAttackCounter;
    [Header("空中射击")]
    public float jumpShootForceX;
    public float jumpShootForceY;
    public int jumpShootMax;
    public int jumpShootCounter;
    [Header("交互相关")]
    public static bool isPause;
    //是否可以对话
    public bool canDialogue;
    //是否可以切换场景
    public  bool canChangeScene;
    //目标场景
    public string targetScene;
    public Vector3 playerTargetPos;
    //记录上一个检查点
    public Transform lastCheckPoint;
    public bool isReturnCheckPoint;
    [Header("受伤")]
    public bool isDead;
    public bool isHurt;
    public float hurtForce;
    public GameObject hurtEffectBlade;
    public GameObject hurtEffectBullet;

    private void OnEnable()
    {
        this.playerInput.Enable();
    }
    private void OnDisable()
    {
        this.playerInput.Disable();
    }
    protected virtual void Awake()
    {        

        #region 组件获取
        phyCheck = this.GetComponent<PhysicsCheck>();
        rb = this.GetComponent<Rigidbody>();
        pAnimation = this.GetComponent<PlayerAnimation>();
        #endregion
        #region 变量初始化
        playerInput = new PlayerControllerInput();
        Physics.gravity = new Vector3(0,-39.24f,0);
        currentSpeed = runSpeed;
        shootCounter = maxShootCount;
        stopTimeCounter = stopTimeNeed;
        dashCDCounter = dashCD;
        bulletBeginOffsetX=Mathf.Abs(bulletBeginOffset.x);
        #endregion
        #region 按键事件
        playerInput.GamePlay.Jump.started += playerJump;       
        //playerInput.GamePlay.StopTime.started += StopTime;
        playerInput.GamePlay.RangeStopTime.started += RangeStopTime;
        playerInput.GamePlay.Shoot.started += playerShoot;
        playerInput.GamePlay.Attack.started += playerAttackNew;
        playerInput.GamePlay.Dash.started += playerReadyToDash;
        playerInput.UI.Interact.started += playerInteract;

        #endregion
        #region 事件监听
        #endregion
    }



    protected virtual void Update()
    {
        // if (!GameDataMgr.Instance.inStoreTimeLine)
        //     playerInput.GamePlay.RangeStopTime.Disable();
        // else if (!hasEnableST && GameDataMgr.Instance.inStoreTimeLine)
        // {
        //     hasEnableST = true;
        //     playerInput.GamePlay.RangeStopTime.Enable();
        // }
        playerTurn();
        playerStopTimeAdd();
        #region 测试变量
        testIsStopTime = isStopTime;
        testIsRangeStopTime=isRangeStopTime;
        nowSpeed = rb.velocity;
        #endregion
        if(phyCheck.isGround)
        {
            jumpAttackCounter = jumpAttackMax;
            jumpShootCounter = jumpShootMax;
            dashCounter = dashCountMax;
        }
        else if (!phyCheck.isGround && dashCounter > 0)
        {
            dashCounter = 1;
        }
        //获取当前轴输入作为移动方向
        moveDirection = this.playerInput.GamePlay.Move.ReadValue<Vector2>();
        if (moveDirection.x == 0||isAttack||isDash||isHurt||isShoot||!phyCheck.isGround||isHurt)
        {
            AudioMgr.Instance.StopSoundNew(AudioID.playerMove);
        }
        //检测二段跳
        if(isCanJump2&&phyCheck.isGround&&currentJumpCount< maxJupmCount)
        {
            currentJumpCount = maxJupmCount;
        }
        else if(isCanJump2 && !phyCheck.isGround&& currentJumpCount>0)
        {
            currentJumpCount = 1;
        }

        //Attack_v03
        playerAttackInput();
        //判断是否播放收枪动画
        playerCanEndShoot();
    }

    protected virtual void FixedUpdate()
    {
        if (!isHurt&&!isAttack && !isDash&&!isReadyShoot&&!isShoot)
        {
            this.playerMove();
        }
        playerDashNew();    
    }
    //移动函数
    private void SpeedUp()
    {
        time += Time.deltaTime;
        currentSpeed=Mathf.Lerp(0, runSpeed, time);
        if(currentSpeed>=runSpeed)
        {
            time = 0;
        }
    }
        //转向
    void playerTurn()
    {
        if (isPause||isDash || isHurt || (isAttack && !phyCheck.isGround) || (isReadyShoot && !phyCheck.isGround))
            return;

        if (moveDirection.x > 0)
            this.transform.localScale = rightDirection;
        if (moveDirection.x < 0)
            this.transform.localScale = leftDirection;
               
    }
    protected void playerMove() 
    {
        if (isPause)
            return;
        currentSpeed = runSpeed;
        rb.velocity = new Vector2(moveDirection.x*currentSpeed*Time.deltaTime,rb.velocity.y);
        if (moveDirection.x > 0)
            this.transform.localScale = rightDirection;
        if(moveDirection.x < 0)
            this.transform.localScale = leftDirection;
    }

    //跳跃函数
    private void playerJump(InputAction.CallbackContext context)
    {
        if (isPause)
            return;
        if ((phyCheck.isGround||(isCanJump2&&currentJumpCount>0))
            &&!isDash&&!isReadyShoot&&!isHurt)
        {
            //if (currentJumpCount == 1)
            //    rb.velocity = new Vector2(rb.velocity.x,0);
            //rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            AudioMgr.Instance.PlaySoundNew(AudioID.playerJump);
            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
            if(currentJumpCount>0)
                currentJumpCount--;
        }
            
    }

    //攻击函数
    //v01
    private void playerAttack(InputAction.CallbackContext context)
    {
        if(phyCheck.isGround&&!isDash&&!isHurt)
        {           
            this.isAttack = true;
            pAnimation.AttackAnimationTrigger();
            rb.AddForce(new Vector3(this.transform.localScale.x,0,0) * attackForce, ForceMode.Impulse);
            //rb.velocity = new Vector2(attackForce, rb.velocity.y);
        }
       
    }
    //v02
    private void playerAttackV2(InputAction.CallbackContext context)
    {
        if(phyCheck.isGround && !isDash)
        {
            if (Time.time - lastComboEndTime > time1 && comboCunter <comboList.Count)
            {
                CancelInvoke("EndCombo");
                if (Time.time - lastInputTime >= time2)
                {
                    isAttack = true;
                    rb.AddForce(new Vector3(this.transform.localScale.x, 0, 0) * attackForce, ForceMode.Impulse);
                    pAnimation.animator.runtimeAnimatorController = comboList[comboCunter].animatorOV;
                    pAnimation.animator.Play("playerAttack1", 1, 1);
                    comboCunter++;
                    lastInputTime = Time.time;
                    if (comboCunter >= comboList.Count)
                    {
                        comboCunter = 0;
                    }
                }
            }
        }        
    }
    private void ExitAttack()
    {
        //if (pAnimation.animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack"))
        //{
        //    print(pAnimation.animator.GetCurrentAnimatorStateInfo(1).normalizedTime);
        //}

        if (pAnimation.animator.GetCurrentAnimatorStateInfo(1).normalizedTime >=this.normalizedTime
            &&pAnimation.animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack"))
        {
            Invoke("EndCombo", endComboTime);
        }
    }
    private void EndCombo()
    {
        comboCunter = 0;
        lastComboEndTime = Time.time;
    }
    //v03
    void playerAttackInput()
    {
        animStateInfo = pAnimation.animator.GetCurrentAnimatorStateInfo(1);
        if ((animStateInfo.IsName("playerAttack1")|| 
            animStateInfo.IsName("playerAttack2")||
            animStateInfo.IsName("playerAttack3")||
            animStateInfo.IsName("playerAttack4"))&&
            animStateInfo.normalizedTime>1.0f
            )
        {
            comboCunter = 0;
            pAnimation.animator.SetInteger("comboCounter", comboCunter);
            isAttack = false;
        }
    }
    void playerAttackNew(InputAction.CallbackContext context)
    {
        if (isPause)
            return;
        if (!isHurt&&!phyCheck.isGround&&!isDash&&!isReadyShoot)
        {
            if(jumpAttackCounter>=jumpAttackMax)
            {
                isAttack= true;
                pAnimation.JumpAttackAnimationTrigger();
                this.rb.velocity = new Vector2(jumpAttackForceX*-transform.localScale.x,jumpAttackForceY);
                jumpAttackCounter--;
            }
            
        }

        if (!isHurt&&phyCheck.isGround && !isDash&&!isReadyShoot)
        {            
            if (comboCunter == 0)
            {
                isAttack = true;
                rb.velocity = new Vector3(this.transform.localScale.x, 0, 0) * attackForce;
                comboCunter = 1;
                //EventCenter.Instance.TriggerEvent("PlaySound", attack1Clip);
                //AudioMgr.Instance.PlaySoundNew(attack1Clip);
                //AudioMgr.Instance.PlaySoundNew(AudioID.playerA1);
                pAnimation.animator.SetInteger("comboCounter", comboCunter);
            }
            else if (animStateInfo.IsName("playerAttack1") && comboCunter == 1 && animStateInfo.normalizedTime < this.normalizedTime)
            {
                isAttack = true;
                rb.velocity = new Vector3(this.transform.localScale.x, 0, 0) * attackForce;
                //EventCenter.Instance.TriggerEvent("PlaySound", attack2Clip);
                //AudioMgr.Instance.PlaySoundNew(attack2Clip);
                //AudioMgr.Instance.PlaySoundNew(AudioID.playerA2);
                comboCunter = 2;
            }
            else if (animStateInfo.IsName("playerAttack2") && comboCunter == 2 && animStateInfo.normalizedTime < this.normalizedTime)
            {
                isAttack = true;
                rb.velocity = new Vector3(this.transform.localScale.x, 0, 0) * attackForce;
                //EventCenter.Instance.TriggerEvent("PlaySound", attack3Clip);
                //AudioMgr.Instance.PlaySoundNew(attack3Clip);
                //AudioMgr.Instance.PlaySoundNew(AudioID.playerA3);
                comboCunter = 3;
            }
            else if (animStateInfo.IsName("playerAttack3") && comboCunter == 3 && animStateInfo.normalizedTime < this.normalizedTime)
            {
                isAttack = true;
                rb.velocity = new Vector3(this.transform.localScale.x, 0, 0) * attackForce;
                //EventCenter.Instance.TriggerEvent("PlaySound", attack4Clip);
                //AudioMgr.Instance.PlaySoundNew(attack4Clip);
                //AudioMgr.Instance.PlaySoundNew(AudioID.playerA4);
                comboCunter = 4;
            }
        }
        
    }

    //冲刺函数
    //v02
    private void playerReadyToDash(InputAction.CallbackContext context)
    {
        if (isHurt||isPause)
            return;
        if(!isDash&&dashCDCounter>=dashCD&&dashCounter>0)
        {
            //EventCenter.Instance.TriggerEvent("PlaySound", dashClip);
            //AudioMgr.Instance.PlaySound(this.audioSource, dashClip);
            AudioMgr.Instance.PlaySoundNew(AudioID.playerDash);
            isDash = true;
            nowDashTime = 0;
            dashCounter--;
        }
        
    }
    private void playerDashNew()
    {
        if(isDash)
        {
            if(isHurt)
            {
                isDash = false;
                rb.velocity = Vector3.zero;
                rb.useGravity = true;
                return;
            }
            if(nowDashTime<dashTime)
            {
                rb.useGravity = false;
                rb.velocity = new Vector2(dashSpeed * this.transform.localScale.x, 0);
                nowDashTime += Time.deltaTime;
                this.gameObject.layer = LayerMask.NameToLayer("Invincible");
                StartCoroutine(TriggerShadow());
                if (nowDashTime >= dashTime)
                {
                    dashCDCounter = 0;
                    isDash =false;
                    rb.velocity = Vector3.zero;
                    rb.useGravity = true;
                    this.gameObject.layer = LayerMask.NameToLayer("Player");
                    canShootEnd = false;
                }
            }
        }
        else
        {
            if(dashCDCounter<dashCD)
                dashCDCounter += Time.deltaTime;
        }
    }
    private IEnumerator TriggerShadow()
    {
        while(isDash)
        {
            yield return new WaitForEndOfFrame();
            PoolManager.Instance.GetObj("Shadow/DashShadow");
        }
    }

    //v01
    private void playerDash(InputAction.CallbackContext context)
    {
        if(isHurt||isPause)
            return;
        if(!isDash)
        {
            isDash = true;
            isAttack = false;
        }
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        targetPos = new Vector3(this.transform.position.x + dashDistance * transform.localScale.x, transform.position.y,transform.position.z);
        this.gameObject.layer = LayerMask.NameToLayer("Invincible");
        //shadowObj.SetActive(true);
        StartCoroutine(TriggerDash());
    }
    private IEnumerator TriggerDash()
    {
        do
        {
            yield return null;
            if (phyCheck.isWall)
            {
                isDash = false;
                break;  
            }
            rb.MovePosition(new Vector3(transform.position.x + transform.localScale.x * dashSpeed, targetPos.y, transform.position.z));
            PoolManager.Instance.GetObj("Shadow/DashShadow");
        }
        while (Mathf.Abs(targetPos.x - this.transform.position.x) > 0.5f);
        isDash = false;
        rb.useGravity = true;
        this.gameObject.layer = LayerMask.NameToLayer("Player");
        //shadowObj.SetActive(false);
    }
    //时停函数
    private void playerStopTimeAdd()
    {
        if (stopTimeCounter<stopTimeNeed&&!isStopTime)
        {
            stopTimeCounter += Time.deltaTime;           
            if (stopTimeCounter >= stopTimeNeed)
            {
                stopTimeCounter = stopTimeNeed;
            }
        }
        
        if(isStopTime)
        {
            nowStopTime-= Time.deltaTime;
            if(nowStopTime<=0)
            {                
                isStopTime= false;
            }
        }

        if (isRangeStopTime)
        {
            nowStopTime -= Time.deltaTime;
            if (nowStopTime <= 0)
            {
                isRangeStopTime = false;
                stopRangeObj.transform.SetParent(this.transform);
                stopRangeObj.transform.localPosition = new Vector3(0, 1.45f, 0);
                stopRangeObj.SetActive(false);
                
            }
        }

        EventCenter.Instance.TriggerEvent("TimeChange", this);
    }
    private void StopTime(InputAction.CallbackContext context)
    {
        if (stopTimeCounter>=stopTimeNeed)
        {
            isStopTime = true;
            isCounterMax = false;
            nowStopTime = stopTime;
            stopTimeCounter =0;

        }
    }
    private void RangeStopTime(InputAction.CallbackContext context)
    {
        if (isPause)
            return;
        if (stopTimeCounter>=rangeStopTimeNeed&&!isRangeStopTime)
        {
            stopRangeObj.SetActive(true);
            stopRangeObj.transform.parent = null;
            stopTimeCounter-=rangeStopTimeNeed;
            isRangeStopTime = true;
            nowStopTime= stopTime;
        }
        else if (isRangeStopTime)
        {
            nowStopTime = 0;
        }
        
    }
    //射击
    private void playerShoot(InputAction.CallbackContext context)
    {
        if (isHurt||isAttack||isDash||isPause)
        {            
            return;
        }            
        if(!isReadyShoot&&!phyCheck.isGround)
        {
            if (jumpShootCounter >= jumpShootMax)
            {
                isReadyShoot = true;
                pAnimation.JumpShootAnimationTrigger();
                this.rb.velocity = new Vector2(jumpShootForceX * -transform.localScale.x, jumpShootForceY);
                jumpShootCounter--;
            }

        }
        if(!isReadyShoot&&phyCheck.isGround)
        {
            isReadyShoot = true;           
        }
        if (shootCounter > 0 && phyCheck.isGround)
        {
            isShoot = true;
            pAnimation.ShootAnimationTrigger();
        }


    }
        //判断是否播放收枪动画
    private void playerCanEndShoot()
    {
        if(isHurt||isDash||!phyCheck.isGround||Mathf.Abs(this.rb.velocity.x)>1)
        {
            canShootEnd = false;
        }
        else
        {
            canShootEnd= true;
        }
    }
    private void OnDrawGizmosSelected()
    {
        //bulletBeginOffset = new Vector2(this.transform.localScale.x * bulletBeginOffsetX, bulletBeginOffset.y);
        Gizmos.DrawWireSphere((Vector2)this.transform.position + bulletBeginOffset, drawBulletPosR);
    }

    //交互
    protected void playerInteract(InputAction.CallbackContext context)
    {
        if (isPause)
            return;
        //切换场景
        if (canChangeScene)
        {
            print("切换场景");
            MySceneManager.Instance.ChangeSceneTo(targetScene,playerTargetPos);
        }
        //对话
        if (canDialogue) 
        {
            EventCenter.Instance.TriggerEvent("TriggerDialogue",null);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ChangeScene"))
        {
            canChangeScene = true;
            targetScene = other.gameObject.name;
            playerTargetPos = other.GetComponent<ChangeScene>().nextScenePos;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("ChangeScene"))
        {
            canChangeScene = false;
        }
    }
    //返回检查点
    public void playerReturnCheckPoint()
    {
        if(lastCheckPoint!=null)
            this.transform.position = lastCheckPoint.position;
    }
    //播放TimeLine禁用操作
    public void DisablePlay()
    {
        this.playerInput.Disable();
    }

    public void EnablePlay()
    {
        this.playerInput.Enable();
    }

    public void DisableStopTime()
    {
        this.playerInput.GamePlay.RangeStopTime.Disable();
    }

    public void EnableStopTime()
    {
        this.playerInput.GamePlay.RangeStopTime.Enable();
    }

    //受伤函数
    public void GetHurt(Transform attackerTrans,bool attackType=false)
    {
        if (isPause)
            return;
        if (attackerTrans.CompareTag("MovePlatform"))
        {
            print("传送");
            isReturnCheckPoint = true;
        }
        if (attackType)
        {
            hurtEffectBullet.SetActive(true);
            AudioMgr.Instance.PlaySoundNew(AudioID.pHurtBullet);
        }
        else
        {
            hurtEffectBlade.SetActive(true);
            AudioMgr.Instance.PlaySoundNew(AudioID.pHurtBlade);
        }
        StartCoroutine(HurtEffect());
        isHurt = true;
        rb.velocity = Vector2.zero;
        hurtForce = attackerTrans.GetComponentInParent<Attack>().attackForce;
        Vector2 dir = new Vector2((transform.position.x - attackerTrans.position.x), 0).normalized;
        rb.AddForce(dir*hurtForce,ForceMode.Impulse);
    }
    private IEnumerator HurtEffect()
    {
        yield return new WaitForSeconds(0.2f);
        hurtEffectBlade.SetActive(false);
        hurtEffectBullet.SetActive(false);
    }
    //死亡
    public void GetDead()
    {
        this.isDead=true;
        EventCenter.Instance.TriggerEvent("HpChange", this.GetComponent<Character>());
        playerInput.GamePlay.Disable();
        if(this.lastCheckPoint!=null)
            MySceneManager.Instance.ChangeSceneTo(SceneManager.GetActiveScene().name, this.lastCheckPoint.position);
        else
            MySceneManager.Instance.ChangeSceneTo(SceneManager.GetActiveScene().name);
        Invoke("Die", 2f);
    }
    private void Die()
    {
        Destroy(this.gameObject);
    }
}

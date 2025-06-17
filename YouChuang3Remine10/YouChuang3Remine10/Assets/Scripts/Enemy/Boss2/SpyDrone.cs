using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyDrone : Enemy
{
    private Dictionary<SpyDroneStateEnum, BaseState> stateDic = new Dictionary<SpyDroneStateEnum, BaseState>();
    // Start is called before the first frame update
    [Header("Idle")]
    public float idleTime;
    [Header("Ñ²Âß")]
    //public float minX;
    //public float maxX;
    //public float minY;
    //public float maxY;
    public float patrolLengthX;
    public float patrolLengthY;
    public float patrolSpeed;
    public Vector3 spawnPoint;
    [Header("ÊÜÉË")]
    public bool isHurt;
    public bool isDead;
    public GameObject hurtEffectBlade;
    public GameObject hurtEffectBullet;
    [Header("µØÃæ¼ì²â")]
    public bool isGround;
    public float groundRadius;
    public LayerMask groundLayer;
    private Collider[] groundCls;
    public Vector2 groundOffset;
    [HideInInspector] public float groundOffsetX;

    protected override void Awake()
    {
        base.Awake();
        stateDic.Add(SpyDroneStateEnum.Idle, new SpyDroneIdle());
        stateDic.Add(SpyDroneStateEnum.Patrol, new SpyDronePatrol());
        stateDic.Add(SpyDroneStateEnum.Hurt, new SpyDroneHurt());
        stateDic.Add(SpyDroneStateEnum.Dead, new SpyDroneDead());

        //spawnPoint = this.transform.position;
        groundOffsetX = Mathf.Abs(groundOffset.x);
    }
    private bool isRuning;
    private void OnEnable()
    {
        SwitchState(SpyDroneStateEnum.Idle);
        isRuning = true;
    }

    private void Update()
    {
        this.Check();
        EndRangeTimeStop();
        if ((isInRange && PlayerController.isRangeStopTime) || PlayerController.isStopTime)
        {
            StopTimeDo();
        }
        else
        {
            beStop = false;
            animator.speed = 1;
        }
        currentState.LogicUpdate();
    }

    private void CalculateOffset()
    {
        groundOffset = new Vector2(groundOffsetX * this.transform.localScale.x, groundOffset.y);
    }
    private void Check()
    {
        CalculateOffset();
        #region ¼ì²âÍæ¼Ò
      
        #endregion
        #region µØÃæ¼ì²â
        Collider[] cls = Physics.OverlapSphere(this.transform.position + (Vector3)groundOffset, groundRadius, groundLayer);
        if (cls.Length > 0)
            isGround = true;
        else
            isGround = false;

        #endregion
    }
    public override void GetHurt(Transform attackerTrans, bool attackType = false)
    {
        PlayHurtSound(attackType);
        if (attackType)
            hurtEffectBullet.SetActive(true);
        else
            hurtEffectBlade.SetActive(true);
        StartCoroutine(HurtEffect());
        if (beStop)
            return;
        isHurt = true;
        Vector2 dir = new Vector2(this.transform.position.x - attackerTrans.position.x, 0).normalized;
        this.rb.velocity = dir * hurtForce;
    }
    private IEnumerator HurtEffect()
    {
        yield return new WaitForSeconds(0.2f);
        hurtEffectBlade.SetActive(false);
        hurtEffectBullet.SetActive(false);
    }
    public override void GetDead()
    {
        PlayHurtSound(false);
        this.isDead = true;
        animator.speed = 1;
        this.gameObject.layer = LayerMask.NameToLayer("Dead");
        Invoke("DestroyThis", 1);
    }
    private void DestroyThis()
    {
        PoolManager.Instance.PushObj("Component/Enemy/SpyDrone", this.gameObject);
    }
    public Vector3 GetNewPoint()
    {
        float targetX = Random.Range(-patrolLengthX/2, patrolLengthX/2);
        float targetY = Random.Range(-patrolLengthY/2, patrolLengthY/2);
        targetX += spawnPoint.x;
        targetY += spawnPoint.y;
        return new Vector3(targetX, targetY);
    }

    protected override void StopTimeDo()
    {
        beStop = true;
        if (!isDead)
            animator.speed = 0;
    }
    public void SwitchState(SpyDroneStateEnum spyDroneStateNum)
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = stateDic[spyDroneStateNum];
        currentState.OnEnter(this);
    }
    public void FlipTo(Vector3 targetPos)
    {
        if (targetPos != null)
        {
            float dir = targetPos.x - this.transform.position.x;
            this.transform.localScale = new Vector3(dir > 0 ? -1 : 1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {

        //Ñ²Âß·¶Î§
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnPoint, new Vector3(patrolLengthX, patrolLengthY));
        //Gizmos.DrawWireSphere(spawnPoint, patrolRadius);
        //if (!isRuning)
        //    Gizmos.DrawWireSphere(this.transform.position, patrolRadius);
      
    }
}

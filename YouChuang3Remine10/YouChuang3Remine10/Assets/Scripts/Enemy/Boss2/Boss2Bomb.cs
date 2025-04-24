using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Bomb : BaseGameLevel
{
    // Start is called before the first frame update

    public float throwSpeed;
    public float hideTime;
    public float nowExistTime;
    public float maxFlyTime;
    private float nowFlyTime;
    private float currentSpeed;

    public Vector3 moveDir;
    public bool inPickRange;
    public bool hasPick;
    public bool isFly;
    private bool hasLand;
    private Rigidbody rigidbody;
    private Collider collider;
    private GameObject boomEffectBoss;
    private GameObject boomEffectOthers;
    public GameObject tipsObj;
    private float randomDir;
    private bool mustVertical;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        nowExistTime = hideTime;
    }
    private void OnEnable()
    {
        mustVertical = false;
        //hasPick = false;
        hasLand = false;
        //isFly = false;
        //rigidbody.isKinematic =false;
        //collider.isTrigger = false;
        nowExistTime = hideTime;
        nowFlyTime = maxFlyTime;
        this.gameObject.layer = LayerMask.NameToLayer("Boss2Bomb");
        this.moveDir=Vector3.zero;
         randomDir = UnityEngine.Random.Range(-8, 9);
        
    }
    protected override void OnTriggerStay(Collider other)
    {
        if (!hasPick||!isFly)
            return;
        base.OnTriggerEnter(other);
        if(other.CompareTag("Ground")||other.CompareTag("MovePlatform"))
        {
            print("扎到箱子或平台");
            boomEffectOthers = PoolManager.Instance.GetObj("Bullet/EnemyBullet/RocketBoom");
            boomEffectOthers.transform.position = this.transform.position;
            Invoke("PushEffect", 1f);
            PoolManager.Instance.PushObj("Bullet/EnemyBullet/Boss2Bomb", this.gameObject);
            hasPick = false;
            isFly = false;
            rigidbody.isKinematic = false;
            collider.isTrigger = false;
        }
        if (other.CompareTag("Boss"))
        {
            print("扎到Boss");
            other.GetComponent<Character>()?.TakeDamage(this,true);
            boomEffectBoss = PoolManager.Instance.GetObj("Bullet/EnemyBullet/PlaneExplosion");
            boomEffectBoss.transform.position = this.transform.position;
            Invoke("PushEffect", 1f);
            PoolManager.Instance.PushObj("Bullet/EnemyBullet/Boss2Bomb", this.gameObject);
            hasPick = false;
            isFly = false;
            rigidbody.isKinematic = false;
            collider.isTrigger = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")
           || collision.gameObject.CompareTag("MovePlatform"))
        {
            if(collision.gameObject.CompareTag("MovePlatform"))
            {
                this.transform.SetParent(collision.gameObject.transform, true);
            }
            rigidbody.isKinematic = true;
            collider.isTrigger = true;
            hasLand=true;
            this.gameObject.layer = LayerMask.NameToLayer("Default");
        }
            
    }
    // Update is called once per frame
    void Update()
    {
        if(!hasPick&&Boss2Character.isStageTwo&&!Boss2Character.hasStageTwo)
        {
            boomEffectOthers = PoolManager.Instance.GetObj("Bullet/EnemyBullet/RocketBoom");
            boomEffectOthers.transform.position = this.transform.position;
            Invoke("PushEffect", 1f);
            PoolManager.Instance.PushObj("Bullet/EnemyBullet/Boss2Bomb", this.gameObject);
            hasPick = false;
            isFly = false;
            rigidbody.isKinematic = false;
            collider.isTrigger = false;
        }
        if(!hasLand&&!isInRange&&!isFly&&!hasPick)
        {
            if(mustVertical)
                rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
            else
                rigidbody.velocity = new Vector2(randomDir, rigidbody.velocity.y);
        }
        if(hasLand&&!hasPick)
        {
            nowExistTime = Time.time;
            if(nowExistTime<=0)
            {
                PoolManager.Instance.PushObj("Bullet/EnemyBullet/Boss2Bomb", this.gameObject);
                Boss2PlatformCharacter.hasBomb = false;
                hasPick = false;
                isFly = false;
                rigidbody.isKinematic = false;
                collider.isTrigger = false;
            }
        }
        EndRangeTimeStop();
        if ((isInRange && PlayerController.isRangeStopTime) || PlayerController.isStopTime)
        {
            StopTimeDo();
        }
        else
        {
            currentSpeed = throwSpeed;
        }
        if (isFly)
        {
            nowFlyTime -= Time.deltaTime;
            if (nowFlyTime <= 0)
            {
                isFly = false;
                PoolManager.Instance.PushObj("Bullet/EnemyBullet/Boss2Bomb", this.gameObject);
                boomEffectOthers = PoolManager.Instance.GetObj("Bullet/EnemyBullet/RocketBoom");
                boomEffectOthers.transform.position = this.transform.position;
                Invoke("PushEffect", 1f);
                hasPick = false;
                rigidbody.isKinematic = false;
                collider.isTrigger = false;
            }
            this.transform.Translate(moveDir * currentSpeed * Time.deltaTime, Space.World);
        }   
        if(hasLand&&!hasPick&&!isFly)
        {
            tipsObj.SetActive(inPickRange);
        }
        if(hasPick||isFly||!hasLand)
        {
            tipsObj.SetActive(false);
        }
    }
    protected override void StopTimeDo()
    {
        currentSpeed = 0;
        rigidbody.velocity = Vector3.zero;
        mustVertical = true;
    }

    private void PushEffect()
    {
        if (boomEffectOthers != null)
        {
            PoolManager.Instance.PushObj("Bullet/EnemyBullet/RocketBoom", boomEffectOthers);
        }
        if(boomEffectBoss != null)
            PoolManager.Instance.PushObj("Bullet/EnemyBullet/PlaneExplosion", boomEffectBoss);
        
    }
}

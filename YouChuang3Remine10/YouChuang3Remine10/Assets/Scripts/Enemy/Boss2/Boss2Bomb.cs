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
    private GameObject boomEffect;
    public GameObject tipsObj;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        nowExistTime = hideTime;
    }
    private void OnEnable()
    {
        hasPick = false;
        hasLand = false;
        isFly = false;
        rigidbody.isKinematic =false;
        collider.isTrigger = false;
        nowExistTime = hideTime;
        nowFlyTime = maxFlyTime;
        this.gameObject.layer = LayerMask.NameToLayer("Boss2Bomb");
        this.moveDir=Vector3.zero;
        
    }
    protected override void OnTriggerStay(Collider other)
    {
        if (!hasPick||!isFly)
            return;
        base.OnTriggerEnter(other);
        if(other.CompareTag("Ground")||other.CompareTag("MovePlatform"))
        {
            boomEffect=PoolManager.Instance.GetObj("Bullet/EnemyBullet/RocketBoom");
            boomEffect.transform.position = this.transform.position;
            Invoke("PushEffect", 1f);
            PoolManager.Instance.PushObj("Bullet/EnemyBullet/Boss2Bomb", this.gameObject);
        }
        if(other.CompareTag("Boss"))
        {
            other.GetComponent<Character>()?.TakeDamage(this,true);
            boomEffect = PoolManager.Instance.GetObj("Bullet/EnemyBullet/RocketBoom");
            boomEffect.transform.position = this.transform.position;
            Invoke("PushEffect", 1f);
            PoolManager.Instance.PushObj("Bullet/EnemyBullet/Boss2Bomb", this.gameObject);
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
        if(hasLand&&!hasPick)
        {
            nowExistTime = Time.time;
            if(nowExistTime<=0)
            {
                PoolManager.Instance.PushObj("Bullet/EnemyBullet/Boss2Bomb", this.gameObject);
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
                boomEffect = PoolManager.Instance.GetObj("Bullet/EnemyBullet/RocketBoom");
                boomEffect.transform.position = this.transform.position;
                Invoke("PushEffect", 1f);
            }
            this.transform.Translate(moveDir * currentSpeed * Time.deltaTime, Space.World                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               );
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
    }

    private void PushEffect()
    {
        if (boomEffect != null)
        {
            PoolManager.Instance.PushObj("Bullet/EnemyBullet/RocketBoom", boomEffect);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2PlatformCharacter : Character
{
    public float probability;
    public float generateCD;
    public float nowCD;
    public bool isCD;
    public bool hasRevive;
    public bool isBroken;
    [Header("受击特效")]
    public GameObject hurtEffectBlade;
    public GameObject hurtEffectBullet;
    //[HideInInspector]
    public static bool hasBomb;
    private bool hasStageTwo;
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Attack>() != null)
        {
            this.GetDamage(collision.GetComponent<Attack>().damage, collision.GetComponent<Attack>().attackType);
        }
    }*/
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Attack>() != null && !other.CompareTag("Boss2Bomb") && !other.CompareTag("STRocket"))
        {
            this.GetDamage(other.GetComponent<Attack>().damage, other.GetComponent<Attack>().attackType);
        }
    }*/
    void Start()
    {
        this.currentHp = this.maxHp;
    }

    protected override void Update()
    {
        base.Update();
        if (Boss2Character.isStageTwo && !this.hasStageTwo)
        {
            this.GetDamage(maxHp, true);           
            this.hasStageTwo = true;
        }
            
        if (isCD)
        {
            nowCD -= Time.deltaTime;
            if (nowCD <= 0)
            {
                isCD = false;
                hasRevive = true;
                isBroken = false;
            }
        }
        print(hasBomb);
    }
    public override void TakeDamage(Attack attacker, bool attackType = false)
    {
        print("箱子受伤");
        this.GetDamage(attacker.damage, attackType);
    }
    private void GetDamage(float damage, bool attackType = false)
    {
        this.currentHp -= damage;
        if (attackType)
        {
            hurtEffectBullet.SetActive(true);
        }
        else
        {
            hurtEffectBlade.SetActive(true);
        }
        Invoke("HideHurtEffect", 0.2f);
        if (this.currentHp <= 0)
        {
            this.isBroken = true;
            int numberBomb = UnityEngine.Random.Range(1, 101);
            if (true || numberBomb <= 100 * probability)
            {
                //生成炸弹道具
                if (!isCD && !hasBomb)
                {
                    GameObject bombObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/Boss2Bomb");
                    print("生成炸弹");
                    bombObj.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 2, -1.1f);
                    bombObj.GetComponent<Rigidbody>().isKinematic = false;
                    bombObj.GetComponent<Collider>().isTrigger = false;
                    bombObj.GetComponent<Boss2Bomb>().hasPick = false;
                    hasBomb = true;
                }              
                isCD = true;
                hasRevive = false;
                nowCD = generateCD;                
                this.currentHp = maxHp;
            }            
        }
    }

    private void HideHurtEffect()
    {
        hurtEffectBullet?.SetActive(false);
        hurtEffectBlade?.SetActive(false);
    }
   
}

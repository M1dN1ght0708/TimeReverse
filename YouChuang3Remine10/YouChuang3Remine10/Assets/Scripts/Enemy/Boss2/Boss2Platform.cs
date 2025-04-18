using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Platform : MonoBehaviour
{
    public float maxHp;
    private float nowHp;
    public float probability;
    public float generateCD;
    public float nowCD;
    public bool isCD;
    [Header("受击特效")]
    public GameObject hurtEffectBlade;
    public GameObject hurtEffectBullet;
    [HideInInspector]
    public static bool hasBomb;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Attack>()!=null)
        {
            this.GetDamage(collision.GetComponent<Attack>().damage, collision.GetComponent<Attack>().attackType);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Attack>() != null&&!other.CompareTag("Boss2Bomb") && !other.CompareTag("STRocket"))
        {
            this.GetDamage(other.GetComponent<Attack>().damage,other.GetComponent<Attack>().attackType);
        }
    }
    private void GetDamage(float damage,bool attackType=false)
    {
        this.nowHp-= damage;
        if(attackType)
        {
            hurtEffectBullet.SetActive(true);
        }
        else
        {
            hurtEffectBlade.SetActive(true);
        }
        Invoke("HideHurtEffect", 0.2f);
        if(this.nowHp <=0)
        {           
            int numberBomb=UnityEngine.Random.Range(1, 101);
            if(true||numberBomb <= 100*probability)
            {
                //生成炸弹道具
                if (isCD||Boss2Platform.hasBomb)
                    return;
                GameObject bombObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/Boss2Bomb");
                bombObj.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 2, 0);
                isCD = true;
                nowCD = generateCD;
                Boss2Platform.hasBomb=true;
            }
            this.nowHp = maxHp;
        }
    }

    private void HideHurtEffect()
    {
        hurtEffectBullet?.SetActive(false);
        hurtEffectBlade?.SetActive(false);
    }
    void Start()
    {
        this.nowHp = this.maxHp;
    }

   
    void Update()
    {
        if(isCD)
        {
            nowCD -= Time.deltaTime;
            if(nowCD<=0)
            {
                isCD = false;
            }
        }
    }
}

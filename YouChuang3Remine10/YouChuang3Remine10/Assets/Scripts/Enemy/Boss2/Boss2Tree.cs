using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Boos2Tasks;
using Aura2API;
using Unity.VisualScripting;

public class Boss2Tree : BehaviorTree.Tree
{
    public bool playerDead;
    public Dictionary<string, object> blackBoard = new Dictionary<string, object>();
    [Header("UI相关")]
    public GameObject hpFadeObj;
    public GameObject hpObj;
    [Header("受击特效")]
    public GameObject hurtEffectBlade;
    public GameObject hurtEffectBullet;
    [Header("动画")]
    public SpriteRenderer sprite;
    public Animator animator;
    public int moveAniIndex;
    public int dashAniIndex;
    [Header("移动")]
    public float speed;
    public Transform[] targets;
    public int nowIndex = 0;
    public int targetIndex;
    public bool canMove = true;
    public float moveDelta;
    [Header("技能相关")]
    public int testSkillID=0;
    public int skillID;
    public Transform playerTrans;
    public GameObject aimEffect;
    public GameObject explosionEffect;
    public float rocketDelayTime=1;
    [Header("技能一")]   
    public float aimMoveSpeed=1;
    private Vector3 skillOneTraceDir;
    public float aimDelayTime;
    public bool isAim;
    public float skill1AimTime;
    public float nowAimTime1;
    public float skill1ExplosionTime;
    public float nowExplosionTime;
    private Coroutine aimCoroutine;
    [Header("技能二")]
    public float skill2AimTime;
    public float nowAimTime2;
    public int skill2MaxCount;
    public int skill2NowCount;
    public float skill2BulletSpeed;
    public float skill2DeltaTime;
    public float skill2NowDeltaTime;
    public Transform skill2Target;
    public float skill2MoveSpeed;
    [Header("技能三")]
    public Transform skill3TargetL;
    public Transform skill3TargetR;
    public float skill3DashSpeed;
    public float skill3MoveSpeed;
    public float skill3BulletSpeed;
    public float skill3BulletDeltaTime;
    public float skill3DashDistance;
    [Header("技能四")]
    public GameObject dashAttackL;
    public GameObject dashAttackR;
    public float maxDashY;
    public float minDashY;
    public GameObject skill4WarnEffect;
    public Transform skill4TargetL;
    public Transform skill4TargetR;
    public float skill4DashSpeed;
    public float skill4MoveSpeed;
    public float skill4DashDistance;
    public float skill4WarnTime;
    private Coroutine dashShadowCoro;
    public float shadowDelta;
    [Header("转阶段技能")]
    public Transform stageSkillTarget;
    public float stageSkillMoveSpeed;
    public int rocketCount;
    private int tempCount = 0;
    public float warnDelta;
    public float perWarnDelta;
    public float rocketDelta;
    public float warnDistance;
    private Coroutine warnOneCoro;
    private Coroutine warnTwoCoro;
    private Coroutine rocketOneCoro;
    private Coroutine rocketTwoCoro;
    [Header("转阶段无人机")]
    public float droneCD;
    private float nowDroneCD;
    public bool hasDrone;
    public Vector3 spawnPoint;
    public float spawnX;
    public float spawnY;
    protected override Node SetUpTree()
    {
        Node root = new Selector(new List<Node>
        {
            new StageSkillTask(this.transform),
            new BehaviorTree.Sequence(new List<Node>
            {
                new SkillIdTask(this.transform),
                new Selector(new List<Node>
                {
                     new SkillOneTask(this.transform),
                     new SkillTwoTask(this.transform),
                     new SkillThreeTask(this.transform),
                     new SkillFourTask(this.transform),
                }),
            }),           
            new MoveTask(this.transform),
            
        });
        return root;
    }

    private void Awake()
    {
        EventCenter.Instance.AddEventListener("Boss2StageAim", Boss2StageAim);
    }


    protected override void Start()
    {
        base.Start();
        nowAimTime1 = skill1AimTime;
        nowExplosionTime = skill1ExplosionTime;

        nowAimTime2 = skill2AimTime;
        skill2NowCount = skill2MaxCount;
        Invoke("ShowHpUI", 5f);
    }
    protected override void Update()
    {
        base.Update();
       if(Boss2Character.hasStageTwo)
        {
            nowDroneCD -= Time.deltaTime;
            if(nowDroneCD <=0) 
            {
                this.CallSpyDrone();
            }
        }
        print("is: "+Boss2Character.isStageTwo);
        print("has: " + Boss2Character.hasStageTwo);

    }

    public void HideExplosion()
    {
        StartCoroutine(IEHideExplosion());
    }
    private IEnumerator IEHideExplosion()
    {
        yield return new WaitForSeconds(2f);
        this.explosionEffect.SetActive(false);
    }


    public void AddData(string key, object value)
    {
        if (!blackBoard.ContainsKey(key))
        {
            blackBoard.Add(key, value);
        }
    }
    public object GetData(string key)
    {
        object value = null;
        if (blackBoard.TryGetValue(key, out value))
            return value;
        return null;
    }

    public bool RemoveData(string key)
    {
        if (blackBoard.ContainsKey(key))
        {
            blackBoard.Remove(key);
            return true;
        }
        return false;

    }
    //瞄准特效
    public void StartAim()
    {
        aimEffect.transform.position = this.playerTrans.position + new Vector3(2, 3, 0);
        aimCoroutine =StartCoroutine(AimCoroutine());
    }
    public void EndAim()
    {
        StopCoroutine(aimCoroutine);
    }
    IEnumerator AimCoroutine()
    {
        while(true)
        {
            skillOneTraceDir = this.playerTrans.position + new Vector3(0, 2, 0)- aimEffect.transform.position;
            //aimEffect.transform.position = this.playerTrans.position + new Vector3(0, 2, 0);
            aimEffect.transform.Translate(skillOneTraceDir * aimMoveSpeed * Time.deltaTime, Space.Self);
            yield return new WaitForSeconds(this.aimDelayTime);           
        }
    }
    //阶段二定时召唤无人机
    private void CallSpyDrone()
    {
        if(!hasDrone)
        {
            for(int i=0; i<2; i++)
            {
                float posX=Random.Range(-spawnX, spawnX+1);
                float posY=Random.Range(-spawnY, spawnY+1);
                GameObject spyDrone = PoolManager.Instance.GetObj("Component/Enemy/SpyDrone");
                spyDrone.transform.position=new Vector3(posX+spawnPoint.x, posY+spawnPoint.y, 0);
            }       
            hasDrone = true;
            nowDroneCD = droneCD;
        }
    }

    //阶段二额外瞄准
    private void Boss2StageAim(object info)
    {
        GameObject aimObj=PoolManager.Instance.GetObj("Effect/PlaneTarget");
        float startX = Random.Range(-2, 3);
        float startY=Random.Range(-3, 4);
        aimObj.transform.position = this.playerTrans.position + new Vector3(startX, startY, 0);
        float nowTime = Time.time;
        StartCoroutine(StageAimCoroutine(nowTime,aimObj));
    }
    IEnumerator StageAimCoroutine(float nowTime,GameObject aimObj)
    {
        while (Time.time - nowTime < this.skill1AimTime)
        {
            skillOneTraceDir = this.playerTrans.position + new Vector3(0, 2, 0) - aimObj.transform.position;
            //aimEffect.transform.position = this.playerTrans.position + new Vector3(0, 2, 0);
            aimObj.transform.Translate(skillOneTraceDir * aimMoveSpeed * Time.deltaTime, Space.Self);
            yield return new WaitForSeconds(this.aimDelayTime);
        }
        //float boomTime= Time.time;
        //while (Time.time - boomTime < this.skill1ExplosionTime) { }
        yield return new WaitForSeconds(this.skill1ExplosionTime);
        Vector3 boomPos=aimObj.transform.position;
        PoolManager.Instance.PushObj("Effect/PlaneTarget", aimObj);
        GameObject boomObj=PoolManager.Instance.GetObj("Effect/PlaneExplosion");
        boomObj.transform.position=boomPos;
        yield return new WaitForSeconds(2f);
        PoolManager.Instance.PushObj("Effect/PlaneExplosion", boomObj);
        yield break;

    }

    //残影特效
    public void TriggerShadow()
    {
        this.dashShadowCoro = StartCoroutine(TriggerShadowCoro());
    }
    public void EndShadow()
    {
        StopCoroutine(dashShadowCoro);
        print("stopcoro");
    }

    IEnumerator TriggerShadowCoro()
    {
        while(true)
        {
            yield return new WaitForSeconds(this.shadowDelta);
            PoolManager.Instance.GetObj("Shadow/PlaneShadow");
        }
    }

    //受伤死亡
    public void GetHurt(Transform attackTrans, bool attackType = false)
    {
        if (attackType)
        {
            hurtEffectBullet.SetActive(true);
            AudioMgr.Instance.PlaySoundNew(AudioID.mHurtBullet);
        }
        else
        {
            hurtEffectBlade.SetActive(true);
            AudioMgr.Instance.PlaySoundNew(AudioID.mHurtBlade);
        }
        Invoke("HideHurtEffect", 0.2f);
    }
    private void HideHurtEffect()
    {      
        hurtEffectBlade.SetActive(false);
        hurtEffectBullet.SetActive(false);
    }
    public void GetDead()
    {
        this.gameObject.Destroy();
    }
    //UI显示
    private void ShowHpUI()
    {
        hpFadeObj.SetActive(true);
        hpObj.SetActive(true);
    }
    //发射单个导弹
    public void GenerateRocketOnce(float warnPosX, float warnPosY, float rocketPosX, float rocketPosY)
    {
        StartCoroutine(IEGenerateRocketOnce(warnPosX, warnPosY, rocketPosX,rocketPosY));
    }
    IEnumerator IEGenerateRocketOnce(float warnPosX,float warnPosY,float rocketPosX,float rocketPosY)
    {
        GameObject warnObj = PoolManager.Instance.GetObj("Component/Enemy/Boss2RocketsWarn");
        warnObj.transform.position = new Vector3(warnPosX, 11, -1.1f);
        yield return new WaitForSeconds(rocketDelayTime);
        GameObject rocketObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/STRocket");
        rocketObj.transform.position = new Vector3(rocketPosX, rocketPosY, -1.2f);
        rocketObj.GetComponent<STRocket>().landSpeed = this.skill2BulletSpeed;
        yield break;
    }

    //转阶段技能特效
    public void ShowWarn()
    {
        warnOneCoro=StartCoroutine(TriggerWarnOne());
        Invoke("ShowWarnTwo", this.warnDelta);
    }
    public void ShowWarnTwo()
    {
        warnTwoCoro=StartCoroutine(TriggerWarnTwo());
    }
    IEnumerator TriggerWarnOne()
    {
        int index =0;
        print("Warn1");
        while (index<this.rocketCount)
        {
            GameObject rocketObj = PoolManager.Instance.GetObj("Component/Enemy/Boss2RocketsWarn");
            rocketObj.transform.position = new Vector3(-16+this.warnDistance*2*index, 11, -1.1f);
            index++;
            if(index>=this.rocketCount)
                yield break;
            yield return new WaitForSeconds(this.perWarnDelta);
        }                
    }

    IEnumerator TriggerWarnTwo()
    {
        int index = 0;
        print("Warn2");
        while (index < this.rocketCount)
        {
            GameObject rocketObj = PoolManager.Instance.GetObj("Component/Enemy/Boss2RocketsWarn");
            rocketObj.transform.position = new Vector3(-16+ this.warnDistance + this.warnDistance*2 * index, 11, -1.1f);
            index++;
            if (index >= this.rocketCount)
            {
                print("Warn2End");
                GenerateRocketsOne();
                Invoke("GenerateRocketsTwo", rocketDelta);
                yield break;
            }
            yield return new WaitForSeconds(this.perWarnDelta);
        }
    }
    private void GenerateRocketsOne()
    {
        int index = 0;
        print("Generate1");
        while(index < this.rocketCount)
        {
            GameObject rocketObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/STRocket");
            rocketObj.transform.position = new Vector3(-16 + this.warnDistance * 2 * index, 22, -1.2f);
            rocketObj.GetComponent<STRocket>().landSpeed = this.skill2BulletSpeed;
            index++;
        }
    }    
    private void GenerateRocketsTwo()
    {
        int index = 0;
        print("Generate2");
        while (index < this.rocketCount)
        {
            GameObject rocketObj = PoolManager.Instance.GetObj("Bullet/EnemyBullet/STRocket");
            rocketObj.transform.position = new Vector3(-16 + this.warnDistance + this.warnDistance * 2 * index, 22, -1.2f);
            rocketObj.GetComponent<STRocket>().landSpeed = this.skill2BulletSpeed;
            index++;            
        }
        Invoke("OnConnectedToServer", 1f);
    }
    private void OnConnectedToServer()
    {
        if (!Boss2Character.hasStageTwo)
        {
            Boss2Character.hasStageTwo = true;
        }
    }
}

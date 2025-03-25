using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Boos2Tasks;

public class Boss2Tree : BehaviorTree.Tree
{
    public Dictionary<string, object> blackBoard = new Dictionary<string, object>();
    [Header("移动")]

    public float speed;
    public Transform[] targets;
    public int nowIndex = 0;
    public int targetIndex;
    public bool canMove = true;
    public float moveDelta;
    [Header("技能相关")]
    public int skillID;
    public Transform playerTrans;
    public GameObject aimEffect;
    public GameObject explosionEffect;
    [Header("技能一")]
    public float skill1AimTime;
    public float nowAimTime1;
    public float skill1ExplosionTime;
    public float nowExplosionTime;
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
    public GameObject skill4WarnEffect;
    public Transform skill4TargetL;
    public Transform skill4TargetR;
    public float skill4DashSpeed;
    public float skill4MoveSpeed;
    public float skill4DashDistance;
    public float skill4WarnTime;
    protected override Node SetUpTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
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
    protected override void Start()
    {
        base.Start();
        nowAimTime1 = skill1AimTime;
        nowExplosionTime = skill1ExplosionTime;

        nowAimTime2 = skill2AimTime;
        skill2NowCount = skill2MaxCount;
    }
    protected override void Update()
    {
        base.Update();
       
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


}

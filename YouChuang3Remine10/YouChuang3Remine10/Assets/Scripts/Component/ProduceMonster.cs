using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceMonster : MonoBehaviour
{
    private bool canProuceMonster;
    public string cageName;
    public float deltaTime;
    private float timeCounter=0;
    public GameObject receiverObj;
    private Receiver receiver;

    [Header("巡逻点")]
    public Transform[] walkPoints;
    [Header("追击点")]
    public Transform[] runPoints;
    [Header("刷怪位置偏移")]
    public float offsetX;
    public float offsetY;

    int testCount = 0;
    private void Awake()
    {
        //receiver =receiverObj?.GetComponent<Receiver>();
    }
    void Start()
    {
        EventCenter.Instance.AddEventListener(cageName, this.BeginProduceMonsters);
    }



    void Update()
    {
        //if (this.canProuceMonster)
        //{
        //    this.BeginProduceMonsters();
        //}
        //if(receiver.hasReceive)
        //{
        //    this.BeginProduceMonsters();
        //}
        
    }
    private void CanProduceMonsters(object info)
    {
        this.canProuceMonster = true;
    }
    private void BeginProduceMonsters(object info)
    {
        //timeCounter += Time.deltaTime;
        //if (timeCounter >= this.deltaTime)
        //{
        //    timeCounter = 0;
        //    testCount++;
        //    //生成怪物
        //    print("生成怪物" + testCount);
        //    //设置位置
        //}
        float posX = Random.Range(-offsetX, offsetX + 1);
        float posY = this.transform.position.y;
        posX += this.transform.position.x;
        GameObject e1bObj = PoolManager.Instance.GetObj("Component/Enemy/Enemy1BladeL2");
        e1bObj.transform.position = new Vector3(posX, posY, -1.5f);
        E1B e1b=e1bObj.GetComponent<E1B>();
        for(int i= 0; i < this.walkPoints.Length; i++)
        {
            e1b.walkPoints[i] = this.walkPoints[i];
            e1b.runPoints[i]= this.runPoints[i];
        }

        posX = Random.Range(-offsetX, offsetX + 1);
        posX += this.transform.position.x;
        GameObject e1gObj = PoolManager.Instance.GetObj("Component/Enemy/Enmey1GunL2");
        e1gObj.transform.position = new Vector3(posX, posY, -1.5f);
        E1G e1g = e1gObj.GetComponent<E1G>();
        for (int i = 0; i < this.walkPoints.Length; i++)
        {
            e1g.walkPoints[i] = this.walkPoints[i];
            e1g.runPoints[i] = this.runPoints[i];
        }


    }
}

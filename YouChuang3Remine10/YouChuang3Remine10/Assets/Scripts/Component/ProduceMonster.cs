using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceMonster : MonoBehaviour
{
    private bool canProuceMonster;
    public string cageName;
    public float deltaTime;
    private float timeCounter=0;

    int testCount = 0;
    void Start()
    {
        EventCenter.Instance.AddEventListener(cageName, this.CanProduceMonsters);
    }



    void Update()
    {
        if (this.canProuceMonster)
        {
            this.BeginProduceMonsters();
        }
    }
    private void CanProduceMonsters(object info)
    {
        this.canProuceMonster = true;
    }
    private void BeginProduceMonsters()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter >= this.deltaTime)
        {
            timeCounter = 0;
            testCount++;
            //���ɹ���
            print("���ɹ���" + testCount);
            //����λ��
        }
    }
}

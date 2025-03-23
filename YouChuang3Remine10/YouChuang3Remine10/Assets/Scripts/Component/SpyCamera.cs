using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyCamera : BaseGameLevel
{
    public string spyCameraName;
    public GameObject redCamera;
    public GameObject blueCamera;
    public GameObject redBase;
    public GameObject blueBase;

    public float exitTime;
    public string cageName;
    private float nowExitTime;
    private bool isFindPlayer;


    public float speed;
    public float maxRotation;
    public float currentSpeed;
    public Transform target;
    public int rotateDirection;
    public float rotateCounter = 0;

    void Start()
    {
        this.nowExitTime = this.exitTime;
        currentSpeed= this.speed;
        EventCenter.Instance.AddEventListener(spyCameraName, SwitchRedOrBule);
    }

    private void SwitchRedOrBule(object isRed)
    {
        this.redCamera.SetActive((bool)isRed);
        this.blueCamera.SetActive((bool)isRed);
        this.redBase.SetActive((bool)isRed);
        this.blueBase.SetActive((bool)isRed);

    }

    void Update()
    {
        //ÉãÏñÍ·ÔË¶¯
        rotateCounter += currentSpeed * Time.deltaTime * rotateDirection;
        this.transform.RotateAround(target.position, target.forward, currentSpeed * Time.deltaTime * rotateDirection);
        if (Mathf.Abs(rotateCounter) - maxRotation > 0.1f)
        {
            rotateDirection *= -1;
        }
    }
}

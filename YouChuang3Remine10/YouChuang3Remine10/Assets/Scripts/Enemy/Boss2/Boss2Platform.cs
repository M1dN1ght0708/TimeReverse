using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss2Platform : PlatformV
{
    private Rigidbody rigidbody;
    private Collider collider;
    private Boss2PlatformCharacter character;
    public GameObject goodPlatform;
    public GameObject brokenPlatform;
    public bool isStageTwo;
    public bool hasStageTwo;
    public float upSpeed;
    public float downSpeed;
    public Vector3 startPos;
    public Vector3 endPos;
    public bool isUp;
    public bool isDown;
    public bool hasDown;
    public float verticalSpeed;
    private bool inTrigger;
    public GameObject brokenEffect1;
    public GameObject brokenEffect2;
    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        collider = this.GetComponent<Collider>();
        character=this.GetComponent<Boss2PlatformCharacter>();
        EventCenter.Instance.AddEventListener("PlaneStageTwo",PlaneStageTwo);
    }
    private void OnEnable()
    {
        this.isCanMove = false;
        this.verticalSpeed = this.upSpeed;
        this.isUp = true;
        this.isDown = false;
        this.hasDown = false;
    }

    void Start()
    {
        this.isUp = false;
        this.isDown=false;
        this.hasDown = false;
        this.isCanMove=false;
    }

   
    protected override void Update()
    {
        if (character.currentHp <= character.maxHp / 2||this.isDown)
        {
            this.brokenEffect1.SetActive(true);
            this.brokenEffect2.SetActive(true);
            this.goodPlatform.SetActive(false);
            this.brokenPlatform.SetActive(true);
        }
        else
        {
            this.brokenEffect1.SetActive(false);
            this.brokenEffect2.SetActive(false);
            this.goodPlatform.SetActive(true);
            this.brokenPlatform.SetActive(false);
        }
        if (Boss2Character.isStageTwo)
        {
            this.isCanMove = true;
            this.isStageTwo = true;
        }

        if(character.hasRevive)
        {
            this.isUp=true;
            this.hasDown = false;
            character.hasRevive=false;
            print("¿ªÊ¼ÉÏÉý");
        }
        if(character.isBroken&&!this.hasDown)
        {           
            this.isDown = true;
            //character.isBroken=false;
        }        

        if (hasDown)
            return;      
        if ((isInRange && PlayerController.isRangeStopTime)&&!isDown)
        {
            audioSource.Stop();
            StopTimeDo();
        }
        else
        {
            if (isCanMove && !audioSource.isPlaying)
                audioSource.Play();
            currentSpeed = speed;
            if(isUp)
                verticalSpeed = upSpeed;
            else if(isDown)
                verticalSpeed = downSpeed;

        }
        if (isCanMove && !isDown && !isUp&&!hasDown)
        {
            PlatformMove();
        }
        this.Boss2PlatformUp();
        this.Boss2PlatformDown();
        EndRangeTimeStop();
    }
    private void Boss2PlatformUp()
    {
        if(isUp)
        {
            hasDown = false;
            this.transform.Translate(this.transform.up * verticalSpeed * Time.deltaTime, Space.World);
            if(!inTrigger&&this.transform.position.y>-0.7f)
                this.collider.isTrigger = false;
            if (Mathf.Abs(startPos.y - this.transform.position.y) < 0.1f)
            {
                if(!inTrigger)
                {
                    this.collider.isTrigger = false;
                }
                isUp = false;
                /*if (!this.hasStageTwo)
                    this.hasStageTwo = true;*/
                if(this.isStageTwo)
                    this.isCanMove = true;
                else
                    this.isCanMove = false;
            }
        }

    }
    private void Boss2PlatformDown()
    {
        if (isDown&&!hasDown)
        {
            this.collider.isTrigger = true;
            this.transform.Translate(-this.transform.up * verticalSpeed * Time.deltaTime, Space.World);
            if (Mathf.Abs(endPos.y - this.transform.position.y) <0.5f)
            {
                print(endPos.y - this.transform.position.y);
                isDown = false;
                hasDown = true;
            }
        }
    }
    private void PlaneStageTwo(object info)
    {
        this.isCanMove = true;
        this.isStageTwo = true;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(other.CompareTag("Player"))
            this.inTrigger = true;
        else
            this.inTrigger = false;
    }
    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        if (other.CompareTag("Player"))
            this.inTrigger = true;
        else
            this.inTrigger = false;
    }
    private void OnTriggerExit(Collider other)
    {
        if (isDown || isUp ||hasDown)
            return;
        if(other.CompareTag("Player"))
        {
            this.collider.isTrigger = false;
            this.inTrigger = false;
        }
    }
    protected override void StopTimeDo()
    {
        base.StopTimeDo();
        //this.verticalSpeed = 0;
    }
}

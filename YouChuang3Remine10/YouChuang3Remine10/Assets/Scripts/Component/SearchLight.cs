using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchLight : BaseGameLevel
{
    public GameObject warnningEffect;

    public GameObject blueLight;
    public GameObject redLight;
    //public Material matRed;
    //public Material matBlue;
    private MeshRenderer meshR;

    public string cageName;
    public string spyCameraName;

    public float exitTime;
    private float nowExitTime;
    private bool isFindPlayer;


    private void Awake()
    {
        this.meshR=this.GetComponent<MeshRenderer>();
    }
    void Start()
    {
        this.nowExitTime = this.exitTime;
    }

   
    void Update()
    {
        //发现玩家
        if (isFindPlayer)
        {
            nowExitTime -= Time.deltaTime;
            if(nowExitTime <= 0)
            {
                isFindPlayer = false;
                nowExitTime = this.exitTime;
                blueLight.SetActive(true);
                redLight.SetActive(false);
                EventCenter.Instance.TriggerEvent(this.spyCameraName, false);
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("发现玩家");
            isFindPlayer = true;
            nowExitTime = this.exitTime;
            //meshR.material=matRed;
            blueLight.SetActive(false);
            redLight.SetActive(true);
            EventCenter.Instance.TriggerEvent(this.cageName, null);
            EventCenter.Instance.TriggerEvent(this.spyCameraName, true);
            this.Warnning();
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            isFindPlayer = true;
            //meshR.material=matRed;
            blueLight.SetActive(false);
            redLight.SetActive(true);
            EventCenter.Instance.TriggerEvent(this.cageName,null);
            EventCenter.Instance.TriggerEvent(this.spyCameraName, true);
            this.Warnning();
        }
    }
    private void Warnning()
    {
        if(warnningEffect != null)
            warnningEffect.SetActive(true);
    }
}

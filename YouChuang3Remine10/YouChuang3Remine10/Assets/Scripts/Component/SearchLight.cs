using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchLight : BaseGameLevel
{
    public GameObject warnningEffect;

    public Material matRed;
    public Material matBlue;
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
        //∑¢œ÷ÕÊº“
        if (isFindPlayer)
        {
            nowExitTime -= Time.deltaTime;
            if(nowExitTime <= 0)
            {
                isFindPlayer = false;
                nowExitTime = this.exitTime;
            }

        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            isFindPlayer = true;
            meshR.material=matRed;
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

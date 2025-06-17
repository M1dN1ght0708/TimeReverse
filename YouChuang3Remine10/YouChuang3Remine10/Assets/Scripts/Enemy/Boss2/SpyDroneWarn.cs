using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyDroneWarn : MonoBehaviour
{
    private SpyDrone spyDrone;

    public GameObject blueLight;
    public GameObject redLight;

    public float exitTime;
    private float nowExitTime;
    private bool isFindPlayer;

    public float aimCD;
    public float nowCD;
    private void Awake()
    {
        spyDrone=this.GetComponentInParent<SpyDrone>();
    }
    private void OnEnable()
    {
        isFindPlayer = false;
        nowExitTime = 0;
        nowCD = 0;
    }

    void Update()
    {
        if(isFindPlayer)
        {
            if(nowCD > 0)
                nowCD-=Time.deltaTime;           
            nowExitTime += Time.deltaTime;
            if(nowExitTime >= exitTime ) 
            { 
                isFindPlayer = false;
                nowExitTime = 0;
                blueLight.SetActive(true);
                redLight.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")&&!spyDrone.beStop)
        {
            print("发现玩家");
            isFindPlayer = true;
            nowExitTime = 0;
            //meshR.material=matRed;
            blueLight.SetActive(false);
            redLight.SetActive(true);
            if(nowCD<=0)
            {
                nowCD = aimCD;
                print("触发额外瞄准");
                EventCenter.Instance.TriggerEvent("Boss2StageAim",null);
            }                   
        }
    }
}

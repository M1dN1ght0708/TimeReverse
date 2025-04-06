using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneDashShadow : MonoBehaviour
{
    private Transform planeTrans;
    private Transform planeSprite;
    private SpriteRenderer planeSpriteRender;
    private SpriteRenderer shadowSpriteRender;
    private int dir;

    public float offsetX;
    public float offsetY;

    private Color color;
    [Header("时间控制参数")]
    public float activeTime;  //显示时间
    public float activeStart; //开始显示的时间

    [Header("不透明度控制")]
    public float alpha;
    public float alphaStart; //初始值
    public float alphaAtten;  //透明度衰减
    private void OnEnable()
    {
        planeTrans = GameObject.Find("PlaneBoss").transform;
        planeSprite = planeTrans.Find("PlaneSprite");
        shadowSpriteRender = this.GetComponent<SpriteRenderer>();
        planeSpriteRender = planeSprite.GetComponent<SpriteRenderer>();

        alpha = alphaStart;
        shadowSpriteRender.sprite = planeSpriteRender.sprite;
        if(planeSpriteRender.flipX)
            dir = 1;
        else
            dir = -1;
        this.transform.position = new Vector3(planeTrans.position.x + offsetX*-dir , planeTrans.position.y + offsetY, planeTrans.position.z);
        this.shadowSpriteRender.flipX = planeSpriteRender.flipX;

        activeStart = Time.time;
    }
    void Update()
    {
        alpha -= alphaAtten;
        color = new Color(0.5f, 0.5f, 1f, alpha);

        shadowSpriteRender.color = color;
        if (Time.time >= activeStart + activeTime)
        {
            PoolManager.Instance.PushObj("Shadow/PlaneShadow", this.gameObject);
        }
    }
}

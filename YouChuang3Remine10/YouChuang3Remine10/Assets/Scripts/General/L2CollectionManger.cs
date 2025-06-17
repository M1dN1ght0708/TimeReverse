using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class L2CollectionManger : MonoBehaviour
{
    // Start is called before the first frame update
    private static L2CollectionManger instance;
    public static L2CollectionManger Instance => instance;

    public int L2CollectionCount = 0;
    public GameObject collectCountUI;
    public GameObject collectTextObj;
    private Text collectTipText;

    private bool hasTrigger;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        collectTipText = collectTextObj.GetComponent<Text>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(L2CollectionCount==6&&!hasTrigger)
        {
            hasTrigger = true;
            //触发事件
        }
        if (hasTrigger)
            return;
        if(L2CollectionCount>0)
        {
            collectCountUI.SetActive(true);
            //if (collectTipText == null)
            //{
            //    collectTipText = collectTextObj.GetComponent<Text>();
            //}
            collectTipText.text = "当前收集进度：" + L2CollectionCount + "/6";
        }
        else
        {
            collectCountUI.SetActive(false);
        }


    }
}

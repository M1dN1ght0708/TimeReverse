using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTips : MonoBehaviour
{
    public GameObject tipObj;
    public GameObject[] uiObjs;
    public GameObject[] tipObjs;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tipObj.SetActive(true);
            if (uiObjs.Length <= 0 || tipObjs.Length <= 0)
                return;
            for (int i = 0; i < uiObjs.Length; i++)
            {
                uiObjs[i].transform.position= Camera.main.WorldToScreenPoint(tipObjs[i].transform.position);
                uiObjs[i].GetComponent<UITrack>().targetTrans = tipObjs[i].transform;
            }           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tipObj.SetActive(false);
        }
    }
}

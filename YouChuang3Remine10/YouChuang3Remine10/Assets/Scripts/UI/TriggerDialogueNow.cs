using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogueNow : MonoBehaviour
{
    private bool hasTrigger;
    private void Awake()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!hasTrigger)
            {
                EventCenter.Instance.TriggerEvent("TriggerDialogue", null);
                hasTrigger = true; 
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}

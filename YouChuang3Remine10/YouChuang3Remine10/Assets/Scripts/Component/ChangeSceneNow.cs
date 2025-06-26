using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneNow : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            MySceneManager.Instance.ChangeSceneTo(this.gameObject.name);
        }      
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControllerL2 : MonoBehaviour
{
    private PlayerController pController;

    private void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pController = other.GetComponent<PlayerController>();
            pController.playerInput.GamePlay.Disable();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            pController = other.GetComponent<PlayerController>();
            pController.playerInput.GamePlay.Disable();
        }    
    }
    private void OnDisable()
    {
        pController.playerInput.GamePlay.Enable();
    }
}

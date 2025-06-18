using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Server : MonoBehaviour
{
    // Start is called before the first frame update
    public int id=0;
    public GameObject fTip;
    public PlayerControllerInput playerInput;
    public bool canF;
    private bool hasCollect;
    private void Awake()
    {
        playerInput = new PlayerControllerInput();
        playerInput.UI.Interact.started += playerInteractServer;
    }


    void Start()
    {
        this.playerInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(L2CollectionManger.Instance.collectedID[this.id])
        {
            this.hasCollect = true;
            fTip.SetActive(false);
        }    
            
    }
    private void playerInteractServer(InputAction.CallbackContext context)
    {
        if(this.canF&&!this.hasCollect)
        {
            this.hasCollect = true;
            L2CollectionManger.Instance.L2CollectionCount++;
            L2CollectionManger.Instance.collectedID[this.id] = true;
            fTip.SetActive(false);
            canF = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")&&!this.hasCollect)
        {
            fTip.SetActive(true);
            canF = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !this.hasCollect)
        {
            fTip.SetActive(true);
            canF = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fTip.SetActive(false);
            canF = false;
        }
    }
}

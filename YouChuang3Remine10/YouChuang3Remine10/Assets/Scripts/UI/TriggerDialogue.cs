using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public TextAsset textAsset;
    public TextAsset finishedAsset;
    public GameObject npc;

    private PlayerController pController;
    private PlayerControllerNew pControllerNew;
    private DialogueUIMgr dialogueUIMgr;

    public GameObject afterDialogueTrigger;
    public GameObject afterDialogueHide;

    //public bool hasTips;

    private bool hasTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pControllerNew = other.GetComponent<PlayerControllerNew>();
            if(pControllerNew != null )
            {
                print("触发对话New");
                dialogueUIMgr = other.GetComponentInChildren<DialogueUIMgr>();
                pControllerNew.canDialogue = true;
                dialogueUIMgr.textAsset = this.textAsset;
                dialogueUIMgr.afterDialogueTrigger = this.afterDialogueTrigger;
                dialogueUIMgr.afterDialogueHide = this.afterDialogueHide;
                if (finishedAsset != null)
                    dialogueUIMgr.finishedTextAsset = this.finishedAsset;
                dialogueUIMgr.npc = this.npc;
                if (!hasTrigger)
                {
                    hasTrigger = true;
                    dialogueUIMgr.isFinished = false;
                }
            }
            else
            {
                print("触发对话");
                pController = other.GetComponent<PlayerController>();
                dialogueUIMgr = other.GetComponentInChildren<DialogueUIMgr>();
                pController.canDialogue = true;
                dialogueUIMgr.textAsset = this.textAsset;
                dialogueUIMgr.afterDialogueTrigger = this.afterDialogueTrigger;
                dialogueUIMgr.afterDialogueHide = this.afterDialogueHide;
                if (finishedAsset != null)
                    dialogueUIMgr.finishedTextAsset = this.finishedAsset;
                dialogueUIMgr.npc = this.npc;
                if (!hasTrigger)
                {
                    hasTrigger = true;
                    dialogueUIMgr.isFinished = false;
                }
            }
           
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            pController = other.GetComponent<PlayerController>();
            dialogueUIMgr = other.GetComponentInChildren<DialogueUIMgr>();
            pController.canDialogue = false;
            dialogueUIMgr.isShowing = false;
            dialogueUIMgr.skip=false;
            dialogueUIMgr.index=0;
            dialogueUIMgr.dialogueBox.SetActive(false);
        }
    }

}

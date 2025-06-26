using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public TextAsset textAsset;
    public TextAsset finishedAsset;
    public GameObject npc;

    private PlayerController pController;
    private PlayerControllerNew pControllerNew;
    private DialogueUIMgr dialogueUIMgr;

    public GameObject[] afterDialogueTriggers;
    public GameObject[] afterDialogueHides;

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
                pControllerNew.canDialogue = true;
            }
            else
            {
                print("触发对话");
                pController = other.GetComponent<PlayerController>();
                pController.canDialogue = true;
            }
            dialogueUIMgr = other.GetComponentInChildren<DialogueUIMgr>();
            dialogueUIMgr.textAsset = this.textAsset;
            int count = math.min(dialogueUIMgr.afterDialogueTriggers.Length, this.afterDialogueTriggers.Length);
            for (int i = 0; i < count; i++)
            {
                dialogueUIMgr.afterDialogueTriggers[i] = this.afterDialogueTriggers[i];
            }
            count = math.min(dialogueUIMgr.afterDialogueHides.Length, this.afterDialogueHides.Length);
            for (int i = 0; i < count; i++)
            {
                dialogueUIMgr.afterDialogueHides[i] = this.afterDialogueHides[i];
            }
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

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            print("退出对话");
            pController = other.GetComponent<PlayerController>();
            dialogueUIMgr = other.GetComponentInChildren<DialogueUIMgr>();
            pController.canDialogue = false;
            dialogueUIMgr.isShowing = false;
            dialogueUIMgr.skip=false;
            dialogueUIMgr.index=0;
            dialogueUIMgr.dialogueBox.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if(pController != null)
            pController.canDialogue=false;
        if (pControllerNew != null)
            pControllerNew.canDialogue = false;
    }

}

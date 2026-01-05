using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueInst dialogue;

    public void TriggerDialogue()
    {
        //find object of type is extremely expensive - but I realize this is a test and so it doesnt matter
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}

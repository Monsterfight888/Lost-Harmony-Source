using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpeakAfterQuest : NPC
{
    public DialogueInst[] afterDialogues; 
    private DialogueInst[] beforeDialogues;
    public DialogueInst[] middleTLDR;
    public int levels = 1;

    public Vector3Int[] barriersBeforeQuest;

    public int FirstLevelNumber;
    bool shouldUnlock = true;
    public override void Start()
    {
        if (SceneMessenger.instance.CompletedGames >= FirstLevelNumber + levels)
        {
            beforeDialogues = dialogues;
            dialogues = afterDialogues;
            shouldUnlock = false;
        }
        if (SceneMessenger.instance.failed)
        {
            dialogueIndex = dialogues.Length - 1;
        }
        else if(SceneMessenger.instance.CompletedGames < FirstLevelNumber + levels && SceneMessenger.instance.CompletedGames > FirstLevelNumber)
        {
            beforeDialogues = dialogues;
            dialogues = middleTLDR;
        }
        SceneMessenger.instance.npcDomain.Add(this);
        base.Start();
        if (SceneMessenger.instance.CompletedGames < FirstLevelNumber + levels)
        {
            for (int i = 0; i < barriersBeforeQuest.Length; i++)
            {
                worldData.UnWalkablePos.Add(worldData.map.WorldToCell(barriersBeforeQuest[i]));
            }
        }
    }
    public override void Update()
    {
    }
    public override void Notify()
    {
        if (dialogues.Length - 1 == dialogueIndex /*&& SceneMessenger.instance.CompletedGames >= FirstLevelNumber*/ && shouldUnlock)
        {
            worldData.thisPaintingHolder.RemovePadlock(SceneMessenger.instance.CompletedGames);
            shouldUnlock = false;
        }
        base.Notify();
        /*if(SceneMessenger.instance.CompletedGames >= GamesCompleteBeforeTalk)
        {
            if (dialogues.Length != 0)
            {
                worldData.dialogues.Remove(dialogueOutThere);
                dialogues = beforeDialogues;

                worldData.dialogues.Add(dialogues[0]);
                dialogueOutThere = dialogues[0];
                ActivateDialogues();
            }
        }*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandpa : NPC
{
    int counter = 0;
    public int numToWalk;
    public DialogueInst[] afterDialoguesL1;
    public DialogueInst[] MiddleDialogue;
    bool shouldUnlock = true;
    public override void Start()
    {
        if (SceneMessenger.instance.CompletedGames > 1)
        {
            SetPosition(new Vector3(-33, 12, 0));
            dialogues = afterDialoguesL1;
            rend.flipX = false;
            shouldUnlock = false;
        }
        else if (SceneMessenger.instance.CompletedGames > 0)
        {
            SetPosition(new Vector3(-33, 12, 0));
            dialogues = MiddleDialogue;
            rend.flipX = false;
            //dialogues = null;
        }
        base.Start();

    }
    public override void Notify()
    {
        float raduis = dialogues[dialogueIndex].raduis;

        //if time fix player unlocking early
        if (dialogues.Length - 1 == dialogueIndex && shouldUnlock)
        {
            worldData.thisPaintingHolder.RemovePadlock(SceneMessenger.instance.CompletedGames);
            shouldUnlock = false;
        }
        base.Notify();

        /*if (SceneMessenger.instance.CompletedGames > 0)
        {

        }
        else
        {
            
                base.Notify();
        }*/
        if (counter == numToWalk && SceneMessenger.instance.CompletedGames <= 0)
        {
            worldData.UnWalkablePos.Remove(thisPos);
            NextPath();
            GetComponent<Animator>().Play("Base Layer.GranpaRun");
        }
        counter++;

        /*if(dialogues.Length <= dialogueIndex && SceneMessenger.instance.hasDoneIntro)
        {
            //dont have time for smart system, removes the barrier to mansion after you talk to mr. grandpa
            worldData.UnWalkablePos.Remove(worldData.map.WorldToCell(new Vector3Int(-19, -5, 0)));
            worldData.UnWalkablePos.Remove(worldData.map.WorldToCell(new Vector3Int(-18, -5, 0)));
            
        }
        else if (dialogues.Length <= dialogueIndex)
        {
            SceneMessenger.instance.hasDoneIntro = true;
        }*/

        //dumb but works for now

        if(raduis == 0.05f)
        {
            if (worldData.colorEnum != null)
            {

                worldData.StopCoroutine(worldData.colorEnum);
                worldData.colorEnum = null;
            }
            //IEnumerator thing = 
            IEnumerator thign = worldData.ChangeRaduis(0.05f, 3f, true);
            worldData.StartCoroutine((thign));
            worldData.musicPlayer.Stop();
        }
        else if (raduis != 0)
        {

            worldData.colorEnum = worldData.ChangeRaduis(raduis, 5f, true);
            worldData.StartCoroutine(worldData.colorEnum);
        }
    }
}

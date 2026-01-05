using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPC : Character
{
    public Path[] paths;
    int[] currentPathData;
    int currentPathIndex = 0;

    public bool nextPathOnDialogue = false;

    public DialogueInst[] dialogues;
    protected int dialogueIndex = 0;
    public Vector3Int thisPos;
    public Material GreyScaleMat; //so can light up areas
    protected DialogueInst dialogueOutThere;

    public GameObject TextBoxNotify;

    public SpriteRenderer rend;


    public override void Start()
    {
        base.Start();
        ActivateDialogues();
        dialogueOutThere = dialogues[0];
        worldData.dialogues.Add(dialogues[0]);
        thisPos = worldData.map.WorldToCell(transform.position);
        worldData.UnWalkablePos.Add(thisPos);
        TextBoxNotify = transform.Find("TextNotify").gameObject;
        //NextPath();
        /*if (dialogues.Length - 1 == dialogueIndex)
        {
            TextBoxNotify.SetActive(false);
        }*/
    }
    public void ActivateDialogues()
    {

        for (int i = 0; i < dialogues.Length; i++)
        {
            dialogues[i].NotifyReciever = this;

            /*Vector3Int movingDir = Vector3Int.zero;
            if (currentPathData != null && currentPathData.Length > currentPathIndex && currentPathData[currentPathIndex] != 0)
            {
                movingDir = currentPathData[currentPathIndex] % 2 == 0 ? Vector3Int.right : Vector3Int.up;
                movingDir * currentPathData[currentPathIndex]; simpler just to reset it - im getting buried in my workarounds
            }*/

            dialogues[i].coord = worldData.map.WorldToCell(snapPos);
        }
    }

    public void NextPath()
    {
        currentPathData = new int[paths[0].pathData.Length];

        for (int i = 0; i < currentPathData.Length; i++)
        {
            currentPathData[i] = paths[0].pathData[i];

        }

        for (int i = 0; i < dialogues.Length; i++)
        {
            dialogues[i].coord = new Vector3Int(500, 500, 0);
        }
        currentPathIndex = 0;
    }
    public void FollowPath(Path newPath)
    {
        currentPathData = new int[newPath.pathData.Length];

        for (int i = 0; i < currentPathData.Length; i++)
        {
            currentPathData[i] = newPath.pathData[i];

        }
        currentPathIndex = 0;
    }
    public override bool Move(Vector3 movementVector)
    {
        bool returner = base.Move(movementVector);
        if (currentPathIndex >= currentPathData.Length - 1)//means path finished & minus one bc of 0, length always more than path index
        {
            ActivateDialogues();
        }
        return returner;
    }
    public override void Update()
    {
        if (!inTransition)
        {
            /*string str = "";
            for (int i = 0; i < currentPathData.Length; i++)
            {
                str += currentPathData[i] + " ";
            }
            print(str);*/
            PositionSnap();


            if (currentPathData != null && currentPathIndex >= currentPathData.Length)
            {
                currentPathData = null;

                if (GetComponent<Animator>())
                {
                    GetComponent<Animator>().Play("Base Layer.New State");
                    rend.flipX = false;
                }
            }

            if (currentPathData != null && currentPathData[currentPathIndex] == 0)
            {
                currentPathIndex++;
                worldData.UnWalkablePos.Remove(thisPos);
                thisPos = worldData.map.WorldToCell(transform.position);
                worldData.UnWalkablePos.Add(thisPos);
            }
            if (currentPathData != null && currentPathIndex < currentPathData.Length && currentPathData[currentPathIndex] != 0)
            {
                int movementVector = Mathf.RoundToInt(Mathf.Sign(currentPathData[currentPathIndex])); //make movement vector handle the sign of move and also the decrement/incrementation

                currentPathData[currentPathIndex] -= movementVector;
                if (currentPathIndex % 2 == 0) //x movement
                {
                    Move(Vector3.right * movementVector);
                }
                else //y movement
                {
                    Move(Vector3.up * movementVector);

                }
            }

        }

        base.Update();
    }

    public virtual void Notify()
    {
        bool shouldRemove = true;
        if (dialogues.Length - 1 == dialogueIndex)
        {
            shouldRemove = false;
            TextBoxNotify.SetActive(false);


        }
        else
        {

            TextBoxNotify.SetActive(true);
        }
        //next thing to do, dissable dialogues when npcs are walking so you cant talk to where the npc's dialogue point was
        if (shouldRemove)
        {

            worldData.dialogues.Remove(dialogues[dialogueIndex]);
            dialogueIndex++;
            if (dialogues.Length > dialogueIndex)
            {
                worldData.dialogues.Add(dialogues[dialogueIndex]);
                dialogueOutThere = dialogues[dialogueIndex];
            }
        }


        if (nextPathOnDialogue)
        {

        }
        else
        {

        }
        ActivateDialogues();
        //for lighting up world
        Debug.Log("Base notification");
    }

}

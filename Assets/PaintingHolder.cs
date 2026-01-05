using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingHolder : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject[] paintings;

    private GameObject[] padLocks;


    private AudioSource paintingAudio;

    //public int paintingsNum;

    private WorldData worldData;

    public string[] paintingSceneList;
    int accessableLevels = 0;

    public bool gameFinished = false;//when game finish all paitngs are fair game
    private void Awake()
    {
        paintings = new GameObject[paintingSceneList.Length];
        for (int i = 0; i < paintings.Length; i++)
        {
            paintings[i] = transform.Find("Painting " + (i + 1)).gameObject;
        }
        padLocks = new GameObject[paintingSceneList.Length];

        for (int i = 0; i < padLocks.Length; i++)
        {
            padLocks[i] = paintings[i].transform.Find("Padlock").gameObject;
        }
    }
    private void Start()
    {
        worldData = WorldData.instance;
        if (SceneMessenger.instance.CompletedGames > 0)
        {
            SetPaintingsWithPadlocksLinear(SceneMessenger.instance.CompletedGames);
            //RemovePadlock(SceneMessenger.instance.CompletedGames);

        }

        paintingAudio = GetComponent<AudioSource>();
    }

    public void SetPaintingsWithPadlocksLinear(int levelsCompleted)
    {
        for (int i = 0; i < levelsCompleted; i++)
        {
            if(i < paintings.Length)
            {
                paintings[i].GetComponent<Animator>().SetTrigger("InstantBreak");
                accessableLevels++;

            }
        }
    }
    public void RemovePadlock(int levelsCompleted)
    {
        paintings[levelsCompleted].GetComponent<Animator>().SetTrigger("Break");
        paintingAudio.Play();
        //just do an animation that sets inactive & remove above line of code
    }

    public string PaintingAvailableAtLocation(Vector3Int pos)
    {
        string returner = "";
        for (int i = 0; i < paintings.Length; i++)
        {
            if(!padLocks[i].activeSelf)
            {
                if(worldData.map.WorldToCell(paintings[i].transform.position) == pos)
                {
                    if (!gameFinished)
                    {
                        if((SceneMessenger.instance.CompletedGames) == i)
                        {
                            returner = paintingSceneList[i];

                        }
                    }
                    else
                    {
                        returner = paintingSceneList[i];

                    }
                }
            }
        }

        return returner;

    }

    
}

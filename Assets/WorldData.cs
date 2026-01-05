using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
public class WorldData : MonoBehaviour
{
    #region Singleton
    public static WorldData instance;//singleton creation
    [System.NonSerialized]
    public bool[,] walkMap;
    public DialogueInst[] StartingDialogues;

    private float raduisCircle;
    private float tempRaduisCircle;
    public bool BlockMansion;
    Material matUsed;

    public PlayerTop player;
    public IEnumerator colorEnum;
    public IEnumerator colorEnumOut;


    public Vector2Int[] restrictor;

    public Material inside;
    public Material outside;

    public Renderer[] renderersToSwitch;
    bool isOutside = false;

    public PaintingHolder thisPaintingHolder;
    public TileData lvl3Trans;
    public TileData lvl2Trans; //for when bonus level happens
    public int levelsToDirectSubway;

    public DialogueInst failDialogue;

    public AudioClip[] music;
    public AudioSource musicPlayer;

    public NPC[] winningNPCS;
    public GameObject recordPlayer;
    public AudioClip Bang;

    


    public void SwitchMat()
    {
        isOutside = !isOutside;
        for (int i = 0; i < renderersToSwitch.Length; i++)
        {
            if (isOutside)
            {
                renderersToSwitch[i].material = outside;
                matUsed = outside;
                if (colorEnumOut == null)
                {
                    if (SceneMessenger.instance.CompletedGames == 2 && !SceneMessenger.instance.GoneOutsideSinceMinigameComplete)
                    {
                        Debug.Log("Hit");
                        colorEnumOut = ChangeRaduis(10f, 3f, false);
                        StartCoroutine(colorEnumOut);
                        SceneMessenger.instance.GoneOutsideSinceMinigameComplete = true;
                    }
                    if (SceneMessenger.instance.CompletedGames == 5 && !SceneMessenger.instance.GoneOutsideSinceMinigameComplete)
                    {
                        colorEnumOut = ChangeRaduis(20f, 4f, false);
                        StartCoroutine(colorEnumOut);
                        SceneMessenger.instance.GoneOutsideSinceMinigameComplete = true;
                    }

                    if (SceneMessenger.instance.CompletedGames == 6 && !SceneMessenger.instance.GoneOutsideSinceMinigameComplete)
                    {
                        colorEnumOut = ChangeRaduis(50, 3f, false);
                        StartCoroutine(colorEnumOut);
                        SceneMessenger.instance.GoneOutsideSinceMinigameComplete = true;
                    }
                    if (SceneMessenger.instance.CompletedGames == 7 && !SceneMessenger.instance.GoneOutsideSinceMinigameComplete)
                    {
                        colorEnumOut = ChangeRaduis(500, 30f, false);
                        StartCoroutine(colorEnumOut);
                        SceneMessenger.instance.GoneOutsideSinceMinigameComplete = true;
                    }

                }

            }
            else
            {
                renderersToSwitch[i].material = inside;
                matUsed = inside;
                if(colorEnumOut != null)
                {
                    StopCoroutine(colorEnumOut);
                    colorEnumOut = null;

                }

                /*if (SceneMessenger.instance.theLastHouse.completed == true)
                {
                    matUsed.SetFloat("_UnEffectedRaduis", 15f);
                }
                else
                {
                    matUsed.SetFloat("_UnEffectedRaduis", 0);
                }*/
            }
        }
    }


    private void Awake()
    {
        for (int i = 0; i < tilesDictionary.Length; i++)
        {
            tilesDictionary[i].completed = false;
        }
        instance = this;
        //start blocking off the mansion so must talk to grandpa
        BlockMansion = true;
        Debug.Log("baking map");

        map.CompressBounds();

        Debug.Log(map.cellBounds);

        walkMap = new bool[map.cellBounds.size.x, map.cellBounds.size.y];

        gridPosition = map.cellBounds.position;

        //process map
        string mape = "";
        for (int i = 0; i < map.cellBounds.size.x; i++)
        {
            for (int l = 0; l < map.cellBounds.size.y; l++)
            {
                walkMap[i, l] = CheckWalkable(i, l);

                if(walkMap[i, l])
                {
                    mape += "0";
                }
                else
                {
                    mape += "1";
                }

            }
            mape += "\n";
        }
        Debug.Log(mape);
        Debug.Log("");
        if (SceneMessenger.instance != null && SceneMessenger.instance.CompletedGames == 8)
        {
            for (int l = 0; l < winningNPCS.Length; l++)
            {
                winningNPCS[l].gameObject.SetActive(true);
                recordPlayer.SetActive(true);
                //winningNPCS[l].Start();
            }
        }
    }

    public bool CheckWalkable(int x, int y)
    {
        Vector3Int tilePosition = gridPosition;
        TileBase tileToCheckBack = map.GetTile(new Vector3Int(x, y, 0) + tilePosition);
        TileBase tileToCheckFore = mapForeground.GetTile(new Vector3Int(x, y, 0) + tilePosition);
        bool returner = false;
        bool isABlock = false ;
        for (int i = 0; i < tilesDictionary.Length; i++)
        {
            for (int l = 0; l < tilesDictionary[i].tileDomain.Length; l++)
            {
                if (tilesDictionary[i].tileDomain[l] == tileToCheckBack || tilesDictionary[i].tileDomain[l] == tileToCheckFore)
                {

                    /*if (tileToCheckBack.name == "GrassMiddle_M")
                    {

                    }*/
                    returner = true;
                    /*if (tilesDictionary[i].canBlock)
                    {
                        
                    }*/
                }
            }
        }
        if (tileToCheckBack == null)
        {
            returner = true;
        }
        for (int i = 0; i < restrictor.Length; i++)
        {
            if (restrictor[i].y == 0)
            {
                if(y + tilePosition.y == restrictor[i].x)
                {
                    returner = true;
                }
            }
            else
            {
                if (x + tilePosition.x == restrictor[i].x)
                {
                    returner = true;
                }
            }
        }
        
        return returner;
    }
    #endregion
    private void Start()
    {
        matUsed = inside;
        if (SceneMessenger.instance.CompletedGames <= 1)
        {

            
            
            UnWalkablePos.Add(map.WorldToCell(new Vector3Int(-28, 12, 0)));
            UnWalkablePos.Add(map.WorldToCell(new Vector3Int(-27, 12, 0)));

        }

        if (SceneMessenger.instance.CompletedGames <= 0)
        {
            for (int i = 0; i < StartingDialogues.Length; i++)
            {

                dialogueMan.StartDialogue(StartingDialogues[i]);
                musicPlayer.PlayOneShot(Bang);
            }

        }
        if(SceneMessenger.instance.CompletedGames >= levelsToDirectSubway)
        {
            tilesDictionary[tilesDictionary.Length - 1] = lvl3Trans;
        }
        if(SceneMessenger.instance.CompletedGames == 2 || SceneMessenger.instance.CompletedGames == 5 || SceneMessenger.instance.CompletedGames > 5 )
        {
            if (!SceneMessenger.instance.failed)
            {
                SceneMessenger.instance.currentSpread = 0;
                colorEnum = ChangeRaduis(15f, 10f, true);
                SceneMessenger.instance.theLastHouse.completed = true;
                StartCoroutine(colorEnum);

            }
        }
        if (SceneMessenger.instance.failed)
        {
            dialogueMan.StartDialogue(failDialogue);
        }


        //matUsed.SetFloat("_UnEffectedRaduis", SceneMessenger.instance.currentSpread);
    }
    private void Update()
    {

        if (SceneMessenger.instance.CompletedGames == 8)
        {
            bool sendToCredits = true;
            for (int l = 0; l < winningNPCS.Length; l++)
            {
                if (winningNPCS[l].TextBoxNotify.activeSelf)
                {
                    sendToCredits = false;
                }
                //winningNPCS[l].gameObject.SetActive(true);
                //winningNPCS[l].Start();
            }
            if (sendToCredits)
            {
                SceneMessenger.instance.LoadNewScene("Credits_Scene", true);
            }
        }
    }

    public IEnumerator ChangeRaduis(float raduisToChange/*, Material mat*/, float timeToWait, bool flipIE)
    {
        //matUsed = mat;
        if(raduisToChange != 0 && raduisToChange != 0.05f && matUsed == inside)
        {
            musicPlayer.PlayOneShot(music[SceneMessenger.instance.currentMusic]);
            SceneMessenger.instance.currentMusic++;

        }

        float time = 0f;
        float t_spread = SceneMessenger.instance.currentSpread;
        if(matUsed == outside)
        {
            t_spread = SceneMessenger.instance.currentSpreadOut;
        }
        while (time < timeToWait)
        {
            time += Time.unscaledDeltaTime;

            matUsed.SetFloat("_UnEffectedRaduis", t_spread + ((time / timeToWait) *  (raduisToChange - t_spread)));
            if(matUsed == outside)
            {
                SceneMessenger.instance.currentSpreadOut = t_spread + ((time / timeToWait) * (raduisToChange - t_spread));

            }
            else
            {
                SceneMessenger.instance.currentSpread = t_spread + ((time / timeToWait) * (raduisToChange - t_spread));

            }

            yield return null;
        }
        matUsed.SetFloat("_UnEffectedRaduis", raduisToChange);
        if (flipIE)
        {

            colorEnum = null;
        }
        else
        {
            colorEnumOut = null;
        }

    }
    public IEnumerator ReverseRaduis(Material mat)
    {
        matUsed = mat;

        float time = SceneMessenger.instance.currentSpread;
        while (time >= 0f + Time.unscaledDeltaTime)
        {
            time -= Time.unscaledDeltaTime;

            matUsed.SetFloat("_UnEffectedRaduis", time);

            yield return null;
        }
        matUsed.SetFloat("_UnEffectedRaduis", 0);
    }

    public TileData[] tilesDictionary;
    //[SerializeField]
    public Tilemap map;
    public Tilemap mapForeground;

    public List<DialogueInst> dialogues;
    public List<Vector3Int> UnWalkablePos;

    public DialogueManager dialogueMan;
    [System.NonSerialized]
    public Vector3Int gridPosition;
}

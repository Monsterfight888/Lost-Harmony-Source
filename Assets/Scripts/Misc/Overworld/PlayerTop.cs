using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTop : Character
{
    public bool isDialogue;
    public Sprite upLook;
    public Sprite downLook;
    public SpriteRenderer playerLook;
    public Sprite sideLook;
    public bool debug;
    public GameObject EPress;
    public Material GreyScaleMat; //so can light up areas
    private GameObject diskGFX;
    //bool DiskActive = false;
    private Animator anim;
    bool step = false;
    public AudioClip composerEvil;
    public AudioClip drumSolo;
    public AudioClip doorFX;
    public AudioClip Foot1;
    public AudioClip Foot2;
    AudioSource mySource;


    /*public void EnableDiskGFX(bool enabled)
    {
        diskGFX.SetActive(enabled);
        DiskActive = true;
    }*/
    public override void Start()
    {

        base.Start();
        mySource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        diskGFX = transform.Find("DiskGFX").gameObject;
        SetPosition(SceneMessenger.instance.playerTrans);
        if (debug)
        {
            SceneMessenger.instance.hasDoneIntro = true;
            SceneMessenger.instance.CompletedGames = 500;
        }
        if(SceneMessenger.instance.CompletedGames == 0)
        {

            GreyScaleMat.SetVector("_Offset", new Vector4(-26, 10, 1f, 1f));
            GreyScaleMat.SetFloat("_UnEffectedRaduis", 0);
        }
        //must have completed a minigame
        if (!SceneMessenger.instance.GoneOutsideSinceMinigameComplete)
        {
            //EnableDiskGFX(true);
        }
    }
    public override void Update()
    {
        //debug, remove
        /*if (Input.GetKeyDown(KeyCode.Q))
        {
            //SceneMessenger.instance.CompletedGames++;

            SceneMessenger.instance.LoadNewScene("Overworld", true);
            
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            snapPos = new Vector3Int(29, -21);
        }
        */
        if (!inTransition && !isDialogue)
        {

            PositionSnap();
            //should be an int anyways, but getaxisraw returns float for some reason so just using as a cast
            int inpX = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            int inpY = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

            if (inpX != 0)
            {

                
                step = !step;
                CheckInteractables(Vector3Int.zero);
                if ( Move(Vector3.right * inpX))
                {
                    if (step)
                    {
                        mySource.PlayOneShot(Foot1);
                        anim.Play("Base Layer.RunAlt");
                        
                    }
                    else
                    {
                        mySource.PlayOneShot(Foot2);
                        anim.Play("Base Layer.run");

                    }
                }
                if (Mathf.Abs(inpX) != inpX)
                {
                    dir = -Vector3Int.right;
                    playerLook.sprite = sideLook;
                    playerLook.flipX = true;
                }
                else
                {
                    dir = Vector3Int.right;
                    playerLook.sprite = sideLook;
                    playerLook.flipX = false;

                }
            }
            else if (inpY != 0)
            {
                step = !step;
                CheckInteractables(Vector3Int.zero);
                if (Move(Vector3.up * inpY))
                {

                    if (Mathf.Abs(inpY) != inpY)
                    {
                        dir = -Vector3Int.up;
                        playerLook.sprite = downLook;
                        playerLook.flipX = false;
                        if (step)
                        {
                            mySource.PlayOneShot(Foot1);
                            anim.Play("Base Layer.DownRunAlt");
                        }
                        else
                        {
                            mySource.PlayOneShot(Foot2);
                            anim.Play("Base Layer.DownRun");

                        }
                    }
                    else
                    {
                        if (step)
                        {
                            mySource.PlayOneShot(Foot1);
                            anim.Play("Base Layer.UpRunAlt");
                        }
                        else
                        {
                            mySource.PlayOneShot(Foot2);
                            anim.Play("Base Layer.UpRun");

                        }
                        dir = Vector3Int.up;
                        playerLook.sprite = upLook;
                        playerLook.flipX = false;
                    }
                }
            }
            else
            {

            }

            if ( !isDialogue && !inTransition)
            {
                if(!CheckInteractables(transform.position + dir))
                {
                    if(!CheckInteractables(transform.position - dir))
                    {
                        if(dir.x == 0)
                        {
                            if(!CheckInteractables(transform.position + Vector3Int.right)){
                                CheckInteractables(transform.position - Vector3Int.right);
                            }
                        }
                        else
                        {
                            if (!CheckInteractables(transform.position + Vector3Int.up))
                            {
                                CheckInteractables(transform.position - Vector3Int.up);
                            }
                        }
                    }
                }
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.E) && isDialogue)
        {
            worldData.dialogueMan.DisplayNextSentence();
        }
        base.Update();
       }

    public bool CheckInteractables(Vector3 pos)
    {
        bool wasInteractable = false ;
        Vector3Int lookingAtPosition = worldData.map.WorldToCell(pos); //for some reason, the offset gets off by one in each axis - but its fixed now
        TileBase lookingAtTile = worldData.map.GetTile(lookingAtPosition);
        TileBase lookingAtTileOther = worldData.mapForeground.GetTile(lookingAtPosition);
        //Debug.Log(lookingAtTile.name);
        for (int i = 0; i < worldData.dialogues.Count; i++)
        {

            if (worldData.dialogues[i].coord.Equals(lookingAtPosition))
            {
                wasInteractable = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    worldData.dialogueMan.StartDialogue(worldData.dialogues[i]);
                }
            }
        }

        for (int i = 0; i < worldData.tilesDictionary.Length; i++)
        {
            for (int l = 0; l < worldData.tilesDictionary[i].tileDomain.Length; l++)
            {
                if (worldData.tilesDictionary[i].tileDomain[l] == lookingAtTile || worldData.tilesDictionary[i].tileDomain[l] == lookingAtTileOther)
                {
                    if (worldData.tilesDictionary[i].TeleportLocation != Vector2Int.zero)//valid locaiton
                    {
                        wasInteractable = true;
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            worldData.musicPlayer.PlayOneShot(doorFX);
                            if (worldData.tilesDictionary[i].FonographLocation == new Vector2Int(500, 500))
                            {
                                SetPosition((Vector2)(worldData.tilesDictionary[i].TeleportLocation)/* + (Vector2.one * .5f)*/);
                                if(worldData.tilesDictionary[i].name == "SubwayTransLvl3")
                                {
                                    worldData.musicPlayer.Stop();
                                    worldData.musicPlayer.PlayOneShot(composerEvil);
                                }
                                else if(worldData.tilesDictionary[i].name == "IntoSubway")
                                {
                                    worldData.musicPlayer.Stop();
                                    worldData.musicPlayer.PlayOneShot(drumSolo);

                                }

                            }
                            else
                            {
                                worldData.SwitchMat();
                                if (worldData.colorEnum != null)
                                {
                                    worldData.StopCoroutine(worldData.colorEnum);
                                    worldData.colorEnum = null;
                                }
                                if (worldData.tilesDictionary[i].FonographLocation != Vector2Int.zero)
                                {

                                    GreyScaleMat.SetVector("_Offset", new Vector4(worldData.tilesDictionary[i].FonographLocation.x, worldData.tilesDictionary[i].FonographLocation.y, 1f, 1f));
                                    if (worldData.tilesDictionary[i].completed == true)
                                    {
                                        GreyScaleMat.SetFloat("_UnEffectedRaduis", 15f);
                                    }
                                    else
                                    {
                                        GreyScaleMat.SetFloat("_UnEffectedRaduis", 0);

                                    }
                                    SceneMessenger.instance.theLastHouse = worldData.tilesDictionary[i];

                                }
                                else
                                {

                                }

                                SetPosition((Vector2)(worldData.tilesDictionary[i].TeleportLocation)/* + (Vector2.one * .5f)*/);
                            }

                        }
                    }
                    /*if (worldData.tilesDictionary[i].sceneName != "" && worldData.tilesDictionary[i].sceneName != null)
                    {
                        wasInteractable = true;
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            SceneMessenger.instance.LoadNewScene(worldData.tilesDictionary[i].sceneName);
                        }
                    }*/
                }
            }

        }

        string SceneName = worldData.thisPaintingHolder.PaintingAvailableAtLocation(lookingAtPosition);
        if(SceneName != "")
        {
            wasInteractable = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneMessenger.instance.LoadNewScene(SceneName, false);
            }
        }


        /*if (lookingAtPosition == worldData.map.WorldToCell(new Vector3(-26, 10, 0)) && DiskActive)
        {
            wasInteractable = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (worldData.colorEnum == null)
                {
                    worldData.colorEnum = worldData.ChangeRaduis(SceneMessenger.instance.CompletedGames, GreyScaleMat, 2f);
                    worldData.StartCoroutine(worldData.colorEnum);

                }
                else
                {
                    worldData.StopCoroutine(worldData.colorEnum);
                    worldData.colorEnum = worldData.ChangeRaduis(SceneMessenger.instance.CompletedGames, GreyScaleMat, 2f);
                    worldData.StartCoroutine(worldData.colorEnum);
                }
            }
        }*/
        if (wasInteractable)
        {

            EPress.transform.position = pos;
            EPress.SetActive(true);
        }
        else
        {
            EPress.SetActive(false);    
        }

        return wasInteractable;
    }
    public IEnumerator Transition()
    {
        yield return null;
    }
}

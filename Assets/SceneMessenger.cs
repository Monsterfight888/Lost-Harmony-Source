using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMessenger : MonoBehaviour
{
    public static SceneMessenger instance;
    public bool hasDoneIntro = false;
    public Vector3 playerTrans;

    public int CompletedGames = 0;

    public List<NPCSpeakAfterQuest> npcDomain = new List<NPCSpeakAfterQuest>();
    public float currentSpread = 0;
    public float currentSpreadOut = 0;

    [System.NonSerialized]
    public bool GoneOutsideSinceMinigameComplete = true;

    public TileData theLastHouse;

    public bool failed = false;
    public int tries;

    public int currentMusic = 0;
    void Awake()
    {

        if (instance != null)
        {
            Destroy(gameObject);//dumb but im going with it bc 
            return;
        }
            //playerTrans = WorldData.instance.player.snapPos;
            DontDestroyOnLoad(gameObject);
        instance = this;

        Application.targetFrameRate = 60;
    }

    public void LoadNewScene(string t_sceneName, bool shouldComplete)
    {
        bool alreadyLoadedScene = false;
        if (t_sceneName == "Overworld" && shouldComplete)
        {
            CompletedGames++;
            failed = false;
            tries = 0;

        }
        else if(t_sceneName == "Overworld" && !shouldComplete)
        {
            failed = true;
            tries++;
            if (CompletedGames == 6)
            {
                StartCoroutine("WaitThenGo"); alreadyLoadedScene = true;
            }
        }
        if (WorldData.instance != null)
            playerTrans = WorldData.instance.player.transform.position;
        npcDomain = new List<NPCSpeakAfterQuest>();
        if(!alreadyLoadedScene)
            SceneManager.LoadScene(t_sceneName);
        GoneOutsideSinceMinigameComplete = false;
    }

    IEnumerator WaitThenGo()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Overworld");
    }
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject currentPlatform1;
    public GameObject currentPlatform2;

    private Vector3 initialPlatformPos1;
    private Vector3 initialPlatformPos2;

    public Camera mainCam;
    public GameObject largeBranch;
    public GameObject medBranch;
    public GameObject smallBranch;

    //[System.NonSerialized]
    public int levelCounter;

    private bool lastWasRight = true;
    private int streak = 0;

    public Level levelManager;
    public PlayerControllerTree player;

    public GameObject ps1;
    public GameObject ps2;
    public GameObject speaker;
    public GameObject winPlat;

    public GameObject[] hints;
    public GameObject F;
    private int numHints = 2;
    private bool newLevel = true;

    private Vector3 oldPos;
    public float camChangeY;

    public List<GameObject> platforms;
    [System.NonSerialized]
    public bool levelResetInTransition = false;

    public GameObject escMenu;
    public GameObject part1;
    public GameObject part2;
    public Speaker ps1Menu;
    public Speaker ps2Menu;
    public Speaker ps3Menu;
    [System.NonSerialized]
    public bool escMenuOpen;
    private bool escMenuPlaying = true;



    void Start()
    {
        levelCounter = 0;

        initialPlatformPos1 = new Vector3(currentPlatform1.transform.position.x, currentPlatform1.transform.position.y, currentPlatform1.transform.position.z);
        initialPlatformPos2 = new Vector3(currentPlatform2.transform.position.x, currentPlatform2.transform.position.y, currentPlatform2.transform.position.z);

        currentPlatform1.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = levelManager.levelStats[levelCounter].rhythm1;
        ps1.GetComponent<Speaker>().rhythm = levelManager.levelStats[levelCounter].AudioRhy1;

        currentPlatform2.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = levelManager.levelStats[levelCounter].rhythm2;
        ps2.GetComponent<Speaker>().rhythm = levelManager.levelStats[levelCounter].AudioRhy2;

        GenerateNewPlatforms(levelCounter);
        levelCounter++;

        platforms = new List<GameObject>();
        platforms.Add(currentPlatform1);
        platforms.Add(currentPlatform2);

        escMenu.SetActive(true);
        part1.SetActive(true);
        part2.SetActive(false);
        escMenuOpen = true;

        StartCoroutine("playAllRhythmsEscMenu");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && numHints > -1 && newLevel && player.GetNewLevelStarted() && !escMenuOpen)
        {
            newLevel = false;

            ps1.SetActive(true);
            ps2.SetActive(true);

            hints[numHints].SetActive(false);

            numHints--;

            if (numHints == 0)
            {
                F.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            escMenuOpen = true;

            escMenu.SetActive(true);
            part1.SetActive(true);
            part2.SetActive(false);
        }

        if (escMenuOpen)
        {
            if (part1.activeSelf && !escMenuPlaying)
            {

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    part1.SetActive(false);
                    part2.SetActive(true);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ps1Menu.buttonPress();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ps2Menu.buttonPress();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ps3Menu.buttonPress();
                }
            }
            else if (part2.activeSelf && Input.GetKeyDown(KeyCode.Space))
            {
                escMenuOpen = false;
                escMenu.SetActive(false);
            }
        }



        if (!escMenuOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                speaker.GetComponent<Speaker>().buttonPress();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && ps1.activeSelf)
            {
                ps1.GetComponent<Speaker>().buttonPress();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && ps2.activeSelf)
            {
                ps2.GetComponent<Speaker>().buttonPress();
            }
        }




    }

    public IEnumerator playAllRhythmsEscMenu()
    {
        escMenuPlaying = true;

        yield return new WaitForSecondsRealtime(1f);

        ps1Menu.buttonPress();

        yield return new WaitForSecondsRealtime(2.4f);

        ps2Menu.buttonPress();

        yield return new WaitForSecondsRealtime(2.4f);

        ps3Menu.buttonPress();

        yield return new WaitForSecondsRealtime(2.4f);


        escMenuPlaying = false;
    }


    public void GenerateNextLevel()
    {
        StartCoroutine("ChangeCamPos");
        initialPlatformPos1 = new Vector3(initialPlatformPos1.x, initialPlatformPos1.y + 6, initialPlatformPos1.z);
        initialPlatformPos2 = new Vector3(initialPlatformPos2.x, initialPlatformPos2.y + 6, initialPlatformPos2.z);

        currentPlatform1.gameObject.layer = LayerMask.NameToLayer("Ground");
        currentPlatform2.gameObject.layer = LayerMask.NameToLayer("Ground");

        currentPlatform1 = Instantiate(medBranch, initialPlatformPos1, Quaternion.identity).gameObject;
        currentPlatform2 = Instantiate(medBranch, initialPlatformPos2, Quaternion.identity).gameObject;

        platforms.Add(currentPlatform1);
        platforms.Add(currentPlatform2);


        currentPlatform2.GetComponent<SpriteRenderer>().flipX = true;


        currentPlatform1.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = levelManager.levelStats[levelCounter].rhythm1;
        ps1.GetComponent<Speaker>().rhythm = levelManager.levelStats[levelCounter].AudioRhy1;

        currentPlatform2.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = levelManager.levelStats[levelCounter].rhythm2;
        ps2.GetComponent<Speaker>().rhythm = levelManager.levelStats[levelCounter].AudioRhy2;


        GenerateNewPlatforms(levelCounter);
        levelCounter++;
        //if (levelCounter == 5)
        //  SceneMessenger.instance.LoadNewScene("Overworld");

        
        /*else
        {
            StartCoroutine("ChangeCamPos");
            levelCounter++;
        }*/
    }

    public IEnumerator ChangeCamPos()
    {
        //start - any normal function

        oldPos = mainCam.transform.position;

        //loop
        float timer = 0;
        float timeRemaining = 1.5f;
        while (timer < timeRemaining)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / timeRemaining;

            mainCam.transform.position = Vector3.Lerp(oldPos, new Vector3(oldPos.x, camChangeY + oldPos.y, oldPos.z), t);

            yield return null;
        }

        player.SetNewLevelStarted(true);
    }

    public void DisappearPlatforms()//3:30
    {
        if (currentPlatform2.gameObject.layer == LayerMask.NameToLayer("Wrong"))
        {
            currentPlatform2.gameObject.SetActive(false);
        }
        else
        {
            currentPlatform1.gameObject.SetActive(false);
        }
    }

    public void ReappearPlatforms()
    {
        if (currentPlatform2.gameObject.layer == LayerMask.NameToLayer("Wrong"))
        {
            currentPlatform2.gameObject.SetActive(true);
        }
        else
        {
            currentPlatform1.gameObject.SetActive(true);
        }
    }


    public void GenerateNewPlatforms(int lvl)
    {
        newLevel = true;
        ps1.SetActive(false);
        ps2.SetActive(false);
        int randomNumber;
        bool left = false;

        if (lvl < levelManager.levelStats.Length - 1)
        {
            randomNumber = Random.Range(0, 2);
        }
        else
        {
            randomNumber = 0;
            left = true;
        }
        
        if(left || (randomNumber == 0 && (lastWasRight || (streak <= 2 && !lastWasRight))))
        {
            currentPlatform1.gameObject.layer = LayerMask.NameToLayer("Correct");
            currentPlatform2.gameObject.layer = LayerMask.NameToLayer("Wrong");

            speaker.GetComponent<Speaker>().rhythm = levelManager.levelStats[lvl].AudioRhy1;

            if (!lastWasRight)
            {
                streak++;
                Debug.Log("Streak left: " + streak);
            }
            else
            {
                streak = 0;
                lastWasRight = false;
                Debug.Log("Broke streak!");
            }
        }
        else  //(!lastWasRight || (streak <= 2 && lastWasRight))
        {
            currentPlatform2.gameObject.layer = LayerMask.NameToLayer("Correct");
            currentPlatform1.gameObject.layer = LayerMask.NameToLayer("Wrong");

            speaker.GetComponent<Speaker>().rhythm = levelManager.levelStats[lvl].AudioRhy2;

            if (lastWasRight)
            {
                streak++;
                Debug.Log("Streak right: " + streak);
            }
            else
            {
                streak = 0;
                lastWasRight = true;
                Debug.Log("Broke streak!");
            }
        }
    }

    public void DisablePlatforms()
    {
        if (!levelResetInTransition)
        {
            currentPlatform1.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            currentPlatform2.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void EnablePlatforms()
    {
        if (!levelResetInTransition)
        {
            currentPlatform1.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            currentPlatform2.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }    
    }

    public void ResetInitialPlatformsAndList()
    {
        levelCounter = 0;

        currentPlatform1 = platforms[0];
        currentPlatform2 = platforms[1];

        GenerateNewPlatforms(0);
        

        for(int i = 2; i < platforms.Count; i++)
        {
            Destroy(platforms[i].gameObject);
        }

        platforms = new List<GameObject>();
        platforms.Add(currentPlatform1);
        platforms.Add(currentPlatform2);
        

        initialPlatformPos1 = new Vector3(currentPlatform1.transform.position.x, currentPlatform1.transform.position.y, currentPlatform1.transform.position.z);
        initialPlatformPos2 = new Vector3(currentPlatform2.transform.position.x, currentPlatform2.transform.position.y, currentPlatform2.transform.position.z);
        ps1.GetComponent<Speaker>().rhythm = levelManager.levelStats[levelCounter].AudioRhy1;
        ps2.GetComponent<Speaker>().rhythm = levelManager.levelStats[levelCounter].AudioRhy2;

        levelCounter++;

        winPlat.layer = LayerMask.NameToLayer("Correct");


        for(int i = 0; i < hints.Length; i++)
        {
            hints[i].SetActive(true);
        }
        F.SetActive(true);

        numHints = 2;


        levelResetInTransition = false;

    }

    public void RemoveSpeakers()
    {
        ps1.SetActive(false);
        ps2.SetActive(false);
    }
}

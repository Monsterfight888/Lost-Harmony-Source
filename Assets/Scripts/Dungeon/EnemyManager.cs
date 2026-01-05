using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum IntroArrow
{
    empty, //nothing
    eight, //half a beat - or more
    quarter, //full beat
    half, // 2 beats
    full // 4 beats
}
public struct SuccessOpportunity
{
    public int releaseDate;
    public string name;

    public SuccessOpportunity(int t_releaseDate, string t_name)
    {
        releaseDate = t_releaseDate;
        name = t_name;
    }
}
public class EnemyManager : MonoBehaviour
{
    public int BPM;

    public static EnemyManager enemyMan;


    public PlayerSide player;

    //internal pieces
    private IEnumerator TickCoroutine;
    private IEnumerator SubTickCoroutine;

    private bool tickChange;

    public List<Enemy> enemyList;

    private int subdivisions = 0;

    public GameObject[] PrefList;

    public Animator camAnim;
    public int beatsOfPause = 0;//including the current measure

    public UIManager UIMan;

    public AudioSource metronomeSource;

    [System.NonSerialized]
    public int currentAttackerIndex = -1;
    [System.NonSerialized]
    public int nextAttackerIndex = -1;

    public AudioSource song;

    bool first = true;

    public SpriteRenderer backround;
    public Sprite minoBack;

    #region IntroCode
    public bool isIntro = true; //intro minigame
    public int introQueing; //how long notice does the player get before the first arrow spawns?

    public int IntroCounter = 0;
    public IntroPattern[] levelIntroPattern;

    public GameObject[] arrowsPrefs;

    List<Transform> arrowsInProgress = new List<Transform>();
    List<Transform> arrowsLeavingScreen = new List<Transform>();

    List<SuccessOpportunity> SuccessOpportunities = new List<SuccessOpportunity>();

    public bool arrowInBox = true;
    bool hasMissed = true;
    int removeCostInProgress = 0;
    int removeCostOutProgress = 0;
    bool FirstToLeaveTheScreen = true;
    int beatsOfWarning = 3;
    public float introLeeway = .1f;
    public bool successOnOpportunity = false;

    public GameObject introScreen;

    public GameObject mainScreen;

    private int counter = 0;

    public GameObject disk;

    bool shouldMetro = false;


    #endregion

    #region misc

    int countInCounter = 0;
    public Text countText;


    #endregion


    public void TakeDamageP(int damage)
    {
        /*if(beatsOfPause <= 0)
        {

            beatsOfPause = 1;

            player.hp -= damage;
            UIMan.UpdateHealth(player.hp);
        }*/
        SceneMessenger.instance.LoadNewScene("Overworld", false);
    }


    private void Awake()
    {
        enemyMan = this;

    }
    public void Start()
    {
        //if (SceneMessenger.instance.CompletedGames == 5)
        //{
            backround.sprite = minoBack;
            //isIntro = true;

        //}
        TickCoroutine = Tick(BPM);
        StartCoroutine(TickCoroutine);
        CreateNewSpawnWave();

        player.maxSpeed = 2 * (BPM / 60f);

        UIMan.UpdateHealth(player.hp);

        //enemyList = new Enemy[4];//up to four enemies in one room

        if (isIntro == false)
        {
            StopIntro();
        }

    }


    public void CreateNewSpawnWave()
    {
        //scenemanager overide - 0 = minotaur, 1 = angel

        enemyList = new List<Enemy>();
        /*for (int i = 0; i < PrefList.Length; i++)
        {
            enemyList.Add(Instantiate(PrefList[i]).GetComponent<Enemy>());
        }*/

        //if(SceneMessenger.instance.CompletedGames == 5)
        //{
            enemyList.Add(Instantiate(PrefList[0]).GetComponent<Enemy>());
        //}
        /*if (SceneMessenger.instance.CompletedGames == 6)
        {
            enemyList.Add(Instantiate(PrefList[1]).GetComponent<Enemy>());

        }
        if (SceneMessenger.instance.CompletedGames == 7)
        {
            enemyList.Add(Instantiate(PrefList[0]).GetComponent<Enemy>());
            enemyList.Add(Instantiate(PrefList[1]).GetComponent<Enemy>());
            shouldMetro = true;


        }*/

        for (int i = 0; i < enemyList.Count; i++)
        {

            enemyList[i].Initialize(this);
        }
        currentAttackerIndex = Random.Range(0, enemyList.Count);
        nextAttackerIndex = Random.Range(0, enemyList.Count);
        //currentAttackerIndex = 0;
        //nextAttackerIndex = 0;
        enemyList[nextAttackerIndex].SetNextSliderIcon();

        UIMan.enemyDomainSliderIcon.sprite = enemyList[currentAttackerIndex].headPic;



    }

    public void InitializeTick()
    {
        tickChange = true;
    }


    IEnumerator SubdivisionTick(int t_BPM, int subdivisions, int counter)
    {
        counter++;
        yield return new WaitForSeconds((60f / t_BPM) / (subdivisions * 2));
        Debug.Log("SubTick");




        if (enemyList != null && enemyList[0] != null)
        {
            enemyList[0].AttackMeasure();
        }

        if((subdivisions * 2) - 1 > counter)
        {
            SubTickCoroutine = SubdivisionTick(t_BPM, subdivisions, counter);
            StartCoroutine(SubTickCoroutine);
        }
    }
    IEnumerator Tick(int t_BPM)
    {
        //main beat
        counter++;

        //universal operations
        camAnim.SetTrigger("Beat");
        if (counter ==2 /*&& SceneMessenger.instance.CompletedGames == 5*/)
        {

            song.Play();
            first = false;
        }
        if (shouldMetro)
        {
            metronomeSource.Play();

        }
        if (countInCounter <= 0)
        {
            countText.text = "";
        }
        if (countInCounter > 0)
        {
            countText.text = countInCounter.ToString();
            countInCounter--;
            
        }
        else
        {

            //subdivide function
            if (subdivisions > 0 && enemyList != null && enemyList[0] != null && beatsOfPause <= 0)
            {
                //SubTickCoroutine = SubdivisionTick(t_BPM, subdivisions, 0);
                //StartCoroutine(SubTickCoroutine);
            }

            //"intstance" gameloop (called once every tick)
            if (!isIntro)
            {
                //main gameloop
                player.frozen = false;
                EnemyNotify();
                player.ToggleBox();
            }
            else
            {
                //intro gameloop
                IntroBeat();
            }
        }



        //"continous" game loop (called every frame)

        float timeTaken = 0;
        float timeToWait = (60f / t_BPM);
        while (timeTaken < timeToWait)
        {
            timeTaken += Time.deltaTime;
            //intro
            if (isIntro)
            {
                

                UpdateIntro(timeTaken, t_BPM);
            }
            else //dungeon
            {

            }

            yield return null;
        }
        
        FirstToLeaveTheScreen = true;

        

        //yield return new WaitForSecondsRealtime(60f/ t_BPM);
        if (tickChange)
        {
            tickChange = false;

            player.maxSpeed = 2 * (BPM / 60f);
        }
        
        //restarting tick courotine function - like update
        TickCoroutine = Tick(BPM);
        StartCoroutine(TickCoroutine);
    }
    public void StartCountIn(int countInBeats)
    {
        countInCounter = countInBeats;
        player.frozen = true;
    }

    #region IntroCode
    //every beat logic
    public void IntroBeat()
    {
        if (IntroCounter < levelIntroPattern.Length)
        {
            //levelIntroPattern[IntroCounter] find intro pattern and spawn arrows based on that
            int nonEmptySlots = 0;
            for (int i = 0; i < levelIntroPattern[IntroCounter].Slots.Length; i++)
            {
                if (levelIntroPattern[IntroCounter].Slots[i] != IntroArrow.empty)
                {
                    nonEmptySlots++;
                }
            }

            levelIntroPattern[IntroCounter].ArrowCount = nonEmptySlots;

            for (int i = 0; i < levelIntroPattern[IntroCounter].Slots.Length; i++)
            {
                if (levelIntroPattern[IntroCounter].Slots[i] == IntroArrow.empty)
                {

                }
                else if (levelIntroPattern[IntroCounter].Slots[i] == IntroArrow.quarter)
                {

                    Transform arrowTrans = Instantiate(arrowsPrefs[i]).GetComponent<Transform>();
                    arrowTrans.gameObject.layer = LayerMask.NameToLayer("Quarter"); // dumb but out of ideas for ways to store info

                    Transform tendril = arrowTrans.Find("Tendril");
                    float length = 1.666666f;//calced w/ desmos https://www.desmos.com/calculator/o34z1uhn4a
                    //(10 + arrowSize) / (1f / beatsOfWarning);
                    tendril.localScale = new Vector3(1, (length) * 2, 1);
                    tendril.localPosition = -Vector3.up * length/* + arrowTrans.position*/;

                    arrowsInProgress.Add(arrowTrans);
                }
                else if (levelIntroPattern[IntroCounter].Slots[i] == IntroArrow.half)
                {
                    Transform arrowTrans = Instantiate(arrowsPrefs[i]).GetComponent<Transform>();
                    arrowTrans.gameObject.layer = LayerMask.NameToLayer("Half"); // dumb but out of ideas for ways to store info

                    Transform tendril = arrowTrans.Find("Tendril");
                    float length = 1.666666f * 2;//too lazy fix for reals later
                    //(10 + arrowSize) / (1f / beatsOfWarning);
                    tendril.localScale = new Vector3(1, (length) * 2, 1);
                    tendril.localPosition = -Vector3.up * length/* + arrowTrans.position*/;

                    arrowsInProgress.Add(arrowTrans);
                }
            }
        }
            IntroCounter++;
        if (IntroCounter > beatsOfWarning && IntroCounter - beatsOfWarning < levelIntroPattern.Length)
        {
            bool checker = false;
            for (int i = 0; i < levelIntroPattern[IntroCounter - beatsOfWarning].Slots.Length; i++)
            {
                if(levelIntroPattern[IntroCounter - beatsOfWarning].Slots[i] != IntroArrow.empty)
                {
                    checker = true;
                }
            }
            if (hasMissed)
            {
                SceneMessenger.instance.LoadNewScene("Overworld", false);
                Debug.Log("has missed");
            }
            hasMissed = checker;
        }


        CheckForHalfIntersectingArrows();
    }
    public void CheckForHalfIntersectingArrows()
    {
        if (IntroCounter >= beatsOfWarning)
        {
            //logic for each object going past it's point at the initial lerp (-5, 5) and keeps going on past the screen
            int currentRow = IntroCounter - beatsOfWarning;
            if (currentRow > levelIntroPattern.Length - 1)
                return;
            for (int i = 0; i < levelIntroPattern[currentRow].ArrowCount; i++)
            {

                //instead of running complicated code, just gonna take off the last ArrowCount amount of arrows off the top of the list - much easier
                arrowsLeavingScreen.Add(arrowsInProgress[0]);//no longer in transit
                arrowsInProgress.RemoveAt(0);
                removeCostInProgress++;

            }


        }
    }
    //every tick logic
    public void UpdateIntro(float timeTaken, int t_BPM)
    {
        if (IntroCounter - beatsOfWarning > levelIntroPattern.Length - 1)
        {
            StopIntro();
        }

        arrowInBox = false;
        UpdateArrowPosition(timeTaken, t_BPM);
        UpdateSuccessOportunities();


    }
    public void UpdateArrowPosition(float timeTaken, int t_BPM)
    {
        //function "global" vars
        float timeToWait = (60f / t_BPM);
        float globalLerpVal = ((timeTaken / timeToWait));

        //animate the arrows who appear on the screen
        ArrowGraphicAnimation(timeTaken, timeToWait, arrowsInProgress, -5f, 5f, removeCostInProgress);
        //animate the arrows leaving the screen (mostly for tail's sake)
        ArrowGraphicAnimation(timeTaken, timeToWait, arrowsLeavingScreen, 5f, 15f, removeCostOutProgress);

        //logic on screen arrows
        for (int i = 0; i < arrowsInProgress.Count; i++)
        {

            int thisRow = GetRow(i + removeCostInProgress); //in order to get the ACTIVE row, must add remove cost b/c will search everything
            int placeInBeatsOfWarning = ((IntroCounter - thisRow) % beatsOfWarning);

            float lerpVal = ((timeTaken / timeToWait)/* / beatsOfWarning) + (placeInBeatsOfWarning * (1f / beatsOfWarning)*/);
            //arrowsInProgress[i].position = new Vector3(arrowsInProgress[i].position.x, Mathf.Lerp(-5f, 5f, lerpVal), arrowsInProgress[i].position.z);

            
        }

        //actual logic
        /*Transform[] arrowsCurrentRowInProgress = new Transform[levelIntroPattern[currentRow].ArrowCount];
        for (int i = 0; i < arrowsCurrentRowInProgress.Length; i++)
        {
            arrowsCurrentRowInProgress[i] = arrowsInProgress[i];//take from top
        }
        float leniancyValue = ((timeTaken / leeway) / timeToWait); //for one singular beat at the rate of leeway
        //Input check
        for (int i = 0; i < arrowsCurrentRowInProgress.Length; i++)
        {
            if (leniancyValue < 1)
            {
                if (StartSucessOpportunity(arrowsCurrentRowInProgress[i], 0))
                {
                    successOnOpportunity = true;
                }
            }
        }
        float lerpVal = ((timeTaken / timeToWait));
        //for logics sake - same empty thing as above
        if (lerpVal < leeway)
        {
            arrowInBox = true;
        }*/

        /*if (IntroCounter < levelIntroPattern.Length)
        {
            ArrowPressLogic(timeTaken, timeToWait, IntroCounter - 2, true);
        }*/
        if (globalLerpVal < introLeeway || globalLerpVal > 1f - introLeeway)
        {
            arrowInBox = true;
        }

        //leaving screen
        //introcounter begins at 1

        if (FirstToLeaveTheScreen)
        {
            int currentRow = IntroCounter - beatsOfWarning;
            if (currentRow >= 3)
            {
                for (int i = 0; i < levelIntroPattern[currentRow - 3].ArrowCount; i++)
                {
                    Destroy(arrowsLeavingScreen[0].gameObject);
                    arrowsLeavingScreen.RemoveAt(0);//take from top
                    removeCostOutProgress++;
                }
                FirstToLeaveTheScreen = false;
            }
        }

        if (IntroCounter >= beatsOfWarning - 1)
        {
            int currentRow = IntroCounter - beatsOfWarning + 1;
            if (currentRow <= levelIntroPattern.Length - 1)
            {

                Transform[] arrowsIn = new Transform[levelIntroPattern[currentRow].ArrowCount];
                for (int i = 0; i < arrowsIn.Length; i++)
                {
                    arrowsIn[i] = arrowsInProgress[i];//take from top
                }
                for (int i = 0; i < arrowsIn.Length; i++)
                {
                    if (globalLerpVal > 1f - introLeeway)
                    {
                        //Debug.Log(arrowsIn[i].name + " in");
                        StartSucessOpportunity(arrowsIn[i], 1);
                    }
                }
            }

        }
        if (IntroCounter >= beatsOfWarning)
        {
            int currentRow = IntroCounter - beatsOfWarning;
            if (currentRow > levelIntroPattern.Length - 1)
                return;

            Transform[] arrowsOut = new Transform[levelIntroPattern[currentRow].ArrowCount];
            for (int i = 0; i < arrowsOut.Length; i++)
            {
                arrowsOut[i] = arrowsLeavingScreen[arrowsLeavingScreen.Count - i - 1];//take from top
            }

            for (int i = 0; i < arrowsOut.Length; i++)
            {
                if (globalLerpVal < introLeeway)
                {
                    StartSucessOpportunity(arrowsOut[i], 0);
                    //Debug.Log(arrowsOut[i].name + " out");
                }
            }
        }







        /*
        //first frame of each one takes out the last ones "arrowsl eaving the screen"
        

        //actual logic
        //ArrowPressLogic(timeTaken, timeToWait, currentRow, false);
        if(currentRow >= levelIntroPattern.Length)
        {
            return;
        }

        

        //float leniancyValue = ((timeTaken / leeway) / timeToWait); //for one singular beat at the rate of leeway
        //Input check
        for (int i = 0; i < arrowsOut.Length; i++)
        { 
            if (globalLerpVal > 1f - leeway)
            {
                if (StartSucessOpportunity(arrowsOut[i], 0))
                {
                    successOnOpportunity = true;
                }
            }

        }
        for (int i = 0; i < arrowsIn.Length; i++)
        {
            if (globalLerpVal < leeway)
            {
                StartSucessOpportunity(arrowsInProgress[i], 1);
            }
        }


     }*/
    }

    public void UpdateSuccessOportunities()
    {
        for (int i = 0; i < SuccessOpportunities.Count; i++)
        {
            if (Input.GetKeyUp(GetSucessOpportunityKeyCode(SuccessOpportunities[i].name)))
            {
                //keycode has been let up

                if (IntroCounter == SuccessOpportunities[i].releaseDate && arrowInBox)
                {
                    //is valid
                    Debug.Log("Successful arrow!");
                }
                else
                {
                    Debug.Log("Unseccessful arrow");
                    SceneMessenger.instance.LoadNewScene("Overworld", false);
                    SuccessOpportunities.RemoveAt(i);
                    i++;
                }
            }
        }
    }
    //miscilanous functions
    public void ArrowPressLogic(float timeTaken, float timeToWait, int currentRow, bool inverse)
    {
        Transform[] arrows = new Transform[levelIntroPattern[currentRow].ArrowCount];
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i] = arrowsLeavingScreen[i];//take from top
        }

        float leniancyValue = ((timeTaken / introLeeway) / timeToWait); //for one singular beat at the rate of leeway
        float lerpVal = ((timeTaken) / timeToWait);
        Debug.Log(lerpVal);
        for (int i = 0; i < arrows.Length; i++)
        {
            if (lerpVal < introLeeway && !inverse)
            {
                if (StartSucessOpportunity(arrows[i], 0))
                {
                }
            }
            else if(lerpVal > (1f - introLeeway) && inverse)
            {
                if (StartSucessOpportunity(arrows[i], 1))
                {
                    // arrowInBox = true;
                }
            }
        }
        //for logics sake - same empty thing as above
        if (lerpVal < introLeeway && !inverse)
        {
            arrowInBox = true;
        }
        else if (lerpVal > (1f - introLeeway) && inverse)
        {
            arrowInBox = true;
        }
    }

    public void ArrowGraphicAnimation(float timeTaken, float timeToWait, List<Transform> arrowsToMove, float min, float max, int removeCost)
    {
        for (int i = 0; i < arrowsToMove.Count; i++)
        {
            
            int thisRow = GetRow(i + removeCost); //in order to get the ACTIVE row, must add remove cost b/c will search everything
            int placeInBeatsOfWarning = ((IntroCounter - thisRow) % beatsOfWarning);

            float lerpVal = ((timeTaken / timeToWait) / beatsOfWarning) + (placeInBeatsOfWarning * (1f / beatsOfWarning));
            arrowsToMove[i].position = new Vector3(arrowsToMove[i].position.x, Mathf.Lerp(min, max, lerpVal), arrowsToMove[i].position.z);

            /*if (lerpVal > (1f - leeway))
            {
                StartSucessOpportunity(arrowsToMove[i], 1);
            }*/
        }
    }
    public bool StartSucessOpportunity(Transform arrow, int adder)
    {
        bool returner = false;
        if (Input.GetKeyDown(GetSucessOpportunityKeyCode(arrow.name)))
        {
            if(arrow.gameObject.layer == LayerMask.NameToLayer("Quarter"))
            {
                adder += 1;
            }
            else if (arrow.gameObject.layer == LayerMask.NameToLayer("Half"))
            {
                adder += 2;
            }
            SuccessOpportunities.Add(new SuccessOpportunity(IntroCounter + adder, arrow.name));//adder is for whether or not the arrow is on one or the other side
            Debug.Log("Successful start");
            returner = true;
            hasMissed = false;// arrowInBox = true;
        }

        return returner;
    }
    public int GetRow(int i)//index in arrows
    {
        //i = i + removeCost; //this is a reminder to keep everything tidy, convet BEFORE the data is used, not after - some weird bugs occur otherwise
        int levelCounter = 0;
        int thisRow = 0;
        for (int l = 0; l < levelIntroPattern.Length; l++)
        {
            for (int p = 0; p < levelIntroPattern[l].ArrowCount; p++)
            {

                if (levelCounter + p == i)
                {

                    return l; // remove cost is for the interpreting
                }
            }
            levelCounter += levelIntroPattern[l].ArrowCount;
        }
        return -1;
    }

    public KeyCode GetSucessOpportunityKeyCode(string name)
    {
        //return KeyCode.W;
        string pre = "Arrow_";
        string suf = "(Clone)";
        if (name.Equals(pre + "Right" + suf))
        {
            return KeyCode.D;
        }
        else if (name.Equals(pre + "Left" + suf))
        {
            return KeyCode.A;
        }
        else if (name.Equals(pre + "Up" + suf))
        {
            return KeyCode.W;
        }
        else if (name.Equals(pre + "Down" + suf))
        {
            return KeyCode.S;
        }

        return KeyCode.Q;
    }

    /*public Transform[] GetArrowsFromRow(int row) I wrote out whole function that did not need to exist
    {
        Transform[] returner = new Transform[levelIntroPattern[row].ArrowCount];
        int levelCounter = 0;
        int thisRow = 0;
        for (int l = 0; l < levelIntroPattern.Length; l++)
        {
            if(l == row)
            {
                for (int p = 0; p < levelIntroPattern[l].ArrowCount; p++)
                {
                    returner[p] = arrowsLeavingScreen
                }
            }
            levelCounter += levelIntroPattern[l].ArrowCount;
        }
        return returner;
    }*/

    public void StopIntro()
    {
        isIntro = false;

        introScreen.SetActive(false);
        mainScreen.SetActive(true);


        StartCountIn(3);

    }

    #endregion

    public void EnemyNotify()
    {
        if (beatsOfPause <= 0 && enemyList.Count != 0)
        {
            if(currentAttackerIndex == -1)
            {
                currentAttackerIndex = Random.Range(0, enemyList.Count);
                nextAttackerIndex = Random.Range(0, enemyList.Count);
                //currentAttackerIndex = 0;
                //nextAttackerIndex = 0;
                enemyList[nextAttackerIndex].SetNextSliderIcon();


            }
            if (enemyList.Count != 0 && enemyList[currentAttackerIndex] != null)
            {

                if (enemyList[currentAttackerIndex].AttackMeasure())// time to select new
                {
                    enemyList[currentAttackerIndex].TakeDamage();
                    currentAttackerIndex = nextAttackerIndex;
                    if(enemyList.Count == 1)
                    {
                        nextAttackerIndex = 0;
                        enemyList[nextAttackerIndex].SetNextSliderIcon();
                    }
                    else if (enemyList.Count == 0)
                    {
                        Debug.Log("Win");
                        disk.SetActive(true);
                        //SceneMessenger.instance.LoadNewScene("Overworld", true);
                    }
                    else
                    {

                        nextAttackerIndex = Random.Range(0, enemyList.Count);
                        /*while (enemyList[nextAttackerIndex].currentHealth == 1)
                        {
                            nextAttackerIndex = Random.Range(0, enemyList.Count);

                        }*/
                        enemyList[nextAttackerIndex].SetNextSliderIcon();

                    }

                }


            }
        }
        /*else
        {
            if (enemyList != null && enemyList[currentAttackerIndex] != null)
            {
                enemyList[currentAttackerIndex].AbortMeasure();
            }
            beatsOfPause--;
        }*/
    }

    /*public void RemoveEnemy()
    {
        int index = 0;
        for (int i = 0; i < enemyList.Length; i++)
        {
            if(enemyList[i] == null)
            {
                index = i;
            }
        }

        if(currentAttackerIndex > index)
        {
            currentAttackerIndex--;
        }
        if(nextAttackerIndex > index)
        {
            nextAttackerIndex--;
        }
        Enemy[] newEnemList = new Enemy[enemyList.c - 1];
        int adder = 0;
        for (int i = 0; i < newEnemList.Length; i++)
        {
            if(i == index)
            {
                adder = 1;
            }
            newEnemList[i] = enemyList[i + adder];
        }
        enemyList = newEnemList;

        
    }*/
}

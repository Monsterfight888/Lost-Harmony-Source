using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerScale : MonoBehaviour
{


    public float speed;
    private float moveInput;
    private bool frozen = false;

    public float jumpForce;

    private int extraJumps;
    public int extraJumpsValue;

    [System.NonSerialized]
    public Rigidbody2D rb;
    private Animator anim;

    private bool facingRight = true;

    private bool isGrounded;
    public Transform GroundCheck;
    public float checkRadius;

    public LayerMask whatIsGround;
    public LayerMask correct;
    public LayerMask wrong;
    public LayerMask scale;
    public LayerMask end;

    private bool newLevelStarted = true;
    private bool levelResetInProgress = false;
    private bool platChoiceInProgress = false;
    private bool showcasePlayingATM = false;
    private bool isFirstPlayOfShowcase = true;
    private Platforms prevPlatformSet = null;

    public Camera mainCam;
    public Manager manager;

    private AudioSource audioSchmaudio;

    public Color grey;
    public Color darkGrey;
    public Color reEmphasis;
    private List<GameObject> camTriggers;
    private List<GameObject> columns;
    private GameObject platformPrev = null;

    public GameObject escMenu; 
    public GameObject menuSpeaker;
    private bool escMenuOpen;
    private bool playingScale = false;

    public Scale scaleNotes;

    public GameObject record;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        extraJumps = extraJumpsValue;

        audioSchmaudio = gameObject.GetComponent<AudioSource>();

        camTriggers = new List<GameObject>();
        columns = new List<GameObject>();

        escMenuOpen = true;
        escMenu.SetActive(true);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        StartCoroutine("playScale", true);


    }

    private void Update()
    {
        

        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && extraJumps > 0 && !escMenuOpen)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && extraJumps == 0 && isGrounded == true && !escMenuOpen)
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        if(!escMenuOpen && platChoiceInProgress && !showcasePlayingATM && Input.GetKeyDown(KeyCode.E))
        {
            if(prevPlatformSet != null)
            {
                StartCoroutine("ShowCase", prevPlatformSet);
            }
        }
        /*else if(!escMenuOpen && !showcasePlayingATM && Input.GetKeyDown(KeyCode.E))
        {
            showcasePlayingATM = true;
            manager.StartCoroutine("PlayScale");
        }*/

        if (escMenuOpen)
        {
            if (Input.GetKeyDown(KeyCode.F) && !playingScale)
            {
                StartCoroutine("playScale", false);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !playingScale)
            {
                escMenuOpen = false;
                escMenu.SetActive(false);

                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 0.01f);
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            escMenuOpen = true;
            escMenu.SetActive(true);
        }


    }

    public IEnumerator playScale(bool firstPlayOfMenu)
    {
        playingScale = true;
        if (firstPlayOfMenu)
        {
            yield return new WaitForSecondsRealtime(1f);
        }


        
        menuSpeaker.GetComponent<Image>().color = new Color(255, 255, 255, .7f);

        for(int i = 0; i < scaleNotes.notes.Length; i++)
        {
            GetComponent<AudioSource>().PlayOneShot(scaleNotes.notes[i].note);

            yield return new WaitForSecondsRealtime(.5f);
        }

        for (int i = 6; i > 0; i--)
        {
            GetComponent<AudioSource>().PlayOneShot(scaleNotes.notes[i].note);

            yield return new WaitForSecondsRealtime(.5f);
        }

        GetComponent<AudioSource>().PlayOneShot(scaleNotes.notes[0].note);

        yield return new WaitForSecondsRealtime(.2f);

        menuSpeaker.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
        playingScale = false;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, whatIsGround);

        if (!isGrounded)
        {
            anim.SetBool("IsJumping", true);
        }
        else
        {
            anim.SetBool("IsJumping", false);
        }

        if (!frozen)
        {
            moveInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

            if (rb.velocity.x >= 2 || rb.velocity.x <= -2)
            {
                anim.SetBool("IsRunning", true);
            }
            else
            {
                anim.SetBool("IsRunning", false);
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("IsRunning", false);
        }


        if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct) && newLevelStarted)
        {
            Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct).gameObject.GetComponent<ScaleStep>().playNote(new Color(0, 255, 0));
            newLevelStarted = false;
            SetFrozen(true);

            manager.setInfoInactive();
            platChoiceInProgress = false;
            isFirstPlayOfShowcase = true;

            StartCoroutine("NextLevel");
        }
        else if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, wrong) && !levelResetInProgress)
        {
            Physics2D.OverlapCircle(GroundCheck.position, checkRadius, wrong).gameObject.GetComponent<ScaleStep>().playNote(new Color(255, 0, 0)); 
            //newLevelStarted = false; 
            levelResetInProgress = true;
            SetFrozen(true);

            manager.setInfoInactive();
            platChoiceInProgress = false;
            isFirstPlayOfShowcase = true;

            StartCoroutine("StartOver");
        }
        else if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, scale))
        {
            GameObject step = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, scale).gameObject;
            step.GetComponent<ScaleStep>().playNote(new Color(255, 255, 255));
            columns.Add(step);
        }


        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }

    }

    public void SetFrozen(bool isFrozen)
    {
        frozen = isFrozen;
        //anim.SetBool("IsJumping", false);
        //anim.SetBool("IsRunning", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CamTrigger"))
        {
            if (collision.gameObject.GetComponent<CameraTrigger>().special)
            {
                camTriggers.Add(collision.gameObject);
                newLevelStarted = true;
                CameraTrigger camTrig = collision.gameObject.GetComponent<CameraTrigger>();
                camTrig.StartCoroutine("ChangeCamPos", mainCam);

                platChoiceInProgress = true;
                manager.setInfoActive();

                prevPlatformSet = camTrig.platformSet;

                isFirstPlayOfShowcase = true;
                StartCoroutine("ShowCase", prevPlatformSet);
            }
            else
            {
                camTriggers.Add(collision.gameObject);
                collision.gameObject.GetComponent<CameraTrigger>().StartCoroutine("ChangeCamPos", mainCam);
            }
        }
        else if (collision.gameObject.CompareTag("MainCamera") && !levelResetInProgress)
        {
            isFirstPlayOfShowcase = true;
            platChoiceInProgress = false;
            manager.setInfoInactive();
            ResetLevel();
        }
        else if (collision.gameObject.CompareTag("End"))
        {
            StartCoroutine("End");
        }
        else if (collision.gameObject.CompareTag("PickUp"))
        {
            collision.gameObject.SetActive(false);
        }


    }

    public IEnumerator End()
    {
        yield return new WaitForSecondsRealtime(.5f);
        SceneMessenger.instance.LoadNewScene("Overworld", true);
    }

    public IEnumerator ShowCase(Platforms platformSet)
    {
        showcasePlayingATM = true;


        if (isFirstPlayOfShowcase)
        {
            isFirstPlayOfShowcase = false;

            yield return new WaitForSecondsRealtime(.2f);
            SetFrozen(true);
            yield return new WaitForSecondsRealtime(1f);
        }
        else
        {
            GameObject curStep = columns[columns.Count - 1];
            curStep.GetComponent<ScaleStep>().repeatNote(reEmphasis);
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            yield return new WaitForSecondsRealtime(1f);
            curStep.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }
        
        

        platformSet.platform1.gameObject.GetComponent<SpriteRenderer>().color = grey;
        GetComponent<AudioSource>().PlayOneShot(platformSet.platform1N);

        yield return new WaitForSecondsRealtime(1f);

        platformSet.platform1.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        platformSet.platform2.gameObject.GetComponent<SpriteRenderer>().color = grey;
        GetComponent<AudioSource>().PlayOneShot(platformSet.platform2N);

        yield return new WaitForSecondsRealtime(1f);

        platformSet.platform2.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        platformSet.platform3.gameObject.GetComponent<SpriteRenderer>().color = grey;
        GetComponent<AudioSource>().PlayOneShot(platformSet.platform3N);

        yield return new WaitForSecondsRealtime(1f);

        platformSet.platform3.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);

        SetFrozen(false);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 0.01f);
        showcasePlayingATM = false;
    }



    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    public void SetShowCasePlayingATM(bool isPlaying)
    {
        showcasePlayingATM = isPlaying;
    }


    public IEnumerator NextLevel()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        GameObject platform = null;
        Platforms platGroup = null;
        //Animator anim = null;

        if(platformPrev != null)
        {
            platformPrev.GetComponentInParent<Platforms>().setActiveTrue();
        }

        if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct))
        {
            platform = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct).gameObject;
            platform.gameObject.layer = LayerMask.NameToLayer("Ground");
            platformPrev = platform;
            //anim = platform.GetComponent<Animator>();
            //anim.SetTrigger("ReadyToTransport");
            gameObject.transform.SetParent(platform.transform);
            platGroup = platform.GetComponentInParent<Platforms>();
            platGroup.Transport(platform, true);

            
            //Physics2D.OverlapCircle(GroundCheck.position, checkRadius, whatIsGround).gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);

            // wait for the platform animation to move the player up and over
            yield return new WaitForSeconds(1.7f);

            gameObject.transform.SetParent(null);
            SetFrozen(false);
            //newLevelStarted = true;
            // time before platform resets color and layer so that the platform can be re-triggered if the level is reset
            yield return new WaitForSeconds(5f);

            platform.gameObject.layer = LayerMask.NameToLayer("Correct");
            platform.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            platform.GetComponent<ScaleStep>().triggered = false;
        }



        
    }

    public IEnumerator StartOver()
    {
        

        yield return new WaitForSecondsRealtime(0.5f);

        GameObject platform = null;

        if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, wrong))
        {
            platform = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, wrong).gameObject;

            platform.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(2f);
        ResetLevel();

        

        if (platform != null)
        {
            platform.SetActive(true);
            platform.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            platform.GetComponent<ScaleStep>().triggered = false;
        }

        //newLevelStarted = true;
        levelResetInProgress = false;
        SetFrozen(false);
    }

    public void ResetLevel()
    {
        manager.Abort();

        for (int i = 0; i < camTriggers.Count; i++)
        {
            camTriggers[i].gameObject.SetActive(true);
        }

        for(int i = 0; i < columns.Count; i++)
        {
            columns[i].gameObject.GetComponent<ScaleStep>().triggered = false;
            columns[i].gameObject.GetComponent<SpriteRenderer>().color = darkGrey;
        }

        gameObject.transform.position = new Vector3(-7.6f, 4.05f, gameObject.transform.position.z);

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        mainCam.transform.position = new Vector3(0f, 0f, mainCam.transform.position.z);
        escMenuOpen = true;
        escMenu.SetActive(true);
        record.gameObject.SetActive(true);
        StartCoroutine("playScale", true);

    }

}

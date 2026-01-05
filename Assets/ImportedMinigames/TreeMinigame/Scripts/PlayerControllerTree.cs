using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTree : MonoBehaviour
{
    //I think reall weird that player landing on same branch kills them but whatever


    public float speed;
    private float moveInput;

    public float jumpForce;

    private int extraJumps;
    public int extraJumpsValue;

    private Rigidbody2D rb;
    private Animator anim;

    private bool facingRight = true;

    private bool isGrounded;
    public Transform GroundCheck;
    public float checkRadius;

    public LayerMask whatIsGround;
    public LayerMask correct;
    public LayerMask wrong;
    public LayerMask notInAir;
    private bool newLevelStarted = true;

    public PlatformGenerator platformGenerator;
    public GameObject platformWithDoor;

    public Camera mainCam;
    private Vector3 oldPos;

    private float curLevelPosY;

    [System.NonSerialized]
    public bool soundAlreadyPlaying = false;


    public AudioSource camera;
    public AudioClip footsteps;
    public AudioClip thump;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        extraJumps = extraJumpsValue;

        curLevelPosY = 0;

    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, whatIsGround);

        if (rb.velocity.y != 0 && !Physics2D.OverlapCircle(GroundCheck.position, checkRadius, whatIsGround))
        {
            anim.SetBool("IsJumping", true);
        }
        else
        {
            anim.SetBool("IsJumping", false);
        }

        if (!platformGenerator.escMenuOpen)
        {
            moveInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
        


        if (rb.velocity.x >= 2 || rb.velocity.x <= -2)
        {
            anim.SetBool("IsRunning", true);

            if (!camera.isPlaying)
            {
               // canvas.PlayOneShot(footsteps);
            }
            
        }
        else
        {
            anim.SetBool("IsRunning", false);
            camera.Stop();
        }


        if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct) && newLevelStarted)
        {
            Correct();
        }
        else if(Physics2D.OverlapCircle(GroundCheck.position, checkRadius, wrong) && newLevelStarted)
        {
            Physics2D.OverlapCircle(GroundCheck.position, checkRadius, wrong).gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            Wrong();
        }

        if (rb.velocity.y > 1)
        {
            platformGenerator.DisablePlatforms();
        }
        else
        {
            platformGenerator.EnablePlatforms();
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

    private void Update()
    {
        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && extraJumps > 0 && !platformGenerator.escMenuOpen)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && extraJumps == 0 && isGrounded == true && !platformGenerator.escMenuOpen)
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        if (gameObject.transform.position.y <= curLevelPosY - 6 && !platformGenerator.levelResetInTransition)
        {
            StartCoroutine("Reset");
        }

        
    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    public bool GetNewLevelStarted()
    {
        return newLevelStarted;
    }

    public void SetNewLevelStarted(bool newLevel)
    {
        newLevelStarted = newLevel;
    }

    void Correct()
    {
        if(platformGenerator.levelCounter < platformGenerator.levelManager.levelStats.Length)
        {
            Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct).gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            newLevelStarted = false;
            curLevelPosY = transform.position.y;
            StartCoroutine("NextLevel");
        }
        else if(platformGenerator.levelCounter == platformGenerator.levelManager.levelStats.Length)
        {
            Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct).gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            newLevelStarted = false;
            curLevelPosY = transform.position.y;
            platformGenerator.levelCounter++;
            platformGenerator.RemoveSpeakers();
            StartCoroutine("Win");
        }
        else if (platformGenerator.levelCounter == platformGenerator.levelManager.levelStats.Length + 1)
        {
            platformGenerator.levelCounter++;
            newLevelStarted = false;//
            Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct).gameObject.layer = LayerMask.NameToLayer("Ground");
            platformGenerator.RemoveSpeakers();
            StartCoroutine("MoveCamUp");
        }
    }

    IEnumerator Win()
    {
        oldPos = mainCam.transform.position;

        yield return new WaitForSecondsRealtime(0.25f);

        if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct))
        {
            GameObject gam = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, correct).gameObject;
            gam.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            gam.layer = LayerMask.NameToLayer("Ground");
        }

        //loop
        float timer = 0;
        float timeRemaining = 1.5f;
        while (timer < timeRemaining)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / timeRemaining;

            mainCam.transform.position = Vector3.Lerp(oldPos, new Vector3(oldPos.x, 5 + oldPos.y, oldPos.z), t);

            yield return null;
        }

        newLevelStarted = true;
    }

    IEnumerator MoveCamUp()
    {
        oldPos = mainCam.transform.position;
        platformGenerator.currentPlatform1 = platformWithDoor;

        float timer = 0;
        float timeRemaining = 1f;
        while (timer < timeRemaining)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / timeRemaining;

            mainCam.transform.position = Vector3.Lerp(oldPos, new Vector3(5 + oldPos.x, 4.8f + oldPos.y, oldPos.z), t);

            yield return null;
        }
        newLevelStarted = true;//
    }


    void Wrong()
    {
        newLevelStarted = false;
        StartCoroutine("PreviousLevel");
    }

    public IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(0.25f);

        platformGenerator.GenerateNextLevel();

        if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, whatIsGround))
        {
            Physics2D.OverlapCircle(GroundCheck.position, checkRadius, whatIsGround).gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

        yield return new WaitForSeconds(0.5f);
        
        //newLevelStarted = true;
    }

    public IEnumerator PreviousLevel()
    {
        yield return new WaitForSeconds(0.5f);

        if (Physics2D.OverlapCircle(GroundCheck.position, checkRadius, wrong))
        {
            Physics2D.OverlapCircle(GroundCheck.position, checkRadius, wrong).gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

        platformGenerator.DisappearPlatforms();

        yield return new WaitForSeconds(0.5f);

        platformGenerator.ReappearPlatforms();

        yield return new WaitForSeconds(0.25f);

        //gameObject.transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y - 3, gameObject.transform.position.z);

        newLevelStarted = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("End"))
        {
            StartCoroutine("End");
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("grounded");
        //camera.PlayOneShot(thump);
    }



    public IEnumerator End()
    {
        yield return new WaitForSecondsRealtime(.5f);
        SceneMessenger.instance.LoadNewScene("Overworld", true);
    }

    public IEnumerator Reset()
    {
        platformGenerator.levelResetInTransition = true;

        curLevelPosY = 0;
        transform.position = new Vector3(0f, -3.42f, transform.position.z);


        yield return new WaitForSecondsRealtime(1f);


        oldPos = mainCam.transform.position;

        platformGenerator.RemoveSpeakers();

        //loop
        float timer = 0;
        float timeRemaining = 1f;
        while (timer < timeRemaining)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / timeRemaining;

            mainCam.transform.position = Vector3.Lerp(oldPos, new Vector3(0, 0, oldPos.z), t);

            yield return null;
        }

        platformGenerator.ResetInitialPlatformsAndList();

    }

}

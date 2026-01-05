using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSide : MonoBehaviour
{
    //multiplier
    public float maxSpeed = 1f;

    //acceleration (closer to one the slower acceleration)
    public float acceleration = .5f;

    private float lastVelocityX = 0f;
    private float lastVelocityY = 0f;

    public CircleCollider2D circleCollider;

    private Rigidbody2D rb;

    public bool frozen;

    public int hp = 6;
    public bool isMusicBox = true;
    [System.NonSerialized]
    public AudioSource thisMusicBox;
    public AudioSource otherMusicBox;
    public int noteNumbers = 5; //12 = hole octave
    bool startPlayerBox = false;
    public int noteValue;

    bool isBoxOne = false;
    public ParticleSystem thisSing;
    Animator thisAnim;
    public SpriteRenderer rend;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        thisMusicBox = GetComponent<AudioSource>();
        thisAnim = GetComponent<Animator>();

        noteValue = Random.Range(-noteNumbers + 1, noteNumbers - 1);
        otherMusicBox.pitch = Mathf.Pow(2f, (float)noteValue / 12);
        startPlayerBox = false;


    }

    public void ResetMusicBox()
    {
        noteValue = Random.Range(-noteNumbers, noteNumbers);
        otherMusicBox.pitch = Mathf.Pow(2f, (float)noteValue / 12);
        startPlayerBox = false;

    }
    /*public float PositionToPitch(float y)
    {
        return Mathf.Pow(2f, (((y) / 4.5f) * noteNumbers) / 12);
    }
    public float PitchToPosition(float pitch)
    {
        return Mathf.Log(2f, (((pitch) / 4.5f) * noteNumbers) / 12);
    }*/

    // Update is called once per frame

     public void ToggleBox()
    {
        isBoxOne = !isBoxOne;
        if (isMusicBox && thisMusicBox != null && otherMusicBox != null)
        {

            if (isBoxOne)
            {

                thisMusicBox.Play();
                thisMusicBox.volume = 1;
                thisSing.Play();
                //otherMusicBox.volume = 0;
            }
            else
            {
                thisSing.Stop();
                otherMusicBox.Play();

                otherMusicBox.volume = 1;
                //thisMusicBox.volume = 0;
            }
        }
    }
    void Update()
    {
        if (isMusicBox)
        {

            /*if (startPlayerBox)
            {
                thisMusicBox.volume = 1;
            }
            else
            {
                thisMusicBox.volume = 0;
            }*/
            Debug.Log(thisMusicBox.pitch + "   " + otherMusicBox.pitch);
            int noteRounded = (int)(((transform.position.y) / 4.5f) * noteNumbers);
            thisMusicBox.pitch = Mathf.Pow(2f, noteRounded / 12f);
            //otherMusicBox.volume = 1;
            thisMusicBox.volume = 1;
            otherMusicBox.volume = 1;
        }
        else
        {
            thisMusicBox.volume = 0;
            otherMusicBox.volume = 0;
        }
        //axis raw b/c want to determine acceleration on own
        float inpX = Input.GetAxisRaw("Horizontal");
        float inpY = Input.GetAxisRaw("Vertical");


        if (EnemyManager.enemyMan.isIntro)
        {

        }
        else
        {
            inpX = Mathf.Lerp(inpX, lastVelocityX, acceleration);
            inpY = Mathf.Lerp(inpY, lastVelocityY, acceleration);
            if (inpX != 0 || inpY != 0)
            {
                startPlayerBox = true;
            }

            if (Mathf.Sign(inpX) == -1)
            {
                rend.flipX = true;
            }
            else if (Mathf.Sign(inpX) == 1)
            {
                rend.flipX = false;
            }
            if (inpX != 0 || inpY != 0)
            {
                thisAnim.Play("Base Layer.DungeonPlayerRun");
            }
            else
            {

                thisAnim.Play("Base Layer.Idle");
            }

            if (!frozen)
            {
                rb.velocity = new Vector2(inpX, inpY).normalized * maxSpeed;

            }

            lastVelocityX = inpX;
            lastVelocityY = inpY;
        }



    }

}

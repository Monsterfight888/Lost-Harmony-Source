using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : Enemy
{
    public GameObject angelAttackProj;
    private Transform angelRef;

    public Sprite upArrow;
    public Sprite downArrow;

    public ParticleSystem parts;

    public GameObject playerArrow;
    float distanceToArrow = 2f;
    public override void Initialize(EnemyManager t_eMan)
    {
        base.Initialize(t_eMan);
        transform.position = new Vector2(-5, -2);
        measureLength = 10;

        if(SceneMessenger.instance.tries == 1)
        {
            measureLength = 12;
            distanceToArrow = 1.5f;
            //third try
        }
        else if (SceneMessenger.instance.tries == 2)
        {
            measureLength = 13;
            distanceToArrow = 1f;
            //fourth
        }
        else if(SceneMessenger.instance.tries == 3)
        {
            measureLength = 15;
            distanceToArrow = .5f;
            //fith
        }
        else if (SceneMessenger.instance.tries >= 4)
        {
            measureLength = 15;
            distanceToArrow = 0f;
            //fith
        }
        playerArrow = Instantiate(playerArrow, Vector3.zero, Quaternion.identity);
        playerArrow.SetActive(false);
    }
    private void Update()
    {
        if (dead)
        {
            transform.position += Vector3.up * Time.deltaTime;
        }
    }
    public override void AbortMeasure()
    {

        measureTracker = 0;
    }
    public override bool AttackMeasure()
    {
        float angelAttackPos = ((float)(eMan.player.noteValue) / eMan.player.noteNumbers) * 4.5f;
        bool returner = false ;
        measureTracker++;
        base.AttackMeasure();
        //overide profile w/ arrow (to help people)
        if(measureTracker == 1)
        {

            if (angelRef != null)
            {
                Destroy(angelRef.gameObject);
            }
        }

        if(measureTracker % 2 == 1)
        {
            parts.Play();
            playerArrow.SetActive(false);
        }
        else
        {
            parts.Stop();
            if(measureTracker > 3)
            {
                if (Mathf.Abs(angelAttackPos - eMan.player.transform.position.y) > distanceToArrow)
                {
                    if(eMan.player.otherMusicBox.pitch != eMan.player.thisMusicBox.pitch)
                    {
                        playerArrow.SetActive(true);
                        playerArrow.transform.position = eMan.player.transform.position;

                    }
                    Vector3 vector = Vector3.up * 2.5f;
                    if (Mathf.Sign(angelAttackPos - eMan.player.transform.position.y) == 1)
                    {
                        playerArrow.GetComponent<SpriteRenderer>().sprite = upArrow;
                        playerArrow.GetComponent<Rigidbody2D>().velocity = vector;
                    }
                    else
                    {
                        playerArrow.GetComponent<SpriteRenderer>().sprite = downArrow;
                        playerArrow.GetComponent<Rigidbody2D>().velocity = -vector;

                    }
                    playerArrow.GetComponent<Animator>().Play("Base Layer.Fade");
                }
                else
                {
                    playerArrow.SetActive(false);

                }
            }
            
        }
        if(measureTracker % 3 == 0)
        {
        }
        else
        {

        }

        if (angelAttackPos > eMan.player.transform.position.y)
        {

            //eMan.UIMan.enemyDomainSliderIcon.sprite = upArrow;
        }
        else
        {

            //eMan.UIMan.enemyDomainSliderIcon.sprite = downArrow;
        }

        if (measureTracker == 1)
        {
            eMan.player.isMusicBox = true;
        }
        //start

        if(measureTracker == measureLength)
        {
            //finish and do attack

            measureTracker = 0;
            if (angelRef != null)
            {
                Destroy(angelRef.gameObject);
            }
            angelRef = Instantiate(angelAttackProj, Vector3.zero, Quaternion.identity).transform;
            angelRef.position = new Vector2(0, angelAttackPos);

            if(eMan.player.otherMusicBox.pitch != eMan.player.thisMusicBox.pitch)
            {

                eMan.TakeDamageP(1);

                angelRef.GetComponent<SpriteRenderer>().color = Color.red;
            }
            eMan.player.isMusicBox = false;
            eMan.player.ResetMusicBox();
            returner = true;


        }
        return returner;

        //do attack
    }
}

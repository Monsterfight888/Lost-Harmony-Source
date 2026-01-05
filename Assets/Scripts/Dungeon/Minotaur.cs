using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : Enemy
{

    public Animator anim;

    private Vector3 playerPositionM1;
    public SpriteRenderer rend;

    public float softRaduis = .75f; //used for calculation, however does not reflect real raduis as to shift balance in player's favor
    public override void Initialize(EnemyManager t_eMan)
    {
        health = 2;
        base.Initialize(t_eMan);

        /*if(SceneMessenger.instance.tries == 4)
        {*/
            //activates third try
            leeWay = leeWay - (.004f * SceneMessenger.instance.tries);
        //}

        transform.position = new Vector2(5, 0);
        measureLength = 3;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerSide player = collision.gameObject.GetComponent<PlayerSide>();
        if (player != null)
        {
            Debug.Log("Collided");
            eMan.TakeDamageP(1);
        }

    }

    public override void AbortMeasure()
    {
        rb.velocity = Vector2.zero;

        measureTracker = 0;
    }
    private void Update()
    {
        if (dead)
        {
            rb.velocity = new Vector2(3, 0);
            anim.Play("Base Layer.MinoRun");
        }
    }
    public override bool AttackMeasure()
    {
        if (Mathf.Sign(eMan.player.transform.position.x - transform.position.x) == -1)
        {
            rend.flipX = false;
        }
        else
        {
            rend.flipX = true;
        }
        bool returner = false;

        measureTracker++;

        base.AttackMeasure();
        //Debug.Log("Minotaur " + measureTracker);

        if (measureTracker == 1)
        {
            playerPositionM1 = eMan.player.transform.position;
            anim.Play("Base Layer.MinoIdle");

            //rb.velocity = Vector3.zero;
        }
        else
        {
            

        }
        if (measureTracker == 2)
        {
            //find clossest point on collider to the player's collider 
            Vector3 MinotaurToPlayer = (playerPositionM1 - transform.position);

            /*Vector3 edgeOfPlayerCollider = playerPositionM1 - (MinotaurToPlayer * eMan.player.circleCollider.radius);
            Vector3 edgeOfThisCollider = transform.position + (circleCollider.radius * MinotaurToPlayer);

            Debug.Log("player collider: " + edgeOfPlayerCollider + ", this collider: " + edgeOfThisCollider);*/

            rb.velocity = (MinotaurToPlayer) * (eMan.BPM/60f)/* * leeWay*/; //speed vs. beat are inverse
            anim.Play("Base Layer.MinoRun");
        }
        else if (measureTracker == 3)
        {
            Debug.Log(playerPositionM1 - transform.position);
            //rb.velocity = Vector3.zero;

            Vector3 MinotaurToPlayer = ( eMan.player.transform.position - transform.position ).normalized;

            Vector3 edgeOfPlayerCollider = eMan.player.transform.position - (MinotaurToPlayer * eMan.player.circleCollider.radius);
            Vector3 edgeOfThisCollider = transform.position + (circleCollider.radius * MinotaurToPlayer);

            float speedMult = 1 / ((eMan.player.circleCollider.radius + softRaduis) / 2); //with default values will always calculate out to 8
            speedMult = 8;
            Debug.Log(speedMult);

            rb.velocity = MinotaurToPlayer * (eMan.BPM/60f) * speedMult * leeWay; //player is going to be 4 units away, so must travel 4 units in 1/2 of a beat(since player travels 2 beats per second & must travel one beat to escpae, therefore speed mult must be 8)

            anim.Play("Base Layer.MinoRun");
            //Debug.Log("Distance from player : " + (eMan.player.transform.position - transform.position).magnitude + "\nPosition : " + transform.position);
            //UNCOMMENT THIS WHEN DONE
            //eMan.UIMan.SetSliderValue((3f));
        }
        else if (measureTracker == 4)
        {
            measureTracker = 0;
            returner = true;
            //DELETE THIS WHEN DONE
            //SceneMessenger.instance.LoadNewScene("Overworld");
        }

        return returner;

    }

}

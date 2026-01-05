using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cultist : Enemy
{
    public GameObject stopSpikes;
    public GameObject moveSpike;
    private SpriteRenderer spriteRendM;
    private SpriteRenderer spriteRendS;
    public Sprite circle;
    private Vector3 playerLastPosition;

    private Vector3 enemyPositionGeneration;


    private void Start()
    {
        stopSpikes = new GameObject("Trapp");
        moveSpike = new GameObject("Spike");

        spriteRendM = stopSpikes.AddComponent<SpriteRenderer>();
        spriteRendM.sprite = circle;
        stopSpikes.transform.localScale = Vector3.one;
        spriteRendM.sortingOrder = -1;
        spriteRendM.color = Color.yellow;

        spriteRendS = moveSpike.AddComponent<SpriteRenderer>();
        spriteRendS.sprite = circle;
        moveSpike.transform.localScale = Vector3.one;
        spriteRendS.sortingOrder = -2;
        spriteRendS.color = Color.red;

        stopSpikes.SetActive(true);
        moveSpike.SetActive(false);
        
    }
    public override bool AttackMeasure()
    {

        if(measureTracker % 2 == 0)
        {
            //on beat (even)
            if(measureTracker == 0)
            {
                moveSpike.SetActive(false);
            }
            else
            {
                moveSpike.SetActive(true);
            }

            if(measureTracker == 2)
            {

                //take opposite direction, unless 

                //cases: player moves right or left - find up or down, which ever is open or player moves up or down, find left or right
                
                /*if(eMan.player.transform.position - playerLastPosition.y < 0)
                {

                    transform.position = playerLastPosition + Vector3.right * 1.75f;
                }
                else
                {

                    transform.position = playerLastPosition + Vector3.down;
                }*/
            }

            if(measureTracker == 8)
            {
                measureTracker = 0;
            }
            //transform.position += Vector3.right*4;

            Debug.Log(measureTracker);

            stopSpikes.transform.position = eMan.player.transform.position;
            moveSpike.transform.position = playerLastPosition;
            stopSpikes.SetActive(true);
            moveSpike.SetActive(true);

            playerLastPosition = eMan.player.transform.position;
        }
        else
        {
            //off beat (odd)
            //transform.position += Vector3.right * -4;
            stopSpikes.SetActive(false);
            moveSpike.SetActive(false);

            playerLastPosition = eMan.player.transform.position;
        }

        measureTracker++;

        return false;
    }
}

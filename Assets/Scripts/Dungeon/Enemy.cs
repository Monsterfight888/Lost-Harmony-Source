using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float leeWay = 0.95f;
    public Sprite headPic;

    public int subdivisions = 0; //how many times a signal is sent during the four beat measure (4 for once in a measure, 2 for twice, 1 for four times & .5 for 8 times)
    //(doesnt need to be used for now)

    protected Rigidbody2D rb;
    protected CircleCollider2D circleCollider;
    protected EnemyManager eMan;

    protected int measureTracker = 0;
    protected int measureLength;

    public int health;

    private Transform[] hearts;
    [System.NonSerialized]
    public int currentHealth;
    bool first = true;

    public int finalMeasure;

    protected bool dead = false;

    public virtual void Initialize(EnemyManager t_eMan)
    {
        eMan = t_eMan;
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        currentHealth = health;

        hearts = new Transform[health];

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = transform.Find("Heart " + (i + 1));
        }
        first = true;
    }
    public void TakeDamage()
    {
        currentHealth--;
        if(currentHealth <= 0)
        {
            //SceneMessenger.instance.LoadNewScene("Overworld");
            for (int i = 0; i < eMan.enemyList.Count; i++)
            {
                if(eMan.enemyList[i] == this)
                {
                    /*if(i >= eMan.currentAttackerIndex)
                    {
                        eMan.currentAttackerIndex--;
                    }*/
                    if(i <= eMan.nextAttackerIndex)
                    {
                        eMan.nextAttackerIndex--;
                    }

                }
            }

            eMan.enemyList.Remove(this);

            dead = true;
            //Destroy(gameObject);
        }
        else
        {

            hearts[currentHealth - 1].gameObject.SetActive(false);
        }
    }

    public virtual void activate()
    {

    }
    public virtual bool AttackMeasure()
    {
        //Debug.Log("Based Attack");
        eMan.UIMan.enemyDomainSliderIcon.sprite = headPic;
        if (measureTracker == 1)
        {
            eMan.UIMan.SetSlider(measureLength - 1);
            if (!first)
            {
            }
            first = false;
        }
        else
        {

            eMan.UIMan.SetSliderValue((measureTracker - 1));
        }
        if (measureTracker == finalMeasure)
        {

        }
        return false;
    }

    public void SetNextSliderIcon()
    {
        eMan.UIMan.enemyDomainNextSliderIcon.sprite = headPic;
    }

    public virtual void AbortMeasure()
    {
        Debug.Log("Based Abort");
    }

}

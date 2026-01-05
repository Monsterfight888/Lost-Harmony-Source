using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_Minigame : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject fireballSpawn;

    public float speed;


    public AudioSource dragon;
    public Animator anim;
    public Rigidbody2D rb;

    private int numNotesSoFar;

    public Note half;
    public Note whole;
    public Note quarter;

    public GameObject groundCheck;

    public GameObject disk;

    public Note_Dragon[] level1;

    


    private void Start()
    {
        StartCoroutine("IntroSeq");

        numNotesSoFar = 0;
    }

    IEnumerator IntroSeq()
    {

        yield return new WaitForSecondsRealtime(1f);

        anim.SetTrigger("takeoff");

        yield return new WaitForSecondsRealtime(.5f);

        anim.SetTrigger("land");

        yield return new WaitForSecondsRealtime(1f);

        StartCoroutine("Game");
    }



    IEnumerator Game()
    {
        anim.ResetTrigger("fire");
        anim.SetTrigger("fire");

        dragon.PlayOneShot(level1[numNotesSoFar].sound);

        yield return new WaitForSecondsRealtime(.3335f);

        GameObject temp = Instantiate(fireballPrefab, fireballSpawn.transform.position, Quaternion.identity);

        Vector2 direction =  groundCheck.transform.position - fireballSpawn.transform.position;
        direction.Normalize();
        direction *= speed;
        temp.GetComponent<SpriteRenderer>().sprite = level1[numNotesSoFar].sprite;
        Rigidbody2D fireRidged = temp.GetComponent<Rigidbody2D>();
        fireRidged.velocity = direction;
        


        yield return new WaitForSecondsRealtime(level1[numNotesSoFar].length);

        

        numNotesSoFar++;

        if(numNotesSoFar < level1.Length)
        {
            StartCoroutine("Game");
        }
        else
        {
            StartCoroutine("End");
        }
        
    }

    IEnumerator End()
    {
        yield return new WaitForSecondsRealtime(.5f);

        anim.SetTrigger("takeoff");

        yield return new WaitForSecondsRealtime(.5f);

        anim.SetTrigger("land");

        yield return new WaitForSecondsRealtime(1f);

        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

        yield return new WaitForSecondsRealtime(.5f);

        Instantiate(disk, new Vector2(5.8f, -3f), Quaternion.identity);

        rb.velocity = new Vector2(10, 0);

        
    }
}

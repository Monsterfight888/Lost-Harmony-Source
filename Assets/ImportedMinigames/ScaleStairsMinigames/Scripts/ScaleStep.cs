using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleStep : MonoBehaviour
{
    [System.NonSerialized]
    public bool triggered = false;

    public AudioClip note;

    public AudioSource player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();

    }

    public void playNote(Color color)
    {
        if (!triggered)
        {
            triggered = true;
            player.PlayOneShot(note, 1F);
            gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void repeatNote(Color color)
    {
        player.PlayOneShot(note, 1F);
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }
}

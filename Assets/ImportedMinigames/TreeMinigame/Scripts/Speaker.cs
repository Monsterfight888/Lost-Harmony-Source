using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speaker : MonoBehaviour
{
    public AudioSource player;

    public AudioClip rhythm;
    PlayerControllerTree playerScript;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        playerScript = player.gameObject.GetComponent<PlayerControllerTree>();
    }

    public void buttonPress()
    {
        if (playerScript.GetNewLevelStarted() && !playerScript.soundAlreadyPlaying)
        {
            StartCoroutine("playAudio");
        }
    }

    public IEnumerator playAudio()
    {
        GetComponent<Image>().color = new Color(255, 255, 255, .7f);

        playerScript.soundAlreadyPlaying = true;

        player.PlayOneShot(rhythm, 1F);

        yield return new WaitForSecondsRealtime(2.4f);

        GetComponent<Image>().color = new Color(255, 255, 255, 1f);

        playerScript.soundAlreadyPlaying = false;
        
    }
}

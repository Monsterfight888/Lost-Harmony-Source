using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public Camera cam;
    private Vector3 oldPos;
    private bool abort = false;
    private bool movingCam = false;
    public Text info;

    private int levelCounter = 0;

    public AudioClip[] scale;

    public GameObject player;
    private AudioSource playerAudio;
    private PlayerControllerScale playerController;

    private void Start()
    {
        info.gameObject.SetActive(false);

        playerAudio = player.GetComponent<AudioSource>();
        playerController = player.GetComponent<PlayerControllerScale>();
    }

    public void setInfoActive()
    {
        info.gameObject.SetActive(true);
    }

    public void setInfoInactive()
    {
        info.gameObject.SetActive(false);
    }

    public IEnumerator ChangeCamPos(Vector3 newCamPos)
    {
        movingCam = true;
        //start - any normal function

        oldPos = cam.transform.position;

        //loop
        float timer = 0;
        float timeRemaining = 2f;
        while (timer < timeRemaining)
        {
            if (abort)
            {
                abort = false;
                timer = timeRemaining + 1;
            }
            else
            {
                timer += Time.unscaledDeltaTime;

                float t = timer / timeRemaining;

                cam.transform.position = Vector3.Lerp(oldPos, new Vector3(newCamPos.x, newCamPos.y, oldPos.z), t);


                yield return null;
            }
            
        }


        //finish

        levelCounter++;
        if(levelCounter == 4)
        {
            Debug.Log("Won");
        }

        movingCam = false;
    }

    public IEnumerator PlayScale() 
    {
        playerController.rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSecondsRealtime(1f);


        playerController.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerController.rb.velocity = new Vector2(playerController.rb.velocity.x, playerController.rb.velocity.y - 0.01f);
        playerController.SetShowCasePlayingATM(false);
    }

    public void Abort()
    {
        if(movingCam == true)
        {
            abort = true;
        }
        
    }
}

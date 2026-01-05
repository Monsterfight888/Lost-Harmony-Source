using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    private Vector3 oldPos;
    private bool active = true;
    private bool finished = false;
    private float changeY;
    private float changeX;

    public AudioClip platform1N;
    public AudioClip platform2N;
    public AudioClip platform3N;

    public GameObject platform1;
    public GameObject platform2;
    public GameObject platform3;

    public void Transport(GameObject platform, bool ascending)
    {
        if (!active)
        {
            return;
        }

        active = false;
        finished = false;

        if (platform1 == platform)
        {
            if (ascending)
            {
                changeX = 8.8f;
                changeY = 5.9f;
            }
            else
            {
                changeX = -8.8f;
                changeY = -5.9f;
            }
        }
        else if (platform2 == platform)
        {
            if (ascending)
            {
                changeX = 5;
                changeY = 5.9f;
            }
            else
            {
                changeX = -5;
                changeY = -5.9f;
            }
        }
        else
        {
            if (ascending)
            {
                changeX = 1.2f;
                changeY = 5.9f;
            }
            else
            {
                changeX = -1.2f;
                changeY = -5.9f;
            }
        }

        StartCoroutine("ChangePlatPosY", platform);
    }

    public IEnumerator ChangePlatPosY(GameObject platform)
    {
        //start - any normal function

        oldPos = platform.transform.position;

        //loop
        float timer = 0;
        float timeRemaining = 0.85f;
        while (timer < timeRemaining)
        {
            timer += Time.deltaTime;

            float t = timer / timeRemaining;

            platform.transform.position = Vector3.Lerp(oldPos, new Vector3(oldPos.x, changeY + oldPos.y, oldPos.z), t);


            yield return null;
        }

        StartCoroutine("ChangePlatPosX", platform);

    }

    public IEnumerator ChangePlatPosX(GameObject platform)
    {
        //start - any normal function

        oldPos = platform.transform.position;

        //loop
        float timer = 0;
        float timeRemaining = 0.85f;
        while (timer < timeRemaining)
        {
            timer += Time.deltaTime;

            float t = timer / timeRemaining;

            platform.transform.position = Vector3.Lerp(oldPos, new Vector3(changeX + oldPos.x, oldPos.y, oldPos.z), t);


            yield return null;
        }

        if (!finished)
        {
            finished = true;

            yield return new WaitForSecondsRealtime(3f);
    

            changeX *= -1;
            changeY *= -1;

            StartCoroutine("ChangePlatPosY", platform);
        }

    }


    public void setActiveTrue()
    {
        active = true;
    }
}


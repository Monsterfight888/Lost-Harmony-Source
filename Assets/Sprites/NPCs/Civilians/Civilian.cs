using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : MonoBehaviour
{

    private Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();

        StartCoroutine("StartAnimation");
    }

    IEnumerator StartAnimation()
    {
        float waitTime = Random.Range(0f, 3.5f);
        //Debug.Log(waitTime);

        yield return new WaitForSecondsRealtime(waitTime);

        anim.SetTrigger("StartAnimation");

    }

}

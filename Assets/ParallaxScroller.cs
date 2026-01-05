using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    public float damping;
    public float scale = 6;

    public bool isY = false;
    public bool doesRepeat = true;

    private Transform cam;
    private float offset;
    private Vector3 staticOffset;

    private void Start()
    {
        cam = transform.parent.GetComponent<Transform>();
        offset = 0;
        staticOffset = transform.localPosition;
    }

    public void Update()//must be parented to cam
    {
        float pos = cam.position.x;
        if (isY)
        {
            pos = cam.position.y;
        }


        /*if(pos / damping >= transform.localScale.x*2)
        {
            offset += transform.localScale.x * 2;
        }*/
        float wrappConst = (scale * 3);
        if (!doesRepeat)
        {
            wrappConst = 500;

        }
        offset = Mathf.Floor((pos / damping) / (wrappConst)) *(wrappConst);//this half + two next hals = 3
        transform.localPosition = new Vector3(-pos/ damping + offset, 0, 10) + staticOffset;

        if (isY)
        {
            transform.localPosition = new Vector3(0, -pos / damping + offset, 10) + staticOffset;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger: MonoBehaviour
{
    public Vector3 newCamPos;
    public Manager manager;

    public bool special;
    public Platforms platformSet;

    public void ChangeCamPos()
    {
        manager.StartCoroutine("ChangeCamPos", newCamPos);
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        SceneMessenger.instance.LoadNewScene("Overworld", true);
    }
}

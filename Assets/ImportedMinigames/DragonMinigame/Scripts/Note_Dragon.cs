using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewNote", menuName = "ScriptableObjects/NewNote", order = 1)]
public class Note_Dragon : ScriptableObject
{
    public float length;

    public Sprite sprite;

    public AudioClip sound;
}

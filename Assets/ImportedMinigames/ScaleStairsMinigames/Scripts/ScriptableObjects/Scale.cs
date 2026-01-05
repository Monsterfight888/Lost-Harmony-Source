using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Scale", menuName = "ScriptableObjects/Scale", order = 1)]
public class Scale : ScriptableObject
{
    public Note[] notes;
}


[Serializable]
public struct Note
{
    public AudioClip note;
}
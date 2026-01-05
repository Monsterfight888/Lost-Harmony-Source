using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DialogueInst", menuName = "ScriptableObjects/DialogueInstance", order = 2)]
public class DialogueInst : ScriptableObject
{
    public string name;
    public Sprite profile;

    public string otherName;
    public Sprite otherProfile;

    [TextArea(3, 10)]
    public string[] sentences;

    public Vector3Int coord;

    public NPC NotifyReciever;
    public float raduis; //if going to light up area after talk

    public Path pathInst;
}

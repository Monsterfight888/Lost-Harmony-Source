using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewLevel", menuName = "ScriptableObjects/NewLevel", order = 1)]
public class Level : ScriptableObject
{
    public LevelStats[] levelStats;
}


[Serializable]
public struct LevelStats
{
    public Sprite rhythm1;
    public Sprite rhythm2;

    public AudioClip AudioRhy1;
    public AudioClip AudioRhy2;
}
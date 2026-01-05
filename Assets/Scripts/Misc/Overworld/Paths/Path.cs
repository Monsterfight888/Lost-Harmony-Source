using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "PathInst", menuName = "ScriptableObjects/Path", order = 2)]
public class Path : ScriptableObject
{
    [Header("The path alternates based on which it is, element 0 starts on x axis & then goes to y & repeats")]
    public int[] pathData;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntroPatternInst", menuName = "ScriptableObjects/IntroPattern", order = 2)]
public class IntroPattern : ScriptableObject
{
    [Header("from 0 - 3: left, up, down, right")]
    public IntroArrow[] Slots = new IntroArrow[4];

    [System.NonSerialized]
    public int ArrowCount;

}

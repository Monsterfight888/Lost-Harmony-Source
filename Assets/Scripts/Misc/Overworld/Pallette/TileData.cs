using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileData : ScriptableObject
{
    public TileBase[] tileDomain;

    public Vector2Int TeleportLocation; //when 0,0,0 then it's not a teleporter

    public Vector2Int FonographLocation; //when 0,0,0 then it's not a teleporter
    public bool completed = false;

    public string sceneName;
}

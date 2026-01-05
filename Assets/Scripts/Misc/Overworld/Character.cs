using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{

    //configurables
    public float WalkTransitionTime;


    //transitional values
    public bool inTransition;
    public bool lastTransition;

    //external components
    protected WorldData worldData;

    //internal components
    public Rigidbody2D rb;
    public IEnumerator TransitionIE;
    public Vector3 snapPos;
    public Vector3Int dir;

    public Vector3 velocity;
    Vector3 lastPosition;
    private float timeBetween = .1f;

    public virtual void Start()
    {
        worldData = WorldData.instance;
        rb = GetComponent<Rigidbody2D>();
        snapPos = transform.position;
    }
    public void PositionSnap() // b/c of floating point presicion errors, gradually player gets off with the grid
    {
            if (lastTransition)//was in the transition last frame
            {
                //transform.position = snapPos;
                velocity = Vector3.zero;
            }
        
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        snapPos = pos;
    }
    public virtual void Update()
    {
        /*if (inTransition)
        {

            transform.position += velocity * Time.deltaTime;
        }*/

        lastTransition = inTransition;
    }

    private void FixedUpdate()
    {
        /*if (inTransition)
        {
            float difference = velocity.x == 0 ? transform.position.y - snapPos.y : transform.position.x - snapPos.x;
            
            if (Mathf.Abs(difference) < 0.01)
            {
                PositionSnap();
                StopCoroutine(TransitionIE);
                inTransition = false;
            }
        }*/
    }
    public IEnumerator Transition(float time)
    {
        inTransition = true;
        float waitTimer = 0;
        time += timeBetween;
        while(waitTimer <= time)
        {
            waitTimer += Time.unscaledDeltaTime;
            float lerpValue = waitTimer / time;

            transform.position = Vector3.Lerp(lastPosition, snapPos, lerpValue);

            yield return null;
        }

        inTransition = false;

    }

    public virtual bool Move(Vector3 movementVector)//takes larger value b/c no need for more fleshed out system
    { 
        Vector3Int gridPlacement = worldData.map.WorldToCell(transform.position + movementVector);
        //TileBase tileToMoveTo = worldData.map.GetTile(gridPlacement) ;
        bool escapeFlag = false;

        int posInBoolx = gridPlacement.x - worldData.gridPosition.x;
        int posInBooly = gridPlacement.y - worldData.gridPosition.y;

        if (posInBoolx >= worldData.walkMap.GetLength(0) - 1 || posInBooly >= worldData.walkMap.GetLength(1) - 1 ||
            posInBoolx < 0 || posInBooly < 0)
        {
            escapeFlag = true;

        }
        else
        {
            escapeFlag = worldData.walkMap[posInBoolx, posInBooly];

        }
        if(worldData.map.GetTile(gridPlacement) == null)
        {
            escapeFlag = true;
        }
        /*string mape = "";
        for (int i = 0; i < worldData.walkMap.GetLength(0); i++)
        {
            for (int l = 0; l < worldData.walkMap.GetLength(1); l++)
            {
                if(i == gridPlacement.x - worldData.gridPosition.x && l == gridPlacement.y - worldData.gridPosition.y)
                {
                    mape += "p";
                }

                if (worldData.walkMap[i, l])
                {
                    mape += "0 ";
                }
                else
                {
                    mape += "1 ";
                }
            }
            mape += "\n";
        }

        Debug.Log(mape);*/

        /*for (int i = 0; i < worldData.tilesDictionary.Length; i++)
        {
            for (int l = 0; l < worldData.tilesDictionary[0].tileDomain.Length; l++)
            {
                if (worldData.tilesDictionary[i].tileDomain[l] == tileToMoveTo)
                {
                    if (!worldData.tilesDictionary[i].isWalkable)
                    {
                        escapeFlag = true;
                    }
                }
            }
        }*/
        for (int i = 0; i < worldData.UnWalkablePos.Count; i++)
        {
            //Vector3Int debug = worldData.map.WorldToCell(transform.position + movementVector);
            //Vector3Int f = worldData.gridPosition;
            if (worldData.UnWalkablePos[i] == worldData.map.WorldToCell(transform.position + movementVector))
            {
                escapeFlag = true;
            }
        }
        /*if (escapeFlag)//break when attempting to move onto an unwalkable tile
            return false;*/

        TransitionIE = Transition(WalkTransitionTime);

        if(!escapeFlag)
        {

            snapPos = snapPos + (movementVector);
        }
        lastPosition = transform.position;
        StartCoroutine(TransitionIE);
        //velocity = movementVector * (1 / WalkTransitionTime);
        return true;
    }
}

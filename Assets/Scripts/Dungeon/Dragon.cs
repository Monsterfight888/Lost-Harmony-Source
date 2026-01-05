using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy
{
    public Transform fireballTrans;
    public Rigidbody2D fireballRB;

    public override void Initialize(EnemyManager t_eMan)
    {
        health = 2;
        base.Initialize(t_eMan);
        measureLength = 5;
    }


    public override bool AttackMeasure()
    {

        bool returner = false;
        measureTracker++;

        if(measureTracker == 1)
        {
            //fly in and do whole thing
        }
        else if (measureTracker == 2)
        {
            
        }

        return returner;
    }
}

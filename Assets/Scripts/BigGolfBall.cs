using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BigGolfBall : GolfBall
{
    private new void Start()
    {
        base.Start();
        playerPrefix = "P1";
    }
}
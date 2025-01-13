using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SmallGolfBall : GolfBall
{
    private new void Start()
    {
        base.Start();
        playerPrefix = "P2";
    }
}
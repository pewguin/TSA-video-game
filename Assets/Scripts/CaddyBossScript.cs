using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public class CaddyBossScript : MonoBehaviour
{
    public Transform player;
    public bool fightStarted;

    public int health;

    public bool attacking;
    public float startAttackInterval;
    private float attackIntervalTimer;
    public int numOfAttacks;
    private int numOfAttacksCounter;

    public GameObject clubAttack;

    public HatchLidScript hatch;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartFightDelay());
        numOfAttacksCounter = numOfAttacks;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fightStarted) { return; }
        if (attacking)
        {
            if(attackIntervalTimer <= 0)
            {
                Attack();
                numOfAttacksCounter--;
                attackIntervalTimer = startAttackInterval;
            }
            else
            {
                attackIntervalTimer -= Time.deltaTime;
            }
            if (numOfAttacksCounter <= 0)
            {
                attacking = false;
                numOfAttacksCounter = numOfAttacks; // end attacks
                OpenHatch();
            }
        }
        if (health < 0)
        {
            // WIN!
        }

        
    }

    public void OpenHatch()
    {
        Debug.Log("openhatch");
        hatch.RotateToEnd();
    }
    public void CloseHatch()
    {
        hatch.RotateToStart();
    }

    public void Attack()
    {
        Instantiate(clubAttack, new Vector3(player.position.x, clubAttack.transform.position.y, 0), clubAttack.transform.rotation); 
    }

    private IEnumerator StartFightDelay()
    {
        yield return new WaitForSeconds(1f);
        fightStarted = true;
    }

    public void SelfDestruct()
    {
        health--;
    }
}

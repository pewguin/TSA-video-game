using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] int playerMaxHealth = 3; // Die when you hit 0
    public int playerHealth;

    public GameObject ejectEffect;
    public GameObject catchEffect;
    public Image[] hearts;
    public Slider slid;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = playerMaxHealth;
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
            Debug.Log("you win!");
        }
        if (playerHealth < playerMaxHealth)
        {
            hearts[playerHealth].color = Color.white * 0.5f;
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
        Instantiate(clubAttack, new Vector3(player.position.x, clubAttack.transform.position.y + player.position.y, 0), clubAttack.transform.rotation); 
    }

    private IEnumerator StartFightDelay()
    {
        yield return new WaitForSeconds(1f);
        fightStarted = true;
    }

    public void SelfDestruct()
    {
        health--;
        Debug.Log("hit!");
        StartCoroutine(ToggleActiveForSeconds(1.5f, ejectEffect));
        StartCoroutine(ToggleActiveForSeconds(1.5f, catchEffect));
        hatch.RotateToStart();
        attackIntervalTimer = startAttackInterval;
        attacking = true;
        slid.value = health / 3f;
        if (health == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public IEnumerator ToggleActiveForSeconds(float time, GameObject go)
    {
        go.SetActive(!go.activeSelf);
        yield return new WaitForSeconds(time);
        go.SetActive(!go.activeSelf);
        hatch.RotateToStart();
    }
}

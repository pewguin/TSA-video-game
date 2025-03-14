using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GolfBall : MonoBehaviour
{
    [Tooltip("How slow the ball has to be before it can be considered \"still\"")]
    [SerializeField] private float speedThreshold = 1f;
    [Tooltip("How long the ball must remain under the threshold before it is considered \"still\"")]
    [SerializeField] private float stillDuration = 0.5f;
    [Tooltip("How much force is applied to the ball at full charge")]
    [SerializeField] private float maxHitStrength = 2500f;
    [Tooltip("Time in seconds to reach full charge and time to fall back to zero charge")]
    [SerializeField] private float chargeSpeed = 1f;
    [SerializeField] private float maxChargeSeconds = 3f;
    [Tooltip("How fast the target angle changes in deg/sec")]
    [SerializeField] private float aimSpeed = 120f;
    [SerializeField] private float minForce = 2f;
    [SerializeField] private string playerPrefix1;
    [SerializeField] private string playerPrefix2;
# nullable enable
    [SerializeField] private CaddyBossScript? boss;
# nullable disable
    [SerializeField] private float knockback = 1000;

    [SerializeField] private float minRotationaVelo = -10f;
    [SerializeField] private float maxRotationaVelo = 10f;
    private Rigidbody2D rb;
    private float targetAngle1 = 0f;
    private float targetAngle2 = 0f;
    public float chargeTime1 = 0f;
    public float chargeTime2 = 0f;
    private bool still = false;
    private float stillTime = 0f;
    private bool P1shoot = true;
    private bool P2shoot = true;

    private bool canTrigger = true;

    TrajectoryPredictor1 trajPredictor1;
    TrajectoryPredictor2 trajPredictor2;

    Collider2D jankCoding;

    //public AudioSource HitSFX;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trajPredictor1 = GetComponent<TrajectoryPredictor1>();
        trajPredictor2 = GetComponent<TrajectoryPredictor2>();
    }
    protected void Update()
    {

        if (rb.velocity.magnitude < speedThreshold && !still)
        {
            stillTime += Time.deltaTime;
            if (stillTime > stillDuration)
            {
                stillTime = 0f;
                still = true;
                P1shoot = true;
                P2shoot = true;
            }
        }
        else if (rb.velocity.magnitude > speedThreshold)
        {
            still = false;
            stillTime = 0f;
        }

        // Adjust target angle according to input
        targetAngle1 += (Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0) * Time.deltaTime * aimSpeed;
        targetAngle2 += (Input.GetKey(KeyCode.LeftArrow) ? -1 : Input.GetKey(KeyCode.RightArrow) ? 1 : 0) * Time.deltaTime * aimSpeed;

        // Charge the hit
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            chargeTime1 = Mathf.Min(maxChargeSeconds, chargeTime1 + Time.deltaTime);
            if (P1shoot == true)
            {
                trajPredictor1.Show();
            }
            else
            {
                trajPredictor1.makeSeethrough();
            }

            Vector2 force = (CalculateHitPower(chargeTime1) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle1 * Mathf.Deg2Rad), Mathf.Sin(targetAngle1 * Mathf.Deg2Rad)));
            trajPredictor1.UpdateDots(transform.position, force / rb.mass);
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            chargeTime1 = Mathf.Max(0.011f, chargeTime1 - Time.deltaTime);
            if (P1shoot == true)
            {
                trajPredictor1.Show();
            }
            else
            {
                trajPredictor1.makeSeethrough();
            }

            Vector2 force = (CalculateHitPower(chargeTime1) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle1 * Mathf.Deg2Rad), Mathf.Sin(targetAngle1 * Mathf.Deg2Rad)));
            trajPredictor1.UpdateDots(transform.position, force / rb.mass);

        }
        // Hit the ball
        else if (chargeTime1 > 0.01 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            if (P1shoot)
            {
                rb.velocity = Vector3.zero;

                rb.AddForce(
                        CalculateHitPower(chargeTime1) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle1 * Mathf.Deg2Rad), Mathf.Sin(targetAngle1 * Mathf.Deg2Rad)), ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(minRotationaVelo, maxRotationaVelo), ForceMode2D.Impulse);
                P1shoot = false;
            }
            chargeTime1 = 0;
            trajPredictor1.Hide();
        }
        else
        {
            Vector2 force = (CalculateHitPower(chargeTime1) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle1 * Mathf.Deg2Rad), Mathf.Sin(targetAngle1 * Mathf.Deg2Rad)));
            trajPredictor1.UpdateDots(transform.position, force / rb.mass);
        }
        if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            chargeTime2 = Mathf.Min(maxChargeSeconds, chargeTime2 + Time.deltaTime);

            if (P2shoot == true)
            {
                trajPredictor2.Show();
            }
            else
            {
                trajPredictor2.makeSeethrough();
            }
            Vector2 force = (CalculateHitPower(chargeTime2) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle2 * Mathf.Deg2Rad), Mathf.Sin(targetAngle2 * Mathf.Deg2Rad)));
            trajPredictor2.UpdateDots(transform.position, force / rb.mass);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            chargeTime2 = Mathf.Max(0.011f, chargeTime2 - Time.deltaTime);

            if (P2shoot == true)
            {
                trajPredictor2.Show();
            }
            else
            {
                trajPredictor2.makeSeethrough();
            }
            Vector2 force = (CalculateHitPower(chargeTime2) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle2 * Mathf.Deg2Rad), Mathf.Sin(targetAngle2 * Mathf.Deg2Rad)));
            trajPredictor2.UpdateDots(transform.position, force / rb.mass);
        }
        // Hit the ball
        else if (chargeTime2 > 0.01 && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            if (P2shoot)
            {
                rb.velocity = Vector3.zero;

                rb.AddForce(
                        CalculateHitPower(chargeTime2) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle2 * Mathf.Deg2Rad), Mathf.Sin(targetAngle2 * Mathf.Deg2Rad)), ForceMode2D.Impulse);

                rb.AddTorque(Random.Range(minRotationaVelo, maxRotationaVelo), ForceMode2D.Impulse);
                P2shoot = false;
            }
            chargeTime2 = 0;
            trajPredictor2.Hide();
        }
        else
        {
            Vector2 force = (CalculateHitPower(chargeTime2) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle2 * Mathf.Deg2Rad), Mathf.Sin(targetAngle2 * Mathf.Deg2Rad)));
            trajPredictor2.UpdateDots(transform.position, force / rb.mass);
        }

    }
    private float CalculateHitPower(float time)
    {
        return chargeSpeed * Mathf.Pow(Mathf.Max(0.01f, time), 0.5f) + minForce;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitRefresher") && canTrigger)
        {
            jankCoding = collision;
            P1shoot = true;
            P2shoot = true;
            
            StartCoroutine(Cooldown(collision));
            
        }
        if (collision.gameObject.CompareTag("club")) //SELF DESTRUCT
        {
            collision.transform.parent.GetComponent<CaddyBossScript>().SelfDestruct();
        }
    }
    private IEnumerator Cooldown(Collider2D collider)
    {
        SpriteRenderer renderer = collider.gameObject.GetComponent<SpriteRenderer>();
        collider.enabled = false;
        Color color = renderer.color;
        color.a = 0.5f;
        renderer.color = color;
        color.a = 1f;
        canTrigger = false;
        yield return new WaitForSecondsRealtime(5);
        canTrigger = true;
        renderer.color = color;
        collider.enabled = true;

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("club"))
        {
            Die();
            rb.AddForce((col.GetContact(0).point - (Vector2)transform.position) * knockback, ForceMode2D.Force);
        }
    }
    void Die()
    {
        if (boss != null)
        {
            boss.playerHealth--;
            if (boss.playerHealth <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
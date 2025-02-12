using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
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
    [Tooltip("How fast the target angle changes in deg/sec")]
    [SerializeField] private float aimSpeed = 120f;
    [SerializeField] private float minForce = 2f;
    [SerializeField] private string playerPrefix1;
    [SerializeField] private string playerPrefix2;

    [SerializeField] private float minRotationaVelo = -10f;
    [SerializeField] private float maxRotationaVelo = 10f;
    private Rigidbody2D rb;
    private float targetAngle1 = 0f;
    private float targetAngle2 = 0f;
    private float chargeTime1 = 0f;
    private float chargeTime2 = 0f;
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
        targetAngle1 += Input.GetAxis(playerPrefix1 + "Horizontal") * Time.deltaTime * aimSpeed;
        targetAngle2 += Input.GetAxis(playerPrefix2 + "Horizontal") * Time.deltaTime * aimSpeed;

        // Charge the hit
        if (Input.GetAxisRaw(playerPrefix1 + "Vertical") > 0.1)
        {
            chargeTime1 += Time.deltaTime;
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
        else if (chargeTime1 > 0.01)
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
        if (Input.GetAxisRaw(playerPrefix2 + "Vertical") > 0.1)
        {
            chargeTime2 += Time.deltaTime;

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
        else if (chargeTime2 > 0.01)
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

    }
    // Equation maps [0, infinity) to [0, 1] and bounces between 0 and 1




    private float CalculateHitPower(float time)
    {
        return 0.5f * (1 + -Mathf.Cos(Mathf.PI / chargeSpeed * time)) + minForce;
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

}
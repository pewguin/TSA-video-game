using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    [SerializeField] private string playerPrefix;
    private Rigidbody2D rb;
    private float targetAngle = 0f;
    private float chargeTime = 0f;
    private bool still = false;
    private float stillTime = 0f;

    TrajectoryPredictor trajPredictor;


    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trajPredictor = GetComponent<TrajectoryPredictor>();
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
            }
        }
        else if (rb.velocity.magnitude > speedThreshold)
        {
            still = false;
            stillTime = 0f;
        }

        // Adjust target angle according to input
        targetAngle += Input.GetAxis(playerPrefix + "Horizontal") * Time.deltaTime * aimSpeed;

        // Charge the hit
        if (Input.GetAxisRaw(playerPrefix + "Vertical") > 0.1)
        {
            chargeTime += Time.deltaTime;
            trajPredictor.Show();
            Vector2 force = (CalculateHitPower(chargeTime) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle * Mathf.Deg2Rad), Mathf.Sin(targetAngle * Mathf.Deg2Rad)));
            trajPredictor.UpdateDots(transform.position, force / rb.mass);





        }
        // Hit the ball
        else if (chargeTime > 0.01)
        {
            if (still)
            {
                rb.AddForce(
                        CalculateHitPower(chargeTime) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle * Mathf.Deg2Rad), Mathf.Sin(targetAngle * Mathf.Deg2Rad)), ForceMode2D.Impulse);
            }
            chargeTime = 0;
            trajPredictor.Hide();

        }
    }
    // Equation maps [0, infinity) to [0, 1] and bounces between 0 and 1




    private float CalculateHitPower(float time)
    {
        return 0.5f * (1 + -Mathf.Cos(Mathf.PI / chargeSpeed * time));
    }
    // Draw the aim line, waiting on Gavi for an implementation in-game
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Color.red, Color.green, CalculateHitPower(chargeTime));
        Vector2 targetVector = new Vector2(-Mathf.Cos(targetAngle * Mathf.Deg2Rad), Mathf.Sin(targetAngle * Mathf.Deg2Rad));
        targetVector *= (CalculateHitPower(chargeTime) + 0.2f) * 5;
        Gizmos.DrawLine(transform.position, targetVector + (Vector2)transform.position);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GolfBall : MonoBehaviour
{
    [Tooltip("Player ID, 0 for player one and 1 for player two")]
    [SerializeField] int player = 0;
    [Tooltip("Transform of the golf club visual")]
    [SerializeField] Transform golfClub;
    [Tooltip("How slow the ball has to be before it can be considered \"still\"")]
    [SerializeField] float speedThreshold = 1f;
    [Tooltip("How long the ball must remain under the threshold before it is considered \"still\"")]
    [SerializeField] float stillDuration = 0.5f;
    [Tooltip("How much force is applied to the ball at maximum power. Power asymptotically approaches the maximum")]
    [SerializeField] float maxHitStrength = 2500f;

    [SerializeField] ClubProperties clubProperties;
    [SerializeField] SwingAnimationProperties swingAnimation;
    
    private float swingCurrentSpeed = 0f;
    private float swingCurrentRot = 0f;
    private string playerPrefix;
    private Transform golfClubPivot;
    private Rigidbody2D rb;
    private float targetAngle = 0f;
    private float chargeTime = 0f;
    private bool swinging = false;
    private bool charging = false;
    private Quaternion golfStickPreSwingRotation;
    private Vector3 initialScale;
    private bool still = false;
    private float stillTime = 0f;
    private bool ballInCorrectPlace = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        golfClubPivot = golfClub.GetChild(0);
        initialScale = golfClub.transform.localScale;
        playerPrefix = "P" + (player + 1);
    }
    private void Update()
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
            ballInCorrectPlace = false;
            stillTime = 0f;
        }
        // Position golf club correctly
        if (still && !swinging && !charging)
        {
            golfClub.position = transform.position;
            golfClub.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Cos(targetAngle * Mathf.Deg2Rad) < 0 ? 180 : 0));
            Vector3 newScale = initialScale;
            newScale.y *= Mathf.Cos(targetAngle * Mathf.Deg2Rad) < 0 ? -1 : 1;
            golfClub.localScale = newScale;
            ballInCorrectPlace = true;
        }

        // Adjust target angle according to input
        targetAngle += Input.GetAxis(playerPrefix + "Horizontal") * Time.deltaTime * clubProperties.aimSpeed;
        
        // Charge the club
        if (Input.GetAxisRaw(playerPrefix + "Vertical") > 0.1) {
            charging = true;
            chargeTime += Time.deltaTime;
            golfClubPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, 180 * CalculateHitPower(chargeTime)));
        }
        // Released the club, prepare swing
        else if (chargeTime > 0.01 && !swinging)
        {
            swinging = true;
            charging = false;
            golfStickPreSwingRotation = golfClubPivot.localRotation;
        } 
        else
        {
            charging = false;
        }
        // Swing the club
        // Uses an accelerating slerp to look like a swing
        if (swinging)
        {
            golfClubPivot.localRotation = Quaternion.Slerp(golfStickPreSwingRotation, Quaternion.identity, swingCurrentRot);
            swingCurrentSpeed += swingAnimation.swingAcceleration * Time.deltaTime;
            swingCurrentRot += swingCurrentSpeed * Time.deltaTime;
            // Add force to the ball when it is hit and reset swing variables
            if (swingCurrentRot > 1)
            {
                swinging = false;
                swingCurrentRot = 0;
                swingCurrentSpeed = 0;
                if (ballInCorrectPlace)
                {
                    rb.AddForce(
                        CalculateHitPower(chargeTime) * maxHitStrength *
                        new Vector2(-Mathf.Cos(targetAngle * Mathf.Deg2Rad), Mathf.Sin(targetAngle * Mathf.Deg2Rad)));
                }
                chargeTime = 0;
            }
        }
    }
    // Equation maps [0, infinity) to [0, 180)
    private float CalculateHitPower(float time)
    {
        return Mathf.Pow(time, clubProperties.chargeShape) 
            / (1 / clubProperties.chargeSpeed + Mathf.Pow(time, clubProperties.chargeShape));
    }
    // Draw the aim line, waiting on Gavi for an implementation in-game
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(-Mathf.Cos(targetAngle * Mathf.Deg2Rad), Mathf.Sin(targetAngle * Mathf.Deg2Rad)) * 100);
    }
}

[System.Serializable]
public class ClubProperties
{
    [Tooltip("How fast power approaches the maximum")]
    [SerializeField] public float chargeSpeed = 2f;
    [Tooltip("The shape of the power curve, probably don't touch")]
    [SerializeField] public float chargeShape = 1f;
    [Tooltip("Speed the target angle changes at (deg/sec)")]
    [SerializeField] public float aimSpeed = 90f;
}
[System.Serializable]
public class SwingAnimationProperties
{
    [Tooltip("How fast the golf club speeds up in the animation")]
    [SerializeField] public float swingAcceleration = 4f;
}
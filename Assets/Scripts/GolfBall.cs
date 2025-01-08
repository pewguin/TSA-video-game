using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GolfBall : MonoBehaviour
{
    private bool playerAiming = false;
    private bool swinging = false;
    Quaternion golfStickPreSwingRotation;
    [SerializeField] float golfStickRotationMultiplier = 1f;
    // how powerful the hit from the player is
    [SerializeField] float hitStrength = 1f;
    // how slow ball must be going before player could hit it
    [SerializeField] float speedThreshhold = 1f;
    [SerializeField] float swingAcceleration = 0.4f;
    private float swingCurrentSpeed = 0f;
    private float swingCurrentRot = 0f;
    // golf stick visual
    [SerializeField] Transform golfStick;
    // makes rotating the stick easy
    private Transform golfStickPivot;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        golfStickPivot = golfStick.GetChild(0);
    }
    private void Update()
    {
        if (playerAiming)
        {
            Vector2 mouseToBall = (Vector2)transform.position - getWorldMousePosition();
            golfStick.position = transform.position;
            float angle = Mathf.Atan2(mouseToBall.y, mouseToBall.x) * Mathf.Rad2Deg;
            golfStick.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));
            golfStickPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, 179 * (1 - Mathf.Pow(2.71828f, -calculateShotVector().magnitude * golfStickRotationMultiplier))));
        } else if (swinging)
        {
            golfStickPivot.localRotation = Quaternion.Slerp(golfStickPreSwingRotation, Quaternion.identity, swingCurrentRot);
            swingCurrentSpeed += swingAcceleration * Time.deltaTime;
            swingCurrentRot += swingCurrentSpeed * Time.deltaTime;
            if (swingCurrentRot > 1f)
            {
                swinging = false;
                swingCurrentRot = 0f;
                swingCurrentSpeed = 0f;
                rb.AddForce(calculateShotVector());
            }
        }
    }
    private void OnMouseDown()
    {
        if (rb.velocity.magnitude < speedThreshhold)
        {
            playerAiming = true;
        }
    }
    private void OnMouseUp()
    {
        if (rb.velocity.magnitude < speedThreshhold)
        {
            golfStickPreSwingRotation = golfStickPivot.localRotation;
            swinging = true;
            playerAiming = false;
        }
    }

    private Vector2 calculateShotVector(Vector2 fromPos)
    {
        Vector2 shotVec = (Vector2)transform.position - getWorldMousePosition();
        shotVec *= hitStrength;
        return shotVec;
    }
    private Vector2 calculateShotVector()
    {
        return calculateShotVector(getWorldMousePosition());
    }
    private Vector2 getWorldMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnDrawGizmos()
    {
        if (playerAiming)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(getWorldMousePosition(), transform.position);
        }
    }
}

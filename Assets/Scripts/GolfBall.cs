using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GolfBall : MonoBehaviour
{
    private bool swinging = false;
    private Quaternion golfStickPreSwingRotation;
    [SerializeField] float golfStickRotationMultiplier = 1f;
    [SerializeField] int player = 0; // 0 or 1
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
    // used for kb controls
    public float golfStickRotation = 0f;
    private float keyboardChargeTime = 0f;
    // https://www.desmos.com/calculator/k0qjfnotew
    // a is speed, b is initial
    // f(x) is how shot power grows over time, g(x) is the total power with respect to time (antiderivative)
    [SerializeField] float keyboardChargeSpeed = 5f;
    [SerializeField] float keyboardChargeInitial = 4f;
    [SerializeField] float keyboardGolfStickRotationSpeed = 1f;
    Vector3 initialScale;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        golfStickPivot = golfStick.GetChild(0);
        initialScale = golfStick.transform.localScale;
    }
    private void Update()
    {
        golfStick.position = transform.position;
        golfStick.rotation = Quaternion.Euler(new Vector3(0, 0,  Mathf.Cos(golfStickRotation * Mathf.Deg2Rad) < 0 ? 180 : 0));
        Vector3 newScale = initialScale;
        newScale.y *= Mathf.Cos(golfStickRotation * Mathf.Deg2Rad) < 0 ? -1 : 1;
        golfStick.localScale = newScale;
        golfStickRotation +=
            (player == 0 ? Input.GetAxis("P1Horizontal") :
            player == 1 ? Input.GetAxis("P2Horizontal") : 0) * Time.deltaTime * keyboardGolfStickRotationSpeed;
        string axis =
            player == 0 ? "P1Vertical" :
            player == 1 ? "P2Vertical" : "";
        if (Input.GetAxisRaw(axis) > 0.1) {
            keyboardChargeTime += Time.deltaTime;
            golfStickPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, 179 * (1 - Mathf.Pow(2.71828f, CalculateHitPower(keyboardChargeTime) * golfStickRotationMultiplier))));
        }
        else if (keyboardChargeTime > 0.001)
        {
            swinging = true;
            golfStickPreSwingRotation = golfStickPivot.localRotation;
            keyboardChargeTime = 0;
        }
        if (swinging)
        {
            golfStickPivot.localRotation = Quaternion.Slerp(golfStickPreSwingRotation, Quaternion.identity, swingCurrentRot);
            swingCurrentSpeed += swingAcceleration * Time.deltaTime;
            swingCurrentRot += swingCurrentSpeed * Time.deltaTime;
            if (swingCurrentRot > 1)
            {
                swinging = false;
                swingCurrentRot = 0;
                swingCurrentSpeed = 0;
                ShootBall(
                    CalculateHitPower(keyboardChargeTime) * hitStrength,
                    new Vector2(Mathf.Cos(golfStickRotation * Mathf.Deg2Rad), Mathf.Sin(golfStickRotation * Mathf.Deg2Rad)));
            }
        }
    }
    private void ShootBall(float power, Vector2 direction)
    {
        rb.AddForce(direction * power);
    }
    private float CalculateHitPower(float time)
    {
        return -keyboardChargeSpeed * 
            ((time - keyboardChargeInitial) * Mathf.Atan(time - keyboardChargeInitial) 
            - 0.5f * Mathf.Log(1 + Mathf.Pow(time + keyboardChargeInitial, 2)) 
            + (keyboardChargeSpeed * Mathf.PI * time));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(-Mathf.Cos(golfStickRotation * Mathf.Deg2Rad), Mathf.Sin(golfStickRotation * Mathf.Deg2Rad)) * 100);
    }
}

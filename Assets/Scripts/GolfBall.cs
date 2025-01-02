using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    Vector2 worldMousePos;
    bool playerAiming;
    [SerializeField] float hitStrength = 1f;
    [SerializeField] float speedThreshhold = 1f;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnMouseDown()
    {
        if (rb.velocity.magnitude < speedThreshhold)
        {
            playerAiming = true;
        }
    }
    private void OnMouseDrag()
    {
        if (playerAiming)
        {
            worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if (playerAiming)
        {
            Vector2 hitForce = (Vector2)transform.position - worldMousePos;
            hitForce *= hitStrength;
            rb.AddForce(hitForce);
        }
        playerAiming = false;
    }
    private void OnDrawGizmos()
    {
        if (playerAiming)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(worldMousePos, transform.position);
        }
    }
}

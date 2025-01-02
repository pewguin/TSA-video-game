using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GolfBall : MonoBehaviour
{
    Vector2 worldMousePos;
    bool playerAiming;
    // how powerful the hit from the player is
    [SerializeField] float hitStrength = 1f;
    // how slow ball must be going before player could hit it
    [SerializeField] float speedThreshhold = 1f;
    // should be front, middle, back
    [SerializeField] GameObject[] arrowSegments;
    Vector2[] arrowSizes;
    [SerializeField] float arrowWidth = 1f;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowSizes = new Vector2[arrowSegments.Length];
        for (int i = 0; i < arrowSegments.Length; i++)
        {
            arrowSizes[i] = arrowSegments[i].GetComponent<SpriteRenderer>().bounds.size;
        }
    }
    private void Update()
    {
        if (playerAiming && worldMousePos != null)
        {
            Vector2 ballToMouse = worldMousePos - (Vector2)transform.position;
            Vector2 frontPosition = (Vector2)transform.position + (ballToMouse.normalized * arrowSizes[0].y/2);
            arrowSegments[0].transform.position = frontPosition;
            Vector3 direction = transform.position - arrowSegments[0].transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowSegments[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
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

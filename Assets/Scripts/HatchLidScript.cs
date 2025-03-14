using UnityEngine;
using System.Collections;

public class HatchLidScript : MonoBehaviour
{
    public float startRotationZ = 0f;    // Initial rotation in degrees
    public float endRotationZ = 90f;     // Target rotation in degrees
    public float rotationSpeed = 2f;     // Rotation speed in degrees per second

    private Coroutine rotationCoroutine;
    [ContextMenu("RotateToStart")]
    public void RotateToStart()
    {
        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(RotateZ(startRotationZ));
    }
    [ContextMenu("RotateToEnd")]
    public void RotateToEnd()
    {
        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(RotateZ(endRotationZ));
    }

    private IEnumerator RotateZ(float targetZ)
    {
        float startZ = transform.eulerAngles.z;
        // Get the smallest angle difference (works correctly if crossing 0°/360°)
        float totalAngle = Mathf.DeltaAngle(startZ, targetZ);
        float duration = Mathf.Abs(totalAngle) / rotationSpeed;
        float elapsedTime = 0f;

        // If there's no rotation needed, snap to target and exit
        if (duration <= 0f)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetZ);
            rotationCoroutine = null;
            yield break;
        }

        while (elapsedTime < duration)
        {
            float newZ = Mathf.LerpAngle(startZ, targetZ, elapsedTime / duration);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newZ);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we hit the target exactly
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetZ);
        rotationCoroutine = null;
    }
}

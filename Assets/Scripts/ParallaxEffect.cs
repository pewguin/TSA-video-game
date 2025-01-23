using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform cameraTransform; // Reference to the camera's transform
    public Vector2 parallaxMultiplier; // How much the background moves relative to the camera

    private Vector3 lastCameraPosition;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Move the background based on the camera's movement and the multiplier
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y, 0);

        lastCameraPosition = cameraTransform.position;
    }
}

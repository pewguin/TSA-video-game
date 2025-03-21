using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    [SerializeField] Transform obj;
    [SerializeField] float speed;
    Vector3 bottomLeft;
    Vector3 topRight;
    float z = -10;
    private void Start()
    {
        z = transform.position.z;
        cam = GetComponent<Camera>();
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == "bottomLeft")
            {
                bottomLeft = t.transform.position;
            }
            else if (t.name == "topRight")
            {
                topRight = t.transform.position;
            }
        }
        if (topRight == null || bottomLeft == null)
        {
            Debug.LogError("Give the camera two gameobjects, named bottomLeft and topRight to specify bounds.");
        }
    }
    private void Update()
    {
       
        Vector3 newPos = obj.position;
        if (newPos.y + cam.orthographicSize > topRight.y)
        {
            newPos.y = topRight.y - cam.orthographicSize;
        }
        if (newPos.y - cam.orthographicSize < bottomLeft.y)
        {
            newPos.y = bottomLeft.y + cam.orthographicSize;
        }
        if (newPos.x + (cam.orthographicSize * cam.aspect) > topRight.x)
        {
            newPos.x = topRight.x - (cam.orthographicSize * cam.aspect);
        }
        if (newPos.x - (cam.orthographicSize * cam.aspect) < bottomLeft.x)
        {
            newPos.x = bottomLeft.x + (cam.orthographicSize * cam.aspect);
        }
        newPos.z = z;
        
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * speed);
    }
}
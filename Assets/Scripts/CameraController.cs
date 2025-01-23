using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    [SerializeField] Transform Object1;
    [SerializeField] Transform Object2;
    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] float scaleSpeed = 0.1f;
    [SerializeField] float bufferAroundBalls = 2f;
    [SerializeField] Vector2 minBound;
    [SerializeField] Vector2 maxBound;

    public Vector3 offset;

    private Vector3 setPos;
    private Vector2 setExtends;
    private float minYExtend;
    private float z;
    private void Start()
    {
        z = transform.position.z;
        cam = GetComponent<Camera>();
        minYExtend = cam.orthographicSize;
        setPos = Vector3.forward * z;
    }
    private void Update()
    {
        
        setExtends.x = Mathf.Abs(Object1.position.x - Object2.position.x) + 2 * bufferAroundBalls;
        setExtends.y = Mathf.Abs(Object1.position.y - Object2.position.y) + 2 * bufferAroundBalls;
        setExtends /= 2;
     

        setPos.x = (Object1.position.x + Object2.position.x) / 2;
        float low = Mathf.Min(Object1.position.y, Object2.position.y);
        setPos.y = low + GetHeightExtend(setExtends) - bufferAroundBalls;

        transform.position = Vector3.Lerp(transform.position, setPos, moveSpeed) + offset;
        
        cam.orthographicSize = Mathf.Max(Mathf.Lerp(cam.orthographicSize, GetHeightExtend(setExtends), scaleSpeed), minYExtend);
        
    }

    private Vector2 GetCameraExtends()
    {
        return new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);
    }
    private float GetHeightExtend(Vector2 extends)
    {
        float vert = extends.y;
        float horz = extends.x / cam.aspect;

        return Mathf.Max(vert, horz);
    }
}

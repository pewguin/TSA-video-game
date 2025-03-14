using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubAttack : MonoBehaviour
{
    public float introTime;
    private float fadeInTimer;
    private float startRot;
    public float endRot;
    private float t;
    public float duration;
    private float fadingTimer;
    public SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        startRot = transform.rotation.eulerAngles.z;
        sr.color = new Color(1f, 1f, 1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeInTimer <= 1f)
        {
            sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, fadeInTimer));
            fadeInTimer += Time.deltaTime / introTime;
        }
        if (fadeInTimer > 1f)
        {
            Vector3 newRotation = Vector3.forward * Mathf.SmoothStep(startRot, endRot, t);
            transform.rotation = Quaternion.Euler(newRotation);
            t += Time.deltaTime / duration;
        }
        if (t > 0.9f)
        {
            sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, fadingTimer));
            fadingTimer += Time.deltaTime;
        }
        if (fadingTimer > 1f)
        {
            Destroy(gameObject);
        }
    }
}

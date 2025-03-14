using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner: MonoBehaviour
{
    private Animator doorAnim;
    public Animator transitionAnim;
    public Vector3 targetPos;
    private Rigidbody2D playerRB;
    public bool pulling;
    public Collider2D escapeWall;
    public float pullForce;
    public float pullWaitSeconds;
    public bool acceptPulledBall;

    private void Start()
    {
        doorAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (pulling)
        {
            playerRB.AddForce((targetPos - playerRB.transform.position).normalized * pullForce * Time.deltaTime, ForceMode2D.Force);
            if (acceptPulledBall & (playerRB.transform.position - targetPos).sqrMagnitude < 0.02f)
            {
                playerRB.velocity = Vector2.zero;
                playerRB.transform.position = targetPos;
                StartCoroutine(TransitionNextLevel());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !pulling)
        {
            StartCoroutine(ExitThroughDoor(collision.gameObject));
            playerRB = collision.attachedRigidbody;
        }
    }

    IEnumerator ExitThroughDoor(GameObject player)
    {
        doorAnim.SetTrigger("open");
        yield return new WaitForSeconds(0.4f);
        pulling = true;
        escapeWall.enabled = true;
        yield return new WaitForSeconds(pullWaitSeconds);
        acceptPulledBall = true;
    }

    IEnumerator TransitionNextLevel()
    {
        transitionAnim.SetTrigger("Exit");
        yield return new WaitForSeconds(0.5f); //change to anim length
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

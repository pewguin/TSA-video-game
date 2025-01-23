using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator anim;
    public void PlayGame()
    {
        StartCoroutine(Transition());
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator Transition()
    {
        anim.SetTrigger("Exit");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadSceneAsync(1);
    }
}

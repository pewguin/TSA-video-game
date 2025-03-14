using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    // prefix text with A for alphie or C for caddy
    [SerializeField] private string[] seperatedText;
    [SerializeField] GameObject alphiePfp;
    [SerializeField] GameObject caddyPfp;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float textSpeed = 0.1f;
    public int textId = 0;
    private int midTextId = 0;
    private float timeSinceLetterAdded = 0f;
    void Start()
    {
        OnTextChange();
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) ||
            Input.GetKey(KeyCode.UpArrow) ||
            Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetMouseButtonDown(0))
        {
            if (midTextId < seperatedText[textId].Length - 2)
            {
                midTextId = seperatedText[textId].Length - 2;
                text.text = seperatedText[textId].Substring(1, 1 + midTextId);
            } 
            else if (textId < seperatedText.Length - 1)
            {
                textId++;
                midTextId = 0;
                OnTextChange();
            }
            else if (textId >= seperatedText.Length - 1)
            {
                if (SceneManager.GetActiveScene().buildIndex == 6)
                {
                    Application.Quit();
                } else
                {
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
                }
                
            }
        }

        if (midTextId < seperatedText[textId].Length - 2)
        {
            timeSinceLetterAdded += Time.deltaTime;
            if (timeSinceLetterAdded > textSpeed)
            {
                timeSinceLetterAdded = 0;
                midTextId++;
                text.text = seperatedText[textId].Substring(1, 1 + midTextId);
            }
        }
    }
    void OnTextChange()
    {
        if (seperatedText[textId].StartsWith('A'))
        {
            alphiePfp.SetActive(true);
            caddyPfp.SetActive(false);
        }
        else if (seperatedText[textId].StartsWith('C')) 
        {
            alphiePfp.SetActive(false);
            caddyPfp.SetActive(true);
        }
        text.text = "";
    }
}

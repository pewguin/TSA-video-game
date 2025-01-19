using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField] int dotsNum;
    [SerializeField] GameObject DotsParent;
    [SerializeField] GameObject DotsPrefab;
    [SerializeField] float dotSpacing;
    [SerializeField] float dotMinScale;
    [SerializeField] float dotMaxScale;

    Transform[] dotsList;
    public Vector2 pos;
    float time;

    private void Start()
    {
        Hide();
        prepDots();

    }
    public void Show()
    {
        DotsParent.SetActive(true);
    }
    public void Hide()
    {
        DotsParent.SetActive(false);
    }

    void prepDots()
    {

        dotsList = new Transform[dotsNum];
        DotsPrefab.transform.localScale = Vector3.one * dotMaxScale;

        float scale = dotMaxScale;
        float factor = scale / dotsNum;
        for (int i = 0; i < dotsNum; i++)
        {
            dotsList[i] = Instantiate(DotsPrefab, null).transform;

            dotsList[i].parent = DotsParent.transform;
            if (scale > dotMinScale)
            {
                scale -= factor;

            }

        }

    }
    public void UpdateDots(Vector3 ballPos, Vector2 appForce)

    {
        time = dotSpacing;
        for (int i = 0; i < dotsNum; i++)
        {
            pos.x = (ballPos.x + appForce.x * time);
            pos.y = (ballPos.y + appForce.y * time) - (Physics.gravity.magnitude * time * time) / 2f;
            dotsList[i].position = pos;
            time += dotSpacing;
        }
    }


}






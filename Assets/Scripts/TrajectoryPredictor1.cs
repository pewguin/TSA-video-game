using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using System;
using System.Linq;

public class TrajectoryPredictor1 : MonoBehaviour
{
    [SerializeField] int dotsNum;
    [SerializeField] GameObject DotsParent;
    [SerializeField] GameObject DotsPrefab;
    [SerializeField] float dotSpacing;
    [SerializeField] float dotMinScale;
    [SerializeField] float dotMaxScale;
    [SerializeField] Color trajColor = Color.blue;

    Transform[] dotsList;
    public Vector2 pos;
    float time;
    private List<GameObject> trajectoryDots = new List<GameObject>();
    private void Start()
    {
        Hide();
        prepDots();

    }
    public void Show()
    {
        DotsParent.SetActive(true);
        trajectoryDots.ForEach(obj => obj.SetActive(true));
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
            trajectoryDots.Add(Instantiate(DotsPrefab, null));
            SpriteRenderer render = trajectoryDots[i].GetComponent<SpriteRenderer>();
            render.color = trajColor;
            dotsList[i] = trajectoryDots[i].transform;
            dotsList[i].parent = DotsParent.transform;
            trajectoryDots[i].transform.localScale *= scale;
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
            Vector2 direction = new Vector2 ((ballPos.x + appForce.x * (time + dotSpacing)) - pos.x, ((ballPos.y + appForce.y * time) - (Physics.gravity.magnitude * time * time) / 2f) - pos.y);
            RaycastHit2D hit = Physics2D.Raycast(pos, direction);
            Transform val = DotsParent.transform;
            
            if (hit.distance < dotSpacing)
            {
                for (int j = i; j < dotsNum; j++)
                {
                    trajectoryDots[j].SetActive(false);
                }
                break;
            }
            
            

            time += dotSpacing;
        }
    }


}






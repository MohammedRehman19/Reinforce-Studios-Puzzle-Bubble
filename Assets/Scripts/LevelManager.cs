﻿using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

   /* #region Singleton
    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion*/

    public Grid grid;
    public Transform bubblesArea;
    public List<GameObject> bubblesPrefabs;
    public List<GameObject> bubblesInScene;
    public List<string> colorsInScene;

    public float offset = 1f;
    public GameObject leftLine;
    public GameObject rightLine;
    private bool lastLineIsLeft = true;
    public string playerTag;
    public GameManager Manager;
    private void Start()
    {
        grid = GetComponent<Grid>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddNewLine();
        }
    }

    public void GenerateLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            FillWithBubbles(GameObject.FindGameObjectWithTag(playerTag), bubblesPrefabs);
        }
            SnapChildrensToGrid(bubblesArea);
            UpdateListOfBubblesInScene();
            //playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();
            //foreach (playercontroller p in pc)
            //{
            //    if (p.GetComponent<PhotonView>().IsMine)
            //    {
            //        p.callupdatesnape();
            //    }
            //}

        
        
    }

    #region Snap to Grid
    public void SnapChildrensToGrid(Transform parent)
    {
        foreach (Transform t in parent)
        {
            SnapToNearestGripPosition(t);
        }
    }

    public void SnapToNearestGripPosition(Transform t)
    {
        Vector3Int cellPosition = grid.WorldToCell(t.position);
        t.position = grid.GetCellCenterWorld(cellPosition);
    }
    #endregion

    #region Add new line
    [ContextMenu("AddLine")]
    public void AddNewLine()
    {
        OffsetGrid();
        OffsetBubblesInScene();
        GameObject newLine = lastLineIsLeft == true ? Instantiate(rightLine) : Instantiate(leftLine);
        FillWithBubbles(newLine, bubblesInScene);
        SnapChildrensToGrid(bubblesArea);
        lastLineIsLeft = !lastLineIsLeft;
    }

    private void OffsetGrid()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y - offset);
    }

    private void OffsetBubblesInScene()
    {
        foreach (Transform t in bubblesArea)
        {
            t.transform.position = new Vector2(t.position.x, t.position.y - offset);
        }
    }
    #endregion

    private void FillWithBubbles(GameObject go, List<GameObject> bubbles)
    {
        GameManager[] GM = GameObject.FindObjectsOfType<GameManager>();
        GameObject goClone = null;
        Transform bubbleAreaClone = null;
        int counter = 0;
        foreach (GameManager G in GM)
        {

            if (!G._ismine)
            {
                 goClone = GameObject.FindGameObjectWithTag(G.LM.playerTag);
                 bubbleAreaClone = G.LM.bubblesArea;
            }

        }
      
        foreach (Transform t in go.transform)
        {
            var bubble = PhotonNetwork.Instantiate(bubbles[(int)(Random.Range(0, bubbles.Count * 1000000f) / 1000000f)].name, bubblesArea.position,Quaternion.identity,0);
            bubble.transform.SetParent(bubblesArea);
            bubble.transform.position = t.position;
            bubble.GetComponent<Bubble>().Lm = bubblesArea.GetComponent<BubbleHandler>().Lm;
            bubble.GetComponent<Bubble>().Gm = bubblesArea.GetComponent<BubbleHandler>().GM;
            bubble.GetComponent<Bubble>()._isGameoverLineChecker = true;
            playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();

            if (Manager._ismine)
            {
                foreach (playercontroller p in pc)
                {
                    if (p.GetComponent<PhotonView>().IsMine && goClone != null)
                    {
                        p.callCreatebubble(bubbleAreaClone.name, bubble.GetComponent<PhotonView>().ViewID, goClone.transform.GetChild(counter).position.x, goClone.transform.GetChild(counter).position.y, goClone.transform.GetChild(counter).position.z);
                    }
                }
            }
            if (!Manager._ismine)
            {
                foreach (playercontroller p in pc)
                {
                    if (p.GetComponent<PhotonView>().IsMine && goClone != null)
                    {
                        p.callCreatebubble(bubblesArea.name, bubble.GetComponent<PhotonView>().ViewID, t.position.x, t.position.y, t.position.z);
                    }
                }
            }
            counter += 1;
        }

        Destroy(go);
    }

    public void UpdateListOfBubblesInScene()
    {
        List<string> colors = new List<string>();
        List<GameObject> newListOfBubbles = new List<GameObject>();

        foreach (Transform t in bubblesArea)
        {
            Bubble bubbleScript = t.GetComponent<Bubble>();
            if (colors.Count < bubblesPrefabs.Count && !colors.Contains(bubbleScript.bubbleColor.ToString()))
            {
                string color = bubbleScript.bubbleColor.ToString();
                colors.Add(color);

                foreach (GameObject prefab in bubblesPrefabs)
                {
                    if (color.Equals(prefab.GetComponent<Bubble>().bubbleColor.ToString()))
                    {
                        newListOfBubbles.Add(prefab);
                    }
                }
            }
        }

        colorsInScene = colors;
        bubblesInScene = newListOfBubbles;
    }

    public void SetAsBubbleAreaChild(Transform bubble)
    {
        SnapToNearestGripPosition(bubble);
        bubble.SetParent(bubblesArea);
       
    }
}

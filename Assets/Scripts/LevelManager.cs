using System.Collections;
using System.Collections.Generic;
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
           StartCoroutine(FillWithBubbles(GameObject.FindGameObjectWithTag(playerTag), bubblesPrefabs));
           // StartCoroutine(FillWithBubblesNewLine(GameObject.FindGameObjectWithTag(playerTag), bubblesPrefabs));
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
        if (PhotonNetwork.IsMasterClient)
        {
            allmastercallinonce();
        }
        else
        {
            
            playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();
            playercontroller pv = null;
            foreach (playercontroller p in pc)
            {
                if (p.GetComponent<PhotonView>().IsMine)
                {
                    pv = p;
                    pv.callonMasternewLine();
                }
            }
        }
    }

    public void allmastercallinonce()
    {

        MasterCallBeforeNewLine();
        MasterCallNewLine();
        MasterCallAfterNewLine();

        playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();
        playercontroller pv = null;
        foreach (playercontroller p in pc)
        {
            if (p.GetComponent<PhotonView>().IsMine)
            {
                pv = p;
                pv.callonMasternewLine();
            }
        }
    }

  public void MasterCallNewLine()
    {
        GameObject newLine = lastLineIsLeft == true ? Instantiate(rightLine) : Instantiate(leftLine);
        StartCoroutine(FillWithBubbles(newLine, bubblesPrefabs));
        StartCoroutine(FillWithBubblesNewLine(newLine, bubblesPrefabs));
    }
  public void MasterCallBeforeNewLine()
    {
        OffsetGrid();
        OffsetBubblesInScene();

        
    }

    public void MasterCallAfterNewLine()
    {
        
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


    private IEnumerator FillWithBubblesNewLine(GameObject go, List<GameObject> bubbles)
    {
        int counter = 0;
        playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();
        playercontroller pv = null;
        foreach (playercontroller p in pc)
        {
            if (p.GetComponent<PhotonView>().IsMine)
            {
                pv = p;
            }
        }
        while (counter > bubbles.Count)
        {
            var bubble = Instantiate(bubbles[(int)(Random.Range(0, bubbles.Count * 1000000f) / 1000000f)]);
            print(bubble.gameObject.name);
            bubble.transform.SetParent(bubblesArea);
            bubble.transform.localPosition = bubblesArea.transform.GetChild(counter).localPosition;
            print(bubble.gameObject.name +" = "+bubble.transform.localPosition);
            bubble.GetComponent<Bubble>().Lm = bubblesArea.GetComponent<BubbleHandler>().Lm;
            bubble.GetComponent<Bubble>().Gm = bubblesArea.GetComponent<BubbleHandler>().GM;
            bubble.GetComponent<Bubble>()._isGameoverLineChecker = true;
            pv.callCreatebubble(bubble.GetComponent<Bubble>().bubbleColor.ToString(), bubblesArea.name, bubblesArea.transform.GetChild(counter).position.x, bubblesArea.transform.GetChild(counter).position.y, bubblesArea.transform.GetChild(counter).position.z);
            counter += 1;
            yield return new WaitForSeconds(0.1f);
        }

        //  Destroy(go);
    }

    private IEnumerator FillWithBubbles(GameObject go, List<GameObject> bubbles)
    {
        int counter = 0;
        playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();
        playercontroller pv = null;
        foreach (playercontroller p in pc)
        {
            if (p.GetComponent<PhotonView>().IsMine)
            {
                pv = p;
           }
        }
        while (go.transform.childCount > counter)
        {
            var bubble = Instantiate(bubbles[(int)(Random.Range(0, bubbles.Count * 1000000f) / 1000000f)]);
            print(bubble.gameObject.name);
            bubble.transform.SetParent(bubblesArea);
            bubble.transform.localPosition = go.transform.GetChild(counter).localPosition;
            
            bubble.GetComponent<Bubble>().Lm = bubblesArea.GetComponent<BubbleHandler>().Lm;
            bubble.GetComponent<Bubble>().Gm = bubblesArea.GetComponent<BubbleHandler>().GM;
            bubble.GetComponent<Bubble>()._isGameoverLineChecker = true;
            pv.callCreatebubble(bubble.GetComponent<Bubble>().bubbleColor.ToString(),bubblesArea.name, go.transform.GetChild(counter).localPosition.x, go.transform.GetChild(counter).localPosition.y, go.transform.GetChild(counter).localPosition.z);
            counter += 1;
            yield return new WaitForSeconds(0.1f);
        }

      //  Destroy(go);
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

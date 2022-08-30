using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class InGameManager : MonoBehaviourPunCallbacks
{
  /*  #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion*/

    public Shooter shootScript;
    public Transform pointerToLastLine;

    private int sequenceSize = 3;
    [SerializeField]
    public List<Transform> bubbleSequence;
    public LevelManager LM;
    public float counter = 10;
    public TextMeshProUGUI countertxt,scoretxt;
    public PhotonView pv;
    public bool _ismine;
    public bool _isgamestarted = false;
    public TextMeshProUGUI WaitingTxt;
    void Start()
    {
       
       
            
        
    }

    void Update()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            WaitingTxt.text = "Please wait, Loading Game now.";
            Invoke("startGame",2);
        }
        else
        {
            WaitingTxt.text = "Please wait, Looking for another player to the game.";
            return;
        }

        if (pv == null)
        {
            playercontroller[] Listp = GameObject.FindObjectsOfType<playercontroller>();
            foreach (playercontroller p in Listp)
            {
                if (p.GetComponent<PhotonView>().IsMine == _ismine && PhotonNetwork.IsMasterClient && p.OurShooter != null && _isgamestarted)
                {
                    pv = p.GetComponent<PhotonView>();
                 //   print(this.gameObject.name);
                    bubbleSequence = new List<Transform>();

                    LM.GenerateLevel();



                    shootScript.canShoot = true;
                    p.callcreateshoot(_ismine.ToString());
                     shootScript.CreateNextBubble();
                   
                        //a
                    
                }
               

            }

            return;
        }
        if (!pv.IsMine)
            return;

       

     
    }
     public void startGame()
    {
        if (_isgamestarted)
            return;

        _isgamestarted = true;
    }
    private void FixedUpdate()
    {
      /*  counter -= Time.deltaTime;
        if (counter > 4 && counter < 5)
        {
            countertxt.enabled = true;
            countertxt.text = "Hurry Yup !";
        }
        else if (counter < 4 && counter > 0)
        {
            countertxt.text = Mathf.RoundToInt(counter).ToString();
        }
        else if (counter <= 0)
        {
            shootScript.canShoot = false;
            shootScript.Shoot();
            counter = 10;
            countertxt.enabled = false;
            playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();
            foreach (playercontroller p in pc)
            {
                if (p.GetComponent<PhotonView>().IsMine)
                {
                    p.callshoot();
                }
            }
        }*/
    }
    public void ProcessTurn(Transform currentBubble)
    {
        bubbleSequence.Clear();
        CheckBubbleSequence(currentBubble);

        if(bubbleSequence.Count >= sequenceSize)
        {
            DestroyBubblesInSequence();
            DropDisconectedBubbles();
        }

       LM.UpdateListOfBubblesInScene();
        if (PhotonNetwork.IsMasterClient)
        {
            callfromMaster();
        }
        else
        {
            playercontroller[] Listp = GameObject.FindObjectsOfType<playercontroller>();
            foreach (playercontroller p in Listp)
            {
                p.callonMaster(_ismine.ToString());
            }
        }

    }

    public void callfromMaster()
    {
        shootScript.CreateNextBubble();
        shootScript.canShoot = true;
        playercontroller[] Listp = GameObject.FindObjectsOfType<playercontroller>();
        foreach (playercontroller p in Listp)
        {
            p.callcreateshoot(_ismine.ToString());
        }
    }

    private void CheckBubbleSequence(Transform currentBubble)
    {
        bubbleSequence.Add(currentBubble);

        Bubble bubbleScript = currentBubble.GetComponent<Bubble>();
        List<Transform> neighbors = bubbleScript.GetNeighbors();

        foreach(Transform t in neighbors)
        {
            if (!bubbleSequence.Contains(t))
            {

                Bubble bScript = t.GetComponent<Bubble>();

                if (bScript.bubbleColor == bubbleScript.bubbleColor)
                {
                    CheckBubbleSequence(t);
                }
            }
        }
    }

    private void DestroyBubblesInSequence()
    {
        foreach(Transform t in bubbleSequence)
        {
            Destroy(t.gameObject);
            playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();
            foreach (playercontroller p in pc)
            {
                if (p.GetComponent<PhotonView>().IsMine)
                {
                    print("calling");
                    p.callAddscore(_ismine.ToString());
                }
            }
        }
    }

    private void DropDisconectedBubbles()
    {
        SetAllBubblesConnectionToFalse();
        SetConnectedBubblesToTrue();
        SetGravityToDisconectedBubbles();
    }

    #region Drop Disconected Bubbles
    private void SetAllBubblesConnectionToFalse()
    {
        foreach (Transform bubble in LM.bubblesArea)
        {
            bubble.GetComponent<Bubble>().isConnected = false;
        }
    }

    private void SetConnectedBubblesToTrue()
    {
        bubbleSequence.Clear();

        RaycastHit2D[] hits = Physics2D.RaycastAll(pointerToLastLine.position, pointerToLastLine.right, 15f);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.tag.Equals("Bubble"))
                SetNeighboursConnectionToTrue(hits[i].transform);
        }
    }

    private void SetNeighboursConnectionToTrue(Transform bubble)
    {
        Bubble bubbleScript = bubble.GetComponent<Bubble>();
        bubbleScript.isConnected = true;
        bubbleSequence.Add(bubble);

        foreach(Transform t in bubbleScript.GetNeighbors())
        {
            if(!bubbleSequence.Contains(t))
            {
                SetNeighboursConnectionToTrue(t);
            }
        }
    }

    private void SetGravityToDisconectedBubbles()
    {
        foreach (Transform bubble in LM.bubblesArea)
        {
            if (!bubble.GetComponent<Bubble>().isConnected)
            {
                bubble.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                if(!bubble.GetComponent<Rigidbody2D>())
                {
                    Rigidbody2D rb2d = bubble.gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
                }       
            }
        }
    }
    #endregion
}
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooter : MonoBehaviourPunCallbacks
{
    public Transform gunSprite;
    public bool canShoot;
    public float speed = 6f;

    public Transform nextBubblePosition;
    public GameObject currentBubble;
    public GameObject nextBubble;

    public Vector2 lookDirection;
    public float lookAngle;
    public bool isSwaping = false;
    public float time = 0.02f;
    public LevelManager Lm;
    public InGameManager Gm;
    public PhotonView pv;
    public bool _ismine;
    public void Update()
    {

        if (pv == null)
            return;


        if (!pv.IsMine)
            return;



        if (isSwaping)
        {
            if (Vector2.Distance(currentBubble.transform.position, nextBubblePosition.position) <= 0.2f
                && Vector2.Distance(nextBubble.transform.position, transform.position) <= 0.2f)
            {
                nextBubble.transform.position = transform.position;
                currentBubble.transform.position = nextBubblePosition.position;

                currentBubble.GetComponent<Collider2D>().enabled = true;
                nextBubble.GetComponent<Collider2D>().enabled = true;

                isSwaping = false;

                GameObject reference = currentBubble;
                currentBubble = nextBubble;
                nextBubble = reference;
            }

            nextBubble.transform.position = Vector2.Lerp(nextBubble.transform.position, transform.position, time);
            currentBubble.transform.position = Vector2.Lerp(currentBubble.transform.position, nextBubblePosition.position, time);
        }
    }

    public void Shoot()
    {

        currentBubble.transform.rotation = transform.rotation;
        currentBubble.GetComponent<Rigidbody2D>().AddForce(currentBubble.transform.up * speed);
        currentBubble = null;
    }

    [ContextMenu("SwapBubbles")]
    public void SwapBubbles()
    {
        currentBubble.GetComponent<Collider2D>().enabled = false;
        nextBubble.GetComponent<Collider2D>().enabled = false;
        isSwaping = true;
    }

    [ContextMenu("CreateNextBubble")]
    public void CreateNextBubble()
    {
        List<GameObject> bubblesInScene = Lm.bubblesInScene;
        List<string> colors = Lm.colorsInScene;

        if (nextBubble == null)
        {
            nextBubble = InstantiateNewBubble(Lm.bubblesPrefabs);
        }
        else
        {
            if (!colors.Contains(nextBubble.GetComponent<Bubble>().bubbleColor.ToString()))
            {
                Destroy(nextBubble);
                nextBubble = InstantiateNewBubble(Lm.bubblesPrefabs);
            }
        }

        if (currentBubble == null)
        {
            currentBubble = nextBubble;
            currentBubble.transform.position = new Vector2(transform.position.x, transform.position.y);
            nextBubble = InstantiateNewBubble(Lm.bubblesPrefabs);
        }
    }
    public void CreateNextBubbleClo(string name)
    {
        List<GameObject> bubblesInScene = Lm.bubblesInScene;
        List<string> colors = Lm.colorsInScene;

        if (nextBubble == null)
        {
            nextBubble = InstantiateNewBubbleClo(name);
        }
        else
        {
            if (!colors.Contains(nextBubble.GetComponent<Bubble>().bubbleColor.ToString()))
            {
                Destroy(nextBubble);
                nextBubble = InstantiateNewBubbleClo(name);
            }
        }

        if (currentBubble == null)
        {
            currentBubble = nextBubble;
            currentBubble.transform.position = new Vector2(transform.position.x, transform.position.y);
            nextBubble = InstantiateNewBubbleClo(name);
        }
    }
    private GameObject InstantiateNewBubble(List<GameObject> bubblesInScene)
    {
        GameObject newBubble = Instantiate(bubblesInScene[(int)(Random.Range(0, bubblesInScene.Count * 1000000f) / 1000000f)]);
        newBubble.transform.position = new Vector2(nextBubblePosition.position.x, nextBubblePosition.position.y);
        newBubble.GetComponent<Bubble>().isFixed = false;
        Rigidbody2D rb2d = newBubble.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;
        newBubble.GetComponent<Bubble>().Lm = Lm;
        newBubble.GetComponent<Bubble>().Gm = Gm;
        playercontroller[] pc = GameObject.FindObjectsOfType<playercontroller>();
        foreach(playercontroller p in pc)
        {
            if (p.GetComponent<PhotonView>().IsMine)
            {
//                print(_ismine);
                p.callnewbubble(_ismine.ToString(),newBubble.GetComponent<Bubble>().bubbleColor.ToString());
            }
        }
        return newBubble;
    }

    private GameObject InstantiateNewBubbleClo(string Shoootername)
    {

        GameObject bubbleclo = null;
        foreach (var bub in Lm.bubblesPrefabs)
        {

            if (Shoootername.ToLower() == bub.gameObject.name.ToLower())
            {
                bubbleclo = bub;
            }
        }

                GameObject newBubble = Instantiate(bubbleclo);
        newBubble.transform.position = new Vector2(nextBubblePosition.position.x, nextBubblePosition.position.y);
        newBubble.GetComponent<Bubble>().isFixed = false;
        Rigidbody2D rb2d = newBubble.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;
        newBubble.GetComponent<Bubble>().Lm = Lm;
        newBubble.GetComponent<Bubble>().Gm = Gm;
        return newBubble;
    }
}

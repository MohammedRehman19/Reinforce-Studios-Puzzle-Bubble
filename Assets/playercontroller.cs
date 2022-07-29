using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class playercontroller : MonoBehaviourPunCallbacks
{
    public Shooter[] Shooters;
    public Shooter OurShooter;
    public string Vid;
    // Start is called before the first frame update
    void Start()
    {
      Vid = photonView.Controller.ToString()[2].ToString();
        Shooters = GameObject.FindObjectsOfType<Shooter>();
        foreach(Shooter S in Shooters)
        {
            if (S._ismine && Vid == "1")
            {
                OurShooter = S;
                OurShooter.pv = this.photonView;
            }
            else if (!S._ismine && Vid == "2")
            {
                OurShooter = S;
                OurShooter.pv = this.photonView;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && Vid.Length > 0)
        {
            OurShooter.lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            OurShooter.lookAngle = Mathf.Atan2(OurShooter.lookDirection.y, OurShooter.lookDirection.x) * Mathf.Rad2Deg;
            OurShooter.gunSprite.rotation = Quaternion.Euler(0f, 0f, OurShooter.lookAngle - 90f);
            OurShooter.transform.rotation = Quaternion.Euler(0f, 0f, OurShooter.lookAngle - 90f);
            photonView.RPC("move", RpcTarget.Others,Vid, OurShooter.lookAngle);
            if (OurShooter.canShoot
           && Input.GetMouseButtonUp(0)
           && (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > OurShooter.transform.position.y))
            {
                OurShooter.canShoot = false;
                OurShooter.Shoot();
                photonView.RPC("shoot", RpcTarget.Others,Vid);
                // counter = 10;
                //  countertxt.enabled = false;
            }
        }

    }
    public void callCreatebubble(string Area,int id,float xs, float ys, float zs)
    {
        photonView.RPC("createbubble", RpcTarget.Others, Area,id,xs,ys,zs);
    }
    public void callupdatesnape()
    {
        photonView.RPC("updatesnape", RpcTarget.All);
    }
    public void callcreateshoot()
    {
        photonView.RPC("createshoot", RpcTarget.Others);
    }
    [PunRPC]
    void move(string Vid,float r)
    {
        Shooter tempshooter = null;
        bool _isFirst = false;
        if (Vid == "1")
        {
            _isFirst = true;
        }
        else if(Vid == "2")
        {
            _isFirst = false;
        }
        foreach (Shooter S in Shooters)
        {
            if (S._ismine == _isFirst)
            {
                tempshooter = S;
            }
        }

        print("move");
        tempshooter.gunSprite.rotation = Quaternion.Euler(0f, 0f, r-90);
        tempshooter.transform.rotation = Quaternion.Euler(0f, 0f, r- 90f);
             
    }
    [PunRPC]
    void shoot(string Vid)
    {
        Shooter tempshooter = null;
        bool _isFirst = false;
        if (Vid == "1")
        {
            _isFirst = true;
        }
        else if (Vid == "2")
        {
            _isFirst = false;
        }
        foreach (Shooter S in Shooters)
        {
            if (S._ismine == _isFirst)
            {
                tempshooter = S;
            }
        }

        print("shoot");
        tempshooter.canShoot = false;
        tempshooter.Shoot();
        
    }

    [PunRPC]
    void createbubble(string Area,int bubblenum, float xs, float ys, float zs)
    {
      Transform bubblesArea =  GameObject.Find(Area).transform;
        PhotonView [] ps = GameObject.FindObjectsOfType<PhotonView>();
        foreach (PhotonView p in ps)
        {
            if (p.ViewID == bubblenum && p.GetComponent<Bubble>() != null)
            {
                p.transform.SetParent(bubblesArea);
                p.transform.position = new Vector3(xs,ys,zs);
                p.GetComponent<Bubble>().Lm = bubblesArea.GetComponent<BubbleHandler>().Lm;
                p.GetComponent<Bubble>().Gm = bubblesArea.GetComponent<BubbleHandler>().GM;
                p.GetComponent<Bubble>()._isGameoverLineChecker = true;
            }
        }
    }

    [PunRPC]
    void updatesnape()
    {
        
        foreach (Shooter S in Shooters)
        {
            
                S.Lm.SnapChildrensToGrid(S.Lm.bubblesArea);
                S.Lm.UpdateListOfBubblesInScene();
            
        }
    }

    [PunRPC]
    void createshoot()
    {
        foreach (Shooter S in Shooters)
        {
            
                S.canShoot = true;
                S.CreateNextBubble();
            
        
        }
      
    }
    public void callbubbleSequence()
    {
        photonView.RPC("bubbleSequence", RpcTarget.Others);
    }
    [PunRPC]
    void bubbleSequence()
    {
        foreach (Shooter S in Shooters)
        {
            
//                S.Gm.bubbleSequence = new List<Transform>();
            

        }
    }
    public void callnewbubble(int viewid)
    {
        photonView.RPC("newbubble", RpcTarget.Others,viewid);
    }
    [PunRPC]
    void newbubble(int viewid)
    {
       
                foreach (Shooter S in Shooters)
                {
                   if(S._ismine == false)
            {

                PhotonView[] pv = GameObject.FindObjectsOfType<PhotonView>();
                foreach (PhotonView pp in pv)
                {

                    if(pp.ViewID == viewid)
                    {
                        pp.transform.position = new Vector2(S.nextBubblePosition.position.x, S.nextBubblePosition.position.y);
                        pp.GetComponent<Bubble>().isFixed = false;
                        Rigidbody2D rb2d = pp.gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
                        rb2d.gravityScale = 0f;
                        pp.GetComponent<Bubble>().Lm = S.Lm;
                        pp.GetComponent<Bubble>().Gm = S.Gm;
                    }

                }

                 
            }
                }
               
            
    }
}

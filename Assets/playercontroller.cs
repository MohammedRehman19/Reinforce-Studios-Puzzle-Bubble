using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class playercontroller : MonoBehaviourPunCallbacks
{
    public Shooter[] Shooters;
    public Shooter OurShooter;
    public int Vid;
    Quaternion to;
    float speed = 0.01f;
    float timeCount = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Vid = int.Parse(photonView.Controller.ToString()[2].ToString());
        Shooters = GameObject.FindObjectsOfType<Shooter>();
        foreach (Shooter S in Shooters)
        {
            if (S._ismine && Vid == 1)
            {
                OurShooter = S;
                OurShooter.pv = this.photonView;
            }
            else if (!S._ismine && Vid > 1)
            {
                OurShooter = S;
                OurShooter.pv = this.photonView;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       

      

    }
    private void Update()
    {

        if (photonView.IsMine && OurShooter.Gm._isgamestarted)
        {
            OurShooter.lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - OurShooter.transform.position;
            OurShooter.lookAngle = Mathf.Atan2(OurShooter.lookDirection.y, OurShooter.lookDirection.x) * Mathf.Rad2Deg;
            to = Quaternion.Euler(0f, 0f, OurShooter.lookAngle - 90f);
            /*    OurShooter.transform.rotation = Quaternion.Euler(0f, 0f, OurShooter.lookAngle - 90f);*/
            photonView.RPC("move", RpcTarget.Others, Vid, OurShooter.lookAngle);
            if (OurShooter.canShoot
           && Input.GetMouseButtonUp(0)
           && (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > OurShooter.transform.position.y))
            {
                OurShooter.canShoot = false;
                OurShooter.Shoot();
                photonView.RPC("shoot", RpcTarget.Others, Vid);
                OurShooter.Gm.counter = 10;
                OurShooter.Gm.countertxt.enabled = false;
            }
        }


        Quaternion from = new Quaternion(OurShooter.transform.rotation.x, OurShooter.transform.rotation.y, OurShooter.transform.rotation.z, OurShooter.transform.rotation.w);
        OurShooter.transform.rotation = Quaternion.Lerp(from,to, timeCount * speed);
        OurShooter.gunSprite.rotation = Quaternion.Lerp(from, to, timeCount * speed);
        timeCount = timeCount + Time.deltaTime;
    }
    public void callshoot(int vidd)
    {
        photonView.RPC("shoot", RpcTarget.Others, vidd);
    }
    public void callCreatebubble(string Area,string BubbleArea,float xs, float ys, float zs)
    {
        photonView.RPC("createbubble", RpcTarget.Others, Area,BubbleArea,xs,ys,zs);
    }
    public void callupdatesnape()
    {
        photonView.RPC("updatesnape", RpcTarget.All);
    }
    public void callcreateshoot(string _isMine)
    {
        photonView.RPC("createshoot", RpcTarget.Others, _isMine);
    }
    [PunRPC]
    void move(int Vid,float r)
    {
        Shooter tempshooter = null;
        bool _isFirst = false;
        if (Vid == 1)
        {
            _isFirst = true;
        }
        else if(Vid > 1)
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
        if (tempshooter != null)
        {
         //   print("move");
           to = Quaternion.Euler(0f, 0f, r - 90);
           
        }
    }
    [PunRPC]
    void shoot(int Vid)
    {
        Shooter tempshooter = null;
        bool _isFirst = false;
        if (Vid == 1)
        {
            _isFirst = true;
        }
        else if (Vid > 1)
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
        if (tempshooter != null)
        {
          //  print("shoot");
            tempshooter.canShoot = false;
            tempshooter.Shoot();
            OurShooter.Gm.counter = 10;
            OurShooter.Gm.countertxt.enabled = false;
        }
        
    }

    [PunRPC]
    void createbubble(string Area,string BubbleArea,float xs, float ys, float zs)
    {
      
        GameObject bubble = null;
        GameObject bubbleArea = GameObject.Find(BubbleArea);
       foreach( var bub in OurShooter.Lm.bubblesPrefabs)
        {
            
            if (Area.ToLower() == bub.gameObject.name.ToLower())
            {
               
                bubble = Instantiate(bub);
                bubble.transform.SetParent(bubbleArea.transform);
                bubble.transform.position = new Vector3(xs, ys, zs);
                bubble.GetComponent<Bubble>().Lm = bubbleArea.GetComponent<BubbleHandler>().Lm;
                bubble.GetComponent<Bubble>().Gm = bubbleArea.GetComponent<BubbleHandler>().GM;
                bubble.GetComponent<Bubble>()._isGameoverLineChecker = true;
                return;
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
    void createshoot(string _ismine)
    {
        bool tempmine = false;
        if(_ismine.ToLower() == "true")
        {
            tempmine = true;
        }
        else
        {
            tempmine = false;
        }

        Shooters = GameObject.FindObjectsOfType<Shooter>();
        foreach (Shooter S in Shooters)
        {
            if (S._ismine == tempmine)
            {
                S.Gm.bubbleSequence = new List<Transform>();
                S.canShoot = true;
            }
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
    public void callnewbubble(string _ismine,string name)
    {
        photonView.RPC("newbubble", RpcTarget.Others, _ismine,name);
    }
    [PunRPC]
    void newbubble(string _ismine,string name)
    {
        bool tempmine = false;
        if (_ismine.ToLower() == "true")
        {
            tempmine = true;
        }
        else
        {
            tempmine = false;
        }
        print(tempmine +" = aaa");
        Shooters = GameObject.FindObjectsOfType<Shooter>();
        foreach (Shooter S in Shooters)
        {
            if (S._ismine == tempmine)
            {
                S.CreateNextBubbleClo(name);
            }
        }
    }
}

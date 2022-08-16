using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleHandler : MonoBehaviour
{

    public LevelManager Lm;
    public InGameManager GM;
    public float min = 2f;
    public float max = 3f;
    public bool _movement = false;
    Vector3 Startingpoint;
    // Start is called before the first frame update
    void Start()
    {
        min = transform.position.x-0.1f;
        max = transform.position.x + 0.1f;
        Startingpoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_movement)
        {
            transform.position = new Vector3(Mathf.PingPong(Time.time * 2, max - min) + min, transform.position.y, transform.position.z);
        }
    }


    public void addnewLine()
    {
        Lm.AddNewLine();
    }
}

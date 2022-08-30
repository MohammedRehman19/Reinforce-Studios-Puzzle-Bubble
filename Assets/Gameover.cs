using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameover : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelManager Lm;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bubble>() == null)
            return;

        //  print(collision.GetComponent<Bubble>()._isGameoverLineChecker);
        if (collision.GetComponent<Bubble>()._isGameoverLineChecker)
        {
            print("gameover");
            GenerateGameover();
        }
        else
        {
            collision.GetComponent<Bubble>()._isGameoverLineChecker = true;
        }
    }

    public void GenerateGameover()
    {
        foreach (Transform t in Lm.bubblesArea)
        {
            t.transform.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);
        }
        Invoke("RestartGame",4);
    }

    public void RestartGame()
    {
        Application.LoadLevel(0);
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float raycastRange = 0.7f;
    public float raycastOffset = 0.51f;

    public bool isFixed;
    public bool isConnected;

    public BubbleColor bubbleColor;
    public LevelManager Lm;
    public InGameManager Gm;
    public bool _isGameoverLineChecker = false;

    [SerializeField]
    [Tooltip("Just for debugging, adds some velocity during OnEnable")]
    private Vector3 initialVelocity;

    [SerializeField]
    private float minVelocity = 10f;

    private Vector3 lastFrameVelocity;
    private Rigidbody rb;

    private void OnEnable()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = initialVelocity;
        }
    }

    private void Update()
    {
        if (rb != null)
        {
            lastFrameVelocity = rb.velocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Bubble" && collision.gameObject.GetComponent<Bubble>().isFixed)
        {

            if (!isFixed)
            {
                HasCollided();
            }
        }

        if (collision.gameObject.tag == "Limit")
        {

            if (!isFixed)
            {
                HasCollided();
            }
        }


        Bounce(collision.contacts[0].normal);
        /*
                if (collision.gameObject.tag == "walls")
                {
                    print("walls");
                    GetComponent<Rigidbody2D>().AddForce(-collision.contacts[0].normal +
                        new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)));
                }*/
    }

   
    private void Bounce(Vector3 collisionNormal)
    {
        var speed = lastFrameVelocity.magnitude;
        var direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);

        Debug.Log("Out Direction: " + direction);
        rb.velocity = direction * Mathf.Max(speed, minVelocity);
    }

    private void HasCollided()
    {
        var rb = GetComponent<Rigidbody2D>();
        Destroy(rb);
        isFixed = true;
        Lm.SetAsBubbleAreaChild(transform);
        Gm.ProcessTurn(transform);
       
    }

   

    public List<Transform> GetNeighbors()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        List<Transform> neighbors = new List<Transform>();

        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x - raycastOffset, transform.position.y), Vector3.left, raycastRange));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x + raycastOffset, transform.position.y), Vector3.right, raycastRange));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x - raycastOffset, transform.position.y + raycastOffset), new Vector2(-1f, 1f), raycastRange));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x - raycastOffset, transform.position.y - raycastOffset), new Vector2(-1f, -1f), raycastRange));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x + raycastOffset, transform.position.y + raycastOffset), new Vector2(1f, 1f), raycastRange));
        hits.Add(Physics2D.Raycast(new Vector2(transform.position.x + raycastOffset, transform.position.y - raycastOffset), new Vector2(1f, -1f), raycastRange));

        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider != null && hit.transform.tag.Equals("Bubble"))
            {
                neighbors.Add(hit.transform);
            }
        }

        return neighbors;
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
    }

    public enum BubbleColor
    {
        BLUE, YELLOW, RED, GREEN
    }
}

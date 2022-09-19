using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyAI : MonoBehaviour
{
    private Orientation orientation;

    [SerializeField]
    private Transform playerTransform;

    private float moveSpeed = 2.0f; // units per second
    private float stopDistance = 1.4f; // units away the enemy stops to attack the player

    private Vector2 position;

    public bool isStopped;

    public Vector3 Position
    {
        get{return position;}
        set{position = value;}
    }

    public Transform PlayerTransform
    {
        set { playerTransform = value; }
    }

    public GameObject punchBox;
    public GameObject kickBox;

    [SerializeField]
    private int health = 10;
     public int Health
    {
        set { health = value; }
        get { return health; }
    }

    // Start is called before the first frame update
    void Start()
    {
        orientation = Orientation.up;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the player's position this frame
        Vector2 playerPosition = (Vector2)playerTransform.position;
        //position = transform.position;
        // Get the vector from this enemy to the player
        Vector2 moveVector = playerPosition - (Vector2)transform.position;

        moveVector = moveVector.normalized;

        if (moveVector.y > 0)
        {
            orientation = Orientation.up;
        }
        else if (moveVector.y < 0)
        {

            orientation = Orientation.down;
        }
        else if (moveVector.x > 0)
        {

            orientation = Orientation.right;
        }
        else if (moveVector.x < 0)
        {
            orientation = Orientation.left;
        }

        // If the new position would be outside the stopDistance radius
        if ((playerPosition - (position + (moveVector * moveSpeed * Time.deltaTime))).sqrMagnitude > Mathf.Pow(stopDistance, 2))
        {
            // Apply the movement to the enemy's transform
            position += moveVector * moveSpeed * Time.deltaTime;
            isStopped = false;
        }
        // If it's already inside the radius
        else if ((playerPosition - position).sqrMagnitude <= Mathf.Pow(stopDistance, 2))
        {
            // Don't move
            isStopped = true;
        }
        // Otherwise it would be intruding into the radius
        else
        {
            // Apply the movement but only to the edge of that circle
            position += moveVector * ((playerPosition - position).magnitude - stopDistance);
            isStopped = true;
        }
        
        transform.position = position;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SpriteRenderer>().color == punchBox.GetComponent<SpriteRenderer>().color)
        {
            Debug.Log("PUNCH DETECTED");
        }
        else if (collision.GetComponent<SpriteRenderer>().color == kickBox.GetComponent<SpriteRenderer>().color)
        {
            Debug.Log("KICK DETECTED");
        }

    }
}

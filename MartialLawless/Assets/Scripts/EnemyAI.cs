using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyAI : MonoBehaviour
{
    private Orientation orientation;

    [SerializeField]
    private Transform playerTransform;

    private float moveSpeed = 2.0f;

    private Vector3 position;

    public Vector3 Position{
        get{return position;}
        set{position = value;}
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



        // Apply the movement to the enemy's transform
        position += (Vector3)(moveVector.normalized * moveSpeed * Time.deltaTime);

        transform.position = position;

    }
}

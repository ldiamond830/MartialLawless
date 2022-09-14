using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    private float moveSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get the player's position this frame
        Vector2 playerPosition = (Vector2)playerTransform.position;
        // Get the vector from this enemy to the player
        Vector2 moveVector = playerPosition - (Vector2)transform.position;

        // Apply the movement to the enemy's transform
        transform.position += (Vector3)(moveVector.normalized * moveSpeed * Time.deltaTime);

    }
}

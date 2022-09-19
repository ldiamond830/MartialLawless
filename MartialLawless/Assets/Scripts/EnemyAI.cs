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
    public float attackTimer = 0.0f; // seconds
    private float attackCooldown = 1.0f; // seconds between attacks
    private float blockDuration = 0.5f; // seconds
    private float kickDuration = 0.3f; // seconds
    private float punchDuration = 0.1f; // seconds
    private bool onCooldown = false;

    private Vector2 position;

    // Copied from PlayerController.cs
    private State state;

    //stats are public so they can be edited in the inspector
    public int health = 100;
    public int punchDamage = 10;
    public int kickDamage = 20;
    public int throwDamage = 25;

    //different sprites to show for each pose
    private SpriteRenderer spriteRenderer;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    //variables for controlling combat
    public GameObject punch;
    public GameObject kick;
    public GameObject block;
    public List<GameObject> attacks;

    public Vector3 Position
    {
        get{return position;}
        set{position = value;}
    }

    public Transform PlayerTransform
    {
        set { playerTransform = value; }
    }


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
        state = State.isMoving;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
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

        // UP
        if (moveVector.y > Mathf.Abs(moveVector.x))
        {
            orientation = Orientation.up;
        }
        // DOWN
        else if (moveVector.y < 0 && Mathf.Abs(moveVector.y) > Mathf.Abs(moveVector.x))
        {

            orientation = Orientation.down;
        }
        // RIGHT
        else if (moveVector.x > 0)
        {

            orientation = Orientation.right;
        }
        // LEFT
        else if (moveVector.x < 0)
        {
            orientation = Orientation.left;
        }

        if (onCooldown)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                onCooldown = false;
                attackTimer = 0.0f;
            }
        }

        switch (state)
        {
            case State.isIdle:
                // If this enemy is out of range
                if ((playerPosition - (position + (moveVector * moveSpeed * Time.deltaTime))).sqrMagnitude > Mathf.Pow(stopDistance, 2))
                {
                    state = State.isMoving;
                }
                
                break;

            case State.isMoving:
                // If it's already inside the radius
                if ((playerPosition - position).sqrMagnitude <= Mathf.Pow(stopDistance, 2))
                {
                    // Don't move
                    state = State.isIdle;
                }
                // If the new position would be inside the stopDistance radius
                else if ((playerPosition - (position + (moveVector * moveSpeed * Time.deltaTime))).sqrMagnitude < Mathf.Pow(stopDistance, 2))
                {
                    // Apply the movement but only to the edge of that circle
                    position += moveVector * ((playerPosition - position).magnitude - stopDistance);
                    state = State.isIdle;
                }
                else
                {
                    position += moveVector * moveSpeed * Time.deltaTime;
                }

                transform.position = position;

                break;

            case State.isBlocking:
                attackTimer += Time.deltaTime;

                if (attackTimer >= blockDuration)
                {
                    //after 60 cycles the player is able to move again
                    onCooldown = true;
                    attackTimer -= blockDuration;
                    Destroy(attacks[0]);
                    attacks.RemoveAt(0);
                    state = State.isMoving;
                }

                break;

            case State.isKicking:
                attackTimer += Time.deltaTime;

                if (attackTimer > kickDuration)
                {
                    //after 60 cycles the player is able to move again
                    onCooldown = true;
                    attackTimer -= kickDuration;
                    Destroy(attacks[0]);
                    attacks.RemoveAt(0);
                    state = State.isMoving;
                }

                break;

            case State.isPunching:
                attackTimer += Time.deltaTime;

                if (attackTimer >= punchDuration)
                {
                    //after 60 cycles the player is able to move again
                    onCooldown = true;
                    attackTimer -= punchDuration;
                    Destroy(attacks[0]);
                    attacks.RemoveAt(0);
                    state = State.isMoving;
                }

                break;

            case State.isThrowing:

                break;

            case State.isStunned:

                break;

        }

        // Only punches right now
        if (state == State.isIdle && !onCooldown)
        {
            Punch();
        }
    }

    private void Punch()
    {
        Debug.Log("Enemy punch");
        state = State.isPunching;

        //checks for orientation and spawns a hitbox in front of the player
        switch (orientation)
        {
            case Orientation.up:
                attacks.Add(Instantiate(punch, new Vector2(position.x, position.y + 0.5f), Quaternion.identity));

                break;
            case Orientation.down:
                attacks.Add(Instantiate(punch, new Vector2(position.x, position.y - 0.5f), Quaternion.identity));

                break;
            case Orientation.left:
                attacks.Add(Instantiate(punch, new Vector2(position.x - 0.5f, position.y), Quaternion.identity));

                break;
            case Orientation.right:
                attacks.Add(Instantiate(punch, new Vector2(position.x + 0.5f, position.y), Quaternion.identity));

                break;
        }
        //sound effect here
    }
}

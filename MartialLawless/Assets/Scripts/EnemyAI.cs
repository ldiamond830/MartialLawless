using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyAI : MonoBehaviour
{
    private Orientation orientation;

    [SerializeField]
    private Transform playerTransform;

    
    private float moveSpeed = 2.0f; // units per second
    private float stopDistance = 1.35f; // units away the enemy stops to attack the player
    public float attackTimer = 0.0f; // seconds
    private float attackCooldown = 1.0f; // seconds between attacks
    private float blockDuration = 0.5f; // seconds
    private float kickDuration = 0.3f; // seconds
    private float punchDuration = 0.1f; // seconds
    private bool onCooldown = false;

    private Vector2 position;

    private List<EnemyAI> enemies;

    // Copied from PlayerController.cs
    private State state;

    //stats are serialized so they can be edited in the inspector
    [SerializeField]
    private int punchDamage = 10;
    [SerializeField]
    private int kickDamage = 20;
    [SerializeField]
    private int throwDamage = 25;
    

    //different sprites to show for each pose
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite upSprite;
    [SerializeField]
    private Sprite downSprite;
    [SerializeField]
    private Sprite leftSprite;
    [SerializeField]
    private Sprite rightSprite;

    //variables for controlling combat
    [SerializeField]
    private AttackCollision punch;
    [SerializeField]
    private AttackCollision kick;

    private List<AttackCollision> attacks;

    public Manager gameManager;
    [SerializeField]
    public AudioSource gruntSound;

    private float hitIndicatorInterval;
    private float hitIndicatorTimer;

    public AttackCollision PunchObj
    {
        get { return punch; }
    }

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
    private int health = 1;

     public int Health
    {
        set { health = value; }
        get { return health; }
    }

    // Start is called before the first frame update
    void Start()
    {
        hitIndicatorInterval = 0.4f;
        hitIndicatorTimer = hitIndicatorInterval;

        orientation = Orientation.up;
        state = State.isMoving;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        attacks = new List<AttackCollision>();


        //intializes the punch hit box
        punch.manager = gameManager;
        punch.Damage = punchDamage;
        punch.IsPlayer = false;
        punch.ParentEnemy = this;
       

        //initializes the kick hit box
        kick.manager = gameManager;
        kick.Damage = kickDamage;
        kick.IsPlayer = false;
        kick.ParentEnemy = this;

        gruntSound = GameObject.FindGameObjectWithTag("gun").GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        //gruntSound.enabled = true;

        // Get the player's position this frame
        Vector2 playerPosition = (Vector2)playerTransform.position;
        //position = transform.position;
        // Get the vector from this enemy to the player
        Vector2 moveVector = playerPosition - (Vector2)transform.position;

        // Get an updated list of other active enemies
        enemies = gameManager.EnemyList;

        Vector2 personalSpaceVector = Vector2.zero;
        for (int i = 0; i < enemies.Count; i++)
        {
            // If this enemy is in this enemy's personal space
            if ((enemies[i].Position - transform.position).sqrMagnitude < Mathf.Pow(stopDistance, 2))
            {
                // Move away from them
                personalSpaceVector += (Vector2)(transform.position - enemies[i].Position).normalized;
            }
        }

        moveVector = moveVector.normalized;
        moveVector += personalSpaceVector.normalized;

        moveVector = moveVector.normalized;

        if(state!=State.isIdle)
        {
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


        if (spriteRenderer.color == Color.red)
        {
            if(hitIndicatorTimer <= 0)
            {
                hitIndicatorTimer = hitIndicatorInterval;
                spriteRenderer.color = Color.white;
            }
            else
            {
                hitIndicatorTimer -= Time.deltaTime;
            }
        }

        switch (state)
        {
            case State.isIdle:
                // If this enemy is out of range
                if ((playerPosition - (position + (moveVector * moveSpeed * Time.deltaTime))).sqrMagnitude > Mathf.Pow(stopDistance*1.5f, 2))
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
                //currently not being implimented
            case State.isBlocking:

                break;

            case State.isKicking:
                attackTimer += Time.deltaTime;

                if (attackTimer > kickDuration)
                {
                    //after 60 cycles the player is able to move again
                    onCooldown = true;
                    attackTimer -= kickDuration;
                    //returns punch hitbox to intial position
                    kick.gameObject.transform.position = position;
                    kick.IsActive = false;
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
                    punch.gameObject.transform.position = position;
                    punch.IsActive = false;
                    state = State.isMoving;
                }

                break;

            case State.isThrowing:

                break;

            case State.isStunned:

                break;

        }

        
        if (state == State.isIdle && !onCooldown)
        {
            //randomly selects the enemy's attack when they are in range
            int selector = Random.Range(0, 10);
            if(selector <= 6)
            {
                Punch();
            }
            else
            {
                Kick();
                
            }
            
        }
    }

    private void Punch()
    {
        gruntSound.enabled = true;

        if (gruntSound != null)
        {
            Debug.Log(gruntSound.isActiveAndEnabled);
            gruntSound.Play();
            Debug.Log("grunt Played");
        }
        Debug.Log("Enemy punch");
        state = State.isPunching;

        //AttackCollision newPunch;

        //checks for orientation and spawns a hitbox in front of the player
        //checks for orientation and spawns a hitbox in front of the player
        switch (orientation)
        {

            case Orientation.up:
                //punch.gameObject.SetActive(true);
                punch.gameObject.transform.position = new Vector2(position.x, position.y + 0.5f);

                break;

            case Orientation.down:
                punch.gameObject.transform.position = new Vector2(position.x, position.y - 0.5f);


                break;

            case Orientation.left:
                punch.gameObject.transform.position = new Vector2(position.x - 0.5f, position.y);

                break;
            case Orientation.right:
                punch.gameObject.transform.position = new Vector2(position.x + 0.5f, position.y);

                break;
        }

        

        
        punch.IsActive = true;
    }

    private void Kick()
    {
       
        Debug.Log("Enemy kick");
        state = State.isKicking;

        //AttackCollision newKick = null;

        //checks for orientation and spawns a hitbox in front of the player
        //checks for orientation and spawns a hitbox in front of the player
        switch (orientation)
        {
            case Orientation.up:
                kick.gameObject.transform.position = new Vector2(position.x, position.y + 0.5f);

                break;
            case Orientation.down:
                kick.gameObject.transform.position = new Vector2(position.x, position.y - 0.5f);

                break;
            case Orientation.left:
                kick.gameObject.transform.position = new Vector2(position.x - 0.5f, position.y);

                break;
            case Orientation.right:
                kick.gameObject.transform.position = new Vector2(position.x + 0.5f, position.y);

                break;
        }
        //sound effect here
        gruntSound.enabled = true;
        if (gruntSound != null)
       {
            gruntSound.Play();
            Debug.Log("grunt Played");
       }
        kick.IsActive = true;
    }
}

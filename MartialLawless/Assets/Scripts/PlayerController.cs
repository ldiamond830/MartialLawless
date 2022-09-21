using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public enum Orientation
{
    up,
    down,
    left,
    right
}
public enum State
{
    isIdle,
    isMoving,
    isPunching,
    isKicking,
    isThrowing,
    isStunned,
    isBlocking
}

public class PlayerController : MonoBehaviour
{
    public InputAction playerControls;
    private Vector2 direction = Vector2.zero;
    private Vector3 velocity = Vector2.zero;
    private Vector3 position;
    
    private Orientation orientation;
    private State state;

    //stats are public so they can be edited in the inspector
    public int moveSpeed = 5;
    public int health = 100;
    public int punchDamage = 10;
    public int kickDamage = 20;
    public int throwDamage = 25;
    public int currentAttackDamage = 0;

    //different sprites to show for each pose
    private SpriteRenderer spriteRenderer;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    //variables for controlling combat
    public AttackCollision punch;
    public AttackCollision kick;

    //needs to be changed to another script type when created
    public AttackCollision block;

    public float wait = 0.0f;
    public bool isAttacking = false;
    public List<AttackCollision> attacks;


    public Manager gameManager;

    //sounds
    public AudioSource gruntSound;


    // Start is called before the first frame update
    void Start()
    {
        position = this.transform.position;
        state = State.isMoving;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //what behavior the player is able to access is determined by the state of the player character
        switch (state)
        {
            case State.isMoving:
                Movement();

            break;

            case State.isBlocking:
                if (wait >= 0.5f)
                {
                    //after half a second the player can move and the hitbox is destroyed
                    wait = 0;
                    state = State.isMoving;
                    Destroy(attacks[0]);
                    attacks.RemoveAt(0);
                    isAttacking = false; 
                }
                else
                {
                    wait += Time.deltaTime;
                }

                    
                break;

            case State.isKicking:
                if(wait>0.3f)
                {
                    //after a third of a second the player can move and the hitbox is destroyed
                    wait = 0;
                    state = State.isMoving;
                    attacks[0].IsActive = false;
                    attacks.RemoveAt(0);
                    isAttacking = false;
                }
                else
                {
                    wait += Time.deltaTime;
                }
                   
                break;

            case State.isPunching:

                if (wait >= 0.1f)
                {
                    //after a tenth of a second the player can move and the hitbox is destroyed
                    wait = 0;
                    state = State.isMoving;
                    attacks[0].IsActive = false;
                    attacks.RemoveAt(0);
                    isAttacking = false;
                }
                else
                {
                    wait += Time.deltaTime;
                }
               
                break;

            case State.isThrowing:

                break;

            case State.isStunned:

                break;

        }
        
    }



    private void Movement()
    {
        //reads in the direction from the controls
        direction = playerControls.ReadValue<Vector2>();

        //sets the orientation based on the player's direction
        //NOTE: the player is able to move up/down and left/right at the same time, the way this is set up the up/down orientation will always override
        if (direction.y > 0)
        {
            orientation = Orientation.up;
        }
        else if (direction.y < 0)
        {

            orientation = Orientation.down;
        }
        else if (direction.x > 0)
        {

            orientation = Orientation.right;
        }
        else if (direction.x < 0)
        {
            orientation = Orientation.left;
        }
        

        //moves the player based on speed value, read in direction and scales by delta time
        velocity = new Vector3(direction.x * moveSpeed, direction.y * moveSpeed, 0);
        position += velocity * Time.deltaTime;
        transform.position = position;
    }

    private void OnPunch(InputValue value)
    {
        if(!isAttacking)
        {
            Debug.Log("Punch");
            state = State.isPunching;

            AttackCollision newPunch = null;
            

            //checks for orientation and spawns a hitbox in front of the player
            switch (orientation)
            {

                case Orientation.up:
                    newPunch = Instantiate(punch, new Vector2(position.x, position.y + 0.5f), Quaternion.identity);

                    break;

                case Orientation.down:
                    newPunch = Instantiate(punch, new Vector2(position.x, position.y - 0.5f), Quaternion.identity);
                    

                    break;

                case Orientation.left:
                    newPunch = Instantiate(punch, new Vector2(position.x - 0.5f, position.y), Quaternion.identity);

                    break;
                case Orientation.right:
                    newPunch = Instantiate(punch, new Vector2(position.x + 0.5f, position.y), Quaternion.identity);

                    break;
            }
            //sound effect here
            gruntSound.enabled = true;
            if (gruntSound != null)
            {
                Debug.Log("Sound Played");
                gruntSound.Play();
            }
         
            isAttacking = true;

            newPunch.manager = gameManager;
            newPunch.Damage = punchDamage;
            newPunch.IsPlayer = true;
            attacks.Add(newPunch);
        }
        
        
    }

    private void OnKick(InputValue value)
    {
        if(!isAttacking)
        {
            Debug.Log("Kick");
            state = State.isKicking;
            currentAttackDamage = kickDamage;
            //checks for orientation and spawns a hitbox in front of the player
            switch (orientation)
            {
                case Orientation.up:
                    attacks.Add(Instantiate(kick, new Vector2(position.x, position.y + 0.5f), Quaternion.identity));

                    break;
                case Orientation.down:
                    attacks.Add(Instantiate(kick, new Vector2(position.x, position.y - 0.5f), Quaternion.identity));

                    break;
                case Orientation.left:
                    attacks.Add(Instantiate(kick, new Vector2(position.x - 0.5f, position.y), Quaternion.identity));

                    break;
                case Orientation.right:
                    attacks.Add(Instantiate(kick, new Vector2(position.x + 0.5f, position.y), Quaternion.identity));

                    break;
            }
            //sound effect here
            isAttacking = true;
        }
        

    }

    private void OnBlock(InputValue value)
    {
        if(!isAttacking)
        {
            Debug.Log("Block");
            state = State.isBlocking;

            //checks for orientation and spawns a hitbox in front of the player
            switch (orientation)
            {
                case Orientation.up:
                    attacks.Add(Instantiate(block, new Vector2(position.x, position.y + 0.5f), Quaternion.Euler(0.0f, 0.0f, 90.0f)));

                    break;
                case Orientation.down:
                    attacks.Add(Instantiate(block, new Vector2(position.x, position.y - 0.5f), Quaternion.Euler(0.0f, 0.0f, -90.0f)));

                    break;
                case Orientation.left:
                    attacks.Add(Instantiate(block, new Vector2(position.x - 0.5f, position.y), Quaternion.identity));

                    break;
                case Orientation.right:
                    attacks.Add(Instantiate(block, new Vector2(position.x + 0.5f, position.y), Quaternion.identity));

                    break;
            }
            //sound effect here
            isAttacking = true;
        }
        
    }

    //might be necessary later
    /*
    private void OnMove()
    {
        if(state != State.isStunned)
        {
            state = State.isMoving;
        }
    }
    */

    //needed to for controls to work 
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
    
}

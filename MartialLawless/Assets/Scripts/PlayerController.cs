using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    isBlocking,
    isDodging,
    isPaused
}

public class PlayerController : MonoBehaviour
{
    public InputAction playerControls;
    private Vector2 direction = Vector2.zero;
    private Vector3 velocity = Vector2.zero;
    private Vector3 position;
    
    private Orientation orientation;
    private State state;

    //pause variables
    private State stateBeforePause;
    [SerializeField]
    private PauseController pauseController;

    //stats are public so they can be edited in the inspector
    public int moveSpeed = 5;
    private int health;
    public int maxHealth;
    public float maxStamina;
   
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

    //animation assets
    [SerializeField]
    private Animator animator;

    //variables for controlling combat
    public AttackCollision punch;
    public AttackCollision kick;

    //needs to be changed to another script type when created
    public AttackCollision thrown;

    public float wait = 0.0f;
    public bool isAttacking = false;

    private float staminaRechargeInterval = 0.75f;
    private float staminaRechargeTimer;
    private float stamina = 50;
   // private float maxStamina;

    public Manager gameManager;

    private bool damageAble;

    //borders
    private Bounds playerBounds;
    public GameObject leftBorder;
    private Bounds leftBorderBounds;
    public GameObject topBorder;
    private Bounds topBorderBounds;
    public GameObject bottomBorder;
    private Bounds bottomBorderBounds;
    public GameObject rightBorder;
    private Bounds rightBorderBounds;

    private BoxCollider2D collider;
    
    [SerializeField]
    public Text playerStaminaText;
    public Image fillImageSta;
    public Slider staminaSlider;

    float staminFill;

    public SpecialMove special;

    //sounds

    public AudioSource kickSound;
    public AudioSource punchSound;
    public AudioSource throwSound;
    public AudioSource specialSound;
    public AudioSource healthPickUpSound;

    private bool isRed;
    private float hitIndicatorInterval;
    private float hitIndicatorTimer;
    
    public BoxCollider2D Collider
    {
        get { return collider; }
    } 
    public SpriteRenderer SpriteRender
    {
        get { return spriteRenderer; }
    }

    public bool DamageAble
    {
        get { return damageAble; }
        set { damageAble = value; }
    }

    public bool IsAttacking
    {
        get { return isAttacking; }
    }

    public Orientation ReturnOrientation
    {
        get { return orientation; }
    }

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public State PlayerState
    {
        set { state = value; }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        collider = gameObject.GetComponent<BoxCollider2D>();


        health = maxHealth;
        maxStamina = stamina;
        isRed = false;
        hitIndicatorInterval = 0.4f;
        hitIndicatorTimer = hitIndicatorInterval;

        position = this.transform.position;
        state = State.isIdle;
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        //intializes the punch hit box
        punch.manager = gameManager;
        punch.Damage = punchDamage;
        punch.IsPlayer = true;

        //initializes the kick hit box
        kick.manager = gameManager;
        kick.Damage = kickDamage;
        kick.IsPlayer = true;

        //intializes the throw hit box
        thrown.manager = gameManager;
        thrown.Damage = punchDamage;
        thrown.IsPlayer = true;
        

        //gets bounds of each border object and the player sprite
        playerBounds = this.GetComponent<SpriteRenderer>().bounds;
        leftBorderBounds = leftBorder.GetComponent<SpriteRenderer>().bounds;
        topBorderBounds = topBorder.GetComponent<SpriteRenderer>().bounds;
        bottomBorderBounds = bottomBorder.GetComponent<SpriteRenderer>().bounds;
        rightBorderBounds = rightBorder.GetComponent<SpriteRenderer>().bounds;

        staminaSlider.GetComponent<Slider>();

        staminFill = stamina / 50.0f;
        staminaSlider.value = staminFill;
        playerStaminaText.text = "Stamina: " + (int)stamina;

        staminaRechargeTimer = staminaRechargeInterval;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("stamina: " + stamina);

        // Animation logic
        animator.SetBool("isMoving", state == State.isMoving);
        animator.SetInteger("orientation", (int)orientation);

        //prevents the player from moving out of bounds
        BoundsCheck();

        if (isRed)
        {
            
            if(hitIndicatorTimer <= 0)
            {
                if(special.IsActive)
                {
                    spriteRenderer.color = Color.cyan;
                    isRed = false;
                    hitIndicatorTimer = hitIndicatorInterval;
                }
                else
                {
                    spriteRenderer.color = Color.white;
                    isRed = false;
                    hitIndicatorTimer = hitIndicatorInterval;
                }
                
            }
            else
            {
                hitIndicatorTimer -= Time.deltaTime;
            }
        }

        if (!damageAble)
        {

        }
       
        //checks if the special is active
        if(special.IsActive)
        {
            //player gets infinite stamina while active
            stamina = maxStamina;
        }
        //what behavior the player is able to access is determined by the state of the player character
        switch (state)
        {
            case State.isIdle:
                Movement();

                //when recharge timer is zero and stamina is below max recharges stamina
                if (staminaRechargeTimer <= 0 && stamina < maxStamina)
                {
                    //increase or decrease constant to change stamina recharge rate
                    stamina += 7 * Time.deltaTime;
                    staminFill = stamina / 50.0f;
                    staminaSlider.value = staminFill;

                    if (stamina > maxStamina)
                    {
                        stamina = maxStamina;
                        staminFill = stamina / 100f;
                        staminaSlider.value = staminFill;
                        playerStaminaText.text = "Stamina: " + (int)stamina;

                    }
                }
                //uses else if so if stamina is maxed recharge timer doesn't change
                else if (staminaRechargeTimer > 0)
                {
                    staminaRechargeTimer -= Time.deltaTime;
                }

                break;

            case State.isMoving:
                Movement();

                //when recharge timer is zero and stamina is below max recharges stamina
                if(staminaRechargeTimer <= 0 && stamina < maxStamina)
                {
                    //increase or decrease constant to change stamina recharge rate
                    stamina += 7 * Time.deltaTime;
                    staminFill = stamina / 50.0f;
                    staminaSlider.value = staminFill;

                    if (stamina > maxStamina)
                    {
                        stamina = maxStamina;
                        staminFill = stamina / 100f;
                        staminaSlider.value = staminFill;
                        playerStaminaText.text = "Stamina: " + (int)stamina;

                    }
                }
                //uses else if so if stamina is maxed recharge timer doesn't change
                else if(staminaRechargeTimer > 0)
                {
                    staminaRechargeTimer -= Time.deltaTime;
                }

                break;

            case State.isThrowing:
                if (wait >= 0.5f)
                {
                    //after half a second the player can move and the hitbox is destroyed
                    wait = 0;
                    state = State.isMoving;
                    //returns punch hitbox to intial position
                    thrown.gameObject.transform.position = position;
                    thrown.IsActive = false;
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
                    //returns punch hitbox to intial position
                    kick.gameObject.transform.position = position;
                    kick.IsActive = false;
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
                    //returns punch hitbox to intial position
                    punch.gameObject.transform.position = position;
                    punch.IsActive = false;
                    isAttacking = false;
                }
                else
                {
                    wait += Time.deltaTime;
                }
               
                break;
                //not being included currently
            case State.isBlocking:

                break;

            case State.isStunned:

                break;

            case State.isDodging:
                if(wait >= 0.2f)
                {
                    state = State.isMoving;
                    damageAble = true;
                    wait = 0;
                    position.z++;

                }
                else
                {
                    wait += Time.deltaTime;

                    direction = playerControls.ReadValue<Vector2>();
                    velocity = new Vector3(direction.x * moveSpeed, direction.y * moveSpeed, 0);
                    velocity *= 2.5f;
                    position += velocity * Time.deltaTime;
                    
                    transform.position = position;
                    collider.enabled = true;
                }
                break;
            case State.isPaused:
                //does nothing while paused
                break;


        }
        
        staminFill = stamina / 100.0f;
        staminaSlider.value = staminFill;
        playerStaminaText.text = "Stamina: " + (int)stamina;
        
    }



    private void Movement()
    {
        //reads in the direction from the controlsas
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
        
        if (direction.x == 0 && direction.y == 0)
        {
            state = State.isIdle;
        }
        else
        {
            state = State.isMoving;
        }
        

        //moves the player based on speed value, read in direction and scales by delta time
        velocity = new Vector3(direction.x * moveSpeed, direction.y * moveSpeed, 0);
        position += velocity * Time.deltaTime;
        transform.position = position;
    }

    private void OnPunch(InputValue value)
    {
        if(!isAttacking && stamina >= 5.0f)
        {
            stamina -= 5.0f;
            staminFill = stamina / 100f;
            staminaSlider.value = staminFill;
            playerStaminaText.text = "Stamina: " + (int)stamina;

            staminaRechargeTimer = staminaRechargeInterval;

            Debug.Log("Punch");
            state = State.isPunching;

            //empties the list to remove any previously killed enemies
            punch.EnemyList.Clear();
           

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
                 punch.gameObject.transform.position =  new Vector2(position.x - 0.5f, position.y);

                    break;
                case Orientation.right:
                    punch.gameObject.transform.position = new Vector2(position.x + 0.5f, position.y);

                    break;
            }

            //sound effect here
            punchSound.enabled = true;
            if (punchSound != null)
            {
                punchSound.Play();
                Debug.Log("Punch Sound Played");
            }
         
            isAttacking = true;
            punch.IsActive = true;

            //adds each enemy to list so that the attack collision can check for collisions
            for (int i = 0; i < gameManager.EnemyList.Count; i++)
            {
                punch.EnemyList.Add(gameManager.EnemyList[i].GetComponent<BoxCollider2D>());
            }

        }
        
        
    }

    private void OnKick(InputValue value)
    {
        if(!isAttacking && stamina >= 10.0f)
        {
            stamina -= 10.0f;
            staminFill = stamina / 100f;
            staminaSlider.value = staminFill;
            playerStaminaText.text = "Stamina: " + (int)stamina;
            staminaRechargeTimer = staminaRechargeInterval;

            Debug.Log("Kick");
            state = State.isKicking;

            kick.EnemyList.Clear();

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
                    kick.gameObject.transform.position = new Vector2(position.x -0.5f, position.y);

                    break;
                case Orientation.right:
                    kick.gameObject.transform.position = new Vector2(position.x + 0.5f, position.y);

                    break;
            }
            //sound effect here
            kickSound.enabled = true;
            if (kickSound != null)
            {

                kickSound.Play();
                Debug.Log("Kick Sound Played");
            }

            isAttacking = true;

            kick.IsActive = true;

            //adds each enemy to list so that the attack collision can check for collisions
            for (int i = 0; i < gameManager.EnemyList.Count; i++)
            {
                kick.EnemyList.Add(gameManager.EnemyList[i].GetComponent<BoxCollider2D>());
            }
        }
        

    }

    private void OnThrow(InputValue value)
    {
        if(!isAttacking && stamina >= 15.0f)
        {
            //sets stamina and slider values
            stamina -= 15.0f;
            staminFill = stamina / 100.0f;
            staminaSlider.value = staminFill;
            playerStaminaText.text = "Stamina: " + (int)stamina;
            staminaRechargeTimer = staminaRechargeInterval;

            Debug.Log("throw");
            state = State.isThrowing;
            //sound effect here
            throwSound.enabled = true;
            if (throwSound != null)
            {
                throwSound.Play();
                Debug.Log("throw Sound Played");
            }

            thrown.EnemyList.Clear();

            //checks for orientation and spawns a hitbox in front of the player
            switch (orientation)
            {
                case Orientation.up:
                    thrown.gameObject.transform.position = new Vector2(position.x, position.y + 0.5f);
                    thrown.gameObject.transform.rotation = new Quaternion(90.0f, 90.0f, 0.0f, 0.0f);
                    break;
                case Orientation.down:
                    thrown.gameObject.transform.position = new Vector2(position.x, position.y - 0.5f);
                    thrown.gameObject.transform.rotation = new Quaternion(90.0f, 90.0f, 0.0f, 0.0f);

                    break;
                case Orientation.left:
                    thrown.gameObject.transform.position = new Vector2(position.x - 0.5f, position.y);
                    thrown.gameObject.transform.rotation = Quaternion.identity;

                    break;
                case Orientation.right:
                    thrown.gameObject.transform.position = new Vector2(position.x + 0.5f, position.y);
                    thrown.gameObject.transform.rotation = Quaternion.identity;

                    break;
            }
            //sound effect here
            isAttacking = true;

            thrown.IsActive = true;

            //adds each enemy to list so that the attack collision can check for collisions
            for (int i = 0; i < gameManager.EnemyList.Count; i++)
            {
                thrown.EnemyList.Add(gameManager.EnemyList[i].GetComponent<BoxCollider2D>());
            }
        }
        
    }
    private void OnSpecial(InputValue value)
    {
        //if the special attack bar is 10 or higher
        Debug.Log("Special bar fill: " + gameManager.SpecialAmountFull + "/ 10");
        if (gameManager.SpecialAmountFull >= 10)
        {
            //activate the special attack and reset the special attack bar
            gameManager.SpecialAmountFull = 0;
            special.ActivateSpecial();
        }

        //sound effect added here
        specialSound.enabled = true;
        if (specialSound != null)
        {

            specialSound.Play();
            Debug.Log("Special Sound Played");
        }
    }

    private void OnDodge(InputValue value)
    {
        //dodge sound
        if(state != State.isDodging && stamina >= 5.0f && state == State.isMoving)
        {
            stamina -= 5.0f;
            staminFill = stamina / 50.0f;
            staminaSlider.value = staminFill;
            playerStaminaText.text = "Stamina: " + (int)stamina;
            staminaRechargeTimer = staminaRechargeInterval;

            state = State.isDodging;
            damageAble = false;

            position.z--;

            collider.enabled = false;
        }
       
       
    }

    //method called by resume button on pause screen, needed because player controls methods are inaccessable 
    public void ButtonResume()
    {
        OnTogglePause(null);
    }

    private void OnTogglePause(InputValue value)
    {
        //resume
        if (pauseController.IsPaused)
        {
            //returns the player to whatever action they were taking before pausing
            state = stateBeforePause;
            pauseController.HidePauseScreen();

        }
        //pause
        else
        {
            
            stateBeforePause = state;
            //stops the player from being able to take actions while paused, since all other control methods can only be called when state is isMoving
            state = State.isPaused;
            pauseController.ShowPauseScreen();
        }
    }

    private void BoundsCheck()
    {
        if (position.x - playerBounds.extents.x <= leftBorderBounds.max.x)
        {
            position.x = leftBorderBounds.max.x + playerBounds.extents.x;
        }
        else if (position.x + playerBounds.extents.x >= rightBorderBounds.min.x)
        {
            position.x = rightBorderBounds.min.x - playerBounds.extents.x;
        }

        if (position.y + playerBounds.extents.y >= topBorderBounds.min.y)
        {
            position.y = topBorderBounds.min.y - playerBounds.extents.y;
        }
        else if (position.y - playerBounds.extents.y <= bottomBorderBounds.max.y)
        {
            position.y = bottomBorderBounds.max.y + playerBounds.extents.y;
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
       // Gizmos.DrawWireCube(leftBorderBounds.offset, leftBorderBounds.size);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("player collided");
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void Damage(int amount)
    {
        isRed = true;

        spriteRenderer.color = Color.red;
        health -= amount;
        if (health < 0)
        {
            health = 0;
        }
    }
}

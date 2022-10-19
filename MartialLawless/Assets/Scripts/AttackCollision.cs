using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{

    private BoxCollider2D collider;
    public Manager manager;
    private BoxCollider2D player;
    private List<BoxCollider2D> enemyList;
    private int damage;
    private bool isPlayer = true;
    public Throw throwObject;
    
    private bool isActive;

    public int Damage
    {
        get { return damage; }
        set {  damage = value; }
    }
    public bool IsPlayer
    {
        set { isPlayer = value; }
    }

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    public List<BoxCollider2D> EnemyList
    {
        get { return enemyList; }
        set { enemyList = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        isActive = false;

        collider = GetComponent<BoxCollider2D>();

        //stores colliders for each enemy
        enemyList = new List<BoxCollider2D>();

        player = manager.Player.gameObject.GetComponent<BoxCollider2D>();

        for(int i = 0; i < manager.EnemyList.Count; i++)
        {
            enemyList.Add(manager.EnemyList[i].GetComponent<BoxCollider2D>());
        }    
    }

    // Update is called once per frame
    void Update()
    {
        //prevents collision hitboxes from killing enemies while they are not being used to attack
        if (isActive)
        {
            if (isPlayer)
            {

             
            
                //checks collisions
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (enemyList[i] != null)
                    {
                        if (collider.IsTouching(enemyList[i]))
                        {
                            if(collider.GetComponent<AttackCollision>() == manager.Player.thrown)
                            {
                                throwObject.ThrowEnemy(enemyList[i], player.GetComponent<PlayerController>().ReturnOrientation, player);
                            }

                            //deals damage
                            manager.EnemyList[i].Health -= damage;
                            isActive = false;
                            
                            
                        }
                    }

                }
             

            }
            else
            {
                if (collider.IsTouching(player) && manager.Player.DamageAble)
                {
                    //deals damage
                    manager.Player.Damage(damage);
                    //manager.UpdatePlayerHealth();
                    isActive = false;
                }
            }
        }
        

       


        
    }
    /*
    private void OnTriggerEnter2D(Collision2D collision)
    {
        if(collision is EnemyAI)
        {
            Debug.Log("Enemy Hit");
        }
    }
    */
}

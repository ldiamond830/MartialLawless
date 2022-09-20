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

    private float remainOnScreen;
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

    // Start is called before the first frame update
    void Start()
    {
        remainOnScreen = 0.2f;
        isActive = true;

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
        if (isPlayer)
        {
            //checks collisions
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (collider.IsTouching(enemyList[i]))
                {
                    //deals damage
                    manager.EnemyList[i].Health -= damage;
                }
            }
        }
        else
        {
            if (collider.IsTouching(player))
            {
                //deals damage
                manager.Player.health -= damage;
            }
        }

        //add code to delete object
        if(isActive == false)
        {
            gameObject.SetActive(false);
        }


        
    }
}

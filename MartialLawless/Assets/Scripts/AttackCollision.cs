using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{

    private BoxCollider2D collider;
    public Manager manager;
    private List<BoxCollider2D> enemyList;
    private int damage;

    public int Damage
    {
        get { return damage; }
        set {  damage = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();

        //stores colliders for each enemy
        enemyList = new List<BoxCollider2D>();

        for(int i = 0; i < manager.EnemyList.Count; i++)
        {
            enemyList.Add(manager.EnemyList[i].GetComponent<BoxCollider2D>());
        }    
    }

    // Update is called once per frame
    void Update()
    {
        //checks collisions
        for(int i = 0; i < enemyList.Count; i++)
        {
            if (collider.IsTouching(enemyList[i]))
            {
                //deals damage
                manager.EnemyList[i].Health -= damage;
            }
        }

        //add code to delete object
        
    }
}

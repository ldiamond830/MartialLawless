using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    private BoxCollider2D collider;
    public Manager manager;
    private List<BoxCollider2D> enemyList;
    private int damage;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();

        enemyList = new List<BoxCollider2D>();

        for (int i = 0; i < manager.EnemyList.Count; i++)
        {
            enemyList.Add(manager.EnemyList[i].GetComponent<BoxCollider2D>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(BoxCollider2D enemy in enemyList)
        {
            if(collider.IsTouching(enemy))
            {
                Debug.Log("THROW");
            }
        }
    }
}

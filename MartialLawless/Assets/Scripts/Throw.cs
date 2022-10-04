using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public float wait;
    private bool isThrown;
    // Start is called before the first frame update
    void Start()
    {
        wait = 0;
        isThrown = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ThrowEnemy(BoxCollider2D enemy, Orientation orientation)
    {
        
        Debug.Log("Enemy Thrown");

        switch (orientation)
        {

            case Orientation.up:
                enemy.GetComponent<EnemyAI>().Position += new Vector3(0.0f, 0.3f);
                break;

            case Orientation.down:
                enemy.GetComponent<EnemyAI>().Position -= new Vector3(0.0f, 0.3f);
                break;

            case Orientation.left:
                enemy.GetComponent<EnemyAI>().Position -= new Vector3(1.0f, 0.0f);
                break;
            case Orientation.right:
                enemy.GetComponent<EnemyAI>().Position += new Vector3(1.0f, 0.0f);
                break;

        }
        
        
    }
}

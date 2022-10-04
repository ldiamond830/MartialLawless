using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{

    public float wait = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        wait += Time.deltaTime;
    }

    public void ThrowEnemy(BoxCollider2D enemy, Vector2 newPosition)
    {
        enemy.transform.position = newPosition; 
    }
}

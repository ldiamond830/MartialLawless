using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMove : MonoBehaviour
{
    private bool isActive = false;
    private int amountLeft = 0;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (amountLeft <= 0)
            {
                isActive = false;
            }
            else
            {
                if (timer >= 1)
                {
                    amountLeft--;
                    timer = 0;
                }
            }
            timer += Time.deltaTime;
            //Debug.Log("special is active");
            Debug.Log("time left: " + amountLeft);
        }
        else
        {
           // Debug.Log("Special is inactive");
        }
        
    }

    public void ActivateSpecial(int enemiesKilled)
    {
        if (isActive == false && enemiesKilled >= 10)
        {
            amountLeft = 10;
            isActive = true;
        }
    }
}

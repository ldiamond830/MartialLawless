using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMove : MonoBehaviour
{
    private bool isActive;
    private int amountLeft;
    private float timer;
    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        amountLeft = 0;
        timer = 0;
        player = this.GetComponent<PlayerController>();
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
                    player.Heal(5);
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

    public void ActivateSpecial()
    {
        amountLeft = 10;
        isActive = true;
    }
}

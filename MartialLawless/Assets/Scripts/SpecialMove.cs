using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMove : MonoBehaviour
{
    private bool isActive;
    private int amountLeft;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            if(amountLeft <= 0)
            {
                isActive = false;
            }
            else
            {
                if(timer >= 1)
                {
                    amountLeft--;

                }
            }
        }
        timer += Time.deltaTime;
        Debug.Log("special is active");
        Debug.Log("time left: " + amountLeft);
    }

    public void ActivateSpecial()
    {
        if(amountLeft >= 10 && isActive == false)
        {
            isActive = true;
        }
    }
}

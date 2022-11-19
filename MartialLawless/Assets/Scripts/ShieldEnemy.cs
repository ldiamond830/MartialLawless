using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : EnemyAI
{
    [SerializeField]
    private GameObject shield;
    public PauseController temp;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        base.PauseController = temp;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        switch (Orientation)
        {
            case Orientation.up:
                shield.transform.localPosition = new Vector3(0.0f, 0.6f, 0.0f);
                shield.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                break;
            case Orientation.down:
                shield.transform.localPosition = new Vector3(0.0f, -0.6f, 0.0f);
                shield.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
                break;
            case Orientation.left:
                shield.transform.localPosition = new Vector3(-0.6f, 0.0f, 0.0f);
                shield.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case Orientation.right:
                shield.transform.localPosition = new Vector3(0.6f, 0.0f, 0.0f);
                shield.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
        }
    }
}

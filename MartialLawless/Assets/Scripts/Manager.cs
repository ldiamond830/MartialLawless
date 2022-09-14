using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private waveCount;
    public PlayerController player;

    private float timeBetweenSpawn;

    //when set to true spawns new wave of enemies, when set to false wave is in progress
    private bool isSpawning;


    // Start is called before the first frame update
    void Start()
    {
        timeBetweenSpawn = 0.2;
        waveCount = 0;
        isSpawning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isSpawning){

        }
        else{

        }
    }
}

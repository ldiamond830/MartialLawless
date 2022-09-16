using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private int waveCount;
    public PlayerController player;

    private float timeBetweenSpawn;

    //when set to true spawns new wave of enemies, when set to false wave is in progress
    private bool isSpawning;
    private List<EnemyAI> enemyList;
    public EnemyAI enemyPrefab;

    public GameObject topSpawn;
    public GameObject bottomSpawn;
    public GameObject leftSpawn;
    public GameObject rightSpawn;


    // Start is called before the first frame update
    void Start()
    {
        timeBetweenSpawn = 0.2f;
        waveCount = 0;
        isSpawning = true;
        enemyList = new List<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isSpawning){
            
            for(int i = 0; i < waveCount; i++){
                EnemyAI newEnemy = Instantiate(enemyPrefab);

                 enemyList.Add(newEnemy);
                 
                 int doorSelect = Random.Range(0,4);

                 if(doorSelect == 0){
                    newEnemy.Position = topSpawn.transform.position;
                 }
                 else if(doorSelect == 1){
                    newEnemy.Position = rightSpawn.transform.position;
                 }
                 else if(doorSelect == 2){
                    newEnemy.Position = leftSpawn.transform.position;
                 }
                 else{
                    newEnemy.Position = rightSpawn.transform.position;
                 }
            }
        }
        else{

        }
    }
}

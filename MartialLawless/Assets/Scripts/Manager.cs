using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private Text playerHealthText;
    [SerializeField]
    private Text waveCountText;

    public static System.Random random = new System.Random();

    private int waveCount;
    public PlayerController player;

    private float timeBetweenSpawn;

    //when set to true spawns new wave of enemies, when set to false wave is in progress
    private bool isSpawning;
    public List<EnemyAI> enemyList;
    public EnemyAI enemyPrefab;

    /* failed idea may be useful later so I'm not deleting
    public GameObject topSpawn;
    public GameObject bottomSpawn;
    public GameObject leftSpawn;
    public GameObject rightSpawn;
    */
    private float cameraHeight;
    private float cameraWidth;
    public Camera cameraObject;

    public PlayerController Player
    {
        get { return player; }
    }
    public List<EnemyAI> EnemyList
    {
        get { return enemyList; }
    }

    private ScoreTracker scoreTracker;

    public List<EnemyAI> basicEnemySpawnPool = new List<EnemyAI>();

    // Start is called before the first frame update
    void Start()
    {
        scoreTracker = gameObject.GetComponent<ScoreTracker>();

        timeBetweenSpawn = 0.2f;
        waveCount = 1;
        UpdateWaveCountText();

        isSpawning = true;
        enemyList = new List<EnemyAI>();

        cameraHeight = cameraObject.orthographicSize * 2f;
        cameraWidth = cameraHeight * cameraObject.aspect;

        //populating spawn queue this sets the maximum number of enemies that can be on the screen at one time
        for(int i = 0; i < 10; i++)
        {
            EnemyAI newEnemy = Instantiate(enemyPrefab);
            newEnemy.PlayerTransform = player.transform;
            newEnemy.gameObject.SetActive(false);
            newEnemy.gameManager = this;
            basicEnemySpawnPool.Add(newEnemy);

        }

        //sets the initial value for player health
        UpdatePlayerHealth();
        player.DamageAble = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(player.health <= 0)
        {
            
            //build index for the loss scene
            SceneManager.LoadScene(1);
            
        }

        if(isSpawning){
            
                //creates a short interval between spawns so the player isn't rushed all at once
                if (basicEnemySpawnPool.Count > 0)
                {
                    for(int i = 0; i < waveCount ; i++)
                    {
                        if (waveCount == 4)
                        {
                            Debug.Log("test");
                        }

                        EnemyAI newEnemy = basicEnemySpawnPool[i];


                        enemyList.Add(newEnemy);
                        basicEnemySpawnPool.Remove(newEnemy);

                        //chooses a random spawn point for the new enemy
                        int doorSelect = Random.Range(0, 4);

                        if (doorSelect == 0)
                        {
                            //constant value makes it so enemy doesnt pop in on screen
                            newEnemy.Position = new Vector3(0, cameraHeight / 2 + 5, 0);
                        }
                        else if (doorSelect == 1)
                        {
                            newEnemy.Position = new Vector3(0, cameraHeight / -2 - 5, 0);
                        }
                        else if (doorSelect == 2)
                        {
                            newEnemy.Position = new Vector3(cameraWidth / -2 - 5, 0, 0);
                        }
                        else
                        {
                            newEnemy.Position = new Vector3(cameraWidth / 2 + 5, 0, 0);
                        }


                        newEnemy.gameObject.SetActive(true);
                        
                    }
                    
                }
                

            


            isSpawning = false;
        }
        else{

            if (enemyList.Count == 0)
            {
                isSpawning = true;
                
                waveCount++;
                UpdateWaveCountText();
            }

          foreach(EnemyAI enemy in enemyList)
            {
                if(waveCount >= 4)
                {
                    Debug.Log("error");
                }
                if (enemy.Health <= 0)
                {
                    //keeps track of al the enemies killed
                   //scoreTracker.enemies

                    enemy.PunchObj.IsActive = false;
                    enemy.PunchObj.transform.position = enemy.transform.position;

                    enemyList.Remove(enemy);

                    enemy.gameObject.SetActive(false);

                    enemy.Health = enemyPrefab.Health;

                    //returns the enemy to the spawning pool for reuse
                   basicEnemySpawnPool.Add(enemy);
                    ScoreTracker.enemiesKilled++;
                }
            }
        }
    }


    public void UpdatePlayerHealth()
    {
        playerHealthText.text = "Player Health: " + player.health;
    }

    public void UpdateWaveCountText()
    {
        waveCountText.text = "Wave Count: " + waveCount;
    }
}

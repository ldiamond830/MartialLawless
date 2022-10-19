using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField]
    public Text playerHealthText;
    public Image fillImage;
    public Slider healthSlider;

    

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

    private List<GameObject> healthDropPool;
    private List<GameObject> activeHealthDrops;
    public GameObject healthDropPrefab;

    /* failed idea may be useful later so I'm not deleting
    public GameObject topSpawn;
    public GameObject bottomSpawn;
    public GameObject leftSpawn;
    public GameObject rightSpawn;
    */
    private float cameraHeight;
    private float cameraWidth;
    public Camera cameraObject;

    //health
    float healthFill;

    //variable for tracking/controlling the special attack bar
    private int specialAmountFull;

    //sounds
    public AudioSource beginningWavesSound;

    //and getter setter for special attack bar
    public int SpecialAmountFull
    {
        get { return specialAmountFull; }
        set { specialAmountFull = value; }
    }

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
        beginningWavesSound.enabled = true;
        if (beginningWavesSound != null)
        {
            beginningWavesSound.Play();
            Debug.Log("beginningWavesSound Played");
        }

        healthSlider.GetComponent<Slider>();

        scoreTracker = gameObject.GetComponent<ScoreTracker>();

        timeBetweenSpawn = 0.2f;
        waveCount = 1;
        UpdateWaveCountText();

        isSpawning = true;
        enemyList = new List<EnemyAI>();

        healthDropPool = new List<GameObject>();
        activeHealthDrops = new List<GameObject>();

        for (int i = 0; i < 20; i++)
        {
            GameObject drop = Instantiate(healthDropPrefab);
            drop.transform.position = new Vector3(100.0f, 0.0f, 0.0f);
            healthDropPool.Add(drop);
        }

        cameraHeight = cameraObject.orthographicSize * 2f;
        cameraWidth = cameraHeight * cameraObject.aspect;

        //populating spawn queue this sets the maximum number of enemies that can be on the screen at one time
        for (int i = 0; i < 10; i++)
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
        if (player.Health <= 0)
        {
            //build index for the loss scene
            SceneManager.LoadScene(1);
        }

        if (isSpawning)
        {

            //creates a short interval between spawns so the player isn't rushed all at once
            if (basicEnemySpawnPool.Count > 0)
            {
                for (int i = 0; i < waveCount; i++)
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
        else
        {

            if (enemyList.Count == 0)
            {
                isSpawning = true;

                waveCount++;
                UpdateWaveCountText();
            }

            foreach (EnemyAI enemy in enemyList)
            {
               
                if (enemy.Health <= 0)
                {
                    //keeps track of al the enemies killed
                    //scoreTracker.enemies

                    //increases special bar for each enemy killed
                    specialAmountFull++;
                    Debug.Log("enemy killed");

                    if (random.Next(0, 100) < 100)
                    {
                        GameObject drop;
                        if (healthDropPool.Count > 0)
                        {
                            drop = healthDropPool[0];
                            activeHealthDrops.Add(drop);
                            healthDropPool.RemoveAt(0);
                        }
                        else
                        {
                            drop = Instantiate(healthDropPrefab);
                            activeHealthDrops.Add(drop);
                        }
                        drop.transform.position = enemy.transform.position;
                    }

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

            BoxCollider2D playerHitBox = player.GetComponent<BoxCollider2D>();
            foreach (GameObject healthDrop in activeHealthDrops)
            {
                // Debug.Log(healthDrop.GetComponent<BoxCollider2D>().IsTouching(playerHitBox));
                // Debug.Log(playerHitBox);
                // Check if any of the health drops are colliding with the player
                if (healthDrop.GetComponent<BoxCollider2D>().IsTouching(playerHitBox))
                {
                    // If they are, heal the player and delete them
                    Debug.Log("Drop touched");
                    player.Heal(20);
                    healthDropPool.Add(healthDrop);
                    activeHealthDrops.Remove(healthDrop);
                    healthDrop.transform.position = new Vector3(100.0f, 0.0f, 0.0f);

                }
            }
            
        }
        UpdatePlayerHealth();
    }


    public void UpdatePlayerHealth()
    {
        //Player health and Stamina
        healthFill = player.Health / 100f;
        healthSlider.value = healthFill;
        playerHealthText.text = "Player Health: " + player.Health;
    }

    public void UpdateWaveCountText()
    {
        waveCountText.text = "Wave Count: " + waveCount;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField]
    public Text playerHealthText;
    public Image fillImage;
    public Slider healthSlider;

    public Text specialText;
    public Slider specialSlider;

    [SerializeField]
    private SpriteRenderer bloodTint;
    

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
    public List<EnemyAI> basicEnemySpawnPool = new List<EnemyAI>();
    private Vector2 enemyPoolPosition = new Vector2(40.0f, 0.0f);

    private List<HealthDrop> healthDropPool;
    private List<HealthDrop> activeHealthDrops;
    public GameObject healthDropPrefab;
    private const float healthDropPickupRadius = 0.75f;
    private const float healthDropDuration = 15.0f;
    private Vector2 healthPoolPosition = new Vector2(40.0f, 5.0f);

    [SerializeField]
    private float healthDropRate = 20.0f; // percent

    //variable for special move
    public SpecialMove special;

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
    public AudioSource endingWavesSound;
    public AudioSource healthPickupSound;

    [SerializeField]
    private PauseController pauseController;

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
        specialSlider.GetComponent<Slider>();

        scoreTracker = gameObject.GetComponent<ScoreTracker>();

        timeBetweenSpawn = 0.2f;
        waveCount = 1;
        UpdateWaveCountText();

        isSpawning = true;
        enemyList = new List<EnemyAI>();

        healthDropPool = new List<HealthDrop>();
        activeHealthDrops = new List<HealthDrop>();

        for (int i = 0; i < 20; i++)
        {
            HealthDrop drop = Instantiate(healthDropPrefab).GetComponent<HealthDrop>();
            drop.transform.position = healthPoolPosition;
            healthDropPool.Add(drop);
        }

        cameraHeight = cameraObject.orthographicSize * 2f;
        cameraWidth = cameraHeight * cameraObject.aspect;

        //populating spawn queue this sets the maximum number of enemies that can be on the screen at one time
        for (int i = 0; i < 10; i++)
        {
            EnemyAI newEnemy = Instantiate(enemyPrefab);
            newEnemy.transform.position = enemyPoolPosition;
            newEnemy.Player = player;
            newEnemy.PlayerTransform = player.transform;
            newEnemy.gameObject.SetActive(false);
            newEnemy.gameManager = this;
            newEnemy.PauseController = pauseController;
            basicEnemySpawnPool.Add(newEnemy);

        }

        //sets the initial value for player health
        UpdatePlayerUI();
        player.DamageAble = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (player.Health <= 0)
        {
            //sets health to 0
            player.Health = 0;
            UpdatePlayerUI();

            float alpha = bloodTint.color.a;
            alpha += Time.deltaTime;
            bloodTint.color = new Color(245, 0, 0, alpha);
            //prevents the player from moving during the fade to red
            player.PlayerState = State.isIdle;
            if (alpha >= 1)
            {
                //takes the player to a game over screen when the fade is complete
                SceneManager.LoadScene("LossScene");

            }

        }
        else
        {
            if (!pauseController.IsPaused)
            {


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
                                //constant value makes it so enemy doesnt pop in on screen ll
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

                            //if the player is not currently using their special
                            if (!special.IsActive && specialAmountFull < 10)
                            {
                                //increases special bar for each enemy killed
                                specialAmountFull++;
                                Debug.Log("enemy killed");
                            }


                            if (random.NextDouble() * 100 < healthDropRate)
                            {
                                HealthDrop drop;
                                if (healthDropPool.Count > 0)
                                {
                                    Debug.Log("Drop pulled from pool");
                                    drop = healthDropPool[0];
                                    activeHealthDrops.Add(drop);
                                    healthDropPool.RemoveAt(0);
                                }
                                else
                                {
                                    drop = Instantiate(healthDropPrefab).GetComponent<HealthDrop>();
                                    activeHealthDrops.Add(drop);
                                }
                                drop.transform.position = enemy.Position;
                                drop.StartTimer();
                            }

                            enemy.transform.position = enemyPoolPosition;

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

                    foreach (HealthDrop healthDrop in activeHealthDrops)
                    {
                        // Check if any of the health drops are close enough to the player
                        if ((healthDrop.transform.position - player.Position).sqrMagnitude <= Mathf.Pow(healthDropPickupRadius, 2))
                        {
                            // If they are, heal the player and send them back to the pool
                            player.Heal(20);
                            healthDropPool.Add(healthDrop);
                            activeHealthDrops.Remove(healthDrop);
                            healthDrop.transform.position = new Vector3(100.0f, 0.0f, 0.0f);
                        }

                        // If it's reached it's despawn time threshold
                        if (healthDrop.Timer >= healthDropDuration)
                        {
                            healthDropPool.Add(healthDrop);
                            activeHealthDrops.Remove(healthDrop);
                            healthDrop.transform.position = new Vector3(100.0f, 0.0f, 0.0f);
                        }
                    }

                }
            }


            //outside of else statement so player health is updated when it reaches 0
            UpdatePlayerUI();

            if (waveCount > 5)
            {
                beginningWavesSound.Stop();
                endingWavesSound.enabled = true;
                endingWavesSound.Play();
            }
        }
    }


    public void UpdatePlayerUI()
    {
        //Player health
        healthFill = player.Health / 100f;
        healthSlider.value = healthFill;
        playerHealthText.text = "Player Health: " + player.Health;

        // Special
        specialSlider.value = specialAmountFull / 10.0f;
        specialText.text = "Special: " + specialAmountFull;
    }

    public void UpdateWaveCountText()
    {
        waveCountText.text = "Wave Count: " + waveCount;
    }

    public void CollectHealthDrop(HealthDrop drop)
    {
        //add pick up sound
        activeHealthDrops.Remove(drop);
        healthDropPool.Add(drop);
        drop.transform.position = new Vector3(100.0f, 0.0f, 0.0f);
        healthPickupSound.Play();
    }
}

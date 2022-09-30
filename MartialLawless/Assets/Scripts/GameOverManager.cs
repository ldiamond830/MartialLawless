using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;

    // Start is called before the first frame update
    Scene mainMenu;
    void Start()
    {
        scoreText.text = "Enemies Killed: " + ScoreTracker.enemiesKilled;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        ScoreTracker.enemiesKilled = 0;
    }
}

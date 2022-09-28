using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverManager : MonoBehaviour
{
    // Start is called before the first frame update
    Scene mainMenu;
    void Start()
    {
        //mainMenu = SceneManager.GetSceneByName("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

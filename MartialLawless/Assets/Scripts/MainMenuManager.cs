using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MainMenuManager : MonoBehaviour
{
    public AudioSource introSong;

    // Start is called before the first frame update
    void Start()
    {
        introSong.enabled = true;
        if (introSong != null)
        {
            introSong.Play();
            Debug.Log("Sound Played");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        //0 is the index of gameScene in the build settings
        introSong.Stop();
        SceneManager.LoadScene(0);
       

    }
}

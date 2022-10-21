using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public AudioSource introSong;
    public Image instructions;
    private bool instructionsVisible = false;


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

    public void ToggleInstructions()
    {
        //if instructions box is visible hides it
        if (instructionsVisible)
        {
            instructionsVisible = false;
            instructions.gameObject.SetActive(false);
        }
        //if instruction box is invisible shows it
        else
        {
            instructionsVisible = true;
            instructions.gameObject.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

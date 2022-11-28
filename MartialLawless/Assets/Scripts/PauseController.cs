using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PauseController : MonoBehaviour
{
    private bool isPaused;

    [SerializeField]
    private Image greyFilter;
    [SerializeField]
    private GameObject pauseContent;


    public bool IsPaused
    {
        get { return isPaused; }
    }
    // Start is called before the first frame update
    void Start()
    {
        //greyFilter.color = new Color(190, 190, 190, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPauseScreen()
    {
        isPaused = true;
        greyFilter.color = new Color(190, 190, 190, 0.7f);
        pauseContent.SetActive(true);
    
    }

    public void HidePauseScreen()
    {
        isPaused = false;
        greyFilter.color = new Color(190, 190, 190, 0.0f);
        pauseContent.SetActive(false);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// A main menü gombjait működtető scriptek
public class MainMenu : MonoBehaviour
{
    public void SinglePlayer()
    {
        SceneManager.LoadScene("Tetris");
    }
    public void MultiPlayer()
    {
        SceneManager.LoadScene("Loading");
    }
    public void Mainmenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit()
    {
        Application.Quit();    
    }
}
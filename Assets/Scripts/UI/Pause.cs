using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    //stop time
    public void PauseGame()
    {
        //do things
    }

    //resume time
    public void ResumeGame()
    {
        //undo things
    }

    //restart scene
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    //to menu scene
    public void ToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour 
{
    public void ResetGame()
    {
        // reset the time scale
        Time.timeScale = 1;
        // load the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RestartLevel : MonoBehaviour
{
    // Start is called before the first frame update

    public GameOverScript gameOverScript;

    //Function to restart the level
    public void restartLevel()
    {
        Time.timeScale = 1;
        gameOverScript.health = 3;
        SceneManager.LoadScene(0);
    }
}

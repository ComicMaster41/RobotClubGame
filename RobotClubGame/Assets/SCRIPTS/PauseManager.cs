using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public GameObject pauseScreen;//, crosshair, retical;
    public int pauseState;


    // Start is called before the first frame update
    void Start()
    {
        //Initial Pause Screen state (off)
        pauseState = 0;
        pauseScreen.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (pauseState)
        {
            case 0:
                {
                    //Turn off Pause Screen
                   pauseScreen.gameObject.SetActive(false);
                   break;
                }
            case 1: 
                {
                    //Turn on Pause Screen
                    pauseScreen.gameObject.SetActive(true);
                    break;
                }

        }

        //Function to pause game when pressing key (Q)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(pauseState == 0)
            {
                pauseState = 1;
                Time.timeScale = 0;
            }
            else
            {
                pauseState = 0;
                Time.timeScale = 1;
            }
        }


    }
}

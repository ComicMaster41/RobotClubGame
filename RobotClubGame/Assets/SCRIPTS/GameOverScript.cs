using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject gameOver, heart1, heart2, heart3;
    public int health;

    void Start()
    {
        //Initializing UI elements
        health = 3;
        heart1.gameObject.SetActive(true);
        heart2.gameObject.SetActive(true);
        heart3.gameObject.SetActive(true);
        gameOver.gameObject.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        //Switch case for Health Lose
        switch(health)
        {
            case 3:
                {
                    heart1.gameObject.SetActive(true);
                    heart2.gameObject.SetActive(true);
                    heart3.gameObject.SetActive(true);
                    break;
                }
            case 2:
                {
                    heart1.gameObject.SetActive(true);
                    heart2.gameObject.SetActive(true);
                    heart3.gameObject.SetActive(false);
                    break;
                }
            case 1:
                {
                    heart1.gameObject.SetActive(true) ;
                    heart2.gameObject.SetActive(false);
                    heart3.gameObject.SetActive(false);
                    break;
                }
            default:
                {
                    heart1.gameObject.SetActive(false);
                    heart2.gameObject.SetActive(false);
                    heart3.gameObject.SetActive(false);
                    gameOver.gameObject.SetActive(true);
                    Time.timeScale = 0;
                    break;
                }
        }

        //CAN DELETE LATER, ONLY HERE SINCE THERE ARE NO WAYS TO LOSE HEALTH
        if(Input.GetKeyDown(KeyCode.P)) 
        {
            health--;
        }
       
        
    }
}

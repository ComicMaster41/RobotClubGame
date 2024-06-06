using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleToggle : MonoBehaviour
{

    public GameObject crosshair, retical;

    // Update is called once per frame
    void Update()
    {
        //Turns on and off retical depending on game/pause state, may change later
        if(Time.timeScale == 0)
        {
            crosshair.gameObject.SetActive(false);
            retical.gameObject.SetActive(false);
        }
        else
        {
            crosshair.gameObject.SetActive(true);
            retical.gameObject.SetActive(true);
        }
    }
}

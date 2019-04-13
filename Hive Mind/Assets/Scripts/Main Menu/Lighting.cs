using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    Light light;

    float timeOut = 0f;
    float timeOn = 0f;

    bool on = true;
    bool reload = false;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        timeOut = generateTimeOut();
        timeOn = generateTimeOn();
    }

    private void Update()
    {
        if (on)
        {
            timeOn -= Time.deltaTime;

            if (timeOn > 0f)
            {
                light.enabled = true;
            }
            else
            {
                on = false;
            }
        }
        else
        {
            if (timeOut > 0f)
            {
                timeOut -= Time.deltaTime;
                light.enabled = false;
            }
            else
            {
                reload = true;
            }
        }

        if (reload)
        {
            timeOut = generateTimeOut();
            timeOn = generateTimeOn();

            on = true;
            reload = false;
        }
    }

    float generateTimeOut()
    {
        return Random.Range(0f,2f);
    }

    float generateTimeOn()
    {
        return Random.Range(.1f, .35f);
    }
}

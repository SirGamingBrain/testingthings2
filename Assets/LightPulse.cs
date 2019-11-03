using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public Light consoleLight;
    public float bounceTime;
    // Start is called before the first frame update
    void Start()
    {
        consoleLight = this.gameObject.GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        Flux();
    }

    public void Flux()
    {
        float pulse = Mathf.PingPong(Time.time, bounceTime);
        consoleLight.intensity = pulse;
    }
}

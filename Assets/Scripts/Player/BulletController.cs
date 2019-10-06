using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    float bulletSpeed = 0.25f;
    float timeToLive = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Translate(0, 0, bulletSpeed);
        timeToLive -= 0.1f;

        if (timeToLive <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Detection Sphere")
        {
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhistleAOEUI : MonoBehaviour
{
    // Start is called before the first frame update
    Material AOE;

    void Start()
    {
        AOE = GetComponent<Renderer>().material;
        AOE.color = new Color(0.3f, 0.15f, 0.15f, 0f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            AOE.color = new Color(0.3f,0.15f,0.15f,0.17f);

        }
        else
        {
            AOE.color = new Color(0.3f, 0.15f, 0.15f, 0f);
        }
    }
}

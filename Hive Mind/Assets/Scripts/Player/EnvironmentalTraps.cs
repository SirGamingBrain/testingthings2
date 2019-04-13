using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalTraps : MonoBehaviour
{
    public GameObject player;

    public Animator door1;
    public Animator door2;

    float distance;

    bool interactable = true;

    public Text text;

    public void Start()
    {
        if (this.gameObject.name == "closing entrance")
        {
            door1.SetBool("Open", true);
            door2.SetBool("Open", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate the distance between the gameobject with this script attached and the player.
        if (this.gameObject.name == "checkpointOpen")
        {
            distance = Vector3.Distance(player.transform.position, this.transform.position);
        }

        //If we are within about 3 units of distance, we want to display text to the user and allow them to interact with the appropriate object.
        if (distance <= 3f)
        {
            if (this.gameObject.name == "checkpointOpen" && interactable == true)
            {
                text.text = "Hit E to open the next safety room doors.";
            }

            if (Input.GetKeyDown(KeyCode.E) && interactable == true)
            {
                //Call the function to check the name of the object to see what it should do. (Open a door, break a pipe, block something)
                //Also I have the interactable object currently set to be a one time use, this can change however.
                InteractName();
                interactable = false;
            }
        }
        else if (distance > 3f)
        {
            text.text = " ";
        }
    }

    void InteractName()
    {
        //Calling this function to see what operation we should take based on the name of the interactable object.

        if (this.gameObject.name == "checkpointOpen")
        {
            door1.SetBool("Open", true);
            door2.SetBool("Open", true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (this.tag == "lock")
        {
            door1.SetBool("Open", false);
            door2.SetBool("Open", false);

            interactable = true;
        }
        else if (this.tag == "entranceLock")
        {
            door1.SetBool("Open", false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessPanelVariables : MonoBehaviour
{
    public Animator checkpointDoor;
    public Animator checkpointDoor2;

    public bool interactable = true;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.tag == "door access") {
            checkpointDoor.SetBool("Open", false);
            checkpointDoor2.SetBool("Open", false);
            interactable = true;
        }
        else if (this.gameObject.tag == "refill")
        {
            interactable = false;
        }
        else if (this.gameObject.tag == "lock")
        {
            interactable = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Hitbox")
        {
            if (this.gameObject.tag == "refill")
            {
                checkpointDoor.SetBool("Open", false);
                checkpointDoor2.SetBool("Open", true);
            }
            else if (this.gameObject.tag == "lock")
            {
                checkpointDoor2.SetBool("Open", false);
            }
        }
    }
}

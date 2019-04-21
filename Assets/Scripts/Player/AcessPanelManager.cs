using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcessPanelManager : MonoBehaviour
{
    public GameObject player;

    float distance;

    public Text text;

    GameObject[] accessPanels;

    public GameObject currentPanel;
    AccessPanelVariables AccessPanelScripts;

    public void Start()
    {
        accessPanels = GameObject.FindGameObjectsWithTag("door access");
    }

    // Update is called once per frame
    void Update()
    {
        //For every access panel in the level.
        foreach (GameObject accessPanel in accessPanels)
        {
            //Calculate the distance from every access panel to the player.
            distance = Vector3.Distance(player.transform.position, accessPanel.transform.position);

            //If an access panel is within 3 units of the player, we need to do a few things.
            if (distance <= 3f)
            {
                //We grab the script that holds the specific access panel's variables.
                currentPanel = accessPanel;
                AccessPanelScripts = currentPanel.GetComponent<AccessPanelVariables>();
                
                //We update the text shown to the player indicating the fact that they can open the next zone.
                if (AccessPanelScripts.interactable == true)
                {
                    text.text = "Hit E to unlock the next checkpoint zone.";
                }
                else
                {
                    text.text = " ";
                }

                //Finally we check to see that if the player hits the E key, and the specific access panel is interactable, then the panel's door will be opened.
                if (Input.GetKeyDown(KeyCode.E) && AccessPanelScripts.interactable == true)
                {
                    AccessPanelScripts.interactable = false;
                    AccessPanelScripts.checkpointDoor.SetBool("Open", true);
                }
                break;
            }
            else if (distance > 3f)
            {
                text.text = " ";
            }
        }
    }
}

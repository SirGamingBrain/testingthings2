using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VariableHolder : MonoBehaviour
{
    public bool cutscene = false;
    public bool paused = false;
    public bool tutorial = false;
    public bool sectionDisplay = false;

    public string levelName = "null";
    public string sectionName = "null";
    public string displayLevelName = "null";
    public string displaySectionName = "null";

    float titleAlpha = 0f;

    Scene scene;

    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "NewTutorial")
        {
            tutorial = true;
            displayLevelName = "Medical Ward";
        }
        else if (scene.name == "1st Level")
        {
            tutorial = false;
            displayLevelName = "Feeding Grounds";
        }
        else if (scene.name == "2nd Level")
        {
            tutorial = false;
            displayLevelName = "Heart of the Hive";
        }
        else if (scene.name == "3rd Level")
        {
            tutorial = false;
            displayLevelName = "Breeding Grounds";
        }

        if (sectionName == "new" || sectionName == "End")
        {
            sectionName = "new";
            cutscene = true;

            if (scene.name == "NewTutorial")
            {
                sectionName = "Cryo Pod";
            }
            else
            {
                sectionName = "Entrance";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial == true)
        {

        }
    }
}

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
    public bool displaySection = false;

    public float tutorialTimer = 0f;

    Scene scene;

    // 2 - UI Stuff
    public CanvasGroup cutsceneBars;

    float barsAlpha = 0f;

    // 3 - Player Stuff
    public GameObject playerModel;

    public Rigidbody playerbody;

    Quaternion newRotation;

    public Animator playerAnimations;

    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "NewTutorial")
        {
            tutorial = true;
            cutscene = true;
            barsAlpha = 1f;
            cutsceneBars.alpha = 1f;
        }
        else
        {
            tutorial = false;
            cutscene = true;
            barsAlpha = 1f;
            cutsceneBars.alpha = 1f;
        }

        displaySection = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial == true)
        {
            tutorialTimer += Time.deltaTime;
        }
    }
}

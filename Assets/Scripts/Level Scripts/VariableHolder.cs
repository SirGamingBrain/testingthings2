using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VariableHolder : MonoBehaviour
{
    // 1 - General Stuff
    public bool cutscene = false;
    public bool paused = false;
    public bool tutorial = false;
    public bool taserCutscene = false;
    public bool displaySection = false;

    public float tutorialTimer = 0f;
    public float cutsceneTimer = 0f;

    Scene scene;

    // 2 - UI Stuff
    public CanvasGroup cutsceneBars;
    public CanvasGroup playerText;

    float barsAlpha = 0f;
    float playerTextAlpha = 0f;

    public Text playerWords;
    public Text cutsceneText;

    public bool showText = false;

    // 3 - Player Stuff
    public GameObject playerModel;

    public Rigidbody playerbody;

    Quaternion newRotation;

    public Animator playerAnimations;

    // 4 - Camera Stuff
    public GameObject cameraPlacement;
    private Vector3 newPlace;
    private Vector3 oldPlace;

    private float cameraHeight = 10f;

    private float moveTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "NewTutorial" && PlayerPrefs.GetString("Last Checkpoint") == "new")
        {
            tutorial = true;
            cutscene = true;
            barsAlpha = 1f;
            cutsceneBars.alpha = 1f;
        }
        else if (PlayerPrefs.GetString("Last Checkpoint") == "End" || PlayerPrefs.GetString("Last Checkpoint") == "new")
        {
            tutorial = false;
            cutscene = true;
            barsAlpha = 1f;
            cutsceneBars.alpha = 1f;
        }
        else
        {
            tutorial = false;
            cutscene = false;
            barsAlpha = 0f;
            cutsceneBars.alpha = 0f;
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
        else if (taserCutscene == true)
        {
            cutsceneTimer += Time.deltaTime;
        }
        else
        {

        }
    }
}

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
    public GameObject player;

    public GameObject playerModel;

    public Rigidbody playerbody;

    Quaternion newRotation;

    public Animator playerAnimations;

    // 4 - Camera Stuff
    public GameObject cameraPlacement;
    private Vector3 newPlace;
    private Vector3 oldPlace;

    readonly private float cameraHeight = 10f;

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

            if (PlayerPrefs.GetString("Last Checkpoint") == "End")
            {
                PlayerPrefs.SetString("Last Checkpoint", "new");
            }
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
        if (tutorial == true && cutscene == true)
        {
            if (PlayerPrefs.GetString("Last Checkpoint") == "new")
            {
                if (cutsceneTimer >= 10.2f)
                {
                    cutsceneTimer = 0f;

                    cutscene = false;
                }
                else if (cutsceneTimer >= 7.2f)
                {
                    if (playerTextAlpha < 1f)
                    {
                        playerWords.text = "Seems like these untouched checkpoint zones are gonna be my ticket out of here...";
                        playerTextAlpha += Time.deltaTime;
                        playerText.alpha = playerTextAlpha;
                    }
                    else if (playerTextAlpha > 1f)
                    {
                        playerTextAlpha = 1f;
                        playerText.alpha = playerTextAlpha;
                    }

                    oldPlace = cameraPlacement.transform.position;

                    newPlace = new Vector3(37f, 10f, -1f);

                    moveTime += Time.deltaTime;

                    if (moveTime > 2f)
                    {
                        moveTime = 2f;
                    }

                    float perc = moveTime / 2f;

                    cameraPlacement.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);
                }
                else if (cutsceneTimer >= 6.2f)
                {
                    if (playerTextAlpha < 0f)
                    {
                        playerTextAlpha = 0f;
                        playerText.alpha = playerTextAlpha;
                        playerWords.text = "Seems like these untouched checkpoint zones are gonna be my ticket out of here...";
                    }
                    else if (playerTextAlpha <= 1f)
                    {
                        playerTextAlpha -= Time.deltaTime;
                        playerText.alpha = playerTextAlpha;
                    }

                    moveTime = 0f;
                }
                else if (cutsceneTimer >= 4.2f)
                {
                    if (playerTextAlpha < 1f)
                    {
                        playerTextAlpha += Time.deltaTime;
                        playerText.alpha = playerTextAlpha;
                    }
                    else if (playerTextAlpha > 1f)
                    {
                        playerTextAlpha = 1f;
                        playerText.alpha = playerTextAlpha;
                    }

                    oldPlace = cameraPlacement.transform.position;

                    newPlace = new Vector3(28f, 4f, -15f);

                    moveTime += Time.deltaTime;

                    if (moveTime > 2f)
                    {
                        moveTime = 2f;
                    }

                    float perc = moveTime / 2f;

                    cameraPlacement.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);
                }
                else if (cutsceneTimer >= 0f)
                {
                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .008f * 2.3f);
                    playerbody.MovePosition(newPosition);
                }

                cutsceneTimer += Time.deltaTime;
            }
            else if (PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 1")
            {
                if (cutsceneTimer >= 5f)
                {
                    cutsceneTimer = 0f;
                    cutscene = true;
                    showText = true;
                }
                else if (cutsceneTimer >= 1f)
                {
                    if (playerTextAlpha < 1f)
                    {
                        playerTextAlpha += Time.deltaTime;
                        playerText.alpha = playerTextAlpha;
                    }
                    else if (playerTextAlpha > 1f)
                    {
                        playerTextAlpha = 1f;
                        playerText.alpha = playerTextAlpha;
                    }

                    oldPlace = cameraPlacement.transform.position;

                    newPlace = new Vector3(76f, 5f, 0f);

                    moveTime += Time.deltaTime;

                    if (moveTime > 2f)
                    {
                        moveTime = 2f;
                    }

                    float perc = moveTime / 2f;

                    cameraPlacement.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);

                    playerAnimations.SetBool("running", false);

                    moveTime = 0f;
                }
                else if (cutsceneTimer >= 0f)
                {
                    playerWords.text = "I might be able to lure these guys away with a noise...";

                    playerAnimations.SetBool("walking", false);
                    playerAnimations.SetBool("running", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .008f * 15f);
                    playerbody.MovePosition(newPosition);
                }

                cutsceneTimer += Time.deltaTime;
            }
            else if (PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 2")
            {
                if (cutsceneTimer >= 6f)
                {
                    cutsceneTimer = 0f;
                    PlayerPrefs.SetString("Cutscene", "false");
                    PlayerPrefs.SetString("Paused", "false");
                    showText = true;
                }
                else if (cutsceneTimer >= 1f)
                {

                    if (playerTextAlpha < 1f)
                    {
                        playerTextAlpha += Time.deltaTime;
                        playerText.alpha = playerTextAlpha;
                    }
                    else if (playerTextAlpha > 1f)
                    {
                        playerTextAlpha = 1f;
                        playerText.alpha = playerTextAlpha;
                    }

                    oldPlace = cameraPlacement.transform.position;

                    newPlace = new Vector3(101f, 4f, 4f);

                    moveTime += Time.deltaTime;

                    if (moveTime > 3f)
                    {
                        moveTime = 3f;
                    }

                    float perc = moveTime / 3f;

                    cameraPlacement.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);
                }
                else if (cutsceneTimer >= 1f)
                {

                    if (playerTextAlpha < 1f)
                    {
                        playerTextAlpha += Time.deltaTime;
                        playerText.alpha = playerTextAlpha;
                    }
                    else if (playerTextAlpha > 1f)
                    {
                        playerTextAlpha = 1f;
                        playerText.alpha = playerTextAlpha;
                    }

                    oldPlace = cameraPlacement.transform.position;

                    newPlace = new Vector3(101f, 4f, 2f);

                    moveTime += Time.deltaTime;

                    if (moveTime > 3f)
                    {
                        moveTime = 3f;
                    }

                    float perc = moveTime / 3f;

                    cameraPlacement.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);
                }
                else if (cutsceneTimer >= .6f)
                {
                    playerWords.text = "I hear a creature up ahead, I don't think I can run past him...";

                    if (playerTextAlpha < 1f)
                    {
                        playerTextAlpha += Time.deltaTime;
                        playerText.alpha = playerTextAlpha;
                    }
                    else if (playerTextAlpha > 1f)
                    {
                        playerTextAlpha = 1f;
                        playerText.alpha = playerTextAlpha;
                    }

                    playerAnimations.SetBool("running", false);
                    PlayerPrefs.SetString("Paused", "true");
                    moveTime = 0f;
                }
                else if (cutsceneTimer >= 0f)
                {
                    PlayerPrefs.SetString("Section Display", "true");
                    playerAnimations.SetBool("walking", false);
                    playerAnimations.SetBool("running", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .008f * 15f);
                    playerbody.MovePosition(newPosition);
                    Debug.Log("Beginning Cutscene 2");
                }

                cutsceneTimer += Time.deltaTime;
            }
        }
        else if (tutorial == true && cutscene == false)
        {
            if (playerTextAlpha > 0f)
            {
                playerTextAlpha -= Time.deltaTime;
                playerText.alpha = playerTextAlpha;
            }
            else if (playerTextAlpha < 0f)
            {
                playerTextAlpha = 0f;
                playerText.alpha = 0f;
            }

            if (barsAlpha > 0f)
            {
                barsAlpha -= Time.deltaTime * 2f;
                cutsceneBars.alpha = barsAlpha;
                playerText.alpha = barsAlpha;
            }
            else if (barsAlpha < 0f)
            {
                barsAlpha = 0f;
                cutsceneBars.alpha = barsAlpha;
                playerText.alpha = barsAlpha;
            }
        }
        else if (taserCutscene == true && cutscene == true)
        {
            cutsceneTimer += Time.deltaTime;
        }
        else if (cutscene == true && tutorial == false)
        {
            if (PlayerPrefs.GetString("Last Checkpoint") == "new")
            {
                if (cutsceneTimer >= 2.5f)
                {
                    cutsceneTimer = 0f;
                    cutscene = false;
                }
                else if (cutsceneTimer >= 0f)
                {
                    playerAnimations.SetBool("running", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .01f * 15f);
                    playerbody.MovePosition(newPosition);
                }
            }
            else if (PlayerPrefs.GetString("Last Checkpoint") == "End")
            {
                if (cutsceneTimer >= 4f)
                {
                    cutsceneTimer = 0f;
                    cutscene = false;
                }
                else if (cutsceneTimer >= 0f)
                {
                    playerAnimations.SetBool("running", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .01f * 15f);
                    playerbody.MovePosition(newPosition);
                }
            }
            else
            {
                cutscene = false;

                if (showText == true)
                {
                    if (PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 1")
                    {
                        cutsceneText.text = "Press Q to call over enemies towards you.";
                    }
                    else if (PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 2")
                    {
                        cutsceneText.text = "Press R to lay down an electric trap to stun enemies.";
                    }

                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        cutsceneText.text = "";
                        showText = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.R))
                    {
                        cutsceneText.text = "";
                        showText = false;
                    }
                }
            }
        }
    }
}

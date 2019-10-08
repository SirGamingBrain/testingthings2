using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialScripts : MonoBehaviour
{
    //We need to grab 4 things we shall section off by comments...
    // 1 - General Stuff
    float cutsceneTimer = 0f;

    public GameObject player;

    Scene scene;

    // 2 - UI Stuff
    public CanvasGroup cutsceneBars;
    public CanvasGroup playerText;

    float barsAlpha = 0f;
    float playerTextAlpha = 0f;

    public Text playerWords;
    public Text cutsceneText;

    bool showText = false;

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

    // Then we can setup this script since it will be the only time we use this script on the new tutorial scene...
    void Start()
    {
        cutsceneText.text = "";

        scene = SceneManager.GetActiveScene();

        if (PlayerPrefs.GetString("Last Checkpoint") == "new") {
            PlayerPrefs.SetString("Cutscene", "true");
            cutsceneBars.alpha = 1f;
        }
        else if (PlayerPrefs.GetString("Last Checkpoint") == "End")
        {
            PlayerPrefs.SetString("Last Checkpoint", "new");
            PlayerPrefs.SetString("Cutscene", "true");
            cutsceneBars.alpha = 1f;
        }
        else
        {
            PlayerPrefs.SetString("Cutscene", "false");
            cutsceneBars.alpha = 0f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerPrefs.GetString("Cutscene") == "true" && scene.name == "NewTutorial")
        {
            if (barsAlpha < 1f)
            {
                barsAlpha += Time.deltaTime * 2f;
                cutsceneBars.alpha = barsAlpha;
            }
            else if (barsAlpha > 1f)
            {
                barsAlpha = 1f;
                cutsceneBars.alpha = barsAlpha;
            }

            if (PlayerPrefs.GetString("Last Checkpoint") == "new")
            {
                if (cutsceneTimer >= 8f)
                {
                    cutsceneTimer = 0f;

                    PlayerPrefs.SetString("Cutscene", "false");
                    PlayerPrefs.SetString("Paused", "false");
                }
                else if (cutsceneTimer >= 4.6f)
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

                    newPlace = new Vector3(37f, 10f, -1f);

                    moveTime += Time.deltaTime;

                    if (moveTime > 2f)
                    {
                        moveTime = 2f;
                    }

                    float perc = moveTime / 2f;

                    cameraPlacement.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);
                }
                else if (cutsceneTimer >= 3.5f)
                {
                    if (playerTextAlpha < 0f)
                    {
                        playerTextAlpha = 0f;
                        playerText.alpha = playerTextAlpha;
                        playerWords.text = "Seems like these emergency supply zones are gonna be my ticket out of here...";
                    }
                    else if (playerTextAlpha <= 1f)
                    {
                        playerTextAlpha -= Time.deltaTime;
                        playerText.alpha = playerTextAlpha;
                        Debug.Log(playerTextAlpha);
                    }
                    

                    moveTime = 0f;
                }
                else if (cutsceneTimer >= 1.3f)
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
                else if (cutsceneTimer >= 1.1f)
                {
                    playerAnimations.SetBool("walking", false);

                    newRotation = Quaternion.Euler(0f, 180f, 0f);
                    playerModel.transform.rotation = Quaternion.RotateTowards(playerModel.transform.rotation, newRotation, 5f);
                    PlayerPrefs.SetString("Paused", "true");
                    moveTime = 0f;
                }
                else if (cutsceneTimer >= 0f)
                {
                    PlayerPrefs.SetString("Section Display", "true");
                    playerAnimations.SetBool("walking", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .008f * 8.3f);
                    playerbody.MovePosition(newPosition);
                }

                cutsceneTimer += Time.deltaTime;
            }
            else if (PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 1")
            {
                if (cutsceneTimer >= 5f)
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

                    newPlace = new Vector3(76f, 5f, 0f);

                    moveTime += Time.deltaTime;

                    if (moveTime > .5f)
                    {
                        moveTime = .5f;
                    }

                    float perc = moveTime / .5f;

                    cameraPlacement.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);

                    playerAnimations.SetBool("running", false);
                    
                    moveTime = 0f;
                    PlayerPrefs.SetString("Section Display", "true");
                }
                else if (cutsceneTimer >= 0f)
                {
                    playerWords.text = "I might be able to lure this thing away with a whistle by pressing Q...";

                    playerAnimations.SetBool("walking", false);
                    playerAnimations.SetBool("running", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .008f * 10f);
                    playerbody.MovePosition(newPosition);
                    PlayerPrefs.SetString("Paused", "true");
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
                    playerWords.text = "I hear a creature up ahead, I don't think I can run past it...";

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
            else if (PlayerPrefs.GetString("Last Checkpoint") == "End")
            {
                if (cutsceneTimer >= 0f)
                {
                    PlayerPrefs.SetString("Section Display", "false");
                    playerAnimations.SetBool("walking", false);
                    playerAnimations.SetBool("running", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .008f * 15f);
                    playerbody.MovePosition(newPosition);
                }
            }
        }
        else if (PlayerPrefs.GetString("Cutscene") == "true")
        {
            if (barsAlpha < 1f)
            {
                barsAlpha += Time.deltaTime * 2f;
                cutsceneBars.alpha = barsAlpha;
            }
            else if (barsAlpha > 1f)
            {
                barsAlpha = 1f;
                cutsceneBars.alpha = barsAlpha;
            }

            cutsceneTimer += Time.deltaTime;

            if (PlayerPrefs.GetString("Last Checkpoint") == "End")
            {
                if (cutsceneTimer >= 3f)
                {
                    cutsceneTimer = 0f;
                    PlayerPrefs.SetString("Cutscene", "false");
                    PlayerPrefs.SetString("Paused", "false");
                }
                else if (cutsceneTimer >= 0f)
                {
                    PlayerPrefs.SetString("Section Display", "true");
                    playerAnimations.SetBool("running", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .008f * 6.5f);
                    playerbody.MovePosition(newPosition);
                }
            }
        }
        else
        {
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
                PlayerPrefs.SetString("Section Display", "false");
            }

            if (showText == true)
            {
                if (PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 1")
                {
                    cutsceneText.text = "Press Q to distract enemies and call them over towards you by whistling.";
                }
                else if (PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 2")
                {
                    cutsceneText.text = "Press R to lay down an electric trap that damages and stuns enemies.";
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

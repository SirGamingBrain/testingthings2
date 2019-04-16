using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EntranceExit : MonoBehaviour
{
    //We need to grab 4 things we shall section off by comments...
    // 1 - General Stuff
    float cutsceneTimer = 0f;
    public GameObject player;

    Scene scene;

    // 2 - UI Stuff
    public CanvasGroup cutsceneBars;
    public CanvasGroup areaTitle;

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

        if (PlayerPrefs.GetString("Last Checkpoint") == "End")
        {
            PlayerPrefs.SetString("Last Checkpoint", "new");
        }

        if (PlayerPrefs.GetString("Last Checkpoint") == "new" && (scene.name == "1st Level" || scene.name == "2nd Level" || scene.name == "3rd Level"))
        {
            PlayerPrefs.SetString("Cutscene", "true");
            PlayerPrefs.SetString("Section Display", "true");
            cutsceneBars.alpha = 1f;
        }
        else
        {
            cutsceneBars.alpha = 0f;
            PlayerPrefs.SetString("Cutscene", "true");
            PlayerPrefs.SetString("Paused", "false");
            PlayerPrefs.SetString("Section Display", "true");
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetString("Cutscene") == "true" && PlayerPrefs.GetString("Last Checkpoint") == "new")
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

            if (PlayerPrefs.GetString("Last Checkpoint") == "new")
            {
                if (cutsceneTimer >= 2.5f)
                {
                    cutsceneTimer = 0f;
                    PlayerPrefs.SetString("Cutscene", "false");
                    PlayerPrefs.SetString("Paused", "false");
                    PlayerPrefs.Save();
                }
                else if (cutsceneTimer >= 0f)
                {
                    PlayerPrefs.SetString("Section Display", "true");
                    playerAnimations.SetBool("running", true);

                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    playerModel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    Vector3 newPosition = player.transform.position + (transform.right * .01f * 10f);
                    playerbody.MovePosition(newPosition);
                }
            }
        }
        else if (PlayerPrefs.GetString("Cutscene") == "true" && PlayerPrefs.GetString("Last Checkpoint") == "End")
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

            if (cutsceneTimer >= 4f)
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
                Vector3 newPosition = player.transform.position + (transform.right * .01f * 10f);
                playerbody.MovePosition(newPosition);
            }
        }
        else if (PlayerPrefs.GetString("Cutscene") == "true")
        {
            if (barsAlpha > 0f)
            {
                barsAlpha -= Time.deltaTime * 2f;
                cutsceneBars.alpha = barsAlpha;
            }
            else if (barsAlpha < 0f)
            {
                barsAlpha = 0f;
                cutsceneBars.alpha = barsAlpha;
                PlayerPrefs.SetString("Section Display", "true");
            }

            cutsceneTimer += Time.deltaTime;

            if (cutsceneTimer >= 1f)
            {
                cutsceneTimer = 0f;
                PlayerPrefs.SetString("Cutscene", "false");
                PlayerPrefs.SetString("Paused", "false");
            }
        }
        else
        {
            if (barsAlpha > 0f)
            {
                barsAlpha -= Time.deltaTime * 2f;
                cutsceneBars.alpha = barsAlpha;
            }
            else if (barsAlpha < 0f)
            {
                barsAlpha = 0f;
                cutsceneBars.alpha = barsAlpha;
                PlayerPrefs.SetString("Section Display", "false");
            }
        }
    }
}

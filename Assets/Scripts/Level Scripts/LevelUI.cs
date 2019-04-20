using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelUI : MonoBehaviour
{
    string checkpointName;
    string sectionName;

    int width = 0;
    int height = 0;
    int index = 0;
    int qualityNum = 0;

    float fadeAlpha = 0f;
    float UIAlpha = 0f;
    float textboxAlpha = 0f;
    float titleAlpha = 1f;
    float settingsAlpha = 0f;

    float masterLevel = 0f;

    bool tradeWindows = false;
    bool fadeUI = false;
    bool fadeUIIn = false;
    public bool fadeAll = false;
    bool fadeAllIn = false;
    bool fadeSettingsIn = false;

    bool firstLoad = false;
    bool backingOut = false;

    public Button continueGame;
    public Button settingsB;
    public Button exitB;
    public Button resolutionB;
    public Button fullscreenB;
    public Button qualityB;
    public Button applyB;

    public Text fullscreenText;
    public Text areaTitle;

    public Slider masterS;

    public CanvasGroup UIGroup;
    public CanvasGroup fadeGroup;
    public CanvasGroup SettingsWindow;
    public CanvasGroup textGroup;
    public CanvasGroup barsGroup;
    public CanvasGroup sectionTitle;

    public GameObject fadeObject;
    public GameObject mainWindow;
    public GameObject settingsWindow;

    public GameObject player;

    public AudioSource hover;
    public AudioSource select;
    public AudioSource audio;

    public AudioClip levelMusic;

    public AudioClip hovering;
    public AudioClip selecting;

    Scene scene;

    Resolution[] resolutions;
    string[] qualities;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        scene = SceneManager.GetActiveScene();

        if (PlayerPrefs.GetString("Screen Mode") == "true")
        {
            fullscreenB.GetComponentInChildren<Text>().text = "Fullscreen";
            Screen.fullScreen = true;
        }
        else
        {
            fullscreenB.GetComponentInChildren<Text>().text = "Windowed";
            Screen.fullScreen = false;
        }

        resolutions = Screen.resolutions;

        foreach (var res in resolutions)
        {
            if (res.width == PlayerPrefs.GetInt("Resolution"))
            {
                width = res.width;
                height = res.height;
            }
            index += 1;
        }

        if (height == 0)
        {
            index = resolutions.Length - 1;

            height = resolutions[index].height;
            width = resolutions[index].width;
        }

        resolutionB.GetComponentInChildren<Text>().text = (width + " x " + height);
        Screen.SetResolution(width, height, Screen.fullScreen);

        qualities = QualitySettings.names;

        foreach (string name in qualities)
        {

            if (qualityNum == PlayerPrefs.GetInt("Quality"))
            {
                QualitySettings.SetQualityLevel(qualityNum, true);
                qualityB.GetComponentInChildren<Text>().text = ("Level " + (qualityNum + 1));
                break;
            }
            qualityNum += 1;
        }

        if (qualityNum != PlayerPrefs.GetInt("Quality"))
        {
            PlayerPrefs.SetInt("Quality", qualityNum);
            QualitySettings.SetQualityLevel(qualityNum, true);
            qualityB.GetComponentInChildren<Text>().text = ("Level " + qualityNum);
        }

        //A whole lot of buttons and stuff that I will also have to add to the in-game script.
        SettingsWindow.alpha = 0f;
        resolutionB.enabled = false;
        fullscreenB.enabled = false;
        qualityB.enabled = false;
        applyB.enabled = false;
        masterS.enabled = false;

        settingsWindow.SetActive(false);

        UIGroup.alpha = 0f;
        continueGame.enabled = false;
        settingsB.enabled = false;
        exitB.enabled = false;

        mainWindow.SetActive(false);

        //We always set the in game scene to start out as black, since we're just going to fade into the game.
        fadeAlpha = 1f;
        fadeAllIn = true;
        fadeUIIn = false;

        masterLevel = PlayerPrefs.GetFloat("Master Volume");

        hover.volume = masterLevel;
        select.volume = masterLevel;
        audio.loop = true;

        audio.clip = levelMusic;

        audio.volume = masterLevel * 0f;

        audio.Play();

        sectionTitle.alpha = 0f;

        fadeGroup.alpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetString("Last Checkpoint") == "End")
        {
            fadeAll = true;
            PlayerPrefs.SetString("Cutscene", "true");
        }

        //This is just us slowly fading the black screen away from the player to reveal the opening animations.
        if (fadeAllIn == true)
        {
            if (player.GetComponent<PlayerController>().respawning == false) {

                fadeAlpha -= Time.deltaTime / 2f;
                fadeGroup.alpha = fadeAlpha;
                audio.volume = masterLevel * .8f * Mathf.Abs(1f - fadeAlpha);

                if (fadeAlpha <= 0f)
                {
                    fadeAllIn = false;
                    fadeGroup.alpha = 0;
                    audio.volume = masterLevel * .8f;
                    fadeObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Still respawning.");
                fadeAlpha -= Time.deltaTime / 2.5f;
                fadeGroup.alpha = fadeAlpha;

                if (fadeAlpha <= 0f)
                {
                    fadeAllIn = false;
                    fadeGroup.alpha = 0;
                    fadeObject.SetActive(false);
                    player.GetComponent<PlayerController>().respawning = false;
                }
            }
        }

        //We are transitioning out of the game.
        if (fadeAll == true)
        {
            if (player.GetComponent<PlayerController>().respawning == false) {
                fadeObject.SetActive(true);

                fadeAlpha += Time.deltaTime / 2f;
                fadeGroup.alpha = fadeAlpha;
                audio.volume = masterLevel * .8f * Mathf.Abs(1f - fadeAlpha);

                if (fadeAlpha > 1f)
                {
                    fadeGroup.alpha = 1f;
                    fadeAll = false;
                    audio.volume = masterLevel * 0f;
                    PlayerPrefs.Save();

                    if (backingOut == true)
                    {
                        StartCoroutine(LoadScene("Main Menu"));
                    }
                    else if (scene.name == "NewTutorial")
                    {
                        StartCoroutine(LoadScene("1st Level"));
                    }
                    else if (scene.name == "1st Level")
                    {
                        StartCoroutine(LoadScene("2nd Level"));
                    }
                    else if (scene.name == "2nd Level")
                    {
                        StartCoroutine(LoadScene("3rd Level"));
                    }
                    else if (scene.name == "3rd Level")
                    {
                        StartCoroutine(LoadScene("Main Menu"));
                    }
                }
            }
            else
            {
                Debug.Log("Beginning respawning.");
                fadeObject.SetActive(true);

                fadeAlpha += Time.deltaTime / 2.5f;
                fadeGroup.alpha = fadeAlpha;
                audio.volume = masterLevel * .8f * Mathf.Abs(1f - fadeAlpha);

                if (fadeAlpha > 1f)
                {
                    fadeGroup.alpha = 1f;
                    fadeAll = false;
                    player.GetComponent<PlayerController>().health = 1;
                    StartCoroutine(LoadScene(PlayerPrefs.GetString("Last Section")));
                }
            }
        }

        //We are closing the in game pause menu.
        if (fadeUI == true)
        {
            UIAlpha -= Time.deltaTime * 2f;
            UIGroup.alpha = UIAlpha;

            hover.volume = UIAlpha * masterLevel * .5f;
            select.volume = UIAlpha * masterLevel * .5f;

            if (UIAlpha <= 0f)
            {
                UIAlpha = 0f;
                hover.volume = UIAlpha * masterLevel * .5f;
                select.volume = UIAlpha * masterLevel * .5f;
                fadeUI = false;
            }
        }

        //We are opening the in game pause menu.
        if (fadeUIIn == true)
        {
            UIAlpha += Time.deltaTime * 2f;
            UIGroup.alpha = UIAlpha;
            hover.volume = UIAlpha * masterLevel * .5f;
            select.volume = UIAlpha * masterLevel * .5f;

            if (UIAlpha >= 1f)
            {
                UIAlpha = 1f;
                hover.volume = UIAlpha * masterLevel * .5f;
                select.volume = UIAlpha * masterLevel * .5f;
                fadeUIIn = false;
            }
        }

        if (tradeWindows == true && PlayerPrefs.GetString("Paused") == "true" && PlayerPrefs.GetString("Cutscene") == "false")
        {
            UIAlpha -= Time.deltaTime * 2f;
            UIGroup.alpha = UIAlpha;
            hover.volume = UIAlpha * masterLevel * .5f;
            select.volume = UIAlpha * masterLevel * .5f;

            if (UIAlpha <= 0f)
            {
                mainWindow.SetActive(false);
                UIGroup.alpha = 0f;
                UIAlpha = 0f;
                hover.volume = UIAlpha * masterLevel * .5f;
                select.volume = UIAlpha * masterLevel * .5f;

                settingsWindow.SetActive(true);

                settingsAlpha += Time.deltaTime * 2f;
                SettingsWindow.alpha = settingsAlpha;

                if (settingsAlpha >= 1f)
                {
                    settingsAlpha = 1f;
                    SettingsWindow.alpha = 1f;
                    resolutionB.enabled = true;
                    fullscreenB.enabled = true;
                    qualityB.enabled = true;
                    applyB.enabled = true;
                    masterS.enabled = true;
                }
            }
        }
        else if (tradeWindows == false && PlayerPrefs.GetString("Paused") == "true" && PlayerPrefs.GetString("Cutscene") == "false")
        {
            settingsAlpha -= Time.deltaTime * 2f;
            SettingsWindow.alpha = settingsAlpha;

            if (settingsAlpha <= 0f)
            {
                settingsWindow.SetActive(false);
                settingsAlpha = 0f;
                SettingsWindow.alpha = 0f;

                UIAlpha += Time.deltaTime * 2f;
                UIGroup.alpha = UIAlpha;
                hover.volume = UIAlpha * masterLevel * .5f;
                select.volume = UIAlpha * masterLevel * .5f;

                mainWindow.SetActive(true);

                if (UIAlpha >= 1f)
                {
                    UIAlpha = 1f;
                    hover.volume = UIAlpha * masterLevel * .5f;
                    select.volume = UIAlpha * masterLevel * .5f;
                    UIGroup.alpha = 1f;
                    continueGame.enabled = true;
                    settingsB.enabled = true;
                    exitB.enabled = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && PlayerPrefs.GetString("Cutscene") == "false" && PlayerPrefs.GetString("Paused") == "false")
        {
            PlayerPrefs.SetString("Paused", "true");
            PlayerPrefs.Save();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && PlayerPrefs.GetString("Cutscene") == "false" && PlayerPrefs.GetString("Paused") == "true" && tradeWindows == false)
        {
            PlayerPrefs.SetString("Paused", "false");
            PlayerPrefs.Save();
            continueGame.enabled = false;
            settingsB.enabled = false;
            exitB.enabled = false;
            fadeUI = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && PlayerPrefs.GetString("Cutscene") == "false" && PlayerPrefs.GetString("Paused") == "true" && tradeWindows == true)
        {
            PlayerPrefs.Save();
            fadeSettingsIn = false;
            resolutionB.enabled = false;
            fullscreenB.enabled = false;
            qualityB.enabled = false;
            applyB.enabled = false;
            masterS.enabled = false;

            tradeWindows = false;
        }

        if (PlayerPrefs.GetString("Section Display") == "true")
        {
            if (titleAlpha < 1f)
            {
                if (PlayerPrefs.GetString("Last Section") == "NewTutorial")
                {
                    sectionName = "Medical Ward";
                }
                else if (PlayerPrefs.GetString("Last Section") == "1st Level")
                {
                    sectionName = "Feeding Grounds";
                }
                else if (PlayerPrefs.GetString("Last Section") == "2nd Level")
                {
                    sectionName = "Heart of the Hive";
                }
                else if (PlayerPrefs.GetString("Last Section") == "3rd Level")
                {
                    sectionName = "Breeding Grounds";
                }

                if (PlayerPrefs.GetString("Last Checkpoint") == "new" && PlayerPrefs.GetString("Last Section") == "NewTutorial")
                {
                    checkpointName = "Cryo Pod";
                }
                else if (PlayerPrefs.GetString("Last Checkpoint") == "new")
                {
                    checkpointName = "Entrance";
                }
                else
                {
                    checkpointName = PlayerPrefs.GetString("Last Checkpoint");
                }

                areaTitle.text = (sectionName + " - " + checkpointName);
                titleAlpha += Time.deltaTime/3f;
                sectionTitle.alpha = titleAlpha;
            }
            else if (titleAlpha > 1f)
            {
                titleAlpha = 1f;
                sectionTitle.alpha = titleAlpha;
            }
        }
        else
        {
            if (titleAlpha > 0f)
            {
                titleAlpha -= Time.deltaTime/3f;
                sectionTitle.alpha = titleAlpha;
            }
            else if (titleAlpha < 0f)
            {
                titleAlpha = 0f;
                sectionTitle.alpha = titleAlpha;
            }
        }
    }

    public void Back()
    {
        PlayerPrefs.SetString("Paused", "false");
        PlayerPrefs.Save();

        fadeUI = true;
        continueGame.enabled = false;
        settingsB.enabled = false;
        exitB.enabled = false;
    }

    public void Settings()
    {
        tradeWindows = true;
        continueGame.enabled = false;
        settingsB.enabled = false;
        exitB.enabled = false;
    }

    public void changeFullscreen()
    {
            if (Screen.fullScreen == true)
            {
                fullscreenB.GetComponentInChildren<Text>().text = "Windowed";
                PlayerPrefs.SetString("Screen Mode", "false");
                Screen.fullScreen = false;
            }
            else
            {
                fullscreenB.GetComponentInChildren<Text>().text = "Fullscreen";
                PlayerPrefs.SetString("Screen Mode", "true");
                Screen.fullScreen = true;
            }

            Debug.Log(PlayerPrefs.GetString("Screen Mode"));
    }

    public void changeResolution()
    {
            if (index < (resolutions.Length - 1))
            {
                index += 1;
            }
            else
            {
                index = 0;
            }

            height = resolutions[index].height;
            width = resolutions[index].width;

            resolutionB.GetComponentInChildren<Text>().text = (width + " x " + height);
            //Screen.SetResolution(width, height, Screen.fullScreen);
            PlayerPrefs.SetString("Resolution", height.ToString());
    }

    public void changeQuality()
    {
        if (qualityNum < (qualities.Length - 1))
        {
            qualityNum += 1;
        }
        else
        {
            qualityNum = 0;
        }

        QualitySettings.SetQualityLevel(qualityNum, true);
        PlayerPrefs.SetInt("Quality", qualityNum);
        qualityB.GetComponentInChildren<Text>().text = ("Level " + (qualityNum + 1));
    }

    //Used to go back between the settings of the menu.
    public void SaveApply()
    {
        Screen.SetResolution(width, height, Screen.fullScreen);

        fadeSettingsIn = false;
        resolutionB.enabled = false;
        fullscreenB.enabled = false;
        qualityB.enabled = false;
        applyB.enabled = false;
        masterS.enabled = false;

        tradeWindows = false;

        PlayerPrefs.Save();
    }

    public void ExitGame()
    {
        PlayerPrefs.SetString("Paused", "false");
        PlayerPrefs.Save();
        fadeUI = true;
        fadeAll = true;
        backingOut = true;
    }

    public void HoverSound()
    {
        hover.clip = hovering;
            hover.Play();
    }

    public void clickSound()
    {
        select.clip = selecting;
            select.Play();
    }

    public void masterV(float value)
    {
        Debug.Log(masterS.value);
        masterLevel = value;
        PlayerPrefs.SetFloat("Master Volume", value);
        PlayerPrefs.Save();
        hover.volume = masterLevel * UIAlpha;
        select.volume = masterLevel * UIAlpha;
    }

    //This function handles loading the scene in
    IEnumerator LoadScene(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        if (firstLoad == false) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
            firstLoad = true;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        // Wait until the asynchronous scene fully loads
        
    }
}

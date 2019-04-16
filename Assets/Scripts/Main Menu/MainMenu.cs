﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Light light;

    int width = 0;
    int height = 0;
    int index = 0;
    int qualityNum = 0;

    float timeOut = 0f;
    float timeOn = 0f;
    float UIAlpha = 1f;
    float fadeAlpha = 0f;
    float settingsAlpha = 0f;

    float masterLevel = 0f;

    bool on = true;
    bool reload = false;
    bool fadeUI = false;
    bool fadeUIIn = false;
    bool fadeAll = false;
    bool fadeAllIn = false;
    bool fadeSettingsIn = false;
    bool tradeWindows = false;
    bool loadingScene = false;

    public Button newGame;
    public Button continueGame;
    public Button settingsB;
    public Button exitB;
    public Button resolutionBUp;
    public Button resolutionBDown;
    public Button fullscreenB;
    public Button qualityB;
    public Button applyB;

    public Text fullscreenText;
    public Text resolutionText;

    public Slider masterS;

    public CanvasGroup UI;
    public CanvasGroup Fade;
    public CanvasGroup SettingsWindow;

    public GameObject brutey;
    public GameObject fadeScreen;
    public GameObject mainWindow;
    public GameObject settingsWindow;

    public Rigidbody theBrute;

    public Animator brute;

    public AudioSource music;
    public AudioSource hover;
    public AudioSource select;

    public AudioClip mainmusic;
    public AudioClip hovering;
    public AudioClip selecting;

    Resolution[] resolutions;
    string[] qualities;

    float reversal = 0f;

    string window = "main";

    //The name will be stored here, and other values will be grabbed from player prefs in order to make sure everything works.
    string lastScene = "NewTutorial";

    // Start is called before the first frame update
    void Start()
    {
        fadeScreen.SetActive(false);

        //lastScene = PlayerPrefs.GetString("Last Section");

        if (!PlayerPrefs.HasKey("Screen Mode"))
        {
            PlayerPrefs.SetString("Screen Mode", "false");
        }

        if (!PlayerPrefs.HasKey("Resolution"))
        {
            PlayerPrefs.SetInt("Resolution", 1920);
        }

        if (!PlayerPrefs.HasKey("Quality"))
        {
            PlayerPrefs.SetInt("Quality", 5);
        }

        //We shall use these variables to hold the rest of the game to mark.
        if(PlayerPrefs.GetString("Screen Mode") == "true")
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
                break;
            }
            index += 1;
        }

        if (height == 0)
        {
            index = resolutions.Length - 1;

            height = resolutions[index].height;
            width = resolutions[index].width;
        }

        resolutionText.text = (width + " x " + height);
        Screen.SetResolution(width,height,Screen.fullScreen);

        qualities = QualitySettings.names;

        foreach (string name in qualities)
        {
            
            if (name == qualities[PlayerPrefs.GetInt("Quality")])
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
        resolutionBUp.enabled = false;
        fullscreenB.enabled = false;
        qualityB.enabled = false;
        applyB.enabled = false;
        masterS.enabled = false;

        settingsWindow.SetActive(false);

        newGame.enabled = true;
        continueGame.enabled = true;
        settingsB.enabled = true;
        exitB.enabled = true;

        mainWindow.SetActive(true);

        timeOut = generateTimeOut();
        timeOn = generateTimeOn();

        //Grab the values of differing files.
        //Video Options(Screen Resolution, Quality), Audio Options (Master Volume, Effect Volume), Controls (Controls Re-Bindable). 
        brute.SetBool("Walk", true);

        music.loop = true;

        music.clip = mainmusic;
        hover.clip = hovering;
        select.clip = selecting;

        masterLevel = PlayerPrefs.GetFloat("Master Volume");
        masterS.value = masterLevel;
        music.volume = masterLevel * .6f;
        hover.volume = masterLevel;
        select.volume = masterLevel;

        music.Play();
    }

    private void Update()
    {
        if (fadeUI == true)
        {
            //Debug.Log("Working as intended!");
            UIAlpha -= Time.deltaTime;
            UI.alpha = UIAlpha;
            music.volume = UIAlpha * masterLevel * .5f;
            hover.volume = UIAlpha * masterLevel * .5f;
            select.volume = UIAlpha * masterLevel * .5f;

            if (UIAlpha <= 0f)
            {
                fadeAll = true;
                fadeUI = false;
            }
        }

        if (fadeUIIn == true)
        {
            UIAlpha += Time.deltaTime;
            UI.alpha = UIAlpha;
            music.volume = UIAlpha * masterLevel * .5f;
            hover.volume = UIAlpha * masterLevel * .5f;
            select.volume = UIAlpha * masterLevel * .5f;

            if (UIAlpha >= 1f)
            {
                fadeUIIn = false;
                fadeScreen.SetActive(true);
            }
        }

        if (fadeAll == true)
        {
            //Debug.Log("Loading Scene");
            fadeAlpha += Time.deltaTime / 2f;
            Fade.alpha = fadeAlpha;

            if (fadeAlpha >= 1f)
            {
                fadeAll = false;
                StartCoroutine(LoadScene(PlayerPrefs.GetString("Last Section")));
            }
        }

        if (fadeAllIn == true)
        {
            fadeAlpha -= Time.deltaTime / 2f;
            Fade.alpha = fadeAlpha;

            if (fadeAlpha <= 0f)
            {
                fadeAllIn = false;
                Fade.alpha = 0;
                fadeUIIn = true;
            }
        }

        if (tradeWindows && loadingScene == false)
        {
            UIAlpha -= Time.deltaTime * 2f;
            UI.alpha = UIAlpha;

            if (UIAlpha <= 0f)
            {
                mainWindow.SetActive(false);
                UI.alpha = 0f;
                UIAlpha = 0f;

                settingsWindow.SetActive(true);

                settingsAlpha += Time.deltaTime * 2f;
                SettingsWindow.alpha = settingsAlpha;

                if (settingsAlpha >= 1f)
                {
                    settingsAlpha = 1f;
                    SettingsWindow.alpha = 1f;
                    resolutionBUp.enabled = true;
                    resolutionBDown.enabled = true;
                    fullscreenB.enabled = true;
                    qualityB.enabled = true;
                    applyB.enabled = true;
                    masterS.enabled = true;
                }
            }
        }
        else if (!tradeWindows && loadingScene == false)
        {
            settingsAlpha -= Time.deltaTime * 2f;
            SettingsWindow.alpha = settingsAlpha;

            if (settingsAlpha < 0f)
            {
                settingsAlpha = 0f;
                SettingsWindow.alpha = 0f;
                settingsWindow.SetActive(false);

                UIAlpha += Time.deltaTime * 2f;
                UI.alpha = UIAlpha;

                mainWindow.SetActive(true);

                if (UIAlpha > 1f)
                {
                    UIAlpha = 1f;
                    UI.alpha = 1f;
                    newGame.enabled = true;
                    continueGame.enabled = true;
                    settingsB.enabled = true;
                    exitB.enabled = true;
                }
            }
        }

        reversal += Time.deltaTime/3f;

        if (reversal < 10f) {
            Vector3 newPosition = theBrute.transform.position + (transform.right * .02f);
            theBrute.MovePosition(newPosition);

            if (reversal < 10f && reversal > 9f)
            {
                brutey.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            }
        }
        else if (reversal < 20f)
        {
            Vector3 newPosition = theBrute.transform.position - (transform.right * .02f);
            theBrute.MovePosition(newPosition);
        }
        else
        {
            reversal = 0f;
            brutey.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        if (on)
        {
            timeOn -= Time.deltaTime;

            if (timeOn > 0f)
            {
                light.enabled = true;
            }
            else
            {
                on = false;
            }
        }
        else
        {
            if (timeOut > 0f)
            {
                timeOut -= Time.deltaTime;
                light.enabled = false;
            }
            else
            {
                reload = true;
            }
        }

        if (reload)
        {
            timeOut = generateTimeOut();
            timeOn = generateTimeOn();

            on = true;
            reload = false;
        }
    }

    float generateTimeOut()
    {
        return Random.Range(0f, 2f);
    }

    float generateTimeOn()
    {
        return Random.Range(.1f, .35f);
    }

    public void NewGame()
    {
        fadeUI = true;
        PlayerPrefs.SetString("Last Section", "NewTutorial");
        PlayerPrefs.SetString("Last Checkpoint", "new");
        loadingScene = true;
        PlayerPrefs.Save();
        fadeScreen.SetActive(true);
    }

    public void Continue()
    {
        fadeUI = true;
        loadingScene = true;
        fadeScreen.SetActive(true);
    }

    public void Settings()
    {
        tradeWindows = true;
        newGame.enabled = false;
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

    public void changeResolutionUp()
    {
        if (index < (resolutions.Length - 1))
        {
            index ++;
        }
        else
        {
            index = 0;
        }

        height = resolutions[index].height;
        width = resolutions[index].width;

        resolutionText.text = (resolutions[index].width + " x " + resolutions[index].height);
        //Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        //PlayerPrefs.SetInt("Resolution", resolutions[index].width);
    }

    public void changeResolutionDown()
    {
        if (index > 0)
        {
            index --;
        }
        else
        {
            index = resolutions.Length - 1;
        }

        //height = resolutions[index].height;
        //width = resolutions[index].width;

        resolutionText.text = (resolutions[index].width + " x " + resolutions[index].height);
        //Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        //PlayerPrefs.SetInt("Resolution", resolutions[index].width);
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
        //Screen.SetResolution(width, height, Screen.fullScreen);

        fadeSettingsIn = false;
        resolutionBUp.enabled = false;
        resolutionBDown.enabled = false;
        fullscreenB.enabled = false;
        qualityB.enabled = false;
        applyB.enabled = false;
        masterS.enabled = false;

        tradeWindows = false;

        PlayerPrefs.SetInt("Resolution", resolutions[index].width);

        if (PlayerPrefs.GetString("Screen Mode") == "true")
        {
            Screen.SetResolution(resolutions[index].width, resolutions[index].height, true);
        }
        else
        {
            Screen.SetResolution(resolutions[index].width, resolutions[index].height, false);
        }

        PlayerPrefs.Save();
    }

    public void ExitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    public void HoverSound()
    {
        hover.Play();
    }

    public void ClickSound()
    {
        select.Play();
    }

    public void MasterV(float value)
    {
        Debug.Log(masterS.value);
        masterLevel = masterS.value;
        PlayerPrefs.SetFloat("Master Volume", masterS.value);
        PlayerPrefs.Save();
        music.volume = masterLevel * .6f;
        hover.volume = masterLevel;
        select.volume = masterLevel;
    }

    IEnumerator LoadScene(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}

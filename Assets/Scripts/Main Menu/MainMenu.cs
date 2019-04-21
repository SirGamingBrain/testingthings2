using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public new Light light;

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
    bool tradeWindows = false;
    bool loadingScene = false;

    public Button newGame;
    public Button continueGame;
    public Button settingsB;
    public Button exitB;
    public Button resolutionB;
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

    readonly int[] heights = new int[] {450, 576, 720, 768, 900, 1080, 1440};
    readonly int[] widths = new int[] {800, 1024, 1280, 1366, 1600, 1920, 2560};

    string[] qualities;

    float reversal = 0f;

    //The name will be stored here, and other values will be grabbed from player prefs in order to make sure everything works.
    string lastScene = "NewTutorial";

    // Start is called before the first frame update
    void Start()
    {
        fadeScreen.SetActive(false);

        lastScene = PlayerPrefs.GetString("Last Section");

        if (!PlayerPrefs.HasKey("Screen Mode"))
        {
            PlayerPrefs.SetString("Screen Mode", "false");
        }

        if (!PlayerPrefs.HasKey("Resolution"))
        {
            PlayerPrefs.SetInt("Resolution", 1080);
        }

        if (!PlayerPrefs.HasKey("Quality"))
        {
            PlayerPrefs.SetInt("Quality", 5);
        }

        if (!PlayerPrefs.HasKey("Master Volume"))
        {
            PlayerPrefs.SetFloat("Master Volume", 1f);
        }

        PlayerPrefs.Save();

        //We set the screen to either fullscreen or windowed based on the player's settings.
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

        //We set the resolution of the game based on the player's settings.
        foreach (int resolution in heights)
        {
            Debug.Log(resolution);
            if (resolution == PlayerPrefs.GetInt("Resolution"))
            {
                width = widths[index];
                height = resolution;
                break;
            }

            index += 1;
        }

        if (height == 0)
        {
            index = 5;

            height = heights[index];
            width = widths[index];
        }

        resolutionText.text = (width + " x " + height);
        Screen.SetResolution(width,height,Screen.fullScreen);

        //We set the quality of the game based on the player's settings.
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

        //These are all of the buttons, and their respective alphas being set to the correct starting positions.
        SettingsWindow.alpha = 0f;
        resolutionB.enabled = false;
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

        //This little thing here handles the background light in the scene.
        timeOut = GenerateTimeOut();
        timeOn = GenerateTimeOn();

        //We tell the background brute to walk menacingly.
        brute.SetBool("Walk", true);

        //Audio settings being laid out..
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
        //Skip to the next comment as the lines below deal with fading out of the scene, or fading the UI;
        if (fadeUI == true)
        {
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
                    resolutionB.enabled = true;
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
        // We gucci now.

        //Runtime code that handles moving the brute at a fixed speed.
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

        //Runtime code that handles the blinking light in the background.
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
            timeOut = GenerateTimeOut();
            timeOn = GenerateTimeOn();

            on = true;
            reload = false;
        }
    }

    //These two generate time functions generate and random interval for the light to be active or deactivated.
    float GenerateTimeOut()
    {
        return Random.Range(0f, 2f);
    }

    float GenerateTimeOn()
    {
        return Random.Range(.1f, .35f);
    }

    //Button that handles creating a new session for the player to play through.
    public void NewGame()
    {
        fadeUI = true;
        PlayerPrefs.SetString("Last Section", "NewTutorial");
        PlayerPrefs.SetString("Last Checkpoint", "new");
        loadingScene = true;
        PlayerPrefs.Save();
        fadeScreen.SetActive(true);
    }

    //Button that handles loading the previous area the player last left out on.
    public void Continue()
    {
        fadeUI = true;
        loadingScene = true;
        fadeScreen.SetActive(true);
    }

    //Button that tells the UI to swap the two windows.
    public void Settings()
    {
        tradeWindows = true;
        newGame.enabled = false;
        continueGame.enabled = false;
        settingsB.enabled = false;
        exitB.enabled = false;
    }

    //Button that swaps between fullscreen and windowed mode.
    public void ChangeFullscreen()
    {
        if (Screen.fullScreen == true)
        {
            fullscreenB.GetComponentInChildren<Text>().text = "Windowed";
            PlayerPrefs.SetString("Screen Mode", "false");
            Screen.fullScreen = false;
            Screen.SetResolution(width, height, false);
        }
        else
        {
            fullscreenB.GetComponentInChildren<Text>().text = "Fullscreen";
            PlayerPrefs.SetString("Screen Mode", "true");
            Screen.fullScreen = true;
            Screen.SetResolution(width, height, true);
        }

        Debug.Log(PlayerPrefs.GetString("Screen Mode"));
    }

    //Button to change the resolution.
    public void ChangeResolution()
    {
        if (index < 6)
        {
            index += 1;
        }
        else
        {
            index = 0;
        }

        height = heights[index];
        width = widths[index];

        resolutionText.text = (width + " x " + height);
        PlayerPrefs.SetInt("Resolution", height);
    }

    //Button to change the quality of the game.
    public void ChangeQuality()
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

    //Button that saves the current settings and return the player to the main menu window.
    public void SaveApply()
    {
        resolutionB.enabled = false;
        fullscreenB.enabled = false;
        qualityB.enabled = false;
        applyB.enabled = false;
        masterS.enabled = false;

        tradeWindows = false;

        PlayerPrefs.SetInt("Resolution", height);

        if (PlayerPrefs.GetString("Screen Mode") == "true")
        {
            Screen.SetResolution(width, height, true);
        }
        else
        {
            Screen.SetResolution(width, height, false);
        }

        PlayerPrefs.Save();
    }

    //Button that quits the game.
    public void ExitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    //Script that handles when someone hovers over a button.
    public void HoverSound()
    {
        hover.Play();
    }

    //Script that handles when someone clicks on screen.
    public void ClickSound()
    {
        select.Play();
    }

    //Slider that changes the master volume of the game.
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

    //Script that loads the correct scene, called when the game has faded out.
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

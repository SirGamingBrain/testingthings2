using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerPrefs.SetString("Last Section", "1st Level");
            PlayerPrefs.SetString("Last Checkpoint", "new");
            SceneManager.LoadScene("1st Level");
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerPrefs.SetString("Last Section", "2nd Level");
            PlayerPrefs.SetString("Last Checkpoint", "new");
            SceneManager.LoadScene("2nd Level");
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerPrefs.SetString("Last Section", "1st Level");
            PlayerPrefs.SetString("Last Checkpoint", "Checkpoint 2");
            PlayerPrefs.Save();
            SceneManager.LoadScene("1st Level");
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("Main Menu");
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerPrefs.SetString("Last Section", "NewTutorial");
            PlayerPrefs.SetString("Last Checkpoint", "new");
            SceneManager.LoadScene("NewTutorial");
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            PlayerPrefs.SetString("Last Section", "NewTutorial");
            PlayerPrefs.SetString("Last Checkpoint", "Checkpoint 1");
            SceneManager.LoadScene("NewTutorial");
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerPrefs.SetString("Last Section", "NewTutorial");
            PlayerPrefs.SetString("Last Checkpoint", "Checkpoint 2");
            SceneManager.LoadScene("NewTutorial");
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefs.SetString("Last Section", "2nd Level");
            PlayerPrefs.SetString("Last Checkpoint", "Checkpoint 3");
            SceneManager.LoadScene("2nd Level");
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerPrefs.SetString("Last Section", "1st Level");
            PlayerPrefs.SetString("Last Checkpoint", "Checkpoint ?");
            SceneManager.LoadScene("1st Level");
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.SetString("Last Section", "2nd Level");
            PlayerPrefs.SetString("Last Checkpoint", "Checkpoint ?");
            SceneManager.LoadScene("2nd Level");
        }
    }
}

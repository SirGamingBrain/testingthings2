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
        else if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}

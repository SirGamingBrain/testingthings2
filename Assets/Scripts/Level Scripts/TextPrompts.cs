using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextPrompts : MonoBehaviour
{
    private Scene scene;

    public Text playerWords;
    public CanvasGroup playerText;
    public float textTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerText.alpha = 1f;
        textTimer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        textTimer += Time.deltaTime;
        if (scene.name == "1st Level")
        {
            playerText.alpha = 1f;
            playerWords.text = "The taser broke during the run; I'll need special supplies to fix it.";
            
        }

        if(textTimer > 5f)
        {
            playerText.alpha = 0f;
            playerWords.text = "";
        }

        
    }
}

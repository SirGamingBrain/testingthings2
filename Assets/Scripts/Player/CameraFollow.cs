﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    private Vector3 newPlace;
    private Vector3 oldPlace;

    private float cameraHeight = 10f;

    private float moveTime;

    private float waitTime = 0f;

    bool moving = false;
    bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        //We set the camera to start where the player starts.
        this.transform.position = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - 6.5f);

        if (PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 1" || PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 2" || PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 3" || PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 4" || PlayerPrefs.GetString("Last Checkpoint") == "Checkpoint 5")
        {
            waitTime = 1.6f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If the intro cutscene shouldn't play, then the camera is set to follow the players commands.
        //Or you haven't paused the game, don't worry about the code atm.
        if ((PlayerPrefs.GetString("Cutscene") == "true" && PlayerPrefs.GetString("Paused") == "true"))
        {
            waitTime = 1.6f;
            Debug.Log("I'm doing nothing!");
        }
        else if (PlayerPrefs.GetString("Cutscene") == "false" && PlayerPrefs.GetString("Paused") == "false") {

            oldPlace = this.transform.position;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                moving = true;
                waitTime = 0f;
            }
            else
            {
                moving = false;
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
            {
                turning = true;

                if (turning == true)
                {
                    moveTime = 0f;
                    turning = false;
                }
            }
            else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)) && moveTime <= .5f)
            {
                turning = true;

                if (turning == true)
                {
                    moveTime = 1f;
                    turning = false;
                }
            }

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                if (moving == false)
                {
                    moveTime = 0f;
                }
                newPlace = new Vector3(player.transform.position.x - 1.5f, cameraHeight, player.transform.position.z - 5f);
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                if (moving == false)
                {
                    moveTime = 0f;
                }
                newPlace = new Vector3(player.transform.position.x + 1.5f, cameraHeight, player.transform.position.z - 5f);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                if (moving == false)
                {
                    moveTime = 0f;
                }
                newPlace = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - 5f);
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                if (moving == false)
                {
                    moveTime = 0f;
                }
                newPlace = new Vector3(player.transform.position.x - 1.5f, cameraHeight, player.transform.position.z - 8f);
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                if (moving == false)
                {
                    moveTime = 0f;
                }
                newPlace = new Vector3(player.transform.position.x + 1.5f, cameraHeight, player.transform.position.z - 8f);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (moving == false)
                {
                    moveTime = 0f;
                }
                newPlace = new Vector3(player.transform.position.x - 1.5f, cameraHeight, player.transform.position.z - 6.5f);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (moving == false)
                {
                    moveTime = 0f;
                }
                newPlace = new Vector3(player.transform.position.x + 1.5f, cameraHeight, player.transform.position.z - 6.5f);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (moving == false)
                {
                    moveTime = 0f;
                }
                newPlace = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - 8f);
            }
            else
            {
                if (waitTime < 1.5f)
                {
                    waitTime += Time.deltaTime;
                }
                else if (waitTime > 1.5f)
                {
                    moveTime = 0f;

                    waitTime = 1.5f;
                    newPlace = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - 6.5f);
                }
            }

            moveTime += Time.deltaTime;

            if (moveTime > 3f)
            {
                moveTime = 3f;
            }

            float perc = moveTime / 3f;

            this.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);
        }
        else if (PlayerPrefs.GetString("Cutscene") == "false" && PlayerPrefs.GetString("Paused") == "true")
        {
            oldPlace = this.transform.position;

            newPlace = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - 6.5f);

            moveTime += Time.deltaTime;

            if (moveTime > 3f)
            {
                moveTime = 3f;
            }

            float perc = moveTime / 3f;

            this.transform.position = Vector3.Lerp(oldPlace, newPlace, perc);
        }
        else
        {

        }
    }
}

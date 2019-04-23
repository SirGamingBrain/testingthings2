using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject scriptHolder;
    VariableHolder variableScript;

    public GameObject player;
    private Vector3 newPlace;
    private Vector3 oldPlace;

    readonly private float cameraHeight = 10f;

    private float moveTime;

    private float waitTime = 0f;

    bool moving = false;
    bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        scriptHolder = GameObject.Find("Level Scripts");
        variableScript = scriptHolder.GetComponent<VariableHolder>();

        //We set the camera to start where the player starts.
        this.transform.position = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - 6.5f);
    }

    // Update is called once per frame
    void Update()
    {
        //Here we tell the camera to follow the player at all times when a cutscene isn't playing and we aren't paused. If we are paused the camera moves back to being on top of the player, and if we are in a cutscene we take away the control of the camera completely.
        if (variableScript.cutscene == false && variableScript.paused == false) {

            oldPlace = this.transform.position;

            if (newPlace == new Vector3(0f, 0f, 0f))
            {
                newPlace = oldPlace;
            }

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
        else if (variableScript.cutscene == false && variableScript.paused == true)
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
        else if (variableScript.cutscene == true)
        {
            waitTime = 1.6f;
        }
    }
}

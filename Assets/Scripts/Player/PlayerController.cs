﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //A crapton of objects I'm not even gonna worry about commenting for now.
    public string sceneName = "null";
    string checkpointName = "null";
    string sectionName = "null";

    public int health = 1;
    private int traps = 0;

    private float gunCounter = 0f;
    private float speed = 0f;
    private float maxSpeed = 10f;
    readonly private float bulletSpeed = 1000f;

    private bool hasTaser = false;
    private bool hasTraps = false;
    public bool sneaking = false;
    private bool hasFired = false;
    private bool drawGun = false;
    private bool fireGun = false;

    public bool cutscenePlaying = false;
    public bool respawning = false;

    private Scene scene;

    public Quaternion newRotation;

    public AudioSource playerAudio;

    public AudioClip whistle;
    public AudioClip tasing;

    public GameObject gun;
    public GameObject barrel;
    public GameObject bullet;
    public GameObject player;
    public GameObject UIScriptHolder;

    VariableHolder variableScript;

    public GameObject trap;
    public GameObject trapSpot;
    public GameObject currentTile;

    public Animator gunAnimation;
    public Animator playerAnimations;

    public bool whistleCooldown = false;
    float lastWhistle = 0.0f;

    private Rigidbody rb;

    public GameObject spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        //We find the script that contains all of the variables we need.
        UIScriptHolder = GameObject.Find("Level Scripts");
        variableScript = UIScriptHolder.GetComponent<VariableHolder>();

        //We grab the rigidbody component of the player.
        rb = GetComponent<Rigidbody>();

        //We grab the current scene and set it's name to a string holder. Same for the last checkpoint and the last level.
        scene = SceneManager.GetActiveScene();

        sceneName = scene.name;

        sectionName = PlayerPrefs.GetString("Last Section");
        
        checkpointName = PlayerPrefs.GetString("Last Checkpoint");

        //Here we tell the player that if the previous checkpoint was at the end of a level or the beginning of one we ensure that they are dropped at the beginning of the level. Otherwise we just need to spawn them at the previos checkpoint.
        //Also the taser is unlocked for the player at the third checkpoint.
        if (checkpointName == "new" || checkpointName == "End")
        {
            spawnPoint = GameObject.Find("new");
            variableScript.displaySection = true;
        }
        else if ((checkpointName == "Checkpoint 3" || checkpointName == "Checkpoint 4") && sceneName == "2nd Level")
        {
            spawnPoint = GameObject.Find(checkpointName);
            variableScript.displaySection = true;
            hasTaser = true;
        }
        else
        {
            spawnPoint = GameObject.Find(checkpointName);
            variableScript.displaySection = true;
        }

        //If they've made it to the third level then they have the taser.
        if (scene.name == "3rd Level")
        {
            hasTaser = true;
        }

        //We set the spawn point of the player.
        this.transform.position = spawnPoint.transform.position;

        //We set the rotation of the player to be facing towards the camera at spawn for ease of access.
        player.transform.rotation = Quaternion.Euler(0f,90f,0f);

        //Some debug information to tell us the scene's name, and the checkpoint we spawned at.
        Debug.Log("Actual Name:" + sceneName + ", Spawnpoint: " + checkpointName);
    }

    // Update is called once per frame
    void Update()
    {
        //We will only allow the player to perform their abilities if they aren't paused or a cutscene isn't playing.
        if (variableScript.cutscene == false && variableScript.paused == false) {
            
            //The player has basic movement in 8 directions.
            //Up, Down, Left, Right, Up-Left, Down-Left, Up-Right, Down-Right.
            //This script handles the basic controls of the player and their tools.
            if ((drawGun == false && fireGun == false) && health > 0) {

                if (respawning == true)
                {
                    spawnPoint = GameObject.Find(PlayerPrefs.GetString("Last Checkpoint"));
                    this.transform.position = spawnPoint.transform.position;
                    playerAnimations.SetBool("isdead", false);
                }

                //This script handles whether or not the player is walking, running, or being idle. (Soon to be crouching)
                if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && Input.GetKey(KeyCode.LeftShift))
                {
                    playerAnimations.SetBool("running", true);
                }
                else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    playerAnimations.SetBool("running", false);
                    playerAnimations.SetBool("walking", true);
                }
                else
                {
                    playerAnimations.SetBool("walking", false);
                    playerAnimations.SetBool("running", false);
                }

                //These key's are used to speed up and slow down the player.
                //The balance of unlimited sprint is traded off for even less speed rather than a real dash.
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    maxSpeed = 15f;
                    sneaking = false;
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    maxSpeed = 4f;
                    sneaking = true;
                }
                else
                {
                    maxSpeed = 6.5f;
                    sneaking = true;
                }
                
                //This handles the build up of speed for the player, as well as the slow down (might have to get rid of).
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    if (speed < maxSpeed)
                    {
                        speed += Time.deltaTime * 25f;
                    }
                    else if (Input.GetKey(KeyCode.LeftShift) && speed > maxSpeed)
                    {
                        speed = maxSpeed;
                    }
                    else if (speed > maxSpeed)
                    {
                        speed = maxSpeed;
                    }
                }
                else
                {
                    if (speed > 0f)
                    {
                        speed -= Time.deltaTime * 50f;
                    }
                    else if (speed < 0f)
                    {
                        speed = 0f;
                    }
                }

                //Establish a new position variable which we set depending on where the player is going.
                Vector3 newPosition;

                //This area handles the rotation and the new position of wherever the player is moving to at a fixed speed.
                if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
                {
                    newRotation = Quaternion.Euler(0f, -45f, 0f);
                    newPosition = transform.position + (Vector3.Normalize(transform.forward - transform.right) * .008f * speed);
                    rb.MovePosition(newPosition);
                }
                else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
                {
                    newRotation = Quaternion.Euler(0f, 45f, 0f);
                    newPosition = transform.position + (Vector3.Normalize(transform.forward + transform.right) * .008f * speed);
                    rb.MovePosition(newPosition);
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    newRotation = Quaternion.Euler(0f, 0f, 0f);
                    newPosition = transform.position + (transform.forward * .008f * speed);
                    rb.MovePosition(newPosition);
                }
                else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
                {
                    newRotation = Quaternion.Euler(0f, -135f, 0f);
                    newPosition = transform.position + (Vector3.Normalize(-transform.forward - transform.right) * .008f * speed);
                    rb.MovePosition(newPosition);
                }
                else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
                {
                    newRotation = Quaternion.Euler(0f, 135f, 0f);
                    newPosition = transform.position + (Vector3.Normalize(-transform.forward + transform.right) * .008f * speed);
                    rb.MovePosition(newPosition);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    newRotation = Quaternion.Euler(0f, -90f, 0f);
                    newPosition = transform.position - (transform.right * .008f * speed);
                    rb.MovePosition(newPosition);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    newRotation = Quaternion.Euler(0f, 90f, 0f);
                    newPosition = transform.position + (transform.right * .008f * speed);
                    rb.MovePosition(newPosition);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    newRotation = Quaternion.Euler(0f, 180f, 0f);
                    newPosition = transform.position - (transform.forward * .008f * speed);
                    rb.MovePosition(newPosition);
                }

                //We then update the rotation and movment here based off of where were rotating and moving to and from.
                player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, newRotation, 5f);
            }
            else if (health <= 0)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;

                playerAnimations.SetBool("isdead", true);
                playerAnimations.SetBool("walking", false);
                playerAnimations.SetBool("running", false);

                respawning = true;

                UIScriptHolder.GetComponent<LevelUI>().fadeAll = true;
                variableScript.paused = true;
            }
            else
            {
                playerAnimations.SetBool("walking", false);
                playerAnimations.SetBool("running", false);
                playerAnimations.SetBool("isdead", false);

                if (speed > 0f)
                {
                    speed -= Time.deltaTime * 50f;
                }
                else if (speed < 0f)
                {
                    speed = 0f;
                }
            }

            //The player presses Q to whistle
            //For a brief moment, enemies within a certain range of the player will be alerted to his/her position.
            if (Input.GetKeyDown(KeyCode.Q) && whistleCooldown == false)
            {
                playerAudio.clip = whistle;
                playerAudio.Play();
                whistleCooldown = true;
                lastWhistle = 2f;
            }

            //This statement puts the whistle on a 2 second cooldown, once finished it allows the user to whistle again.
            if (lastWhistle <= 0)
            {
                whistleCooldown = false;
            }
            else
            {
                lastWhistle -= Time.deltaTime;
            }

            //The player presses space to use their taser, assuming they haven't already fired it once and they actually have the taser.
            if (Input.GetKeyDown(KeyCode.Space) && hasTaser == true && hasFired == false)
            {
                drawGun = true;
            }

            //These next two sections deal with handling the animations of firing.
            if (drawGun == true && speed == 0f)
            {
                playerAnimations.SetBool("firing", true);
                gunAnimation.SetTrigger("Fire");

                if (gunCounter >= 60)
                {
                    drawGun = false;
                    fireGun = true;
                    gunCounter = 0;
                }
                else
                {
                    gunCounter += 1.1f;
                }
            }

            if (fireGun == true)
            {
                playerAudio.clip = tasing;
                playerAudio.loop = false;
                playerAudio.Play();

                GameObject temporaryBullet;
                temporaryBullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation) as GameObject;
                //Bullet travels a set range before dissapearing (Might have a set time, would be easier to track)

                Rigidbody temporaryRigidbody;
                temporaryRigidbody = temporaryBullet.GetComponent<Rigidbody>();

                temporaryRigidbody.AddForce(player.transform.forward * bulletSpeed);

                Destroy(temporaryBullet, .5f);

                fireGun = false;
                hasFired = true;
                gunAnimation.ResetTrigger("Fire");
                playerAnimations.SetBool("firing", false);
            }

            //If the player presses R then they will place down a trap.
            if (Input.GetKeyDown(KeyCode.R) && hasTraps == true)
            {
                traps -= 1;
                GameObject temporaryTrap;

                temporaryTrap = Instantiate(trap, new Vector3(currentTile.transform.position.x, trapSpot.transform.position.y, currentTile.transform.position.z), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
                //Empty is on the ground underneath the player.
                //When player attempts to place a trap, it locates the center of the most recent tile stepped on by the player.
                //Places trap on top of that tile.
                //After 10 seconds, it gets destroyed (Leaving this in for testing purposes, but could become an actual feature).
                // Explain it away as having a limited self storage battery.
                Destroy(temporaryTrap, 20f);

                if (traps == 0)
                {
                    hasTraps = false;
                }
            }
        }
        else if (variableScript.cutscene == false && variableScript.paused == true)
        {
            playerAnimations.SetBool("walking", false);
            playerAnimations.SetBool("running", false);
        }
    }

    //This script handles all of the possible things the player will run into when running past, into, or through certain objects.
    private void OnTriggerEnter(Collider other)
    {
        //If the object is tagged as a refill, we check for a few things. No matter what we always give the player back their trap, and we set the last known checkpoint to be the thing they just walked through.
        //We also check to see if they have a taser, if they do we give them their shot back so that they can fire again. Finally we set the section display to be faded in for the player.
        if (other.gameObject.tag == "refill")
        {
            traps = 1;
            hasTraps = true;

            if (hasTaser)
            {
                hasFired = false;
            }

            PlayerPrefs.SetString("Last Checkpoint", other.gameObject.name);
            variableScript.displaySection = true;
        }
        else if (other.gameObject.tag == "lock")
        {
            variableScript.displaySection = false;

            if (scene.name == "NewTutorial")
            {
                variableScript.cutscene = true;
            }

            if (other.gameObject.name == "End")
            {
                PlayerPrefs.SetString("Last Checkpoint", "End");
                PlayerPrefs.Save();
            }
        }

        //This script checks to see if the stuff we are walking over contains the name ground and if it does we set the position of where we will drop a trap to be on the last tile we just entered.
        if (other.name.Contains("Ground"))
        {
            currentTile = other.gameObject;
        }
    }
}

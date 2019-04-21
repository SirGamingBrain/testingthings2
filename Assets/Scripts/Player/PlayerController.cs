using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
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
        UIScriptHolder = GameObject.Find("Level Scripts");

        PlayerPrefs.SetString("Paused", "false");

        rb = GetComponent<Rigidbody>();

        scene = SceneManager.GetActiveScene();

        sceneName = scene.name;

        sectionName = PlayerPrefs.GetString("Last Section");
        
        checkpointName = PlayerPrefs.GetString("Last Checkpoint");

        Debug.Log(checkpointName);

        if (checkpointName == "new")
        {
            spawnPoint = GameObject.Find("new");
            PlayerPrefs.SetString("Paused", "true");
            PlayerPrefs.SetString("Section Display", "true");
        }
        else if (checkpointName == "End")
        {
            spawnPoint = GameObject.Find("new");
            PlayerPrefs.SetString("Paused", "true");
            PlayerPrefs.SetString("Section Display", "true");
        }
        else
        {
            spawnPoint = GameObject.Find(checkpointName);
            PlayerPrefs.SetString("Section Display", "true");
        }

        this.transform.position = spawnPoint.transform.position;

        player.transform.rotation = Quaternion.Euler(0f,90f,0f);

        Debug.Log(sceneName + sectionName + checkpointName);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetString("Cutscene") == "false" && PlayerPrefs.GetString("Paused") == "false") {
            //The player has basic movement in 8 directions.
            //Up, Down, Left, Right, Up-Left, Down-Left, Up-Right, Down-Right.
            //Handles the basic controls of the player and their tools.
            if ((drawGun == false && fireGun == false) && health > 0) {

                if (respawning == true)
                {
                    spawnPoint = GameObject.Find(PlayerPrefs.GetString("Last Checkpoint"));
                    this.transform.position = spawnPoint.transform.position;
                    playerAnimations.SetBool("isdead", false);
                }

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

                //Editing the sneaking state here
                //Otherwise the player could be detected even if standing still
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
                
                //This handles the build up of speed for the player, as well as the slow down.
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

                Vector3 newPosition;

                //This area handles the rotation of the player by changing euler angles.
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

                if (Input.GetKeyDown(KeyCode.M))
                {
                    health = 0;
                }

                //We then update the rotation and movment here based off of where were rotating and moving to and from.
                player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, newRotation, 5f);
            }
            else if (health <= 0)
            {
                playerAnimations.SetBool("isdead", true);
                playerAnimations.SetBool("walking", false);
                playerAnimations.SetBool("running", false);
                

                respawning = true;

                UIScriptHolder.GetComponent<LevelUI>().fadeAll = true;
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
            //For a limited amount of time, enemies within the a certain range of the player will be alerted to his/her position.
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

            if (Input.GetKeyDown(KeyCode.Space) && hasTaser == true && hasFired == false)
            {
                drawGun = true;
            }

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

            if (Input.GetKeyDown(KeyCode.R) && hasTraps == true)
            {
                traps -= 1;
                GameObject temporaryTrap;

                temporaryTrap = Instantiate(trap, new Vector3(currentTile.transform.position.x, trapSpot.transform.position.y, currentTile.transform.position.z), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
                //Empty is on the ground underneath the player.
                //When player attempts to place a trap, it locates the center of the tile under the player.
                
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
        else if (PlayerPrefs.GetString("Cutscene") == "false" && PlayerPrefs.GetString("Paused") == "true")
        {
            playerAnimations.SetBool("walking", false);
            playerAnimations.SetBool("running", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "refill")
        {
            traps = 3;
            hasTraps = true;

            if (hasTaser)
            {
                hasFired = false;
            }

            PlayerPrefs.SetString("Last Checkpoint", other.gameObject.name);
            PlayerPrefs.SetString("Last Section", scene.name);
            PlayerPrefs.SetString("Section Display", "true");
            PlayerPrefs.Save();
        }
        else if (other.gameObject.tag == "newFloor")
        {
            if (scene.name == "NewTutorial")
            {
                PlayerPrefs.SetString("Last Section", "NewTutorial");
            }
            else if (scene.name == "1st Level")
            {
                PlayerPrefs.SetString("Last Section", "1st Level");
            }
            else if (scene.name == "2nd Level")
            {
                PlayerPrefs.SetString("Last Section", "2nd Level");
            }
            else if (scene.name == "3rd Level")
            {
                PlayerPrefs.SetString("Last Section", "3rd Level");
            }
            PlayerPrefs.Save();
        }
        else if (other.gameObject.tag == "lock")
        {
            PlayerPrefs.SetString("Section Display","false");

            if (scene.name == "NewTutorial")
            {
                PlayerPrefs.SetString("Cutscene", "true");
                PlayerPrefs.SetString("Paused", "true");
            }

            if (other.gameObject.name == "End")
            {
                PlayerPrefs.SetString("Last Checkpoint", "End");
                PlayerPrefs.Save();
            }
        }


        if (other.name.Contains("Ground"))
        {
            currentTile = other.gameObject;
        }
    }
}

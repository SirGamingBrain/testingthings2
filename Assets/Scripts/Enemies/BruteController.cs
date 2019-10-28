using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteController : EnemyBase
{
    Vector3 chargePoint;
    float chargeSpeed = 40.0f;
    float chargeRange = 25.0f;

    bool chargeCooldown = false;
    public float lastCharge = 0.0f;

    public IEnumerator Wait()
    {
        waiting = true;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
        animationController.Play("TurnRight");
        transform.Rotate(0, Mathf.Lerp(0, 1, 1), 0);
        yield return new WaitForSecondsRealtime(patrolWait / 2);
        animationController.Play("TurnLeft");
        transform.Rotate(0, Mathf.Lerp(0, -2, 2), 0);
        yield return new WaitForSecondsRealtime(patrolWait / 2);
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = patrolSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = true;
        waiting = false;
    }

    void Start()
    {
        startingPosition = this.transform.position;
        animationController = GetComponent<Animator>();
        freeze = false;
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");

        //If the Brute has Patrol Points
        if (patrolPoints != null)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
        }
    }
    
    void FixedUpdate()
    {
        //Keeps track of the player's location
        playerPosition = player.transform.position;

        //If not frozen
        if (!freeze)
        {
            //If Alert
            if (alertStatus)
            {
                //Target Player
                //Increase the amount that the Brute can see
                //Set the Brute to Alert and Chase the player
                targetPosition = player.transform.position;
                viewDistance = 35;
                ChasePlayer();
            }
            //If not alert and not waiting
            else if (!alertStatus && !waiting)
            {
                //Reduce amount that Brute can see
                //Set the Brute to not alert
                viewDistance = 15;

                //If there is 1 or less patrol points
                if (patrolPoints.Length <= 1)
                {
                    //Lounge in one spot
                    Lounge();
                }
                else
                {
                    //Patrol through waypoints
                    Patrol();
                }
            }
        }

        //If the charge timer is empty
        if (lastCharge <= 0)
        {
            //Turn off the charge cooldown
            chargeCooldown = false;
        }
        else
        {
            lastCharge -= 0.1f;
            chargeCooldown = true;
        }
    }

    public void ChasePlayer()
    {
        //Set speed to alert speed
        currentSpeed = alertSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;

        //If the player is still in the detection sphere, chase after the player
        if (detectionSphere.GetComponent<DetectionColliderController>().playerNear)
        {
            targetPosition = playerPosition;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
        }

        //If the enemy makes it to the targeted position but the player is no longer within the detection sphere
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f && !detectionSphere.GetComponent<DetectionColliderController>().playerNear && alertStatus)
        {
            //Brute is no longer alert
            alertStatus = false;
        }

        //If the enemy makes it to the targeted position and the player is within the detection sphere
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f && detectionSphere.GetComponent<DetectionColliderController>().playerNear && alertStatus)
        {
            //Continue chasing player
            targetPosition = playerPosition;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
        }

        //The enemy sends out a ray to detect if they can see the player in their charging range
        Vector3 direction = player.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, chargeRange) && alertStatus)
        {
            //If the Brute can see the player within charge range and their charge is not on cooldown
            if (hit.collider.gameObject.tag == "Player" && !chargeCooldown)
            {
                //Charge at the player
                //Set up the charge cooldown
                Debug.Log("Ready to Charge");
                Charge();
                lastCharge = 25.0f;
            }
        }
    }

    //Navigate between multiple points
    public void Patrol()
    {
        //Start Walk animation
        //Set speed to patrolling speed
        animationController.Play("Walk");
        currentSpeed = patrolSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;

        //If close to current destination
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.25f)
        {
            //Begin waiting before moving to next position
            StartCoroutine(Wait());
            NextPosition();
        }
    }

    //Sit in a single position and use an idle animation
    void Lounge()
    {
        //Sends the Brute back to their starting position
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = startingPosition;
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 2.5f)
        {
            //Starts the idle animation
            animationController.Play("Idle");
        }
        else
        {
            //Plays the walk animation if the enemy is moving to their starting position
            animationController.Play("Walk");
        }
    }

    //Charge attack towards the player
    void Charge()
    {
        Debug.Log("Charging");
        //The run animation begins
        //The Brute targets the current position of the player
        //The Brute begins running it's charge speed
        //The Brute is set to charging
        animationController.Play("Run");
        //chargePoint = player.transform.position;
        currentSpeed = chargeSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;
        //GetComponent<UnityEngine.AI.NavMeshAgent>().destination = chargePoint;
    }

    //Default stats for a Brute when created
    public BruteController()
    {
        maxHealth = 3;
        patrolSpeed = 2.0f;
        patrolWait = 4.0f;
        alertSpeed = 4.0f;
        viewDistance = 15.0f;
        freezeTimer = 10.0f;
        fieldOfView = 90.0f;
        alertStatus = false;
        waiting = false;
        freeze = false;
        chargeCooldown = false;
        chargeSpeed = 40.0f;
        chargeRange = 25.0f;
        lastCharge = 0.0f;
    }
}

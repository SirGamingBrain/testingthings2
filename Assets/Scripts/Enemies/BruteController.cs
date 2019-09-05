﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteController : EnemyBase
{
    Vector3 chargePoint;
    float chargeSpeed = 25.0f;
    float chargeRange = 25.0f;
    bool charging;

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
        charging = false;
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
                viewDistance = 45;
                detectionSphere.GetComponent<DetectionColliderController>().bruteAlert = true;
                ChasePlayer();
            }
            //If not alert and not waiting
            else if (!alertStatus && !waiting)
            {
                //Reduce amount that Brute can see
                //Set the Brute to not alert
                viewDistance = 25;
                detectionSphere.GetComponent<DetectionColliderController>().bruteAlert = false;

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
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = alertSpeed;

        //If the player is still in the detection sphere, chase after the player
        if (detectionSphere.GetComponent<DetectionColliderController>().playerNear)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = playerPosition;
        }

        //GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;

        //If the enemy makes it to the targeted position but the player is no longer within the detection sphere
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f && !detectionSphere.GetComponent<DetectionColliderController>().playerNear)
        {
            //Brute is no longer alert
            alertStatus = false;
        }

        //If the enemy makes it to the targeted position and the player is within the detection sphere
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f && detectionSphere.GetComponent<DetectionColliderController>().playerNear)
        {
            //Continue chasing player
            targetPosition = playerPosition;
        }

        //If the enemy can see the player and the player is within the charge range
        Vector3 direction = player.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, chargeRange))
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
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f)
        {
            //Begin waiting before moving to next position
            StartCoroutine(Wait());
            NextPosition();
        }

        //If the player whistles and is less than 1.2 * the view distance away
        if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 1.2))
        {
            //Target the player and pursue
            targetPosition = playerPosition;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
        }
    }

    //Sit in a single position and use an idle animation
    void Lounge()
    {
        //Starts the idle animation
        animationController.Play("Idle");
        
        //If the player whistles and is less than 1.2 * it's view distance
        if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 1.2))
        {
            //Target and pursue the player
            targetPosition = playerPosition;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
        }
    }

    //Charge attack towards the player
    void Charge()
    {
        //The run animation begins
        //The Brute targets the current position of the player
        //The Brute begins running it's charge speed
        //The Brute is set to charging
        animationController.Play("Run");
        chargePoint = player.transform.position;
        currentSpeed = chargeSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = chargePoint;
        charging = true;
    }

    //Sets the Brute to respawn in case of a level reset
    public void Respawn()
    {
        //Sends the Brute back to their starting position
        //Sets their patrol to their first patrol point
        //Resest their health, speed, and alert status
        GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(startingPositon);
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
        currentHealth = maxHealth;
        currentSpeed = patrolSpeed;
        alertStatus = false;
        freeze = false;
        chargeCooldown = false;
    }

    //Default stats for a Brute when created
    public BruteController()
    {
        maxHealth = 3;
        patrolSpeed = 2.0f;
        patrolWait = 4.0f;
        alertSpeed = 4.0f;
        viewDistance = 25.0f;
        freezeTimer = 10.0f;
        fieldOfView = 90.0f;
        alertStatus = false;
        waiting = false;
        charging = false;
    }
}

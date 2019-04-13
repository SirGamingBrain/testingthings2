using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutController : EnemyBase
{
    int hidePosition;
    public Transform[] hidingSpots;

    public float friendDistance = -999;
    public GameObject closestFriend;
    GameObject[] allFriends;

    public bool screamCooldown = false;
    public float lastScream = 0.0f;
    float screamRange = 25.0f;

    public IEnumerator Wait()
    {
        waiting = true;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
        animationController.Play("TurnRight");
        transform.Rotate(0, Mathf.Lerp(0, 1, 1), 0);
        yield return new WaitForSecondsRealtime(patrolWait);
        animationController.Play("TurnLeft");
        transform.Rotate(0, Mathf.Lerp(0, -2, 1), 0);
        yield return new WaitForSecondsRealtime(patrolWait);
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = patrolSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = true;
        waiting = false;
    }
    
    void Start()
    {
        animationController = GetComponent<Animator>();
        freeze = false;
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
    }
    
    void FixedUpdate()
    {
        //Makes a list of all other enemies in the level and constantly searches for the closest one that is not itself
        allFriends = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject g in allFriends)
        {
            if (Vector3.Distance(this.transform.position, g.transform.position) < Mathf.Abs(friendDistance) && Vector3.Distance(this.transform.position, g.transform.position) > 0)
            {
                if (g == this.gameObject)
                {
                    return;
                }
                else
                {
                    closestFriend = g;
                    friendDistance = Vector3.Distance(this.transform.position, g.transform.position);
                }
            }
            //checks to see if the enemies are close enough to hear the scream; if so, they are alerted
            if (screamCooldown && Vector3.Distance(this.transform.position, g.transform.position) <= screamRange)
            {
                g.GetComponent<EnemyBase>().alertStatus = true;
                g.GetComponent<EnemyBase>().targetPosition = player.transform.position;
            }
        }

        //Triggering alert states
        if (!freeze)
        {
            if (alertStatus)
            {
                currentSpeed = alertSpeed;
                if (!screamCooldown)
                {
                    animationController.Play("Run");
                    Scream();
                }
                NextHidingSpot();
            }
            if (!alertStatus && !waiting)
            {
                if (patrolPoints.Length <= 1)
                {
                    animationController.Play("Idle");
                    if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 2.5))
                    {
                        targetPosition = playerPosition;
                        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
                        alertStatus = true;
                    }

                }
                else
                {
                    Patrol();
                }
            }
        }

        if (lastScream <= 0)
        {
            alertStatus = false;
            screamCooldown = false;
        }
        else
        {
            lastScream -= Time.deltaTime;
        }
    }

    //Patrolling between waypoints
    public void Patrol()
    {
        animationController.Play("Walk");
        currentSpeed = patrolSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;

        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f)
        {
            StartCoroutine(Wait());
            NextPosition();
        }

        if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 2.5))
        {
            targetPosition = playerPosition;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
        }
    }

    //Triggers the prolonged scream
    //The scream alerts nearby enemies as the scout runs throughout the level
    void Scream()
    {
            screamCooldown = true;
            lastScream = 5.0f;
    }

    //When alert the scout moves from patrolling to hiding
    //The hiding places are at the far reaches of the level
    //The scout being alerted more than once will cause it to scream and run across the level
    public void NextHidingSpot()
    {
        if (hidingSpots.Length == 0)
            return;

        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hidingSpots[hidePosition].position;
        hidePosition = (hidePosition + 1) % hidingSpots.Length;
    }

    public void Respawn()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(startingPositon);
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
        currentHealth = maxHealth;
        currentSpeed = patrolSpeed;
        alertStatus = false;
        freeze = false;
        screamCooldown = false;
    }

    public ScoutController()
    {
        maxHealth = 1;
        patrolSpeed = 4.5f;
        alertSpeed = 10.0f;
        patrolWait = 3.0f;
        viewDistance = 20.0f;
        freezeTimer = 12.0f;
        fieldOfView = 100.0f;
        alertStatus = false;
        waiting = false;
    }
}

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
        //Checks to see if the enemy has died yet
        Die();

        //Makes a list of all other enemies in the level and constantly searches for the closest one that is not itself
        allFriends = GameObject.FindGameObjectsWithTag("Enemy");

        //For Every enemy contained in the list of friends
        foreach (GameObject g in allFriends)
        {
            //If the scout is closer to this enemy than their closest friend
            if (Vector3.Distance(this.transform.position, g.transform.position) < Mathf.Abs(friendDistance) && Vector3.Distance(this.transform.position, g.transform.position) > 0)
            {
                //If the enemy is this scout
                if (g == this.gameObject)
                {
                    return;
                }
                //If the enemy is not this scout
                else
                {
                    //The enemy becomes the new closest friend
                    //This enemy sets the new closest friend distance
                    closestFriend = g;
                    friendDistance = Vector3.Distance(this.transform.position, g.transform.position);
                }
            }
            //If the scout is screaming
            //and the enemy is close enough to the scout to hear the scream
            if (screamCooldown && Vector3.Distance(this.transform.position, g.transform.position) <= screamRange)
            {
                //The enemy becomes alert
                //The enemy targets the player
                g.GetComponent<EnemyBase>().alertStatus = true;
                g.GetComponent<EnemyBase>().targetPosition = player.transform.position;
            }
        }

        //If not frozen
        if (!freeze)
        {
            //Allows the animations of the scout to play
            animationController.enabled = true;

            //If alert
            if (alertStatus)
            {
                //Set speed to alert
                currentSpeed = alertSpeed;
                GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;

                //If the scream is not on cooldown
                if (!screamCooldown)
                {
                    //Begin the run animation
                    //Begin screaming
                    animationController.Play("Run");
                    Scream();
                }
                //Move to the next hiding spot
                NextHidingSpot();
            }
            //If not alert and not waiting
            if (!alertStatus && !waiting)
            {
                currentSpeed = patrolSpeed;
                GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;

                //When distracted
                if (distracted)
                {
                    //Run to the next hiding spot
                    currentSpeed = alertSpeed;
                    if(hidingSpots.Length < 1)
                    {
                        distracted = false;
                    }
                    else
                    {
                        targetPosition = hidingSpots[0].position;
                        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
                    }

                    //If the scout makes it to the hiding spot, the distraction is over
                    if (Vector3.Distance(this.gameObject.transform.position, targetPosition) < 0.25f)
                    {
                        distracted = false;
                    }
                }

                //If is 1 or less patrol points
                if (patrolPoints.Length <= 1)
                {
                    //Play the idle animation
                    animationController.Play("Idle");

                    //If the player whistles and is within 2.5 * the view distance of the scout
                    if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 2.5))
                    {
                        //Becomes distracted
                        distracted = true;
                    }

                }
                //If there are at least 2 patrol points
                else
                {
                    //Go on Patrol
                    Patrol();
                }
            }
        }
        //If the scout is frozen
        else
        {

            //The animation that the scout is in is paused
            //The Scout stops waiting if it is currently waiting
            animationController.enabled = false;
            waiting = false;
        }

        //If the value of the scream is 0 or less
        if (lastScream <= 0)
        {
            //No longer Alert
            //The scream is no longer on cooldown
            alertStatus = false;
            screamCooldown = false;
        }
        else
        {
            //The scream duration runs out
            //The scream is placed on cooldown
            lastScream -= Time.deltaTime;
            screamCooldown = true;
        }
    }

    //Patrolling between waypoints
    public void Patrol()
    {
        //Sets the animation to walk
        //Sets the speed to the patrol speed
        animationController.Play("Walk");
        currentSpeed = patrolSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;

        //If the scout is almost at its destinatiion
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.25f)
        {
            //Begin waiting before moving on to the next position
            StartCoroutine(Wait());
            NextPosition();
        }

        //If the player whistles and the player is 2.5 * the view distance from the scout
        if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 2.5))
        {
            //Become distracted
            distracted = true;
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
        //If there are no hiding spots
        if (hidingSpots.Length == 0)
            return;

        //Move to the the next hiding spot
        //Queue up the next hiding spot for the next call
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = hidingSpots[hidePosition].position;
        hidePosition = (hidePosition + 1) % hidingSpots.Length;
    }

    //Respawn method for the scout to prevent reloading the scene
    public void Respawn()
    {
        //Send the scout back to its starting position
        //Set the scout to go to its first waypoint
        //Set the scout's health to max
        //Sets the speed to the patrol speed
        //Resets alert status
        GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(startingPositon);
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
        currentHealth = maxHealth;
        currentSpeed = patrolSpeed;
        alertStatus = false;
        freeze = false;
        screamCooldown = false;
    }

    public void Die()
    {
        if (currentHealth <= 0)
        {
            animationController.Play("DieLeft");
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        }
    }

    //Default stats for a spawned Scout
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

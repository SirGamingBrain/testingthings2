using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public GameObject player;

    public Animator animationController;

    public int currentHealth;
    public int maxHealth = 1;
    public float currentSpeed;
    public float patrolSpeed = 3.0f;
    public float alertSpeed = 7.0f;
    public float patrolWait = 2.0f;
    public float viewDistance = 10.0f;
    public bool alertStatus;
    public bool distracted;

    int distractPosition;
    public int patrolPosition;
    public Transform[] patrolPoints;

    public bool waiting = false;
    public bool freeze = false;
    public float freezeTimer = 15.0f;
    public float lastFreeze = 0.0f;

    public Vector3 startingPositon;
    public Vector3 playerPosition;
    public Vector3 targetPosition;

    public float fieldOfView = 120.0f;
    public GameObject detectionSphere;

    //Wait function for pausing between waypoints
    public IEnumerator Wait()
    {
        //Makes the model look to the right, then pivots to the right
        //Then Left
        //Then the turning is reset
        waiting = true;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
        animationController.SetBool("turn_right", true);
        transform.Rotate(0, Mathf.Lerp(0, 1, 1), 0);
        yield return new WaitForSecondsRealtime(patrolWait / 3);
        animationController.SetBool("turn_right", false);
        animationController.SetBool("turn_left", true);
        transform.Rotate(0, Mathf.Lerp(0, -2, 2), 0);
        yield return new WaitForSecondsRealtime(patrolWait / 3);
        animationController.SetBool("turn_left", false);
        animationController.SetBool("turn_right", true);
        transform.Rotate(0, Mathf.Lerp(0, 1, 1), 0);
        animationController.SetBool("turn_right", false);
        yield return new WaitForSecondsRealtime(patrolWait / 3);
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = patrolSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = true;
        waiting = false;
    }

	//Initializes the states for this agent
	//Finds the Player
	//Begins navigating along waypoints
    void Start()
    {
        startingPositon = this.transform.position;
        animationController = GetComponent<Animator>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
    }
    
    void FixedUpdate()
    {
        //Reduces the frozen state of the enemy over time
		//Once the frozen state is 0 or below, the enemy returns to being active
        lastFreeze -= 0.1f;
        if (lastFreeze <= 0)
        {
            freeze = false;
        }
        else
        {
            freeze = true;
        }

        //Checks to see if the enemy has died yet
        Die();

        //Keeps track of the player's location
        playerPosition = player.transform.position;

        //Only react when not frozen
        if (!freeze)
        {
			//If the enemy is not alert and not waiting
            if (!alertStatus && !waiting)
            {
                if (distracted)
                {
                    //Speed is walk speed
                    //Save current destination as pre-distraction destination
                    //Head to distraction
                    //Make it to the distraction
                    //If nothing is found, return to the predistraction destination
                    //Completely ignores running
                    currentSpeed = patrolSpeed;
                    distractPosition = patrolPosition;
                    if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.25f)
                    {
                        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[distractPosition].position;
                    }
                }
				//If the enemy has only 1 or less spaces to patrol
                else if (patrolPoints.Length <= 1 && !distracted)
                {
					//Sets the enemy to remain still in a guarding position
                    animationController.SetInteger("battle", 0);
                    animationController.SetInteger("moving", 0);
					//If the player whistles and they are less than Twice the Sight distance of the agent away
                    if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 2))
                    {
                        //The enemy heads toward the distraction point
                        targetPosition = playerPosition;
                        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
                        distracted = true;
                    }
                }
				//If the enemy has at least 2 locations to patrol between
                else
                {
					//Patrol between the points
                    Patrol();
                }
            }
			//If the enemy is alert
            if (alertStatus)
            {
				//Pursue the player
                ChasePlayer();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //On colliding with taser shot
		//Sets state to Frozen
        if (collision.gameObject.tag == "Taser")
        {
            animationController.Play("hit_1");
            FreezeState();
            Debug.Log("Hit By Taser");
        }

        //Attack when colliding with the player and resets alertness as player should be dead
        if (collision.gameObject.tag == "Player" && currentHealth > 0)
        {
            animationController.Play("attack2");
            player.GetComponent<PlayerController>().health -= 1;
            alertStatus = false;
        }

        //Takes damage and freezes in place if trapped
        if (collision.gameObject.tag == "Trap")
        {
            currentHealth -= 1;
            animationController.Play("hit_2");
            FreezeState();
        }
    }

    //Move to the next patrol position
    public void NextPosition()
    {
		//If there are no waypoints to patrol to
        if (patrolPoints.Length == 0)
            return;

		//The next waypoint is set to the next point in patrolPoints
		//Then the next waypoint is readied in the system
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[patrolPosition].position;
        patrolPosition = (patrolPosition + 1) % patrolPoints.Length;
    }

    //Freezes the enemy and locks them in place
    //They do not get a new destination until after the freeze timer runs out
    public void FreezeState()
    {
		//The Speed is reduced to 0
		//They are no longer Alert
		//Their destination is the location they currently stand in
		//They are Frozen
        currentSpeed = 0;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;
        lastFreeze = freezeTimer;
        alertStatus = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = this.transform.position;
        freeze = true;
    }

    //Chase down the plauyer
    public void ChasePlayer()
    {
		//Set the animation to chasing
        animationController.SetInteger("battle", 1);
        animationController.SetInteger("moving", 1);
        
		//Set the agent to the faster, alert speed
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = alertSpeed;

        //If the player is still in the detection sphere, chase after the player
        if (detectionSphere.GetComponent<DetectionColliderController>().playerNear)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = playerPosition;
        }
        //GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
        //If the enemy makes it to the targeted position but the player is no longer within the detection sphere
        if (alertStatus && GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f && !detectionSphere.GetComponent<DetectionColliderController>().playerNear)
        {
			//Enemy is no longer alert
            alertStatus = false;
        }
        //If the enemy makes it to the targeted position and the player is within the detection sphere
        if (alertStatus && GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f && detectionSphere.GetComponent<DetectionColliderController>().playerNear)
        {
			//Continue to chase the player
            targetPosition = playerPosition;
        }
    }

    //Patrol through assigned way points
    public void Patrol()
    {
        //Sets the animation controller to walk
        animationController.SetInteger("battle", 0);
        animationController.SetInteger("moving", 1);

		//Sets the agent to move at the patrolling pace
        currentSpeed = patrolSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;

        //If not alert and almost at the destination
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.25f && !alertStatus)
        {
            //Set the animation to idle and wait
            animationController.SetInteger("battle", 0);
            animationController.SetInteger("moving", 0);
            StartCoroutine(Wait());
            NextPosition();
        }
    }

    //Play the death animation
    //Disable the NavMeshAgent, so the dead enemy doesn't move
    public void Die()
    {
        if (currentHealth <= 0)
        {
            animationController.Play("alt_death");
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        }
    }

    //Allows the enemies to reset without resetting the level entirely
    public void Respawn()
    {
		//Resets the enemy to be in their starting position so that the experience is consistent
		//The starting position is wherever they are placed, so it can change dynamically
		//The enemy begins navigating to their first patrol point
		//Reenables the NavMeshAgent
        GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(startingPositon);
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
		GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        currentHealth = maxHealth;
        currentSpeed = patrolSpeed;
        alertStatus = false;
        freeze = false;
    }
}

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
        lastFreeze -= 0.1f;
        if (lastFreeze <= 0)
        {
            freeze = false;
        }
        else
        {
            freeze = true;
        }

        Die();
        //Keeps track of the player's location
        playerPosition = player.transform.position;

        //Only react when not frozen
        if (!freeze)
        {
            if (!alertStatus && !waiting)
            {
                if (patrolPoints.Length <= 1)
                {
                    animationController.SetInteger("battle", 0);
                    animationController.SetInteger("moving", 0);
                    if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 2))
                    {
                        //Go after the player
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
            if (alertStatus)
            {
                ChasePlayer();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Get hit and freeze in place when frozen
        if (collision.gameObject.tag == "Taser")
        {
            animationController.Play("hit_1");
            FreezeState();
        }

        //Attack when colliding with the player and resets alertness as player should be dead
        if (collision.gameObject.tag == "Player")
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
        if (patrolPoints.Length == 0)
            return;

        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[patrolPosition].position;
        patrolPosition = (patrolPosition + 1) % patrolPoints.Length;
    }

    //Freezes the enemy and locks them in place
    //They do not get a new destination until after the freeze timer runs out
    public void FreezeState()
    {
        currentSpeed = 0;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
        lastFreeze = freezeTimer;
        alertStatus = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = this.transform.position;
        freeze = true;
    }

    //Chase down the plauyer
    public void ChasePlayer()
    {
        animationController.SetInteger("battle", 1);
        animationController.SetInteger("moving", 1);
        
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
            alertStatus = false;
        }
        //If the enemy makes it to the targeted position and the player is within the detection sphere
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f && detectionSphere.GetComponent<DetectionColliderController>().playerNear)
        {
            targetPosition = playerPosition;
        }
    }

    //Patrol through assigned way points
    public void Patrol()
    {
        //Sets the animation controller to walk
        animationController.SetInteger("battle", 0);
        animationController.SetInteger("moving", 1);

        currentSpeed = patrolSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;

        //If not alert and almost at the destination
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 0.1f && !alertStatus)
        {
            //Set the animation to idle and wait
            animationController.SetInteger("battle", 0);
            animationController.SetInteger("moving", 0);
            StartCoroutine(Wait());
            NextPosition();
        }
        //If the player whistles and they are close enough
        if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 2.5))
        {
            //Go after the player
            targetPosition = playerPosition;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
            alertStatus = true;
        }
    }

    //Play the death animation
    //Eventually will also delete the object after some time
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
        GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(startingPositon);
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
        currentHealth = maxHealth;
        currentSpeed = patrolSpeed;
        alertStatus = false;
        freeze = false;
    }
}

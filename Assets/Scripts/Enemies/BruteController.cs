using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteController : EnemyBase
{
    Vector3 chargePoint;
    float chargeSpeed = 15.0f;
    float chargeRange = 20.0f;
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
        if (patrolPoints != null)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
        }
    }
    
    void FixedUpdate()
    {
        if (!freeze)
        {
            if (alertStatus)
            {
                targetPosition = player.transform.position;
                viewDistance = 45;
                detectionSphere.GetComponent<DetectionColliderController>().bruteAlert = true;
                ChasePlayer();
            }
            else if (!alertStatus && !waiting)
            {
                viewDistance = 25;
                detectionSphere.GetComponent<DetectionColliderController>().bruteAlert = true;

                if (patrolPoints.Length <= 1)
                {
                    Lounge();
                }
                else
                {
                    Patrol();
                }
            }
        }

        if (lastCharge <= 0)
        {
            chargeCooldown = false;
        }
        else
        {
            lastCharge -= 0.1f;
        }
    }

    public void ChasePlayer()
    {
        currentSpeed = alertSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
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
        //If the enemy can see the player and the player is within the charge range
        Vector3 direction = player.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, chargeRange))
        {
            if (hit.collider.gameObject.tag == "Player" && !chargeCooldown)
            {
                Debug.Log("Ready to Charge");
                Charge();
            }
        }
    }

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

        if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 1.2))
        {
            targetPosition = playerPosition;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
        }
    }

    //Sit in a single position and use an idle animation
    void Lounge()
    {
        animationController.Play("Idle");
        if (player.GetComponent<PlayerController>().whistleCooldown && Vector3.Distance(this.gameObject.transform.position, playerPosition) < (viewDistance * 1.2))
        {
            targetPosition = playerPosition;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = targetPosition;
        }
    }

    void Charge()
    {
        animationController.Play("Run");
        chargePoint = player.transform.position;
        currentSpeed = chargeSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = currentSpeed;
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = chargePoint;
        charging = true;
        lastCharge = 10.0f;
    }

    public void Respawn()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(startingPositon);
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = patrolPoints[0].position;
        currentHealth = maxHealth;
        currentSpeed = patrolSpeed;
        alertStatus = false;
        freeze = false;
        chargeCooldown = false;
    }

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

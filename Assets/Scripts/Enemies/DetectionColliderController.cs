using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionColliderController : MonoBehaviour
{
    //Will be paired with an enemy
    //Allows there to be a reference to the enemy's view distance
    //And for adjusting the max angle that an enemy can see
    //Possible for the view distance to affect the scale of the sphere

    public GameObject enemy;
    public bool playerNear = false;
    Vector3 detectionRadius;

    public float meshResolution;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    void Start()
    {
        //Scales the detection ring to the size of the enemy's viewDistance
        detectionRadius = this.transform.localScale;
        if (enemy.GetComponent<ScoutController>() == null && enemy.GetComponent<BruteController>() == null)
        {
            GruntResize();
        }
        else if (enemy.GetComponent<ScoutController>() != null)
        {
            ScoutResize();
        }
        else if (enemy.GetComponent<BruteController>() != null)
        {
            BruteResize();
        }
        this.transform.localScale = detectionRadius;
    }
    
    void FixedUpdate()
    {
        //Updates the size of the detection sphere of the Brute as it changes when Alert and Passive
        detectionRadius = this.transform.localScale;
        if (enemy.GetComponent<BruteController>() != null)
        {
            BruteResize();
        }
        this.transform.localScale = detectionRadius;
    }

    void OnAwake()
    {

    }

    //As long as another object is within this collider
    void OnTriggerStay (Collider other)
    {
        //while the player remains in the detection sphere
        if (other.gameObject.tag == "Player")
        {
            //Keeps track that the player is near
            //Saves the direction of the player
            //Save the angle between the enemy and the player
            playerNear = true;
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            //Detects if the player is within the enemy field of view
            if (angle < enemy.GetComponent<EnemyBase>().fieldOfView * 0.5f)
            {
                RaycastHit hit;
                //Checks to see if the player is obscured
                if (Physics.Raycast(transform.position, direction.normalized, out hit, enemy.GetComponent<EnemyBase>().viewDistance))
                {
                    //If the player is not obscured
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        //Chase the Player
                        CatchPlayer();
                    }
                }
            }
            //If the player is near the enemy and running and the enemy is not distracted
            if (!other.GetComponent<PlayerController>().sneaking && !enemy.GetComponent<EnemyBase>().distracted)
            {
                //Chase the Player
                CatchPlayer();
            }
        }
    }

    //Trying to get the field of view to be visually drawn
    //Not working the way I want it to, so currently disabled
    /*
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(enemy.GetComponent<EnemyBase>().fieldOfView * meshResolution);
        float stepAngleSize = enemy.GetComponent<EnemyBase>().fieldOfView / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = enemy.transform.eulerAngles.y - enemy.GetComponent<EnemyBase>().fieldOfView / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = enemy.transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, enemy.GetComponent<EnemyBase>().viewDistance, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * enemy.GetComponent<EnemyBase>().viewDistance, enemy.GetComponent<EnemyBase>().viewDistance, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }*/

    //When leaving the detection collider
    void OnTriggerExit(Collider other)
    {
        //If the player leaves the detection collider
        if (other.gameObject.tag == "Player")
        {
            //The player is no longer considered near the enemy
            playerNear = false;
        }
    }

    //Sets the enemy to being alert
    void CatchPlayer()
    {
        enemy.GetComponent<EnemyBase>().alertStatus = true;
    }

    //Resizes the sphere for the scout's view
    void ScoutResize()
    {
        detectionRadius.x = enemy.GetComponent<ScoutController>().viewDistance;
        detectionRadius.y = enemy.GetComponent<ScoutController>().viewDistance;
        detectionRadius.z = enemy.GetComponent<ScoutController>().viewDistance;
    }

    //Resizes the sphere for the grunt's view
    void GruntResize()
    {
        detectionRadius.x = enemy.GetComponent<EnemyBase>().viewDistance;
        detectionRadius.y = enemy.GetComponent<EnemyBase>().viewDistance;
        detectionRadius.z = enemy.GetComponent<EnemyBase>().viewDistance;
    }

    //Resizes the sphere for the brute's view
    void BruteResize()
    {
        detectionRadius.x = enemy.GetComponent<BruteController>().viewDistance;
        detectionRadius.y = enemy.GetComponent<BruteController>().viewDistance;
        detectionRadius.z = enemy.GetComponent<BruteController>().viewDistance;
    }
}

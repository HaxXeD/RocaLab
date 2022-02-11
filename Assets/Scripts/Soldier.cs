using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Soldier : MonoBehaviour
{   
    [SerializeField] Transform pathHolder;
    Transform player;
    Color originalSpotlightColor;

    [SerializeField] float soldierSpeed = 5f, waitTime = .3f,turnSpeed = 90f;

    bool isCoroutineStarted = false;
    Vector2[] waypoints;
    int targetWaypointIndex;
    Vector2 targetWaypoint;

    [SerializeField] Light2D spotLight;
    [SerializeField] LayerMask viewMask;
    
    float viewAngle, viewDistance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotLight.pointLightOuterAngle;
        viewDistance = spotLight.pointLightOuterRadius;
        originalSpotlightColor = spotLight.color;


        waypoints = new Vector2[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        transform.position = waypoints[0];
        FollowPath();

        StartCoroutine(FollowPath(waypoints));
    }

    private void FollowPath()
    {
        targetWaypointIndex = 1;
        targetWaypoint = waypoints[targetWaypointIndex];
        Vector2 dir = targetWaypoint - (Vector2)transform.position;
        float rot_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    bool CanSeePlayer()
    {
        if (Vector2.Distance(transform.position, player.position) < viewDistance)
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector2.Angle(transform.up, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics2D.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
                
            }
        }
        return false;
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            spotLight.color = Color.red;
            StopAllCoroutines();
            isCoroutineStarted = false;

        }
        else if (!CanSeePlayer()&&!isCoroutineStarted)
        {
            spotLight.color = originalSpotlightColor;
            StartCoroutine(FollowPath(waypoints));
        }
    }

    //FollowPath coroutine
    IEnumerator FollowPath(Vector2[] waypoints)
    {
        isCoroutineStarted = true;
        

        while (true)
        {
            spotLight.color = originalSpotlightColor;
            transform.position = Vector2.MoveTowards(transform.position, targetWaypoint, soldierSpeed * Time.deltaTime);
            if ((Vector2)transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];

                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }        
    }

    IEnumerator TurnToFace(Vector2 looktarget)
    {
        Vector2 dirtolooktarget = (looktarget - (Vector2)transform.position).normalized;
        float targetangle =Mathf.Atan2(dirtolooktarget.y, dirtolooktarget.x) * Mathf.Rad2Deg - 90f;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetangle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetangle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.forward * angle;
            yield return null;

        }
    }

    //Visual Gizmo Guide
    private void OnDrawGizmos()
    {
        Vector2 startPosition = pathHolder.GetChild(0).position;
        Vector2 previousPosition = startPosition;
        foreach(Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.up * viewDistance);
    }
}

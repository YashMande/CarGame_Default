using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCar : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float speed = 5f;
    public float rotationSpeed = 2f;
    public float detectionRadius = 10f;
    public float raycastDistance = 5f;
    public LayerMask obstacleLayer;

    private int currentPatrolIndex;
    private Transform currentPatrolPoint;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentPatrolIndex = 0;
        currentPatrolPoint = patrolPoints[currentPatrolIndex];
    }

    private void Update()
    {
        if(currentPatrolIndex > 3)
        {
            currentPatrolIndex = 0;
        }
        // Move towards current patrol point
        Vector3 direction = currentPatrolPoint.position - transform.position;
        rb.AddForce(direction.normalized * speed);

        // Rotate towards current patrol point
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // Check if reached patrol point
        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1f)
        {
            // Set next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1);
            currentPatrolPoint = patrolPoints[currentPatrolIndex];
        }

  
        // Obstacle avoidance
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, obstacleLayer))
        {
            // Steer away from obstacle
            Vector3 avoidDirection = transform.position - hit.point;
            rb.AddForce(avoidDirection.normalized * speed);
        }
        if (Physics.Raycast(transform.position, transform.right, out hit, raycastDistance, obstacleLayer))
        {
            // Steer away from obstacle
            Vector3 avoidDirection = transform.position - hit.point;
            rb.AddForce(avoidDirection.normalized * speed);
        }
        if (Physics.Raycast(transform.position, transform.up, out hit, raycastDistance, obstacleLayer))
        {
            // Steer away from obstacle
            Vector3 avoidDirection = transform.position - hit.point;
            rb.AddForce(avoidDirection.normalized * speed);
        }

    }
}

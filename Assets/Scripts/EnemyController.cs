using UnityEngine;
using Pathfinding;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public float WalkSpeed = 100;
    public float WaypointArrivalThreshold = 0.1f;
    public LayerMask MouseSelectionLayerMask;

    private Animator m_animator;
    private CharacterController m_controller;
    private Seeker m_seeker;
    private Path m_path;
    private bool m_moving = false;
    private int m_currentWaypoint = -1;

    private Transform target;
    private float distance;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        m_animator = GetComponent<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_seeker = GetComponent<Seeker>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        distance = Vector3.Distance(transform.position, target.position);
        // If the distance is less than 10 and the enemy is not
        // currently moving...
        if (distance < 10 && distance > 2)
        {
            // Attempt to path to the ray intersection.
            m_seeker.StartPath(transform.position, target.position,
                OnPathComplete);
        }

        // If we don't have a path return.
        if (m_path == null)
        {
            return;
        }

        //Test to see if Player is in range to attack
        //If so, stop and attack
        if (distance <= 2)
        {
            m_path = null;
            m_currentWaypoint = -1;
            m_animator.SetFloat("Speed", 0);
            m_moving = false;
            transform.rotation = Quaternion.LookRotation(
            m_controller.velocity);
            m_animator.SetBool("Attack", true);
            return;
        }

        // Test if we have just reached the end of the path. 
        if (m_currentWaypoint >= m_path.vectorPath.Count)
        {
            m_path = null;
            m_currentWaypoint = -1;
            m_animator.SetFloat("Speed", 0);
            m_moving = false;
            return;
        }

        // Find direction and distance to the next waypoint and move the
        // player via the attached character controller.
        Vector3 direction = (m_path.vectorPath[m_currentWaypoint] -
            transform.position).normalized;
        Vector3 displacement = direction * WalkSpeed * Time.deltaTime;
        m_controller.SimpleMove(displacement);
        // Keep the adventurer looking in direction of movement.
        transform.rotation = Quaternion.LookRotation(
            m_controller.velocity);

        // If close enough to current waypoint switch to next waypoint.
        if (Vector3.Distance(transform.position, m_path.vectorPath[
            m_currentWaypoint]) < WaypointArrivalThreshold)
        {
            m_currentWaypoint++;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            m_path = path;
            m_currentWaypoint = 0;
            m_animator.SetFloat("Speed", 1);
            m_moving = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnDisable()
    {
        m_seeker.pathCallback -= OnPathComplete;
    }
}

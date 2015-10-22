using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{

    public float WalkSpeed = 100;
    public float WaypointArrivalThreshold = 0.1f;
    public LayerMask MouseSelectionLayerMask;

    private Animator m_animator;
    private CharacterController m_controller;
    private Seeker m_seeker;
    private Path m_path;
    private bool m_moving = false;
    private int m_currentWaypoint = -1;

    public int Health;
    public int Speed;
    public int Attack;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_seeker = GetComponent<Seeker>();

        // Positions the player correctly in the map, based on the last portal taken.
        switch (StateMigrator.lastPortalActionTaken)
        {
            case PortalAction.ExitLevel1:
                transform.position = new Vector3(10.0f, 0.0f, 0.0f);
                break;
            case PortalAction.ExitLevel2:
                transform.position = new Vector3(-10.0f, 0.0f, 0.0f);
                break;
        }

        Health = this.gameObject.GetComponent<Player>().Health;
        Speed = this.gameObject.GetComponent<Player>().Speed;
        Attack = this.gameObject.GetComponent<Player>().Attack;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        // If the right mouse button was pressed start moving.
        if (Input.GetMouseButtonUp(1))
        {
                // Cast a ray from the cursor to find the point it intersects
                // the floor in world coordinates.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, MouseSelectionLayerMask))
                {
                    // Attempt to path to the ray intersection.
                    m_seeker.StartPath(transform.position, hit.point, 
                        OnPathComplete);
                }         
        }

        // If we don't have a path return.
        if (m_path == null)
        {
            return;
        }

        // Test if we have just reached the end of the path. 
        if (m_currentWaypoint >= m_path.vectorPath.Count)
        {
            StopMoving();
            return;
        }
        
        // Find direction and distance to the next waypoint and move the
        // player via the attached character controller.
        Vector3 direction = (m_path.vectorPath[m_currentWaypoint] -
            transform.position).normalized;
        Vector3 displacement = direction * WalkSpeed * Time.deltaTime;
        m_controller.SimpleMove(displacement);
        // Keep the adventurer looking in direction of movement (ignores y axis)
        transform.rotation = Quaternion.LookRotation(
            new Vector3(m_controller.velocity.x, 0.0f, m_controller.velocity.z));

        // If close enough to current waypoint switch to next waypoint.
        if (Vector3.Distance(transform.position, m_path.vectorPath[
            m_currentWaypoint]) < WaypointArrivalThreshold)
        {
            m_currentWaypoint++;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            m_animator.SetTrigger("Attack");
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

    public void TakeDamage(int damage)
    {
        Health = Health - damage;
    }

    public int GiveDamage()
    {
        return Attack;
    }

    public void SetSpeed()
    {
        WalkSpeed = Speed;
    }

    public void StopMoving()
    {
        m_path = null;
        m_currentWaypoint = -1;
        m_animator.SetFloat("Speed", 0);
        m_moving = false;
    }
}

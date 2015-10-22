using UnityEngine;
using System.Collections;
using Pathfinding;

public class GolemController : MonoBehaviour
{

    enum GolemMode
    {
        Idling = 0,
        Patrolling = 1,
        Sleeping = 2,
        Hunting = 3
    }


    public float WalkSpeed = 65;
    public float WaypointArrivalThreshold = 0.1f;
    public PlayerController Player;
    private Animator m_animator;
    private CharacterController m_controller;
    private Seeker m_seeker;
    private Path m_currentPath;
    private int m_currentWaypoint = -1;
    private GolemMode m_mode = GolemMode.Idling;
    float m_sleepingDuration = 0f;
    float m_pathfindingUpdateTime = 0f;
    float sleepChance = 0.001f;
    float patrolChance = 0.005f;
    Vector3 m_playerPrevPos;
    float m_playerDistance;
    public float MinSleepDuration = 5.0f;
    public float WakeUpChance = 0.001f;
    public float HuntingPathfindingInterval = 3.0f;
    public float MaxDetectionDistance = 10.0f;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_seeker = GetComponent<Seeker>();

    }
    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        // When the enemy gets close enough to the player we should initiate 
        // combat. At this stage I'm just destroying the enemy.
        m_playerDistance = Vector3.Distance(transform.position,
            Player.transform.position);
        if (m_playerDistance < 1.5f)
        {
            Destroy(this.gameObject);
            return;
        }

        if (m_mode == GolemMode.Idling)
        {
            UpdateIdling();
        }
        else if (m_mode == GolemMode.Patrolling)
        {
            UpdatePatrolling();
        }
        else if (m_mode == GolemMode.Hunting)
        {
            UpdateHunting();
        }
        else if (m_mode == GolemMode.Sleeping)
        {
            UpdateSleeping();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public float MaxDetectionChance = 0.55f;
    public float MinDetectionChance = 0.0f;
    public float MaxSleepingDetectionChance = 0.1f;
    public float MinSleepingDetectionChance = 0.0f;
    public float MinDetectionDistance = 0.0f;

    private void UpdateIdling()
    {
        // If the player is within detection range, attempt to detect the player.
        if (m_playerDistance < MaxDetectionDistance)
        {
            float detectionChance = LinearTransform(m_playerDistance, MinDetectionDistance,
                MaxDetectionDistance, MaxDetectionChance, MinDetectionChance);
            if (Random.value < detectionChance)
            {
                while (true)
                {
                    Path path = m_seeker.StartPath(transform.position, Player.transform.position);
                    if (!path.error)
                    {
                        m_pathfindingUpdateTime = 0.0f;
                        m_currentPath = path;
                        m_currentWaypoint = 0;
                        m_mode = GolemMode.Hunting;
                        m_animator.SetBool("Hunting", true);
                        return;
                    }
                }
            }
        }

        if (Random.value < sleepChance)
        {
            m_mode = GolemMode.Sleeping;
            m_animator.SetBool("Sleeping", true);
            return;
        }

        if (Random.value < patrolChance)
        {

            Vector2 point = Random.insideUnitCircle * 15;
            Vector3 end = transform.position;
            end.x += point.x;
            end.y = 0.5f;
            end.z += point.y;
            while (true)
            {
                Path path = m_seeker.StartPath(transform.position, end);
                if (!path.error)
                {
                    m_currentPath = path;
                    m_currentWaypoint = 0;
                    m_mode = GolemMode.Patrolling;
                    m_animator.SetBool("Patrolling", true);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdatePatrolling()
    {
        if (m_playerDistance < MaxDetectionDistance)
        {
            float detectionChance = LinearTransform(m_playerDistance, 0, 10, 0.95f, 0f);
            if (Random.value < detectionChance)
            {
                Path path = m_seeker.StartPath(transform.position, Player.transform.position);
                if (!path.error)
                {
                    m_currentPath = path;
                    m_currentWaypoint = 0;
                    m_mode = GolemMode.Hunting;
                    m_animator.SetBool("Hunting", true);
                    m_animator.SetBool("Patrolling", false);
                }
            }
        }

        // Test if we have just reached the end of the path. 
        if (m_currentWaypoint >= m_currentPath.vectorPath.Count)
        {
            m_currentPath = null;
            m_currentWaypoint = -1;
            m_animator.SetBool("Patrolling", false);
            m_mode = GolemMode.Idling;
            return;
        }

        // Find direction and distance to the next waypoint and move the
        // player via the attached character controller.
        Vector3 direction = (m_currentPath.vectorPath[m_currentWaypoint] -
            transform.position).normalized;
        Vector3 displacement = direction * WalkSpeed * Time.deltaTime;
        m_controller.SimpleMove(displacement);
        // Keep the player orientated in the direction of movement.
        transform.rotation = Quaternion.LookRotation(
            m_controller.velocity);

        // If close enough to current waypoint switch to next waypoint.
        Vector2 transform2d = new Vector2(transform.position.x, transform.position.z);
        Vector2 waypoint2d = new Vector2(m_currentPath.vectorPath[m_currentWaypoint].x, m_currentPath.vectorPath[m_currentWaypoint].z);
        if (Vector2.Distance(transform2d, waypoint2d) < WaypointArrivalThreshold)
        {
            m_currentWaypoint++;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateHunting()
    {
        // Check if we have reached the end of the current path. 
        if (m_currentWaypoint >= m_currentPath.vectorPath.Count)
        {
            // If so, find a new path to the player.
            Path path = m_seeker.StartPath(transform.position, Player.transform.position);
            if (!path.error)
            {
                m_pathfindingUpdateTime = 0f;
                m_playerPrevPos = Player.transform.position;
                m_currentPath = path;
                m_currentWaypoint = 0;
            }
        }

        // If the player has moved significantly since last pathfinding and more
        // than the designated pathfinding interval has passed...
        m_pathfindingUpdateTime += Time.deltaTime;
        if (m_pathfindingUpdateTime > HuntingPathfindingInterval &&
            (m_playerPrevPos.magnitude - Player.transform.position.magnitude) > 1)
        {
            // Re-path to the player and reset pathfinding variables.
            Path path = m_seeker.StartPath(transform.position, Player.transform.position);
            if (!path.error)
            {
                m_playerPrevPos = Player.transform.position;
                m_currentPath = path;
                m_currentWaypoint = 0;
            }
            m_pathfindingUpdateTime = 0f;
        }

        // Find direction and distance to the next waypoint and move the
        // character via the attached character controller.
        Vector3 direction = (m_currentPath.vectorPath[m_currentWaypoint] -
            transform.position).normalized;
        Vector3 displacement = direction * WalkSpeed * Time.deltaTime;
        m_controller.SimpleMove(displacement);
        // Keep the character orientated in the direction of movement.
        transform.rotation = Quaternion.LookRotation(
            m_controller.velocity);

        // If close enough to current waypoint switch to next waypoint.
        Vector2 transform2d = new Vector2(transform.position.x, transform.position.z);
        Vector2 waypoint2d = new Vector2(m_currentPath.vectorPath[m_currentWaypoint].x, m_currentPath.vectorPath[m_currentWaypoint].z);
        if (Vector2.Distance(transform2d, waypoint2d) < WaypointArrivalThreshold)
        {
            m_currentWaypoint++;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateSleeping()
    {
        m_sleepingDuration += Time.deltaTime;
        if (m_sleepingDuration > MinSleepDuration)
        {
            if (Random.value < WakeUpChance)
            {
                m_mode = GolemMode.Idling;
                m_animator.SetBool("Sleeping", false);
                return;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private float LinearTransform(float x, float a, float b, float c, float d)
    {
        return (x - a) / (b - a) * (d - c) + c;
    }
}

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
        Hunting = 3,
        Attack1
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
    public float PatrolChance = 0.005f;
    public float PatrolRadius = 15f;
    bool m_playerDetected = false;
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
        else if (m_mode == GolemMode.Attack1)
        {
            UpdateAttack1();
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


    private bool m_attackStart = true;
    private float m_attackTime;
    private void UpdateAttack1()
    {
        if (m_attackStart)
        {
            m_attackTime = 0;
            m_attackStart = false;
        }

        m_attackTime += Time.deltaTime;
        if (m_attackTime > 2.76)
        {
            m_attackTime = 0;
            m_attackStart = true;

            m_currentPath = null;
            m_currentWaypoint = -1;
            m_animator.SetBool("Attack1", false);

            m_mode = GolemMode.Idling;
        }
    }

    private void UpdateIdling()
    {




        if (m_playerDetected)
        {
            WalkSpeed = 0;
            // If within striking range attack the player at random intervals. If 
            // not, attempt to get a path to the player and resume hunting.
            if (m_playerDistance < 3)
            {
                Vector3 displacement = Player.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, displacement);

                if (Random.value < 0.25f && Mathf.Abs(angle) < 10)
                {
                    float r = Random.value;
                    if (r < 1)
                    {
                        m_animator.SetBool("Attack1", true);
                        m_mode = GolemMode.Attack1;
                    }
                    return;
                }

                // Reorientate the controller to face towards the player.
                Vector3 direction = Player.transform.position - transform.position;
                direction.Normalize();
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 3.5f * Time.deltaTime);
            }
            else
            {
                Path path = m_seeker.StartPath(transform.position,
                    Player.transform.position);
                if (!path.error)
                {
                    // Reset pathfinding variables.
                    m_lastPathfindingUpdate = 0f;
                    m_currentPath = path;
                    m_currentWaypoint = 0;
                    // Change the change to skeleton state and update animation 
                    // variables.
                    m_mode = GolemMode.Hunting;

                    m_animator.SetFloat("Speed", 1);

                }
            }
        }

        else
        {
            m_playerDetected = DetectPlayer();
            if (m_playerDetected) return;

            if (Random.value < sleepChance)
            {
                m_mode = GolemMode.Sleeping;
                m_animator.SetBool("Sleeping", true);
                return;
            }

            // Check if the controller should transition from idle to patrolling.
            if (Random.value < PatrolChance)
            {
                // Get a point within a radius of PatrolRadius units.
                Vector2 randomPoint = Random.insideUnitCircle * PatrolRadius;
                Vector3 end = transform.position;
                end.x += randomPoint.x;
                end.y = 0.5f;
                end.z += randomPoint.y;

                Path path = m_seeker.StartPath(transform.position, end);
                if (!path.error)
                {
                    m_lastPathfindingUpdate = 0f;
                    m_currentPath = path;
                    m_currentWaypoint = 0;
                    m_mode = GolemMode.Patrolling;

                    m_animator.SetFloat("Speed", 0.5f);

                    return;
                }
            }
        }








    }

    private float m_lastPathfindingUpdate = 0f;



    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool DetectPlayer()
    {
        // Check if the player is within detection range, and if so, attempt 
        // detection.
        if (m_playerDistance < MaxDetectionDistance)
        {
            // Calculate the chance of detection based on the range to 
            // the player. 
            float playerDetectionChance = LinearTransform(m_playerDistance, 0,
                MaxDetectionDistance, MaxDetectionChance, 0);
            if (Random.value < playerDetectionChance)
            {
                // If we have detected the player, attempt to get a path.
                if (m_seeker.IsDone())
                {
                    Path path = m_seeker.StartPath(transform.position,
                        Player.transform.position);
                    if (!path.error)
                    {
                        // Reset pathfinding variables.

                        m_lastPathfindingUpdate = 0f;
                        m_currentPath = path;
                        m_currentWaypoint = 0;
                        // Change the change to skeleton state and update animation 
                        // variables.
                        m_mode = GolemMode.Hunting;

                        m_animator.SetFloat("Speed", 1);

                        return true;

                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdatePatrolling()
    {



        m_playerDetected = DetectPlayer();
        if (m_playerDetected) return;

        // Test if we have just reached the end of the path. 
        if (m_currentWaypoint >= m_currentPath.vectorPath.Count)
        {
            m_currentPath = null;
            m_currentWaypoint = -1;
            m_animator.SetFloat("Speed", 0);
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

        if (m_playerDistance < 3.0)
        {

            m_animator.SetFloat("Speed", 0);
            m_mode = GolemMode.Idling;
            return;
        }

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
        if (m_pathfindingUpdateTime > HuntingPathfindingInterval)
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
    public float pushPower = 2.0F;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.gameObject.CompareTag("Enemy") ||
            hit.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            m_controller.Move(pushDir * pushPower);
        }
    }
}

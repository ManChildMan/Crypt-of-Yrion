using UnityEngine;
using System.Collections;
using Pathfinding;

public class MonsterController : MonoBehaviour
{

    public PlayerController Player;
    public float MaxDetectionDistance = 12f;
    public float MaxDetectionChance = 0.15f;
    public float WaypointArrivalThreshold = 0.1f;
    public float PatrolChance = 0.005f;
    public float PatrolRadius = 15f;
    public float WalkSpeed = 50f;
    public float HuntingPathfindingInterval = 3.0f;

    enum MonsterMode
    {
        Idle,
        Patrolling,
        Hunting,
        Attack1,
        Attack2,
        Attack3,
        Roar,
        Dying
    }


    private Animator m_animator;
    private CharacterController m_controller;
    private Seeker m_seeker;
    private MonsterMode m_mode = MonsterMode.Idle;
    private float m_playerDistance;
    private Path m_currentPath;
    private int m_currentWaypoint = -1;
    private Vector3 m_lastPlayerPos;
    private float m_lastPathfindingUpdate = 0f;
    private bool m_attackStart = true;
    private float m_attackTime;


    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_seeker = GetComponent<Seeker>();
    }


    void Update()
    {
        // Cache distance from the controller to the player.
        m_playerDistance = Vector3.Distance(transform.position,
            Player.transform.position);

        if (m_mode == MonsterMode.Idle)
        {
            UpdateIdle();
        }
        else if (m_mode == MonsterMode.Patrolling)
        {
            UpdatePatrolling();
        }
        else if (m_mode == MonsterMode.Hunting)
        {
            UpdateHunting();
        }
        else if (m_mode == MonsterMode.Attack1)
        {
            UpdateAttack1();
        }
        else if (m_mode == MonsterMode.Attack2)
        {
            UpdateAttack2();
        }
        else if (m_mode == MonsterMode.Attack3)
        {
            UpdateAttack3();
        }
        else if (m_mode == MonsterMode.Roar)
        {
            UpdateRoar();
        }
        //else if (m_mode == SkeletonMode.TakingDamage)
        //{
        //    UpdateTakingDamage();
        //}
        //else if (m_mode == SkeletonMode.KnockedBack)
        //{
        //    UpdateKnockedBack();
        //}
        //else if (m_mode == SkeletonMode.Dying)
        //{
        //    UpdateDying();
        //}   
    }

    bool m_playerDetected = false;
    private void UpdateIdle()
    {

        if (m_playerDetected)
        {
            WalkSpeed = 0;
            // If within striking range attack the player at random intervals. If 
            // not, attempt to get a path to the player and resume hunting.
            if (m_playerDistance < 2.5)
            {
                Vector3 displacement = Player.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, displacement);

                if (Random.value < 0.25f && Mathf.Abs(angle) < 10)
                {
                    float r = Random.value;
                    if (r < 0.33)
                    {
                        m_animator.SetBool("Attack1", true);
                        m_mode = MonsterMode.Attack1;
                    }
                    else if  (r < 0.66)
                    {
                        m_animator.SetBool("Attack2", true);
                        m_mode = MonsterMode.Attack2;
                    }
                    else if (r < 82.5)
                    {
                        m_animator.SetBool("Attack3", true);
                        m_mode = MonsterMode.Attack3;
                    }
                    else
                    {
                        m_animator.SetBool("Roar", true);
                        m_mode = MonsterMode.Roar;
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
                        m_mode = MonsterMode.Hunting;

                        m_animator.SetFloat("Speed", 1);

                    }
                }
            }
        }

        else
        {
            m_playerDetected = DetectPlayer();
            if (m_playerDetected) return;

            // Check if the controller should transition from idle to patrolling.
            if (Random.value < PatrolChance)
            {
                // Get a point within a radius of PatrolRadius units.
                Vector2 randomPoint = Random.insideUnitCircle * PatrolRadius;
                Vector3 end = transform.position;
                end.x += randomPoint.x;
                end.y = 0.5f;
                end.z += randomPoint.y;
                if (m_seeker.IsDone())
                {
                    Path path = m_seeker.StartPath(transform.position, end);
                    if (!path.error)
                    {
                        m_lastPathfindingUpdate = 0f;
                        m_currentPath = path;
                        m_currentWaypoint = 0;
                        m_mode = MonsterMode.Patrolling;

                        m_animator.SetFloat("Speed", 0.5f);
                        m_animator.SetBool("FacingOff", false);
                        return;
                    }
                }
            }
        }
    }

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
            m_animator.SetBool("FacingOff", false);
            m_mode = MonsterMode.Idle;
            return;
        }

        Move();
    }

    private void UpdateHunting()
    {
        WalkSpeed = 75;
        if (m_playerDistance < 2.0)
        {

            m_animator.SetFloat("Speed", 0);
            m_mode = MonsterMode.Idle;
            return;
        }


        // Check if we have reached the end of the current path. 
        if (m_currentWaypoint >= m_currentPath.vectorPath.Count)
        {
            // If so, find a new path to the player.
            Path path = m_seeker.StartPath(transform.position, Player.transform.position);
            if (!path.error)
            {
                m_lastPathfindingUpdate = 0f;
                m_lastPlayerPos = Player.transform.position;
                m_currentPath = path;
                m_currentWaypoint = 0;
            }
        }

        // If the player has moved significantly since last pathfinding and more
        // than the designated pathfinding interval has passed...
        m_lastPathfindingUpdate += Time.deltaTime;
        if (m_lastPathfindingUpdate > HuntingPathfindingInterval)
        {
            // Re-path to the player and reset pathfinding variables.
            if (m_seeker.IsDone())
            {
                Path path = m_seeker.StartPath(transform.position, Player.transform.position);
                if (!path.error)
                {
                    m_lastPlayerPos = Player.transform.position;
                    m_currentPath = path;
                    m_currentWaypoint = 0;
                }
                m_lastPathfindingUpdate = 0f;
            }
        
        }

        Move();
    }

    private void UpdateFacingOff()
    {


    }

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
            m_mode = MonsterMode.Idle;
        }
    }

    private void UpdateAttack2()
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
            m_animator.SetBool("Attack2", false);
            m_mode = MonsterMode.Idle;
        }
    }

    private void UpdateAttack3()
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
            m_animator.SetBool("Attack3", false);
            m_mode = MonsterMode.Idle;
        }
    }



    private void UpdateRoar()
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
            m_animator.SetBool("Roar", false);
            m_mode = MonsterMode.Idle;
        }
    }

    private void Move()
    {
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
                        m_mode = MonsterMode.Hunting;

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

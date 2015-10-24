using UnityEngine;
using System.Collections;
using Pathfinding;

public class SkeletonController : MonoBehaviour 
{
   
    public PlayerController Player;

    public float MaxDetectionDistance = 12f;
    public float MaxDetectionChance = 0.15f;
    public float WaypointArrivalThreshold = 0.1f;
    public float PatrolChance = 0.005f;
    public float PatrolRadius = 15f;
    public float WalkSpeed = 50f;
    enum SkeletonMode
    {
        Idle,
        Patrolling,
        Hunting,
        FacingOff,
        Attacking,
        TakingDamage,
        KnockedBack,
        Dying
    }


    private Animator m_animator;
    private CharacterController m_controller;
    private Seeker m_seeker;
    private SkeletonMode m_mode = SkeletonMode.Idle;
    private float m_playerDistance;
    private Path m_currentPath;
    private int m_currentWaypoint = -1;
    private Vector3 m_lastPlayerPos;
	void Start () {
        m_animator = GetComponentInChildren<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_seeker = GetComponent<Seeker>();
	}
	

	void Update () 
    {
        // Cache distance from the controller to the player.
        m_playerDistance = Vector3.Distance(transform.position,
            Player.transform.position);

        if (m_mode == SkeletonMode.Idle)
        {
            UpdateIdle();
        }
        else if (m_mode == SkeletonMode.Patrolling)
        {
            UpdatePatrolling();
        }
        else if (m_mode == SkeletonMode.Hunting)
        {
            UpdateHunting();
        }
        else if (m_mode == SkeletonMode.FacingOff)
        {
            UpdateFacingOff();
        }
        else if (m_mode == SkeletonMode.Attacking)
        {
            UpdateAttacking();
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



    private void UpdateFacingOff()
    {

        if (m_playerDistance < 2.5)
        {
            Vector3 directionToTarget = Player.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (Random.value < 0.1f && Mathf.Abs(angle) < 10)
            {
         
                m_animator.SetBool("Attack", true);
                m_animator.SetBool("FacingOff", false);


                m_mode = SkeletonMode.Attacking;
                return;
            }
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
                m_mode = SkeletonMode.Hunting;

                m_animator.SetBool("Hunt", true);
                m_animator.SetBool("Patrol", false);
                m_animator.SetBool("FacingOff", false);
                return;
            }
            
        }

        // Orientate to player
        Vector3 direction = Player.transform.position - transform.position;
        direction.Normalize();
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 3.5f * Time.deltaTime);
    }


    private float m_lastPathfindingUpdate = 0f;


    private bool m_attackStart = true;
    private float m_attackTime;
    private void UpdateAttacking()
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
            m_animator.SetBool("Attack", false);
            m_animator.SetBool("FacingOff", true);
            m_mode = SkeletonMode.FacingOff;
        }
    }


    private void UpdateIdle()
    {

        if (DetectPlayer()) return;

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
                m_mode = SkeletonMode.Patrolling;
             
                m_animator.SetBool("Hunt", false);
                m_animator.SetBool("Patrol", true);
                m_animator.SetBool("FacingOff", false);
                return;
            }     
        }
    }

    private void UpdatePatrolling()
    {
        WalkSpeed = 50;
        if (DetectPlayer()) return;

        // Test if we have just reached the end of the path. 
        if (m_currentWaypoint >= m_currentPath.vectorPath.Count)
        {
            m_currentPath = null;
            m_currentWaypoint = -1;
   
            m_animator.SetBool("Hunt", false);
            m_animator.SetBool("Patrol", false);
            m_animator.SetBool("FacingOff", false);
            m_mode = SkeletonMode.Idle;
            return;
        }

        Move();
    }
    public float HuntingPathfindingInterval = 3.0f;
    private void UpdateHunting()
    {
        WalkSpeed = 75;
        if (m_playerDistance < 2.0)
        {
            
            m_animator.SetBool("Hunt", false);
            m_animator.SetBool("Patrol", false);
            m_animator.SetBool("FacingOff", true);
            m_mode = SkeletonMode.FacingOff;
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
            Path path = m_seeker.StartPath(transform.position, Player.transform.position);
            if (!path.error)
            {
                m_lastPlayerPos = Player.transform.position;
                m_currentPath = path;
                m_currentWaypoint = 0;
            }
            m_lastPathfindingUpdate = 0f;
        }

        Move();
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
                    m_mode = SkeletonMode.Hunting;
                 
                    m_animator.SetBool("Hunt", true);
                    m_animator.SetBool("Patrol", false);
                    m_animator.SetBool("FacingOff", false);
                    return true;
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
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }

    private void OnTriggerEnter(Collider collision)
    {


        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            // Idle.
            m_currentPath = null;
            m_currentWaypoint = -1;

            m_animator.SetBool("Hunt", false);
            m_animator.SetBool("Patrol", false);
            m_animator.SetBool("FacingOff", false);
            m_mode = SkeletonMode.Idle;
            return;
        }


    }
}

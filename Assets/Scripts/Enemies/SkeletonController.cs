using UnityEngine;
using System.Collections;
using Pathfinding;

public class SkeletonController : MonoBehaviour 
{
    private PlayerController Player;
   
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
        Attack1,
        Dead,
    }


    private Animator m_animator;
    private CharacterController m_controller;
    private Seeker m_seeker;
    private SkeletonMode m_mode = SkeletonMode.Idle;
    private float m_playerDistance;
    private Path m_currentPath;
    private int m_currentWaypoint = -1;
    private Vector3 m_lastPlayerPos;
    private float m_lastPathfindingUpdate = 0f;
    private bool m_attackStart = true;
    private float m_attackTime;
    public int CurrentHealth = 20;
    public float HuntingPathfindingInterval = 3.0f;
    bool m_playerDetected = false;

    private bool displayObjectName;
    private string objectName;
    private int MaxHealth;

	void Start () {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_animator = GetComponentInChildren<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_seeker = GetComponent<Seeker>();

        objectName = "Skeleton";
        MaxHealth = CurrentHealth;
	}
	

	void Update () 
    {
        // Die if health less than 0.
        if (CurrentHealth < 0)
        {
            m_animator.SetBool("Dead", true);
            m_mode = SkeletonMode.Dead;
            GameObject.Destroy(this.gameObject, 5);
        }
        
        
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
        else if (m_mode == SkeletonMode.Attack1)
        {
            UpdateAttack1();
        }
        else if (m_mode == SkeletonMode.Dead)
        {
            UpdateDead();
        }

	}




    private void UpdateIdle()
    {

        if (m_playerDetected)
        {
            WalkSpeed = 0;
            // If within striking range attack the player at random intervals. If 
            // not, attempt to get a path to the player and resume hunting.
            if (m_playerDistance < 3f)
            {
                Vector3 displacement = Player.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, displacement);

                if (Random.value < 0.05f && Mathf.Abs(angle) < 10)
                {
                    float r = Random.value;
                    if (r < 1)
                    {
                        m_animator.SetBool("Attack1", true);
                        m_mode = SkeletonMode.Attack1;
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
                    m_mode = SkeletonMode.Hunting;
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

                if (m_seeker.IsDone())
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

                    m_animator.SetFloat("Speed", 0.5f);
          
                    return;
                }
            }
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

            m_animator.SetFloat("Speed", 0);
            m_mode = SkeletonMode.Idle;
            return;
        }

        if (m_currentWaypoint >= 0 &&
            m_currentWaypoint < m_currentPath.vectorPath.Count - 1) Move();
    }
    
    

    private void UpdateHunting()
    {
        WalkSpeed = 75;
        
        if (m_playerDistance < 2.0)
        {
            m_animator.SetFloat("Speed", 0f);
            m_mode = SkeletonMode.Idle;
            return;
        }


        // Check if we have reached the end of the current path. 
        if (m_currentPath == null || m_currentWaypoint >= m_currentPath.vectorPath.Count)
        {
            if (m_seeker.IsDone())
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
        }

        // If the player has moved significantly since last pathfinding and more
        // than the designated pathfinding interval has passed...
        m_lastPathfindingUpdate += Time.deltaTime;
        if (m_lastPathfindingUpdate > HuntingPathfindingInterval)
        {
            if (m_seeker.IsDone())
            {
            // Re-path to the player and reset pathfinding variables.
            Path path = m_seeker.StartPath(transform.position, Player.transform.position);
            if (!path.error)
            {
                m_lastPathfindingUpdate = 0f;
                m_lastPlayerPos = Player.transform.position;
                m_currentPath = path;
                m_currentWaypoint = 0;
            }
            }

        }

        if (m_currentWaypoint >= 0 &&
            m_currentWaypoint < m_currentPath.vectorPath.Count - 1) Move();
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

            m_mode = SkeletonMode.Idle;
        }
    }




    private void UpdateDead()
    {



    }



    private void Move()
    {        


        Vector3 pp = m_currentPath.vectorPath[m_currentWaypoint];

        Vector3 direction = (pp -
            transform.position).normalized;
        Vector3 displacement = direction * WalkSpeed * Time.deltaTime;
        m_controller.SimpleMove(displacement);
        
        // Keep the player orientated in the direction of movement.
        if(m_controller.velocity.magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(
                m_controller.velocity);

        // If close enough to current waypoint switch to next waypoint.
        Vector2 transform2d = new Vector2(transform.position.x, transform.position.z);
        Vector2 waypoint2d = new Vector2(
            pp.x,
            pp.z);
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
                if (m_seeker.IsDone())
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








    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    void OnMouseEnter()
    {
        displayObjectName = true;
    }
        
    void OnMouseExit()
    {
        displayObjectName = false;
    }

    void OnGUI()
    {
        if (displayObjectName)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x - 155, Event.current.mousePosition.y, 150, 25), objectName);
        }
        if (!StateMigrator.anyWindowOpen && CurrentHealth > 0)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 3.0f, 0.0f));
            screenPosition.y = Screen.height - (screenPosition.y + 1);
            Rect rect = new Rect(screenPosition.x - MaxHealth / 2, screenPosition.y - 12, MaxHealth, 24);
            GUI.color = Color.red;
            GUI.HorizontalScrollbar(rect, 0, CurrentHealth, 0, MaxHealth);
        }
    }
}

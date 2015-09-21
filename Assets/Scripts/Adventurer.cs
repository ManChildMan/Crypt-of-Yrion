using Pathfinding;
using UnityEngine;


public class Adventurer : MonoBehaviour
{
    public float Speed = 100;
    public float WaypointArrivalThreshold = 0.1f;

    private CharacterController m_characterController;
    private Seeker m_seeker;
    private Animator m_animator;

    public LayerMask MouseSelectionLayerMask;

    private bool m_isMoving = false;
    private int m_currentWaypoint = -1;

    private Path m_path;
    
    public void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_seeker = GetComponent<Seeker>();
        m_animator = GetComponentInChildren<Animator>();
    }



    public void Update()
    {
        if (Input.GetMouseButtonUp(1) && !m_isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, MouseSelectionLayerMask))
            {
                // Start a new path to mouse cursor world position. Return the 
                // result to the OnPathComplete function.
                m_seeker.StartPath(transform.position, hit.point, 
                    OnPathComplete);
            }         
        }

        if (m_path == null)
        {
            return;
        }

        // Handle reaching end of path.
        if (m_currentWaypoint >= m_path.vectorPath.Count)
        {
            m_path = null;
            m_currentWaypoint = -1;
            m_animator.SetFloat("Speed", 0);
            m_isMoving = false;
            return;
        }
        
        // Find direction and distance to the next waypoint and move the
        // adventurer via the character controller.
        Vector3 direction = (m_path.vectorPath[m_currentWaypoint] -
            transform.position).normalized;
        Vector3 displacement = direction * Speed * Time.deltaTime;
        m_characterController.SimpleMove(displacement);
        // Keep the adventurer looking in direction of movement.
        transform.rotation = Quaternion.LookRotation(
            m_characterController.velocity);

        // If we are close enough to the next waypoint, proceed to follow
        // the next waypoint.
        if (Vector3.Distance(transform.position, m_path.vectorPath[
            m_currentWaypoint])< WaypointArrivalThreshold)
        {
            m_currentWaypoint++;
            return;
        }
    }

    public void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            m_path = path;
            m_currentWaypoint = 0;
            m_animator.SetFloat("Speed", 1);
            m_isMoving = true;
        }
    }

    public void OnDisable()
    {
        m_seeker.pathCallback -= OnPathComplete;
    }
}

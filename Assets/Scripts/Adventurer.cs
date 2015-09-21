using System.Collections.Generic;
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




    private int m_baseHealth = 0;
    private int m_baseStamina = 0;
    private int m_baseTimeUnits = 0;
    private int m_health = 0;
    private int m_stamina = 0;
    private int m_timeUnits = 0;

    private int m_morale = 0;
    private int m_hunger = 0;
    private int m_thirst = 0;

    private int m_baseStrength = 0;
    private int m_baseEndurance = 0;
    private int m_baseAgility = 0;
    private int m_baseIntelligence = 0;
    private int m_baseWillpower = 0;

    private int m_strength = 0;
    private int m_endurance = 0;
    private int m_agility = 0;
    private int m_intelligence = 0;
    private int m_willpower = 0;

    // Two units of weight per unit of strength is initial encumbrance.
    private int m_baseEncumbrance = 0;
    private int m_encumbrance = 0;
    private int m_experience = 0;
    private int m_level = 0;

    private int m_armour = 0;

    private bool m_isOverburdened = false;
    private bool m_isCrippled = false;
    private bool m_isStarving = false;
    private bool m_isSleepDeprived = false;
    private bool m_isPoisoned = false;
    private bool m_isBurning = false;
    private bool m_isSuffocating = false;
    private bool m_isPanicing = false;
    private bool m_isThirsty = false;

    private Dictionary<string, Item> m_inventory = 
        new Dictionary<string,Item>();

    void GenerateStats()
    {
        m_baseStrength = Random.Range(30, 65);
        m_baseEndurance = Random.Range(30, 65);
        m_baseAgility = Random.Range(30, 65);
        m_baseIntelligence = Random.Range(30, 65);
        m_baseWillpower = Random.Range(30, 65);
        m_baseHealth = (int)Mathf.Round(m_baseEndurance * 1.75f);
        m_baseStamina = (int)Mathf.Round(
            ((m_baseEndurance + m_baseWillpower) / 2) * 1.75f);
        m_baseTimeUnits = (m_baseStamina + m_baseAgility) / 2;
        m_baseEncumbrance = m_baseStrength * 2;

        m_strength = m_baseStrength;
        m_endurance = m_baseEndurance;
        m_agility = m_baseAgility;
        m_intelligence = m_baseIntelligence;
        m_willpower = m_baseWillpower;
        m_health = m_baseHealth;
        m_stamina = m_baseStamina;
        m_timeUnits = m_baseTimeUnits;
        m_encumbrance = m_baseEncumbrance;

        m_experience = 0;
        m_level = 0;
        m_armour = 0;
    }









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

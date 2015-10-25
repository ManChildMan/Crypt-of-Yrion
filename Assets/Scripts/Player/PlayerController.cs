﻿using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    public float WaypointArrivalThreshold = 0.4f;
    public LayerMask MouseSelectionLayerMask;

    private Animator m_animator;
    private CharacterController m_controller;
    private Seeker m_seeker;
    private Path m_path;
    private bool m_moving = false;
    private int m_currentWaypoint = -1;

    private Player player;
    public int MaxHealth;
    public int CurrentHealth;
    private int Heal;
    private int IncHeal = 10;
    private float HealTimer = 0f;

    public float Speed;
    public int Attack;

    public bool attacking;
    public float attackingDuration;
    
    public void Start()
    {
        player = GetComponent<Player>();
        MaxHealth = player.Health;
        CurrentHealth = MaxHealth;

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
            case PortalAction.ExitLevel3:
                transform.position = new Vector3(0.0f, 0.0f, -10.0f);
                break;
        } 
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {

        HealTimer += Time.deltaTime;

        Speed = player.Speed;
        Attack = player.Attack;

        if (MaxHealth < player.Health)
        {
            Heal = player.Health - MaxHealth;
            CurrentHealth = CurrentHealth + Heal;
            MaxHealth = player.Health;
        }

        if (HealTimer > 10 && CurrentHealth < MaxHealth)
        {
            CurrentHealth = CurrentHealth + IncHeal;
            HealTimer = 0;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            m_animator.SetTrigger("Attack");
            attacking = true;
            attackingDuration = 1.0f;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, MouseSelectionLayerMask))
            {
                Vector3 attackDirection = hit.point - transform.position;
                transform.rotation = Quaternion.LookRotation(new Vector3(attackDirection.x, 0.0f, attackDirection.z));
            }
        }

        if (attacking)
        {
            attackingDuration -= Time.deltaTime;
            if (attackingDuration <= 0.0f)
            {
                attacking = false;
            }
        }

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
        Vector3 displacement = direction * Speed * Time.deltaTime;
        m_controller.SimpleMove(displacement);
        // Keep the adventurer looking in direction of movement (ignores y axis)
        if (!attacking)
        {
            transform.rotation = Quaternion.LookRotation(
                new Vector3(m_controller.velocity.x, 0.0f, m_controller.velocity.z));
        }

        // If close enough to current waypoint switch to next waypoint.
        if (Vector3.Distance(transform.position, m_path.vectorPath[
            m_currentWaypoint]) < WaypointArrivalThreshold)
        {
            m_currentWaypoint++;
        }

        if (transform.position.y < -10.0f)
        {
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
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
            // If path vector list length is greater than 1 then immediately move to the 2nd element instead.
            // Lets the player move more smoothly, especially when spam right clicking.
            if (m_path.vectorPath.Count > 1)
            {
                m_currentWaypoint++;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnDisable()
    {
        m_seeker.pathCallback -= OnPathComplete;
    }
    
    public void StopMoving()
    {
        m_path = null;
        m_currentWaypoint = -1;
        m_animator.SetFloat("Speed", 0);
        m_moving = false;
    }

    void OnGUI()
    {
        if (!StateMigrator.anyWindowOpen)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 2.0f, 0.0f));
            screenPosition.y = Screen.height - (screenPosition.y + 1);
            Rect rect = new Rect(screenPosition.x - MaxHealth / 2, screenPosition.y - 12, MaxHealth, 24);
            GUI.color = Color.green;
            GUI.HorizontalScrollbar(rect, 0, CurrentHealth, 0, MaxHealth);
        }
    }
}

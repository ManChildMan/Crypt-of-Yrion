using UnityEngine;
using Pathfinding;
using System.Collections;

public class BossController : MonoBehaviour {

    private const float WaypointArrivalThreshold = 0.4f;

    private Animator animator;
    private CharacterController controller;
    private Seeker seeker;
    private Path path;

    private bool moving = false;
    private int currentWayPoint = -1;
    private int prevCurrentWayPoint = -1;
    private bool pathLoading;
    private Vector3 direction;

    private Transform target;
    private PlayerController playerController;
    private float distance;

    private bool displayObjectName;
    private string objectName;
    private Renderer[] renderers;
    private Color[] rendererStartColors;

    private bool doingAttackAnimation;
    private string attackAnimationName;
    private float animationTimer;
    private bool damageDealt;

    public int health;
    private int maxHealth;
    public float speed;
    public int attack;

    private State state;
    private enum State
    {
        WaitingForPlayer,
        WalkingToPlayer,
        AttackingPlayer,
        Dying,
        None,
    }

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        maxHealth = health;
        objectName = this.gameObject.name;
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        seeker = GetComponent<Seeker>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        target = player.transform;

        renderers = GetComponentsInChildren<Renderer>();
        rendererStartColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            rendererStartColors[i] = renderers[i].material.color;
        }

        animator.SetFloat("Speed", 1.0f);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        switch (state)
        {
            case State.Dying:
                UpdateDying();
                break;
            case State.WaitingForPlayer:
                UpdateWaitingForPlayer();
                break;
            case State.WalkingToPlayer:
                UpdateWalkingToPlayer();
                break;
            case State.AttackingPlayer:
                UpdateAttackingPlayer();
                break;
        }

        //Test to see if Player is in range to attack
        //If so, stop and attack
        if (distance <= 1.5)
        {
            path = null;
            currentWayPoint = -1;
            moving = false;
            //transform.rotation = Quaternion.LookRotation(
            //m_controller.velocity);
            animator.SetBool("Attack1", false);
            animator.SetBool("Attack2", false);
            animator.SetBool("Attack3", true);
            return;
        }
    }

    private void UpdateWaitingForPlayer()
    {
        distance = Vector3.Distance(transform.position, target.position);
        if (distance <= 10.0f)
        {
            state = State.WalkingToPlayer;
        }
    }

    private void UpdateWalkingToPlayer()
    {
        distance = Vector3.Distance(transform.position, target.position);
        if (health <= 0)
        {
            state = State.Dying;
            return;
        }
        if (!moving)
        {
            if (!pathLoading)
            {
                seeker.StartPath(transform.position, target.position, OnPathComplete);
                pathLoading = true;
            }
        }
        else
        {
            bool setRotation = false;
            if (prevCurrentWayPoint != currentWayPoint)
            {
                prevCurrentWayPoint = currentWayPoint;
                direction = (path.vectorPath[currentWayPoint] - transform.position).normalized;
                setRotation = true;
            }
            controller.SimpleMove(direction * speed * Time.deltaTime);
            if (setRotation)
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(controller.velocity.x, 0.0f, controller.velocity.z));
            }
            if (Vector3.Distance(transform.position, path.vectorPath[
                currentWayPoint]) < WaypointArrivalThreshold)
            {
                currentWayPoint++;
            }
            if (currentWayPoint >= path.vectorPath.Count || distance < 2.0f)
            {
                currentWayPoint = -1;
                prevCurrentWayPoint = -1;
                moving = false;
                if (distance < 2.0f)
                {
                    state = State.AttackingPlayer;
                    animationTimer = 0.3f;
                }
            }
        }
    }

    public void OnPathComplete(Path path)
    {
        pathLoading = false;
        if (!path.error)
        {
            this.path = path;
            currentWayPoint = 0;
            moving = true;
        }
    }

    private void UpdateAttackingPlayer()
    {
        Vector3 directionToTarget = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0.0f, directionToTarget.z));
        if (!doingAttackAnimation)
        {
            // Stand for 0.8 of a second before attacking.
            if (animationTimer > 0.0f)
            {
                animationTimer -= Time.deltaTime;
                if (animationTimer <= 0.0f)
                {
                    doingAttackAnimation = true;
                    animator.SetBool("Attack3", true);
                    animationTimer = 4.0f;
                }
            }
        }
        else
        {
            animationTimer -= Time.deltaTime;
            if (animationTimer <= 3.0f && !damageDealt)
            {
                damageDealt = true;
                if (Vector3.Distance(transform.position, target.position) < 2.0f)
                {
                    playerController.CurrentHealth -= attack;
                }
            }
            if (animationTimer <= 0.0f)
            {
                doingAttackAnimation = false;
                animator.SetBool("Attack3", false);
                state = State.WalkingToPlayer;
                damageDealt = false;
            }
        }
    }

    private void UpdateDying()
    {
        animator.SetFloat("Speed", 0.0f);
        animator.SetBool("Attack1", false);
        animator.SetBool("Die", true);
        state = State.None;
        GameObject.Find("Canvas").GetComponent<UIManager>().StartEndGameTransition();
    }

    void OnGUI()
    {
        if (displayObjectName)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x - 155, Event.current.mousePosition.y, 150, 25), objectName);
        }
        if (!StateMigrator.anyWindowOpen && health > 0)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 3.0f, 0.0f));
            screenPosition.y = Screen.height - (screenPosition.y + 1);
            Rect rect = new Rect(screenPosition.x - maxHealth / 2, screenPosition.y - 12, maxHealth, 24);
            GUI.color = Color.red;
            GUI.HorizontalScrollbar(rect, 0, health, 0, maxHealth);
        }
    }

    void OnMouseEnter()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = Color.red;
        }
        displayObjectName = true;
    }

    void OnMouseExit()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = rendererStartColors[i];
        }
        displayObjectName = false;
    }
}

using UnityEngine;

/// <summary>
/// Represents an entity with stats.
/// </summary>
public class Entity : MonoBehaviour
{
    public int baseHealth;
    public int baseSpeed;
    public int baseAttack;

    protected int totalHealth;
    protected int totalSpeed;
    protected int totalAttack;

    public virtual void Start()
    {
        totalHealth = baseHealth;
        totalSpeed = baseSpeed;
        totalAttack = baseAttack;
    }

    /// <summary>
    /// Gets the maximum health stats of the entity (not current health).
    /// </summary>
    public int Health
    {
        get
        {
            return totalHealth;
        }
    }

    /// <summary>
    /// Gets the speed stats of the entity.
    /// </summary>
    public int Speed
    {
        get
        {
            return totalSpeed;
        }
    }

    /// <summary>
    /// Gets the attack stat of the entity.
    /// </summary>
    public int Attack
    {
        get
        {
            return totalAttack;
        }
    }
}

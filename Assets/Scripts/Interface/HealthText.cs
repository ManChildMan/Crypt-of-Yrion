using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the health value in stats window when necessary.
/// </summary>
public class HealthText : MonoBehaviour
{
    // References set by unity.
    public Player player;

    private int prevHealth;
    private Text healthText;

    void Start()
    {
        healthText = GetComponent<Text>();
    }

    void Update()
    {
        int health = player.Health;
        if (prevHealth != health)
        {
            prevHealth = health;
            healthText.text = string.Format("Max Health: {0:n0}", health);
        }
    }
}

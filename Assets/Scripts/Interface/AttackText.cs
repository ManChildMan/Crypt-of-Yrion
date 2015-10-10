using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the attack value in stats window when necessary.
/// </summary>
public class AttackText : MonoBehaviour
{
    // References set by unity.
    public Player player;

    private int prevAttack;
    private Text attackText;

    void Start()
    {
        attackText = GetComponent<Text>();
    }

    void Update()
    {
        int attack = player.Attack;
        if (prevAttack != attack)
        {
            prevAttack = attack;
            attackText.text = string.Format("Attack: {0:n0}", attack);
        }
    }
}

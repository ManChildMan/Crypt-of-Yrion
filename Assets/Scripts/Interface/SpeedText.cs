using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the speed value in stats window when necessary.
/// </summary>
public class SpeedText : MonoBehaviour
{
    // References set by unity.
    public Player player;

    private int prevSpeed;
    private Text speedText;

    void Start ()
    {
        speedText = GetComponent<Text>();
    }
	
	void Update ()
    {
        int speed = player.Speed;
        if (prevSpeed != speed)
        {
            prevSpeed = speed;
            speedText.text = string.Format("Speed: {0:n0}", speed);
        }
    }
}

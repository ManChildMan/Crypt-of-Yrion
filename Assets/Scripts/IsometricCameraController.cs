using UnityEngine;

/// <summary>
/// This script implements a basic isometric camera. 
/// </summary>
public class IsometricCameraController : MonoBehaviour
{
    public Transform Target;

	void Update () 
    {
        transform.position = Target.position;
	}
}

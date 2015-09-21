using UnityEngine;

/// <summary>
/// This script implements a basic isometric camera. Changing the camera from 
/// orthographic to perspective mode changes the look and feel dramatically.
/// </summary>
public class IsometricCameraController : MonoBehaviour
{
    public Transform Target;

	void Update () 
    {
        transform.position = Target.position;
	}
}

using UnityEngine;

// This script is a basic isometric camera. Changing the camera from 
// orthographic to perspective mode changes the feel dramatically.
public class IsometricCameraController : MonoBehaviour
{
    public Transform Target;

	void Update () 
    {
        transform.position = Target.position;
	}
}

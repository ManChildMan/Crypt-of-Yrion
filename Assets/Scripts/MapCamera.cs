using UnityEngine;

/// <summary>
/// This script implements a simple camera that is used in the mapgen scene.
/// </summary>
public class MapCamera : MonoBehaviour
{
    float cameraDistanceMax = 75f;
    float cameraDistanceMin = 10f;
    float cameraDistance = 50f;
    float scrollSpeed = 10f;
	void Update () 
    {
        transform.position += transform.right * Input.GetAxis("Horizontal");
        transform.position += transform.up * Input.GetAxis("Vertical");
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
        transform.position = new Vector3(transform.position.x, cameraDistance, transform.position.z);
    }
}



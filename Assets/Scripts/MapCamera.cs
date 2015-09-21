using UnityEngine;

/// <summary>
/// This script implements a simple camera that is used in the mapgen scene.
/// </summary>
public class MapCamera : MonoBehaviour 
{
	void Update () 
    {
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        transform.position += transform.right * horz;
        transform.position += transform.up * vert;
	}
}

using UnityEngine;
using System.Collections;

public class MapCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        transform.position += transform.right * horz;
        transform.position += transform.up * vert;
	}
}

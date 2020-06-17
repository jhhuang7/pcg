using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {
	// The Camera
	public Camera MainCamera;

	// Start is called before the first frame update
	void Start () {
		MainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		// Move the camera

		// get the horizontal and vertical controls (arrows keys)
		float dz = Input.GetAxis("Vertical");
		float dx = Input.GetAxis("Horizontal");

		// sensitivity factors for translate and rotate
		float translate_factor = 0.5f;
		float rotate_factor = 3.0f;

		// translate forward or backwards
		MainCamera.transform.Translate(0f, 0f, dz * translate_factor);

		// rotate left or right
		MainCamera.transform.Rotate(0f, dx * rotate_factor, 0f);

		// grab the main camera position
		Vector3 cam_pos = MainCamera.transform.position;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {
	public Camera MainCamera;

	void Start () {
		MainCamera = Camera.main;
	}
	
	// Move the camera
	void Update () {
		// get the horizontal and vertical controls (arrows keys)
		float dz = Input.GetAxis ("Vertical");
		float dx = Input.GetAxis ("Horizontal");

		// sensitivity factors for translate and rotate
		float translate_factor = 0.5f;
		float rotate_factor = 3.0f;

		// translate forward or backwards
		MainCamera.transform.Translate (0, 0, dz * translate_factor);

		// rotate left or right
		MainCamera.transform.Rotate (0, dx * rotate_factor, 0);

		// grab the main camera position
		Vector3 cam_pos = MainCamera.transform.position;
	}
}

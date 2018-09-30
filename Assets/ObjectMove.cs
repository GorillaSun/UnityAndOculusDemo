using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour {
	float horizontalSpeed = 2.0f;
	float verticalSpeed = 2.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float h = horizontalSpeed * Input.GetAxis ("Mouse X");
		float v = verticalSpeed * Input.GetAxis ("Mouse Y");
		transform.Rotate (h, v, 0);
	}
}

using UnityEngine;
using System.Collections;

/*
	Class to make customers kind of blink.
*/
public class CustomerBlink : MonoBehaviour {

	// Some parameters
	private const float lo = 0.3f;
	private const float hi = 0.5f;
	private const float step = 0.005f;
	private bool down;

	// Set down
	void Start() {
		down = true;
	}
	
	// Blink the sphere
	void Update () {
		float size = transform.localScale.x;
		if 		(size <= lo) { down = false; }
		else if (size >= hi) { down = true;  }
		if (down) 	{ size -= step; }
		else 		{ size += step; }
		transform.localScale = new Vector3(size, size, size);
	}
}

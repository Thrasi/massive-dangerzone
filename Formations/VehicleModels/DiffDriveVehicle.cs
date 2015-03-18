using UnityEngine;
using System.Collections.Generic;
using Geometry;

public class DiffDriveVehicle : DirectedVehicle<DiffDriveVehicle> {

	// Max speed
	public static float maxSpeed;
	
	// Max angular velocity
	public static float maxOmega;
	

	// Constructor
	public DiffDriveVehicle(GameObject vehicle, DiffDriveVehicle parent,
		Vector3 relativePosition) : base(parent, vehicle, relativePosition) {
	}

	// Moving the differential drive vehicle
	public override void Move(float dt) {
		if (parent == null) {
			return;
		}

		if (Vector3.Distance(position, endPosition) < 0.5f) {
			if (Mathf.Abs(angleToParent) > 0) {
				Rotate(Mathf.Sign(angleToParent) * 
					Mathf.Min(maxOmega*dt, Mathf.Abs(angleToParent)));
			}
			return;
		}

		// Variables used in computations
		float angle = angleToEnd;
		float v = maxSpeed + 5;		// Is this ok?
		float omega = maxOmega.ToRad();

		// Orientation
		Vector3 ori = direction;
		int directionOfSpeed = 1;

		// Forward or backward
		if (Mathf.Abs(angle) > 90) {
			angle = angle - 180 * Mathf.Sign(angle);
			ori = - ori;
			directionOfSpeed = -1;
		}

		float d = Vector3.Distance(position, endPosition);
		float absAngle = Mathf.Abs(angle);
		float radius = (d/2) / Mathf.Sin(absAngle.ToRad());
		float r = maxSpeed / maxOmega.ToRad();

		// Checking which is better, speed or rotation
		if (radius > r) {
			omega = v / radius;
		} else {
			v = omega * radius;
		}

		// Perform movement
		Rotate(omega.ToDeg()*dt * Mathf.Sign(angle*directionOfSpeed));
		Translate(ori * v * dt);
	}
}

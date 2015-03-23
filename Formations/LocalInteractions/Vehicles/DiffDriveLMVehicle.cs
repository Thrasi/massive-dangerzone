using UnityEngine;
using System.Collections;
using Geometry;

public class DiffDriveLMVehicle : LocalMoveVehicle {

	// Max angular velocity
	public static float omega;

	// Max speed
	public static float speed;

	// Threshold to stop moving
	public static float stopThreshold;

	// Direction of the vehicle
	public Vector3 direction { get { return gobj.transform.right; } }


	// Constructor
	public DiffDriveLMVehicle(GameObject gobj) : base(gobj) {
	}

	// Very complex move
	protected override void VehicleMove(float dt) {
		if (resultMove.magnitude < stopThreshold) {
			return;
		}

		// Variables used in computations
		float angle = Tangents.RotationAngle(direction, resultMove);
		float v = speed;
		float om = omega.ToRad();	// Angular velocity

		// Orientation
		Vector3 ori = direction;
		int directionOfSpeed = 1;

		// Forward or backward
		if (Mathf.Abs(angle) > 90) {
			angle = angle - 180 * Mathf.Sign(angle);
			ori = - ori;
			directionOfSpeed = -1;
		}

		float d = Vector3.Distance(position, position + resultMove);
		float absAngle = Mathf.Abs(angle);
		float radius = (d/2) / Mathf.Sin(absAngle.ToRad());
		float r = speed / om;

		// Checking which is better, speed or rotation
		if (radius > r) {
			om = v / radius;
		} else {
			v = om * radius;
		}

		// Perform movement
		Rotate(om.ToDeg() * dt * Mathf.Sign(angle*directionOfSpeed));
		Translate(ori * v * dt);
	}
}

using UnityEngine;
using System.Collections.Generic;
using Geometry;

public class KinematicCarVehicle : DirectedVehicle<KinematicCarVehicle> {

	// Max wheel angle
	public static float maxPhi;

	// Max speed
	public static float maxSpeed;

	// Length ov the car
	public static float L;


	// Constructor
	public KinematicCarVehicle(GameObject gobj, KinematicCarVehicle parent,
		Vector3 relativePosition) : base(parent, gobj, relativePosition) {
	}

	// Move vehicle
	public override void Move(float dt) {
		if (parent == null) {
			return;
		}

		// Some parameters used
		float r = L / Mathf.Tan(maxPhi.ToRad());
		float omega = (maxSpeed / L * Mathf.Tan(maxPhi.ToRad())).ToDeg();
		float angToEnd = angleToEnd;
		float forward = Mathf.Abs(angToEnd) > 90 ? -1 : 1;	// Which way is forward
		float turn = Mathf.Sign(angToEnd*forward);			// Which way to turn

		Circle c = new Circle(r, position, direction, turn < 0);
		if (c.IsInside(endPosition)
			&& Vector3.Distance(position, c.ClosestOn(endPosition)) < 0.5f) {
			// Car will get stuck in endless loop
			// Now it exits when it reaches closest point to end position
			return;
		}

		// Rotation part
		Rotate(turn * Mathf.Min(Mathf.Abs(angToEnd), omega*dt));

		// Translation part
		float dist = Vector3.Distance(position, endPosition);
		Translate(forward * direction * Mathf.Min(dist, maxSpeed * dt));
	}
}

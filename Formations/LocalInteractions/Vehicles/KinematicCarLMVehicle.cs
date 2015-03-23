using UnityEngine;
using System.Collections;
using MechanicsUtility;
using Geometry;

public class KinematicCarLMVehicle : LocalMoveVehicle {

	// Max wheel angle
	public static float phi;

	// Max speed
	public static float speed;

	// Car length
	public static float L;

	// Moving threshold
	public static float stopThreshold;

	// Direction of the car
	public Vector3 direction { get { return gobj.transform.forward; } }


	// Constructor
	public KinematicCarLMVehicle(GameObject gobj) : base(gobj) {
	}

	// Very complex move
	protected override void VehicleMove(float dt) {
		if (resultMove.magnitude < stopThreshold) {
			return;
		}

		// Some parameters used
		float r = L / Mathf.Tan(phi.ToRad());
		float omega = Mechanics.Omega(speed, L, phi);
		float angToEnd = Tangents.RotationAngle(direction, resultMove);
		float forward = Mathf.Abs(angToEnd) > 90 ? -1 : 1;	// Which way is forward
		float turn = Mathf.Sign(angToEnd*forward);			// Which way to turn
		Vector3 endPosition = position + resultMove;

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
		Translate(forward * direction * Mathf.Min(dist, speed * dt));
	}
}

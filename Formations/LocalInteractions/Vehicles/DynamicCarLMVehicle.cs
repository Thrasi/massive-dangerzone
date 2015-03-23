using UnityEngine;
using System.Collections;
using Geometry;
using MechanicsUtility;

public class DynamicCarLMVehicle : LocalMoveVehicle {

	// Wheel angle
	public static float phi;

	// Max speed, not really used, but can be if needed
	public static float maxSpeed;

	// Accleration of the car
	public static float acceleration;

	// Car length
	public static float L;

	// Stop threshold
	public static float stopThreshold;

	// Current speed
	public float speed = 0;

	// Current direction
	public Vector3 direction { get { return gobj.transform.forward; } }


	// Constructor
	public DynamicCarLMVehicle(GameObject gobj) : base(gobj) {
	}

	// Slightly less complex move :)
	protected override void VehicleMove(float dt) {
		if (resultMove.magnitude < stopThreshold) {
			speed -= Mathf.Sign(speed) * acceleration * dt;
			Translate(direction * speed * dt);
			return;
		}

		// Some parameters used
		Vector3 resMove = resultMove / 2;
		float angToEnd = Tangents.RotationAngle(direction, resMove);
		Vector3 endPosition = position + resMove;

		float forward = Mathf.Abs(angToEnd) > 90 ? -1 : 1;	// Go forward or back
		float turn = Mathf.Sign(angToEnd*forward);			// Which way to turn
		
		// Speed recalculation
		if ((forward > 0) == (speed > 0)) {
			float stopDistance = Mechanics.StoppingDistance(speed, acceleration);
			float realDistance = Vector3.Distance(position, endPosition);
			if (realDistance < 0.5f && Mathf.Abs(speed) < 0.2f) {
				speed -= Mathf.Sign(speed)
					* Mathf.Min(acceleration*dt, Mathf.Abs(speed));
			} else if (realDistance > stopDistance) {
				speed += Mathf.Sign(speed) * acceleration * dt;
			} else {
				speed -= Mathf.Sign(speed) * acceleration * dt;
			}
		} else {
			speed += forward * acceleration * dt;
		}
		
		// Rotation part
		float omega = Mechanics.Omega(speed, L, phi);
		Rotate(turn * Mathf.Min(Mathf.Abs(angToEnd), omega*dt));

		// Translation part
		Translate(direction * speed * dt);
	}

}//End class

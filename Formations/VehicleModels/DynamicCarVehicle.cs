using UnityEngine;
using System.Collections.Generic;
using Geometry;
using MechanicsUtility;

public class DynamicCarVehicle : DirectedVehicle<DynamicCarVehicle> {

	// Max wheel angle
	public static float maxPhi;

	// Max speed
	public static float maxSpeed;

	// Max acceleration
	public static float maxAcceleration;

	// Length ov the car
	public static float L;

	// Current speed of the vehicle
	private float speed = 0;


	// Constructor
	public DynamicCarVehicle(GameObject gobj, DynamicCarVehicle parent,
		Vector3 relativePosition) : base(parent, gobj, relativePosition) {
	}

	// Move dynamic car
	// I know it looks very complicated, but I got tired of this and didnt want
	// to make a better solution, this works fine for now
	public override void Move(float dt) {
		if (parent == null) {
			return;
		}

		// Some parameters used
		float r = L / Mathf.Tan(maxPhi.ToRad());
		float angToEnd = angleToEnd;


		float forward = Mathf.Abs(angToEnd) > 90 ? -1 : 1;	// Go forward or back
		float turn = Mathf.Sign(angToEnd*forward);			// Which way to turn
		Circle c = new Circle(r, position, direction, turn < 0);
		if (c.IsInside(endPosition) && Vector3.Distance(position, endPosition) > 0.5f) {
			// Car will get stuck in endless loop
			// Now it tries to reach closest point to end position
			Vector3 onCirclePoint = c.ClosestOn(endPosition);
			float ang = Vector3.Angle(onCirclePoint - c.center, position - c.center);

			// Define 2 angles to do, one is when going forward
			// the other when going back
			float forwardAngle = forward > 0 ? ang : 360 - ang;
			float backAngle = 360 - forwardAngle;

			// Real distances for both angles
			float forwardDistance = c.r * forwardAngle.ToRad();
			float backDistance = c.r * backAngle.ToRad();
			float stopDistance = Mechanics.StoppingDistance(speed, maxAcceleration);
			
			// Check if its better to go forward or backward or break or what
			float realDistance, realAngle, acc = maxAcceleration * dt;
			if (speed > 0) {	// Driving forward
				acc = forwardDistance > stopDistance ? acc : -acc;
				realDistance = forwardDistance;
				realAngle = forwardAngle;
			} else {			// Driving back
				acc = backDistance > stopDistance ? -acc : acc;
				realDistance = backDistance;
				realAngle = backAngle;
			}

			if (realDistance < 0.5f && Mathf.Abs(speed) < 0.2f) {
				speed -= Mathf.Sign(speed)
					* Mathf.Min(maxAcceleration*dt, Mathf.Abs(speed));
			} else {
				speed += acc;
			}

			// Rotation part
			float omega = Mechanics.Omega(speed, L, maxPhi);
			Rotate(turn * Mathf.Min(realAngle, omega*dt));

			// Or maybe with Mathf.Min, I dont know
			Translate(direction * speed * dt);
		} else {
			// Speed recalculation
			if ((forward > 0) == (speed > 0)) {
				float stopDistance = Mechanics.StoppingDistance(speed, maxAcceleration);
				float realDistance = Vector3.Distance(position, endPosition);
				if (realDistance < 0.5f && Mathf.Abs(speed) < 0.2f) {
					speed -= Mathf.Sign(speed)
						* Mathf.Min(maxAcceleration*dt, Mathf.Abs(speed));
				} else if (realDistance > stopDistance) {
					speed += Mathf.Sign(speed) * maxAcceleration * dt;
				} else {
					speed -= Mathf.Sign(speed) * maxAcceleration * dt;
				}
			} else {
				speed += forward * maxAcceleration * dt;
			}
			
			// Rotation part
			float omega = Mechanics.Omega(speed, L, maxPhi);
			Rotate(turn * Mathf.Min(Mathf.Abs(angToEnd), omega*dt));

			// Translation part
			Translate(direction * speed * dt);
		}
	}

	/*// Move dynamic car
	public override void Move(float dt) {
		if (parent == null) {
			return;
		}

		// Some parameters used
		float r = L / Mathf.Tan(maxPhi.ToRad());
		float angToEnd = angleToEnd;
		float forward = Mathf.Abs(angToEnd) > 90 ? -1 : 1;	// Which way is forward
		float turn = Mathf.Sign(angToEnd*forward);			// Which way to turn
		float dist = Vector3.Distance(position, endPosition);

		Circle c = new Circle(r, position, direction, turn < 0);
		float realDistance = dist;// < 0.5f ? 0 : dist;		// Truncate
		// this line above will work fine without truncation, but when
		// it goes backwards, something fucks up
		if (c.IsInside(endPosition)) {
			// Car will get stuck in endless loop
			// Now it tries to reach closest point to end position
			Vector3 onCirclePoint = c.ClosestOn(endPosition);
			float ang = Vector3.Angle(onCirclePoint - c.center, position - c.center);
			ang = ang < 2.3f ? 0 : ang;		// Truncate to avoid oscillations

			// Update real distance
			float distToOnCirclePoint = Vector3.Distance(onCirclePoint, position);
			realDistance = c.r * ang.ToRad();
			//realDistance = distToOnCirclePoint < 0.51f ? 0 : realDistance;
			//print(ang);
		}

		// Speed recalculation
		float stopDist = Mechanics.StoppingDistance(speed, maxAcceleration);
		speed += stopDist > realDistance ? -maxAcceleration*dt : maxAcceleration*dt;
		//speed = Mathf.Abs(speed) < 0.1f && stopDist > realDistance + 0.5f ? 0 : speed;
		float omega = (speed / L * Mathf.Tan(maxPhi.ToRad())).ToDeg();

		// speed increases because transltion is wrong, fix it

		// Rotation part
		Rotate(turn * Mathf.Min(Mathf.Abs(angToEnd), Mathf.Abs(omega*dt)));

		// Translation part
		Translate(Mathf.Sign(speed) * direction * Mathf.Min(realDistance, Mathf.Abs(speed * dt)));
	}*/
}

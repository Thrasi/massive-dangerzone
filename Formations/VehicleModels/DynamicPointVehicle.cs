using UnityEngine;
using System.Collections.Generic;
using MechanicsUtility;

public class DynamicPointVehicle : Vehicle<DynamicPointVehicle> {

	// Free parameter, maximum acceleration, write-only
	public static float maxAcceleration { private get; set; }

	// Current velocity
	public Vector3 velocity;
	
	// End position computed from parent and relative position to it
	public override Vector3 endPosition {
		get {
			if (parent == null) {
				return position;
			}
			return parent.position + relativePosition;
		}
	}

	// Constructor
	public DynamicPointVehicle(GameObject vehicle, DynamicPointVehicle parent,
		Vector3 relativePosition) : base(parent, vehicle, relativePosition) {
	}

	// Moves point dynamicly, parent doesnt move
	public override void Move(float dt) {
		if (parent == null) {
			return;
		}
		Vector3 goalPos = parent.position + relativePosition;
		Vector3 currPos = position;
		Vector3 desired = (goalPos - currPos) * 5;

		// Pick one of these 2 steerings
		Vector3 steer = desired - velocity;
		//steer = Mechanics.Steer(
		//	velocity, position, endPosition, maxAcceleration, dt)*1.5f;
		steer = steer.normalized * Mathf.Min(steer.magnitude, maxAcceleration*dt);

		velocity += steer;
		Translate(velocity * dt);
	}
}

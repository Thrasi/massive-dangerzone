using UnityEngine;
using System.Collections;
using MechanicsUtility;

public class DynamicPointLMVehicle : LocalMoveVehicle {

	// Acceleration of vehicle
	public static float acceleration;

	// Max speed
	public static float maxSpeed;

	// Current velocity
	public Vector3 velocity;


	// Constructor
	public DynamicPointLMVehicle(GameObject gobj) : base(gobj) {
		this.velocity = Vector3.zero;
	}

	// Move dynamic point
	protected override void VehicleMove(float dt) {
		velocity +=
			Mechanics.Steer(velocity, position, position+resultMove, acceleration, dt);
		velocity = Mathf.Min(maxSpeed, velocity.magnitude) * velocity.normalized;
		Translate(velocity*dt);
	}
}

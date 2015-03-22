using UnityEngine;
using System.Collections.Generic;
using MechanicsUtility;
using System.Linq;

public class DynamicPointLocalInteraction : LocalInteraction<DynamicPointLMVehicle> {

	// Acceleration
	public float acceleration = 1;

	// Speed of the point
	public float maxSpeed = 5;


	// Move the leader
	protected override void MoveLeader(float dt) {
		Vector3 move = LocalInteractionUtilities.DecidePointMove().normalized;
		leader.velocity += move * acceleration * dt;
		leader.velocity = Mathf.Min(maxSpeed, leader.velocity.magnitude)
			* leader.velocity.normalized;
		leader.Translate(leader.velocity * dt);
	}

	// Update speed and acceleration
	protected override void UpdateParameters() {
		DynamicPointLMVehicle.maxSpeed = maxSpeed;
		DynamicPointLMVehicle.acceleration = acceleration;
	}

	// Loads point vehicle
	protected override void LoadGameObject() {
		vehicleObject = Resources.Load("GameObjects/SphericalVehicle") as GameObject;
		vehicleObject.transform.localScale = Vector3.one;
	}

	// Just instantiates new vehicle
	protected override DynamicPointLMVehicle CreateVehicle(GameObject veh) {
		return new DynamicPointLMVehicle(veh);
	}
}

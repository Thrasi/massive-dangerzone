using UnityEngine;
using System.Collections.Generic;
using MechanicsUtility;

public class DynamicPointLeaderFollower : PointLeaderFollower<DynamicPointVehicle> {

	// Maximum acceleration
	public float maxAcceleration = 1;

	// Maximum speed
	public float maxSpeed = 20;

	
	// Move dynamic point leader
	protected override void MoveLeader(float dt) {
		Vector3 move = DecideMove();
		if (move == Vector3.zero) {		// Full brake
			leader.velocity += Mechanics.Brake(leader.velocity, maxAcceleration, dt);	
		} else {						// Moving in direction
			leader.velocity += DecideMove() * maxAcceleration * dt;
		}

		// Limit to max speed
		leader.velocity = leader.velocity.normalized
			* Mathf.Min(leader.velocity.magnitude, maxSpeed);
		
		leader.Translate(leader.velocity * dt);
	}

	// Updates maximum acceleration
	protected override void UpdateParameters() {
		DynamicPointVehicle.maxAcceleration = maxAcceleration;
	}

	// Helper function to add vehicles to formation, leader has to be added manually
	protected override void AddVehicle(int parent, Vector3 relativePosition) {
		GameObject newVeh = Instantiate(vehicles[parent].position + relativePosition);
		newVeh.transform.parent = transform;
		vehicles.Add(
			new DynamicPointVehicle(
				newVeh,
				vehicles[parent],
				relativePosition
			)
		);
	}

	// Initializes leader
	protected override void InitLeader() {
		vehicles.Add(
			new DynamicPointVehicle(
				Instantiate(Vector3.zero),
				null,
				Vector3.zero
			)
		);
		followCamera.transform.parent = vehicles[0].gobj.transform;
		vehicles[0].gobj.transform.parent = transform;
	}

}


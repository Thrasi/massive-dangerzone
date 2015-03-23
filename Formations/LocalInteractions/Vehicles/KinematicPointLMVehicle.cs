using UnityEngine;
using System.Collections.Generic;

public class KinematicPointLMVehicle : LocalMoveVehicle {

	// Speed of the vehicle
	public static float speed = 1;

	// Constructor
	public KinematicPointLMVehicle(GameObject gobj) : base(gobj) {
	}

	// Move kinematic point
	protected override void VehicleMove(float dt) {
		if (resultMove.magnitude < speed*dt) {		// Resulting move is small enough
			Translate(resultMove);
		} else {									// Choose smaller move
			Translate(resultMove.normalized*dt * Mathf.Min(speed, resultMove.magnitude));
		}
	}
}

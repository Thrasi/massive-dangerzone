using UnityEngine;
using System.Collections.Generic;

public class KinematicPointLeaderFollower : PointLeaderFollower<KinematicPointVehicle> {

	// Max speed
	public float maxSpeed = 5;


	// Moves leader kinematicly
	protected override void MoveLeader(float dt) {
		vehicles[0].Translate(DecideMove() * maxSpeed * dt);
	}

	// Updates maximum speed
	protected override void UpdateParameters() {
		KinematicPointVehicle.maxSpeed = maxSpeed;
	}

	// Helper function to add vehicles to formation, leader has to be added manually
	protected override void AddVehicle(int parent, Vector3 relativePosition) {
		GameObject newVeh = Instantiate(vehicles[parent].position + relativePosition);
		newVeh.transform.parent = transform;
		vehicles.Add(
			new KinematicPointVehicle(
				newVeh,
				vehicles[parent],
				relativePosition
			)
		);
	}

	// Initializes leader
	protected override void InitLeader() {
		vehicles.Add(
			new KinematicPointVehicle(
				Instantiate(Vector3.zero),
				null,
				Vector3.zero
			)
		);
		followCamera.transform.parent = vehicles[0].gobj.transform;
		vehicles[0].gobj.transform.parent = transform;
	}

}

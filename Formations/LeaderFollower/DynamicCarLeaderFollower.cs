using UnityEngine;
using System.Collections.Generic;
using Geometry;

public class DynamicCarLeaderFollower : CarLeaderFollower<DynamicCarVehicle> {

	// Max speed of the vehicle
	public float maxSpeed = 10;

	// Max acceleration
	public float maxAcceleration = 5;

	// Current speed of the leader
	private float speed = 0;


	// Moving car leader
	protected override void MoveLeader(float dt) {
		bool w = Input.GetKey("w");
		bool a = Input.GetKey("a");
		bool s = Input.GetKey("s");
		bool d = Input.GetKey("d");
		float omega = (speed / L * Mathf.Tan(maxPhi.ToRad())).ToDeg();
		float turnAngle = 0;

		// Decide what to do
		if (w) { speed = Mathf.Min(maxSpeed, speed+maxAcceleration*dt); }
		if (s) { speed = Mathf.Max(-maxSpeed, speed-maxAcceleration*dt); }
		if (a) { turnAngle = -omega*dt;	}
		if (d) { turnAngle = omega*dt; }
		if (!(w || s)) {	// Max brake
			speed -= Mathf.Sign(speed) * Mathf.Min(Mathf.Abs(speed), maxAcceleration*dt);
		}

		// And then do it
		leader.Rotate(turnAngle);
		leader.Translate(leader.direction.normalized * speed * dt);
	}

	// Updating cars parameters
	protected override void UpdateParameters() {
		DynamicCarVehicle.L = L;
		DynamicCarVehicle.maxSpeed = maxSpeed;
		DynamicCarVehicle.maxPhi = maxPhi;
		DynamicCarVehicle.maxAcceleration = maxAcceleration;
	}

	// Creates leader and sets camera
	protected override void InitLeader() {
		vehicles.Add(
			new DynamicCarVehicle(
				Instantiate(vehicle, Vector3.zero, Quaternion.Euler(0,0,0)) as GameObject,
				null,
				Vector3.zero
			)
		);
		followCamera.transform.parent = leader.gobj.transform;
		leader.gobj.transform.parent = transform;
	}

	// Helper function for adding vehicles
	protected override void AddVehicle(int parent, Vector3 relativePosition) {
		GameObject gobj = Instantiate(
			vehicle,
			vehicles[parent].position + relativePosition,
			Quaternion.Euler(0,0,0)
		) as GameObject;
		
		vehicles.Add(new DynamicCarVehicle(gobj, vehicles[parent], relativePosition));
		gobj.transform.parent = transform;
	}
}

using UnityEngine;
using System.Collections.Generic;
using Geometry;

public class KinematicCarLeaderFollower : CarLeaderFollower<KinematicCarVehicle> {

	// Max speed of the vehicle
	public float maxSpeed;

	// Moving car leader
	protected override void MoveLeader(float dt) {
		bool a = Input.GetKey("a");
		bool w = Input.GetKey("w");
		bool s = Input.GetKey("s");
		bool d = Input.GetKey("d");
		float omega = (maxSpeed / L * Mathf.Tan(maxPhi.ToRad())).ToDeg();

		if (a && w) {
			leader.Rotate(-omega * dt);
			leader.Translate(leader.direction.normalized * maxSpeed * dt);
		} else if (d && w) {
			leader.Rotate(omega * dt);
			leader.Translate(leader.direction.normalized * maxSpeed * dt);
		} else if (a && s) {
			leader.Rotate(omega * dt);
			leader.Translate(-leader.direction.normalized * maxSpeed * dt);
		} else if (s && d) {
			leader.Rotate(-omega * dt);
			leader.Translate(-leader.direction.normalized * maxSpeed * dt);
		} else if (w) {
			leader.Translate(leader.direction.normalized * maxSpeed * dt);
		} else if (s) {
			leader.Translate(-leader.direction.normalized * maxSpeed * dt);
		}
	}

	// Updating cars parameters
	protected override void UpdateParameters() {
		KinematicCarVehicle.L = L;
		KinematicCarVehicle.maxSpeed = maxSpeed;
		KinematicCarVehicle.maxPhi = maxPhi;
	}

	// Creates leader and sets camera
	protected override void InitLeader() {
		vehicles.Add(
			new KinematicCarVehicle(
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
		
		vehicles.Add(new KinematicCarVehicle(gobj, vehicles[parent], relativePosition));
		gobj.transform.parent = transform;
	}

}

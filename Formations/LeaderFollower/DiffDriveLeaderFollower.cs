using UnityEngine;
using System.Collections.Generic;

public class DiffDriveLeaderFollower : LeaderFollower<DiffDriveVehicle> {

	// Max speed
	public float maxSpeed = 10;

	// Max angular velocity
	public float maxOmega = 40;

	// Object for vehicle
	protected GameObject vehicle;

	// Use this for initialization
	void Start () {
		vehicle = Resources.Load("Tank/Prefabs/Tank_pref") as GameObject;
		vehicle.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

		InitFormation();
	}
	
	// Updates speed and angular velocity
	protected override void UpdateParameters() {
		DiffDriveVehicle.maxSpeed = maxSpeed;
		DiffDriveVehicle.maxOmega = maxOmega;
	}

	// Function for moving the leader
	protected override void MoveLeader(float dt) {
		if (Input.GetKey("a")) {
			leader.Rotate(-maxOmega * dt);
		}
		if (Input.GetKey("s")) {
			leader.Translate(-leader.direction.normalized * maxSpeed * dt);
		}
		if (Input.GetKey("d")) {
			leader.Rotate(maxOmega * dt);
		}
		if (Input.GetKey("w")) {
			leader.Translate(leader.direction.normalized * maxSpeed * dt);
		}
	}

	// Some formation
	protected virtual void InitFormation() {
		// Create leader
		vehicles.Add(
			new DiffDriveVehicle(
				Instantiate(vehicle, Vector3.zero, Quaternion.Euler(0,-90,0)) as GameObject,
				null,
				Vector3.zero
			)
		);
		followCamera.transform.parent = leader.gobj.transform;
		leader.gobj.transform.parent = transform;

		// Add followers
		AddVehicle(0, Vector3.left * 10);
		AddVehicle(0, Vector3.right * 10);
		AddVehicle(0, Vector3.back * 10);
		AddVehicle(3, Vector3.left * 10);
		AddVehicle(3, Vector3.right * 10);
	}

	// Helper function for adding vehicles
	protected void AddVehicle(int parent, Vector3 relativePosition) {
		GameObject gobj = Instantiate(
			vehicle,
			vehicles[parent].position + relativePosition,
			Quaternion.Euler(0,-90,0)
		) as GameObject;
		
		vehicles.Add(new DiffDriveVehicle(gobj,	vehicles[parent], relativePosition));
		gobj.transform.parent = transform;
	}

}

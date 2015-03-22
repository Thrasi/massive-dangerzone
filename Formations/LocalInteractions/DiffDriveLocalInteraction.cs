using UnityEngine;
using System.Collections;

public class DiffDriveLocalInteraction : LocalInteraction<DiffDriveLMVehicle> {

	// Max speed ot vehicle
	public float speed = 5;

	// Max angular velocity
	public float omega = 40;

	// Threshold to stop moving
	public float stopThreshold = 0.1f;


	// Used to change camera position
	new void Start() {
		base.Start();
		Vector3 camPos = followCamera.transform.position;
		camPos.y += 10;
		camPos.z -= 15;
		followCamera.transform.position = camPos;
	}

	// Move the leader
	protected override void MoveLeader(float dt) {
		if (Input.GetKey("a")) {
			leader.Rotate(-omega * dt);
		}
		if (Input.GetKey("s")) {
			leader.Translate(-leader.direction.normalized * speed * dt);
		}
		if (Input.GetKey("d")) {
			leader.Rotate(omega * dt);
		}
		if (Input.GetKey("w")) {
			leader.Translate(leader.direction.normalized * speed * dt);
		}
	}

	// Update speed and angular velocity
	protected override void UpdateParameters() {
		DiffDriveLMVehicle.speed = speed;
		DiffDriveLMVehicle.omega = omega;
		DiffDriveLMVehicle.stopThreshold = stopThreshold;
	}

	// Loads tank
	protected override void LoadGameObject() {
		vehicleObject = Resources.Load("Tank/Prefabs/Tank_pref") as GameObject;
		vehicleObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
	}

	// Just instantiates new vehicle
	protected override DiffDriveLMVehicle CreateVehicle(GameObject veh) {
		return new DiffDriveLMVehicle(veh);
	}
}

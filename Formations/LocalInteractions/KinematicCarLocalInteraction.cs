using UnityEngine;
using System.Collections;
using MechanicsUtility;

public class KinematicCarLocalInteraction : LocalInteraction<KinematicCarLMVehicle> {

	// Max speed
	public float speed = 5;

	// Max wheel angle
	public float phi = 40;

	// Car length
	public float L = 5;

	// Not to move threshold
	public float stopThreshold = 0.1f;


	// Used to change camera position
	new void Start() {
		base.Start();
		Vector3 camPos = followCamera.transform.position;
		camPos.y += 10;
		camPos.z -= 15;
		followCamera.transform.position = camPos;
	}

	// Moves kinematic car leader
	protected override void MoveLeader(float dt) {
		bool a = Input.GetKey("a");
		bool w = Input.GetKey("w");
		bool s = Input.GetKey("s");
		bool d = Input.GetKey("d");
		float omega = Mechanics.Omega(speed, L, phi);

		if (a && w) {
			leader.Rotate(-omega * dt);
			leader.Translate(leader.direction.normalized * speed * dt);
		} else if (d && w) {
			leader.Rotate(omega * dt);
			leader.Translate(leader.direction.normalized * speed * dt);
		} else if (a && s) {
			leader.Rotate(omega * dt);
			leader.Translate(-leader.direction.normalized * speed * dt);
		} else if (s && d) {
			leader.Rotate(-omega * dt);
			leader.Translate(-leader.direction.normalized * speed * dt);
		} else if (w) {
			leader.Translate(leader.direction.normalized * speed * dt);
		} else if (s) {
			leader.Translate(-leader.direction.normalized * speed * dt);
		}
	}

	// Updates necessary parameters
	protected override void UpdateParameters() {
		KinematicCarLMVehicle.speed = speed;
		KinematicCarLMVehicle.phi = phi;
		KinematicCarLMVehicle.L = L;
		KinematicCarLMVehicle.stopThreshold = stopThreshold;
	}

	// Loads nice yellow car
	protected override void LoadGameObject() {
		vehicleObject = Resources.Load("Low_Poly_Sport_Car_ME_R4/CC_ME_R4") as GameObject;
		vehicleObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
	}

	// Just constructor call
	protected override KinematicCarLMVehicle CreateVehicle(GameObject gobj) {
		return new KinematicCarLMVehicle(gobj);
	}
}

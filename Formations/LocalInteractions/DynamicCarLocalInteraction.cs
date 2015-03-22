using UnityEngine;
using System.Collections;
using MechanicsUtility;
using Geometry;

public class DynamicCarLocalInteraction : LocalInteraction<DynamicCarLMVehicle> {

	// Car acceleration
	public float acceleration = 5;

	// Wheel angle
	public float phi = 40;

	// Car length
	public float L = 5;

	// Max speed
	public float maxSpeed = 5;

	// Threshold to stop moving
	public float stopThreshold = 0.35f;


	// Used to change camera position
	new void Start() {
		base.Start();
		Vector3 camPos = followCamera.transform.position;
		camPos.y += 10;
		camPos.z -= 15;
		followCamera.transform.position = camPos;
	}

	// Move dynamic car
	protected override void MoveLeader(float dt) {
		bool w = Input.GetKey("w");
		bool a = Input.GetKey("a");
		bool s = Input.GetKey("s");
		bool d = Input.GetKey("d");
		
		float turnAngle = 0;

		// Decide what to do
		if (w) { leader.speed = Mathf.Min(maxSpeed, leader.speed+acceleration*dt); }
		if (s) { leader.speed = Mathf.Max(-maxSpeed, leader.speed-acceleration*dt); }

		float omega = (leader.speed / L * Mathf.Tan(phi.ToRad())).ToDeg();

		if (a) { turnAngle = -omega*dt;	}
		if (d) { turnAngle = omega*dt; }

		// And then do it
		leader.Rotate(turnAngle);
		leader.Translate(leader.direction.normalized * leader.speed * dt);
	}

	// Updates a lot
	protected override void UpdateParameters() {
		DynamicCarLMVehicle.acceleration = acceleration;
		DynamicCarLMVehicle.phi = phi;
		DynamicCarLMVehicle.L = L;
		DynamicCarLMVehicle.maxSpeed = maxSpeed;
		DynamicCarLMVehicle.stopThreshold = stopThreshold;
	}

	// Load the car and make it smaller
	protected override void LoadGameObject() {
		vehicleObject = Resources.Load("Low_Poly_Sport_Car_ME_R4/CC_ME_R4") as GameObject;
		vehicleObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
	}

	// Create car
	protected override DynamicCarLMVehicle CreateVehicle(GameObject gobj) {
		return new DynamicCarLMVehicle(gobj);
	}
}

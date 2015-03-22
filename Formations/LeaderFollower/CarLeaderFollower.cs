using UnityEngine;
using System.Collections.Generic;

public abstract class CarLeaderFollower<T> : LeaderFollower<T> where T : Vehicle<T> {

	// Car length
	public float L = 5;

	// Max wheel angle
	public float maxPhi = 40;

	// Gameobject for car
	protected GameObject vehicle;

	// Load gameobject
	void Start() {
		vehicle = Resources.Load("Low_Poly_Sport_Car_ME_R4/CC_ME_R4") as GameObject;
		InitFormation();
	}

	// Some formation
	protected virtual void InitFormation() {
		// Create leader
		InitLeader();

		// Adjust camera
		Vector3 pos = followCamera.transform.position;
		pos.y += 15;
		pos.z -= 30;
		followCamera.transform.position = pos;

		// Add followers
		AddVehicle(0, Vector3.left * 15);
		AddVehicle(0, Vector3.right * 15);
		AddVehicle(0, Vector3.back * 15);
		AddVehicle(3, backLeft30 * 15);
		AddVehicle(3, backRight30 * 15);
	}

	// Add vehicle to formation
	protected abstract void AddVehicle(int parent, Vector3 relativePosition);

	// Initializes leader
	protected abstract void InitLeader();
}

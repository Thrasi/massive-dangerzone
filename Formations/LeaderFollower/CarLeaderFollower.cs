using UnityEngine;
using System.Collections;

public abstract class CarLeaderFollower<T> : LeaderFollower<T> where T : Vehicle<T> {

	public float L;
	public float maxPhi;

	private GameObject vehicle;

	void Start() {
//		vehicles = Resources.Load("");
	}

	// Some formation
	protected virtual void InitFormation() {
		// Create leader
		InitLeader();

		// Add followers
		AddVehicle(0, Vector3.left * 10);
		AddVehicle(0, Vector3.right * 10);
		AddVehicle(0, Vector3.back * 10);
		AddVehicle(3, backLeft30 * 10);
		AddVehicle(3, backRight30 * 10);
	}

	protected abstract void AddVehicle(int parent, Vector3 relativePosition);
	protected abstract void InitLeader();
}

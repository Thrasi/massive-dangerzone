using UnityEngine;
using System.Collections.Generic;

public abstract class PointLeaderFollower<T> : LeaderFollower<T> where T : Vehicle<T> {

	// Formation to use
	public int formation = 1;

	// GameObject used for all vehicles
	private GameObject vehicle;
	

	// Initialize gameobject and formation
	void Start () {
		vehicle = Resources.Load("GameObjects/SphericalVehicle") as GameObject;
		vehicle.transform.localScale = new Vector3(1, 1, 1);

		InitFormation();
	}

	// Formation initializer
	protected virtual void InitFormation() {
		if (formation == 1) {
			InitFormation1();	
		} else if (formation == 2) {
			InitFormation2();
		} else if (formation == 3) {
			InitFormation3();
		}
	}

	// This will create leader
	protected abstract void InitLeader();

	// This is used for adding vehicles
	protected abstract void AddVehicle(int parent, Vector3 relativePosition);

	// Just a helper function to simplify Instantiate calls
	protected GameObject Instantiate(Vector3 position) {
		return Instantiate(vehicle, position, zeroDir) as GameObject;
	}

	// Formation 1	
	private void InitFormation1() {
		// Leader
		InitLeader();
		
		// Followers
		AddVehicle(0, backRight30*10);
		AddVehicle(0, backLeft30*10);
		AddVehicle(1, backLeft30*10);
		AddVehicle(1, backRight30*10);
		AddVehicle(2, backLeft30*10);
		AddVehicle(2, backRight30*10);
	}
	
	// Formation 2
	private void InitFormation2() {
		InitLeader();
		AddVehicle(0, Vector3.right*10);
		AddVehicle(0, Vector3.left*10);
		AddVehicle(0, Vector3.back*10);
		AddVehicle(3, Vector3.right*10);
		AddVehicle(3, Vector3.left*10);
		AddVehicle(3, Vector3.back*10);
		AddVehicle(1, Vector3.right*10);
		AddVehicle(2, Vector3.left*10);
		AddVehicle(6, Vector3.left*10);
		AddVehicle(6, Vector3.right*10);
	}

	// Formation 3
	private void InitFormation3() {
		InitLeader();
		AddVehicle(0, Vector3.left*10);
		AddVehicle(0, Vector3.right*10);
		AddVehicle(0, backLeft30*10);
		AddVehicle(0, backRight30*10);
		AddVehicle(0, frontLeft30*10);
		AddVehicle(0, frontRight30*10);
	}


	// Decides which direction should a formation move based on key input
	// Returns normalized vector for direction
	protected Vector3 DecideMove() {
		bool w = Input.GetKey("w");
		bool a = Input.GetKey("a");
		bool s = Input.GetKey("s");
		bool d = Input.GetKey("d");
		
		if (w && s || a && d) { return Vector3.zero; }
		if (a && w) { return forwardLeft;  }
		if (a && s) { return backLeft;     }
		if (s && d) { return backRight;    }
		if (d && w) { return forwardRight; }
		if (a) { return Vector3.left;    }
		if (s) { return Vector3.back;    }
		if (d) { return Vector3.right;   }
		if (w) { return Vector3.forward; }

		return Vector3.zero;
	}

}

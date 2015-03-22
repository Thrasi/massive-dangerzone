using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class LocalInteraction<T> : MonoBehaviour where T : LocalMoveVehicle {

	// Main camera from scene
	public GameObject followCamera;

	// Use only 7 vehicles
	public bool use7 = false;

	// Distance to keep between vehicles
	public float distance = 10;

	// Neighborhood to use
	public int neighborhood = 3;

	// These variables are for gizmos
	public bool showMoves = false;
	public bool showResultingMove = false;
	public bool showNeighborhood = false;


	// Parent object for moving camera
	private GameObject cameraMover;

	// List of vehicles in formations
	protected List<T> vehicles = new List<T>();

	// GameObject for vehicle
	protected GameObject vehicleObject;

	// Vehicle that can be moves from outside
	private int leaderIdx;
	protected T leader { get { return vehicles[leaderIdx]; } }


	// Load shit
	protected void Start() {
		LoadGameObject();
		InitFormation();
		
		cameraMover = new GameObject();
		cameraMover.name = "CameraMover";
		cameraMover.transform.position = Vector3.zero;
		followCamera.transform.parent = cameraMover.transform;
	}

	// Loads specific gameobject
	protected abstract void LoadGameObject();

	// Initializes some formation, can be also overriden if needed
	protected virtual void InitFormation() {
		AddVehicle(new Vector3(5, 0, 5), Quaternion.identity);
		AddVehicle(new Vector3(-5, 0, 5), Quaternion.identity);
		AddVehicle(new Vector3(5, 0, -5), Quaternion.identity);
		AddVehicle(new Vector3(-5, 0, -5), Quaternion.identity);
		AddVehicle(new Vector3(0, 0, 0), Quaternion.identity);
		AddVehicle(new Vector3(12, 0, 3), Quaternion.identity);
		AddVehicle(new Vector3(4, 0, -7), Quaternion.identity);
		leaderIdx = 0;
		
		if (!use7) {
			AddVehicle(new Vector3(-8, 0, 8), Quaternion.identity);
			AddVehicle(new Vector3(-9, 0, -9), Quaternion.identity);
			AddVehicle(new Vector3(0, 0, -9), Quaternion.identity);
			AddVehicle(new Vector3(9, 0, -0), Quaternion.identity);
			AddVehicle(new Vector3(-3, 0, 0), Quaternion.identity);
			AddVehicle(new Vector3(0, 0, -3), Quaternion.identity);
			AddVehicle(new Vector3(3, 0, 0), Quaternion.identity);
			AddVehicle(new Vector3(3, 0, 9), Quaternion.identity);
			AddVehicle(new Vector3(-4, 0, 12), Quaternion.identity);
			AddVehicle(new Vector3(5, 0, 10), Quaternion.identity);
			AddVehicle(new Vector3(20, 0, 3), Quaternion.identity);
			AddVehicle(new Vector3(22, 0, 1), Quaternion.identity);
			AddVehicle(new Vector3(15, 0, 6), Quaternion.identity);
			AddVehicle(new Vector3(-23, 0, 9), Quaternion.identity);
			AddVehicle(new Vector3(-17, 0, 8), Quaternion.identity);
			AddVehicle(new Vector3(3, 0, 20), Quaternion.identity);
			AddVehicle(new Vector3(1, 0, 22), Quaternion.identity);
			AddVehicle(new Vector3(6, 0, 15), Quaternion.identity);
			AddVehicle(new Vector3(9, 0, -23), Quaternion.identity);
			AddVehicle(new Vector3(8, 0, -17), Quaternion.identity);
		}
	}

	// Helper function for adding vehicles
	protected void AddVehicle(Vector3 position, Quaternion orientation) {
		GameObject newVehicle = Instantiate(position, orientation);
		vehicles.Add(CreateVehicle(newVehicle));
		newVehicle.transform.parent = transform;
	}

	// This is supposed to call spefici constructor of type T with parameter veh
	protected abstract T CreateVehicle(GameObject veh);

	// Helper function for instantiating vehicle game objects	
	private GameObject Instantiate(Vector3 position, Quaternion orientation) {
		return Instantiate(vehicleObject, position, orientation) as GameObject;
	}


	// Very general description how every local interaction formation should work
	void Update () {
		float dt = Time.deltaTime;
		if (Input.GetKeyDown("space")) {
			ChangeLeader();
		}
		UpdateParameters();
		if (neighborhood > 0) {			// To avoid sending 0
			LocalMoveVehicle.N = neighborhood;	
		}

		// If it is being controlled, leader is moved only here
		if (IsLeaderControlled()) {
			MoveLeader(dt);
		}

		// Move all vehicles and update camera position
		Vector3 cameraPosition = Vector3.zero;
		foreach (T veh in vehicles) {
			IEnumerable<LocalMoveVehicle> lmVehicles =
				from v in vehicles select v as LocalMoveVehicle;
			
			if (veh == leader && IsLeaderControlled()) {
				// Dont move leader if it is being controlled
				veh.Move(lmVehicles, distance, dt, false);
			} else {
				veh.Move(lmVehicles, distance, dt, true);
			}

			cameraPosition += veh.position;
		}

		cameraPosition /= vehicles.Count;
		cameraMover.transform.position = cameraPosition;
	}

	// Changes leader, makes the leader next vehicle in the list
	private void ChangeLeader() {
		leaderIdx = (leaderIdx + 1) % vehicles.Count;
	}

	// Checks if leader is currently being controlled by some input
	private bool IsLeaderControlled() {
		return Input.GetKey("a") || Input.GetKey("s")
			|| Input.GetKey("d") || Input.GetKey("w");
	}

	// Method to move only one leading vehicle
	protected abstract void MoveLeader(float dt);

	// Method to update parameters for vehicle such as speed and max acceleration
	// This way, all attributes of vehicles can be changed while running the game
	protected abstract void UpdateParameters();


	// Drawing some things for easier debugging later
	void OnDrawGizmos() {
		foreach (T veh in vehicles) {
			// Show all move vectors that vehicle has
			if (showMoves) {
				Gizmos.color = Color.red;
				foreach (Vector3 m in veh.moves) {
					Gizmos.DrawLine(veh.position, veh.position + m);
				}
			}

			// Show resulting move of the vehicle
			if (showResultingMove) {
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(veh.position, veh.position + veh.resultMove);
			}

			// Show connections of vehicle to its neighbors
			if (showNeighborhood) {
				Gizmos.color = Color.magenta;
				foreach (Vector3 v in from v3 in veh.closest3 select v3._1) {
					Gizmos.DrawLine(veh.position, v);
				}
			}
		}
		
		// Draw sphere above the leader
		if (vehicles.Count > 0) {
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(leader.position + new Vector3(0, 1.5f, 0), 0.3f);
		}
	}

}//End class

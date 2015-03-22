using UnityEngine;
using System.Collections.Generic;
using Geometry;

/**
	Class for all leader follower formations.
**/
public abstract class LeaderFollower<T> : MonoBehaviour where T : Vehicle<T> {

	// Some directional vectors
	protected static readonly Vector3 forwardLeft = new Vector3(-1, 0, 1).normalized;
	protected static readonly Vector3 forwardRight = new Vector3(1, 0, 1).normalized;
	protected static readonly Vector3 backLeft = new Vector3(-1, 0, -1).normalized;
	protected static readonly Vector3 backRight = new Vector3(1, 0, -1).normalized;
	protected static readonly Vector3 backLeft30 = new Vector3(-1.73f, 0, -3).normalized;
	protected static readonly Vector3 backRight30 = new Vector3(1.73f, 0, -3).normalized;
	protected static readonly Vector3 frontLeft30 = new Vector3(-1.73f, 0, 3).normalized;
	protected static readonly Vector3 frontRight30 = new Vector3(1.73f, 0, 3).normalized;
	protected static readonly Vector3 frontLeft75 = new Vector3(-3.73f, 0, 1).normalized;
	protected static readonly Vector3 frontRight75 = new Vector3(3.73f, 0, 1).normalized;

	// Zero direction
	protected static readonly Quaternion zeroDir = Quaternion.Euler(Vector3.zero);
	

	// Camera that will follow the leader
	public GameObject followCamera;

	// List of vehicles in formations
	protected List<T> vehicles = new List<T>();

	// Returns the main leader of the formation
	protected T leader { get { return vehicles[0]; } }
	
	
	// Very general description how every leader follower formation should work
	void Update () {
		float dt = Time.deltaTime;
		UpdateParameters();
		MoveLeader(dt);
		foreach (T v in vehicles) {
			v.Move(dt);
		}
	}

	// Method to move only the main leader
	protected abstract void MoveLeader(float dt);

	// Method to update parameters for vehicle such as speed and max acceleration
	// This way, all attributes of vehicles can be changed while running the game
	protected abstract void UpdateParameters();

	// Some drawings for formations
	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		foreach (T v in vehicles) {
			// Draw positions
			foreach (Edge e in GeometryUtils.Hexagon(v.endPosition, 0.7f)) {
				e.GizmosDraw(Color.blue);
			}
		
			// Draw links to parents
			T p = v.parent;
			if (p != null) {
				Gizmos.color = Color.magenta;
				Gizmos.DrawLine(v.position, p.position);
			}
		}
	}

}

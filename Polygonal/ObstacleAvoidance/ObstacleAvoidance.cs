using UnityEngine;
using System.Collections;

public abstract class ObstacleAvoidance : AbstractPolygonalVehicles {

	// Distance condition to end
	private const float END_DIST = 0.1f;

	// Velocity condition to end
	private const float END_VEL = 0.5f;


	// Filename for map
	public string filename;

	// Vehicle radius
	public float R;

	// Maximum acceleration
	public float maxAcc = 1.0f;

	// To show visible vertices from each vehicle
	public bool showVisible = false;

	// To show line from vehicle to goal
	public bool showToGoal = false;


	// Number of vehicles
	protected int N;

	// Their goals
	protected Vector3[] goals;

	// If all vehicles are at their goals
	protected bool done = false;


	// For label
	private GUIStyle labelStyle;
	private Rect labelRect;
	protected float cost;
	protected float started;


	void Awake() {
		// Initialize label stuff
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		labelRect = new Rect(20, 20, 20, 20);
	}

	// Shows stopwatch and final time when its done
	void OnGUI() {
		GUI.Label(
			labelRect,
			"Time: " + Time.realtimeSinceStartup.ToString("0.00")
				+ "\nResult: " + cost.ToString("0.00"),
			labelStyle
		);
	}

	// Checks if all vehicles have reached the goal. Vehicles has reached the goal if
	// it's closer than END_DIST and its speed is less than END_VEL
	protected bool AllReached() {
		for (int i = 0; i < N; i++) {
			Vector3 pos = vehicles[i].transform.position;
			if (!(Vector3.Distance(pos, goals[i]) < END_DIST) || !(Speed(i) < END_VEL)) {
				return false;
			}
		}
		return true;
	}

	// Returns speed of ith vehicle
	protected abstract float Speed(int i);

}

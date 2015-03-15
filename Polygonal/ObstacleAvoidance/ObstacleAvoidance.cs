using UnityEngine;
using System.Collections;

public class ObstacleAvoidance : AbstractPolygonalVehicles {


	// Filename for map
	public string filename;

	// Vehicle radius
	public float R;

	// Maximum acceleration
	public float maxAcc = 1.0f;

	// How far should I check for vehicles
	public float distanceNeighborhood = 40.0f;

	// Number of vehicles to take into account
	public int neighborhoodSize = 12;

	// Time horizon for checing collision, how far in time to see if the collide
	public float timeHorizon = 10.0f;

	// Same as time horizon but used for obstacles
	public float timeHorizonObst = 5.0f;

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
	protected string strCost;
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

}

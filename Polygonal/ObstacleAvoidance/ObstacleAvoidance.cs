using UnityEngine;
using System.Collections.Generic;
using System.Linq;




// you have TODOs in this file
// there is case when obstacles will fail, check it out and try to fix it



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

	
	// Distance condition to end
	private const float END_DIST = 0.1f;

	// Velocity condition to end
	private const float END_VEL = 0.5f;

	// Slowing distance cant be less than this
	private const float SLOWING_MIN = 3;


	// Number of vehicles
	private int N;

	// Their goals
	private Vector3[] goals;

	// Their velocities (not used at the moment)
	private Vector3[] velocities;

	// Preferred velocities (used only for drawing and debugging)
	private Vector3[] prefs;

	// If all vehicles are at their goals
	private bool done = false;

	// Simulator
	private DynamicRVO sim;


	// For label
	private GUIStyle labelStyle;
	private Rect labelRect;
	private string strCost;
	private float cost;
	private float started;


	// Use this for initialization
	void Start () {
		// Load vehicles and set size
		material = Resources.Load("Materials/ObstacleMaterial") as Material;
		vehicle = Resources.Load("GameObjects/SphericalVehicle") as GameObject;
		vehicle.transform.localScale = new Vector3(2*R, 2*R, 2*R);
		
		// Read map and set some variables
		PolygonMap map = new PolygonMap("Assets/_Data/ColAvoidPolyg/" + filename);
		N = map.N;
		goals = map.goals;
		velocities = Enumerable.Repeat(Vector3.zero, N).ToArray();
		prefs = Enumerable.Repeat(Vector3.zero, N).ToArray();
		obstacles = map.GetObstacles();
		GenerateObstacles(obstacles);	
		GenerateVehicles(map.GetVehiclePositions());

		// Initialize RVO
		sim = DynamicRVO.Instance;
		sim.setAgentDefaults(
			distanceNeighborhood,
			neighborhoodSize,
			timeHorizon,
			timeHorizonObst,
			R,
			Vector3.zero
		);
		for (int i = 0; i < N; i++) {
			sim.addAgent(vehicles[i].transform.position);
		}
		sim.setMaxAcceleration(maxAcc);
		foreach (Polygon pol in obstacles) {
			sim.addObstacle(pol);
		}
		sim.processObstacles();
		
		// Initialize label printing
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		labelRect = new Rect(20, 20, 20, 20);
		started = Time.realtimeSinceStartup;
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

	// Only call to update function, easier to change it if some better update
	// is discovered
	void Update () {
		RVODynamicUpdate(Time.deltaTime);
	}

	// Makes update for dynamic point RVO
	private void RVODynamicUpdate(float dt) {
		// All vehicles are at their goals
		if (done) {
			return;
		}

		// Set time step
		sim.setTimeStep(dt);
		
		// Set preferred velocities
		for (int i = 0; i < N; i++) {
			// Checking for edges of the ball, not the center
			Vector3 currPos = sim.getAgentPosition(i);
			Vector3 goalVec = (goals[i] - currPos).normalized;
			Vector3 pointA = currPos + Quaternion.Euler(0, 90, 0) * goalVec * R;
			Vector3 pointB = currPos + Quaternion.Euler(0, -90, 0) * goalVec * R;
			Edge toGoalA = new Edge(pointA, goals[i]);
			Edge toGoalB = new Edge(pointB, goals[i]);

			// Distance to closest intersection with obstacles
			float distClosest = Mathf.Min(
				DistToClosest(toGoalA, currPos),
				DistToClosest(toGoalB, currPos)
			);
			Tuple<Edge, float> wall = ClosestWall(currPos);
			Vector3 prefVel;
			if (distClosest == float.MaxValue		// No intersections on path
				|| wall._1 == null					// No obstacles
				|| wall._2 > 3*R 					// Obstacle too far
				|| distClosest > 4*R) {				// Intersection too far
				//TODO tweaks shits above

				// In this case use preferred velocity towards the goal
				prefVel = PrefVelocity(
					sim.getAgentVelocity(i), currPos, goals[i], maxAcc, dt);
			} else {
				Vector3 vert = wall._1.Vertical(currPos);
				prefVel = Quaternion.Euler(0, -90, 0) * vert*10;	//TODO tweak this shit
			}
			
			// Set preferred
			sim.setAgentPrefVelocity(i, prefVel);
			prefs[i] = prefVel;
		}

		// Do a simulation step
		sim.doStep();

		// Move vehicle and set velocities
		for (int i = 0; i < N; i++) {
			velocities[i] = sim.getAgentVelocity(i);
			vehicles[i].transform.position = sim.getAgentPosition(i);
		}

		// Check if all vehicles have reached their goals
		done = AllReached();
		if (done) {
			cost = Time.realtimeSinceStartup - started;
		}
	}

	// Checks if all vehicles have reached the goal. Vehicles has reached the goal if
	// it's closer than END_DIST and its speed is less than END_VEL
	private bool AllReached() {
		for (int i = 0; i < N; i++) {
			Vector3 v3pos = vehicles[i].transform.position;
			if (!(Vector3.Distance(v3pos, goals[i]) < END_DIST)
				|| !(velocities[i].magnitude < END_VEL)) {
				
				return false;
			}
		}
		return true;
	}

	// Taking current position and velocity and goal position, calculates
	// steering (acceleraton) needed to reach goal. Returned vector is normalized
	private static Vector3 Steer(Vector3 position, Vector3 velocity, Vector3 goal) {
		Vector3 desired = goal - position;
		return (desired - velocity).normalized;
	}

	// Acceleration in time step used for maximum braking
	private static Vector3 Brake(Vector3 velocity, float maxAcc, float dt) {
		Vector3 direction = -velocity.normalized;
		return direction * Mathf.Min(velocity.magnitude, maxAcc*dt);
	}

	// Stopping distance
	private static float StoppingDistance(float speed, float maxAcc) {
		return 0.5f * speed * speed / maxAcc;
	}

	// Calculates how much it needs to steer to reach the goal without considering
	// any obstacles and other vehicles
	private static Vector3 Steer(Vector3 oldVelocity,
		Vector3 pos, Vector3 goal, float maxAcc, float dt) {
		
		float dist = Vector3.Distance(pos, goal);
		float speed = oldVelocity.magnitude;
		float slowing = Mathf.Max(StoppingDistance(speed, maxAcc), SLOWING_MIN);
		Vector3 desired =  (goal - pos).normalized * (speed + maxAcc*dt);
		if (dist < slowing) {
			desired /= slowing;
		}
		Vector3 steer = desired - oldVelocity;
		return Mathf.Min(maxAcc*dt, steer.magnitude) * steer.normalized;
	}

	// Although this function is very similar to Steer, I still prefer having both
	// because they have small differences and this one might need tweaks in future
	// This one returns preferred velocity of an agent, that includes stopping in
	// the end with precision
	private static Vector3 PrefVelocity(Vector3 oldVelocity,
		Vector3 pos, Vector3 goal, float maxAcc, float dt) {
		
		float dist = Vector3.Distance(pos, goal);
		if (dist < SLOWING_MIN) {
			return (goal - pos) / 2;
		}
		float speed = oldVelocity.magnitude;
		float slowing = Mathf.Max(StoppingDistance(speed, maxAcc), SLOWING_MIN);
		Vector3 desired =  (goal - pos).normalized * (speed + maxAcc*dt);
		if (dist < slowing) {
			desired /= slowing;
		}
		return desired;
	}

	// Draw goals of vehicles in their respective colors
	void OnDrawGizmos() {
		// Goals
		for (int i = 0; i < N; i++) {
			Gizmos.color = vehicles[i].renderer.material.color;
			Gizmos.DrawSphere(goals[i], R/2);
		}

		// Preferred velocities
		// Enable this to debug with preferred velocities
		/*if (prefs != null) {
			Gizmos.color = Color.red;
			for (int i = 0; i < N; i++) {
				Vector3 currPos = vehicles[i].transform.position;
				Gizmos.DrawLine(currPos, currPos + prefs[i]);
			}
		}*/
	}

}//End class

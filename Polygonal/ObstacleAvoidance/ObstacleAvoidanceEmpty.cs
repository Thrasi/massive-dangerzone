using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RVO;

public class ObstacleAvoidanceEmpty : AbstractVehicles {

	// Filename for map
	public string filename;

	// Vehicle radius
	public float R;

	// Maximum acceleration
	public float maxAcc = 1.0f;

	
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
		vehicle = Resources.Load("GameObjects/SphericalVehicle") as GameObject;
		vehicle.transform.localScale = new Vector3(R, R, R);
		
		// Read map and set some variables
		PolygonMap map = new PolygonMap("Assets/_Data/ColAvoidPolyg/" + filename);
		N = map.N;
		goals = map.goals;
		velocities = Enumerable.Repeat(Vector3.zero, N).ToArray();		
		GenerateVehicles(map.GetVehiclePositions());

		// Initialize RVO
		sim = DynamicRVO.Instance;
		sim.setAgentDefaults(40, N, 10.0f, 5.0f, R/2, Vector3.zero);
		for (int i = 0; i < N; i++) {
			sim.addAgent(vehicles[i].transform.position);
		}
		sim.setMaxAcceleration(maxAcc);

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

	// Update is called once per frame
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
		
		// Set preffered velocities
		for (int i = 0; i < N; i++) {
			Vector3 prefVel = PrefVelocity(
				sim.getAgentVelocity(i), sim.getAgentPosition(i), goals[i], maxAcc, dt);
			sim.setAgentPrefVelocity(i, prefVel);
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
	// because they have small differences
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

	void OnDrawGizmos() {
		for (int i = 0; i < N; i++) {
			Gizmos.color = vehicles[i].renderer.material.color;
			Gizmos.DrawSphere(goals[i], R/4);
		}
	}

	// Finds positions of K closest vehicles to vehicle with index i
	private IEnumerable<KeyValuePair<Vector3, float>> KNearest(int i, int K) {
		FFFBHeap<Vector3> closestVehicles = new FFFBHeap<Vector3>(K);
		Vector3 currPos = vehicles[i].transform.position;
		for (int j = 0; j < N; j++) {
			if (i == j) {
				continue;
			}
			Vector3 other = vehicles[j].transform.position;
			float d = Vector3.Distance(currPos, other);
			closestVehicles.Insert(d, other);
		}
		return closestVehicles;
	}

	// Other method
	private void ForcesUpdate(int neighborhood, float dt) {
		for (int i = 0; i < N; i++) {
			if (!activeV[i]) {
				continue;
			}
			Vector3 currPos = vehicles[i].transform.position;
			Vector3 goalPos = goals[i];
			float dist = Vector3.Distance(currPos, goalPos);
			if (dist < 0.1f && velocities[i].magnitude < 0.5f) {
				activeV[i] = false;
				continue;
			}
			
			Vector3 steer = Steer(velocities[i], currPos, goalPos, maxAcc, Time.deltaTime);
			Vector3 mov = steer.normalized;
			float v = velocities[i].magnitude;
			
			foreach (KeyValuePair<Vector3, float> kv in KNearest(i, neighborhood)) {
				float d = kv.Value;
				Vector3 other = kv.Key;
				if (d < 5*R) {
					mov += 7*(currPos - other).normalized / (d);
				}	
			}
			
			mov = mov.normalized * maxAcc;
			velocities[i] += mov * dt;
			vehicles[i].transform.Translate(velocities[i] * dt, Space.World);
		}
	}

}

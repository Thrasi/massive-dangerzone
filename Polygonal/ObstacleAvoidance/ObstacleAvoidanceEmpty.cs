using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RVO;

public class ObstacleAvoidanceEmpty : AbstractVehicles {

	public string filename;

	public float R;

	public float maxAcc = 1.0f;

	private int N;
	private Vector3[] goals;
	private Vector3[] velocities;
	private bool[] activeV;

	private float[,] slopes;

	private const float SLOWING_MIN = 3;

	// For label
	protected float cost;
	protected float startup;
	private GUIStyle labelStyle;
	private Rect labelRect;
	private string strCost;


	// Use this for initialization
	void Start () {
		vehicle = Resources.Load("GameObjects/SphericalVehicle") as GameObject;
		vehicle.transform.localScale = new Vector3(R, R, R);
		
		PolygonMap map = new PolygonMap("Assets/_Data/ColAvoidPolyg/" + filename);
		N = map.N;
		goals = map.goals;
		velocities = Enumerable.Repeat(Vector3.zero, N).ToArray();
		activeV = Enumerable.Repeat(true, N).ToArray();
		slopes = new float[N,N];
		
		GenerateVehicles(map.GetVehiclePositions());
		ScenarioTest();
		// Initialize label printing
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		labelRect = new Rect(20, 20, 20, 20);
	}

	
	// Update is called once per frame
	void Update () {
		//ForcesUpdate(3, Time.deltaTime);
		//print(velocities[0].magnitude);
		RVOKinematicUpdate(Time.deltaTime);
	}

	void OnGUI() {
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < N; i++) {
			sb.Append("Vehicle ").Append(i).Append(": ")
				.Append(velocities[i].magnitude.ToString("0.00")).Append('\n');
		}
		sb.Append("Time: ").Append(Time.realtimeSinceStartup.ToString("0.00"));
		GUI.Label(labelRect, sb.ToString(), labelStyle);
	}


	Simulator sim;
	// velocity obstacle
	void ScenarioTest() {
		sim = Simulator.Instance;
		//sim.setTimeStep(0.025f);
		sim.setAgentDefaults(20*R, N, 10.0f, 5.0f, R, 10000, new RVO.Vector2(0.0f,0.0f));
		for (int i = 0; i < N; i++) {
			Vector3 pos = vehicles[i].transform.position;
			sim.addAgent(new RVO.Vector2(pos.x, pos.z));
		}

	}

	bool done = false;
	private void RVOKinematicUpdate(float dt) {

		if (done) {
			return;
		}

		sim.setTimeStep(dt);
		
		Vector3[] prevPos = new Vector3[N];
		for (int i = 0; i < N; i++) {
			prevPos[i] = ToVec3(sim.getAgentPosition(i));
			/*if (Vector3.Distance(prevPos[i], goals[i]) < 0.1f) {
				sim.setAgentPrefVelocity(i, new RVO.Vector2(0, 0));
			} else {
				Vector3 pref = goals[i] - prevPos[i];
				sim.setAgentPrefVelocity(i, ToVec2(pref));
			}*/

			//if (Vector3.Distance(prevPos[i], goals[i]) < 3 ) {
			//	sim.setAgentPrefVelocity(i, ToVec2((goals[i] - prevPos[i]) / 2));
			//} else {
				sim.setAgentPrefVelocity(i, ToVec2(PrefVelocity(
					velocities[i], prevPos[i], goals[i], maxAcc, dt)));
			//}
		}

		sim.doStep();

		Vector3[] currPos = new Vector3[N];
		for (int i = 0; i < N; i++) {
			currPos[i] = ToVec3(sim.getAgentPosition(i));
			
			Vector3 desiredVelocity = ToVec3(sim.getAgentVelocity(i));
			Vector3 currentVelocity = velocities[i];
			Vector3 steer = desiredVelocity - currentVelocity;
			if (Random.value < 0.1f && Vector3.Distance(currPos[i], goals[i]) > 10) {
			//	steer = Random.onUnitSphere;
			//	steer.y = 0;
			}
			velocities[i] += steer.normalized * Mathf.Min(maxAcc*dt, steer.magnitude);

			vehicles[i].transform.Translate(velocities[i]*dt, Space.World);
			sim.agents_[i].position_ = ToVec2(vehicles[i].transform.position);
		}
		
		done = AllReached();
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

	private bool AllReached() {
		for (int i = 0; i < N; i++) {
			Vector3 v3pos = vehicles[i].transform.position;
			if (!(Vector3.Distance(v3pos, goals[i]) < 0.1f) || !(velocities[i].magnitude < 0.5f)) {
				return false;
			}
		}
		return true;
	}

	private static Vector3 ToVec3(RVO.Vector2 v) {
		return new Vector3(v.x(), 0, v.y());
	}

	private static RVO.Vector2 ToVec2(Vector3 v) {
		return new RVO.Vector2(v.x, v.z);
	}


	// Taking current position and velocity and goal position, calculates
	// steering (acceleraton) needed to reach goal. Returned vector is normalized
	private static Vector3 Steer(Vector3 position, Vector3 velocity, Vector3 goal) {
		Vector3 desired = goal - position;
		return (desired - velocity).normalized;
	}

	private static Vector3 Brake(Vector3 velocity, float maxAcc, float dt) {
		Vector3 direction = -velocity.normalized;
		return direction * Mathf.Min(velocity.magnitude, maxAcc*dt);
	}

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

	private static Vector3 PrefVelocity(Vector3 oldVelocity,
		Vector3 pos, Vector3 goal, float maxAcc, float dt) {
		
		float dist = Vector3.Distance(pos, goal);
		if (dist < 3) {
			return (goal - pos) / 2;
		}
		float speed = oldVelocity.magnitude;
		float slowing = Mathf.Max(StoppingDistance(speed, maxAcc), 3);
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

	private void RVODynamicUpdate(float dt) {
		if (done) {
			return;
		}
		sim.setTimeStep(dt);
		Vector3[] prevPos = new Vector3[N];
		for (int i = 0; i < N; i++) {
			RVO.Vector2 agentPos = sim.getAgentPosition(i);
			Vector3 v3pos = ToVec3(agentPos);
			prevPos[i] = v3pos;
			if (Vector3.Distance(v3pos, goals[i]) < 0.1f) {
				sim.setAgentPrefVelocity(i, new RVO.Vector2(0, 0));
			} else {
				Vector3 pref = goals[i] - v3pos;
				sim.setAgentPrefVelocity(i, ToVec2(pref));
			}
		}

		sim.doStep();
		Vector3[] currPos = new Vector3[N];
		for (int i = 0; i < N; i++) {
			RVO.Vector2 agentPos = sim.getAgentPosition(i);
			currPos[i] = ToVec3(agentPos);
			Vector3 desired = currPos[i] - prevPos[i];
			Vector3 current = velocities[i];
			Vector3 steer = desired - current;
			velocities[i] += steer.normalized * Mathf.Min(maxAcc*dt, steer.magnitude);
			vehicles[i].transform.Translate(velocities[i]*dt, Space.World);
		}
		
		bool reached = true;
		for (int i = 0; i < N; i++) {
			Vector3 v3pos = vehicles[i].transform.position;
			if (!(Vector3.Distance(v3pos, goals[i]) < 0.1f)) {
				reached = false;
				break;
			}
		}
		done = reached;
	}


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

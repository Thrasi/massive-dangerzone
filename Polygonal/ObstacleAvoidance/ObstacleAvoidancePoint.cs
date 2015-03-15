using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ObstaclesExt;
using MechanicsUtility;


/**
	Collision avoidance with and without obstacles.
	Some parameters are tweakable in RVODynamicUpdate functions if faster times
	are needed or if something starts to suck.
**/
public class ObstacleAvoidancePoint : ObstacleAvoidance {

	// Should I turn left
	public bool turnLeft = true;

	// To show preferred velocities
	public bool showPrefs = false;


	// Their velocities (not used at the moment)
	private Vector3[] velocities;

	// Preferred velocities (used only for drawing and debugging)
	private Vector3[] prefs;

	// Angle to turn, it should be either -90 left or 90 right
	private float turn;

	// Simulator
	private DynamicRVO sim;


	// Distance condition to end
	private const float END_DIST = 0.1f;

	// Velocity condition to end
	private const float END_VEL = 0.5f;



	// Use this for initialization
	void Start () {
		// Load vehicles and set size
		material = Resources.Load("Materials/ObstacleMaterial") as Material;
		vehicle = Resources.Load("GameObjects/SphericalVehicle") as GameObject;
		vehicle.transform.localScale = new Vector3(2*R, 2*R, 2*R);
		
		// Read map and set some variables
		PolygonMap map = new PolygonMap("Assets/_Data/ColAvoidPolyg/" + filename);
		N = map.N;
		obstacles = map.GetObstacles();
		GenerateObstacles(obstacles);	
		GenerateVehicles(map.GetVehiclePositions());
		goals = map.goals;
		turn = turnLeft ? -90 : 90;
		velocities = Enumerable.Repeat(Vector3.zero, N).ToArray();
		prefs = Enumerable.Repeat(Vector3.zero, N).ToArray();
		
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
		
		// Set startup time
		started = Time.realtimeSinceStartup;
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

			// By checking both points, it makes it go faster around the
			// spikes (vertices) of the polygonal obstacles
			// Although if the radius is very small, this isnt necessary,
			// but I will leave it because time performance isnt an issue right now
			Tuple<Edge, float> intersA = obstacles.ClosestIntersection(pointA, goals[i]);
			Tuple<Edge, float> intersB = obstacles.ClosestIntersection(pointB, goals[i]);
			Tuple<Edge, float> wall = obstacles.ClosestWall(currPos);

			// Here are conditions to make it prettier
			Polygon intPolA = obstacles.FindPolygonNull(intersA._1);
			Polygon intPolB = obstacles.FindPolygonNull(intersB._1);
			Polygon wallPol = obstacles.FindPolygonNull(wall._1);
			bool samePolA = intPolA != null && intPolA.Equals(wallPol);
			bool samePolB = intPolB != null && intPolB.Equals(wallPol);
			bool samePol = samePolA || samePolB;

			// Some conditions are tweakable if needed
			Vector3 prefVel;
			if ((intersA._1 == null && intersB._1 == null)	// No intersections on path
				|| wall._1 == null							// No obstacles
				|| wall._2 > 3*R 							// Obstacle too far
				|| (intersA._2 > 4*R && intersB._2 > 4*R 	// Intersection too far
					&& !samePol)	// closest wall and intersection not in same polygon
				) {	

				// In this case use preferred velocity towards the goal
				prefVel = Mechanics.PrefVelocity(
					sim.getAgentVelocity(i), currPos, goals[i], maxAcc, dt);
			} else {
				Vector3 vert = wall._1.Vertical(currPos);
				Vector3 leftDirection = Quaternion.Euler(0, turn, 0) * vert;
				Vector3 vdir = wall._1.v - currPos;
				Vector3 wdir = wall._1.w - currPos;
				float vangle = Vector3.Angle(leftDirection, vdir);
				float wangle = Vector3.Angle(leftDirection, wdir);
				// NOTE change this if velocity near the wall is too big
				if (vangle < wangle) {
					prefVel = leftDirection * Vector3.Distance(wall._1.v, currPos) * 0.7f;
				} else {
					prefVel = leftDirection * Vector3.Distance(wall._1.w, currPos) * 0.7f;
				}
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

	// Connects given vertex to all vertices in given list
	private static List<Edge> Connect(List<Vector3> vertices, Vector3 pos) {
		List<Edge> connections = new List<Edge>();
		foreach (Vector3 v in vertices) {
	 		connections.Add(new Edge(v, pos));
		}
		return connections;
	}

	// Draw goals of vehicles in their respective colors
	void OnDrawGizmos() {
		// Goals
		for (int i = 0; i < N; i++) {
			Gizmos.color = vehicles[i].renderer.material.color;
			Gizmos.DrawSphere(goals[i], R/2);
		}
		
		if (showVisible) {
			List<Edge>[] vis = new List<Edge>[N];
			for (int i = 0; i < N; i++) {
				List<Vector3> vert = obstacles.VisibleVertices(sim.getAgentPosition(i));
				vis[i] = Connect(vert, sim.getAgentPosition(i));
			}
			for (int i = 0; i < N; i++) {
				Gizmos.color = vehicles[i].renderer.material.color;
				foreach (Edge e in vis[i]) {
					e.GizmosDraw(Gizmos.color);
				}
			}
		}

		if (showToGoal) {
			for (int i = 0; i < N; i++) {
				Gizmos.color = vehicles[i].renderer.material.color;
				Gizmos.DrawLine(vehicles[i].transform.position, goals[i]);
			}
		}
		
		// Preferred velocities
		// Enable this to debug with preferred velocities
		if (showPrefs) {
			for (int i = 0; i < N; i++) {
				Gizmos.color = vehicles[i].renderer.material.color;
				Vector3 currPos = vehicles[i].transform.position;
				Gizmos.DrawLine(currPos, currPos + prefs[i]);
			}
		}
	}

}//End class

using UnityEngine;
using System.Collections.Generic;
using ObstaclesExt;
using MechanicsUtility;
using System.Linq;
using Geometry;
using ObstaclesExt;

public class ObstacleAvoidanceCar : ObstacleAvoidance {

	// Their velocities (not used at the moment)
	private float[] speeds;

	public float maxPhi = 40;

	// This is the radius that car makes
	private float r;

	private float stopThreshold = 1;


	// Use this for initialization
	void Start () {
		material = Resources.Load("Materials/ObstacleMaterial") as Material;
		vehicle = Resources.Load("GameObjects/Vehicle") as GameObject;
		vehicle.transform.localScale = new Vector3(2, 1, 2*R);

		// Read map and set some variables
		PolygonMap map = new PolygonMap("Assets/_Data/ColAvoidPolyg/" + filename);
		N = map.N;
		goals = map.goals;
		obstacles = map.GetObstacles();
		GenerateObstacles(obstacles);	
		GenerateVehicles(map.GetVehiclePositions());
		speeds = Enumerable.Repeat(0.0f, N).ToArray();

		r = Mechanics.Radius(R, maxPhi);

		// Initial direction towards their goals
		for (int i = 0; i < N; i++) {
			vehicles[i].transform.rotation =
				Quaternion.LookRotation(goals[i] - vehicles[i].transform.position);
		}
		
		// Startup time
		started = Time.realtimeSinceStartup;
	}

	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;

		for (int i = 0; i < N; i++) {
			Vector3 position = vehicles[i].transform.position;
			Vector3 resultMove = goals[i] - position;
			float acceleration = maxAcc;
			Vector3 direction = vehicles[i].transform.forward;


			Tuple<Edge, float> inters = obstacles.ClosestIntersection(position, goals[i]);
			if (inters._1 != null && inters._2 < r) {
				Vector3 v1 = inters._1.v - inters._1.w;
				Vector3 v2 = inters._1.w - inters._1.v;
				float ang1 = Tangents.RotationAngle(direction, v1);
				float ang2 = Tangents.RotationAngle(direction, v2);
				float ang = Mathf.Abs(ang1) > Mathf.Abs(ang2) ? ang2 : ang1;
				float omega = Mechanics.Omega(speeds[i], R, maxPhi);
				vehicles[i].transform.Rotate(0, Mathf.Sign(ang) * Mathf.Min(Mathf.Abs(ang), omega*dt),0
					, Space.World);
				vehicles[i].transform.Translate(direction*speeds[i]*dt, Space.World);
				continue;
			}
			

			// Some parameters used
			Vector3 resMove = resultMove / 2;
			float angToEnd = Tangents.RotationAngle(direction, resMove);
			Vector3 endPosition = position + resMove;

			float forward = Mathf.Abs(angToEnd) > 90 ? -1 : 1;	// Go forward or back
			float turn = Mathf.Sign(angToEnd*forward);			// Which way to turn

			Circle c = new Circle(r, position, direction, turn < 0);
			if (c.IsInside(endPosition) && Vector3.Distance(position, endPosition) > 0.5f) {
				// Car will get stuck in endless loop
				// Now it tries to reach closest point to end position
				Vector3 onCirclePoint = c.ClosestOn(endPosition);
				float ang = Vector3.Angle(onCirclePoint - c.center, position - c.center);

				// Define 2 angles to do, one is when going forward
				// the other when going back
				float forwardAngle = forward > 0 ? ang : 360 - ang;
				float backAngle = 360 - forwardAngle;

				// Real distances for both angles
				float forwardDistance = c.r * forwardAngle.ToRad();
				float backDistance = c.r * backAngle.ToRad();
				float stopDistance = Mechanics.StoppingDistance(speeds[i], maxAcc);
				
				// Check if its better to go forward or backward or break or what
				float realDistance, realAngle, acc = maxAcc * dt;
				if (speeds[i] > 0) {	// Driving forward
					acc = forwardDistance > stopDistance ? acc : -acc;
					realDistance = forwardDistance;
					realAngle = forwardAngle;
				} else {			// Driving back
					acc = backDistance > stopDistance ? -acc : acc;
					realDistance = backDistance;
					realAngle = backAngle;
				}

				if (realDistance < 0.5f && Mathf.Abs(speeds[i]) < 0.2f) {
					speeds[i] -= Mathf.Sign(speeds[i])
						* Mathf.Min(maxAcc*dt, Mathf.Abs(speeds[i]));
				} else {
					speeds[i] += acc;
				}

				// Rotation part
				float omega = Mechanics.Omega(speeds[i], R, maxPhi);
				vehicles[i].transform.Rotate(0, turn * Mathf.Min(realAngle, omega*dt), 0,
					Space.World);

				// Or maybe with Mathf.Min, I dont know
				vehicles[i].transform.Translate(direction * speeds[i] * dt, Space.World);
			} else {
				
				// Speed recalculation
				if ((forward > 0) == (speeds[i] > 0)) {
					float stopDistance = Mechanics.StoppingDistance(speeds[i], acceleration);
					float realDistance = Vector3.Distance(position, endPosition);
					if (realDistance < 0.5f && Mathf.Abs(speeds[i]) < 0.2f) {
						speeds[i] -= Mathf.Sign(speeds[i])
							* Mathf.Min(acceleration*dt, Mathf.Abs(speeds[i]));
					} else if (realDistance > stopDistance) {
						speeds[i] += Mathf.Sign(speeds[i]) * acceleration * dt;
					} else {
						speeds[i] -= Mathf.Sign(speeds[i]) * acceleration * dt;
					}
				} else {
					speeds[i] += forward * acceleration * dt;
				}
				
				// Rotation part
				float omega = Mechanics.Omega(speeds[i], R, maxPhi);
				vehicles[i].transform.Rotate(
					0, turn * Mathf.Min(Mathf.Abs(angToEnd), omega*dt), 0, Space.World);

				// Translation part
				vehicles[i].transform.Translate(direction * speeds[i] * dt, Space.World);
			}
		}



		// Check if all vehicles have reached their goals
		done = AllReached();
		if (done) {
			cost = Time.realtimeSinceStartup - started;
		}


			
	}

	protected override float Speed(int i) {
		return speeds[i];
	}

	private void Rotate(int i, float angle) {
		vehicles[i].transform.Rotate(0, angle, 0, Space.World);
	}

	private void Translate(int i, Vector3 move) {
		vehicles[i].transform.Translate(move, Space.World);
	}

	void OnDrawGizmos() {
		if (goals != null) {
			Gizmos.color = Color.red;
			foreach (var v in goals) {
				Gizmos.DrawSphere(v, 2);
			}
		}
	}
	
}

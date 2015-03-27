using UnityEngine;
using System.Collections.Generic;
using ObstaclesExt;
using MechanicsUtility;
using System.Linq;
using Geometry;

public class ObstacleAvoidanceCar : ObstacleAvoidance {

	// Max wheel angle
	public float maxPhi = 40;

	// How far in time to check collisions
	public float timeHorizon = 10;

	// How many frames to move straight after collision avoidance
	public int fixedMoveLength = 120;

	// Turn left on collision avoidance
	public bool turnLeft = true;

	// To show directions of the car
	public bool showDirections = false;


	// Their velocities (not used at the moment)
	private float[] speeds;

	// Car length
	private float L;

	// This is the radius that car makes
	private float r;

	// Turn multiplier
	private float turnAvoid = -1;

	// How many frames to continue avoiding car
	private int[] leftToAvoid;

	// How many frames to continue going straight
	private int[] leftToFixed;
	

	// Use this for initialization
	void Start () {
		material = Resources.Load("Materials/ObstacleMaterial") as Material;
		vehicle = Resources.Load("GameObjects/Vehicle") as GameObject;
		L = 2*R;
		vehicle.transform.localScale = new Vector3(2, 1, L);

		// Read map and set some variables
		PolygonMap map = new PolygonMap("Assets/_Data/ColAvoidPolyg/" + filename);
		N = map.N;
		goals = map.goals;
		obstacles = map.GetObstacles();
		GenerateObstacles(obstacles);	
		GenerateVehicles(map.GetVehiclePositions());
		speeds = Enumerable.Repeat(0.0f, N).ToArray();
		leftToAvoid = Enumerable.Repeat(0, N).ToArray();
		leftToFixed = Enumerable.Repeat(0, N).ToArray();

		r = Mechanics.Radius(L, maxPhi);
		turnAvoid = turnLeft ? -1 : 1;

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
		if (done) {
			return;
		}

		float dt = Time.deltaTime;

		for (int i = 0; i < N; i++) {
			// Some info for the current car
			Vector3 position = vehicles[i].transform.position;
			Vector3 resultMove = goals[i] - position;
			Vector3 direction = vehicles[i].transform.forward;


			// Next block is collision avoidance with obstacles

			// Find the wall of the closest intersection
			Tuple<Edge, float> intersA = obstacles.ClosestIntersection(Front(i), goals[i]);
			Tuple<Edge, float> intersB = obstacles.ClosestIntersection(Back(i), goals[i]);
			if (   (intersA._1 != null && intersA._2 < 3*r)
				|| (intersB._1 != null && intersB._2 < 3*r)) {

				// Finding smaller angle for best turn
				Tuple<Edge, float> inters = intersA._1 != null ? intersA : intersB;
				Vector3 v = inters._1.v - inters._1.w;
				float ang1 = Tangents.RotationAngle(direction, v);
				float ang2 = Tangents.RotationAngle(direction, -v);
				float ang = Mathf.Abs(ang1) > Mathf.Abs(ang2) ? ang2 : ang1;

				// Move the car
				float omega = Mechanics.Omega(speeds[i], L, maxPhi);
				speeds[i] += maxAcc * dt;
				Rotate(i, Mathf.Sign(ang) * Mathf.Min(Mathf.Abs(ang), omega*dt));
				Translate(i, direction * speeds[i] * dt);
				continue;
			}


			// Perform some important movement
			if (leftToAvoid[i] > 0) {
				leftToAvoid[i]--;
				Rotate(i, turnAvoid * Mechanics.Omega(speeds[i], L, maxPhi) * dt);
				Translate(i, direction * speeds[i] * dt);
				continue;
			} else if (leftToFixed[i] > 0) {
				leftToFixed[i]--;
				Translate(i, direction * speeds[i] * dt);
				continue;
			}


			// This block is collision avoidance with other cars
			bool willCollide = false;
			for (int j = 0; j < N; j++) {
				if (i == j) {	// Self check
					continue;
				}

				// Find intersection point if both cars continue moving
				// straight with constant velocity
				Vector3 otherDir = Direction(j);
				Vector3 otherPos = Position(j);
				Edge e1 = new Edge(
					position,
					position + direction * speeds[i] * timeHorizon
				);
				Edge e2 = new Edge(
					otherPos,
					otherPos + otherDir * speeds[j] * timeHorizon
				);
				Vector3? intersPoint = e1.Intersection(e2);
				if (!intersPoint.HasValue) {
					continue;
				}

				// These are times required for front and back of the cars
				// to reach the collision point
				Vector3 intPoint = intersPoint.Value;
				float t11 = Vector3.Distance(Front(i), intPoint) / Mathf.Abs(speeds[i]);
				float t12 = Vector3.Distance(Back(i),  intPoint) / Mathf.Abs(speeds[i]);
				float t21 = Vector3.Distance(Front(j), intPoint) / Mathf.Abs(speeds[j]);
				float t22 = Vector3.Distance(Back(j),  intPoint) / Mathf.Abs(speeds[j]);

				// Check if the cars will collide in the future
				if ((t11 >= t21 && t11 <= t22) || (t12 >= t21 && t12 <= t22)) {
					float omega = Mechanics.Omega(speeds[i], L, maxPhi);
					float colAngle = Vector3.Angle(direction, otherDir);
					
					// Or maybe divide by 4??
					leftToAvoid[i] = ((int) (colAngle / (omega * dt))) / 2;
					leftToFixed[i] = fixedMoveLength;
					
					Rotate(i, turnAvoid * omega * dt);
					Translate(i, direction * speeds[i] * dt);
					willCollide = true;
					break;
				}
			}

			if (willCollide) {
				continue;
			}


			// The next part is the waypoint movement

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

				// Make the car brake
				if (realDistance < 0.5f && Mathf.Abs(speeds[i]) < 0.2f) {
					speeds[i] -= Mathf.Sign(speeds[i])
						* Mathf.Min(maxAcc*dt, Mathf.Abs(speeds[i]));
				} else {
					speeds[i] += acc;
				}

				// Rotation part
				float omega = Mechanics.Omega(speeds[i], L, maxPhi);
				Rotate(i, turn * Mathf.Min(realAngle, omega*dt));

				// Or maybe with Mathf.Min, I dont know
				Translate(i, direction * speeds[i] * dt);
			} else {
				
				// Speed recalculation
				if ((forward > 0) == (speeds[i] > 0)) {
					float stopDistance = Mechanics.StoppingDistance(speeds[i], maxAcc);
					float realDistance = Vector3.Distance(position, endPosition);
					if (realDistance < 0.5f && Mathf.Abs(speeds[i]) < 0.2f) {
						speeds[i] -= Mathf.Sign(speeds[i])
							* Mathf.Min(maxAcc*dt, Mathf.Abs(speeds[i]));
					} else if (realDistance > stopDistance) {
						speeds[i] += Mathf.Sign(speeds[i]) * maxAcc * dt;
					} else {
						speeds[i] -= Mathf.Sign(speeds[i]) * maxAcc * dt;
					}
				} else {
					speeds[i] += forward * maxAcc * dt;
				}
				
				// Rotation part
				float omega = Mechanics.Omega(speeds[i], L, maxPhi);
				Rotate(i, turn * Mathf.Min(Mathf.Abs(angToEnd), omega*dt));

				// Translation part
				Translate(i, direction * speeds[i] * dt);
			}
		}


		// Check if all vehicles have reached their goals
		done = AllReached();
		if (done) {
			cost = Time.realtimeSinceStartup - started;
		}
	}

	// Speed of ith car
	protected override float Speed(int i) {
		return speeds[i];
	}

	// Helper function to rotate cars
	private void Rotate(int i, float angle) {
		vehicles[i].transform.Rotate(0, angle, 0, Space.World);
	}

	// Helper function to translate cars
	private void Translate(int i, Vector3 move) {
		vehicles[i].transform.Translate(move, Space.World);
	}

	// Front of ith car
	private Vector3 Front(int i) {
		return Position(i) + Direction(i) * L / 2;
	}

	// Back of ith car
	private Vector3 Back(int i) {
		return Position(i) - Direction(i) * L / 2;
	}

	// Position of ith car
	private Vector3 Position(int i) {
		return vehicles[i].transform.position;
	}

	// Direction of ith car
	private Vector3 Direction(int i) {
		return vehicles[i].transform.forward;
	}


	// Plot some shit
	void OnDrawGizmos() {
		if (goals != null) {
			Gizmos.color = Color.red;
			foreach (var v in goals) {
				Gizmos.DrawSphere(v, 2);
			}
		}

		if (vehicles != null && showDirections) {
			for (int i = 0; i < N; i++) {
				Edge e1 = new Edge(
					Position(i),
					Position(i) + Direction(i) * speeds[i] * timeHorizon
				);
				e1.GizmosDraw(Color.red);
			}
		}
	}
	
}

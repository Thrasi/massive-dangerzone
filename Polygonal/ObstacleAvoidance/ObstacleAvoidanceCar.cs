using UnityEngine;
using System.Collections;

public class ObstacleAvoidanceCar : ObstacleAvoidance {

	// Use this for initialization
	void Start () {
		material = Resources.Load("Materials/ObstacleMaterial") as Material;
		vehicle = Resources.Load("GameObjects/Vehicle") as GameObject;
		vehicle.transform.localScale = new Vector3(2, 2, 2*R);

		// Read map and set some variables
		PolygonMap map = new PolygonMap("Assets/_Data/ColAvoidPolyg/" + filename);
		N = map.N;
		goals = map.goals;
		obstacles = map.GetObstacles();
		GenerateObstacles(obstacles);	
		GenerateVehicles(map.GetVehiclePositions());

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
		print(Time.deltaTime);
	}

	/*
				} else {
				Vector3 prevVel = velocities[i];
				Vector3 newVel = sim.getAgentVelocity(i);
				float omega = prevVel.magnitude / R * Mathf.Tan(maxPhi*toRad) * toDeg;
				
				float rot = Mathf.Min(Vector3.Angle(prevVel, newVel), omega*dt);
				float lr = Mathf.Sign(Vector3.Cross(prevVel, newVel).y);
				Vector3 currVel = Quaternion.Euler(0, lr*rot, 0) * prevVel;
				currVel = newVel;//currVel * newVel.magnitude;
				
				vehicles[i].transform.Rotate(0, lr*rot, 0, Space.World);
				vehicles[i].transform.Translate(currVel*dt, Space.World);
				sim.setAgentVelocity(i, currVel);
				sim.setAgentPosition(i, vehicles[i].transform.position);
			}
*/

			/*
			if (!pointNotCar) {
			
		}
*/
}

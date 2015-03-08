using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ObstacleAvoidanceEmpty : AbstractVehicles {

	public string filename;

	public float R;

	public float maxAcc = 1.0f;

	private int N;
	private Vector3[] goals;
	private Vector3[] velocities;
	private bool[] activeV;

	private float[,] slopes;


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
	}
	int c = 0;
	int F = 5;
	// Update is called once per frame
	void Update () {
		c++;
		for (int i = 0; i < N; i++) {
			if (!activeV[i]) {
				continue;
			}
			Vector3 currPos = vehicles[i].transform.position;
			Vector3 goalPos = goals[i];
			float dist = Vector3.Distance(currPos, goalPos);
			if (dist < 0.01f) {
				activeV[i] = true;
				continue;
			}
			
			Vector3 dir = (goalPos - currPos).normalized;
			Vector3 mov = dir;
			float v = velocities[i].magnitude;
			if (dist < 0.5f * v * v / maxAcc && Vector3.Angle(velocities[i], dir) < 0.1f) {
				mov = -dir;
			}

			if (c >= F) {
				c = 0;
				for (int j = i-1; j >= 0; j--) {
					Vector3 other = vehicles[j].transform.position;
					Line l = new Line(currPos, other);
					float sl = l.Slope();
					if (Mathf.Abs(sl - slopes[i,j]) < 0.01f) {
						mov = -dir;
					}
					slopes[i,j] = sl;
				}
			}
			
			mov = mov.normalized * maxAcc;
			velocities[i] += mov * Time.deltaTime;
			vehicles[i].transform.Translate(velocities[i] * Time.deltaTime, Space.World);
			
		}
	}
}

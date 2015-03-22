using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class DiscreteVehicles : AbstractDiscrete {

	// Name of the file which contains a map
	public string filename;

	// Depth to explore
	public int depth;

	// Path of the solution
	private List<State>[] paths;

	
	// Read the map and run astar
	protected override void LocalStart () {
		DiscreteMap map = new DiscreteMap("Assets/_Data/ColAvoidMaze/" + filename);
		map.ReArrange();
		
		GenerateObstacles(map.GetObstaclePositions());
		GenerateVehicles(map.GetStartPositions());

		SpaceTimeAStar ast = new SpaceTimeAStar(
			map,
			depth,
			delegate(Vector3 a, Vector3 b) {		// Heuristic function, manhattan
				return Mathf.Abs(a.x-b.x) + Mathf.Abs(a.z-b.z);
			}
		);
		paths = ast.paths;
		cost = ast.cost;
	}
	
	// Move all vehicles one step
	void Update () {
		c++;
		if (c >= F) {
			c = 0;
			int len = paths.Length;
			for (int i = 0; i < len; i++) {
				if (step < paths[i].Count) {
					vehicles[i].transform.position = paths[i][step].pos;
				}
			}
			step++;
		}
	}

	// Draws paths
	// Watch out, this function can be very slow for some reason
	// If depth is big then turn off sphere drawing
	void OnDrawGizmos() {
		if (paths != null) {
			for (int i = 0; i < paths.Length; i++) {
				Gizmos.color = vehicles[i].renderer.material.color;
				List<State> currPath = paths[i];
				for (int j = 0; j < cost; j++) {
					//Gizmos.DrawSphere(currPath[j].pos, 0.1f);
					Gizmos.DrawLine(currPath[j].pos, currPath[j+1].pos);
				}
				Gizmos.DrawSphere(currPath[(int) cost].pos, 0.3f);
			}
		}
	}
		
}

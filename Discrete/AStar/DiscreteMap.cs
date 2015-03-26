using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

/*
	Class for modelling discrete maps. Has a map of obstacles and
	start end points for all vehicles.
*/
public class DiscreteMap : AbstractDiscreteMap {

	// Goal coordinats for all vehicles
	public Vector3[] goals { get; protected set; }

	// Costs
	public float[] costs;


	/* 	Reads the file for discrete map, the file should have this format:
		first line -> N -> number of vehicles
		next N lines -> Xs Ys Xe Ye -> start and end coordinates i-th of vehicle
		R C -> number of rows and columns in a map (size of the map)
		next R lines with C times 1/0
	*/
	public DiscreteMap(string filename) : base() {
		StreamReader sr = new StreamReader(filename);
		try {
			// Number of vehicles
			string sN = sr.ReadLine();
			this.N = int.Parse(sN);
			this.starts = new Vector3[N];
			this.goals = new Vector3[N];

			// Read coordinates for all vehicles
			for (int i = 0; i < N; i++) {
				string[] coords = sr.ReadLine().Split(' ');
				starts[i] = new Vector3(int.Parse(coords[0]), 0, int.Parse(coords[1]));
				goals[i] = new Vector3(int.Parse(coords[2]), 0, int.Parse(coords[3]));
			}

			// Size of the map
			string[] sSize = sr.ReadLine().Split(' ');
			this.size = Tuple.Create(int.Parse(sSize[0]), int.Parse(sSize[1]));
			this.map = new bool[size._1, size._2];	// x and y are here inverted
			
			// Read the map
			for (int y = 0; y < size._1; y++) {
				string[] line = sr.ReadLine().Split(' ');
				for (int x = 0; x < size._2; x++) {
					map[y,x] = int.Parse(line[x]) == 0;
				}
			}
		} finally {
			sr.Close();
		}
	}

	// Heuristic manhattan
	private static float h(Vector3 x, Vector3 y) {
		return Mathf.Abs(x.x - y.x) + Mathf.Abs(x.z - y.z);
	}

	// Rearrange so that highest cost path is first
	public void ReArrange() {
		if (costs != null) {		// Already arranged
			return;
		}

		// Run astars on all start goal pairs
		costs = new float[N];
		for (int i = 0; i < N; i++) {
			AStar ast = new AStar(this, starts[i], goals[i], h);
			costs[i] = ast.cost;
			Debug.Log(ast.cost);
		}

		// Copy array, sorting is needed twice
		float[] costsCopy = new float[N];
		Array.Copy(costs, costsCopy, N);

		// Sort arrays
		Array.Sort(costs, starts);
		Array.Sort(costsCopy, goals);

		// Reverse arrays, I am too lazy to create IComparers
		Array.Reverse(costs);
		Array.Reverse(starts);
		Array.Reverse(goals);
	}

	// Returns enumerable of successors and costs to state s
	public IEnumerable<Tuple<State, float>> Successors(State s) {
		List<Tuple<State, float>> succ = new List<Tuple<State, float>>();
		foreach (Vector3 move in neighborhood) {
			Vector3 neighbor = s.pos + move;
			int z = (int) neighbor.z, x = (int) neighbor.x;
			if (z < 0 || x < 0 || z >= size._1 || x >= size._2 || !map[z,x]) {
				continue;
			}

			// Make the pause cost less, change it if it starts to fuck up
			float cost = move.Equals(Vector3.zero) ? 0.5f : 1.0f;
			cost = 1.0f;
			succ.Add(Tuple.Create(new State(neighbor, s.t + 1), cost));
		}
		return succ;
	}

	// Returns adjacent states, but vectors and costs
	public IEnumerable<Tuple<Vector3, float>> Successors(Vector3 v) {
		State s = new State(v, 0);
		return Successors(s).Select(t => Tuple.Create(t._1.pos, 1.0f));
	}

}

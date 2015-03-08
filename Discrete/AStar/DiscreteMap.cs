using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

/*
	Class for modelling discrete maps. Has a map of obstacles and
	start end points for all vehicles.
*/
public class DiscreteMap : AbstractDiscreteMap {

	// Goal coordinats for all vehicles
	public Vector3[] goals { get; protected set; }


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
			succ.Add(Tuple.Create(new State(neighbor, s.t + 1), cost));
		}
		return succ;
	} 

}

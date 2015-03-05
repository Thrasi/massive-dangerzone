using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

/*
	Class for modelling discrete maps. Has a map of obstacles and
	start end points for all vehicles.
*/
public class DiscreteMap : Map {

	// Number of vehicles
	private readonly int N;

	// Size of the map, (y, x) == (rows, columns)
	private readonly Tuple<int, int> size;

	// Start and goal coordinates for all vehicles
	private readonly Vector3[] starts;
	private readonly Vector3[] goals;

	// Map of obstacles, true means free, false means obstacle
	private readonly bool[,] map;

	// Neighborhood to use
	private readonly Vector3[] neighborhood;


	// Constructor that reads the map from file
	public DiscreteMap(string filename) {
		CreateMap(filename, out N, out size, out starts, out goals, out map);
		neighborhood = n4;
	}

	// Returns a list of positions for obstacles
	public List<Vector3> GetObstaclePositions() {
		List<Vector3> positions = new List<Vector3>();
		for (int y = 0; y < size._1; y++) {
			for (int x = 0; x < size._2; x++) {
				if (!map[y,x]) {
					positions.Add(new Vector3(x, 0, y));
				}
			}
		}
		return positions;
	}

	// Returns the list of start positions
	public List<Vector3> GetStartPositions() {
		return new List<Vector3>(starts);
	}

	// Returns enumerable of successors and costs to state s
	public IEnumerable<Tuple<State, float>> Successors(State s) {
		List<Tuple<State, float>> succ = new List<Tuple<State, float>>();
		Successors(s.positions, new List<Vector3>(), succ);
		return succ;
	}

	private void Successors(IEnumerable<Vector3> positions, List<Vector3> currExpand,
		List<Tuple<State, float>> acc) {
		
		if (positions.Count() == 0) {
			acc.Add(Tuple.Create(new State(currExpand), 1.0f));
			return;
		}

		Vector3 curr = positions.First();
		foreach (Vector3 move in neighborhood) {
			Vector3 neighbor = curr + move;
			int z = (int) neighbor.z, x = (int) neighbor.x;
			//Debug.Log(z + " " + x);
			if (z < 0 || x < 0 || z >= size._1 || x >= size._2 || !map[z,x]) {
				continue;
			}
			List<Vector3> newExpand = new List<Vector3>(currExpand);
			newExpand.Add(neighbor);
			Successors(positions.Skip(1), newExpand, acc);
		}
	}

	// Returns true if state s is goal state
	public bool Goal(State s) {
		State goal = new State(goals);
		return goal.Equals(s);
	}

	// Returns initial state of the map
	public State Initial() {
		return new State(starts);
	}

	// Heuristic value from state s to goal state
	public float h(State s) {
		Vector3[] curr = s.positions;
		float maxDist = float.MinValue;
		for (int i = 0; i < curr.Length; i++) {
			float dx = curr[i].x - goals[i].x;
			float dy = curr[i].z - goals[i].z;
			float dist = Mathf.Sqrt(dx * dx + dy * dy);
			if (dist > maxDist) {
				maxDist = dist;
			}
		}
		return maxDist;
	}


	/* 	Reads the file for discrete map, the file should have this format:
		first line -> N -> number of vehicles
		next N lines -> Xs Ys Xe Ye -> start and end coordinates i-th of vehicle
		R C -> number of rows and columns in a map (size of the map)
		next R lines with C times 1/0
	*/
	private static void CreateMap(string filename, out int N, out Tuple<int, int> size,
		out Vector3[] starts, out Vector3[] goals, out bool[,] map) {
		
		StreamReader sr = new StreamReader(filename);
		try {
			// Number of vehicles
			string sN = sr.ReadLine();
			N = int.Parse(sN);
			starts = new Vector3[N];
			goals = new Vector3[N];

			// Read coordinates for all vehicles
			for (int i = 0; i < N; i++) {
				string[] coords = sr.ReadLine().Split(' ');
				starts[i] = new Vector3(int.Parse(coords[0]), 0, int.Parse(coords[1]));
				goals[i] = new Vector3(int.Parse(coords[2]), 0, int.Parse(coords[3]));
			}

			// Size of the map
			string[] sSize = sr.ReadLine().Split(' ');
			size = Tuple.Create(int.Parse(sSize[0]), int.Parse(sSize[1]));
			map = new bool[size._1, size._2];	// x and y are here inverted
			
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


	// Here are all neighborhoods

	private static readonly Vector3[] n4 = new Vector3[] {
		Vector3.zero,
		new Vector3( 1, 0,  0),
		new Vector3( 0, 0,  1),
		new Vector3(-1, 0,  0),
		new Vector3( 0, 0, -1)
	};

	private static readonly Vector3[] n8 = new Vector3[] {
		Vector3.zero,
		new Vector3( 1, 0,  0),
		new Vector3( 0, 0,  1),
		new Vector3(-1, 0,  0),
		new Vector3( 0, 0, -1),
		new Vector3( 1, 0,  1),
		new Vector3(-1, 0,  1),
		new Vector3(-1, 0, -1),
		new Vector3( 1, 0, -1)
	};

	private static readonly Vector3[] n16 = new Vector3[] {
		Vector3.zero,
		new Vector3( 1, 0,  0), new Vector3( 2, 0,  1),
		new Vector3( 0, 0,  1), new Vector3(-2, 0,  1),
		new Vector3(-1, 0,  0), new Vector3( 2, 0, -1),
		new Vector3( 0, 0, -1), new Vector3(-2, 0, -1),
		new Vector3( 1, 0,  1), new Vector3( 1, 0,  2),
		new Vector3(-1, 0,  1), new Vector3( 1, 0, -2),
		new Vector3(-1, 0, -1), new Vector3(-1, 0,  2),
		new Vector3( 1, 0, -1), new Vector3(-1, 0, -2)
	};	
}

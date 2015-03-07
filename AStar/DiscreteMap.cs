using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

/*
	Class for modelling discrete maps. Has a map of obstacles and
	start end points for all vehicles.
*/
public class DiscreteMap {

	// Number of vehicles
	public readonly int N;

	// Size of the map, (y, x) == (rows, columns)
	public readonly Tuple<int, int> size;

	// Start and goal coordinates for all vehicles
	public readonly Vector3[] starts;
	public readonly Vector3[] goals;

	// Map of obstacles, true means free, false means obstacle
	private readonly bool[,] map;

	// Neighborhood to use
	private readonly Vector3[] neighborhood;


	/* 	Reads the file for discrete map, the file should have this format:
		first line -> N -> number of vehicles
		next N lines -> Xs Ys Xe Ye -> start and end coordinates i-th of vehicle
		R C -> number of rows and columns in a map (size of the map)
		next R lines with C times 1/0
	*/
	public DiscreteMap(string filename) {
		this.neighborhood = n4;		// Always use 4-connected		
		
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

	public DiscreteGraph GetGraph() {
		return new DiscreteGraph(map, size);
	}


	// Here are all neighborhoods
	// Only n4 is used at the moment

	private static readonly Vector3[] n4 = new Vector3[] {
		Vector3.zero,
		new Vector3( 1, 0,  0),
		new Vector3( 0, 0,  1),
		new Vector3(-1, 0,  0),
		new Vector3( 0, 0, -1)
	};

	// To suppress variable not used
	#pragma warning disable 0414

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

	#pragma warning restore 0414
}

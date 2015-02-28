using UnityEngine;
using System.Collections.Generic;
using System.IO;

/*
	Class for modelling discrete maps. Has a map of obstacles and
	start end points for all vehicles.
*/
public class DiscreteMap {

	// Number of vehicles
	private int N;

	// Size of the map, (y, x) == (rows, columns)
	private Tuple<int, int> size;

	// Start and goal coordinates for all vehicles
	private Vector3[] starts;
	private Vector3[] goals;

	// Map of obstacles, true means free, false means obstacle
	private bool[,] map;


	// Constructor that reads the map from file
	public DiscreteMap(string filename) {
		createMap(filename);
	}

	/* 	Reads the file for discrete map, the file should have this format:
		first line -> N -> number of vehicles
		next N lines -> Xs Ys Xe Ye -> start and end coordinates i-th of vehicle
		R C -> number of rows and columns in a map (size of the map)
		next R lines with C times 1/0
	*/
	private void createMap(string filename) {
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
				starts[i] = new Vector3(int.Parse(coords[0]), int.Parse(coords[1]));
				goals[i] = new Vector3(int.Parse(coords[2]), int.Parse(coords[3]));
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
}
